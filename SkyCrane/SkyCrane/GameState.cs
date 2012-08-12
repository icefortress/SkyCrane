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
        public Entity usersPlayer;
        public GameplayScreen context;

        public List<StateChange> changes = new List<StateChange>();

        public GameState(GameplayScreen g)
        {
            context = g;
        }

        public void handleStateChange(StateChange s)
        {
            changes.Add(s);
        }

        // This should only be called by the server
        public void commitChanges()
        {
            foreach(StateChange c in changes) {
                // Skip entity creation, this won't create fully functional entities!
                if (c.type == StateChangeType.CREATE_ENTITY ||
                    c.type == StateChangeType.CREATE_PLAYER_CHARACTER)
                {
                    continue;
                }

                applyStateChange(c);
            }
        }

        public Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        public SortedDictionary<int, List<Entity>> drawLists = new SortedDictionary<int, List<Entity>>();

        // Should be called by the server to create a player entity in the current game state
        public PlayerCharacter createPlayer(int posX, int posY, String type)
        {
            PlayerCharacter pc = null;
            if (type == "tank")
            {
                pc = new Tank(context, posX, posY);
            }
            else if (type == "wizard")
            {
                pc = new Wizard(context, posX, posY);
            }
            addEntity(100, pc);

            StateChange sc = Entity.createEntityStateChange(pc.id, posX, posY, pc.frameWidth, pc.getDefaultTexture());
            changes.Add(sc);

            return pc;
        }

        public Enemy createEnemy(int posX, int posY, int frameWidth, String type)
        {
            Enemy e = null;
            if (type == "skeleton")
            {
                e = new Skeleton(context, posX, posY);
            }
            addEntity(100, e);

            StateChange sc = Entity.createEntityStateChange(e.id, posX, posY, e.frameWidth, e.getDefaultTexture());
            changes.Add(sc);

            return e;
        }

        public void createBullet(int posX, int posY, Vector2 velocity)
        {
            Bullet b = new Bullet(context, new Vector2(posX, posY), velocity);
            addEntity(200, b);

            StateChange sc = Entity.createEntityStateChange(b.id, posX, posY, Bullet.frameWidth, Bullet.textureName);
            changes.Add(sc);
        }

        public void createMageAttack(int posX, int posY, Vector2 velocity)
        {
            MageAttack m = new MageAttack(context, new Vector2(posX, posY), velocity);
            addEntity(200, m);

            StateChange sc = Entity.createEntityStateChange(m.id, posX, posY, MageAttack.frameWidth, MageAttack.textureName);
            changes.Add(sc);
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

        public void applyAllStatechangs(List<StateChange> ss)
        {
            foreach (StateChange s in ss) applyStateChange(s);
        }

        // This should never create a state change
        public void applyStateChange(StateChange s)
        {
            int entity = s.intProperties[StateProperties.ENTITY_ID];
            if (!entities.ContainsKey(entity)) return; // Deal with ordering issues in a hacky way

            if(s.type == StateChangeType.MOVED) {
                int pos_x = s.intProperties[StateProperties.POSITION_X];
                int pos_y = s.intProperties[StateProperties.POSITION_Y];
                entities[entity].worldPosBack = new Vector2(pos_x, pos_y); // do a change without triggering a statechange
            }
            else if (s.type == StateChangeType.CREATE_ENTITY)
            {
                int pos_x = s.intProperties[StateProperties.POSITION_X];
                int pos_y = s.intProperties[StateProperties.POSITION_Y];
                int frame_width = s.intProperties[StateProperties.FRAME_WIDTH];
                int draw_priority = s.intProperties[StateProperties.DRAW_PRIORITY];
                String texture_name = s.stringProperties[StateProperties.SPRITE_NAME];

                Entity e = new Entity(context, pos_x, pos_y, frame_width, texture_name);
                e.id = entity;
                
                addEntity(draw_priority, e);
            }
            else if (s.type == StateChangeType.DELETE_ENTITY)
            {
                removeEntity(entity);
            }
            else if (s.type == StateChangeType.SET_PLAYER)
            {
                usersPlayer = entities[entity];
            }
            else if (s.type == StateChangeType.CHANGE_SPRITE)
            {
                int frame_width = s.intProperties[StateProperties.FRAME_WIDTH];
                String texture_name = s.stringProperties[StateProperties.SPRITE_NAME];

                entities[entity].changeAnimation(frame_width, texture_name);
            }
        }
        
    }
}
