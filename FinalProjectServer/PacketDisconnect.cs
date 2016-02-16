using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectServer
{
    public class PacketDisconnect : Packet
    {
        public int ID { get; set; }

        public PacketDisconnect(int id, System.Net.IPEndPoint destination) : base("PACKET:DISCONNECT", destination) 
        {
            this.ID = id;
            this.SetData(new PacketData(this, id));
        }

        protected override PacketData ParseData(string data)
        {
            string messageCode = data.Split('{')[1].Split('}')[0];
            int id = Convert.ToInt32(messageCode);
            return new PacketData(new PacketDisconnect(id, null), id);
        }
    }
}
