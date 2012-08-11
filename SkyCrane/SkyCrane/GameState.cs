using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyCrane
{
    public class GameState
    {
        public GameState()
        {
        }

        public List<Entity> entities = new List<Entity>();
        public Level currentLevel;
    }
}
