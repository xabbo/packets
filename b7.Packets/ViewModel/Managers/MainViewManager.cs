using System;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Messaging;

using b7.Packets.Common.Services;
using b7.Packets.Services;

namespace b7.Packets.ViewModel
{
    public class MainViewManager : ViewModelBase
    {
        private readonly IContext _context;
        private readonly IRemoteInterceptor _interceptor;

        public event EventHandler? OpenStructureView;

        public MainViewManager(IContext context, IRemoteInterceptor interceptor)
        {
            _context = context;
            _interceptor = interceptor;
        }

        public Task InitializeAsync()
        {
            // Switch tabs when loading a packet in the structure view
            Messenger.Default.Register<GenericMessage<PacketLogViewModel>>(
                this, x => OpenStructureView?.Invoke(this, EventArgs.Empty)
            );

            _interceptor.ConnectionStart += Remote_ConnectionStart;
            _interceptor.ConnectionEnd += Remote_ConnectionEnd;

            _interceptor.Start();

            return Task.CompletedTask;
        }

        private void Remote_ConnectionStart(object? sender, EventArgs e)
        {

        }

        private void Remote_ConnectionEnd(object? sender, EventArgs e)
        {

        }
    }
}
