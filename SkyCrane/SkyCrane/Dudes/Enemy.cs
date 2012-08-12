using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyCrane.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkyCrane.Engine;

namespace SkyCrane.Dudes
{
    public abstract class Enemy : AttackingDude, AIable
    {

        /// <summary>
        /// The possible types of enemies.
        /// </summary>
        public enum Type
        {
            Goblin,
            Skeleton
        }

        public static Vector2 HITBOX_SIZE = new Vector2(45, 20);
        public static float SCALE = 2;

        public Enemy(GameplayScreen g, int posX, int posY, int frameWidth, int attackFrameWidth,
            String textureLeft, String textureRight, String textureAttackLeft, String textureAttackRight) :
            base(g, posX, posY, frameWidth, attackFrameWidth, textureLeft, textureRight, textureAttackLeft, textureAttackRight, SCALE)
        {
        }

        public override Vector2 GetPhysicsSize()
        {
            return HITBOX_SIZE;
        }

        public abstract void UpdateAI(GameTime time); 

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
    }
}
