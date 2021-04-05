using System;

using Microsoft.Extensions.Configuration;

namespace b7.Packets.Modules
{
    public class GEarthRemoteModule
    {
        public int Port { get; }

        public GEarthRemoteModule(IConfiguration config)
        {
            Port = config.GetValue<int>("Interceptor:Port", 9092);
        }

        /*public override void Load()
        {
            int port = Port;

            if (port == -1)
            {
                Netstat.Info? listener = Netstat.GetTcpListeners().FirstOrDefault(
                    x =>
                        x.Process is not null &&
                        x.Process.ProcessName == "javaw" &&
                        x.Process.MainWindowTitle.StartsWith("G-Earth") &&
                        x.LocalPort >= 9090
                );

                port = listener?.LocalPort ?? 9092;
            }

            Bind<IRemoteInterceptor>().To<GEarthRemoteManager>().InSingletonScope()
                .WithConstructorArgument(new GEarthOptions
                {
                    Port = port,
                    Title = "b7 packets",
                    Author = "b7",
                    Description = "C# packet logger for G-Earth",
                    Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "?.?.?"
                });
        }*/
    }
}
