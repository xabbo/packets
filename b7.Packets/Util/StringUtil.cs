using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace b7.Packets.Util
{
    public static class StringUtil
    {
        public static bool IsHex(char c) =>
            ('0' <= c && c <= '9') ||
            ('a' <= c && c <= 'f') ||
            ('A' <= c && c <= 'F');

        public static char GetAsciiChar(byte value) => (0x20 <= value && value <= 0x7E) ? (char)value : '.';

        public static void AppendHexString(this StringBuilder sb, ReadOnlySpan<byte> bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i > 0) sb.Append(' ');
                sb.Append(bytes[i].ToString("x2"));
            }
        }

        public static string ToHexString(ReadOnlySpan<byte> bytes)
        {
            var sb = new StringBuilder();
            AppendHexString(sb, bytes);
            return sb.ToString();
        }

        public static string Unescape(string value)
        {
            StringBuilder sb = new();

            bool isUnescaping = false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];

                if (isUnescaping)
                {
                    switch (c)
                    {
                        case 'u': // Unicode escape sequence (UTF-16)
                        case 'U': // Unicode escape sequence (UTF-32)
                        case 'x':
                            {
                                int len = c == 'U' ? 8 : 4;
                                bool isVariableLength = c == 'x';

                                int j;
                                for (j = 1; j <= len; j++)
                                {
                                    if (value.Length <= (i + j) || !IsHex(value[i + j]))
                                    {
                                        if (j > 1 && isVariableLength)
                                            break;
                                        throw new FormatException($"Invalid unicode escape sequence: '\\{value[i..]}'");
                                    }
                                }

                                string hex = value[(i + 1)..j];
                                sb.Append((char)int.Parse(hex, NumberStyles.HexNumber));

                                i += j - 1;
                            }
                            break;
                        default:
                            {
                                sb.Append(c switch
                                {
                                    '"' => '"',
                                    '\\' => '\\',
                                    '0' => '\0',
                                    'a' => '\a',
                                    'b' => '\b',
                                    'f' => '\f',
                                    'n' => '\n',
                                    'r' => '\r',
                                    't' => '\t',
                                    'v' => '\v',
                                    _ => throw new FormatException($"Invalid escape sequence: \\{c}")
                                });
                            }
                            break;
                    }

                    isUnescaping = false;
                }
                else
                {
                    if (c == '\\')
                    {
                        isUnescaping = true;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            return sb.ToString();
        }

        public static string Escape(string value)
        {
            return Regex.Replace(value, @"[""\\\p{C}]", x =>
            {
                return x.Value switch
                {
                    "\"" => "\\\"",
                    "\\" => "\\\\",
                    "\0" => "\\0",
                    "\a" => "\\a",
                    "\b" => "\\b",
                    "\f" => "\\f",
                    "\n" => "\\n",
                    "\r" => "\\r",
                    "\t" => "\\t",
                    "\v" => "\\v",
                    _ => x.Value,
                };
            });
        }
    }
}
