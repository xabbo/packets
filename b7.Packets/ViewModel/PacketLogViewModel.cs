using System;

using GalaSoft.MvvmLight;

using Xabbo.Messages;

namespace b7.Packets.ViewModel
{
    public class PacketLogViewModel : ObservableObject
    {
        public IReadOnlyPacket Packet { get; init; }

        public DateTime Timestamp { get; init; }
        public bool? IsOutgoing { get; init; }
        public string DirectionPointer { get; init; }
        public short Id { get; init; }
        public string Name { get; init; }
        public int Length { get; init; }

        public bool IsFlashName { get; init; }
        public bool IsUnityName { get; init; }

        private bool _isBlocked;
        public bool IsBlocked
        {
            get => _isBlocked;
            set => Set(ref _isBlocked, value);
        }

        public PacketLogViewModel() { }
    }
}
