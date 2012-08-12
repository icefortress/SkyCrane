using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyCrane.Screens;
using Microsoft.Xna.Framework;
using SkyCrane.Engine;

namespace SkyCrane.Dudes
{
    class Skeleton : Enemy
    {
        public static String TEXTURE_LEFT = "skeletonl";
        public static String TEXTURE_RIGHT = "skeletonr";
        public static String TEXTURE_ATTACK_LEFT = "skeletonal";
        public static String TEXTURE_ATTACK_RIGHT = "skeletonar";

        public static int FRAME_WIDTH = 45;
        public static int ATK_FRAME_WIDTH = 45;
        
        public Skeleton(GameplayScreen g, int posX, int posY) :
            base(g, posX, posY, FRAME_WIDTH, ATK_FRAME_WIDTH,
            TEXTURE_LEFT, TEXTURE_RIGHT, TEXTURE_ATTACK_LEFT, TEXTURE_ATTACK_RIGHT)
        {
        }

        public override void UpdateAI(GameTime time)
        {
            List<PlayerCharacter> targets = context.gameState.players;

            // Find closest target
            Entity target = null;
            float currentLength = 0;
            foreach (Entity e in targets)
            {
                float sl = (e.worldPosition - worldPosition).Length();
                if (target == null || sl < currentLength)
                {
                    currentLength = sl;
                    target = e;
                }
            }

            // Move towards target
            velocity = (target.worldPosition - worldPosition);

            if (velocity.Length() < 50)
            {
                startAttack(time);
            }
            else
            {
                velocity.Normalize();
                velocity *= 2;
            }
        }
    }
}
