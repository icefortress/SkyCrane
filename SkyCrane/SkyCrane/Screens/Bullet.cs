using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SkyCrane.Screens
{
    class Bullet : Dude
    {
        public static String textureName = "testchar";
        public new static int frameWidth = 30;

        public Bullet(GameplayScreen g, Vector2 position, Vector2 velocity) : base (g)
        {
            this.worldPosBack = position; // Set position without sending update
            this.velocity = velocity;

            // sprite up
            Texture2D chara = g.textureDict[textureName];
            List<int> animationFrames = new List<int>(); // TODO: some way of loading animation
            for (int i = 0; i < chara.Width / frameWidth; i++)
            {
                animationFrames.Add(i);
            }
            InitDrawable(chara, frameWidth, chara.Height, animationFrames, 200, Color.White, 1, true);
            active = true;
        }

        public void LevelUp()
        {
        }

        public void LevelDown()
        {
        }

        public override void HandleCollision(CollisionDirection cd, PhysicsAble entity)
        {
            // Die if you hit a wall
            if (entity is Level)
            {
                StateChange sc = new StateChange();
                sc.type = StateChangeType.DELETE_ENTITY;
                sc.intProperties.Add(StateProperties.ENTITY_ID, id);
                notifyStateChangeListeners(sc);
            }
        }
    }
}
