using Game.Prefabs;
using Game.Routes;
using Unity.Entities;

namespace WE_TFM.Components
{
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