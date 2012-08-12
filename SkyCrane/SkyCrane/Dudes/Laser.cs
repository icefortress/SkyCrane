using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SkyCrane.Engine;
using SkyCrane.Screens;

namespace SkyCrane.Dudes
{
    class Laser : Dude
    {
        public static String textureName = "laser";
        public new static int frameWidth = 45;
        public static Vector2 HITBOX_SIZE = new Vector2(45, 10);
        public static float SCALE = 1F;

        public List<PhysicsAble> hits = new List<PhysicsAble>();

        public Laser(GameplayScreen g, Vector2 position, Vector2 velocity) :
            base(g, (int)position.X, (int)position.Y, frameWidth, textureName, SCALE)
        {
            this.velocity = velocity;
            this.frameTime = 30;
        }

        public override Vector2 GetPhysicsSize()
        {
            return HITBOX_SIZE;
        }

        public override void HandleCollision(CollisionDirection cd, PhysicsAble entity)
        {
            // Die if you hit a wall
            if (entity is Level)
            {
                destroy();
            }
            else if (entity is Enemy)
            {
                if (!hits.Contains(entity))
                {
                    Enemy e = (Enemy)entity;
                    e.applyDamage(4);
                    hits.Add(e);
                }
            }
        }
    }
}
