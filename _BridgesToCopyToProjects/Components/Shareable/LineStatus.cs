using Game.Prefabs;
using Game.Routes;
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

        public enum LineOperationStatus
        {
            NormalOperation,
            NotOperating,
            OperationStopped,
            ReducedSpeed,
            NoUsage,
        }
    }

}