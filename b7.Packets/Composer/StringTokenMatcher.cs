using System;
using System.Collections.Generic;

namespace b7.Packets.Composer
{
    public class StringTokenMatcher : ITokenMatcher
    {
        private static bool IsHex(char c) =>
          ('0' <= c && c <= '9') ||
          ('a' <= c && c <= 'f') ||
          ('A' <= c && c <= 'F');

        public int Precedence { get; }

        public StringTokenMatcher(int precedence = 0)
        {
            Precedence = precedence;
        }

        public IEnumerable<TokenMatch> FindMatches(string input)
        {
            int startIndex = -1;

            bool isMatching = false, isEscaping = false;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (isEscaping)
                {
                    switch (c)
                    {
                        // case '\'': // Single quote
                        case '"': // Double quote
                        case '\\': // Backslash
                        case '0': // Null
                        case 'a': // Alert
                        case 'b': // Backspace
                        case 'f': // Form feed
                        case 'n': // New line
                        case 'r': // Carriage return
                        case 't': // Horizontal tab
                        case 'v': // Vertical tab
                            break;
                        case 'u': // Unicode escape sequence (UTF-16)
                        case 'U': // Unicode escape sequence (UTF-32)
                        case 'x': // Variable length unicode escape sequence
                            {
                                int len = c == 'U' ? 8 : 4;
                                bool isVariableLength = c == 'x';

                                int j;
                                for (j = 1; j <= len; j++)
                                {
                                    if (input.Length <= (i + j) || !IsHex(input[i + j]))
                                    {
                                        if (j > 1 && isVariableLength)
                                            break;
                                        throw new FormatException($"Invalid unicode escape sequence: '\\{input[i..]}'");
                                    }
                                }

                                i += j - 1;
                            }
                            break;
                        default:
                            throw new FormatException($"Invalid escape sequence: \\{c}");
                    }

                    isEscaping = false;
                }
                else
                {
                    if (isMatching)
                    {
                        if (c == '"')
                        {
                            isMatching = false;

                            yield return new TokenMatch()
                            {
                                Type = TokenType.String,
                                Precedence = Precedence,
                                StartIndex = startIndex,
                                EndIndex = i + 1
                            };
                        }
                        else if (c == '\\')
                        {
                            isEscaping = true;
                        }
                    }
                    else
                    {
                        if (c == '"')
                        {
                            startIndex = i;
                            isMatching = true;
                        }
                    }
                }
            }
        }
    }
}
