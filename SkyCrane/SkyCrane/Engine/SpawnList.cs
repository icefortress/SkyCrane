using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SkyCrane.Engine
{
    public class SpawnCommand
    {
        public Vector2 pos;
        public String type;
        TimeSpan offset;

        public SpawnCommand(Vector2 pos, String type, TimeSpan offset)
        {

        }
    }

    public class SpawnList
    {
        public List<SpawnCommand> spawns = new List<SpawnCommand>();
    }

    public class OurSpawnList : SpawnList
    {
        public OurSpawnList()
        {

        }
    }
}
