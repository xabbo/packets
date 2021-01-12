using System;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace b7.Packets.ViewModel
{
    public class DataRowViewModel : ViewModelBase
    {
        public CompositeCollection Items { get; }

        public ObservableCollection<ByteViewModel> Bytes { get; }
        public ObservableCollection<ByteSpanViewModel> Spans { get; }

        public DataRowViewModel()
        {
            Bytes = new ObservableCollection<ByteViewModel>();
            Spans = new ObservableCollection<ByteSpanViewModel>();

            Items = new CompositeCollection()
            {
                new CollectionContainer() { Collection = Bytes },
                new CollectionContainer() { Collection = Spans }
            };
        }
    }
}
