using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FinalProjectServer
{
    public class PacketAddPlayer : Packet
    {
        public Player Object { get; set; }

        public PacketAddPlayer(Player obj, System.Net.IPEndPoint destination)
            : base("PACKET:ADDPLAYER", destination)
        {
            string parameters = obj.Name + ":" + obj.ID + ":" + obj.Team.Identifier + ":" + obj.Area.X + ":" + obj.Area.Y + ":" + obj.Area.Width + ":" + obj.Area.Height;
            this.SetData(new PacketData(this, parameters));
            this.Object = obj;
        }

        protected override PacketData ParseData(string data)
        {
            string messageCode = data.Split('{')[1].Split('}')[0];
            string[] infoSplit = messageCode.Split(':');
            var obj = new Player(-1);
            if (infoSplit.Length > 4)
                obj = new Player(Convert.ToInt32(infoSplit[1]), (float)Convert.ToDouble(infoSplit[3]), (float)Convert.ToDouble(infoSplit[4]), (float)Convert.ToDouble(infoSplit[5]), (float)Convert.ToDouble(infoSplit[6]));
            obj.Team = new GameTeam(Convert.ToInt16(infoSplit[2]));
            obj.Name = infoSplit[0];
            return new PacketData(new PacketAddPlayer(obj, null), obj);
        }
    }
}
