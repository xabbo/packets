using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using GalaSoft.MvvmLight;

using b7.Packets.Services;

namespace b7.Packets.ViewModel
{
    public class MessagesViewManager : ObservableObject
    {
        private readonly IMessageManager _messageManager;

        private readonly ObservableCollection<MessageViewModel> _messages;
        public ICollectionView Messages { get; }

        public MessagesViewManager(IMessageManager messageManager)
        {
            _messageManager = messageManager;

            _messages = new ObservableCollection<MessageViewModel>();
            Messages = CollectionViewSource.GetDefaultView(_messages);
        }

        public void Initialize()
        {

        }
    }
}
