#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SkyCrane.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class CharacterSelectMenuScreen : MenuScreen
    {

        #region Fields

        bool host;
        bool multiplayer;
        int numPlayers = 1;
        Texture2D[] characters;
        int[] characterSelections;
        bool[] characterSelectionsLocked;
        int playerNumber;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        /// <param name="host">Whether or not this player is the host.</param>
        /// <param name="multiplayer">Whether or not this game is multiplayer.</param>
        public CharacterSelectMenuScreen(bool host, bool multiplayer, int playerNumber = -1)
            : base("Character Select", true)
        {
            this.host = host;
            this.multiplayer = multiplayer;

            // Create the single invisible menu entry
            MenuEntry startGameMenuEntry = new MenuEntry(string.Empty, true);
            startGameMenuEntry.Selected += StartGameMenuEntrySelected;
            MenuEntries.Add(startGameMenuEntry);

            // Set up the initial character selections
            characterSelections = new int[ProjectSkyCrane.MAX_PLAYERS];
            characterSelectionsLocked = new bool[ProjectSkyCrane.MAX_PLAYERS];
            for (int i = 0; i < ProjectSkyCrane.MAX_PLAYERS; i += 1)
            {
                characterSelections[i] = 0;
                characterSelectionsLocked[i] = false;
            }

            if (host) // Set up which player slot this person currently fills
            {
                this.playerNumber = 0;
            }
            else
            {
                this.playerNumber = playerNumber;
            }

            return;
        }

        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            characters = new Texture2D[] { content.Load<Texture2D>("Sprites/Wizard"),
                content.Load<Texture2D>("Sprites/PinkWizard") };
            base.LoadContent();
            return;
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// A player has hit cancel while browsing characters.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            if (!characterSelectionsLocked[playerNumber])
            {
                base.OnCancel(playerIndex);
            }
            else
            {
                characterSelectionsLocked[playerNumber] = false;
            }
            return;
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void StartGameMenuEntrySelected(object sender, PlayerInputEventArgs e)
        {
            if (e.MenuAccept) // Handle character selection and game starting
            {
                if (!characterSelectionsLocked[playerNumber])
                {
                    characterSelectionsLocked[playerNumber] = true;
                }
                else if (host && AllLocked())
                {
                    LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new GameplayScreen(host, multiplayer, numPlayers));
                }
            }
            else if (!characterSelectionsLocked[playerNumber]) // Do some toggling
            {
                characterSelections[playerNumber] += e.ToggleDirection;
                if (characterSelections[playerNumber] < 0)
                {
                    characterSelections[playerNumber] = characters.Length - 1;
                }
                else if (characterSelections[playerNumber] >= characters.Length)
                {
                    characterSelections[playerNumber] = 0;
                }
            }

            return;
        }

        /// <summary>
        /// Check to see if all available players have locked in their choices.
        /// </summary>
        private bool AllLocked()
        {
            for (int i = 0; i < numPlayers; i += 1)
            {
                if (!characterSelectionsLocked[i])
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Draw and Update

        /// <summary>
        /// Draw various graphical elements on the character select screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
            Vector2 titleSize = font.MeasureString(MenuTitle);

            spriteBatch.Begin();

            for (int i = 0; i < numPlayers; i += 1)
            {
                spriteBatch.Draw(characters[characterSelections[i]], new Rectangle((int)(titlePosition.X - titleSize.X / 2), (int)(titlePosition.Y + titleSize.Y), 32, 32), new Color(192, 192, 192) * TransitionAlpha);
            }

            spriteBatch.End();
 	        base.Draw(gameTime);
            return;
        }

        #endregion
    }
}
