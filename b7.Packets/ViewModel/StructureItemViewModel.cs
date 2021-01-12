using System.Windows.Media;

namespace b7.Packets.ViewModel
{
    public class StructureItemViewModel : ViewModelBase
    {
        private StructureTypes _structureType;
        public StructureTypes StructureType
        {
            get => _structureType;
            set => _set(ref _structureType, value);
        }

        private int _offset;
        public int Offset
        {
            get => _offset;
            set => _set(ref _offset, value);
        }

        private int _length;
        public int Length
        {
            get => _length;
            set => _set(ref _length, value);
        }

        private string? _valueString;
        public string? ValueString
        {
            get => _valueString;
            set => _set(ref _valueString, value);
        }

        private Brush? _foreground;
        public Brush? Foreground
        {
            get => _foreground;
            set => _set(ref _foreground, value);
        }

        private Brush? _background;
        public Brush? Background
        {
            get => _background;
            set => _set(ref _background, value);
        }

        public StructureItemViewModel()
        {

        }
    }
}