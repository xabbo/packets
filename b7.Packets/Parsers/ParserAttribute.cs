using System;

using b7.Packets.Common.Messages;

namespace b7.Packets.Parsers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public abstract class ParserAttribute : Attribute
    {
        public Destination Destination { get; }
        public string MessageName { get; }

        public ParserAttribute(Destination destination, string messageName)
        {
            Destination = destination;
            MessageName = messageName;
        }
    }
}
