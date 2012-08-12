using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyCrane.Screens;

namespace SkyCrane.Dudes
{
    class JarCat : PlayerCharacter
    {
        public static String TEXTURE_LEFT = "jarcatl";
        public static String TEXTURE_RIGHT = "jarcatr";
        public static String TEXTURE_ATTACK_LEFT = "jarcatal";
        public static String TEXTURE_ATTACK_RIGHT = "jarcatar";

        public static int FRAME_WIDTH = 45;
        public static int ATK_FRAME_WIDTH = 45;

        public override int getAttackLength()
        {
            return 300;
        }

        public override int getAttackCooldown()
        {
            return 400;
        }

        public JarCat(GameplayScreen g, int posX, int posY) :
            base(g, posX, posY, FRAME_WIDTH, ATK_FRAME_WIDTH,
            TEXTURE_LEFT, TEXTURE_RIGHT, TEXTURE_ATTACK_LEFT, TEXTURE_ATTACK_RIGHT)
        {
        }
    }
}
