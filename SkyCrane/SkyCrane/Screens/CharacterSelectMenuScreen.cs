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
using System;
#endregion

namespace SkyCrane.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class CharacterSelectMenuScreen : MenuScreen
    {

        #region Fields

        // Game and player settings
        bool host;
        bool multiplayer;
        int numPlayers = 1;
        int[] characterSelections;
        bool[] characterSelectionsLocked;
        int playerNumber;

        // Textures used to draw characters and buttons
        Texture2D[] characters;
        Texture2D aButtonTextured2D;
        Texture2D bButtonTextured2D;
        Texture2D dPadLeftTexture2D;
        Texture2D dPadRightTexture2D;

        // Constants used for drawing output
        const int PLAYERS_PER_ROW = 2;
        const string selectMessage = "[ENTER] or";
        const string cancelMessage = "[ESC] or";
        const string openSlotMessage = "Slot Open";
        const string closedSlotMessage = "Slot Closed";
        const string startMessage = "Ready to Start! Press [ENTER] or";

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
            characters = new Texture2D[] { content.Load<Texture2D>("Sprites/Tank"),
                content.Load<Texture2D>("Sprites/Wizard"), content.Load<Texture2D>("Sprites/Rogue") };
            aButtonTextured2D = content.Load<Texture2D>("XBox Buttons/button_a");
            bButtonTextured2D = content.Load<Texture2D>("XBox Buttons/button_b");
            dPadLeftTexture2D = content.Load<Texture2D>("XBox Buttons/dpad_left");
            dPadRightTexture2D = content.Load<Texture2D>("XBox Buttons/dpad_right");
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

            int numRows = (int)Math.Ceiling((float)ProjectSkyCrane.MAX_PLAYERS / (float)PLAYERS_PER_ROW);
            float titleEnd = 80 + ScreenManager.Font.MeasureString(MenuTitle).Y;
            int spacePerRow = (graphics.PresentationParameters.BackBufferHeight - (int)titleEnd) / numRows;
            int spacePerColumn = graphics.PresentationParameters.BackBufferWidth / PLAYERS_PER_ROW;

            Color transitionColor = new Color(192, 192, 192) * TransitionAlpha;

            spriteBatch.Begin();

            // Calculate message sizes
            Vector2 selectMessageSize = ScreenManager.Font.MeasureString(selectMessage);
            Vector2 cancelMessageSize = ScreenManager.Font.MeasureString(cancelMessage);
            Vector2 startMessageSize = ScreenManager.Font.MeasureString(startMessage);

            for (int i = 0; i < numPlayers; i += 1) // Draw the individual characters
            {
                int row = i / PLAYERS_PER_ROW;
                int column = i % PLAYERS_PER_ROW;

                Color drawColor;
                if (characterSelectionsLocked[i]) // Shade out locked-in characters
                {
                    drawColor = Color.Gray;
                }
                else // Alpha transition
                {
                    drawColor = transitionColor;
                }
                int xBase = column * spacePerColumn;
                int yBase = (int)titleEnd + row * spacePerRow;
                int centerColumn = xBase + (spacePerColumn - 192) / 2;
                int centerRow = yBase + (spacePerRow - (192 + (int)selectMessageSize.Y)) / 2;
                spriteBatch.Draw(characters[characterSelections[i]], new Rectangle(centerColumn, centerRow, 192, 192),
                    null, drawColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                int selectBase = centerRow + 192;

                if (i == playerNumber)
                {
                    string playerName = "Player " + (i + 1) + ": ";
                    float playerNameSize = ScreenManager.Font.MeasureString(playerName).X;
                    int dPadBase = centerRow + (192 - 64) / 2;
                    
                    if (!characterSelectionsLocked[i]) // Draw the selection items
                    {
                        spriteBatch.Draw(dPadLeftTexture2D, new Rectangle(centerColumn - (64 + 8), dPadBase, 64, 64), drawColor);
                        spriteBatch.Draw(dPadRightTexture2D, new Rectangle(centerColumn + (192 + 8), dPadBase, 64, 64), drawColor);

                        float finalMessageBase = centerColumn + (192 - (playerNameSize + selectMessageSize.X + 64)) / 2;
                        spriteBatch.DrawString(ScreenManager.Font, playerName + selectMessage, new Vector2(finalMessageBase, selectBase), transitionColor);
                        spriteBatch.Draw(aButtonTextured2D, new Vector2(finalMessageBase + playerNameSize + selectMessageSize.X, selectBase - 8), drawColor);
                    }
                    else // Draw the cancel selection button
                    {
                        float finalMessageBase = centerColumn + (192 - (playerNameSize + cancelMessageSize.X + 64)) / 2;
                        spriteBatch.DrawString(ScreenManager.Font, playerName + cancelMessage, new Vector2(finalMessageBase, selectBase), transitionColor);
                        spriteBatch.Draw(bButtonTextured2D, new Vector2(finalMessageBase + playerNameSize + cancelMessageSize.X, selectBase - 8), drawColor);
                    }

                    if (host && AllLocked()) // Draw the "press to continue" message
                    {
                        spriteBatch.DrawString(ScreenManager.Font, startMessage,
                             new Vector2(graphics.PresentationParameters.BackBufferWidth - (startMessageSize.X + 64),
                             graphics.PresentationParameters.BackBufferHeight - startMessageSize.Y), transitionColor);
                        spriteBatch.Draw(aButtonTextured2D, new Vector2(graphics.PresentationParameters.BackBufferWidth - 64,
                            graphics.PresentationParameters.BackBufferHeight - (64 - 8)), transitionColor);
                    }
                }
                else // Draw the other player names
                {
                    string playerName = "Player " + (i + 1);
                    spriteBatch.DrawString(ScreenManager.Font, playerName, new Vector2(centerColumn + (192 - ScreenManager.Font.MeasureString(playerName).X) / 2, selectBase), transitionColor);
                }
            }

            Vector2 slotMessageSize; // Get the size of the redundant slot messages
            if (multiplayer) // Insert slot messages
            {
                slotMessageSize = ScreenManager.Font.MeasureString(openSlotMessage);
            }
            else
            {
                slotMessageSize = ScreenManager.Font.MeasureString(closedSlotMessage);
            }

            for (int i = numPlayers; i < ProjectSkyCrane.MAX_PLAYERS; i += 1) // Draw waiting messages for all remaining players
            {
                int row = i / PLAYERS_PER_ROW;
                int column = i % PLAYERS_PER_ROW;
                int xBase = column * spacePerColumn;
                int yBase = (int)titleEnd + row * spacePerRow;

                if (multiplayer) // Show that slots are open
                {
                    spriteBatch.DrawString(ScreenManager.Font, openSlotMessage,
                        new Vector2(xBase + (spacePerColumn - slotMessageSize.X) / 2,
                            yBase + (spacePerRow - slotMessageSize.Y) / 2), transitionColor);
                }
                else // Show that the other slots are closed
                {
                    spriteBatch.DrawString(ScreenManager.Font, closedSlotMessage,
                        new Vector2(xBase + (spacePerColumn - slotMessageSize.X) / 2,
                            yBase + (spacePerRow - slotMessageSize.Y) / 2), transitionColor);
                }
            }

            spriteBatch.End();
 	        base.Draw(gameTime);
            return;
        }

        #endregion
    }
}
