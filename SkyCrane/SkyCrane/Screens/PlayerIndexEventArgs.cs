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
        public PlayerInputEventArgs(PlayerIndex playerIndex, bool menuAccept = false, bool menuCancel = false, int toggleDirection = 0, bool typingAccepted = false, bool typingCancelled = false, bool typingBackspace = false, String keysTyped = null)
        {
            this.playerIndex = playerIndex;
            this.menuAccept = menuAccept;
            this.menuCancel = menuCancel;
            this.toggleDirection = toggleDirection;
            this.typingAccepted = typingAccepted;
            this.typingCancelled = typingCancelled;
            this.typingBackspace = typingBackspace;
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
        /// Gets whether or not the input was accepted.
        /// </summary>
        public bool MenuAccept
        {
            get { return menuAccept; }
        }
        bool menuAccept;

        /// <summary>
        /// Gets whether or not the input was accepted.
        /// </summary>
        public bool MenuCancel
        {
            get { return menuCancel; }
        }
        bool menuCancel;

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
        public bool TypingAccepted
        {
            get { return typingAccepted; }
        }
        bool typingAccepted;

        /// <summary>
        /// Gets whether or not the input was cancelled.
        /// </summary>
        public bool TypingCancelled
        {
            get { return typingCancelled; }
        }
        bool typingCancelled;

        /// <summary>
        /// Gets whether or not the input was cancelled.
        /// </summary>
        public bool TypingBackspace
        {
            get { return typingBackspace; }
        }
        bool typingBackspace;

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
