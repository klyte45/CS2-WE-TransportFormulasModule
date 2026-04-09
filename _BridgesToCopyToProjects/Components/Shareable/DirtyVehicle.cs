using System.Runtime.InteropServices;
using Unity.Entities;

namespace [YourDll].Components.Shareable
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DirtyVehicle 
    {
        public Entity oldTarget;
    }
}
