using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SkyCrane.Screens
{
    public abstract class Dude : Entity, PhysicsAble
    {
        public Rectangle worldBounds;
        Rectangle leftRect;
        Rectangle rightRect;
        Rectangle topRect;
        Rectangle bottomRect;

        bool facingLeft = true;

        String textureLeft;
        String textureRight;

        public Dude(GameplayScreen g, int posX, int posY, int frameWidth, String textureLeft, String textureRight, String animationName) : base(g, posX, posY, frameWidth, textureLeft, animationName)
        {
            this.textureLeft = textureLeft;
            this.textureRight = textureRight;
        }

        public void UpdatePhysics()
        {
            size = GetPhysicsSize();

            worldBounds = new Rectangle((int)(worldPosition.X - size.X / 2),
                (int)(worldPosition.X - size.Y / 2),
                (int)size.X, (int)size.Y);

            leftRect = new Rectangle(worldBounds.X, worldBounds.Y, worldBounds.Width / 2, worldBounds.Height);
            rightRect = new Rectangle(worldBounds.X + worldBounds.Width / 2, worldBounds.Y, worldBounds.Width / 2, worldBounds.Height);
            topRect = new Rectangle(worldBounds.X, worldBounds.Y, worldBounds.Width, worldBounds.Height / 2);
            bottomRect = new Rectangle(worldBounds.X, worldBounds.Y + worldBounds.Height / 2, worldBounds.Width, worldBounds.Height / 2);
        }

        public Vector2 GetPhysicsSize()
        {
            return size;
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

        public override void setVelocity(Vector2 val)
        {
            /*int frame_width = s.intProperties[StateProperties.FRAME_WIDTH];
                String texture_name = s.stringProperties[StateProperties.SPRITE_NAME];
                String animation_name = s.stringProperties[StateProperties.ANIMATION_NAME];*/

            if (val.X < 0 && !facingLeft)
            {
                facingLeft = true;
                StateChange sc = new StateChange();
                sc.type = StateChangeType.CHANGE_SPRITE;
                sc.intProperties.Add(StateProperties.ENTITY_ID, id);
                sc.stringProperties.Add(StateProperties.SPRITE_NAME, textureLeft);
                sc.stringProperties.Add(StateProperties.ANIMATION_NAME, "poop");

                notifyStateChangeListeners(sc);
            }
            else if (val.X > 0 && facingLeft)
            {
                facingLeft = false;
                StateChange sc = new StateChange();
                sc.type = StateChangeType.CHANGE_SPRITE;
                sc.intProperties.Add(StateProperties.ENTITY_ID, id);
                sc.stringProperties.Add(StateProperties.SPRITE_NAME, textureRight);
                sc.stringProperties.Add(StateProperties.ANIMATION_NAME, "poop");

                notifyStateChangeListeners(sc);
            }
        }

        public CollisionDirection CheckCollision(PhysicsAble entity)
        {
            Vector2 position = entity.GetPhysicsPosition() + entity.GetPhysicsVelocity();
            Vector2 entitySize = entity.GetPhysicsSize();

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
