using System;

namespace b7.Packets
{
    [Flags]
    public enum Direction
    {
        Incoming = 1,
        Outgoing = 2,
        Both = Incoming | Outgoing
    }
}
