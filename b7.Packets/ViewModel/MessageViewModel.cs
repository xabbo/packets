using System;

using Xabbo.Core.Messages;

namespace b7.Packets.ViewModel
{
    public class MessageViewModel : ViewModelBase
    {
        public Header Header { get; }
        public Destination Destination => Header.Destination;
        public bool IsOutgoing => Header.IsOutgoing;
        public bool IsIncoming => Header.IsIncoming;
        public short Value => Header.Value;
        public string Name => Header.Name;

        private bool _isHidden;
        public bool IsHidden
        {
            get => _isHidden;
            set => _set(ref _isHidden, value);
        }

        private bool _isBlocked;
        public bool IsBlocked
        {
            get => _isBlocked;
            set => _set(ref _isBlocked, value);
        }

        public MessageViewModel(Header header)
        {
            Header = header;
        }
    }
}
