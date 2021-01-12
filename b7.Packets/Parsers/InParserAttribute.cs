using System;
using Xabbo.Core.Messages;

namespace b7.Packets.Parsers
{
    public class InParserAttribute : ParserAttribute
    {
        public InParserAttribute(string messageName)
            : base(Destination.Client, messageName)
        { }
    }
}
