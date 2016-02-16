using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FinalProjectServer
{
    public class GameTeam
    {
        public Color Color { get { return _color; } }
        public int Identifier { get { return _identifier; } }
        public List<PlayerInfo> Players { get { return _playerList; } }

        private Color _color;
        private int _identifier;
        private List<PlayerInfo> _playerList = new List<PlayerInfo>();

        public GameTeam(int identifier)
        {
            switch (identifier)
            {
                case 0:
                    _color = Color.Red;
                    break;
                case 1:
                    _color = Color.Blue;
                    break;
                case 2:
                    _color = Color.Green;
                    break;
                case 3:
                    _color = Color.Yellow;
                    break;
                default:
                    _color = Color.White;
                    break;
            }
            this._identifier = identifier;
        }

        public void AddPlayer(PlayerInfo player)
        {
            _playerList.Add(player);
        }
    }
}
