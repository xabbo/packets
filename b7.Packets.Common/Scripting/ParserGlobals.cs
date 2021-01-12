using System;

using b7.Packets.Common.Parsers;

namespace b7.Packets.Common.Scripting
{
    public class ParserGlobals
    {
        private readonly IParserContext _context;

        public ParserGlobals(IParserContext context)
        {
            _context = context;
        }

        public bool ReadBool(string? name = null) => _context.ReadBool(name);

        public byte ReadByte(string? name = null) => _context.ReadByte(name);

        public short ReadShort(string? name = null) => _context.ReadShort(name);

        public int ReadInt(string? name = null) => _context.ReadInt(name);

        public float ReadFloat(string? name = null) => _context.ReadFloat(name);

        public long ReadLong(string? name = null) => _context.ReadLong(name);

        public string ReadString(string? name = null) => _context.ReadString(name);
    }
}
