using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkyCrane.GameStateManager;
using SkyCrane.Screens;
using System.Collections.Generic;

namespace SkyCrane
{

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ProjectSkyCrane : Microsoft.Xna.Framework.Game
    {

        ScreenManager screenManager;

        /// <summary>
        /// The GraphicsDeviceManager currently associated with the game.
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return graphics; }
        }
        GraphicsDeviceManager graphics;

        /// <summary>
        /// Create the main instance of the project and run.
        /// </summary>
        public ProjectSkyCrane()
        {
            Content.RootDirectory = "Content";

            // Initialize the graphics manager
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            // Initialize the screen manager
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            // Activate a few initial screens
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);

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
                game.Run();
            }
        }

    }
}
