using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Media;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using Xabbo.Messages;

using b7.Packets.Services;

namespace b7.Packets.ViewModel
{
    public class StructureViewManager : ObservableObject
    {
        private readonly IContext _context;

        private IReadOnlyPacket? _packet;
        private ReadOnlyMemory<byte> _data;

        private Dictionary<int, List<ByteSpanViewModel>> _spanMap;
        private readonly ObservableCollection<DataRowViewModel> _dataRows;

        public CompositeCollection DataRows { get; set; }
        public ObservableCollection<StructureItemViewModel> StructureItems { get; set; }

        private bool _canAddBool;
        public bool CanAddBool
        {
            get => _canAddBool;
            set => Set(ref _canAddBool, value);
        }

        private bool _canAddByte;
        public bool CanAddByte
        {
            get => _canAddByte;
            set => Set(ref _canAddByte, value);
        }

        private bool _canAddShort;
        public bool CanAddShort
        {
            get => _canAddShort;
            set => Set(ref _canAddShort, value);
        }

        private bool _canAddInt;
        public bool CanAddInt
        {
            get => _canAddInt;
            set => Set(ref _canAddInt, value);
        }

        private bool _canAddLong;
        public bool CanAddLong
        {
            get => _canAddLong;
            set => Set(ref _canAddLong, value);
        }

        private bool _canAddFloat;
        public bool CanAddFloat
        {
            get => _canAddFloat;
            set => Set(ref _canAddFloat, value);
        }

        private bool _canAddString;
        public bool CanAddString
        {
            get => _canAddString;
            set => Set(ref _canAddString, value);
        }

        private bool _canUndo;
        public bool CanUndo
        {
            get => _canUndo;
            set => Set(ref _canUndo, value);
        }

        private bool _canClear;
        public bool CanClear
        {
            get => _canClear;
            set => Set(ref _canClear, value);
        }

        public ICommand AddBoolCommand { get; }
        public ICommand AddByteCommand { get; }
        public ICommand AddShortCommand { get; }
        public ICommand AddIntCommand { get; }
        public ICommand AddLongCommand { get; }
        public ICommand AddFloatCommand { get; }
        public ICommand AddStringCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand ClearCommand { get; }

        public StructureViewManager(IContext context)
        {
            _context = context;
            _spanMap = new Dictionary<int, List<ByteSpanViewModel>>();

            _data = Array.Empty<byte>();
            _dataRows = new ObservableCollection<DataRowViewModel>();

            DataRowViewModel byteOffsets = new DataRowViewModel(0) { ShowOffset = false };
            for (int i = 0; i < 16; i++)
                byteOffsets.Bytes.Add(new ByteViewModel(i) { Brush = Brushes.LightSlateGray });

            DataRows = new CompositeCollection
            {
                new CollectionContainer() { Collection = new[] { byteOffsets } },
                new CollectionContainer() { Collection = _dataRows }
            };

            StructureItems = new ObservableCollection<StructureItemViewModel>();

            AddBoolCommand = new RelayCommand(AddBool);
            AddByteCommand = new RelayCommand(AddByte);
            AddShortCommand = new RelayCommand(AddShort);
            AddIntCommand = new RelayCommand(AddInt);
            AddLongCommand = new RelayCommand(AddLong);
            AddFloatCommand = new RelayCommand(AddFloat);
            AddStringCommand = new RelayCommand(AddString);
            UndoCommand = new RelayCommand(Undo);
            ClearCommand = new RelayCommand(Clear);

            Messenger.Default.Register<GenericMessage<PacketLogViewModel>>(this, x => LoadPacket(x.Content));
        }

        private void Undo()
        {
            if (_packet == null) return;

            StructureItemViewModel? structureItem = StructureItems.LastOrDefault();
            if (structureItem == null) return;

            _packet.Position -= structureItem.Length;

            StructureItems.Remove(structureItem);

            if (_spanMap.TryGetValue(structureItem.Offset, out List<ByteSpanViewModel>? byteSpans))
            {
                foreach (ByteSpanViewModel byteSpan in byteSpans)
                {
                    byteSpan.DataRows.Spans.Remove(byteSpan);
                }
                _spanMap.Remove(structureItem.Offset);    
            }

            Update();
        }

        private void Clear()
        {
            if (_packet != null)
                _packet.Position = 0;

            StructureItems.Clear();
            foreach (var row in _dataRows)
                row.Spans.Clear();

            Update();
        }

        public void LoadPacket(PacketLogViewModel packetLog)
        {
            _packet = packetLog.Packet;
            _packet.Position = 0;
            _data = _packet.GetBuffer();

            _spanMap.Clear();
            _dataRows.Clear();
            StructureItems.Clear();

            DataRowViewModel? currentRow = null;
            for (int i = 0; i < _data.Length; i++)
            {
                if (i % 16 == 0)
                {
                    if (currentRow != null)
                        _dataRows.Add(currentRow);
                    currentRow = new DataRowViewModel(i);
                }

                currentRow?.Bytes.Add(new ByteViewModel(_data, i));
            }

            if (currentRow != null)
                _dataRows.Add(currentRow);

            Update();
        }

        private void Update()
        {
            if (_packet == null)
            {
                CanAddBool =
                CanAddByte =
                CanAddShort =
                CanAddInt =
                CanAddLong =
                CanAddFloat =
                CanAddString =
                CanUndo = 
                CanClear = false;
            }
            else
            {
                CanAddBool = _packet.CanReadBool();
                CanAddByte = _packet.Available >= 1;
                CanAddShort = _packet.Available >= 2;
                CanAddInt = _packet.Available >= 4;
                CanAddLong = _packet.Available >= 8;
                CanAddFloat = _packet.Available >= 4;
                CanAddString = _packet.CanReadString();
                CanUndo = CanClear = StructureItems.Count > 0;
            }
        }

        private void AddStructureItem(TypeCode type)
        {
            if (_packet == null) return;

            object value;
            string? stringValue = null;

            int offset = _packet.Position;
            switch (type)
            {
                case TypeCode.Boolean:
                    value = _packet.ReadBool();
                    stringValue = value.ToString()?.ToLower() ?? "?";
                    break;
                case TypeCode.Byte:
                    value = _packet.ReadByte();
                    break;
                case TypeCode.Int16:
                    value = _packet.ReadShort();
                    break;
                case TypeCode.Int32:
                    value = _packet.ReadInt();
                    break;
                case TypeCode.Single:
                    value = _packet.ReadFloat();
                    break;
                case TypeCode.Int64:
                    value = _packet.ReadLong();
                    break;
                case TypeCode.String:
                    value = _packet.ReadString();
                    break;
                default:
                    return;
            }

            if (stringValue == null)
                stringValue = value.ToString() ?? "?";

            int length = _packet.Position - offset;

            var structureItem = new StructureItemViewModel()
            {
                Type = type,
                Offset = offset,
                Length = length,
                ValueString = stringValue
            };
            StructureItems.Add(structureItem);

            int baseRow = offset / 16;
            int nRows = ((offset % 16 + length) - 1) / 16 + 1;

            _spanMap[offset] = new List<ByteSpanViewModel>();

            for (int i = 0; i < nRows; i++)
            {
                bool
                    isStart = i == 0,
                    isEnd = i == (nRows - 1);

                int col = 0, colSpan = 16;

                if (isStart && isEnd)
                {
                    col = offset % 16;
                    colSpan = length;
                }
                else if (isStart)
                {
                    col = offset % 16;
                    colSpan = 16 - col;
                }
                else if (isEnd)
                {
                    col = 0;
                    colSpan = (offset + length - 1) % 16 + 1;
                }

                DataRowViewModel row = _dataRows[baseRow + i];
                ByteSpanViewModel byteSpan = new(row)
                {
                    Type = type,
                    Column = col,
                    ColumnSpan = colSpan,
                    OpenLeft = !isStart,
                    OpenRight = !isEnd
                };

                _spanMap[offset].Add(byteSpan);
                row.Spans.Add(byteSpan);
            }

            Update();
        }

        private void AddBool()
        {
            if (_packet?.CanReadBool() == true)
            {
                AddStructureItem(TypeCode.Boolean);
            }
        }

        private void AddByte()
        {
            if (_packet?.Available >= 1)
            {
                AddStructureItem(TypeCode.Byte);
            }
        }

        private void AddShort()
        {
            if (_packet?.Available >= 2)
            {
                AddStructureItem(TypeCode.Int16);
            }
        }

        private void AddInt()
        {
            if (_packet?.Available >= 4)
            {
                AddStructureItem(TypeCode.Int32);
            }
        }

        private void AddFloat()
        {
            if (_packet?.Available >= 4)
            {
                AddStructureItem(TypeCode.Single);
            }
        }

        private void AddLong()
        {
            if (_packet?.Available >= 8)
            {
                AddStructureItem(TypeCode.Int64);
            }
        }

        private void AddString()
        {
            if (_packet?.CanReadString() == true)
            {
                AddStructureItem(TypeCode.String);
            }
        }
    }
}
