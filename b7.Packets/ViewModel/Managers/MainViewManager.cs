using System;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

using b7.Packets.Services;
using b7.Modules.Interceptor;

namespace b7.Packets.ViewModel
{
    public class MainViewManager : ObservableObject
    {
        private readonly IContext _context;
        private readonly IRemoteInterceptor _interceptor;

        public event EventHandler? OpenStructureView;

        public LogViewManager Log { get; }
        public StructureViewManager Structure { get; }

        public MainViewManager(IContext context, IRemoteInterceptor interceptor,
            LogViewManager log, StructureViewManager structure)
        {
            _context = context;
            _interceptor = interceptor;

            Log = log;
            Structure = structure;
        }

        public Task InitializeAsync()
        {
            _interceptor.Connected += Interceptor_Connected;
            _interceptor.ConnectionStart += Remote_ConnectionStart;
            _interceptor.ConnectionEnd += Remote_ConnectionEnd;

            _interceptor.Start();

            // Switch tabs when loading a packet in the structure view
            Messenger.Default.Register<GenericMessage<PacketLogViewModel>>(
                this, x => OpenStructureView?.Invoke(this, EventArgs.Empty)
            );

            return Task.CompletedTask;
        }

        private void Interceptor_Connected(object? sender, EventArgs e)
        {

        }

        private void Remote_ConnectionStart(object? sender, EventArgs e)
        {

        }

        private void Remote_ConnectionEnd(object? sender, EventArgs e)
        {

        }
    }
}
