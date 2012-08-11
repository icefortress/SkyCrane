using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyCrane.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SkyCrane
{
    /*public class Enemy : Dude, AIable
    {

        public void UpdateAI(GameTime time)
        {
            List<Entity> targets = new List<Entity>();
            
            // TODO: need to add all player characters
            targets.Add(context.gameState.usersPlayer);

            // Find closest target
            Entity target = null;
            float currentLength = 0;
            foreach(Entity e in targets) {
                float sl = (e.worldPosition - worldPosition).Length();
                if (target == null || sl < currentLength)
                {
                    currentLength = sl;
                    target = e;
                }
            }

            // Move towards target
            velocity = (target.worldPosition - worldPosition);

            if (velocity.Length() < 1)
            {
                velocity = Vector2.Zero;
            }
            else
            {
                velocity.Normalize();
            }
        }

        public override void HandleCollision(CollisionDirection cd, PhysicsAble entity)
        {
            velocity = Vector2.Zero;
        }
    }*/
}
