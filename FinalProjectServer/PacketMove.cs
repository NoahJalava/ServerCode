using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FinalProjectServer
{
    public class PacketMove : Packet
    {
        public int ID { get; set; }
        public RectangleF NewPosition { get; set; }

        public PacketMove(int id, RectangleF newPosition, System.Net.IPEndPoint destination)
            : base("PACKET:MOVE", destination)
        {
            this.ID = id;
            this.NewPosition = newPosition;
            this.SetData(new PacketData(this, newPosition.X + ":" + newPosition.Y + ":" + newPosition.Width + ":" + newPosition.Height, id));
        }

        protected override PacketData ParseData(string data)
        {
            string messageCode = data.Split('{')[1].Split('}')[0];
            string[] dataSplit = messageCode.Split(',');
            string[] infoSplit = dataSplit[0].Split(':');
            RectangleF newPosition = new RectangleF();
            if (infoSplit.Length > 3)
            {
                newPosition.X = (float)Convert.ToDouble(infoSplit[0]);
                newPosition.Y = (float)Convert.ToDouble(infoSplit[1]);
                newPosition.Width = (float)Convert.ToDouble(infoSplit[2]);
                newPosition.Height = (float)Convert.ToDouble(infoSplit[3]);
            }
            return new PacketData(new PacketMove(Convert.ToInt32(dataSplit[1]), newPosition, null), newPosition, Convert.ToInt32(dataSplit[1]));
        }
    }
}
