using System;

using GalaSoft.MvvmLight;

using Xabbo.Messages;

namespace b7.Packets.ViewModel
{
    public class PacketLogViewModel : ObservableObject
    {
        public IReadOnlyPacket Packet { get; }

        public DateTime Timestamp { get; }
        public bool? IsOutgoing { get; }
        public string DirectionPointer { get; }
        public short Id { get; }
        public string Name { get; }
        public int Length { get; }

        public bool IsFlashName { get; }
        public bool IsUnityName { get; }

        public PacketLogViewModel(IMessageManager messages, IReadOnlyPacket packet)
        {
            if (messages.TryGetInfoByHeader(packet.Header.Destination.ToDirection(), packet.Header, out MessageInfo? messageInfo))
            {
                Name = messageInfo.Name;
                IsUnityName = string.Equals(Name, messageInfo.UnityName, StringComparison.OrdinalIgnoreCase);
                IsFlashName = string.Equals(Name, messageInfo.FlashName, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                Name = packet.Header.Value.ToString();
            }

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
            // Name = packet.Header.Name ?? packet.Header.Value.ToString();
            Length = packet.Length;
        }
    }
}
