using Belzont.Interfaces;
using Game;
using Game.Common;
using Game.Net;
using Game.Pathfind;
using Game.Routes;
using Game.SceneFlow;
using Game.Vehicles;
using System;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using WE_TFM.Components.Shareable;

namespace WE_TFM.Systems
{

    public partial class WE_TFM_VehiclePathWatchSystem : BelzontBasicSystem
    {
        private EntityQuery m_vehiclesWithNewPathfind;
        private EntityQuery m_stopsWithoutIncomingInfo;
        private static WE_TFM_VehiclePathWatchSystem Instance;

        protected override AllowedPhase UpdatePhase => AllowedPhase.Modification3;

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return 8;
        }

        protected override void OnCreateWithBarrier()
        {

            m_vehiclesWithNewPathfind = GetEntityQuery([
                ComponentType.ReadOnly<WE_TFM_DirtyVehicle>()
            ]);
            m_stopsWithoutIncomingInfo = GetEntityQuery(new EntityQueryDesc[]{
                new()
                {
                    All= [
                         ComponentType.ReadOnly<ConnectedRoute>()
                    ],
                    None = [
                         ComponentType.ReadOnly<WE_TFM_VehicleIncomingOrderData>()
                    ]
                }
            });


            Instance = this;
        }



        protected override void OnUpdate()
        {
            if (GameManager.instance.isGameLoading) return;
            if (!m_stopsWithoutIncomingInfo.IsEmptyIgnoreFilter)
            {
                new EmptyPlatformInfoFillerJob
                {
                    m_entityType = GetEntityTypeHandle(),
                    m_connectedLinesHandle = GetBufferTypeHandle<ConnectedRoute>(true),
                    m_routeVehiclesLookup = GetBufferLookup<RouteVehicle>(true),
                    m_pathInformationLookup = GetComponentLookup<PathInformation>(true),
                    m_connectedLookup = GetComponentLookup<Connected>(true),
                    m_ownerLookup = GetComponentLookup<Owner>(true),
                    m_pathElementLookup = GetBufferLookup<PathElement>(true),
                    m_cmdBuffer = Barrier.CreateCommandBuffer().AsParallelWriter(),
                    m_carLaneLookup = GetComponentLookup<CarCurrentLane>(true),
                    m_waterLaneLookup = GetComponentLookup<WatercraftCurrentLane>(true),
                    m_airLaneLookup = GetComponentLookup<AircraftCurrentLane>(true),
                    m_curveLookup = GetComponentLookup<Curve>(true),
                    m_trainLaneLookup = GetComponentLookup<TrainCurrentLane>(true)
                }.ScheduleParallel(m_stopsWithoutIncomingInfo, Dependency).Complete();
                return;
            }
            if (m_vehiclesWithNewPathfind.IsEmptyIgnoreFilter)
            {
                return;
            }

            new VehiclePathChangedJob
            {
                m_entityType = GetEntityTypeHandle(),
                m_dirtyVehicleType = GetComponentTypeHandle<WE_TFM_DirtyVehicle>(),
                m_routeVehiclesLookup = GetBufferLookup<RouteVehicle>(true),
                m_pathInformationLookup = GetComponentLookup<PathInformation>(true),
                m_connectedLookup = GetComponentLookup<Connected>(true),
                m_connectedLinesLookup = GetBufferLookup<ConnectedRoute>(true),
                m_ownerLookup = GetComponentLookup<Owner>(true),
                m_pathElementLookup = GetBufferLookup<PathElement>(true),
                m_cmdBuffer = Barrier.CreateCommandBuffer().AsParallelWriter(),
                m_carLaneLookup = GetComponentLookup<CarCurrentLane>(true),
                m_waterLaneLookup = GetComponentLookup<WatercraftCurrentLane>(true),
                m_airLaneLookup = GetComponentLookup<AircraftCurrentLane>(true),
                m_trainLaneLookup = GetComponentLookup<TrainCurrentLane>(true),
                m_curveLookup = GetComponentLookup<Curve>(true)
            }.ScheduleParallel(m_vehiclesWithNewPathfind, Dependency).Complete();
        }


        [BurstCompile]
        private struct VehiclePathChangedJob : IJobChunk
        {
            public EntityTypeHandle m_entityType;
            public ComponentTypeHandle<WE_TFM_DirtyVehicle> m_dirtyVehicleType;
            public BufferLookup<RouteVehicle> m_routeVehiclesLookup;
            public ComponentLookup<PathInformation> m_pathInformationLookup;
            public ComponentLookup<Connected> m_connectedLookup;
            public BufferLookup<ConnectedRoute> m_connectedLinesLookup;
            public ComponentLookup<Owner> m_ownerLookup;
            public BufferLookup<PathElement> m_pathElementLookup;
            public EntityCommandBuffer.ParallelWriter m_cmdBuffer;
            public ComponentLookup<CarCurrentLane> m_carLaneLookup;
            public ComponentLookup<WatercraftCurrentLane> m_waterLaneLookup;
            public ComponentLookup<AircraftCurrentLane> m_airLaneLookup;
            public ComponentLookup<TrainCurrentLane> m_trainLaneLookup;
            public ComponentLookup<Curve> m_curveLookup;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var dirtyVehicles = chunk.GetNativeArray(ref m_dirtyVehicleType);
                var entities = chunk.GetNativeArray(m_entityType);
                for (int i = 0; i < dirtyVehicles.Length; i++)
                {
                    var vehicle = dirtyVehicles[i];
                    var entity = entities[i];

                    var prevTarget = m_connectedLookup[vehicle.oldTarget].m_Connected;
                    var nextTarget = GetPlatformEntity(entity, m_pathInformationLookup, m_connectedLookup);

                    if (m_connectedLinesLookup.TryGetBuffer(prevTarget, out var routesPrev)) MapIncomingVehicle(unfilteredChunkIndex, prevTarget, ref routesPrev,
                        ref m_ownerLookup, ref m_routeVehiclesLookup, ref m_pathInformationLookup,
                         ref m_pathElementLookup, ref m_connectedLookup, ref m_cmdBuffer,
                        ref m_carLaneLookup, ref m_waterLaneLookup, ref m_airLaneLookup, ref m_trainLaneLookup, ref m_curveLookup);
                    if (m_connectedLinesLookup.TryGetBuffer(nextTarget, out var routesNext)) MapIncomingVehicle(unfilteredChunkIndex, nextTarget, ref routesNext,
                        ref m_ownerLookup, ref m_routeVehiclesLookup, ref m_pathInformationLookup,
                     ref m_pathElementLookup, ref m_connectedLookup, ref m_cmdBuffer,
                        ref m_carLaneLookup, ref m_waterLaneLookup, ref m_airLaneLookup, ref m_trainLaneLookup, ref m_curveLookup
                        );

                    m_cmdBuffer.RemoveComponent<WE_TFM_DirtyVehicle>(unfilteredChunkIndex, entity);
                }
            }
        }
        [BurstCompile]
        private struct EmptyPlatformInfoFillerJob : IJobChunk
        {
            public EntityTypeHandle m_entityType;
            public BufferTypeHandle<ConnectedRoute> m_connectedLinesHandle;
            public BufferLookup<RouteVehicle> m_routeVehiclesLookup;
            public ComponentLookup<PathInformation> m_pathInformationLookup;
            public ComponentLookup<Connected> m_connectedLookup;
            public ComponentLookup<Owner> m_ownerLookup;
            public BufferLookup<PathElement> m_pathElementLookup;
            public EntityCommandBuffer.ParallelWriter m_cmdBuffer;
            public ComponentLookup<CarCurrentLane> m_carLaneLookup;
            public ComponentLookup<WatercraftCurrentLane> m_waterLaneLookup;
            public ComponentLookup<AircraftCurrentLane> m_airLaneLookup;
            public ComponentLookup<TrainCurrentLane> m_trainLaneLookup;
            public ComponentLookup<Curve> m_curveLookup;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var entities = chunk.GetNativeArray(m_entityType);
                var platforms = chunk.GetBufferAccessor(ref m_connectedLinesHandle);
                for (int i = 0; i < entities.Length; i++)
                {
                    var entity = entities[i];
                    var connectedRoutes = platforms[i];

                    MapIncomingVehicle(unfilteredChunkIndex, entity, ref connectedRoutes,
                        ref m_ownerLookup, ref m_routeVehiclesLookup, ref m_pathInformationLookup,
                        ref m_pathElementLookup, ref m_connectedLookup, ref m_cmdBuffer,
                        ref m_carLaneLookup, ref m_waterLaneLookup, ref m_airLaneLookup, ref m_trainLaneLookup, ref m_curveLookup);
                }
            }
        }

        private static void MapIncomingVehicle(
              int unfilteredChunkIndex,
              Entity platform,
          ref DynamicBuffer<ConnectedRoute> routesOnPlatform,
          ref ComponentLookup<Owner> m_ownerLookup,
          ref BufferLookup<RouteVehicle> m_routeVehiclesLookup,
          ref ComponentLookup<PathInformation> m_pathInformationLookup,
          ref BufferLookup<PathElement> m_pathElementLookup,
          ref ComponentLookup<Connected> m_connectedLookup,
          ref EntityCommandBuffer.ParallelWriter m_cmdBuffer,
          ref ComponentLookup<CarCurrentLane> m_carLaneLookup,
          ref ComponentLookup<WatercraftCurrentLane> m_waterLaneLookup,
          ref ComponentLookup<AircraftCurrentLane> m_airLaneLookup,
          ref ComponentLookup<TrainCurrentLane> m_trainLaneLookup,
          ref ComponentLookup<Curve> m_curveLookup


            )
        {
            var results = new NativeArray<IncomingVehicle>(4, Allocator.Temp);
            for (int j = 0; j < routesOnPlatform.Length; j++)
            {
                var owner = m_ownerLookup[routesOnPlatform[j].m_Waypoint];
                var vehicles = m_routeVehiclesLookup[owner.m_Owner];
                for (int k = 0; k < vehicles.Length; k++)
                {
                    var destination = GetPlatformEntity(vehicles[k].m_Vehicle, m_pathInformationLookup, m_connectedLookup);
                    if (destination == platform)
                    {
                        var vehicle = vehicles[k].m_Vehicle;
                        CalculateDistance(
                            ref m_pathInformationLookup, ref m_pathElementLookup, ref m_carLaneLookup,
                            ref m_waterLaneLookup, ref m_airLaneLookup, ref m_trainLaneLookup, ref m_curveLookup,
                            vehicle, out float distance, out bool foundLane);
                        if (!foundLane)
                        {
                            continue;
                        }

                        var data = new IncomingVehicle
                        {
                            m_Vehicle = vehicle,
                            distance = distance
                        };
                        for (int l = 0; l < results.Length; l++)
                        {
                            if (results[l].m_Vehicle == Entity.Null || results[l].distance > data.distance)
                            {
                                (data, results[l]) = (results[l], data);
                            }
                            if (data.m_Vehicle == Entity.Null) break;
                        }
                    }
                }
            }
            m_cmdBuffer.AddComponent(unfilteredChunkIndex, platform, new WE_TFM_VehicleIncomingOrderData
            {
                nextVehicle0 = results[0].m_Vehicle,
                nextVehicle1 = results[1].m_Vehicle,
                nextVehicle2 = results[2].m_Vehicle,
                nextVehicle3 = results[3].m_Vehicle
            });
            results.Dispose();
        }

        internal static void CalculateDistance(ref ComponentLookup<PathInformation> m_pathInformationLookup, ref BufferLookup<PathElement> m_pathElementLookup,
            ref ComponentLookup<CarCurrentLane> m_carLaneLookup, ref ComponentLookup<WatercraftCurrentLane> m_waterLaneLookup,
            ref ComponentLookup<AircraftCurrentLane> m_airLaneLookup, ref ComponentLookup<TrainCurrentLane> m_trainLaneLookup,
            ref ComponentLookup<Curve> m_curveLookup, Entity vehicle, out float distance, out bool foundLane)
        {
            distance = 0f;
            foundLane = false;
            var pathElements = m_pathElementLookup[vehicle];
            if (pathElements.Length == 0) return;
            var pathInfo = m_pathInformationLookup[vehicle];

            Entity currentLane;

            if (m_carLaneLookup.TryGetComponent(vehicle, out var carLane))
            {
                currentLane = carLane.m_Lane;
            }
            else if (m_waterLaneLookup.TryGetComponent(vehicle, out var waterLane))
            {
                currentLane = waterLane.m_Lane;
            }
            else if (m_airLaneLookup.TryGetComponent(vehicle, out var airLane))
            {
                currentLane = airLane.m_Lane;
            }
            else if (m_trainLaneLookup.TryGetComponent(vehicle, out var trainCurrentLane))
            {
                currentLane = trainCurrentLane.m_Front.m_Lane;
            }
            else
            {
                return;
            }

            for (int l = 0; l < pathElements.Length; l++)
            {
                if (!foundLane)
                {
                    foundLane = currentLane == pathElements[l].m_Target;
                }
                if (foundLane)
                {
                    distance += m_curveLookup[pathElements[l].m_Target].m_Length * Math.Abs(pathElements[l].m_TargetDelta[1] - pathElements[l].m_TargetDelta[0]);
                }
            }
        }

        private static Entity GetPlatformEntity(
            Entity entity,
            ComponentLookup<PathInformation> m_pathInformationLookup,
            ComponentLookup<Connected> m_connectedLookup)
        {
            return m_connectedLookup[m_pathInformationLookup[entity].m_Destination].m_Connected;
        }

        private struct IncomingVehicle
        {
            public Entity m_Vehicle;
            public float distance;
        }
    }
}