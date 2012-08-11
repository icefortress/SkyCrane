using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SkyCrane
{
    public enum CollisionDirection { NONE, TOP, BOTTOM, LEFT, RIGHT };

    public interface PhysicsAble
    {
        CollisionDirection CheckCollision(Vector2 position, Rectangle bounds);
    }
}
