using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace b7.Packets.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        private readonly ObservableCollection<PacketLogViewModel> _logs;
        public ICollectionView Logs { get; }

        public MainViewModel()
        {
            _logs = new ObservableCollection<PacketLogViewModel>();
        }

        public async Task InitializeAsync()
        {



        }
    }
}
