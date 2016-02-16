using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCreator;

namespace FinalProjectServer
{
    public class Player : PhysicsObject
    {
        public int ID { get; set; }
        public int Health { get; set; }
        public string Name { get; set; }
        public GameTeam Team { get; set; }

        public Player(int ID)
        {
            this.ID = ID;
            this.Team = new GameTeam(-1);
            this.Health = 100;
        }
        public Player(int ID, float x, float y)
            : base(x, y)
        {
            this.ID = ID;
            this.Team = new GameTeam(-1);
            this.Health = 100;
        }
        public Player(int ID, float x, float y, float width, float height)
            : base(x, y, width, height)
        {
            this.ID = ID;
            this.Team = new GameTeam(-1);
            this.Health = 100;
        }
    }
}
