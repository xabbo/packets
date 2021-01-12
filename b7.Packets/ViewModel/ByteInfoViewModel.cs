using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace b7.Packets.ViewModel
{
    public class ByteInfoViewModel : ViewModelBase
    {
        private bool _hasBool;
        public bool HasBool
        {
            get => _hasBool;
            set => _set(ref _hasBool, value);
        }

        private bool _boolValue;
        public bool BoolValue
        {
            get => _boolValue;
            set => _set(ref _boolValue, value);
        }

        private bool _hasByte;
        public bool HasByte
        {
            get => _hasByte;
            set => _set(ref _hasByte, value);
        }

        private byte _byteValue;
        public byte ByteValue
        {
            get => _byteValue;
            set => _set(ref _byteValue, value);
        }

        private bool _hasShort;
        public bool HasShort
        {
            get => _hasShort;
            set => _set(ref _hasShort, value);
        }

        private short _shortValue;
        public short ShortValue
        {
            get => _shortValue;
            set => _set(ref _shortValue, value);
        }

        private bool _hasInt;
        public bool HasInt
        {
            get => _hasInt;
            set => _set(ref _hasInt, value);
        }

        private int _intValue;
        public int IntValue
        {
            get => _intValue;
            set => _set(ref _intValue, value);
        }

        private bool _hasLong;
        public bool HasLong
        {
            get => _hasLong;
            set => _set(ref _hasLong, value);
        }

        private long _longValue;
        public long LongValue
        {
            get => _longValue;
            set => _set(ref _longValue, value);
        }

        private bool _hasString;
        public bool HasString
        {
            get => _hasString;
            set => _set(ref _hasString, value);
        }

        private string? _stringValue;
        public string? StringValue
        {
            get => _stringValue;
            set => _set(ref _stringValue, value);
        }
    }
}
