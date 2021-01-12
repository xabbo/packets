using System;

namespace b7.Packets.Parsers
{
    public interface IMessageParser
    {
        void Parse(IParserContext context);
    }
}