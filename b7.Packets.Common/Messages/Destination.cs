using System;

namespace b7.Packets.Common.Messages
{
    public enum Destination { Unknown, Client, Server }

    public static partial class EnumExtensions
    {
        public static string ToDirection(this Destination destination)
        {
            return destination switch
            {
                Destination.Client => "Incoming",
                Destination.Server => "Outgoing",
                _ => "Unknown",
            };
        }

        public static Destination ToOpposite(this Destination destination)
        {
            return destination switch
            {
                Destination.Client => Destination.Server,
                Destination.Server => Destination.Client,
                _ => Destination.Unknown,
            };
        }
    }
}
