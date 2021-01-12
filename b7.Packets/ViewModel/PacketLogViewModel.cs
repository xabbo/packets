using System;

using b7.Packets.Common.Messages;
using b7.Packets.Common.Protocol;

namespace b7.Packets.ViewModel
{
    public class PacketLogViewModel : ViewModelBase
    {
        public IReadOnlyPacket Packet { get; }

        public DateTime Timestamp { get; }
        public bool? IsOutgoing { get; }
        public string DirectionPointer { get; }
        public short Id { get; }
        public string Name { get; }
        public int Length { get; }

        public PacketLogViewModel(IReadOnlyPacket packet)
        {
            Packet = packet;

            Timestamp = DateTime.Now;
            DirectionPointer = packet.Header.Destination switch
            {
                Destination.Server => ">>",
                Destination.Client => "<<",
                _ => "||"
            };

            if (packet.Header.Destination == Destination.Server)
                IsOutgoing = true;
            else if (packet.Header.Destination == Destination.Client)
                IsOutgoing = false;

            Id = packet.Header;
            Name = packet.Header.Name ?? packet.Header.Value.ToString();
            Length = packet.Length;
        }
    }
}
