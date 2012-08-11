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

        public Level currentLevel;
        public SortedDictionary<int, List<Entity>> drawPriorityEntities = new SortedDictionary<int, List<Entity>>();

        public void addEntity(int drawPriority, Entity e) {
            if (!drawPriorityEntities.ContainsKey(drawPriority))
            {
                drawPriorityEntities.Add(drawPriority, new List<Entity>());
            }
            drawPriorityEntities[drawPriority].Add(e);
        }

        // Could be more efficient, obviously
        public void removeEntity(Entity e)
        {
            foreach (int k in drawPriorityEntities.Keys)
            {
                drawPriorityEntities[k].Remove(e);
            }
        }
    }
}
