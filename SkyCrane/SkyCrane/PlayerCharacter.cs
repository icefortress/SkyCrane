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

        public static StateChange createPlayerStateChange(int posX, int posY, String textureName, String animationName)
        {
            StateChange sc = new StateChange();
            sc.type = StateChangeType.CREATE_PLAYER_CHARACTER;
            sc.intProperties.Add(StateProperties.POSITION_X, posX);
            sc.intProperties.Add(StateProperties.POSITION_Y, posY);
            sc.stringProperties.Add(StateProperties.SPRITE_NAME, textureName);
            sc.stringProperties.Add(StateProperties.ANIMATION_NAME, animationName);

            return sc;
        }

        public PlayerCharacter(GameplayScreen g, int posX, int posY, String textureName, String animationName) : base(g)
        {
            worldPosition = new Vector2(posX, posY);
            Texture2D chara = g.textureDict[textureName];

            List<int> animationFrames = new List<int>(); // TODO: some way of loading animation
            animationFrames.Add(0);
            InitDrawable(chara, chara.Width, chara.Height, animationFrames, 1, Color.White, 1, true);
            active = true;
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
