using System;

namespace b7.Packets.ViewModel
{
    public class GridItemViewModel : ViewModelBase
    {
        private int column;
        public int Column
        {
            get => column;
            set => _set(ref column, value);
        }

        private int row;
        public int Row
        {
            get => row;
            set => _set(ref row, value);
        }

        private int rowSpan = 1;
        public int RowSpan
        {
            get => rowSpan;
            set => _set(ref rowSpan, value);
        }

        private int columnSpan = 1;
        public int ColumnSpan
        {
            get => columnSpan;
            set => _set(ref columnSpan, value);
        }
    }
}
