using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkyCrane.Screens;

namespace SkyCrane
{

    public abstract class PlayerCharacter : AttackingDude
    {
        public static Vector2 HITBOX_SIZE = new Vector2(45, 45);
        Bullet bulletRef = null;

        public PlayerCharacter(GameplayScreen g, int posX, int posY, int frameWidth, int attackFrameWidth,
            String textureLeft, String textureRight, String textureAttackLeft, String textureAttackRight) :
            base(g, posX, posY, frameWidth, attackFrameWidth, textureLeft, textureRight, textureAttackLeft, textureAttackRight)
        {
            scale = 2;

            physicsSize = HITBOX_SIZE;
        }

        public override Vector2 getHitbox()
        {
            return HITBOX_SIZE;
        }

        public void fireBullet(Vector2 velocity)
        {
            if (bulletRef == null) return;
            bulletRef.refire(this, velocity);
            bulletRef = null;
        }

        public override void HandleCollision(CollisionDirection cd, PhysicsAble entity)
        {
            if (entity is Bullet)
            {
                bulletRef = (Bullet)entity;
                bulletRef.attach(this);
            }
            else if (entity is Enemy)
            {
                if (attacking)
                {
                    Console.WriteLine("Hit enemy!");
                }
                else
                {
                    Console.WriteLine("Enemy hit player!");
                }
            }
            else if (entity is Level)
            {
                velocity = Vector2.Zero;
            }

        }

    }
}
