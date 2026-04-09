using Colossal.Entities;
using Game;
using Game.Net;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Game.Simulation;
using Game.Vehicles;
using WE_TFM.Components;
using WE_TFM.Systems;
using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Entities;
using Unity.Mathematics;
using Belzont.Interfaces;
using WE_TFM.Components.Shareable;
using WE_TFM.Enums;

namespace WE_TFM.Formulas
{
    public partial class WE_TFM_IncomingVehicleSystem : BelzontBasicSystem
    {
        public uint CurrentFrame => m_simulationSystem.frameIndex >> 2;
        private SimulationSystem m_simulationSystem;
        private EntityQuery m_dirtyTvInfoVehicles;
        public static WE_TFM_IncomingVehicleSystem Instance { get; private set; }

        protected override AllowedPhase UpdatePhase => AllowedPhase.Modification3;

        public override int GetUpdateInterval(SystemUpdatePhase phase) => 2;

        public WE_TFM_VehicleIncomingDetailData GetIncomingDetailInformation(Entity platform)
        {
            EntityManager.TryGetComponent(platform, out WE_TFM_VehicleIncomingOrderData vehicleData);
            if (vehicleData.nextVehicle0 == Entity.Null)
            {
                return GetDefaultPlatformData(platform);
            }
            var found = EntityManager.TryGetComponent<WE_TFM_VehicleIncomingDetailData>(vehicleData.nextVehicle0, out var data);
            if (!found || !data.IsValid())
            {
                EntityManager.AddComponent<WE_TFM_VehicleIncomingDetailDataDirty>(vehicleData.nextVehicle0);
            }
            return !found ? GetDefaultPlatformData(platform) : data;
        }

        private WE_TFM_VehicleIncomingDetailData GetDefaultPlatformData(Entity platform)
        {
            var hasData = EntityManager.TryGetComponent<WE_TFM_VehicleIncomingDetailData>(platform, out var tvData);
            if (!hasData || !tvData.IsValid())
            {
                EntityManager.TryGetBuffer<ConnectedRoute>(platform, true, out var routes);
                if (routes.Length == 0)
                {
                    tvData = new WE_TFM_VehicleIncomingDetailData(
                    VehicleStatusDescription.ClosedPlatform,
                    VehicleStatusDescription.ClosedPlatform,
                    0,
                    CurrentFrame);
                }
                else
                {
                    var totalAverage = 0;
                    for (var i = 0; i < routes.Length; i++)
                    {
                        if (EntityManager.TryGetComponent(routes[i].m_Waypoint, out WaitingPassengers waitingPassengers))
                        {
                            totalAverage += waitingPassengers.m_AverageWaitingTime;
                        }
                    }
                    tvData = new WE_TFM_VehicleIncomingDetailData(
                    VehicleStatusDescription.NextTrain,
                    VehicleStatusDescription.AverageWaitTime,
                    (ushort)(totalAverage / routes.Length),
                    CurrentFrame);
                }
                if (!hasData)
                {
                    EntityManager.AddComponentData(platform, tvData);
                }
                else
                {
                    EntityManager.SetComponentData(platform, tvData);
                }
            }
            return tvData;
        }

        protected override void OnCreateWithBarrier()
        {
            Instance = this;
            m_simulationSystem = World.GetOrCreateSystemManaged<SimulationSystem>();            
            m_dirtyTvInfoVehicles = GetEntityQuery(new EntityQueryDesc[] {
                new()
                {
                    All = [
                        ComponentType.ReadOnly<WE_TFM_VehicleIncomingDetailDataDirty>(),
                        ComponentType.ReadOnly<PathInformation>(),
                        ComponentType.ReadOnly<PrefabRef>(),
                        ComponentType.ReadOnly<Passenger>(),
                        ComponentType.ReadOnly<PathElement>(),
                        ComponentType.ReadOnly<PathOwner>(),
                    ]
                }
            });
        }

        private readonly Queue<Action> runOnMain = [];

        protected override void OnUpdate()
        {
            while (runOnMain.TryDequeue(out var item))
            {
                item();
            }
            if (!m_dirtyTvInfoVehicles.IsEmpty)
            {
                new VehicleTvDataUpdater
                {
                    entityType = GetEntityTypeHandle(),
                    prefabRefLookup = GetComponentLookup<PrefabRef>(true),
                    pathInformationLookup = GetComponentLookup<PathInformation>(true),
                    pathOwnerLookup = GetComponentLookup<PathOwner>(true),
                    passengerLookup = GetBufferLookup<Passenger>(true),
                    pathElementLookup = GetBufferLookup<PathElement>(true),
                    layoutElementLookup = GetBufferLookup<LayoutElement>(true),
                    cmdBuffer = Barrier.CreateCommandBuffer().AsParallelWriter(),
                    publicTransportVehicleDataLookup = GetComponentLookup<PublicTransportVehicleData>(true),
                    frame = CurrentFrame,
                    tvDataLookup = GetComponentLookup<WE_TFM_VehicleIncomingDetailData>(true),
                    publicTransportLookup = GetComponentLookup<Game.Vehicles.PublicTransport>(true),
                    m_carLaneLookup = GetComponentLookup<CarCurrentLane>(true),
                    m_waterLaneLookup = GetComponentLookup<WatercraftCurrentLane>(true),
                    m_airLaneLookup = GetComponentLookup<AircraftCurrentLane>(true),
                    m_trainLaneLookup = GetComponentLookup<TrainCurrentLane>(true),
                    m_curveLookup = GetComponentLookup<Curve>(true)
                }.ScheduleParallel(m_dirtyTvInfoVehicles, Dependency).Complete();
            }
        }

        [BurstCompile]
        private struct VehicleTvDataUpdater : IJobChunk
        {
            public EntityTypeHandle entityType;
            public ComponentLookup<PrefabRef> prefabRefLookup;
            public ComponentLookup<PathInformation> pathInformationLookup;
            public ComponentLookup<PathOwner> pathOwnerLookup;
            public ComponentLookup<WE_TFM_VehicleIncomingDetailData> tvDataLookup;
            public BufferLookup<Passenger> passengerLookup;
            public BufferLookup<PathElement> pathElementLookup;
            public BufferLookup<LayoutElement> layoutElementLookup;
            public EntityCommandBuffer.ParallelWriter cmdBuffer;
            public ComponentLookup<PublicTransportVehicleData> publicTransportVehicleDataLookup;
            public ComponentLookup<Game.Vehicles.PublicTransport> publicTransportLookup;
            public uint frame;
            public ComponentLookup<CarCurrentLane> m_carLaneLookup;
            public ComponentLookup<WatercraftCurrentLane> m_waterLaneLookup;
            public ComponentLookup<AircraftCurrentLane> m_airLaneLookup;
            public ComponentLookup<TrainCurrentLane> m_trainLaneLookup;
            public ComponentLookup<Curve> m_curveLookup;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var entities = chunk.GetNativeArray(entityType);
                for (int i = 0; i < entities.Length; i++)
                {
                    var entity = entities[i];
                    WE_TFM_VehiclePathWatchSystem.CalculateDistance(
                            ref pathInformationLookup, ref pathElementLookup, ref m_carLaneLookup,
                            ref m_waterLaneLookup, ref m_airLaneLookup, ref m_trainLaneLookup, ref m_curveLookup,
                            entity, out float distance, out _
                            );
                    var result = new WE_TFM_VehicleIncomingDetailData
                    {
                        distanceHm = (ushort)math.clamp(distance / 100, 0, ushort.MaxValue),
                    };
                    switch (result.distanceHm)
                    {
                        case 0:
                            if ((publicTransportLookup[entity].m_State & PublicTransportFlags.Boarding) != 0)
                            {
                                result.title = VehicleStatusDescription.TrainOnPlatform;
                                result.subtitle = VehicleStatusDescription.BoardingNow;
                            }
                            else
                            {
                                result.title = VehicleStatusDescription.TrainOnPlatform;
                                result.subtitle = VehicleStatusDescription.PrepareForBoarding;
                            }
                            break;
                        default:
                            result.title = VehicleStatusDescription.NextTrain;
                            result.subtitle = VehicleStatusDescription.DistanceToStation;
                            break;
                    }
                    if (layoutElementLookup.TryGetBuffer(entity, out var layoutElements))
                    {
                        for (var j = 0; j < layoutElements.Length; j++)
                        {
                            if (j > 7) break;
                            var layoutElement = layoutElements[j];
                            if (publicTransportVehicleDataLookup.TryGetComponent(prefabRefLookup[layoutElement.m_Vehicle].m_Prefab, out var data))
                            {
                                var usage = passengerLookup[layoutElement.m_Vehicle].Length * 255 / data.m_PassengerCapacity;
                                result[j] = (byte)math.clamp(usage, 0, 255);
                            }
                        }
                    }
                    else
                    {
                        result.totalCars = 1;
                        if (publicTransportVehicleDataLookup.TryGetComponent(prefabRefLookup[entity].m_Prefab, out var data))
                        {
                            var usage = passengerLookup[entity].Length * 255 / data.m_PassengerCapacity;
                            result[0] = (byte)math.clamp(usage, 0, 255);
                        }
                    }
                    result.cacheFrame = frame;
                    if (tvDataLookup.HasComponent(entity))
                    {
                        cmdBuffer.SetComponent(unfilteredChunkIndex, entity, result);
                    }
                    else
                    {
                        cmdBuffer.AddComponent(unfilteredChunkIndex, entity, result);
                    }
                    cmdBuffer.RemoveComponent<WE_TFM_VehicleIncomingDetailDataDirty>(unfilteredChunkIndex, entity);
                }
            }
        }
    }




}
