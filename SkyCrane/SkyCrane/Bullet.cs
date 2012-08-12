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
        public static String textureName = "bullet";
        public new static int frameWidth = 20;
        public static Vector2 HITBOX_SIZE = new Vector2(30, 30);

        public Bullet(GameplayScreen g, Vector2 position, Vector2 velocity) :
            base(g, (int)position.X, (int)position.Y, frameWidth, textureName, textureName)
        {
            this.velocity = velocity;
            this.scale = 1;
        }

        public override string getDefaultTexture()
        {
            return textureName;
        }

        public override Vector2 getHitbox()
        {
            return HITBOX_SIZE;
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
