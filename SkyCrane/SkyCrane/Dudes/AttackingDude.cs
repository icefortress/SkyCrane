using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyCrane.Screens;
using Microsoft.Xna.Framework;

namespace SkyCrane.Dudes
{
    public abstract class AttackingDude : Dude
    {
        public bool attacking = false;
        String textureAttackLeft;
        String textureAttackRight;

        int attackFrameWidth;

        TimeSpan lastAttack = new TimeSpan(0);

        public AttackingDude(GameplayScreen g, int posX, int posY, int frameWidth, int attackFrameWidth, String textureLeft, String textureRight, String textureAttackLeft, String textureAttackRight) :
            base(g, posX, posY, frameWidth, textureLeft, textureRight)
        {
            this.textureAttackLeft = textureAttackLeft;
            this.textureAttackRight = textureAttackRight;
            this.attackFrameWidth = attackFrameWidth;
        }

        // Milleseconds
        public virtual int getAttackLength()
        {
            return 100;
        }

        // Milleseconds
        public virtual int getAttackCooldown()
        {
            return 160;
        }

        public bool startAttack(GameTime gameTime)
        {
            // Check if ready to attack again
            TimeSpan diff = gameTime.TotalGameTime.Subtract(lastAttack);
            if(diff.Seconds * 1000 + diff.Milliseconds >= getAttackCooldown()) {
                lastAttack = gameTime.TotalGameTime;
                attacking = true;
                forceCheck = true;
                return true;
            }

            return false;
        }

        public override void setSpriteFromVelocity()
        {
            if (attacking)
            {
                if (facingLeft)
                {
                    StateChange sc = new StateChange();
                    sc.type = StateChangeType.CHANGE_SPRITE;
                    sc.intProperties.Add(StateProperties.ENTITY_ID, id);
                    sc.intProperties.Add(StateProperties.FRAME_WIDTH, attackFrameWidth);
                    sc.stringProperties.Add(StateProperties.SPRITE_NAME, textureAttackLeft);

                    notifyStateChangeListeners(sc);
                }
                else if (!facingLeft)
                {
                    StateChange sc = new StateChange();
                    sc.type = StateChangeType.CHANGE_SPRITE;
                    sc.intProperties.Add(StateProperties.ENTITY_ID, id);
                    sc.intProperties.Add(StateProperties.FRAME_WIDTH, attackFrameWidth);
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
