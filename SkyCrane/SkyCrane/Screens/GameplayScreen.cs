#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkyCrane.GameStateManager;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using SkyCrane.Dudes;
using SkyCrane.NetCode;
using SkyCrane.Engine;
#endregion

namespace SkyCrane.Screens
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        // Our game stuff
        public Vector2 viewPosition;

        public GameState gameState;

        // Server and player data
        public bool isServer;
        public bool isMultiplayer;
        public int numPlayers;
        public int playerId;
        public PlayerCharacter.Type[] characterSelections;
        List<int> playerEntityIds = new List<int>();
        Dictionary<int, ConnectionID> playerIdToConnectionHash; // Player id to connection id mapping for the host

        public Dictionary<int, int> serverIDLookup = new Dictionary<int, int>();

        public PlayerCharacter secondPlayer = null;

        bool canCreate = true;
        bool attackButtonOK = true;

        bool goodtogo = false;
        public bool bulletExists = false;

        public List<Command> commandBuffer = new List<Command>();
        public Dictionary<String, Texture2D> textureDict = new Dictionary<String, Texture2D>();

        float pauseAlpha;

        // References to client variables
        RawServer serverReference = null;
        RawClient clientReference = null;

        // Sound and music variables
        Song backGroundSong;
        SoundEffect pauseSoundEffect;
        SoundEffect doctorAttackSoundEffect;
        SoundEffect rogueAttackSoundEffect;
        SoundEffect tankAttackSoundEffect;
        SoundEffect wizardAttackSoundEffect;
        SoundEffect goblinAttackSoundEffect;
        SoundEffect skeletonAttackSoundEffect;

        // Host variables for spawning enemies
        int maxMinEnemiesSpawn; // Upper bound on minimum enemy spawn amount
        int minMinEnemiesSpawn; // Lower bound on minimum enemy spawn amount
        int maxMaxEnemiesSpawn; // Upper bound on maximum enemy spawn amount
        int minMaxEnemiesSpawn; // Lower bound on maximum enemy  spawn amount
        TimeSpan maxSpawnInterval; // Upper bound on enemy spawn time
        TimeSpan minSpawnInterval; // Lower bound on enemy spawn time
        int currentMaxEnemiesSpawn; // Current max amount of enemies allowed to spawn
        int currentMinEnemiesSpawn; // Current min amount of enemies allowed to spawn
        TimeSpan currentSpawnInterval; // Current interval between enemy spawns
        DateTime nextEnemySpawnTime; // The next time to spawn enemies
        int spawnMaxX; // Upper bound on spawn X coordinate
        int spawnMinX; // Lower bound on spawn X coordinate
        int spawnMaxY; // Upper bound on spawn Y coordinate
        int spawnMinY; // Lower bound on spawn Y coordinate
        Random spawnRandom; // Random generator used for spawning enemies

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(bool isServer, bool isMultiplayer, int numPlayers, int playerId,
            PlayerCharacter.Type[] characterSelections, Dictionary<int, ConnectionID> playerIdToConnectionHash)
        {

            // Set up server, multiplayer and character information
            this.isServer = isServer;
            this.isMultiplayer = isMultiplayer;
            this.numPlayers = numPlayers;
            this.characterSelections = characterSelections;
            this.playerIdToConnectionHash = playerIdToConnectionHash;

            // Set the screen transition times
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            // Set up the spawning rates and times
            maxMinEnemiesSpawn = 3 * numPlayers;
            minMinEnemiesSpawn = 1;
            maxMaxEnemiesSpawn = 4 * numPlayers;
            minMaxEnemiesSpawn = numPlayers;
            maxSpawnInterval = TimeSpan.FromSeconds(30.0);
            minSpawnInterval = TimeSpan.FromSeconds(20.0);
            currentSpawnInterval = maxSpawnInterval;
            currentMaxEnemiesSpawn = minMaxEnemiesSpawn;
            currentMinEnemiesSpawn = minMinEnemiesSpawn;
            nextEnemySpawnTime = DateTime.Now;
            spawnRandom = new Random();
            spawnMinX = 300;
            spawnMaxX = 1550;
            spawnMinY = 600;
            spawnMaxY = 1250;

            // Set the game state
            gameState = new GameState(this);

            return;
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            serverReference = ((ProjectSkyCrane)ScreenManager.Game).RawServer;
            clientReference = ((ProjectSkyCrane)ScreenManager.Game).RawClient;

            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            gameFont = content.Load<SpriteFont>("Fonts/gamefont");

            Texture2D testLevel = content.Load<Texture2D>("Sprites/Level");
            Texture2D testMap = content.Load<Texture2D>("Sprites/Level_CollisionMap_scaleDown");
            textureDict.Add("room2", testLevel);
            textureDict.Add("room2-collision-map", testMap);

            // Load characters
            Texture2D tankl = content.Load<Texture2D>("Sprites/Tank_Animated");
            Texture2D tankr = content.Load<Texture2D>("Sprites/Tank_Animated_Right");
            Texture2D tankal = content.Load<Texture2D>("Sprites/Tank_Attack");
            Texture2D tankar = content.Load<Texture2D>("Sprites/Tank_Attack_Right");

            Texture2D wizardl = content.Load<Texture2D>("Sprites/Wizard_Animated");
            Texture2D wizardr = content.Load<Texture2D>("Sprites/Wizard_Animated_Right");
            Texture2D wizardal = content.Load<Texture2D>("Sprites/Wizard_Attack");
            Texture2D wizardar = content.Load<Texture2D>("Sprites/Wizard_Attack_Right");

            Texture2D doctorl = content.Load<Texture2D>("Sprites/Doctor_Animated");
            Texture2D doctorr = content.Load<Texture2D>("Sprites/Doctor_Animated_Right");
            Texture2D doctoral = content.Load<Texture2D>("Sprites/Doctor_Attack");
            Texture2D doctorar = content.Load<Texture2D>("Sprites/Doctor_Attack_Right");

            Texture2D roguel = content.Load<Texture2D>("Sprites/Rogue_Animated");
            Texture2D roguer = content.Load<Texture2D>("Sprites/Rogue_Animated_Right");
            Texture2D rogueal = content.Load<Texture2D>("Sprites/Rogue_Attack");
            Texture2D roguear = content.Load<Texture2D>("Sprites/Rogue_Attack_Right");

            textureDict.Add("jarcatl", content.Load<Texture2D>("Sprites/JarCat_Animated"));
            textureDict.Add("jarcatr", content.Load<Texture2D>("Sprites/JarCat_Animated_Right"));
            textureDict.Add("jarcatal", content.Load<Texture2D>("Sprites/JarCat_Attack_Right"));
            textureDict.Add("jarcatar", content.Load<Texture2D>("Sprites/JarCat_Attack"));

            textureDict.Add("tankl", tankl);
            textureDict.Add("tankr", tankr);
            textureDict.Add("tankal", tankal);
            textureDict.Add("tankar", tankar);
            textureDict.Add("wizardl", wizardl);
            textureDict.Add("wizardr", wizardr);
            textureDict.Add("wizardal", wizardal);
            textureDict.Add("wizardar", wizardar);
            textureDict.Add("doctorl", doctorl);
            textureDict.Add("doctorr", doctorr);
            textureDict.Add("doctoral", doctoral);
            textureDict.Add("doctorar", doctorar);
            textureDict.Add("roguel", roguel);
            textureDict.Add("roguer", roguer);
            textureDict.Add("rogueal", rogueal);
            textureDict.Add("roguear", roguear);

            // Load enemies
            Texture2D skeletonl = content.Load<Texture2D>("Sprites/Skeleton_Animated");
            Texture2D skeletonr = content.Load<Texture2D>("Sprites/Skeleton_Animated_Right");
            Texture2D skeletonal = content.Load<Texture2D>("Sprites/Skeleton_Attack");
            Texture2D skeletonar = content.Load<Texture2D>("Sprites/Skeleton_Attack_Right");

            textureDict.Add("skeletonl", skeletonl);
            textureDict.Add("skeletonr", skeletonr);
            textureDict.Add("skeletonal", skeletonal);
            textureDict.Add("skeletonar", skeletonar);

            textureDict.Add("goblinl", content.Load<Texture2D>("Sprites/Goblin_Animated"));
            textureDict.Add("goblinr", content.Load<Texture2D>("Sprites/Goblin_Animated_Right"));
            textureDict.Add("goblinal", content.Load<Texture2D>("Sprites/Goblin_Attack"));
            textureDict.Add("goblinar", content.Load<Texture2D>("Sprites/Goblin_Attack_Right"));

            // Load thingamabobs
            Texture2D bullet = content.Load<Texture2D>("Sprites/Charge_Flying");
            textureDict.Add("bullet", bullet);
            Texture2D mageAttack = content.Load<Texture2D>("Sprites/Beam");
            textureDict.Add("wand", mageAttack);
            Texture2D doctorwall = content.Load<Texture2D>("Sprites/Doctor_Wall");
            textureDict.Add("doctorwall", doctorwall);
            Texture2D doctorwallh = content.Load<Texture2D>("Sprites/Doctor_Wall_Horizontal");
            textureDict.Add("doctorwallh", doctorwallh);
            Texture2D realbull = content.Load<Texture2D>("Sprites/TheRealBullet");
            textureDict.Add("realbull", realbull);
            textureDict.Add("bowbolt", content.Load<Texture2D>("Sprites/bowbolt"));
            textureDict.Add("healthbar", content.Load<Texture2D>("Sprites/HealthBar"));
            textureDict.Add("healthchunk", content.Load<Texture2D>("Sprites/HealthPoint"));
            textureDict.Add("laser", content.Load<Texture2D>("Sprites/Laser"));


            Level l = Level.generateLevel(this);
            gameState.currentLevel = l;
            gameState.addEntity(0, l, l.id);

            /*Enemy e = Enemy.createDefaultEnemy(this);
            this.addEntity(100, e);
            gameState.entities.Add(e.id, e);
            physicsAbles.Add(e);
            aiAbles.Add(e);*/

            // Set up the background music
            MediaPlayer.Stop();
            backGroundSong = content.Load<Song>("Music/soundtrack");
            MediaPlayer.Play(backGroundSong);

            // Set up any generic sound effects that will be needed
            pauseSoundEffect = content.Load<SoundEffect>("SoundFX/menu_cancel");
            doctorAttackSoundEffect = content.Load<SoundEffect>("SoundFX/doctor");
            rogueAttackSoundEffect = content.Load<SoundEffect>("SoundFX/rogue");
            tankAttackSoundEffect = content.Load<SoundEffect>("SoundFX/tank");
            wizardAttackSoundEffect = content.Load<SoundEffect>("SoundFX/wizard");
            goblinAttackSoundEffect = content.Load<SoundEffect>("SoundFX/goblin");
            skeletonAttackSoundEffect = content.Load<SoundEffect>("SoundFX/skeleton");

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
            return;
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        /// <summary>
        /// Play an attack sound.
        /// </summary>
        /// <param name="attacker">The attacker making a noise.</param>
        public void PlayAttackSound(AttackingDude attacker)
        {
            if (attacker is Doctor)
            {
                doctorAttackSoundEffect.Play();
            }
            else if (attacker is Rogue)
            {
                rogueAttackSoundEffect.Play();
            }
            else if (attacker is Tank)
            {
                tankAttackSoundEffect.Play();
            }
            else if (attacker is Wizard)
            {
                wizardAttackSoundEffect.Play();
            }
            else if (attacker is Goblin)
            {
                goblinAttackSoundEffect.Play();
            }
            else if (attacker is Skeleton)
            {
                skeletonAttackSoundEffect.Play();
            }
            return;
        }

        public void serverStartGame()
        {
            gameState.usersPlayer = gameState.createPlayer(1280 / 2, 720 / 2 + 50, characterSelections[0]);
            playerEntityIds.Add(gameState.usersPlayer.id);

            // TODO: delete this
            //secondPlayer = gameState.createPlayer(1280 / 2, 720 / 2 - 50, PlayerCharacter.Type.Tank);

            if (isMultiplayer)
            {
                // Create players and broadcast to the clients
                for (int i = 1; i < numPlayers; i++)
                {
                    PlayerCharacter pc = gameState.createPlayer(1280 / 2 + 20 * i, 720 / 2 + 50, characterSelections[i]);
                    playerEntityIds.Add(pc.id);

                    // send notification to player of their entity
                    StateChange sc = StateChangeFactory.createSetPlayerStateChange(pc.id);

                    List<StateChange> l = new List<StateChange>();
                    l.Add(sc);

                    serverReference.signalSC(l, playerIdToConnectionHash[i]);
                }
            }

            // Get the players from the server and send them each a notification of who the fuck theyare


            goodtogo = true;

        }

        #region Update and Draw


        // Move this to GameState at some point
        public void applyCommand(Command c, GameTime g)
        {
            if (c.ct == CommandType.MOVE)
            {
                Entity e = gameState.entities[c.entity_id];
                e.velocity = c.direction * 3;
            }
            else if (c.ct == CommandType.GOBLIN_ATTACK)
            {
                gameState.createBolt((int) c.position.X, (int)c.position.Y, c.direction * 6);
            }
            else if (c.ct == CommandType.SHOOT)
            {
                PlayerCharacter attacker = (PlayerCharacter)gameState.entities[c.entity_id];
                Vector2 vel = c.direction;
                if (vel == Vector2.Zero)
                {
                    if (attacker.facingLeft)
                    {
                        vel = new Vector2(-1, 0);
                    }
                    else
                    {
                        vel = new Vector2(1, 0);
                    }
                }

                Vector2 velocity = vel * 8;
                if (bulletExists)
                {
                    PlayerCharacter shooter = (PlayerCharacter)gameState.entities[c.entity_id];
                    shooter.fireBullet(velocity);
                }
                else
                {
                    Vector2 pos = c.position;
                    gameState.createBullet((int)pos.X, (int)pos.Y, velocity);
                    bulletExists = true;
                }
            }
            else if (c.ct == CommandType.ATTACK)
            {
                PlayerCharacter attacker = (PlayerCharacter)gameState.entities[c.entity_id];
                bool success = attacker.startAttack(g);

                Vector2 vel = c.direction;
                if (vel == Vector2.Zero)
                {
                    if (attacker.facingLeft)
                    {
                        vel = new Vector2(-1, 0);
                    }
                    else
                    {
                        vel = new Vector2(1, 0);
                    }
                }

                if (gameState.entities[c.entity_id] is Wizard)
                {
                    Vector2 pos = c.position;
                    if (success) gameState.createMageAttack((int)pos.X, (int)pos.Y, vel * 5);
                }
                else if (gameState.entities[c.entity_id] is Doctor)
                {
                    bool hor = false;
                    Vector2 offset;

                    Doctor d = (Doctor)gameState.entities[c.entity_id];

                    Vector2 pos = c.position;
                    if (!d.facingLeft)
                    {
                        offset = new Vector2(80, 0);
                    }
                    else
                    {
                        offset = new Vector2(-80, 0);
                    }


                    pos += offset;

                    if (success) gameState.createDoctorWall(c.entity_id, (int)pos.X, (int)pos.Y, hor);
                }
                else if (gameState.entities[c.entity_id] is Rogue)
                {
                    Vector2 pos = c.position + new Vector2(0, 15);
                    if (success) gameState.createRogueAttack((int)pos.X, (int)pos.Y, vel * 8);
                }
                else if (gameState.entities[c.entity_id] is JarCat)
                {
                    Vector2 pos = c.position - new Vector2(0, 30);
                    if (success) gameState.createLaser((int)pos.X, (int)pos.Y, vel * 5);
                }

            }
        }

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Make sure character information has been sent/received before doing anything for real
            if (isServer && !goodtogo)
            {
                serverStartGame();
            }
            else if (!isServer && gameState.usersPlayer == null)
            {
                List<StateChange> changes = clientReference.rcvUPD();
                gameState.applyAllStatechangs(changes);
                return;
            }

            // TODO: Add your update logic here
            viewPosition = gameState.currentLevel.getViewPosition(gameState.usersPlayer);

            if (isServer)
            {
                if (DateTime.Now > nextEnemySpawnTime) // Spawn new enemies
                {
                    int numEnemies = spawnRandom.Next(currentMinEnemiesSpawn, currentMaxEnemiesSpawn + 1);

                    for (int i = 0; i < numEnemies; i += 1)
                    {
                        Vector2 spawnCoordinates = gameState.currentLevel.scaleAbsoluteCoordinates(
                            new Vector2(spawnRandom.Next(spawnMinX, spawnMaxX + 1),
                            spawnRandom.Next(spawnMinY, spawnMaxY + 1)));
                            
                        gameState.createEnemy((int)spawnCoordinates.X, (int)spawnCoordinates.Y,
                            (Enemy.Type)spawnRandom.Next((int)Enemy.Type.Goblin, (int)Enemy.Type.Skeleton + 1));
                    }

                    if (currentMinEnemiesSpawn < maxMinEnemiesSpawn) // Increase min enemies
                    {
                        currentMinEnemiesSpawn += 1;
                    }
                    if (currentMaxEnemiesSpawn < maxMaxEnemiesSpawn) // Increase max enemies
                    {
                        currentMaxEnemiesSpawn += 1;
                    }
                    nextEnemySpawnTime += currentSpawnInterval;
                    if (currentSpawnInterval > minSpawnInterval) // Reduce spawn interval
                    {
                        currentSpawnInterval -= TimeSpan.FromSeconds(1.0);
                    }
                }

                // Apply own commands, and client's commands
                foreach (Command c in commandBuffer)
                {
                    applyCommand(c, gameTime);
                }
                commandBuffer.Clear(); // Important!

                // Apply commands from the client
                if (isMultiplayer && numPlayers > 1)
                {
                    List<Command> clientCommands = serverReference.getCMD();
                    foreach (Command c in clientCommands)
                    {
                        applyCommand(c, gameTime);
                    }
                }

                foreach (Entity e in gameState.entities.Values)
                {
                    if (e is AIable) ((AIable)e).UpdateAI(gameTime);
                    if (e is PhysicsAble) ((PhysicsAble)e).UpdatePhysics();
                }

                // Iterate over the physicsAbles to see if they are colliding with eachther
                foreach (Entity e in gameState.entities.Values)
                {
                    //if (e.velocity == Vector2.Zero) continue; // Things not moving won't check if they collide
                    if (!(e is PhysicsAble)) continue;
                    PhysicsAble p = (PhysicsAble) e;

                    foreach (Entity ee in gameState.entities.Values)
                    {
                        if (e == ee) continue;

                        if (!(ee is PhysicsAble)) continue;
                        PhysicsAble pp = (PhysicsAble)ee;

                        // We let the target decide if there's a collision. This will help with things like the Level special case
                        CollisionDirection cd = pp.CheckCollision(p);
                        if (cd != CollisionDirection.NONE) p.HandleCollision(cd, pp);
                    }
                }

                // Move objects by their post-check velocity
                foreach (Entity e in gameState.entities.Values)
                {
                    if (e.velocity != Vector2.Zero)
                    {
                        Vector2 old = e.worldPosition;
                        Vector2 newn = e.worldPosition + e.velocity;
                        e.worldPosition = newn;
                    } 
                }

                // Push changes to clients
                if (isMultiplayer && numPlayers > 1)
                {
                    //Console.WriteLine(gameState.changes.Count);
                    serverReference.broadcastSC(gameState.changes);
                }
                
                // Commit changes locally
                gameState.commitChanges();
                gameState.changes.Clear();
            }
            else
            {
                
                // Send our input to the server
                clientReference.sendCMD(commandBuffer);

                commandBuffer.Clear();

                // Flush our gamestatemanager changes, we don't trust ourselves
                gameState.changes.Clear();

                // Get changes from server
                List<StateChange> changes = clientReference.rcvUPD();

                // Apply all changes on the server
                foreach(StateChange sc in changes) {
                    gameState.applyStateChange(sc);
                }

            }

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
            {
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            }
            else
            {
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);
            }

            return;
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (gameState.usersPlayer == null)
            {
                return;
            }

            // Look up inputs for the active player profile.

            KeyboardState keyboardState = input.currentKeyboardState;
            GamePadState gamePadState = input.currentGamePadState;

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&  input.gamePadWasConnected;

            if (input.IsPauseGame() || gamePadDisconnected)
            {
                pauseSoundEffect.Play();
                ScreenManager.AddScreen(new PauseMenuScreen());
            }
            else
            {
                /* ===== COPY PASTE PLAYER 2 ====== */

                // Otherwise move the player position.
                /*Vector2 p2movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.A))
                    p2movement.X--;

                if (keyboardState.IsKeyDown(Keys.D))
                    p2movement.X++;

                if (keyboardState.IsKeyDown(Keys.W))
                    p2movement.Y--;

                if (keyboardState.IsKeyDown(Keys.S))
                    p2movement.Y++;

                if (p2movement.Length() > 1)
                    p2movement.Normalize();

                if (keyboardState.IsKeyDown(Keys.T))
                {
                    Command c = new Command();
                    c.entity_id = secondPlayer.id;
                    c.direction = p2movement;
                    c.ct = CommandType.SHOOT;
                    c.position = secondPlayer.worldPosition;
                    commandBuffer.Add(c);
                }

                Command c3 = new Command();
                c3.entity_id = secondPlayer.id;
                c3.direction = p2movement;
                c3.ct = CommandType.MOVE;
                commandBuffer.Add(c3);*/

                /* ===== COPY PASTE PLAYER 2 ====== */

                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (movement.Length() > 1)
                    movement.Normalize();

                if ((keyboardState.IsKeyDown(Keys.Z) || gamePadState.Triggers.Right > 0.5) && attackButtonOK)
                {
                    Command c = new Command();
                    c.entity_id = gameState.usersPlayer.id;
                    c.direction = movement;
                    c.ct = CommandType.SHOOT;
                    c.position = gameState.usersPlayer.worldPosition;
                    commandBuffer.Add(c);
                    //Console.WriteLine("Shoot");
                    attackButtonOK = false;
                }
                else if (keyboardState.IsKeyUp(Keys.Z) || gamePadState.Triggers.Right < 0.5)
                {
                    attackButtonOK = true;
                }

                if ((keyboardState.IsKeyDown(Keys.X) || gamePadState.Triggers.Left > 0.5) && attackButtonOK)
                {
                    Command c = new Command();
                    c.entity_id = gameState.usersPlayer.id;
                    c.ct = CommandType.ATTACK;
                    c.position = gameState.usersPlayer.worldPosition;
                    c.direction = movement;
                    commandBuffer.Add(c);
                    attackButtonOK = false;
                }
                else if (keyboardState.IsKeyUp(Keys.X) || gamePadState.Triggers.Left < 0.5)
                {
                    attackButtonOK = true;
                }

                if (keyboardState.IsKeyDown(Keys.R) && canCreate && isServer)
                {
                    gameState.createEnemy(1280 / 2, 720 / 2 + 200, Enemy.Type.Goblin);
                    canCreate = false;
                } else if (keyboardState.IsKeyUp(Keys.R)) {
                    canCreate = true;
                }

                //if (movement != Vector2.Zero || gameState.isMoving)
                //{
                        Command c2 = new Command();
                        c2.entity_id = gameState.usersPlayer.id;
                        c2.direction = movement;
                        c2.ct = CommandType.MOVE;
                        commandBuffer.Add(c2);
                        gameState.isMoving = false;
                //}
                
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            foreach(int k in gameState.drawLists.Keys) {
                foreach (Entity e in gameState.drawLists[k])
                {
                    e.Draw(gameTime, spriteBatch);
                }
            }

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


        #endregion
    }
}
