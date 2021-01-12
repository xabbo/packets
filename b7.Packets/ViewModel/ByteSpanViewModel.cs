using System;

namespace b7.Packets.ViewModel
{
    public class ByteSpanViewModel : GridItemViewModel
    {
        public DataRowViewModel DataRows { get; }

        private StructureTypes _structureType;
        public StructureTypes StructureType
        {
            get => _structureType;
            set => _set(ref _structureType, value);
        }

        private bool _openLeft;
        public bool OpenLeft
        {
            get => _openLeft;
            set => _set(ref _openLeft, value);
        }

        private bool _openRight;
        public bool OpenRight
        {
            get => _openRight;
            set => _set(ref _openRight, value);
        }

        public ByteSpanViewModel(DataRowViewModel byteRow)
        {
            DataRows = byteRow;
        }
    }
}
