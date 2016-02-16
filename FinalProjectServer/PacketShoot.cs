using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace FinalProjectServer
{
    public class PacketShoot : Packet
    {
        public Bullet Object { get; set; }

        public PacketShoot(Bullet obj, IPEndPoint destination)
            : base("PACKET:SHOOT", destination)
        {
            string parameters = obj.ID + ":" + (int)obj.BulletType + ":" + obj.Shooter + ":" + obj.Area.X + ":" + obj.Area.Y + ":" + obj.Destination.X + ":" + obj.Destination.Y + ":" + obj.Speed;
            this.SetData(new PacketData(this, parameters));
            this.Object = obj;
        }

        protected override PacketData ParseData(string data)
        {
            string messageCode = data.Split('{')[1].Split('}')[0];
            string[] infoSplit = messageCode.Split(':');
            var obj = new Bullet(-1, Bullet.Type.Small, -1, -1, -1, new System.Drawing.Point(), 0);
            if (infoSplit.Length > 4)
                obj = new Bullet(Convert.ToInt32(infoSplit[0]), (Bullet.Type)Convert.ToInt16(infoSplit[1]), Convert.ToInt16(infoSplit[2]), (float)Convert.ToDouble(infoSplit[3]), (float)Convert.ToDouble(infoSplit[4]), new System.Drawing.Point(Convert.ToInt32(infoSplit[5]), Convert.ToInt32(infoSplit[6])), Convert.ToInt32(infoSplit[7]));
            return new PacketData(new PacketShoot(obj, null), obj);
        }
    }
}
