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
    class Bullet : Dude
    {
        public static String textureName = "bullet";
        public new static int frameWidth = 20;
        public static Vector2 HITBOX_SIZE = new Vector2(30, 30);
        public static float SCALE = 1;

        int payload = 1;

        public PlayerCharacter owner = null;
        public PlayerCharacter lastOwner = null;

        public Bullet(GameplayScreen g, Vector2 position, Vector2 velocity) :
            base(g, (int)position.X, (int)position.Y, frameWidth, textureName, textureName, SCALE)
        {
            this.velocity = velocity;
        }

        public override string getDefaultTexture()
        {
            return textureName;
        }

        public override Vector2 getHitbox()
        {
            return HITBOX_SIZE;
        }

        public override void UpdateSprite(GameTime gameTime)
        {
            if(owner != null) this.worldPosition = owner.worldPosition;
            base.UpdateSprite(gameTime);
        }

        public void attach(PlayerCharacter pc)
        {
            if (lastOwner == pc) return;
            owner = pc;
            this.velocity = Vector2.Zero;
        }

        public void refire(PlayerCharacter pc, Vector2 velocity)
        {
            if (owner == pc)
            {
                lastOwner = owner;
                owner = null;
                this.velocity = velocity;
            }
        }

        public void LevelUp()
        {
            payload *= 2;
            scale += 0.5F;
        }

        public override void HandleCollision(CollisionDirection cd, PhysicsAble entity)
        {
            // Die if you hit a wall
            if (entity is Level)
            {
                destroy();
                context.bulletExists = false;
            }
            else if (entity is Enemy)
            {
                Enemy e = (Enemy)entity;
                e.applyDamage(payload);
                destroy();
            }
        }
    }
}
