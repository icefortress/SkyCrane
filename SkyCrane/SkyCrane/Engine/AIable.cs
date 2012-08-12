using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SkyCrane.Engine
{
    public interface AIable
    {
        void UpdateAI(GameTime time);
    }
}
