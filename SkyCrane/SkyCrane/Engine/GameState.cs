using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SkyCrane.Dudes;
using SkyCrane.Screens;
using SkyCrane.NetCode;

namespace SkyCrane.Engine
{
    public class GameState : StateChangeListener
    {
        public Level currentLevel;
        public Entity usersPlayer;
        private int usersEntity = 0;

        public GameplayScreen context;

        public List<StateChange> changes = new List<StateChange>();
        public List<PlayerCharacter> players = new List<PlayerCharacter>();

        public Dictionary<int, DoctorWall> walls = new Dictionary<int, DoctorWall>();

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
        public PlayerCharacter createPlayer(int posX, int posY, PlayerCharacter.Type type)
        {
            PlayerCharacter pc;
            switch (type) // Create a new character based on the type
            {
                case PlayerCharacter.Type.Doctor:
                    pc = new Doctor(context, posX, posY);
                    break;
                case PlayerCharacter.Type.Rogue:
                    pc = new Rogue(context, posX, posY);
                    break;
                case PlayerCharacter.Type.Tank:
                    pc = new Tank(context, posX, posY);
                    break;
                case PlayerCharacter.Type.Wizard:
                    pc = new Wizard(context, posX, posY);
                    break;
                default:
                    throw new ArgumentException();
            }

            // Add the new entity and create an appropriate state to accompany it
            addEntity(100, pc);
            players.Add(pc);

            StateChange sc = Entity.createEntityStateChange(pc.id, posX, posY, pc.frameWidth, pc.getDefaultTexture(), pc.scale, 100);
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

            StateChange sc = Entity.createEntityStateChange(e.id, posX, posY, e.frameWidth, e.getDefaultTexture(), e.scale, 100);
            changes.Add(sc);

            return e;
        }

        public void createBullet(int posX, int posY, Vector2 velocity)
        {
            Bullet b = new Bullet(context, new Vector2(posX, posY), velocity);
            addEntity(200, b);

            StateChange sc = Entity.createEntityStateChange(b.id, posX, posY, Bullet.frameWidth, Bullet.textureName, b.scale, 200);
            changes.Add(sc);
        }

        public void createMageAttack(int posX, int posY, Vector2 velocity)
        {
            MageAttack m = new MageAttack(context, new Vector2(posX, posY), velocity);
            addEntity(150, m);

            StateChange sc = Entity.createEntityStateChange(m.id, posX, posY, MageAttack.frameWidth, MageAttack.textureName, m.scale, 150);
            changes.Add(sc);
        }

        public void createRogueAttack(int posX, int posY, Vector2 velocity)
        {
            RealBullet m = new RealBullet(context, new Vector2(posX, posY), velocity);
            addEntity(150, m);

            StateChange sc = Entity.createEntityStateChange(m.id, posX, posY, RealBullet.frameWidth, RealBullet.textureName, m.scale, 150);
            changes.Add(sc);
        }

        public void createDoctorWall(int entity_id, int posX, int posY, bool horizontal)
        {
            // Delete the player's old wall
            if (walls.ContainsKey(entity_id))
            {
                walls[entity_id].destroy();
            }

            DoctorWall d = new DoctorWall(context, new Vector2(posX, posY), horizontal);
            addEntity(200, d);

            // Keep track of one wall per player
            walls[entity_id] = d;

            StateChange sc = Entity.createEntityStateChange(d.id, posX, posY, DoctorWall.frameWidth, DoctorWall.textureName, d.scale, 200);
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
            if (s.type != StateChangeType.CREATE_ENTITY && !entities.ContainsKey(entity))
            {
                if (s.type == StateChangeType.SET_PLAYER) // Can't ignore this case, need to save the ID for when we get it
                {
                    usersEntity = entity;
                }
                return;
            }

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
                float scale = (float)s.doubleProperties[StateProperties.SCALE];

                Entity e = new Entity(context, pos_x, pos_y, frame_width, texture_name, scale);
                e.id = entity;
                
                addEntity(draw_priority, e);

                if (e.id == usersEntity)
                {
                    usersPlayer = e;
                }
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
            else if (s.type == StateChangeType.CHANGE_SCALE)
            {
                float lscale = (float)s.doubleProperties[StateProperties.SCALE];
                entities[entity].scaleBack = lscale;
            }
        }
        
    }
}
