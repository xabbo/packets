using System;
using System.Reflection;

using Ninject.Modules;

using b7.Packets.Common.Services;
using b7.Packets.Services.Remote.GEarth;

namespace b7.Packets.Modules
{
    public class GEarthModule : NinjectModule
    {
        public int Port { get; }

        public GEarthModule(int port)
        {
            Port = port;
        }

        public override void Load()
        {
            Bind<IRemoteInterceptor>().To<GEarthRemoteManager>().InSingletonScope()
                .WithConstructorArgument(new GEarthOptions
                {
                    Port = Port,
                    Title = "b7 packets",
                    Author = "b7",
                    Description = "C# packet logger for G-Earth",
                    Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown"
                });
        }
    }
}
