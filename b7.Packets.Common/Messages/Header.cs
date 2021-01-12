using System;

namespace b7.Packets.Common.Messages
{
    public struct Header
    {
        public static readonly Header Unknown = new Header(Destination.Unknown, -1);

        public Destination Destination { get; }
        public short Value { get; }
        public string? Name { get; }

        public Header(Destination destination, short value, string name)
        {
            Destination = destination;
            Value = value;
            Name = name;
        }

        public Header(Destination destination, short value)
        {
            Destination = destination;
            Value = value;
            Name = null;
        }

        public Header(short value)
        {
            Destination = Destination.Unknown;
            Value = value;
            Name = null;
        }

        public override int GetHashCode() => Value;

        public override bool Equals(object? obj)
        {
            return
                obj is Header other &&
                Equals(other);
        }

        public bool Equals(Header other)
        {
            return
                Value == other.Value &&
                (Destination == other.Destination ||
                Destination == Destination.Unknown ||
                other.Destination == Destination.Unknown);
        }

        public override string ToString() => Name is null ? Value.ToString() : $"{Name} ({Value})";

        public static implicit operator short(Header header) => header.Value;
        public static implicit operator Header(short value) => new Header(value);

        public static bool operator ==(Header a, Header b) => a.Equals(b);
        public static bool operator !=(Header a, Header b) => !(a == b);
    }
}
