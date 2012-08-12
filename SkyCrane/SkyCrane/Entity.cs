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

        // Accessor which creates a state change but doesn't apply until later
        public Vector2 worldPosition
        {
            get
            {
                return worldPosBack;
            }

            set
            {
                if (value != worldPosBack)
                {
                    StateChange sc = new StateChange();
                    sc.type = StateChangeType.MOVED;
                    sc.intProperties.Add(StateProperties.ENTITY_ID, id);
                    sc.intProperties.Add(StateProperties.POSITION_X, (int)value.X);
                    sc.intProperties.Add(StateProperties.POSITION_Y, (int)value.Y);

                    notifyStateChangeListeners(sc);
                }
            }
        }
        public Vector2 drawingPosition;

        public Vector2 velocity;

        public Vector2 size; // This is the sprite size, not necessarily the physical form

        public List<StateChangeListener> slListeners = new List<StateChangeListener>();

        public GameplayScreen context;

        // Drawable components
        public Texture2D spriteStrip;
        public float scale = 1;
        public int frameTime;
        public List<int> animationFrames;
        int elapsedTime;
        int currentFrame;
        Color color;
        protected float rotation = 0;

        Rectangle sourceRect = new Rectangle();
        Rectangle destinationRect = new Rectangle();

        public int frameWidth;
        public int frameHeight;
        public bool active = false;
        public bool looping;

        public static StateChange createEntityStateChange(int entity_id, int posX, int posY, int frameWidth, String textureName)
        {
            StateChange sc = new StateChange();
            sc.type = StateChangeType.CREATE_ENTITY;
            sc.intProperties.Add(StateProperties.ENTITY_ID, entity_id);
            sc.intProperties.Add(StateProperties.POSITION_X, posX);
            sc.intProperties.Add(StateProperties.POSITION_Y, posY);
            sc.intProperties.Add(StateProperties.FRAME_WIDTH, frameWidth);
            sc.stringProperties.Add(StateProperties.SPRITE_NAME, textureName);

            return sc;
        }

        public Entity(GameplayScreen g, int posX, int posY, int frameWidth, String textureName)
        {
            this.context = g;

            id = next_id;
            next_id++;

            worldPosBack= new Vector2(posX, posY);

            changeAnimation(frameWidth, textureName);
        }

        public void changeAnimation(int frameWidth, String textureName)
        {
            Texture2D chara = context.textureDict[textureName];

            List<int> animationFrames = new List<int>(); // TODO: some way of loading animation
            for (int i = 0; i < chara.Width / frameWidth; i++)
            {
                animationFrames.Add(i);
            }
            InitDrawable(chara, frameWidth, chara.Height, animationFrames, 200, Color.White, this.scale, true);
            active = true;

            currentFrame = 0;
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

        public virtual void UpdateSprite(GameTime gameTime)
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

        public void destroy()
        {
            StateChange sc = new StateChange();
            sc.type = StateChangeType.DELETE_ENTITY;
            sc.intProperties.Add(StateProperties.ENTITY_ID, id);
            notifyStateChangeListeners(sc);
        }

        public void Draw(GameTime gameTime, SpriteBatch sb)
        {
            if (active)
            {
                UpdateSprite(gameTime);
                sb.Draw(spriteStrip, destinationRect, sourceRect, color, rotation, Vector2.Zero, SpriteEffects.None, 0);
            }
        }
    }
}
