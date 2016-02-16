using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectServer
{
    public class PacketPing : Packet
    {
        public double Timestamp { get; set; }

        public PacketPing(System.Net.IPEndPoint destination)
            : base("PACKET:PING", destination)
        {
            Timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
            this.SetData(new PacketData(this, Timestamp));
        }

        protected override PacketData ParseData(string data)
        {
            string messageCode = data.Split('{')[1].Split('}')[0];
            return new PacketData(this, messageCode);
        }
    }
}
