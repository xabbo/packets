using System;

namespace b7.Packets.Common.Parsers
{
    public interface IParserContext
    {
        bool ReadBool(string? name = null);
        byte ReadByte(string? name = null);
        short ReadShort(string? name = null);
        int ReadInt(string? name = null);
        float ReadFloat(string? name = null);
        long ReadLong(string? name = null);
        string ReadString(string? name = null);
    }
}
