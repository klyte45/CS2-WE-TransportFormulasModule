using System;
using System.Runtime.CompilerServices;
using Unity.Entities;
using WE_TFM.Components.Shareable;
using WE_TFM.Formulas;

namespace WE_TFM.BultinFn
{
    public static class WE_TFM_IncomingVehicleFn
    {

        public unsafe static T GetIncomingDetailInformation<T>(Entity platform) where T : unmanaged
        {
            var result = WE_TFM_IncomingVehicleSystem.Instance.GetIncomingDetailInformation(platform);
            return Unsafe.As<WE_TFM_VehicleIncomingDetailData, T>(ref result);
        }
    }
}
