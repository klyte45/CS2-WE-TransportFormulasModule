using Colossal.Serialization.Entities;
using Game.Prefabs;
using [YourDll].Enums;
using System;
using System.Runtime.InteropServices;
using Unity.Entities;

namespace [YourDll].Components.Shareable
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WaypointDestinationConnections : IBufferElementData,  IEquatable<WaypointDestinationConnections>
    {
        public Entity line;
        public uint requestFrame;
        private TransportType transportType;
        public bool isCargo;
        public bool isPassenger;
        public TransportTypeByImportance Importance { get;  set; }
      

        public override bool Equals(object obj) => obj is WaypointDestinationConnections connections && Equals(connections);

        public bool Equals(WaypointDestinationConnections other) => line.Equals(other.line);

        public override readonly int GetHashCode() => HashCode.Combine(line);

        public static bool operator ==(WaypointDestinationConnections left, WaypointDestinationConnections right) => left.Equals(right);

        public static bool operator !=(WaypointDestinationConnections left, WaypointDestinationConnections right) => !(left == right);
        public enum TransportTypeByImportance : byte
        {
            MostPrioritary = 0x00,
            Airplane = 0x80,
            Ship = 0x90,
            Ferry = 0x97,
            Train = 0xA0,
            Subway = 0xA8,
            Tram = 0xC0,
            Bus = 0xF0,
            LessPrioritary = 0xFF
        }
    }

    
}
