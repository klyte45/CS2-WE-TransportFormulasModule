using Colossal.Entities;
using System;
using System.Runtime.CompilerServices;
using Unity.Entities;
using WE_TFM.Components;
using WE_TFM.Components.Shareable;

namespace WE_TFM.Bridge
{
    [Obsolete("Don't reference methods on this class directly. Always use reverse patch to access them, and don't use this mod DLL as hard dependency of your own mod.", true)]
    public static class WE_TFMComponentGetterBridge
    {
        private static World world;
        private static EntityManager EntityManager => (world ??= World.DefaultGameObjectInjectionWorld).EntityManager;

        public static bool TryGetComponent_DirtyVehicle<T>(Entity target, out T component) where T : unmanaged => TryGetComponent<WE_TFM_DirtyVehicle, T>(target, out component);
        public static bool TryGetComponent_PlatformData<T>(Entity target, out T component) where T : unmanaged => TryGetComponent<WE_TFM_PlatformData, T>(target, out component);
        public static bool TryGetComponent_VehicleIncomingDetailData<T>(Entity target, out T component) where T : unmanaged => TryGetComponent<WE_TFM_VehicleIncomingDetailData, T>(target, out component);
        public static bool TryGetComponent_VehicleIncomingOrderData<T>(Entity target, out T component) where T : unmanaged => TryGetComponent<WE_TFM_VehicleIncomingOrderData, T>(target, out component);
        public static bool TryGetComponent_LineStatus<T>(Entity target, out T component) where T : unmanaged => TryGetComponent<WE_TFM_LineStatus, T>(target, out component);
        public static bool TryGetComponent_WaypointDestinationConnectionsToBeUpdated<T>(Entity target, out T component) where T : unmanaged => TryGetComponent<WE_TFM_WaypointDestinationConnectionsToBeUpdated, T>(target, out component);
        public static bool TryGetComponent_PlatformMappingLink<T>(Entity target, out DynamicBuffer<T> buffer) where T : unmanaged, IBufferElementData => TryGetBuffer<WE_TFM_PlatformMappingLink, T>(target, out buffer);
        public static bool TryGetComponent_WaypointDestinationConnections<T>(Entity target, out DynamicBuffer<T> component) where T : unmanaged, IBufferElementData => TryGetBuffer<WE_TFM_WaypointDestinationConnections, T>(target, out component);

        private static unsafe bool TryGetComponent<T, U>(Entity target, out U component) where T : unmanaged, IComponentData where U : unmanaged
        {
            var found = EntityManager.TryGetComponent(target, out T componentOr);
            component = Unsafe.As<T, U>(ref componentOr);
            return found;
        }
        private static bool TryGetBuffer<T, U>(Entity target, out DynamicBuffer<U> component) where T : unmanaged, IBufferElementData where U : unmanaged, IBufferElementData
        {
            var found = EntityManager.TryGetBuffer<T>(target, true, out var componentOr);
            component = componentOr.Reinterpret<U>();
            return found;
        }
    }
}
