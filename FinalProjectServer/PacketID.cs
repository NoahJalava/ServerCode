using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectServer
{
    public class PacketID : Packet
    {
        public int ID { get; set; }
        public GameTeam Team { get; set; }

        public PacketID(int id, GameTeam team, System.Net.IPEndPoint destination)
            : base("PACKET:ID", destination)
        {
            this.ID = id;
            this.Team = team;
            string parameters = "";
            parameters += id + "," + team.Identifier;
            SetData(new PacketData(this, parameters));
        }

        protected override PacketData ParseData(string data)
        {
            string messageCode = data.Split('{')[1].Split('}')[0];
            string[] dataSplit = messageCode.Split(',');
            int id = 0;
            GameTeam team = new GameTeam(-1);
            if (dataSplit.Length > 1)
            {
                id = Convert.ToInt32(dataSplit[0]);
                team = new GameTeam(Convert.ToInt16(dataSplit[1]));
            }

            return new PacketData(new PacketID(id, team, null), id, team);
        }
    }
}
