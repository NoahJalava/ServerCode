using GameCreator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectServer
{
    public class MultiplayerObject : GameObject
    {
        public int ID { get; set; }

        public MultiplayerObject(int id)
        {
            this.ID = id;
        }

        public MultiplayerObject(float x, float y, int id)
            : base(x, y)
        {
            this.ID = id;
        }

        public MultiplayerObject(float x, float y, float width, float height, int id)
            : base(x, y, width, height)
        {
            this.ID = id;
        }
    }
}
