using System.Runtime.InteropServices;
using Unity.Entities;

namespace WE_TFM.Components.Shareable
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WE_TFM_DirtyVehicle : IComponentData
    {
        public Entity oldTarget;
    }
}
