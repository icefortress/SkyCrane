using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SkyCrane.Dudes;

namespace SkyCrane.Engine
{
    /// <summary>
    /// Spawn commands.
    /// </summary>
    public class SpawnCommand
    {
        public Vector2 pos; // Position to create the enemy in
        public Enemy.Type type; // Type of enemy to create
        public TimeSpan offset; // Offset after which to create the enemy

        /// <summary>
        /// Create a new SpawnCommand.
        /// </summary>
        public SpawnCommand(Vector2 pos, Enemy.Type type, TimeSpan offset)
        {
            this.pos = pos;
            this.type = type;
            this.offset = offset;
        }
    }
}
