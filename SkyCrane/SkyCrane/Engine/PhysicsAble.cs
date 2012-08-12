using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SkyCrane.Engine
{
    public enum CollisionDirection { NONE, TOP, BOTTOM, LEFT, RIGHT, TOPRIGHT, BOTTOMRIGHT, BOTTOMLEFT, TOPLEFT };

    public interface PhysicsAble
    {
        Vector2 GetPhysicsSize();
        Vector2 GetPhysicsPosition();
        Vector2 GetPhysicsVelocity();

        void UpdatePhysics();
        CollisionDirection CheckCollision(PhysicsAble entity);
        void HandleCollision(CollisionDirection cd, PhysicsAble entity);
    }
}
