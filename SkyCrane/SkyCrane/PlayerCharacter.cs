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
        public static StateChange createPlayerStateChange(int posX, int posY, int frameWidth, String textureName, String animationName)
        {
            StateChange sc = new StateChange();
            sc.type = StateChangeType.CREATE_PLAYER_CHARACTER;
            sc.intProperties.Add(StateProperties.POSITION_X, posX);
            sc.intProperties.Add(StateProperties.POSITION_Y, posY);
            sc.intProperties.Add(StateProperties.FRAME_WIDTH, frameWidth);
            sc.stringProperties.Add(StateProperties.SPRITE_NAME, textureName);
            sc.stringProperties.Add(StateProperties.ANIMATION_NAME, animationName);

            return sc;
        }

        public PlayerCharacter(GameplayScreen g, int posX, int posY, int frameWidth, String textureLeft, String textureRight, String animationName) : base(g, posX, posY, frameWidth, textureLeft, textureRight, animationName)
        {
            scale = 2;
        }

        public override void HandleCollision(CollisionDirection cd, PhysicsAble entity)
        {
            velocity = Vector2.Zero;
        }

    }
}
