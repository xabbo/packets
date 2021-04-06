using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using GalaSoft.MvvmLight;

using Xabbo.Interceptor;

namespace b7.Packets.ViewModel
{
    public class MessagesViewManager : ObservableObject
    {
        private readonly IRemoteInterceptor _interceptor;
        private readonly ObservableCollection<MessageViewModel> _messages;
        public ICollectionView Messages { get; }

        public MessagesViewManager(IRemoteInterceptor interceptor)
        {
            _interceptor = interceptor;
            _messages = new ObservableCollection<MessageViewModel>();
            Messages = CollectionViewSource.GetDefaultView(_messages);
        }
    }
}
