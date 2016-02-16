using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectServer
{
    public class PacketData
    {
        public Packet Packet;
        public object[] Parameters;

        public PacketData(Packet packet, params object[] parameters)
        {
            this.Packet = packet;
            this.Parameters = parameters;
        }
    }
}
