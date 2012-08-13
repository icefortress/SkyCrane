using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyCrane.Screens;
using Microsoft.Xna.Framework;
using SkyCrane.NetCode;

namespace SkyCrane.Dudes
{
    public abstract class AttackingDude : OrientalDude
    {
        public bool attacking = false;
        String textureAttackLeft;
        String textureAttackRight;

        public bool damageApplied = false; // makes sure an attack only does damage once
        // will unfortunately need to be re-implemented on all attacks, as they do not inherit from some "attack" class

        int attackFrameWidth;

        protected TimeSpan lastAttack = new TimeSpan(0);

        public AttackingDude(GameplayScreen g, int posX, int posY, int frameWidth, int attackFrameWidth, String textureLeft, String textureRight, String textureAttackLeft, String textureAttackRight, float scale) :
            base(g, posX, posY, frameWidth, textureLeft, textureRight, scale)
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
                context.PlayAttackSound(this);
                lastAttack = gameTime.TotalGameTime;
                attacking = true;
                forceCheck = true;
                damageApplied = false;
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
