using System;
using System.Threading.Tasks;
using System.Reflection;

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

        private string _title = "b7 packets";
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public LogViewManager Log { get; }
        public MessagesViewManager Messages { get; }
        public StructureViewManager Structure { get; }

        public MainViewManager(IContext context, IRemoteInterceptor interceptor,
            LogViewManager log,
            MessagesViewManager messages,
            StructureViewManager structure)
        {
            _context = context;
            _interceptor = interceptor;

            Log = log;
            Messages = messages;
            Structure = structure;

            Version? version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version is not null) Title += $" v{version.ToString(3)}";

#if DEBUG
            Title += " [DEBUG]";
#endif
        }

        public Task InitializeAsync()
        {
            _interceptor.RunAsync();

            // Switch tabs when loading a packet in the structure view
            Messenger.Default.Register<GenericMessage<PacketLogViewModel>>(
                this, x => OpenStructureView?.Invoke(this, EventArgs.Empty)
            );

            return Task.CompletedTask;
        }
    }
}
