using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectServer
{
    public class PacketLoadPlayers : Packet
    {
        public List<Player> Objects { get; set; }

        public PacketLoadPlayers(List<Player> objects, System.Net.IPEndPoint destination)
            : base("PACKET:LOADPLAYERS", destination)
        {
            this.Objects = objects;

            string parameters = "";
            for (int i = 0; i < objects.Count; i++)
            {
                parameters += (i == 0 ? "" : ",") + objects[i].ID + ':' + objects[i].Area.X + ':' + objects[i].Area.Y + ':' + objects[i].Area.Width + ':' + objects[i].Area.Height;
            }
            this.SetData(new PacketData(this, parameters));
        }

        protected override PacketData ParseData(string data)
        {
            string messageCode = data.Split('{')[1].Split('}')[0];
            string[] dataSplit = messageCode.Split(',');
            var mObjects = new List<Player>();
            foreach (var item in dataSplit)
            {
                string[] infoSplit = item.Split(':');
                if (infoSplit.Length > 4)
                    mObjects.Add(new Player(Convert.ToInt32(infoSplit[0]), (float)Convert.ToDouble(infoSplit[1]), (float)Convert.ToDouble(infoSplit[2]), (float)Convert.ToDouble(infoSplit[3]), (float)Convert.ToDouble(infoSplit[4])));
            }

            return new PacketData(new PacketLoadPlayers(mObjects, null), mObjects);
        }
    }
}
