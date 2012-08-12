using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyCrane.Screens;

namespace SkyCrane.Dudes
{
    class Doctor : PlayerCharacter
    {
        public static String TEXTURE_LEFT = "doctorl";
        public static String TEXTURE_RIGHT = "doctorr";
        public static String TEXTURE_ATTACK_LEFT = "doctoral";
        public static String TEXTURE_ATTACK_RIGHT = "doctorar";

        public static int FRAME_WIDTH = 45;
        public static int ATK_FRAME_WIDTH = 45;

        public Doctor(GameplayScreen g, int posX, int posY) :
            base(g, posX, posY, FRAME_WIDTH, ATK_FRAME_WIDTH,
            TEXTURE_LEFT, TEXTURE_RIGHT, TEXTURE_ATTACK_LEFT, TEXTURE_ATTACK_RIGHT)
        {
        }

        public override string getDefaultTexture()
        {
            return TEXTURE_LEFT;
        }
    }
}
