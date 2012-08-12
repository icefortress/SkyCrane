using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyCrane.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SkyCrane
{
    public abstract class Enemy : AttackingDude, AIable
    {

        public static Vector2 HITBOX_SIZE = new Vector2(45, 20);

        public Enemy(GameplayScreen g, int posX, int posY, int frameWidth, int attackFrameWidth,
            String textureLeft, String textureRight, String textureAttackLeft, String textureAttackRight) :
            base(g, posX, posY, frameWidth, attackFrameWidth, textureLeft, textureRight, textureAttackLeft, textureAttackRight)
        {
            scale = 2;
        }

        public override Vector2 getHitbox()
        {
            return HITBOX_SIZE;
        }

        public override void HandleCollision(CollisionDirection cd, PhysicsAble entity)
        {
            if (entity is MageAttack)
            {
            }
            else if (entity is Enemy)
            {
            }
            else
            {
                velocity = Vector2.Zero;
            }
            
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

            if (velocity.Length() < 50)
            {
                startAttack(time);
            }
            else
            {
                velocity.Normalize();
                velocity *= 2;
            }
        }
    }
}
