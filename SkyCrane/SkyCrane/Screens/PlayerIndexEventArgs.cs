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
    class PlayerIndexEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PlayerIndexEventArgs(PlayerIndex playerIndex, int toggleDirection )
        {
            this.playerIndex = playerIndex;
            this.toggleDirection = toggleDirection;
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

    }
}
