using System;

using GalaSoft.MvvmLight;

namespace b7.Packets.ViewModel
{
    public class GridItemViewModel : ObservableObject
    {
        private int column;
        public int Column
        {
            get => column;
            set => Set(ref column, value);
        }

        private int row;
        public int Row
        {
            get => row;
            set => Set(ref row, value);
        }

        private int rowSpan = 1;
        public int RowSpan
        {
            get => rowSpan;
            set => Set(ref rowSpan, value);
        }

        private int columnSpan = 1;
        public int ColumnSpan
        {
            get => columnSpan;
            set => Set(ref columnSpan, value);
        }
    }
}
