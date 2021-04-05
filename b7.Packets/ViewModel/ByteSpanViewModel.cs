using System;

using GalaSoft.MvvmLight;

namespace b7.Packets.ViewModel
{
    public class ByteSpanViewModel : GridItemViewModel
    {
        public DataRowViewModel DataRows { get; }

        private TypeCode _type;
        public TypeCode Type
        {
            get => _type;
            set => Set(ref _type, value);
        }

        private bool _openLeft;
        public bool OpenLeft
        {
            get => _openLeft;
            set => Set(ref _openLeft, value);
        }

        private bool _openRight;
        public bool OpenRight
        {
            get => _openRight;
            set => Set(ref _openRight, value);
        }

        public ByteSpanViewModel(DataRowViewModel byteRow)
        {
            DataRows = byteRow;
        }
    }
}
