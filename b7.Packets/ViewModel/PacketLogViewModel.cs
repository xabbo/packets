using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace b7.Packets.ViewModel
{
    public class PacketLogViewModel : ViewModelBase
    {
        public short Id { get; }
        public string Name { get; }
        public ReadOnlyMemory<byte> Data { get; }
    }
}
