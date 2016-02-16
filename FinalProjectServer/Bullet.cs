using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameCreator;

namespace FinalProjectServer
{
    public class Bullet : MultiplayerObject
    {
        public Point Destination { get; set; }
        public int Speed { get; set; }
        public Type BulletType { get; set; }
        public int Shooter { get; set; }

        public Bullet(int id, Type type, int shooter, float x, float y, Point destination, int speed)
            : base(x, y, id)
        {
            switch (type)
            {
                case Type.Small:
                    this.Area.Width = 4;
                    this.Area.Height = 4;
                    break;
                default:
                    this.Area.Width = 8;
                    this.Area.Height = 8;
                    break;
            }

            this.Destination = destination;
            this.Speed = speed;
            this.BulletType = type;
            this.Shooter = shooter;
        }

        public void Shoot()
        {
            Translate(Destination, Speed);
        }

        public override void Update()
        {

            base.Update();
        }

        public static int GetDamage(Type type)
        {
            switch (type)
            {
                case Type.Small:
                    return 5;
                default:
                    return 10;
            }
        }

        public enum Type
        {
            Small,
            Medium,
            Large
        }
    }
}
