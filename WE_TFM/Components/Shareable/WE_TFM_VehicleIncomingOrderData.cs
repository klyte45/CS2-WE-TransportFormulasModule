using Colossal.Serialization.Entities;
using System.Runtime.InteropServices;
using Unity.Entities;

namespace WE_TFM.Components.Shareable
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WE_TFM_VehicleIncomingOrderData : IComponentData, ISerializable
    {
        private const uint CURRENT_VERSION = 0;
        public Entity nextVehicle0;
        public Entity nextVehicle1;
        public Entity nextVehicle2;
        public Entity nextVehicle3;

        public readonly void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
        {
            writer.Write(CURRENT_VERSION);
            writer.Write(nextVehicle0);
            writer.Write(nextVehicle1);
            writer.Write(nextVehicle2);
            writer.Write(nextVehicle3);
        }
        public void Deserialize<TReader>(TReader reader) where TReader : IReader
        {
            reader.Read(out uint version);
            if (version > CURRENT_VERSION)
            {
                throw new System.Exception($"Unsupported version {version} for {nameof(WE_TFM_VehicleIncomingOrderData)}. Current version is {CURRENT_VERSION}.");
            }
            reader.Read(out nextVehicle0);
            reader.Read(out nextVehicle1);
            reader.Read(out nextVehicle2);
            reader.Read(out nextVehicle3);
        }
    }

}
