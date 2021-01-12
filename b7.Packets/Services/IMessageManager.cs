using System;

using b7.Packets.Common.Messages;

namespace b7.Packets.Services
{
    public interface IMessageManager
    {
        bool TryGetHeader(Destination destination, short value, out Header header);
        bool TryGetHeader(Destination destination, string name, out Header header);
    }
}
