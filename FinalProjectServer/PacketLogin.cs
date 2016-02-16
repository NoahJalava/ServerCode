using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectServer
{
    public class PacketLogin : Packet
    {
        public string Name { get; set; }

        public PacketLogin(string name, System.Net.IPEndPoint destination)
            : base("PACKET:LOGIN", destination)
        {
            this.Name = name;
            SetData(new PacketData(this, name));
        }

        protected override PacketData ParseData(string data)
        {
            return new PacketData(this, data.Split('{')[1].Split('}')[0]);
        }
    }
}
