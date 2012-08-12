using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkyCrane.GameStateManager;
using SkyCrane.Screens;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using SkyCrane.NetCode;

namespace SkyCrane
{

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ProjectSkyCrane : Microsoft.Xna.Framework.Game
    {

        // The screen manager that will be used to keep track of all screens in the game
        ScreenManager screenManager;

        // Default important values
        public const int MAX_PLAYERS = 4;
        public const int INITIAL_WIDTH = 1280;
        public const int INITIAL_HEIGHT = 720;
        public const float INITIAL_VOLUME = 0.5f;
        public const bool INITIAL_VSYNC = false;

        /// <summary>
        /// The GraphicsDeviceManager currently associated with the game.
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return graphics; }
        }
        GraphicsDeviceManager graphics;

        /// <summary>
        /// The netcode client used by clients when connecting to a server.
        /// </summary>
        public RawClient RawClient
        {
            get;
            set;
        }

        /// <summary>
        /// The netcode server used when hosting games.
        /// </summary>
        public RawServer RawServer
        {
            get;
            set;
        }

        /// <summary>
        /// Create the main instance of the project and run.
        /// </summary>
        public ProjectSkyCrane()
        {
            Content.RootDirectory = "Content";

            // Initialize the graphics manager
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = INITIAL_WIDTH; // 1280x720 is the XBox 360 default
            graphics.PreferredBackBufferHeight = INITIAL_HEIGHT;
            graphics.SynchronizeWithVerticalRetrace = INITIAL_VSYNC; // Turn off vsync by default

            // Initialize the volume settings
            SoundEffect.MasterVolume = INITIAL_VOLUME;
            MediaPlayer.Volume = INITIAL_VOLUME;

            // Initialize the screen manager
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            // Activate a few initial screens
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);

            // Server and client data
            RawClient = null;
            RawServer = null;

            return;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            return;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            return;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            return;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            // Note: The vast majority of this will be captured on-screen
            //UpdateGameControls(gameTime);

            base.Update(gameTime);
            return;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // All the real drawing happens inside the screen manager component
            base.Draw(gameTime);
            return;
        }
    }

    /// <summary>
    /// Main entry point for the game. Removes the unnecessary "program" file.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ProjectSkyCrane game = new ProjectSkyCrane())
            {
                //NetTest t = new NetTest(9999);
                game.Run();
                //t.exit();
            }
        }
    }
}
