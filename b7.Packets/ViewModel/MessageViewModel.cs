using System;

using GalaSoft.MvvmLight;

using b7.Packets.Common.Messages;

namespace b7.Packets.ViewModel
{
    public class MessageViewModel : ObservableObject
    {
        public Header Header { get; }
        public Destination Destination => Header.Destination;
        public bool IsOutgoing => Header.Destination == Destination.Server;
        public bool IsIncoming => Header.Destination == Destination.Client;
        public short Value => Header.Value;
        public string? Name => Header.Name;

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

        public MessageViewModel(Header header)
        {
            Header = header;
        }
    }
}
