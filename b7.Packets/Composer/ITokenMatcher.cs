using System;
using System.Collections.Generic;

namespace b7.Packets.Composer
{
    public interface ITokenMatcher
    {
        IEnumerable<TokenMatch> FindMatches(string input);
    }
}
