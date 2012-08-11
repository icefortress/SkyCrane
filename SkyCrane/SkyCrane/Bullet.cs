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
        public static String animationName = "animationName";
        public new static int frameWidth = 30;

        public Bullet(GameplayScreen g, Vector2 position, Vector2 velocity) :
            base (g, (int)position.X, (int)position.Y, frameWidth, textureName, animationName)
        {
            this.worldPosBack = position; // Set position without sending update
            this.velocity = velocity;
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
