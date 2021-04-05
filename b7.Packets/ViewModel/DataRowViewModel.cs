using System;
using System.Collections.ObjectModel;
using System.Windows.Data;

using GalaSoft.MvvmLight;

namespace b7.Packets.ViewModel
{
    public class DataRowViewModel : ObservableObject
    {
        private int _offset;
        public int Offset
        {
            get => _offset;
            set => Set(ref _offset, value);
        }

        public bool ShowOffset { get; set; } = true;

        public CompositeCollection Items { get; }
        public ObservableCollection<ByteViewModel> Bytes { get; }
        public ObservableCollection<ByteSpanViewModel> Spans { get; }

        public DataRowViewModel(int offset)
        {
            Offset = offset;

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
