using System;

namespace b7.Packets.Serialization
{
    public class HarbleMessage
    {
        public short Id { get; set; }
        public string Hash { get; set; } = string.Empty;
        public bool IsOutgoing { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Structure { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string ParserName { get; set; } = string.Empty;
    }
}
