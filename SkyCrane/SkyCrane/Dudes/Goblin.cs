using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyCrane.Screens;
using Microsoft.Xna.Framework;
using SkyCrane.Engine;
using SkyCrane.NetCode;

namespace SkyCrane.Dudes
{
    class Goblin : Enemy
    {
        public static String TEXTURE_LEFT = "goblinl";
        public static String TEXTURE_RIGHT = "goblinr";
        public static String TEXTURE_ATTACK_LEFT = "goblinal";
        public static String TEXTURE_ATTACK_RIGHT = "goblinar";

        public static int FRAME_WIDTH = 90;
        public static int ATK_FRAME_WIDTH = 90;
        
        public Goblin(GameplayScreen g, int posX, int posY) :
            base(g, posX, posY, FRAME_WIDTH, ATK_FRAME_WIDTH,
            TEXTURE_LEFT, TEXTURE_RIGHT, TEXTURE_ATTACK_LEFT, TEXTURE_ATTACK_RIGHT)
        {
        }

        public override int getAttackCooldown()
        {
            return 2000;
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

            if (Math.Abs(velocity.Y) < 5)
            {
                if (base.startAttack(time))
                {
                    int vel = 1;
                    if (facingLeft) vel = -1;

                    Command c = new Command();
                    c.ct = CommandType.GOBLIN_ATTACK;
                    c.entity_id = id;
                    c.position = worldPosition;
                    c.direction = new Vector2(vel, 0);

                    context.commandBuffer.Add(c);
                }

            }

            if(Math.Abs(velocity.X) < 80) {
                velocity.X = 0;
            }

            velocity.Y *= 2; // Bias towards moving up and down
            velocity.Normalize();
            velocity *= 2;
        }
    }
}
