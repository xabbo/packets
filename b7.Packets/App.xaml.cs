using System;
using System.Collections.Generic;
using System.Windows;
using System.Reflection;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Xabbo.Messages;
using Xabbo.GEarth;
using Xabbo.Interceptor;

using b7.Packets.Services;
using b7.Packets.ViewModel;

namespace b7.Packets
{
    public partial class App : Application
    {
        private static readonly Dictionary<string, string> _switchMappings = new()
        {
            ["-p"] = "Xabbo:Interceptor:Port",
            ["-s"] = "Xabbo:Interceptor:Service"
        };

        private IHost? _host = null;

        public App() { }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<IContext, WpfContext>(
                provider => ActivatorUtilities.CreateInstance<WpfContext>(provider, Dispatcher)
            );

            services.AddSingleton<IMessageManager, UnifiedMessageManager>();

            string interceptorService = context.Configuration.GetValue<string>("Xabbo:Interceptor:Service", "g-earth");
            switch (interceptorService.ToLower())
            {
                case "g-earth":
                    {
                        services.AddSingleton(new GEarthOptions
                        {
                            Title = "b7 packets",
                            Description = "a packet logger",
                            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "?",
                            Author = "b7"
                        });

                        services.AddSingleton<GEarthExtension>();
                        services.AddSingleton<IInterceptor>(provider => provider.GetRequiredService<GEarthExtension>());
                        services.AddSingleton<IRemoteInterceptor>(provider => provider.GetRequiredService<GEarthExtension>());
                    }
                    break;
                default:
                    throw new Exception($"Unsupported interceptor service: {interceptorService}");
            }

            services.AddSingleton<IContext>(new WpfContext(Dispatcher));

            services.AddSingleton<MainViewManager>();
            services.AddSingleton<LogViewManager>();
            services.AddSingleton<MessagesViewManager>();
            services.AddSingleton<StructureViewManager>();

            services.AddSingleton<MainWindow>();
        }

        private IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.AddCommandLine(args, _switchMappings);
                })
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
    }
}
