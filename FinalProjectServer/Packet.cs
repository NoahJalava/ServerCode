using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace FinalProjectServer
{
    public abstract class Packet
    {
        private static Packet[] packets = new Packet[]{
            new PacketPing(null),
            new PacketLogin("null", null),
            new PacketDisconnect(-1, null),
            new PacketAddPlayer(new Player(-1), null),
            new PacketMove(-1, new System.Drawing.RectangleF(), null),
            new PacketID(-1, new GameTeam(-1), null),
            new PacketShoot(new Bullet(-1, Bullet.Type.Small, -1, -1, -1, new System.Drawing.Point(), 0), null),
            new PacketHit(-1, Bullet.Type.Small, -1, null)
        };
        protected string identifier = "PACKET:DEFAULT";
        protected PacketData data;
        public IPEndPoint Destiniation;

        public Packet(IPEndPoint destination)
        {
            this.Destiniation = destination;
        }

        protected Packet(string identifier, IPEndPoint destination)
        {
            this.identifier = identifier;
            this.Destiniation = destination;
        }

        public virtual void SetData(PacketData data)
        {
            this.data = data;
        }

        public void SendPacket(UdpClient server)
        {
            string parameters = "";
            for (int i = 0; i < data.Parameters.Length; i++)
            {
                parameters += data.Parameters[i] + (i + 1 == data.Parameters.Length ? "" : ",");
            }
            byte[] byteBuffer = Encoding.ASCII.GetBytes(identifier + "{" + parameters + "}");

            server.Send(byteBuffer, byteBuffer.Length, Destiniation);
        }

        protected abstract PacketData ParseData(string data);

        public static PacketData Parse(string data)
        {
            foreach (var packet in packets)
            {
                if (data.StartsWith(packet.identifier))
                {
                    return packet.ParseData(data);
                }
            }
            return null;
        }
    }
}
