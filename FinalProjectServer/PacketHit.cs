using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectServer
{
    public class PacketHit : Packet
    {
        public int BulletID { get; set; }
        public Bullet.Type BulletType { get; set; }
        public int TargetID { get; set; }

        public PacketHit(int bulletID, Bullet.Type bulletType, int targetID, System.Net.IPEndPoint destination)
            : base("PACKET:HIT", destination)
        {
            this.BulletID = bulletID;
            this.BulletType = bulletType;
            this.TargetID = targetID;
            string parameters = bulletID + ":" + bulletType + ":" + targetID;
            this.SetData(new PacketData(this, parameters));
        }

        protected override PacketData ParseData(string data)
        {
            string messageCode = data.Split('{')[1].Split('}')[0];
            string[] infoSplit = messageCode.Split(':');
            int bulletID = Convert.ToInt32(infoSplit[0]);
            Bullet.Type bulletType = (Bullet.Type)Enum.Parse(typeof(Bullet.Type), infoSplit[1]);
            int targetID = Convert.ToInt32(infoSplit[2]);
            return new PacketData(new PacketHit(bulletID, bulletType, targetID, null), bulletID, bulletType, targetID);
        }
    }
}
