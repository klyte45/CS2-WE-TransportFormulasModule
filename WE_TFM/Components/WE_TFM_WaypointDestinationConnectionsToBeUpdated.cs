using Unity.Entities;

namespace WE_TFM.Components
{
    public struct WE_TFM_WaypointDestinationConnectionsToBeUpdated : IComponentData
    {
        public Entity untilWaypoint;
        public uint requestFrame;
    }
}
