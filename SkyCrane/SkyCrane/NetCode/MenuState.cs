using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace SkyCrane.NetCode
{
    
    /// <summary>
    /// A packet representation of a state change in the CharacterSelectMenuScreen.
    /// </summary>
    public class MenuState : Marshable
    {

        /// <summary>
        /// The packet type.
        /// </summary>
        public enum Type
        {

            /// <summary>
            /// Initial connection packet to retrieve a player number.
            /// </summary>
            Connect = 0,

            /// <summary>
            /// Select a particular character on the menu screen.
            /// </summary>
            SelectCharacter,

            /// <summary>
            /// Lock a particular character on the menu screen.
            /// </summary>
            LockCharacter,

            /// <summary>
            /// Unlock a particular character on the menu screen.
            /// </summary>
            UnlockCharacter,

            /// <summary>
            /// Disconnect from the session.
            /// </summary>
            Disconnect

        }

        /// <summary>
        /// The type associated with this packet.
        /// </summary>
        public Type MenuType
        {
            get;
            private set;
        }

        /// <summary>
        /// The id of the player that performed the action.
        /// </summary>
        public int PlayerId
        {
            get;
            private set;
        }

        /// <summary>
        /// Get an array of bytes representing the packet.
        /// </summary>
        /// <returns>An array of bytes representing this packet state.</returns>
        public byte[] getPacketData()
        {
            throw new NotImplementedException();
        }
    }
        
}
