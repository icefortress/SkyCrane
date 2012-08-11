using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkyCrane.Screens;

namespace SkyCrane
{
    public class PlayerCharacter : Dude
    {

        public static PlayerCharacter createDefaultPlayerCharacter(GameplayScreen g)
        {
            PlayerCharacter pc = new PlayerCharacter(g);
            pc.worldPosition = new Vector2(1280 / 2, 720 / 2 + 100);
            Texture2D chara = g.textureDict["testchar"];
            List<int> animationFrames= new List<int>();
            animationFrames.Add(0);
            pc.InitDrawable(chara, chara.Width, chara.Height, animationFrames, 1, Color.White, 1, true);
            pc.active = true;

            return pc;
        }

        public PlayerCharacter(GameplayScreen g) : base(g)
        {
        }

        public void HandleCollision(CollisionDirection cd, PhysicsAble entity)
        {
            velocity = Vector2.Zero;
        }

    }
}
