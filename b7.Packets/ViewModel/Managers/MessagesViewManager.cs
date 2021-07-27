using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

using GalaSoft.MvvmLight;

using Xabbo.Messages;
using Xabbo.Interceptor;

using b7.Packets.Services;
using System.Collections.Generic;

namespace b7.Packets.ViewModel
{
    public class MessagesViewManager : ObservableObject
    {
        private readonly IContext _context;
        private readonly IRemoteInterceptor _interceptor;
        private readonly ObservableCollection<MessageViewModel> _messages;

        public ICollectionView Messages { get; }

        private string _filterText = string.Empty;
        public string FilterText
        {
            get => _filterText;
            set
            {
                if (Set(ref _filterText, value))
                    Messages.Refresh();
            }
        }

        public MessagesViewManager(IContext context, IRemoteInterceptor interceptor)
        {
            _context = context;
            _interceptor = interceptor;
            _messages = new ObservableCollection<MessageViewModel>();
            Messages = CollectionViewSource.GetDefaultView(_messages);
            Messages.Filter = FilterMessages;

            _interceptor.Connected += OnConnected;
            _interceptor.Disconnected += OnDisconnected;
        }

        private bool FilterMessages(object obj)
        {
            if (obj is not MessageViewModel message) return false;

            if (string.IsNullOrWhiteSpace(FilterText))
            {
                return true;
            }
            else
            {
                return message.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase);
            }
        }

        private void OnConnected(object? sender, GameConnectedEventArgs e)
        {
            _context.Invoke(() =>
            {
                HashSet<string>
                    incomingNames = new(StringComparer.OrdinalIgnoreCase),
                    outgoingNames = new(StringComparer.OrdinalIgnoreCase);

                foreach (var message in e.Messages)
                {
                    HashSet<string> set = message.Direction == Direction.Outgoing ? outgoingNames : incomingNames;
                    if (set.Add(message.Name))
                    {
                        _messages.Add(new MessageViewModel
                        {
                            Destination = message.Direction.ToDestination(),
                            Name = message.Name,
                            Header = message.Header
                        });
                    }
                }
            });
        }

        private void OnDisconnected(object? sender, EventArgs e)
        {
            _context.Invoke(() => _messages.Clear());
        }
    }
}
