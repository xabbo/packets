using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Xabbo.Interceptor;
using Xabbo.Messages;

using b7.Packets.Util;

namespace b7.Packets.Composer
{
    public class PacketComposer
    {
        private readonly Tokenizer _tokenizer = new();

        private readonly IRemoteInterceptor _interceptor;
        private IMessageManager? _messages;

        public PacketComposer(IRemoteInterceptor interceptor)
        {
            _interceptor = interceptor;

            _interceptor.Connected += Interceptor_Connected;
            _interceptor.InterceptorDisconnected += Interceptor_Disconnected;
            _interceptor.Disconnected += Interceptor_Disconnected;
        }

        private void Interceptor_Connected(object? sender, GameConnectedEventArgs e)
        {
            _messages = _interceptor.Messages;
        }

        private void Interceptor_Disconnected(object? sender, EventArgs e)
        {
            _messages = null;
        }

        public IPacket ComposePacket(Destination destination, string structure)
        {
            Packet packet = new();

            IEnumerator<Token> e = _tokenizer
                .Tokenize(structure)
                .Where(token => token.Type != TokenType.SequenceTerminator)
                .GetEnumerator();

            // Parse packet header
            if (!AssertNextType(e, TokenType.Identifier, TokenType.Integer))
                throw new FormatException("Expected identifier or integer as first parameter");

            if (e.Current.Type == TokenType.Identifier)
            {
                packet.Header = GetHeader(destination, e.Current.Value);
            }
            else
            {
                if (!short.TryParse(e.Current.Value, out short headerValue) || headerValue < 0)
                    throw new FormatException($"Invalid header value {e.Current.Value}");

                packet.Header = GetHeader(destination, headerValue);
            }

            // Parse packet structure
            bool isNegating = false;

            while (e.MoveNext())
            {
                if (isNegating)
                {
                    if (!AssertType(e.Current, TokenType.Integer, TokenType.LongInteger, TokenType.Number, TokenType.FloatingPointNumber))
                        throw new FormatException($"Unexpected token after negate operator: {e.Current.Value}");
                }

                switch (e.Current.Type)
                {
                    case TokenType.Undefined:
                        throw new FormatException($"Unexpected text '{e.Current.Value}'");
                    case TokenType.Boolean:
                        packet.WriteBool(e.Current.Value == "true");
                        break;
                    case TokenType.Integer:
                        {
                            if (!int.TryParse((isNegating ? "-" : "") + e.Current.Value, out int value))
                                throw new FormatException($"Invalid int value: {e.Current.Value}");
                            packet.WriteInt(value);
                        }
                        break;
                    case TokenType.LongInteger:
                        {
                            if (!long.TryParse((isNegating ? "-" : "") + e.Current.Value[..^1], out long value))
                                throw new FormatException($"Invalid long value: {e.Current.Value}");
                            packet.WriteLong(value);
                        }
                        break;
                    case TokenType.Number:
                    case TokenType.FloatingPointNumber:
                        {
                            int endIndex = e.Current.Type == TokenType.FloatingPointNumber ? 1 : 0;
                            if (!float.TryParse((isNegating ? "-" : "") + e.Current.Value[..endIndex], out float value))
                                throw new FormatException($"Invalid float value: {e.Current.Value}");
                            packet.WriteFloat(value);
                        }
                        break;
                    case TokenType.String:
                        {
                            string value = StringUtil.Unescape(e.Current.Value[1..^1]);
                            packet.WriteString(value);
                        }
                        break;
                    case TokenType.ByteArray:
                        {
                            string[] values = e.Current.Value[1..^1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            byte[] array = new byte[values.Length];
                            for (int i = 0; i < values.Length; i++)
                            {
                                if (!byte.TryParse(values[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte value))
                                    throw new FormatException("Invalid byte array");
                                array[i] = value;
                            }

                            packet.WriteBytes(array);
                        }
                        break;
                    case TokenType.Negate:
                        {
                            isNegating = true;
                        }
                        continue;
                    case TokenType.ByteSpecifier:
                        {
                            if (!AssertNextType(e, TokenType.Integer))
                                throw new FormatException("Expected integer value after byte specifier");
                            if (!byte.TryParse(e.Current.Value, out byte value))
                                throw new FormatException($"Invalid byte value: {e.Current.Value}");
                            packet.WriteByte(value);
                        }
                        break;
                    case TokenType.ShortSpecifier:
                        {
                            if (!AssertNextTypeWithNegation(e, out isNegating, TokenType.Integer))
                                throw new FormatException("Expected integer value after short specifier");
                            if (!short.TryParse((isNegating ? "-" : "") + e.Current.Value, out short value))
                                throw new FormatException($"Invalid short value: {e.Current.Value}");
                            packet.WriteShort(value);
                        }
                        break;
                    case TokenType.IntSpecifier:
                        {
                            if (!AssertNextTypeWithNegation(e, out isNegating, TokenType.Integer))
                                throw new FormatException("Expected integer value after int specifier");
                            if (!int.TryParse((isNegating ? "-" : "") + e.Current.Value, out int value))
                                throw new FormatException($"Invalid int value: {e.Current.Value}");
                            packet.WriteInt(value);
                        }
                        break;
                    case TokenType.LongSpecifier:
                        {
                            if (!AssertNextTypeWithNegation(e, out isNegating, TokenType.Integer))
                                throw new FormatException("Expected integer value after long specifier");
                            if (!long.TryParse((isNegating ? "-" : "") + e.Current.Value, out long value))
                                throw new FormatException($"Invalid long value: {e.Current.Value}");
                            packet.WriteLong(value);
                        }
                        break;
                    case TokenType.Identifier:
                        {
                            // float NaN ?
                            // etc.
                            throw new FormatException($"Unexpected identifier '{e.Current.Value}'");
                        }
                        //break;
                    default:
                        break;
                }

                isNegating = false;
            }

            return packet;
        }

        private Header GetHeader(Destination destination, string name)
        {
            if (_messages is null)
                throw new InvalidOperationException("Message manager is not initialized");

            Header? header;
            bool acquiredHeader;

            if (destination == Destination.Unknown)
            {
                acquiredHeader =
                    _messages.TryGetHeaderByName(Destination.Client, name, out header) ||
                    _messages.TryGetHeaderByName(Destination.Server, name, out header);
            }
            else
            {
                acquiredHeader = _messages.TryGetHeaderByName(destination, name, out header);
            }

            if (!acquiredHeader)
            {
                string directionString = destination switch
                {
                    Destination.Client => "incoming ",
                    Destination.Server => "outgoing ",
                    _ => ""
                };
                throw new Exception($"Unknown {directionString}message name '{name}'");
            }

            return header ?? Header.Unknown;
        }

        private Header GetHeader(Destination destination, short value)
        {
            if (_messages is null)
                throw new InvalidOperationException("Message manager is not initialized");

            if (destination != Destination.Unknown &&
                _messages.TryGetHeaderByValue(destination, _interceptor.ClientType, value, out Header? header))
            {
                return header;
            }
            else
            {
                return new Header(destination, value);
            }
        }

        private static bool AssertType(Token token, params TokenType[] validTypes) => validTypes.Contains(token.Type);

        private static bool AssertNextType(IEnumerator<Token> e, params TokenType[] validTypes)
            => e.MoveNext() && validTypes.Contains(e.Current.Type);

        private static bool AssertNextTypeWithNegation(IEnumerator<Token> e, out bool negated, params TokenType[] validTypes)
        {
            negated = false;

            if (!e.MoveNext()) return false;
            if (e.Current.Type == TokenType.Negate)
            {
                negated = true;
                if (!e.MoveNext())
                    return false;
            }

            return !validTypes.Contains(e.Current.Type);
        }
    }
}
