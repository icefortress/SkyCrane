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
        public static int next_id = 0;

        public int id;

        public Vector2 worldPosBack;

        public Vector2 worldPosition
        {
            get
            {
                return worldPosBack;
            }

            set
            {
                worldPosBack = value;

                StateChange sc = new StateChange();
                sc.entity_id = id;
                sc.new_position = value;

                notifyStateChangeListeners(sc);
            }
        }
        public Vector2 drawingPosition;
        public Vector2 velocity;
        public Vector2 size; // This is the sprite size, not necessarily the physical form

        public List<StateChangeListener> slListeners = new List<StateChangeListener>();

        public GameplayScreen context;

        // Drawable components
        public Texture2D spriteStrip;
        public float scale;
        public int frameTime;
        public List<int> animationFrames;
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
            id = next_id;
            next_id++;
        }

        public void notifyStateChangeListeners(StateChange sc)
        {
            foreach (StateChangeListener l in slListeners)
            {
                l.handleStateChange(sc);
            }
        }

        public void InitDrawable(Texture2D texture,
            int frameWidth, int frameHeight, List<int> animationFrames,
            int frametime, Color color, float scale, bool looping)
        {
            this.spriteStrip = texture;
            this.scale = scale;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.animationFrames = animationFrames;
            this.frameTime = frametime;
            this.color = color;
            this.looping = looping;

            size = new Vector2(frameWidth * scale, frameHeight * scale);
        }

        public void setAnimationFrames(List<int> frs)
        {
            this.animationFrames = frs;
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
                if (currentFrame == animationFrames.Count)
                {
                    currentFrame = 0;
                    // If we are not looping deactivate the animation
                    if (looping == false)
                        active = false;
                }

                // Reset the elapsed time to zero
                elapsedTime = 0;
            }

            int drawFrame = animationFrames[currentFrame];

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            sourceRect = new Rectangle(drawFrame * frameWidth, 0, frameWidth, frameHeight);

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
