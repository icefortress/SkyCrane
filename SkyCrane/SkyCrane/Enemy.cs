using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyCrane.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SkyCrane
{
    public class Enemy : Dude, AIable
    {

        public static Enemy createDefaultEnemy(GameplayScreen g)
        {
            Enemy pc = new Enemy(g);
            pc.worldPosition = new Vector2(1280 / 2, 720 / 2 + 200);
            Texture2D chara = g.textureDict["testchar"];
            List<int> animationFrames = new List<int>();
            animationFrames.Add(0);
            pc.InitDrawable(chara, chara.Width, chara.Height, animationFrames, 1, Color.White, 1, true);
            pc.active = true;

            return pc;
        }

        public Enemy(GameplayScreen g)
            : base(g)
        {
        }

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
    }
}
