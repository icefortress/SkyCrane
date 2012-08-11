using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyCrane.Screens;
using Microsoft.Xna.Framework;

namespace SkyCrane
{
    public class AttackingDude : Dude
    {
        bool attacking = false;
        String textureAttackLeft;
        String textureAttackRight;

        TimeSpan lastAttack = new TimeSpan(0);

        public AttackingDude(GameplayScreen g, int posX, int posY, int frameWidth, String textureLeft, String textureRight, String textureAttackLeft, String textureAttackRight) :
            base(g, posX, posY, frameWidth, textureLeft, textureRight)
        {
            this.textureAttackLeft = textureAttackLeft;
            this.textureAttackRight = textureAttackRight;
        }

        // Milleseconds
        public virtual int getAttackLength()
        {
            return 400;
        }

        // Milleseconds
        public virtual int getAttackCooldown()
        {
            return 1000;
        }

        internal void startAttack(GameTime gameTime)
        {
            // Check if ready to attack again
            TimeSpan diff = gameTime.TotalGameTime.Subtract(lastAttack);
            if(diff.Seconds * 1000 + diff.Milliseconds >= getAttackCooldown()) {
                lastAttack = gameTime.TotalGameTime;
                attacking = true;
                forceCheck = true;
            }
        }

        public override void setSpriteFromVelocity()
        {
            if (attacking)
            {
                if (velocity.X < 0)
                {
                    facingLeft = true;
                    StateChange sc = new StateChange();
                    sc.type = StateChangeType.CHANGE_SPRITE;
                    sc.intProperties.Add(StateProperties.ENTITY_ID, id);
                    sc.intProperties.Add(StateProperties.FRAME_WIDTH, frameWidth);
                    sc.stringProperties.Add(StateProperties.SPRITE_NAME, textureAttackLeft);

                    notifyStateChangeListeners(sc);
                }
                else if (velocity.X > 0)
                {
                    facingLeft = false;
                    StateChange sc = new StateChange();
                    sc.type = StateChangeType.CHANGE_SPRITE;
                    sc.intProperties.Add(StateProperties.ENTITY_ID, id);
                    sc.intProperties.Add(StateProperties.FRAME_WIDTH, frameWidth);
                    sc.stringProperties.Add(StateProperties.SPRITE_NAME, textureAttackRight);

                    notifyStateChangeListeners(sc);
                }
            }
            else
            {
                base.setSpriteFromVelocity();
            }
        }

        public override void UpdateSprite(GameTime gameTime)
        {
            // Check if attack ended
            if (attacking)
            {
                TimeSpan diff = gameTime.TotalGameTime.Subtract(lastAttack);

                if (diff.Seconds * 1000 + diff.Milliseconds > getAttackLength())
                {
                    attacking = false;
                    forceCheck = true;
                }
            }

            base.UpdateSprite(gameTime);  
        }

    }
}
