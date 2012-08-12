using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyCrane.Screens;

namespace SkyCrane.Dudes
{
    class Tank : PlayerCharacter
    {
        public static String TEXTURE_LEFT = "tankl";
        public static String TEXTURE_RIGHT = "tankr";
        public static String TEXTURE_ATTACK_LEFT = "tankal";
        public static String TEXTURE_ATTACK_RIGHT = "tankar";

        public static int FRAME_WIDTH = 45;
        public static int ATK_FRAME_WIDTH = 45;
        
        public Tank(GameplayScreen g, int posX, int posY) :
            base(g, posX, posY, FRAME_WIDTH, ATK_FRAME_WIDTH,
            TEXTURE_LEFT, TEXTURE_RIGHT, TEXTURE_ATTACK_LEFT, TEXTURE_ATTACK_RIGHT)
        {
        }

        public override string getDefaultTexture()
        {
            return TEXTURE_LEFT;
        }

        public override void HandleCollision(Engine.CollisionDirection cd, Engine.PhysicsAble entity)
        {
            if (entity is Enemy && attacking && !damageApplied)
            {
                Enemy e = (Enemy)entity;
                e.applyDamage(3);
                damageApplied = true;
            }
            else
            {
                base.HandleCollision(cd, entity);
            }
        }
    }
}
