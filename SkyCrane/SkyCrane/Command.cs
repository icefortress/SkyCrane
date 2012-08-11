using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SkyCrane
{
    enum CommandType { MOVE }

    class Command
    {
        public int entity_id;
        public CommandType ct;
        public Vector2 direction;
    }
}
