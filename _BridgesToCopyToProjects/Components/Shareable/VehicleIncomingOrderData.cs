using System.Runtime.InteropServices;
using Unity.Entities;

namespace [YourDll].Components.Shareable
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VehicleIncomingOrderData
    {
        public Entity nextVehicle0;
        public Entity nextVehicle1;
        public Entity nextVehicle2;
        public Entity nextVehicle3;
    }

}
