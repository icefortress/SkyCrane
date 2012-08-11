using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SkyCrane
{
    enum CommandType { MOVE, SHOOT }

    class Command
    {
        public int entity_id;
        public CommandType ct;
        public Vector2 position;
        public Vector2 direction;
    }
}
