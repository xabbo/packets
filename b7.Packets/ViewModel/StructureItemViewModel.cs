using System;

using GalaSoft.MvvmLight;

namespace b7.Packets.ViewModel
{
    public class StructureItemViewModel : ObservableObject
    {
        private TypeCode _type;
        public TypeCode Type
        {
            get => _type;
            set
            {
                if (Set(ref _type, value))
                    RaisePropertyChanged(nameof(TypeName));
            }
        }

        public string TypeName => Type switch
        {
            TypeCode.Boolean => "bool",
            TypeCode.Byte => "byte",
            TypeCode.Int16 => "short",
            TypeCode.Int32 => "int",
            TypeCode.Single => "float",
            TypeCode.Int64 => "long",
            TypeCode.String => "string",
            _ => "?"
        };

        private int _offset;
        public int Offset
        {
            get => _offset;
            set => Set(ref _offset, value);
        }

        private int _length;
        public int Length
        {
            get => _length;
            set => Set(ref _length, value);
        }

        private string? _valueString;
        public string? ValueString
        {
            get => _valueString;
            set => Set(ref _valueString, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public StructureItemViewModel()
        {
            _valueString =
            _name = string.Empty;
        }
    }
}