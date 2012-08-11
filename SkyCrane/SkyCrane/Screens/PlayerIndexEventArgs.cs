#region File Description
//-----------------------------------------------------------------------------
// PlayerIndexEventArgs.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace SkyCrane.Screens
{
    /// <summary>
    /// Custom event argument which includes the index of the player who
    /// triggered the event and toggle information. This is used by the MenuEntry.Selected event.
    /// </summary>
    class PlayerInputEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PlayerInputEventArgs(PlayerIndex playerIndex, int toggleDirection = 0, bool inputAccepted = false, bool inputCancelled = false, bool inputBackspace = false, String keysTyped = null)
        {
            this.playerIndex = playerIndex;
            this.toggleDirection = toggleDirection;
            this.inputAccepted = inputAccepted;
            this.inputCancelled = inputCancelled;
            this.inputBackspace = inputBackspace;
            this.keysTyped = keysTyped;
            return;
        }

        /// <summary>
        /// Gets the index of the player who triggered this event.
        /// </summary>
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
        }
        PlayerIndex playerIndex;

        /// <summary>
        /// Gets the direction the player was toggling during the event.
        /// </summary>
        public int ToggleDirection
        {
            get { return toggleDirection; }
        }
        int toggleDirection;

        /// <summary>
        /// Gets whether or not the input was accepted.
        /// </summary>
        public bool InputAccepted
        {
            get { return inputAccepted; }
        }
        bool inputAccepted;

        /// <summary>
        /// Gets whether or not the input was cancelled.
        /// </summary>
        public bool InputCancelled
        {
            get { return inputCancelled; }
        }
        bool inputCancelled;

        /// <summary>
        /// Gets whether or not the input was cancelled.
        /// </summary>
        public bool InputBackspace
        {
            get { return inputBackspace; }
        }
        bool inputBackspace;

        /// <summary>
        /// Get the input that was typed when this event was fired.
        /// </summary>
        public String KeysTyped
        {
            get { return keysTyped; }
        }
        String keysTyped;

    }
}
