using System;
using System.Runtime.CompilerServices;
using Unity.Entities;
using WE_TFM.Components.Shareable;
using WE_TFM.Formulas;

namespace WE_TFM.Bridge
{
    [Obsolete("Don't reference methods on this class directly. Always use reverse patch to access them, and don't use this mod DLL as hard dependency of your own mod.", true)]
    public static class WE_TFMIncomingVehicleBridge
    {

        public unsafe static T GetIncomingDetailInformation<T>(Entity platform) where T : unmanaged
        {
            var result = WE_TFM_IncomingVehicleSystem.Instance.GetIncomingDetailInformation(platform);
            return Unsafe.As<WE_TFM_VehicleIncomingDetailData, T>(ref result);
        }
    }
}
