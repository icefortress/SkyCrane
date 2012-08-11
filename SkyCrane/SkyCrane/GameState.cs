using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SkyCrane.Screens;

namespace SkyCrane
{
    public class GameState : StateChangeListener
    {
        public Level currentLevel;
        public PlayerCharacter usersPlayer;
        public GameplayScreen context;

        public List<StateChange> changes = new List<StateChange>();
        private List<StateChange> deletions = new List<StateChange>(); // The whole state change tracking mechanism is unclean now, and this is a hack to do deletion after iteration

        public GameState(GameplayScreen g)
        {
            context = g;
        }

        public Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        public SortedDictionary<int, List<Entity>> drawLists = new SortedDictionary<int, List<Entity>>();

        // Should be called by the server to create a player entity in the current game state
        public PlayerCharacter createPlayer(int posX, int posY, int frameWidth, String textureName, String animationName)
        {
            StateChange sc = Entity.createEntityStateChange(posX, posY, frameWidth, textureName, animationName);
            changes.Add(sc);

            PlayerCharacter pc = new PlayerCharacter(context, posX, posY, frameWidth, textureName, animationName);
            addEntity(100, pc);

            return pc;
        }

        public void createBullet(int posX, int posY, Vector2 velocity)
        {
            StateChange sc = Entity.createEntityStateChange(posX, posY, Bullet.frameWidth, Bullet.textureName, "poop");
            changes.Add(sc);

            Bullet b = new Bullet(context, new Vector2(posX, posY), velocity);
            addEntity(200, b);
        }

        public void addEntity(int drawPriority, Entity e)
        {
            entities.Add(e.id, e);
            e.slListeners.Add(this);

            if (!drawLists.ContainsKey(drawPriority))
            {
                drawLists.Add(drawPriority, new List<Entity>());
            }
            drawLists[drawPriority].Add(e);
        }

        // Could be more efficient, obviously
        public void removeEntity(Entity e)
        {
            entities.Remove(e.id);
            e.slListeners.Remove(this);

            foreach (int k in drawLists.Keys)
            {
                drawLists[k].Remove(e);
            }
        }

        public void removeEntity(int eid) {
            removeEntity(entities[eid]);
        }

        public void handleStateChange(StateChange s)
        {
            changes.Add(s);

            // If an entity kills itself, we have to handle that
            if (s.type == StateChangeType.DELETE_ENTITY)
            {
                deletions.Add(s);
            }
        }

        public void runDeletions()
        {
            foreach (StateChange s in deletions)
            {
                applyStateChange(s);
            }
        }

        public void applyStateChange(StateChange s)
        {
            if(s.type == StateChangeType.MOVED) {
                int entity = s.intProperties[StateProperties.ENTITY_ID];
                int pos_x = s.intProperties[StateProperties.POSITION_X];
                int pos_y = s.intProperties[StateProperties.POSITION_Y];
                entities[entity].worldPosition += new Vector2(pos_x, pos_y);
            }
            else if (s.type == StateChangeType.CREATE_ENTITY)
            {
                int entity = s.intProperties[StateProperties.ENTITY_ID];
                int pos_x = s.intProperties[StateProperties.POSITION_X];
                int pos_y = s.intProperties[StateProperties.POSITION_Y];
                int frame_width = s.intProperties[StateProperties.FRAME_WIDTH];
                int draw_priority = s.intProperties[StateProperties.DRAW_PRIORITY];
                String texture_name = s.stringProperties[StateProperties.SPRITE_NAME];
                String animation_name = s.stringProperties[StateProperties.ANIMATION_NAME];

                Entity e = new Entity(context, pos_x, pos_y, frame_width, texture_name, animation_name);
                e.id = entity;
                
                addEntity(draw_priority, e);
            }
            else if (s.type == StateChangeType.SET_PLAYER) {
                int entity = s.intProperties[StateProperties.ENTITY_ID];
                usersPlayer = (PlayerCharacter) entities[entity];
            }
            else if (s.type == StateChangeType.DELETE_ENTITY)
            {
                removeEntity(s.intProperties[StateProperties.ENTITY_ID]);
            }
        }
        
    }
}
