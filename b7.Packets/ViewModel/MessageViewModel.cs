using System;

using GalaSoft.MvvmLight;

using Xabbo.Messages;

namespace b7.Packets.ViewModel
{
    public class MessageViewModel : ObservableObject
    {
        public Destination Destination { get; init; }
        public bool IsOutgoing => Destination == Destination.Server;
        public bool IsIncoming => Destination == Destination.Client;
        public Direction Direction => Destination.ToDirection();
        public short Header { get; init; }
        public string Name { get; init; } = string.Empty;

        private bool _isHidden;
        public bool IsHidden
        {
            get => _isHidden;
            set => Set(ref _isHidden, value);
        }

        private bool _isBlocked;
        public bool IsBlocked
        {
            get => _isBlocked;
            set => Set(ref _isBlocked, value);
        }

        public MessageViewModel() { }
    }
}
