using System;

namespace b7.Packets.Composer
{
    public struct TokenMatch
    {
        public TokenType Type { get; init; }
        public int Precedence { get; init; }
        public int StartIndex { get; init; }
        public int EndIndex { get; init; }
    }
}
