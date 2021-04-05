using System;
using System.Collections.Generic;

using b7.Packets.Common.Parsers;
using b7.Packets.Common.Protocol;

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

        private T Read<T>(string? name)
        {
            int offset = _packet.Position;
            
            TypeCode typeCode = Type.GetTypeCode(typeof(T));
            object value = typeCode switch
            {
                TypeCode.Boolean => _packet.ReadBool(),
                TypeCode.Byte => _packet.ReadByte(),
                TypeCode.Int16 => _packet.ReadShort(),
                TypeCode.Int32 => _packet.ReadInt(),
                TypeCode.Single => _packet.ReadFloat(),
                TypeCode.Int64 => _packet.ReadLong(),
                TypeCode.String => _packet.ReadString(),
                _ => throw new Exception($"Invalid type code {typeCode}")
            };

            _structureItems.Add(new StructureItem
            {
                Type = typeCode,
                Offset = offset,
                Length = _packet.Position - offset,
                Value = value,
                Name = name
            });

            return (T)value;
        }

        public bool ReadBool(string? name = null) => Read<bool>(name);

        public byte ReadByte(string? name = null) => Read<byte>(name);

        public short ReadShort(string? name = null) => Read<short>(name);

        public int ReadInt(string? name = null) => Read<int>(name);

        public float ReadFloat(string? name = null) => Read<float>(name);

        public long ReadLong(string? name = null) => Read<long>(name);

        public string ReadString(string? name = null) => Read<string>(name);
    }
}
