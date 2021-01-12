using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using b7.Packets.Common.Protocol;

using b7.Packets.Services;

namespace b7.Packets.ViewModel
{
    public class StructureViewManager : ViewModelBase
    {
        private readonly IContext _context;

        private IReadOnlyPacket? _packet;
        private ReadOnlyMemory<byte> _data;

        private Dictionary<int, List<ByteSpanViewModel>> _spanMap;

        public ObservableCollection<DataRowViewModel> DataRows { get; set; }
        public ObservableCollection<StructureItemViewModel> StructureItems { get; set; }

        private bool _canAddBool;
        public bool CanAddBool
        {
            get => _canAddBool;
            set => _set(ref _canAddBool, value);
        }

        private bool _canAddByte;
        public bool CanAddByte
        {
            get => _canAddByte;
            set => _set(ref _canAddByte, value);
        }

        private bool _canAddShort;
        public bool CanAddShort
        {
            get => _canAddShort;
            set => _set(ref _canAddShort, value);
        }

        private bool _canAddInt;
        public bool CanAddInt
        {
            get => _canAddInt;
            set => _set(ref _canAddInt, value);
        }

        private bool _canAddLong;
        public bool CanAddLong
        {
            get => _canAddLong;
            set => _set(ref _canAddLong, value);
        }

        private bool _canAddFloat;
        public bool CanAddFloat
        {
            get => _canAddFloat;
            set => _set(ref _canAddFloat, value);
        }

        private bool _canAddString;
        public bool CanAddString
        {
            get => _canAddString;
            set => _set(ref _canAddString, value);
        }

        private bool _canUndo;
        public bool CanUndo
        {
            get => _canUndo;
            set => _set(ref _canUndo, value);
        }

        private bool _canClear;
        public bool CanClear
        {
            get => _canClear;
            set => _set(ref _canClear, value);
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

            DataRows = new ObservableCollection<DataRowViewModel>();
            StructureItems = new ObservableCollection<StructureItemViewModel>();

            AddBoolCommand = new RelayCommand(AddBoolExecuted);
            AddByteCommand = new RelayCommand(AddByteExecuted);
            AddShortCommand = new RelayCommand(AddShortExecuted);
            AddIntCommand = new RelayCommand(AddIntExecuted);
            AddLongCommand = new RelayCommand(AddLongExecuted);
            AddFloatCommand = new RelayCommand(AddFloatExecuted);
            AddStringCommand = new RelayCommand(AddStringExecuted);
            UndoCommand = new RelayCommand(UndoExecuted);
            ClearCommand = new RelayCommand(ClearExecuted);

            Messenger.Default.Register<GenericMessage<PacketLogViewModel>>(this, x => LoadPacket(x.Content));
        }

        private void UndoExecuted()
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

        private void ClearExecuted()
        {
            if (_packet != null)
                _packet.Position = 0;

            StructureItems.Clear();
            foreach (var row in DataRows)
                row.Spans.Clear();

            Update();
        }

        public void LoadPacket(PacketLogViewModel packetLog)
        {
           _packet = packetLog.Packet;
            _packet.Position = 0;
            _data = new ReadOnlyMemory<byte>(_packet.GetBuffer().ToArray());

            _spanMap.Clear();
            DataRows.Clear();
            StructureItems.Clear();

            DataRowViewModel? currentRow = null;
            for (int i = 0; i < _data.Length; i++)
            {
                if (i % 16 == 0)
                {
                    if (currentRow != null)
                        DataRows.Add(currentRow);
                    currentRow = new DataRowViewModel();
                }

                currentRow?.Bytes.Add(new ByteViewModel(_data, i));
            }

            if (currentRow != null)
                DataRows.Add(currentRow);

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
                CanAddByte = _packet.CanReadByte();
                CanAddShort = _packet.CanReadShort();
                CanAddInt = _packet.CanReadInt();
                CanAddLong = _packet.Available >= 8;
                CanAddFloat = _packet.Available >= 4;
                CanAddString = _packet.CanReadString();
                CanUndo = CanClear = StructureItems.Count > 0;
            }
        }

        private void AddStructureItem(StructureTypes type)
        {
            if (_packet == null) return;

            object value;
            string? stringValue = null;

            int offset = _packet.Position;
            switch (type)
            {
                case StructureTypes.Bool:
                    value = _packet.ReadBool();
                    stringValue = value.ToString()?.ToLower() ?? "?";
                    break;
                case StructureTypes.Byte:
                    value = _packet.ReadByte();
                    break;
                case StructureTypes.Short:
                    value = _packet.ReadShort();
                    break;
                case StructureTypes.Int:
                    value = _packet.ReadInt();
                    break;
                case StructureTypes.Long:
                    value = _packet.ReadLong();
                    break;
                case StructureTypes.Float:
                    value = _packet.ReadFloat();
                    break;
                case StructureTypes.String:
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
                StructureType = type,
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

                DataRowViewModel row = DataRows[baseRow + i];
                ByteSpanViewModel byteSpan = new(row)
                {
                    StructureType = type,
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

        private void AddBoolExecuted()
        {
            if (_packet?.CanReadBool() != true) return;

            AddStructureItem(StructureTypes.Bool);
        }

        private void AddByteExecuted()
        {
            if (_packet?.CanReadByte() != true) return;
            AddStructureItem(StructureTypes.Byte);
        }

        private void AddShortExecuted()
        {
            if (_packet?.CanReadShort() != true) return;
            AddStructureItem(StructureTypes.Short);
        }

        private void AddIntExecuted()
        {
            if (_packet?.CanReadInt() != true) return;
            AddStructureItem(StructureTypes.Int);
        }

        private void AddLongExecuted()
        {
            if (_packet?.Available < 8) return;
            AddStructureItem(StructureTypes.Long);
        }

        private void AddFloatExecuted()
        {
            if (_packet?.Available < 4) return;
            AddStructureItem(StructureTypes.Float);
        }

        private void AddStringExecuted()
        {
            if (_packet?.CanReadString() != true) return;
            AddStructureItem(StructureTypes.String);
        }
    }
}
