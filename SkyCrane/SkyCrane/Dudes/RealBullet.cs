using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SkyCrane.Engine;
using SkyCrane.Screens;

namespace SkyCrane.Dudes
{
    class RealBullet : Dude
    {
        public static String textureName = "realbull";
        public new static int frameWidth = 10;
        public static Vector2 HITBOX_SIZE = new Vector2(10, 10);
        public static float SCALE = 0.5F;

        public RealBullet(GameplayScreen g, Vector2 position, Vector2 velocity) :
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
                // Do damage
                Enemy e = (Enemy)entity;
                e.applyDamage(1);
                destroy();
            }
        }
    }
}
