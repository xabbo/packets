using System;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

using Xabbo.Interceptor;

using b7.Packets.Services;

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
            _interceptor.Start();

            // Switch tabs when loading a packet in the structure view
            Messenger.Default.Register<GenericMessage<PacketLogViewModel>>(
                this, x => OpenStructureView?.Invoke(this, EventArgs.Empty)
            );

            return Task.CompletedTask;
        }
    }
}
