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

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        /// <param name="host">Whether or not this player is the host.</param>
        /// <param name="multiplayer">Whether or not this game is multiplayer.</param>
        public CharacterSelectMenuScreen(bool host, bool multiplayer)
            : base("Character Select")
        {
            this.host = host;
            this.multiplayer = true;

            // Create our menu entries.
            MenuEntry startGameMenuEntry = new MenuEntry("Start Game");
            MenuEntry backMenuEntry = new MenuEntry("Back");

            // Hook up menu event handlers.
            startGameMenuEntry.Selected += StartGameMenuEntrySelected;
            backMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(startGameMenuEntry);
            MenuEntries.Add(backMenuEntry);
            return;
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void StartGameMenuEntrySelected(object sender, PlayerInputEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new GameplayScreen(host, multiplayer, numPlayers));
            return;
        }

        #endregion
    }
}
