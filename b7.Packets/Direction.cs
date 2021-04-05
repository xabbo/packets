using System;

namespace b7.Packets.Common.Messages
{
    [Flags]
    public enum Direction
    {
        Incoming = 1,
        Outgoing = 2,
        Both = Incoming | Outgoing
    }
}
