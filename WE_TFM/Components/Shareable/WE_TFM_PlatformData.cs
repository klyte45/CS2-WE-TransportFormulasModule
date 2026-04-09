using Game.Prefabs;
using System.Runtime.InteropServices;
using Unity.Entities;

namespace WE_TFM.Components.Shareable
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WE_TFM_PlatformData : IComponentData
    {
        public byte overallNumber;
        public TransportType type;
        public byte transportTypePlatformNumber;
        public byte railsPlatformNumber;
    }
}
