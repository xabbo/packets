using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace b7.Packets.Composer
{
    public class Tokenizer
    {
        public List<ITokenMatcher> TokenMatchers { get; }

        /*
            true / false    Boolean
            1234            Integer
            1234l / 1234L   Long Integer
            1.234           Number
            1.234f          Floating point number
            "Text"          String
            [00 01 02]      Byte array

            b:              Byte specifier
            s:              Short specifier
            i:              Int specifier
            l:              Long specifier

            abcd            Identifier
        */

        public Tokenizer()
        {
            TokenMatchers = new List<ITokenMatcher>()
            {
                new RegexTokenMatcher(TokenType.Boolean, new Regex(@"\b(true|false)\b", RegexOptions.IgnoreCase)),
                new RegexTokenMatcher(TokenType.Integer, new Regex(@"\b\d+\b")),
                new RegexTokenMatcher(TokenType.LongInteger, new Regex(@"\b\d+l\b", RegexOptions.IgnoreCase)),
                new RegexTokenMatcher(TokenType.Number, new Regex(@"\b\d+\.\d+\b")),
                new RegexTokenMatcher(TokenType.FloatingPointNumber, new Regex(@"\b(\d+(\.\d+)?|\.\d+)f\b", RegexOptions.IgnoreCase)),
                new StringTokenMatcher(-1),
                new RegexTokenMatcher(TokenType.ByteArray, new Regex(@"\[\s*[0-9a-f]{2}(\s+[0-9a-f]{2})*\s*\]", RegexOptions.IgnoreCase)),

                new RegexTokenMatcher(TokenType.Negate, new Regex("-")),

                new RegexTokenMatcher(TokenType.ByteSpecifier, new Regex(@"\bb:", RegexOptions.IgnoreCase)),
                new RegexTokenMatcher(TokenType.ShortSpecifier, new Regex(@"\bs:", RegexOptions.IgnoreCase)),
                new RegexTokenMatcher(TokenType.IntSpecifier, new Regex(@"\bi:", RegexOptions.IgnoreCase)),
                new RegexTokenMatcher(TokenType.LongSpecifier, new Regex(@"\bl:", RegexOptions.IgnoreCase)),

                new RegexTokenMatcher(TokenType.Identifier, new Regex(@"\b[a-z][a-z0-9]*\b", RegexOptions.IgnoreCase), precedence: 1),
            };
        }

        public IEnumerable<Token> Tokenize(string input)
        {
            List<TokenMatch> matches = new();

            foreach (ITokenMatcher matcher in TokenMatchers)
                matches.AddRange(matcher.FindMatches(input));

            int previousMatchEnd = 0;

            foreach (var matchGroup in matches
                .GroupBy(match => match.StartIndex)
                .OrderBy(group => group.Key))
            {
                TokenMatch bestMatch = matchGroup
                    .OrderBy(match => match.Precedence)
                    .ThenByDescending(match => match.EndIndex)
                    .First();

                if (bestMatch.StartIndex < previousMatchEnd)
                    continue;

                if (bestMatch.StartIndex > previousMatchEnd &&
                    !IsWhiteSpace(input, previousMatchEnd, bestMatch.StartIndex))
                {
                    yield return new Token()
                    {
                        Type = TokenType.Undefined,
                        Index = previousMatchEnd,
                        Length = bestMatch.StartIndex - previousMatchEnd,
                        Value = input[previousMatchEnd..bestMatch.StartIndex].Trim(),
                    };
                }

                previousMatchEnd = bestMatch.EndIndex;

                yield return new Token()
                {
                    Type = bestMatch.Type,
                    Index = bestMatch.StartIndex,
                    Length = bestMatch.EndIndex - bestMatch.StartIndex,
                    Value = input[bestMatch.StartIndex..bestMatch.EndIndex]
                };
            }

            if (!IsWhiteSpace(input, previousMatchEnd, input.Length))
            {
                yield return new Token()
                {
                    Type = TokenType.Undefined,
                    Index = previousMatchEnd,
                    Length = input.Length - previousMatchEnd,
                    Value = input[previousMatchEnd..].Trim(),
                };
            }

            yield return new Token()
            {
                Type = TokenType.SequenceTerminator,
                Index = input.Length,
                Length = 0,
                Value = string.Empty
            };
        }

        private static bool IsWhiteSpace(string input, int startIndex, int endIndex)
        {
            for (int i = startIndex; i < endIndex; i++)
                if (!char.IsWhiteSpace(input, i))
                    return false;
            return true;
        }
    }
}
