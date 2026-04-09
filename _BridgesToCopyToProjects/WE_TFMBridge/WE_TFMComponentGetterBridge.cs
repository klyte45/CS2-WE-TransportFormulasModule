using [YourDll].Components.Shareable;
using System;
using Unity.Entities;
using [YourDll].Components;

namespace [YourDll].WE_TFMBridge
{
    public static class WE_TFMComponentGetterBridge
    {

        [PatchGenericMethod("TryGetComponent_DirtyVehicle", typeof(DirtyVehicle))] public static bool TryGetComponent(Entity target, out DirtyVehicle component) => throw new NotImplementedException("Stub only!");
        [PatchGenericMethod("TryGetComponent_PlatformData", typeof(PlatformData))] public static bool TryGetComponent(Entity target, out PlatformData component) => throw new NotImplementedException("Stub only!");
        [PatchGenericMethod("TryGetComponent_VehicleIncomingDetailData", typeof(LineStatus))] public static bool TryGetComponent(Entity target, out LineStatus component) => throw new NotImplementedException("Stub only!");
        [PatchGenericMethod("TryGetComponent_VehicleIncomingOrderData", typeof(VehicleIncomingDetailData))] public static bool TryGetComponent(Entity target, out VehicleIncomingDetailData component) => throw new NotImplementedException("Stub only!");
        [PatchGenericMethod("TryGetComponent_LineStatus", typeof(VehicleIncomingOrderData))] public static bool TryGetComponent(Entity target, out VehicleIncomingOrderData component) => throw new NotImplementedException("Stub only!");
        [PatchGenericMethod("TryGetComponent_WaypointDestinationConnectionsToBeUpdated", typeof(WaypointDestinationConnectionsToBeUpdated))] public static bool TryGetComponent(Entity target, out WaypointDestinationConnectionsToBeUpdated component) => throw new NotImplementedException("Stub only!");
        [PatchGenericMethod("TryGetComponent_PlatformMappingLink", typeof(PlatformMappingLink))] public static bool TryGetBuffer(Entity target, out DynamicBuffer<PlatformMappingLink> buffer) => throw new NotImplementedException("Stub only!");
        [PatchGenericMethod("TryGetComponent_WaypointDestinationConnections", typeof(WaypointDestinationConnections))] public static bool TryGetBuffer(Entity target, out DynamicBuffer<WaypointDestinationConnections> component) => throw new NotImplementedException("Stub only!");


    }
}
