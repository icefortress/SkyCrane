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
        public PlayerCharacter usersPlayer;

        public GameState gameState;

        public bool isServer = true;


        public SortedDictionary<int, List<Entity>> drawPriorityEntities = new SortedDictionary<int, List<Entity>>();

        public void addEntity(int drawPriority, Entity e)
        {
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

        public List<AIable> aiAbles = new List<AIable>();
        public List<PhysicsAble> physicsAbles = new List<PhysicsAble>();
        public Dictionary<String, Texture2D> textureDict = new Dictionary<String, Texture2D>();

        float pauseAlpha;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            gameState = new GameState();
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

            Texture2D testLevel = content.Load<Texture2D>("Levels/filback");
            Texture2D testMap = content.Load<Texture2D>("Levels/filmap");
            Texture2D testChar = content.Load<Texture2D>("Sprites/PinkWizard");
            
            textureDict.Add("testback", testLevel);
            textureDict.Add("testmap", testMap);
            textureDict.Add("testchar", testChar);

            Level l = Level.generateLevel(this);
            gameState.currentLevel = l;
            this.addEntity(0, l);
            gameState.entities.Add(l);
            physicsAbles.Add(l);

            usersPlayer = PlayerCharacter.createDefaultPlayerCharacter(this);
            this.addEntity(100, usersPlayer);
            gameState.entities.Add(usersPlayer);
            physicsAbles.Add(usersPlayer);

            Enemy e = Enemy.createDefaultEnemy(this);
            this.addEntity(100, e);
            gameState.entities.Add(e);
            physicsAbles.Add(e);
            aiAbles.Add(e);

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

            // TODO: Add your update logic here
            viewPosition = gameState.currentLevel.getViewPosition(usersPlayer);

            // Server has to update AI characters here
            if (isServer)
            {
                foreach (AIable a in aiAbles)
                {
                    a.UpdateAI(gameTime);
                }
            }

            foreach (PhysicsAble p in physicsAbles)
            {
                p.UpdatePhysics();
            }

            // Iterate over the physicsAbles to see if they are colliding with eachother
            foreach (PhysicsAble p in physicsAbles)
            {
                foreach(PhysicsAble pp in physicsAbles) {
                    if(p == pp) continue;

                    // We let the target decide if there's a collision. This will help with things like the Level special case
                    CollisionDirection cd = pp.CheckCollision(p);
                    if (cd != CollisionDirection.NONE) p.HandleCollision(cd, pp);
                }
            }

            // Move objects by their post-check velocity
            foreach (Entity e in gameState.entities)
            {
                e.worldPosition += e.velocity;
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

                usersPlayer.velocity = 3 * movement;
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

            foreach(int k in drawPriorityEntities.Keys) {
                foreach (Entity e in drawPriorityEntities[k])
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
