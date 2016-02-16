using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using GameCreator;

namespace FinalProjectServer
{
    public class PlayerInfo
    {
        public IPAddress Address { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();
        public bool Restart { get; set; }
        public Player Object { get; set; }

        public PlayerInfo()
        {
            Address = null;
            Restart = true;
        }

        public PlayerInfo(IPAddress address, int port, string username, Player obj)
        {
            Address = address;
            Port = port;
            Username = username;
            Restart = true;
            Object = obj;
        }

        public static PlayerInfo GetPlayer(IPAddress address, List<PlayerInfo> list)
        {
            foreach (var user in list)
            {
                if (user.Address.ToString() == address.ToString())
                {
                    return user;
                }
            }
            return null;
        }
    }
}
