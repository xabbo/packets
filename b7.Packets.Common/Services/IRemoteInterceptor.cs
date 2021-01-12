using System;
using System.Threading.Tasks;

using b7.Packets.Common.Messages;
using b7.Packets.Common.Protocol;

namespace b7.Packets.Common.Services
{
    public interface IRemoteInterceptor
    {
        bool IsRunning { get; }

        void Start();
        void Stop();

        ValueTask SendToServerAsync(IReadOnlyPacket packet);
        ValueTask SendToClientAsync(IReadOnlyPacket packet);

        /// <summary>
        /// Invoked when a connection to the remote interceptor is established.
        /// </summary>
        event EventHandler? Connected;
        /// <summary>
        /// Invoked when the connection to the remove server has been terminated.
        /// </summary>
        event EventHandler? Disconnected;
        /// <summary>
        /// Invoked when the extension has been initialized by the remote interceptor.
        /// </summary>
        event EventHandler? Initialized;
        /// <summary>
        /// Invoked when a connection to the game server has been established.
        /// </summary>
        event EventHandler? ConnectionStart;
        /// <summary>
        /// Invoked when the connection to the game server has been terminated.
        /// </summary>
        event EventHandler? ConnectionEnd;
        /// <summary>
        /// Invoked when a packet has been intercepted by the remote interceptor.
        /// </summary>
        event EventHandler<InterceptArgs>? Intercepted;
    }
}
