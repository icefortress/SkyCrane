using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyCrane.Screens;

namespace SkyCrane.Dudes
{
    class Rogue : PlayerCharacter
    {
        public static String TEXTURE_LEFT = "roguel";
        public static String TEXTURE_RIGHT = "roguer";
        public static String TEXTURE_ATTACK_LEFT = "rogueal";
        public static String TEXTURE_ATTACK_RIGHT = "roguear";

        public static int FRAME_WIDTH = 90;
        public static int ATK_FRAME_WIDTH = 84;

        public Rogue(GameplayScreen g, int posX, int posY) :
            base(g, posX, posY, FRAME_WIDTH, ATK_FRAME_WIDTH,
            TEXTURE_LEFT, TEXTURE_RIGHT, TEXTURE_ATTACK_LEFT, TEXTURE_ATTACK_RIGHT)
        {
        }

        public override int getAttackLength()
        {
            return 150;
        }

        public override int getAttackCooldown()
        {
            return 150;
        }
    }
}
