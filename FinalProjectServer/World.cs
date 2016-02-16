using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameCreator;

namespace FinalProjectServer
{
    public class World
    {
        public static Dictionary<GameObject, Color> ObjectList = new Dictionary<GameObject, Color>();

        public const int WorldWidth = 4320;
        public const int WorldHeight = 2700;

        public static PointF RedSpawn = new PointF(0, 0);
        public static PointF YellowSpawn = new PointF(0, WorldHeight - 80);
        public static PointF GreenSpawn = new PointF(WorldWidth - 80, 0);
        public static PointF BlueSpawn = new PointF(WorldWidth - 80, WorldHeight - 80);

        static World()
        {
            //World boundaries
            ObjectList.Add(new GameObject(-100, -100, WorldWidth + 100, 100), Color.Black);
            ObjectList.Add(new GameObject(-100, -100, 100, WorldHeight + 100), Color.Black);
            ObjectList.Add(new GameObject(-100, WorldHeight, WorldWidth + 100, 100), Color.Black);
            ObjectList.Add(new GameObject(WorldWidth, -100, 100, WorldHeight + 100), Color.Black);
            //Red base
            ObjectList.Add(new GameObject(700, 100, 30, 450), Color.Red);
            ObjectList.Add(new GameObject(100, 700, 450, 30), Color.Red);

            ObjectList.Add(new GameObject(820, 700, 400, 30), Color.Red);
            ObjectList.Add(new GameObject(1480, 600, 400, 30), Color.White);
            ObjectList.Add(new GameObject(1920, 600, 400, 30), Color.White);
            ObjectList.Add(new GameObject(2360, 600, 400, 30), Color.White);
        }
    }
}
