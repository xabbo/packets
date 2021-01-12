using System;
using System.Collections.Generic;
using Xabbo.Core.Protocol;

namespace b7.Packets.Parsers
{
    public class ParserContext : IParserContext
    {
        private readonly IReadOnlyPacket _packet;

        private readonly List<StructureItem> _structureItems;
        public IReadOnlyList<StructureItem> StructureItems { get; }

        public ParserContext(IReadOnlyPacket packet)
        {
            _packet = packet;

            _structureItems = new List<StructureItem>();
            StructureItems = _structureItems.AsReadOnly();
        }

        public bool ReadBool(string? name = null)
        {
            bool value = _packet.ReadBool();

            _structureItems.Add(new StructureItem
            {
                Type = StructureTypes.Bool,
                Offset = _packet.Position - 1,
                Length = 1,
                Value = value,
                Name = name
            });

            return value;
        }

        public byte ReadByte(string? name = null)
        {
            byte value = _packet.ReadByte();

            _structureItems.Add(new StructureItem
            {
                Type = StructureTypes.Byte,
                Offset = _packet.Position - 1,
                Length = 1,
                Value = value,
                Name = name
            });

            return value;
        }

        public short ReadShort(string? name = null)
        {
            short value = _packet.ReadShort();

            _structureItems.Add(new StructureItem
            {
                Type = StructureTypes.Short,
                Offset = _packet.Position - 2,
                Length = 2,
                Value = value,
                Name = name
            });

            return value;
        }

        public int ReadInt(string? name = null)
        {
            int value = _packet.ReadInt();

            _structureItems.Add(new StructureItem
            {
                Type = StructureTypes.Int,
                Offset = _packet.Position - 4,
                Length = 4,
                Value = value,
                Name = name
            });

            return value;
        }

        public long ReadLong(string? name = null)
        {
            long value = _packet.ReadLong();

            _structureItems.Add(new StructureItem
            {
                Type = StructureTypes.Long,
                Offset = _packet.Position - 8,
                Length = 8,
                Value = value,
                Name = name
            });

            return value;
        }

        public string ReadString(string? name = null)
        {
            int offset = _packet.Position;
            string value = _packet.ReadString();
            int length = _packet.Position - offset;

            _structureItems.Add(new StructureItem
            {
                Type = StructureTypes.String,
                Offset = offset,
                Length = length,
                Value = value,
                Name = name
            });

            return value;
        }
    }
}
