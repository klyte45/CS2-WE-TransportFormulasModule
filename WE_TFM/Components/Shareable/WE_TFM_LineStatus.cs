using Game.Prefabs;
using Game.Routes;
using System.Runtime.InteropServices;
using Unity.Entities;
using WE_TFM.Enums;

namespace WE_TFM.Components
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WE_TFM_LineStatus : IComponentData
    {
        public TransportType type;
        public LineOperationStatus lineOperationStatus;
        public int expectedInterval;
        public int actualInterval;
        public bool isPassenger;
        public bool isCargo;
    }

}