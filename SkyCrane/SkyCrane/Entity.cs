using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkyCrane.Screens;

namespace SkyCrane
{
    public class Entity
    {
        public Vector2 worldPosition;
        public Vector2 drawingPosition;
        public Vector2 velocity;

        GameplayScreen context;

        // Drawable components
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

        public Entity(GameplayScreen g)
        {
            this.context = g;
        }

        public void InitDrawable(Texture2D texture,
            int frameWidth, int frameHeight, int frameCount,
            int frametime, Color color, float scale, bool looping)
        {
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

            drawingPosition = worldPosition - context.viewPosition + new Vector2(1280/2, 720/2);

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
            destinationRect = new Rectangle((int)drawingPosition.X - (int)(frameWidth * scale) / 2,
            (int)drawingPosition.Y - (int)(frameHeight * scale) / 2,
            (int)(frameWidth * scale),
            (int)(frameHeight * scale));
        }

        public void Draw(GameTime gameTime, SpriteBatch sb)
        {
            if (active)
            {
                UpdateSprite(gameTime);
                sb.Draw(spriteStrip, destinationRect, sourceRect, color);
            }
        }
    }
}
