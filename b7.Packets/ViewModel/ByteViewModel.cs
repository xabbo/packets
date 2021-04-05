using System;
using System.Windows.Media;
using b7.Packets.Util;

namespace b7.Packets.ViewModel
{
    public class ByteViewModel : GridItemViewModel
    {
        /*private readonly ReadOnlyMemory<byte> _buffer;
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
            set => Set(ref brush, value);
        }

        public ByteViewModel(ReadOnlyMemory<byte> buffer, int position)
        {
            _buffer = buffer;
            _position = position;

            Column = position % 16;
            ColumnSpan = 1;
        }*/

        public Brush Brush { get; set; } = Brushes.DarkSlateGray;

        public byte Value { get; }
        public string HexValue { get; }
        public char AsciiValue { get; }

        public ByteViewModel(int offset)
        {
            Value = (byte)offset;
            HexValue = offset.ToString("x");
            AsciiValue = HexValue[0];
            Column = offset % 16;
        }

        public ByteViewModel(ReadOnlyMemory<byte> buffer, int offset)
        {
            Value = buffer.Span[offset];
            HexValue = Value.ToString("x2");
            AsciiValue = StringUtil.GetAsciiChar(Value);
            Column = offset % 16;
        }
    }
}