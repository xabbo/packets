using System;
using System.Windows;

using Ninject;
using Ninject.Modules;

using b7.Packets.Services;
using b7.Packets.ViewModel;
using b7.Packets.Modules;

namespace b7.Packets
{
    public partial class App : Application
    {
        private readonly IKernel _kernel;

        public App()
        {
            _kernel = new StandardKernel();
        }

        private void InitializeKernel(string[] args)
        {
            int port = 9092;
            if (args.Length == 2)
            {
                if (args[0] == "-p")
                {
                    port = int.Parse(args[1]);
                }
            }

            _kernel.Load((INinjectModule)FindResource("Locator"));

            _kernel.Bind<IContext>().ToConstant(new WpfContext(Dispatcher));
            _kernel.Bind<IMessageManager>().To<MessageManager>().InSingletonScope()
                .WithConstructorArgument("filePath", "messages.ini");

            _kernel.Load(new GEarthModule(port));
            // _kernel.Load(new TanjiModule(port));

            _kernel.Bind<MainViewManager>().ToSelf().InSingletonScope();
            _kernel.Bind<StructureViewManager>().ToSelf().InSingletonScope();
        }

        private void ComposeObjects()
        {
            Current.MainWindow = _kernel.Get<MainWindow>();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                InitializeKernel(e.Args);
                ComposeObjects();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message, "Initialization error",
                    MessageBoxButton.OK, MessageBoxImage.Error
                );
                Shutdown();
                return;
            }

            Current.MainWindow.Show();
        }
    }
}
