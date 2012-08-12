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

        public bool isServer;
        public bool isMultiplayer;
        public int numPlayers;
        public Dictionary<int, int> serverIDLookup = new Dictionary<int, int>();

        bool goodtogo = false;

        List<Command> commandBuffer = new List<Command>();
        public Dictionary<String, Texture2D> textureDict = new Dictionary<String, Texture2D>();

        float pauseAlpha;

        RawServer serverReference = null;
        RawClient clientReference = null;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(bool isServer, bool isMultiplayer, int numPlayers)
        {
            this.isServer = isServer;
            this.isMultiplayer = isMultiplayer;
            this.numPlayers = numPlayers;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            gameState = new GameState(this);

            if (isServer)
            {
            }
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            gameFont = content.Load<SpriteFont>("Fonts/gamefont");

            Texture2D testLevel = content.Load<Texture2D>("Levels/room3");
            Texture2D testMap = content.Load<Texture2D>("Levels/room3-collision_map");

            // Load characters
            Texture2D tankl = content.Load<Texture2D>("Sprites/Tank_Animated");
            Texture2D tankr = content.Load<Texture2D>("Sprites/Tank_Animated_Right");
            Texture2D tankal = content.Load<Texture2D>("Sprites/Tank_Attack");
            Texture2D tankar = content.Load<Texture2D>("Sprites/Tank_Attack_Right");

            Texture2D wizardl = content.Load<Texture2D>("Sprites/Wizard_Animated");
            Texture2D wizardr = content.Load<Texture2D>("Sprites/Wizard_Animated_Right");
            Texture2D wizardal = content.Load<Texture2D>("Sprites/Wizard_Attack");
            Texture2D wizardar = content.Load<Texture2D>("Sprites/Wizard_Attack_Right");
            
            
            textureDict.Add("room2", testLevel);
            textureDict.Add("room2-collision-map", testMap);
            textureDict.Add("tankl", tankl);
            textureDict.Add("tankr", tankr);
            textureDict.Add("tankal", tankal);
            textureDict.Add("tankar", tankar);
            textureDict.Add("wizardl", wizardl);
            textureDict.Add("wizardr", wizardr);
            textureDict.Add("wizardal", wizardal);
            textureDict.Add("wizardar", wizardar);

            Level l = Level.generateLevel(this);
            gameState.currentLevel = l;
            gameState.addEntity(0, l);

            /*Enemy e = Enemy.createDefaultEnemy(this);
            this.addEntity(100, e);
            gameState.entities.Add(e.id, e);
            physicsAbles.Add(e);
            aiAbles.Add(e);*/

            // Some test music
            MediaPlayer.Stop();
            Song bgMusic = content.Load<Song>("Music/Nero - Doomsday");
            MediaPlayer.Volume = 0.3f;
            MediaPlayer.Play(bgMusic);

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

        public void serverStartGame()
        {
            gameState.usersPlayer = gameState.createPlayer(1280 / 2, 720 / 2 + 50, 45, "wizard");

            if (isMultiplayer)
            {
                // Create players and broadcast to the clients
                List<int> playerIds = new List<int>();
                for (int i = 1; i < numPlayers; i++)
                {
                    PlayerCharacter pc = gameState.createPlayer(1280 / 2 + 20 * i, 720 / 2 + 50, 45, "tank");
                    playerIds.Add(pc.id);
                }
            }

            // Get the players from the server and send them each a notification of who the fuck theyare

            goodtogo = true;

        }

        public void clientStartGame()
        {
            // Keep pulling statechanges until we have our character set
            while (gameState.usersPlayer == null)
            {
                List<StateChange> changes = clientReference.rcvUPD();
                gameState.applyAllStatechangs(changes);
            }

            // then enter the game
        }

        #region Update and Draw


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
            if (!goodtogo)
            {
                if (isServer)
                {
                    serverStartGame();
                }
                else
                {
                    clientStartGame();
                }
            }

            // TODO: Add your update logic here
            viewPosition = gameState.currentLevel.getViewPosition(gameState.usersPlayer);

            if (isServer)
            {

                // Apply own commands, and client's commands
                foreach (Command c in commandBuffer)
                {
                    if (c.ct == CommandType.MOVE)
                    {
                        Entity e = gameState.entities[c.entity_id];
                        e.velocity = c.direction * 3;
                    }
                    else if (c.ct == CommandType.SHOOT)
                    {
                        Vector2 pos = c.position;
                        Vector2 velocity = c.direction * 8;

                        gameState.createBullet((int)pos.X, (int)pos.Y, velocity);
                    }
                    else if (c.ct == CommandType.ATTACK)
                    {
                        PlayerCharacter attacker = (PlayerCharacter)gameState.entities[c.entity_id];
                        attacker.startAttack(gameTime);
                    }
                }
                commandBuffer.Clear(); // Important!

                // Apply commands from the client
                if (isMultiplayer)
                {
                    List<Command> clientCommands = serverReference.getCMD();
                    foreach (Command c in clientCommands)
                    {
                        Entity e = gameState.entities[c.entity_id];
                        e.velocity = c.direction * 3;
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
                if(isMultiplayer) serverReference.broadcastSC(gameState.changes);
                
                // Commit changes locally
                gameState.commitChanges();
                gameState.changes.Clear();
            }
            else
            {
                
                // Send our input to the server
                clientReference.sendCMD(commandBuffer);

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
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.currentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.currentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.gamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
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

                if (keyboardState.IsKeyDown(Keys.P))
                {
                    Command c = new Command();
                    c.entity_id = gameState.usersPlayer.id;
                    c.direction = movement;
                    c.ct = CommandType.SHOOT;
                    c.position = gameState.usersPlayer.worldPosition;
                    commandBuffer.Add(c);
                }

                if (keyboardState.IsKeyDown(Keys.X))
                {
                    Command c = new Command();
                    c.entity_id = gameState.usersPlayer.id;
                    c.ct = CommandType.ATTACK;
                    commandBuffer.Add(c);
                }

                Command c2 = new Command();
                c2.entity_id = gameState.usersPlayer.id;
                c2.direction = movement;
                c2.ct = CommandType.MOVE;
                commandBuffer.Add(c2);
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
