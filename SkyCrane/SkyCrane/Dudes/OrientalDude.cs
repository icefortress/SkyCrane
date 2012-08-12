using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SkyCrane.Screens;

namespace SkyCrane.Dudes
{
    class OrientalDude : Dude
    {

        public bool facingLeft = true;
        protected bool forceCheck = false;

        protected String textureLeft;
        protected String textureRight;

        public OrientalDude(GameplayScreen g, int posX, int posY, int frameWidth, String textureLeft, String textureRight, float scale)
            : base(g, posX, posY, frameWidth, textureLeft, scale)
        {
            this.textureLeft = textureLeft;
            this.textureRight = textureRight;
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
                createChangeSpriteStateChange(id, frameWidth, textureLeft, 
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
    }
}
