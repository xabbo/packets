using System;
using System.Windows.Media;
using b7.Packets.Util;

namespace b7.Packets.ViewModel
{
    public class ByteViewModel : GridItemViewModel
    {
        private readonly ReadOnlyMemory<byte> _buffer;
        private readonly int _position;

        public byte Value
        {
            get => _buffer.Span[_position];
            set
            {
                throw new NotSupportedException();

                //if (_buffer.Span[_position] == value) return;
                //_buffer.Span[_position] = value;
                //OnPropertyChanged(nameof(Value));
                //OnPropertyChanged(nameof(AsciiValue));
            }
        }

        public char AsciiValue => StringUtil.GetAsciiChar(Value);

        private Brush brush = Brushes.DarkSlateGray;
        public Brush Brush
        {
            get => brush;
            set => _set(ref brush, value);
        }

        public ByteViewModel(ReadOnlyMemory<byte> buffer, int position)
        {
            this._buffer = buffer;
            this._position = position;

            Column = position % 16;
            ColumnSpan = 1;
        }
    }
}