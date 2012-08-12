using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SkyCrane.Screens;
using SkyCrane.NetCode;
using SkyCrane.Engine;

namespace SkyCrane.Dudes
{
    public abstract class Dude : Entity, PhysicsAble
    {
        public Rectangle worldBounds;
        Rectangle leftRect;
        Rectangle rightRect;
        Rectangle topRect;
        Rectangle bottomRect;

        protected bool facingLeft = true;
        protected bool forceCheck = false;

        protected String textureLeft;
        protected String textureRight;

        public Vector2 physicsSize;

        private int health;

        public Dude(GameplayScreen g, int posX, int posY, int frameWidth, String textureLeft, String textureRight, float scale) : base(g, posX, posY, frameWidth, textureLeft, scale)
        {
            this.textureLeft = textureLeft;
            this.textureRight = textureRight;

            physicsSize = getHitbox();

            health = getMaxHealth();
        }

        public abstract Vector2 getHitbox();
        public abstract String getDefaultTexture();

        public virtual int getMaxHealth()
        {
            return 10;
        }

        public void applyDamage(int dmg)
        {
            health -= dmg;
            if (health < 0)
            {
                destroy();
            }
        }

        public void UpdatePhysics()
        {
            size = GetPhysicsSize();

            worldBounds = new Rectangle((int)(worldPosition.X - size.X / 2),
                (int)(worldPosition.Y - size.Y / 2),
                (int)size.X, (int)size.Y);

            leftRect = new Rectangle(worldBounds.X, worldBounds.Y, worldBounds.Width / 2, worldBounds.Height);
            rightRect = new Rectangle(worldBounds.X + worldBounds.Width / 2, worldBounds.Y, worldBounds.Width / 2, worldBounds.Height);
            topRect = new Rectangle(worldBounds.X, worldBounds.Y, worldBounds.Width, worldBounds.Height / 2);
            bottomRect = new Rectangle(worldBounds.X, worldBounds.Y + worldBounds.Height / 2, worldBounds.Width, worldBounds.Height / 2);
        }

        public Vector2 GetPhysicsSize()
        {
            return physicsSize;
        }

        public Vector2 GetPhysicsPosition()
        {
            return worldPosition;
        }

        public Vector2 GetPhysicsVelocity()
        {
            return velocity;
        }

        public virtual void HandleCollision(CollisionDirection cd, PhysicsAble entity)
        {
            velocity = Vector2.Zero;
        }

        public virtual void setSpriteFromVelocity()
        {
            if (forceCheck) forceCheck = false;

            bool go_left = false;
            bool go_right = false;
            if (velocity.X == 0)
            {
                go_left = facingLeft;
                go_right = !facingLeft;
            }

            if (velocity.X < 0 || go_left)
            {
                facingLeft = true;
                StateChange sc = new StateChange();
                sc.type = StateChangeType.CHANGE_SPRITE;
                sc.intProperties.Add(StateProperties.ENTITY_ID, id);
                sc.intProperties.Add(StateProperties.FRAME_WIDTH, frameWidth);
                sc.stringProperties.Add(StateProperties.SPRITE_NAME, textureLeft);

                notifyStateChangeListeners(sc);
            }
            else if (velocity.X > 0 || go_right)
            {
                facingLeft = false;
                StateChange sc = new StateChange();
                sc.type = StateChangeType.CHANGE_SPRITE;
                sc.intProperties.Add(StateProperties.ENTITY_ID, id);
                sc.intProperties.Add(StateProperties.FRAME_WIDTH, frameWidth);
                sc.stringProperties.Add(StateProperties.SPRITE_NAME, textureRight);

                notifyStateChangeListeners(sc);
            }
        }

        public override void UpdateSprite(GameTime gameTime)
        {
            if ((velocity.X != 0 && velocity.X < 0 != facingLeft) || forceCheck) setSpriteFromVelocity();
            base.UpdateSprite(gameTime);
        }

        public CollisionDirection CheckCollision(PhysicsAble entity)
        {
            Entity e = this;

            Vector2 position = entity.GetPhysicsPosition() + entity.GetPhysicsVelocity();
            Vector2 entitySize = entity.GetPhysicsSize();

            if (entitySize == Vector2.Zero)
            {
                return CollisionDirection.NONE;
            }

            int half_x_bounds = (int)entitySize.X / 2;
            int half_y_bounds = (int)entitySize.Y / 2;

            Rectangle checkingBounds = new Rectangle((int)position.X - half_x_bounds, (int)position.Y - half_y_bounds, (int)entitySize.X, (int)entitySize.Y);

            bool left = leftRect.Intersects(checkingBounds);
            bool right = rightRect.Intersects(checkingBounds);
            bool top = topRect.Intersects(checkingBounds);
            bool bottom = bottomRect.Intersects(checkingBounds);

            if (left && top)
            {
                return CollisionDirection.TOPLEFT;
            }
            else if (right && top)
            {
                return CollisionDirection.BOTTOMRIGHT;
            }
            else if (bottom && right)
            {
                return CollisionDirection.BOTTOMRIGHT;
            }
            else if (bottom && left)
            {
                return CollisionDirection.BOTTOMLEFT;
            }
            else if (top)
            {
                return CollisionDirection.TOP;
            }
            else if (bottom)
            {
                return CollisionDirection.BOTTOM;
            }
            else if (left)
            {
                return CollisionDirection.LEFT;
            }
            else if (right)
            {
                return CollisionDirection.RIGHT;
            }
            else
            {
                return CollisionDirection.NONE;
            }
        }
    }
}
