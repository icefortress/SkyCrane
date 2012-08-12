using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyCrane.Screens;

namespace SkyCrane
{
    class Wizard : PlayerCharacter
    {
        public static String TEXTURE_LEFT = "wizardl";
        public static String TEXTURE_RIGHT = "wizardr";
        public static String TEXTURE_ATTACK_LEFT = "wizardal";
        public static String TEXTURE_ATTACK_RIGHT = "wizardar";

        public static int FRAME_WIDTH = 45;
        public static int ATK_FRAME_WIDTH = 45;
        
        public Wizard(GameplayScreen g, int posX, int posY) :
            base(g, posX, posY, FRAME_WIDTH, ATK_FRAME_WIDTH,
            TEXTURE_LEFT, TEXTURE_RIGHT, TEXTURE_ATTACK_LEFT, TEXTURE_ATTACK_RIGHT)
        {
        }

        public override int getAttackCooldown()
        {
            return 2000;
        }

        public override string getDefaultTexture()
        {
            return TEXTURE_LEFT;
        }

        public override void HandleCollision(CollisionDirection cd, PhysicsAble entity)
        {
            if (entity is Bullet)
            {
                Console.WriteLine("Wizard hit bullet");
            }
            base.HandleCollision(cd, entity);
        }
    }
}
