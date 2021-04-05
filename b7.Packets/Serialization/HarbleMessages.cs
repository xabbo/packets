using System;
using System.Collections.Generic;

namespace b7.Packets.Serialization
{
    public class HarbleMessages
    {
        public string Revision { get; set; } = string.Empty;
        public long FileLength { get; set; }
        public List<HarbleMessage> Incoming { get; set; } = new List<HarbleMessage>();
        public List<HarbleMessage> Outgoing { get; set; } = new List<HarbleMessage>();
    }
}
