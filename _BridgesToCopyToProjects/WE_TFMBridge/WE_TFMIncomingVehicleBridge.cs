using [YourDll].Components.Shareable;
using System;
using Unity.Entities;

namespace [YourDll].WE_TFMBridge
{
    public static class WE_TFMIncomingVehicleBridge
    {
        [PatchGenericMethod(typeof(VehicleIncomingDetailData))]
        public static VehicleIncomingDetailData GetIncomingDetailInformation(Entity platform) => throw new NotImplementedException("Stub only!");
    }
}