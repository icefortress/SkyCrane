using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyCrane.Screens;
using Microsoft.Xna.Framework;
using SkyCrane.Engine;

namespace SkyCrane.Dudes
{
    class JarCat : PlayerCharacter, AIable
    {
        public static String TEXTURE_LEFT = "jarcatl";
        public static String TEXTURE_RIGHT = "jarcatr";
        public static String TEXTURE_ATTACK_LEFT = "jarcatal";
        public static String TEXTURE_ATTACK_RIGHT = "jarcatar";

        public static int FRAME_WIDTH = 90;
        public static int ATK_FRAME_WIDTH = 90;

        public override int getAttackLength()
        {
            return 1000;
        }

        public override int getAttackCooldown()
        {
            return 1000;
        }

        // Hack to change velocity
        public void UpdateAI(GameTime gameTime) {
            TimeSpan diff = gameTime.TotalGameTime.Subtract(lastAttack);
            if (diff.Seconds * 1000 + diff.Milliseconds <= getAttackCooldown())
            {
                velocity = Vector2.Zero;
            }

            base.UpdateSprite(gameTime);
        }

        public JarCat(GameplayScreen g, int posX, int posY) :
            base(g, posX, posY, FRAME_WIDTH, ATK_FRAME_WIDTH,
            TEXTURE_LEFT, TEXTURE_RIGHT, TEXTURE_ATTACK_LEFT, TEXTURE_ATTACK_RIGHT)
        {
        }
    }
}
