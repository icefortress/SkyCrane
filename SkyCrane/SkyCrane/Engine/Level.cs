using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkyCrane.Screens;

namespace SkyCrane.Engine
{
    public class Level : Entity, PhysicsAble
    {
        Texture2D background;
        bool[] bitmap; // For checking collisions
        int bitmapWidth;
        int bitmapHeight;
        Vector2 levelSize; // in pixels, after scaling

        public static float LEVEL_SCALE = 2;

        // TODO: we can use this to build levels with various params
        public static Level generateLevel(GameplayScreen g)
        {
            //return new Level(g, "room2", "room2-collision-map", 1920, 1800);
            return new Level(g, "room2", "room2-collision-map", 1920 * 2, 1800 * 2);
        }

        public Level(GameplayScreen g, String bgKey, String bmKey, int size_x, int size_y)
            : base(g, 1280 / 2, 720 / 2, (int)(size_x / LEVEL_SCALE), 300, bgKey, LEVEL_SCALE)
        {
            this.background = g.textureDict["room2"];
            Texture2D bitmap = g.textureDict["room2-collision-map"];

            Color[] colormap = new Color[bitmap.Width * bitmap.Height];
            this.bitmap = new bool[bitmap.Width * bitmap.Height];
            bitmap.GetData<Color>(colormap);

            for (int i = 0; i < colormap.Length; i++)
            {
                if (colormap[i] == Color.Black)
                {
                    this.bitmap[i] = true;
                } else {
                    this.bitmap[i] = false;
                }
            }

            bitmapWidth = bitmap.Width;
            bitmapHeight = bitmap.Height;

            this.levelSize = new Vector2(size_x, size_y);
        }

        public Vector2 GetPhysicsSize()
        {
            return new Vector2(0, 0); // Level is a special case, is really composed of lots of miniature obstacles
        }

        public Vector2 GetPhysicsPosition()
        {
            return worldPosition;
        }

        public Vector2 GetPhysicsVelocity()
        {
            return Vector2.Zero;
        } 

        public void HandleCollision(CollisionDirection cd, PhysicsAble entity)
        {
        }

        /* Computes the view position (centred) in world coordinates that things should be drawn off of based on player position
         * This is necessary to deal with the edges of the world */
        public Vector2 getViewPosition(Entity c)
        {
            Vector2 characterPosition = c.worldPosition;

            float half_scaled_bg_w = levelSize.X / 2;
            float half_scaled_bg_h = levelSize.Y / 2;

            Vector2 levelPosition = this.worldPosition - new Vector2(half_scaled_bg_w, half_scaled_bg_h); // getting this in terms of top-left coordinate, so we can get player's position in the world

            // Position of the character within the realm of the level
            Vector2 characterInLevel = characterPosition - levelPosition;

            Vector2 position;
            if (characterInLevel.X < 1280 / 2)
            {
                position.X = this.worldPosition.X - half_scaled_bg_w + 1280 / 2;
            }
            else if (characterInLevel.X > (this.levelSize.X - 1280 / 2))
            {
                position.X = this.worldPosition.X + half_scaled_bg_w - 1280/2;
            }
            else
            {
                position.X = characterPosition.X;
            }

            if (characterInLevel.Y < 720 / 2)
            {
                position.Y = this.worldPosition.Y - half_scaled_bg_h + 720 / 2;
            }
            else if (characterInLevel.Y > (2 * half_scaled_bg_h - 720 / 2))
            {
                position.Y = this.worldPosition.Y + half_scaled_bg_h - 720 / 2;
            }
            else
            {
                position.Y = characterPosition.Y;
            }

            return position;
        }

        public CollisionDirection CheckCollision(PhysicsAble entity)
        {
            // Assuming for now position and bounds defined in pixel space, this should be easy to switch out if needed
            Vector2 position = entity.GetPhysicsPosition() + entity.GetPhysicsVelocity();
            Vector2 size = entity.GetPhysicsSize();

            float half_scaled_bg_w = levelSize.X / 2;
            float half_scaled_bg_h = levelSize.Y / 2;
            Vector2 levelPosition = this.worldPosition - new Vector2(half_scaled_bg_w, half_scaled_bg_h);
            Vector2 characterInLevel = position - levelPosition;

            // Define region of pixels under the bounds
            int left = Math.Max(0, (int)Math.Floor((characterInLevel.X - size.X / 2) / levelSize.X * bitmapWidth));
            int width = (int)Math.Ceiling(size.X / levelSize.X * bitmapWidth);
            int top = Math.Max(0, (int)Math.Floor((characterInLevel.Y - size.Y / 2) / levelSize.Y * bitmapHeight));
            int height = (int)Math.Ceiling(size.Y / levelSize.Y * bitmapHeight);

            bool hitTop = false;
            bool hitLeft = false;
            bool hitRight = false;
            bool hitBottom = false;

            for (int x = left; x < left + width; x++)
            {
                for (int y = top; y < top + height; y++)
                {

                    int index = y * bitmapWidth + x;

                    // TODO: shouldn't get too far ahead of myself
                    if (index < bitmap.Length && bitmap[index])
                    {
                        if (x - left < width / 2) hitLeft = true;
                        if (x - left > width / 2) hitRight = true;
                        if (y - top < height / 2) hitTop = true;
                        if (y - top > height / 2) hitBottom = true;
                    }

                }
            }

            if (hitLeft && hitTop)
            {
                return CollisionDirection.TOPLEFT;
            }
            else if (hitRight && hitTop)
            {
                return CollisionDirection.BOTTOMRIGHT;
            }
            else if (hitBottom && hitRight)
            {
                return CollisionDirection.BOTTOMRIGHT;
            }
            else if (hitBottom && hitLeft)
            {
                return CollisionDirection.BOTTOMLEFT;
            }
            else if (hitTop)
            {
                return CollisionDirection.TOP;
            }
            else if (hitBottom)
            {
                return CollisionDirection.BOTTOM;
            }
            else if (hitLeft)
            {
                return CollisionDirection.LEFT;
            }
            else if (hitRight)
            {
                return CollisionDirection.RIGHT;
            }
            else
            {
                return CollisionDirection.NONE;
            }
        }

        public void UpdatePhysics()
        {
        }
    }
}
