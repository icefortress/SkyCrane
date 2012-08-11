using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SkyCrane
{
    public class Level : Entity, PhysicsAble
    {
        Texture2D background;
        Texture2D bitmap; // For checking collisions
        Vector2 size;

        // TODO: we can use this to build levels with various params
        public static Level generateLevel(SpriteBatch sb, ProjectSkyCrane g)
        {
            Level bah = new Level(sb, g, g.textureDict["testlevel"], g.textureDict["testlevel"], new Vector2(1000, 1000));
            bah.position = new Vector2(0, 0);
            bah.active = true;

            return bah;
        }

        public Level(SpriteBatch sb, Game g, Texture2D background, Texture2D bitmap, Vector2 size) : base(g)
        {
            this.background = background;
            this.bitmap = bitmap;
            this.size = size;

            InitDrawable(sb, background, background.Width, background.Height, 1, 1, Color.White, size.X / background.Width, true);

            this.DrawOrder = 0; // levels should be drawn first
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
