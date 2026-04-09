using Game.Prefabs;
using Game.Routes;
using [YourDll].Enums;
using System.Runtime.InteropServices;
using Unity.Entities;

namespace [YourDll].Components.Shareable
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LineStatus 
    {
        public TransportType type;
        public LineOperationStatus lineOperationStatus;
        public int expectedInterval;
        public int actualInterval;
        public bool isPassenger;
        public bool isCargo;
    }

}