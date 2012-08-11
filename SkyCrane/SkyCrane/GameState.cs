using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyCrane
{
    public class GameState : StateChangeListener
    {
        public Level currentLevel;
        public List<StateChange> changes = new List<StateChange>();

        public GameState()
        {
        }

        public Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        public void addEntity(Entity e)
        {
            entities.Add(e.id, e);
        }

        public void handleStateChange(StateChange s)
        {
            changes.Add(s);
        }

        public void applyStateChange(StateChange s)
        {
            entities[s.entity_id].worldPosition = s.new_position;
        }
        
    }
}
