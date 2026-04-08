using Game.Prefabs;
using Unity.Entities;

namespace WE_TFM.Components
{
    public struct WE_TFM_PlatformData : IComponentData
    {
        public byte overallNumber;
        public TransportType type;
        public byte transportTypePlatformNumber;
        public byte railsPlatformNumber;
    }
}
