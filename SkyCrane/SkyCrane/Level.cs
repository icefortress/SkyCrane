using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkyCrane.Screens;

namespace SkyCrane
{
    public class Level : Entity, PhysicsAble
    {
        Texture2D background;
        Texture2D bitmap; // For checking collisions
        Vector2 size;

        // TODO: we can use this to build levels with various params
        public static Level generateLevel(GameplayScreen g)
        {
            Level bah = new Level(g, g.textureDict["testlevel"], g.textureDict["testlevel"], new Vector2(2000, 2000));
            bah.worldPosition = new Vector2(1280/2, 720/2);
            bah.active = true;

            return bah;
        }

        public Level(GameplayScreen g, Texture2D background, Texture2D bitmap, Vector2 size)
            : base(g)
        {
            this.background = background;
            this.bitmap = bitmap;
            this.size = size;

            InitDrawable(background, background.Width, background.Height, 1, 1, Color.White, size.X / background.Width, true);
        }

        /* Computes the view position (centred) in world coordinates that things should be drawn off of based on player position
         * This is necessary to deal with the edges of the world */
        public Vector2 getViewPosition(PlayerCharacter c)
        {
            Vector2 characterPosition = c.worldPosition;

            float half_scaled_bg_w = this.background.Width * scale / 2;
            float half_scaled_bg_h = this.background.Height * scale / 2;

            Vector2 levelPosition = this.worldPosition - new Vector2(half_scaled_bg_w, half_scaled_bg_h); // getting this in terms of top-left coordinate, so we can get player's position in the world

            // Position of the character within the realm of the level
            Vector2 characterInLevel = characterPosition - levelPosition;

            Vector2 position;
            if (characterInLevel.X < 1280 / 2)
            {
                position.X = this.worldPosition.X - half_scaled_bg_w + 1280 / 2;
            }
            else if (characterInLevel.X > (this.size.X - 1280 / 2))
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

        public CollisionDirection CheckCollision(Vector2 position, Rectangle bounds)
        {
            // Assuming for now position and bounds defined in pixel space, this should be easy to switch out if needed

            int left =  (int) Math.Floor((position.X - bounds.Left) / size.X * bitmap.Width);
            int width = (int) Math.Floor(bounds.Width / size.X * bitmap.Width);
            int top = (int) Math.Floor((position.Y - bounds.Top) / size.Y * bitmap.Width);
            int height = (int) Math.Floor(bounds.Height / size.Y * bitmap.Height);

            for (int x = left; x < left + width; x++)
            {
                for (int y = top; y < top + height; y++)
                {
                    // TODO: shouldn't get too far ahead of myself
                }
            }

            return CollisionDirection.NONE;
        }

        public void UpdatePhysics(GameTime time, List<PhysicsAble> others)
        {
        }
    }
}
