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
    class HostSettingsMenuScreen : MenuScreen
    {
        #region Initialization

        bool host;
        MenuEntry hostAddressMenuEntry;
        MenuEntry hostPortMenuEntry;

        static string hostAddress = ""; // Host's address (IP or hostname)
        static int hostPort = 9999; // Host's port

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        /// <param name="host">Whether or not this player is the host.</param>
        /// <param name="multiplayer">Whether or not this game is multiplayer.</param>
        public HostSettingsMenuScreen(bool host)
            : base(host ? "Host Game" : "Join Game")
        {
            this.host = host; // Is this player the host?

            if (!host) // If we're not the host, then we need to specify an address
            {
                hostAddressMenuEntry = new MenuEntry(string.Empty);
                hostAddressMenuEntry.Selected += HostAddressMenuEntrySelected;
                hostAddressMenuEntry.Typed += HostAddressMenuEntryTyped;
                MenuEntries.Add(hostAddressMenuEntry);
            }

            // Both the host and the client should specify ports
            hostPortMenuEntry = new MenuEntry(string.Empty);
            hostPortMenuEntry.Selected += HostPortMenuEntrySelected;            
            MenuEntries.Add(hostPortMenuEntry);
            
            SetMenuEntryText(); // Set the initial menu text

            // Add the back option
            MenuEntry back = new MenuEntry("Back");
            back.Selected += OnCancel;
            MenuEntries.Add(back);
            
            return;
        }

        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            if (!host) // Only non-hosts should need to enter an address
            {
                hostAddressMenuEntry.Text = "Host Address: " + hostAddress;
            }
            hostPortMenuEntry.Text = "Host Port: " + hostPort;
            return;
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void HostAddressMenuEntrySelected(object sender, PlayerInputEventArgs e)
        {
            TypingInput = true;
            return;
        }

        /// <summary>
        /// Event handler for typing this into the host address.
        /// </summary>
        void HostAddressMenuEntryTyped(object sender, PlayerInputEventArgs e)
        {
            if (e.KeysTyped != string.Empty)
            {
                hostAddress += e.KeysTyped;
                SetMenuEntryText();
            }
            return;
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void HostPortMenuEntrySelected(object sender, PlayerInputEventArgs e)
        {
            TypingInput = true;
            return;
        }

        #endregion
    }
}
