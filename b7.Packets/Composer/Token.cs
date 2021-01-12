using System;
using System.Diagnostics;

namespace b7.Packets.Composer
{
    [DebuggerDisplay("\\{{Type}, {Value}\\}")]
    public struct Token
    {
        public TokenType Type { get; init; }
        public int Index { get; init; }
        public int Length { get; init; }
        public string Value { get; init; }
    }
}
