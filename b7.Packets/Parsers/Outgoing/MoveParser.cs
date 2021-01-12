using System;

namespace b7.Packets.Parsers.Outgoing
{
    [OutParser("Move")]
    public class MoveParser : IMessageParser
    {
        public void Parse(IParserContext context)
        {
            context.ReadInt("x");
            context.ReadInt("y");
        }
    }
}
