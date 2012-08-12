using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyCrane.Screens;
using SkyCrane.Dudes;
using SkyCrane.NetCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SkyCrane.Engine
{
    public class HealthBar : Entity
    {
        public static String backTextureName = "healthbar";
        public static String chunkTextureName = "healthchunk";
        public static int frameWidthBack = 102;
        public static int frameWidthChunk = 10;

        public static Rectangle healthSourceRect = new Rectangle(0, 0, frameWidthChunk, 8);

        Texture2D healthBatch;

        int health = 10;

        public HealthBar(GameplayScreen context, Entity parent)
            : base(context, (int)parent.worldPosition.X, (int)(parent.worldPosition.Y - 45), frameWidthBack, backTextureName, 1)
        {
            healthBatch = context.textureDict[chunkTextureName];
        }

        public bool handleStateChange(StateChange s)
        {
            if (s.type == StateChangeType.DELETE_ENTITY)
            {
                destroy();
                return true;
            }
            else if (s.type == StateChangeType.MOVED)
            {
                int x = s.intProperties[StateProperties.POSITION_X];
                int y = s.intProperties[StateProperties.POSITION_Y] - 45;
                this.worldPosBack = new Vector2(x, y);
            }
            else if (s.type == StateChangeType.CHANGE_HEALTH)
            {
                this.health = s.intProperties[StateProperties.HEALTH];
            }

            return false; // return false if we can take more events
        }

        // Override draw to do tiled drawing
        public override void Draw(GameTime gameTime, SpriteBatch sb)
        {
            if (active)
            {
                base.Draw(gameTime, sb);

                drawingPosition += new Vector2(1, 1);

                Rectangle dest = new Rectangle(destinationRect.X + 1, destinationRect.Y + 1, healthSourceRect.Width, healthSourceRect.Height);

                for (int i = 0; i < health; i++)
                {
                    sb.Draw(healthBatch, dest, healthSourceRect, Color.White, rotation, Vector2.Zero, SpriteEffects.None, 0);
                    dest.X += frameWidthChunk;
                }
            }
        }
    }
}
