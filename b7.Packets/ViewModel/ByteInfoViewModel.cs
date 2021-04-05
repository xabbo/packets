using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

namespace b7.Packets.ViewModel
{
    public class ByteInfoViewModel : ObservableObject
    {
        private bool _hasBool;
        public bool HasBool
        {
            get => _hasBool;
            set => Set(ref _hasBool, value);
        }

        private bool _boolValue;
        public bool BoolValue
        {
            get => _boolValue;
            set => Set(ref _boolValue, value);
        }

        private bool _hasByte;
        public bool HasByte
        {
            get => _hasByte;
            set => Set(ref _hasByte, value);
        }

        private byte _byteValue;
        public byte ByteValue
        {
            get => _byteValue;
            set => Set(ref _byteValue, value);
        }

        private bool _hasShort;
        public bool HasShort
        {
            get => _hasShort;
            set => Set(ref _hasShort, value);
        }

        private short _shortValue;
        public short ShortValue
        {
            get => _shortValue;
            set => Set(ref _shortValue, value);
        }

        private bool _hasInt;
        public bool HasInt
        {
            get => _hasInt;
            set => Set(ref _hasInt, value);
        }

        private int _intValue;
        public int IntValue
        {
            get => _intValue;
            set => Set(ref _intValue, value);
        }

        private bool _hasLong;
        public bool HasLong
        {
            get => _hasLong;
            set => Set(ref _hasLong, value);
        }

        private long _longValue;
        public long LongValue
        {
            get => _longValue;
            set => Set(ref _longValue, value);
        }

        private bool _hasString;
        public bool HasString
        {
            get => _hasString;
            set => Set(ref _hasString, value);
        }

        private string? _stringValue;
        public string? StringValue
        {
            get => _stringValue;
            set => Set(ref _stringValue, value);
        }
    }
}
