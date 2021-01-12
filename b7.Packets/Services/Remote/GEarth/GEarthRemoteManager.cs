using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using b7.Packets.Common.Messages;
using b7.Packets.Common.Protocol;
using b7.Packets.Common.Services;

namespace b7.Packets.Services.Remote.GEarth
{
    public class GEarthRemoteManager : IRemoteInterceptor
    {
        private static readonly Encoding _encoding = Encoding.GetEncoding("ISO-8859-1");
        private static readonly byte[]
            _toClientBytes = _encoding.GetBytes("TOCLIENT"),
            _toServerBytes = _encoding.GetBytes("TOSERVER");

        public enum Incoming : short
        {
            DoubleClick = 1,
            InfoRequest = 2,
            PacketIntercept = 3,
            FlagsCheck = 4,
            ConnectionStart = 5,
            ConnectionEnd = 6,
            Init = 7,

            PacketToStringResponse = 20,
            StringToPacketResponse = 21
        }

        public enum Outgoing : short
        {
            Info = 1,
            ManipulatedPacket = 2,
            RequestFlags = 3,
            SendMessage = 4,

            PacketToStringRequest = 20,
            StringToPacketRequest = 21,

            ExtensionConsoleLog = 98
        }

        private TcpClient? _client;
        private NetworkStream? _ns;

        private readonly IMessageManager _messages;

        public event EventHandler? Connected;
        protected virtual void OnConnected() => Connected?.Invoke(this, EventArgs.Empty);

        public event EventHandler? Disconnected;
        protected virtual void OnDisconnected() => Disconnected?.Invoke(this, EventArgs.Empty);


        public event EventHandler? Initialized;
        
        public event EventHandler? ConnectionStart;
        public event EventHandler? ConnectionEnd;
        public event EventHandler<InterceptArgs>? Intercepted;
        public event EventHandler? Clicked;

        public GEarthOptions Options { get; }

        public bool IsRunning { get; private set; }

        public GEarthRemoteManager(IMessageManager messages, GEarthOptions options)
        {
            _messages = messages;

            Options = options;
        }

        public void Start()
        {
            if (IsRunning) return;
            IsRunning = true;

            Task.Run(() => HandleInterceptorAsync());
        }

        public void Stop()
        {
            if (!IsRunning) return;

            try 
            {
                _client?.Close();
            }
            catch
            {

            }
        }

        private async Task HandleInterceptorAsync()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        _client = new TcpClient();
                        await _client.ConnectAsync(IPAddress.Loopback, Options.Port);
                        OnConnected();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        await Task.Delay(1000);
                        continue;
                    }

                    try
                    {
                        await HandlePacketsAsync(_ns = _client.GetStream());
                    }
                    catch
                    {
                        OnDisconnected();
                    }
                }
            }
            finally
            {
                IsRunning = false;
            }
        }

        private async Task HandlePacketsAsync(NetworkStream stream)
        {
            Memory<byte> buffer = new byte[4];
            int totalRead;

            while (true)
            {
                totalRead = 0;
                while (totalRead < 4)
                {
                    int read = await stream.ReadAsync(buffer[totalRead..]);
                    if (read <= 0) throw new EndOfStreamException();
                    totalRead += read;
                }

                int length = BinaryPrimitives.ReadInt32BigEndian(buffer.Span);

                byte[] packetData = new byte[length];
                var packetMemory = new Memory<byte>(packetData);

                totalRead = 0;
                while (totalRead < packetData.Length)
                {
                    int read = await stream.ReadAsync(packetMemory[totalRead..]);
                    if (read <= 0) throw new EndOfStreamException();
                    totalRead += read;
                }

                short header = BinaryPrimitives.ReadInt16BigEndian(packetMemory.Span[0..]);
                var packet = new Packet(header, packetData.AsSpan()[2..]);
                await HandlePacketAsync(packet);
            }
        }

        private Task HandlePacketAsync(Packet packet)
        {
            return ((Incoming)packet.Header.Value) switch
            {
                Incoming.DoubleClick => OnDoubleClick(packet),
                Incoming.InfoRequest => OnInfoRequest(packet),
                Incoming.PacketIntercept => OnPacketIntercept(packet),
                Incoming.FlagsCheck => OnFlagsCheck(packet),
                Incoming.ConnectionStart => OnConnectionStart(packet),
                Incoming.ConnectionEnd => OnConnectionEnd(packet),
                Incoming.Init => OnInit(packet),
                _ => Task.CompletedTask
            };
        }

        private Task OnDoubleClick(Packet packet)
        {
            Clicked?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        private Task OnInfoRequest(Packet packet)
        {
            var response = new Packet((short)Outgoing.Info);

            response.WriteString(Options.Title);
            response.WriteString(Options.Author);
            response.WriteString(Options.Version);
            response.WriteString(Options.Description);
            response.WriteBool(Options.EnableOnClick);
            response.WriteBool(!string.IsNullOrWhiteSpace(Options.FilePath));
            response.WriteString(Options.FilePath);
            response.WriteString(Options.Cookie);
            response.WriteBool(Options.CanLeave);
            response.WriteBool(Options.CanDelete);

            return SendAsync(response).AsTask();
        }

        private async Task OnPacketIntercept(Packet packet)
        {
            int len = packet.ReadInt();
            byte[] bytes = new byte[len];
            packet.ReadBytes(bytes.AsSpan());

            string payload = _encoding.GetString(bytes);
            string[] parts = payload.Split('\t', 4);

            bool isBlocked = parts[0] == "1";
            int index = int.Parse(parts[1]);
            var dest = parts[2] == "TOCLIENT" ? Destination.Client : Destination.Server;

            bool isModified = parts[3][0] == '1';
            byte[] packetData = _encoding.GetBytes(parts[3][1..]);

            short headerValue = BinaryPrimitives.ReadInt16BigEndian(packetData.AsSpan()[4..6]);

            if (!_messages.TryGetHeader(dest, headerValue, out Header header))
                header = new Header(dest, headerValue);

            var interceptedPacket = new Packet(header, packetData.AsSpan()[6..]);

            using var args = new InterceptArgs(dest, interceptedPacket, index);
            Intercepted?.Invoke(this, args);

            var response = new Packet((short)Outgoing.ManipulatedPacket);

            response.WriteInt(-1);

            response.WriteByte((byte)(args.IsBlocked ? '1' : '0'));
            response.WriteByte(0x09);

            response.WriteBytes(_encoding.GetBytes(index.ToString()));
            response.WriteByte(0x09);

            response.WriteBytes(dest == Destination.Client ? _toClientBytes : _toServerBytes);
            response.WriteByte(0x09);

            response.WriteByte((byte)((isModified || args.IsModified) ? '1' : '0'));
            response.WriteInt(2 + args.Packet.Length);
            response.WriteShort(args.Packet.Header);
            response.WriteBytes(args.Packet.GetBuffer());

            response.Position = 0;
            response.WriteInt(response.Length - 4);

            await SendAsync(response);
        }

        private Task OnFlagsCheck(Packet packet)
        {
            return Task.CompletedTask;
        }

        private Task OnConnectionStart(Packet packet)
        {
            ConnectionStart?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        private Task OnConnectionEnd(Packet packet)
        {
            ConnectionEnd?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        private Task OnInit(Packet packet)
        {
            Initialized?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        public ValueTask SendAsync(IReadOnlyPacket packet)
        {
            Memory<byte> buffer = new byte[packet.Length + 6];
            BinaryPrimitives.WriteInt32BigEndian(buffer.Span[0..4], 2 + packet.Length);
            BinaryPrimitives.WriteInt16BigEndian(buffer.Span[4..6], packet.Header);
            packet.CopyTo(buffer.Span[6..]);
            return _ns?.WriteAsync(buffer) ?? ValueTask.CompletedTask;
        }

        public ValueTask SendToServerAsync(IReadOnlyPacket packet) => SendToAsync(Destination.Server, packet);
        public ValueTask SendToClientAsync(IReadOnlyPacket packet) => SendToAsync(Destination.Client, packet);

        private ValueTask SendToAsync(Destination destination, IReadOnlyPacket packet)
        {
            Packet requestPacket = new((short)Outgoing.SendMessage);
            requestPacket.WriteByte(destination == Destination.Server ? 1 : 0);
            requestPacket.WriteInt(6 + packet.Length);
            requestPacket.WriteInt(2 + packet.Length);
            requestPacket.WriteShort(packet.Header);
            requestPacket.WriteBytes(packet.GetBuffer());

            return SendAsync(requestPacket);
        }
    }
}
