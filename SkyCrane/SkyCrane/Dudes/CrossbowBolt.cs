using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SkyCrane.Screens;
using SkyCrane.Engine;

namespace SkyCrane.Dudes
{
    class CrossbowBolt : Dude
    {
        public static String textureName = "bowbolt";
        public new static int frameWidth = 10;
        public static Vector2 HITBOX_SIZE = new Vector2(20, 5);
        public static float SCALE = 1F;

        public CrossbowBolt(GameplayScreen g, Vector2 position, Vector2 velocity) :
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
            else if (entity is PlayerCharacter) // Hurt players
            {
                // Do damage
                PlayerCharacter e = (PlayerCharacter)entity;
                e.applyDamage(1);
                destroy();
            }
        }
    }
}
