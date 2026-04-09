using System.Runtime.InteropServices;
using Unity.Entities;

namespace WE_TFM.Components.Shareable
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WE_TFM_WaypointDestinationConnectionsToBeUpdated : IComponentData
    {
        public Entity untilWaypoint;
        public uint requestFrame;
    }
}
