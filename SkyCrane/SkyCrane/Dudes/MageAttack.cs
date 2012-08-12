using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkyCrane.Screens;
using SkyCrane.Engine;

namespace SkyCrane.Dudes
{
    class MageAttack : Dude
    {
        int bounces = 0;

        public static String textureName = "wand";
        public new static int frameWidth = 22;
        public static Vector2 HITBOX_SIZE = new Vector2(30, 30);
        public static float SCALE = 3;

        public PhysicsAble lastHit = null;

        public MageAttack(GameplayScreen g, Vector2 position, Vector2 velocity) :
            base(g, (int)position.X, (int)position.Y, frameWidth, textureName, 3)
        {
            this.velocity = velocity;
            this.frameTime = 30;
        }

        public override int GetFrameTime()
        {
            return 60;
        }

        public override Vector2 GetPhysicsSize()
        {
            return HITBOX_SIZE;
        }

        public Entity getClosestEnemy()
        {
            Entity choice = null;
            float distance = 0;

            foreach (Entity e in context.gameState.entities.Values)
            {
                if(e == lastHit) continue;
                if (!(e is Enemy)) continue;
                Vector2 dir = e.worldPosition - this.worldPosition;

                float dist = dir.Length();

                if (choice == null || dist < distance)
                {
                    choice = e;
                    distance = dist;
                }
            }

            return choice;
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
                if (lastHit != entity)
                {
                    if (bounces == 3)
                    {
                        destroy();
                        return;
                    }

                    // Do damage
                    Enemy e = (Enemy)entity;
                    e.applyDamage(10);

                    // Bounce
                    lastHit = entity;

                    Entity closestEnemy = getClosestEnemy();
                    Vector2 newVelocity = Vector2.Zero;
                    if (closestEnemy == null)
                    {
                        Random r = new Random();
                        newVelocity = new Vector2(r.Next(0, 10) - 5, r.Next(0, 10) - 5);
                    }
                    else
                    {
                        newVelocity = getClosestEnemy().worldPosition - this.worldPosition;
                    }

                    newVelocity.Normalize();

                    

                    velocity = newVelocity * 8;

                    bounces++;
                }
            }
        }
    }
}
