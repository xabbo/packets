using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace b7.Packets.Composer
{
    public class RegexTokenMatcher : ITokenMatcher
    {
        public TokenType Type { get; }
        public int Precedence { get; }
        public Regex Regex { get; }

        public RegexTokenMatcher(TokenType type, Regex regex, int precedence = 0)
        {
            Type = type;
            Regex = new Regex(regex.ToString(), regex.Options | RegexOptions.Compiled);
            Precedence = precedence;
        }

        public IEnumerable<TokenMatch> FindMatches(string input)
        {
            Match match = Regex.Match(input);

            while (match.Success)
            {
                yield return new TokenMatch()
                {
                    Type = Type,
                    Precedence = Precedence,
                    StartIndex = match.Index,
                    EndIndex = match.Index + match.Length
                };

                match = match.NextMatch();
            }
        }
    }
}
