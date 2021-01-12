using System;
using Xabbo.Core.Messages;

namespace b7.Packets.Parsers
{
    public class OutParserAttribute : ParserAttribute
    {
        public OutParserAttribute(string messageName)
            : base(Destination.Server, messageName)
        { }
    }
}
