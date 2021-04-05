using System;
using System.Windows;
using System.Diagnostics;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using b7.Packets.Services;
using b7.Packets.ViewModel;
using b7.Packets.Util;
using b7.Modules.Interceptor.GEarth;
using b7.Modules.Interceptor;

namespace b7.Packets
{
    public partial class App : Application
    {
        private IHost? _host = null;

        public App() { }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<IContext, WpfContext>(
                provider => ActivatorUtilities.CreateInstance<WpfContext>(provider, Dispatcher)
            );

            // services.AddSingleton<IMessageManager, MessageManager>();

            services.AddSingleton(new GEarthOptions
            {
                Title = "b7 packets",
                Description = "a packet logger",
                Version = "1.0.0",
                Author = "b7"
            });

            string interceptorService = context.Configuration.GetValue<string>("Interceptor:Service");
            switch (interceptorService.ToLower())
            {
                case "g-earth":
                    {
                        services.AddSingleton<IRemoteInterceptor, GEarthRemoteInterceptor>();
                    }
                    break;
                default:
                    throw new Exception($"Unsupported interceptor service: {interceptorService}");
            }

            services.AddSingleton<IContext>(new WpfContext(Dispatcher));

            // services.AddSingleton<IMessageManager, MessageManager>();
            services.AddSingleton<IMessageManager, HarbleMessageManager>();

            services.AddSingleton<MainViewManager>();
            services.AddSingleton<LogViewManager>();
            services.AddSingleton<StructureViewManager>();

            services.AddSingleton<MainWindow>();
        }

        private IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices);
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                _host = CreateHostBuilder(e.Args).Build();

                await _host.StartAsync();

                Current.MainWindow = _host.Services.GetRequiredService<MainWindow>();
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

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null)
            {
                using (_host)
                {
                    await _host.StopAsync(TimeSpan.FromSeconds(10));
                }
            }

            base.OnExit(e);
        }

        static int TryGetPortByProcess(string processName, string? windowTitle = null)
        {
            try
            {
                foreach (var info in Netstat.GetTcpListeners())
                {
                    if (info.Process?.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase) == true &&
                        (windowTitle is null || info.Process.MainWindowTitle.StartsWith(windowTitle, StringComparison.OrdinalIgnoreCase)))
                    {
                        Debug.WriteLine($"Found process {info.Process.ProcessName} \"{info.Process.MainWindowTitle}\" listening on port {info.LocalPort}");
                        return info.LocalPort;
                    }
                }

                return -1;
            }
            catch { return -1; }
        }
    }
}
