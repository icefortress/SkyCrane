using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SkyCrane.Screens;
using SkyCrane.NetCode;

namespace SkyCrane.Dudes
{
    public class DoctorWall : Dude
    {
        int bounces = 0;

        public static String textureName = "doctorwall";
        public static String textureNameH = "doctorwallh";
        public new static int frameWidth = 45;
        public static Vector2 HITBOX_VERT = new Vector2(45, 255);
        public static Vector2 HITBOX_HORZ = new Vector2(255, 45);

        bool horizontal = false;

        public DoctorWall(GameplayScreen g, Vector2 position, bool horizontal) :
            base(g, (int)position.X, (int)position.Y, frameWidth, textureName, textureName)
        {
            this.scale = 1;
            this.frameTime = 30;

            // Change parameters if actually horizontal
            this.horizontal = horizontal;
            if (horizontal)
            {
                StateChange sc = new StateChange();
                sc.type = StateChangeType.CHANGE_SPRITE;
                sc.intProperties.Add(StateProperties.ENTITY_ID, id);
                sc.intProperties.Add(StateProperties.FRAME_WIDTH, 255);
                sc.stringProperties.Add(StateProperties.SPRITE_NAME, textureNameH);

                notifyStateChangeListeners(sc);
            }
        }

        public override string getDefaultTexture()
        {
            return textureName;
        }

        public override Vector2 getHitbox()
        {
            if (horizontal)
            {
                return HITBOX_HORZ;
            }
            else
            {
                return HITBOX_VERT;
            }
        }
    }
}
