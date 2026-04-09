using Game.Prefabs;
using Unity.Entities;

namespace [YourDll].Components.Shareable
{
    public struct PlatformData 
    {
        public byte overallNumber;
        public TransportType type;
        public byte transportTypePlatformNumber;
        public byte railsPlatformNumber;
    }
}
