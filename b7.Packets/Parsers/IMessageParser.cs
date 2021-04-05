using System;

using b7.Packets.Common.Parsers;

namespace b7.Packets.Parsers
{
    public interface IMessageParser
    {
        void Parse(IParserContext context);
    }
}