using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SkyCrane
{
    public class Entity : DrawableGameComponent
    {
        public Vector2 position;
        public Vector2 velocity;

        // Drawable components
        SpriteBatch spriteBatch;
        public Texture2D spriteStrip;
        public float scale;
        public int frameTime;
        public int frameCount;
        int elapsedTime;
        int currentFrame;
        Color color;

        Rectangle sourceRect = new Rectangle();
        Rectangle destinationRect = new Rectangle();

        public int frameWidth;
        public int frameHeight;
        public bool active = false;
        public bool looping;

        public Entity(Game g) : base(g)
        {
            g.Components.Add(this);
        }

        public void InitDrawable(SpriteBatch sb, Texture2D texture,
            int frameWidth, int frameHeight, int frameCount,
            int frametime, Color color, float scale, bool looping)
        {
            this.spriteBatch = sb;
            this.spriteStrip = texture;
            this.scale = scale;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.frameCount = frameCount;
            this.frameTime = frametime;
            this.color = color;
            this.looping = looping;
        }

        public void UpdateSprite(GameTime gameTime)
        {
            // Do not update the game if we are not active
            if (active == false)
                return;

            // Update the elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            // If the elapsed time is larger than the frame time
            // we need to switch frames
            if (elapsedTime > frameTime)
            {
                // Move to the next frame
                currentFrame++;

                // If the currentFrame is equal to frameCount reset currentFrame to zero
                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                    // If we are not looping deactivate the animation
                    if (looping == false)
                        active = false;
                }

                // Reset the elapsed time to zero
                elapsedTime = 0;
            }

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            destinationRect = new Rectangle((int)position.X - (int)(frameWidth * scale) / 2,
            (int)position.Y - (int)(frameHeight * scale) / 2,
            (int)(frameWidth * scale),
            (int)(frameHeight * scale));
        }

        public override void Draw(GameTime gameTime)
        {
            if (active)
            {
                UpdateSprite(gameTime);

                spriteBatch.Begin();
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);

                Console.WriteLine(destinationRect.Top);

                spriteBatch.End();
            }
        }
    }
}
