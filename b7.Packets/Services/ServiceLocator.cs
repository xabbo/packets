using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ninject;
using Ninject.Modules;

using b7.Packets.ViewModel;

namespace b7.Packets.Services
{
    public class ServiceLocator : NinjectModule
    {
        public override void Load() { }

        public MainViewManager MainView => Kernel.Get<MainViewManager>();
        public LogViewManager LogView => Kernel.Get<LogViewManager>();
        public StructureViewManager StructureView => Kernel.Get<StructureViewManager>();
    }
}
