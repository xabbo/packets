using System;
using System.Windows;
using System.Reflection;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Xabbo.Interceptor;
using Xabbo.Interceptor.GEarth;

using b7.Packets.Services;
using b7.Packets.ViewModel;

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

            string interceptorService = context.Configuration.GetValue<string>("Interceptor:Service");
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

                        services.AddSingleton<IRemoteInterceptor, GEarthRemoteInterceptor>();
                    }
                    break;
                default:
                    throw new Exception($"Unsupported interceptor service: {interceptorService}");
            }

            services.AddSingleton<IContext>(new WpfContext(Dispatcher));

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
    }
}
