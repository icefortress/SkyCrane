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

        // Special cases that need to be tracked
        public Dictionary<int, DoctorWall> walls = new Dictionary<int, DoctorWall>();
        public List<int> healthBarWaiters = new List<int>();
        public Dictionary<int, HealthBar> healthBars = new Dictionary<int, HealthBar>();

        public bool isMoving = false;

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
                    c.type == StateChangeType.CREATE_PLAYER_CHARACTER ||
                    c.type == StateChangeType.CREATE_HEALTH_BAR)
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
                case PlayerCharacter.Type.JarCat:
                    pc = new JarCat(context, posX, posY);
                    break;
                default:
                    throw new ArgumentException();
            }

            // Add the new entity and create an appropriate state to accompany it
            addEntity(100, pc, pc.id);
            players.Add(pc);

            StateChange sc = StateChangeFactory.createEntityStateChange(pc.id, posX, posY, pc.frameWidth, pc.GetFrameTime(), pc.getDefaultTexture(), pc.scale, 100);
            changes.Add(sc);

            // Add a health bar to the player
            attachHealthWithStateChange(pc);

            return pc;
        }

        public void attachHealthWithStateChange(Entity e)
        {
            attachHealthBar(e);
            int hbid = healthBars[e.id].id;

            // Need a version of this for creating actual health bars
            StateChange sc = StateChangeFactory.createHealthBarStateChange(e.id, hbid);
            changes.Add(sc);
        }

        public void attachHealthBar(Entity e)
        {
            HealthBar h = new HealthBar(context, e);
            healthBars.Add(e.id, h);
            addEntity(300, h, h.id);
        }
         
        public Enemy createEnemy(int posX, int posY, Enemy.Type type)
        {
            Enemy e = null;
            switch (type)
            {
                case Enemy.Type.Goblin: // Create a smelly goblin
                    e = new Goblin(context, posX, posY);
                    break;
                case Enemy.Type.Skeleton: // Create a scary skeleton
                    e = new Skeleton(context, posX, posY);
                    break;
            }
            addEntity(100, e, e.id);

            StateChange sc = StateChangeFactory.createEntityStateChange(e.id, posX, posY, e.frameWidth, e.GetFrameTime(), e.getDefaultTexture(), e.scale, 100);
            changes.Add(sc);

            return e;
        }

        public void createBolt(int posX, int posY, Vector2 velocity)
        {
            CrossbowBolt b = new CrossbowBolt(context, new Vector2(posX, posY), velocity);
            addEntity(200, b, b.id);

            StateChange sc = StateChangeFactory.createEntityStateChange(b.id, posX, posY, CrossbowBolt.frameWidth, b.GetFrameTime(), CrossbowBolt.textureName, b.scale, 200);
            changes.Add(sc);
        }

        public void createLaser(int posX, int posY, Vector2 velocity)
        {
            Laser b = new Laser(context, new Vector2(posX, posY), velocity);
            addEntity(200, b, b.id);

            StateChange sc = StateChangeFactory.createEntityStateChange(b.id, posX, posY, Laser.frameWidth, b.GetFrameTime(), Laser.textureName, b.scale, 200);
            changes.Add(sc);
        }

        public void createBullet(int posX, int posY, Vector2 velocity)
        {
            Bullet b = new Bullet(context, new Vector2(posX, posY), velocity);
            addEntity(200, b, b.id);

            StateChange sc = StateChangeFactory.createEntityStateChange(b.id, posX, posY, Bullet.frameWidth, b.GetFrameTime(), Bullet.textureName, b.scale, 200);
            changes.Add(sc);
        }

        public void createMageAttack(int posX, int posY, Vector2 velocity)
        {
            MageAttack m = new MageAttack(context, new Vector2(posX, posY), velocity);
            addEntity(150, m, m.id);

            StateChange sc = StateChangeFactory.createEntityStateChange(m.id, posX, posY, MageAttack.frameWidth, m.GetFrameTime(), MageAttack.textureName, m.scale, 150);
            changes.Add(sc);
        }

        public void createRogueAttack(int posX, int posY, Vector2 velocity)
        {
            RealBullet m = new RealBullet(context, new Vector2(posX, posY), velocity);
            addEntity(150, m, m.id);

            StateChange sc = StateChangeFactory.createEntityStateChange(m.id, posX, posY, RealBullet.frameWidth, m.GetFrameTime(), RealBullet.textureName, m.scale, 150);
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
            addEntity(3, d, d.id);

            // Keep track of one wall per player
            walls[entity_id] = d;

            StateChange sc = StateChangeFactory.createEntityStateChange(d.id, posX, posY, DoctorWall.frameWidth, d.GetFrameTime(), DoctorWall.textureName, d.scale, 200);
            changes.Add(sc);
        }

        public void addEntity(int drawPriority, Entity e, int id)
        {
            // Hack to change the id to match the server
            e.id = id;

            entities.Add(id, e);
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

            // If there is a health bar for this entity, pass the event along
            if (healthBars.ContainsKey(entity))
            {
                healthBars[entity].handleStateChange(s);
            }


            if (s.type != StateChangeType.CREATE_ENTITY && !entities.ContainsKey(entity))
            {
                if (s.type == StateChangeType.SET_PLAYER) // Can't ignore this case, need to save the ID for when we get it
                {
                    usersEntity = entity;
                } else if (s.type == StateChangeType.CREATE_HEALTH_BAR) {
                    healthBarWaiters.Add(entity);
                }
                return; // Otherwise, we have to discard this event and wait until the guy shows up
            }
            
            if (s.type == StateChangeType.CREATE_HEALTH_BAR)
            {
                attachHealthBar(entities[entity]);
            }
            else if (s.type == StateChangeType.MOVED)
            {
                if (entity == usersEntity)
                {
                    isMoving = true;
                }

                int pos_x = s.intProperties[StateProperties.POSITION_X];
                int pos_y = s.intProperties[StateProperties.POSITION_Y];
                entities[entity].worldPosBack = new Vector2(pos_x, pos_y); // do a change without triggering a statechange
            }
            else if (s.type == StateChangeType.CREATE_ENTITY)
            {
                int pos_x = s.intProperties[StateProperties.POSITION_X];
                int pos_y = s.intProperties[StateProperties.POSITION_Y];
                int frame_width = s.intProperties[StateProperties.FRAME_WIDTH];
                int frame_time = s.intProperties[StateProperties.FRAME_TIME];
                int draw_priority = s.intProperties[StateProperties.DRAW_PRIORITY];
                String texture_name = s.stringProperties[StateProperties.SPRITE_NAME];
                float scale = (float)s.doubleProperties[StateProperties.SCALE];

                Entity e = new Entity(context, pos_x, pos_y, frame_width, frame_time, texture_name, scale);

                addEntity(draw_priority, e, e.id);

                // Set users player if it didn't exist yet
                if (e.id == usersEntity)
                {
                    usersPlayer = e;
                }

                // Attach health bar if we were waiting for this
                if(healthBarWaiters.Contains(e.id)) {
                    attachHealthBar(e);
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
