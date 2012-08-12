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
using Microsoft.Xna.Framework.Audio;
using SkyCrane.Dudes;
using SkyCrane.NetCode;
using System.Collections.Generic;
#endregion

namespace SkyCrane.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class CharacterSelectMenuScreen : MenuScreen
    {

        // TODO: Pretend that id's are nicely handled all over the place until I actually handle them niceley later after sleep
        // TODO: Prevent spoofing to clients if necessary

        #region Fields

        // Game and player settings
        bool host;
        bool multiplayer;
        PlayerCharacter.Type[] characterSelections;
        bool[] characterSelectionsLocked;
        bool[] playersConnected;
        int playerId;
        Queue<int> hostIds; // Ids the host can give away
        Dictionary<int, int> connectionToPlayerIdHash; // Connection id to player id mapping
        Dictionary<int, ConnectionID> playerIdToConnectionHash; // Player id to connection id mapping

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
        public CharacterSelectMenuScreen(bool host, bool multiplayer)
            : base("Character Select", true)
        {
            this.host = host;
            this.multiplayer = multiplayer;

            // Create the single invisible menu entry
            MenuEntry startGameMenuEntry = new MenuEntry(string.Empty, true);
            startGameMenuEntry.Selected += StartGameMenuEntrySelected;
            MenuEntries.Add(startGameMenuEntry);

            // Set up the initial character selections
            characterSelections = new PlayerCharacter.Type[ProjectSkyCrane.MAX_PLAYERS];
            characterSelectionsLocked = new bool[ProjectSkyCrane.MAX_PLAYERS];
            playersConnected = new bool[ProjectSkyCrane.MAX_PLAYERS];
            for (int i = 0; i < ProjectSkyCrane.MAX_PLAYERS; i += 1)
            {
                characterSelections[i] = 0;
                characterSelectionsLocked[i] = false;
                playersConnected[i] = false;
            }

            if (host) // Set up which player slot this person currently fills
            {
                playerId = 0;
                hostIds = new Queue<int>(ProjectSkyCrane.MAX_PLAYERS - 1);
                connectionToPlayerIdHash = new Dictionary<int, int>(ProjectSkyCrane.MAX_PLAYERS - 1);
                playerIdToConnectionHash = new Dictionary<int, ConnectionID>(ProjectSkyCrane.MAX_PLAYERS - 1);
                for (int i = 1; i < ProjectSkyCrane.MAX_PLAYERS; i += 1) // Serve up id's
                {
                    hostIds.Enqueue(i);
                }
                playersConnected[0] = true;
            }
            else
            {
                playerId = -1;
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
            if (!host && playerId < 0) // If we're not the host, ask for an id
            {
                MenuState connectPacket = new MenuState(MenuState.Type.Connect, 0, (int)MenuState.ConnectionDetails.IdReqest);
                ((ProjectSkyCrane)ScreenManager.Game).RawClient.sendMSC(connectPacket);
                playerId = -1;
            }

            ContentManager content = ScreenManager.Game.Content;

            // Dynamically load character types based on enum
            Array characterTypes = Enum.GetValues(typeof(PlayerCharacter.Type));
            characters = new Texture2D[characterTypes.Length];
            for (int i = 0; i < characters.Length; i += 1)
            {
                characters[i] = content.Load<Texture2D>("Sprites/" + characterTypes.GetValue(i));
            }

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
        protected override void OnCancel()
        {
            if (!characterSelectionsLocked[playerId])
            {
                if (multiplayer) // Only broadcast in multiplayer mode
                {
                    playersConnected[playerId] = false;
                    if (host) // Broadcast disconnect of host to all players
                    {
                        HostBroadcastConnected();
                    }
                    else // Inform the host of a disconnect
                    {
                        MenuState disconnectPacket = new MenuState(MenuState.Type.Connect, playerId, (int)MenuState.ConnectionDetails.Disconnected);
                        ((ProjectSkyCrane)ScreenManager.Game).RawClient.sendMSC(disconnectPacket);
                    }
                }
                base.OnCancel();
            }
            else // Unlock a character selection
            {
                characterSelectionsLocked[playerId] = false;
                if (multiplayer) // Only broadcast in multiplayer mode
                {
                    if (host) // Broadcast lock changes to all players
                    {
                        HostBroadcastLocks();
                    }
                    else // Inform the host of a lock change
                    {
                        MenuState unlockPacket = new MenuState(MenuState.Type.LockCharacter, playerId, (int)MenuState.LockCharacterDetails.Unlocked);
                        ((ProjectSkyCrane)ScreenManager.Game).RawClient.sendMSC(unlockPacket);
                    }
                }
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
                if (!characterSelectionsLocked[playerId]) // Unlock a player
                {
                    characterSelectionsLocked[playerId] = true;
                    if (multiplayer) // Only broadcast in multiplayer mode
                    {
                        if (host) // Broadcast lock changes to all players
                        {
                            HostBroadcastLocks();
                        }
                        else // Inform the host of a lock change
                        {
                            MenuState lockPacket = new MenuState(MenuState.Type.LockCharacter, playerId, (int)MenuState.LockCharacterDetails.Locked);
                            ((ProjectSkyCrane)ScreenManager.Game).RawClient.sendMSC(lockPacket);
                        }
                    }
                }
                else if (host && AllLocked())
                {
                    if (multiplayer) // Only broadcast in multiplayer mode
                    {
                        HostBroadcastGameStart();
                    }
                    LoadingScreen.Load(ScreenManager, false, new GameplayScreen(host, multiplayer, NumConnectedPlayers(), playerId, characterSelections, playerIdToConnectionHash));
                }
                menuSelectSoundEffect.Play();
            }
            else if (!characterSelectionsLocked[playerId] && e.ToggleDirection != 0) // Do some toggling of character selections
            {
                characterSelections[playerId] += e.ToggleDirection;
                if (characterSelections[playerId] < 0)
                {
                    characterSelections[playerId] = (PlayerCharacter.Type)(characters.Length - 1);
                }
                else if ((int)characterSelections[playerId] >= characters.Length)
                {
                    characterSelections[playerId] = 0;
                }

                if (multiplayer) // Only broadcast in multiplayer mode
                {
                    if (host) // Broadcast sprite changes to all players
                    {
                        HostBroadcastSprites();
                    }
                    else // Inform the host of a sprite change
                    {
                        MenuState spritePacket = new MenuState(MenuState.Type.SelectCharacter, playerId, (int)characterSelections[playerId]);
                        ((ProjectSkyCrane)ScreenManager.Game).RawClient.sendMSC(spritePacket);
                    }
                }

                menuScrollSoundEffect.Play();
            }

            return;
        }

        /// <summary>
        /// Check to see if all available players have locked in their choices.
        /// </summary>
        private bool AllLocked()
        {
            for (int i = 0; i < ProjectSkyCrane.MAX_PLAYERS; i += 1)
            {
                if (playersConnected[i] && !characterSelectionsLocked[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get the number of connected players.
        /// </summary>
        private int NumConnectedPlayers()
        {
            int numPlayers = 0;
            for (int i = 0; i < playersConnected.Length; i += 1)
            {
                if (playersConnected[i])
                {
                    numPlayers += 1;
                }
            }
            return numPlayers;
        }

        /// <summary>
        /// Have the host broadcast which players are and aren't connected.
        /// </summary>
        private void HostBroadcastConnected()
        {
            MenuState connectBroadcast = new MenuState(MenuState.Type.Connect);
            for (int i = 0; i < playersConnected.Length; i += 1)
            {
                connectBroadcast.PlayerId = i;
                if (playersConnected[i]) // Broadcast 1 to show connected
                {
                    connectBroadcast.EventDetail = (int)MenuState.ConnectionDetails.Connected;
                }
                else // Broadcast -1 to show not connected
                {
                    connectBroadcast.EventDetail = (int)MenuState.ConnectionDetails.Disconnected;
                }
                ((ProjectSkyCrane)ScreenManager.Game).RawServer.broadcastMSC(connectBroadcast);
            }
            return;
        }

        /// <summary>
        /// Have the host broadcast which characters are and aren't connected.
        /// </summary>
        private void HostBroadcastSprites()
        {
            MenuState spriteBroadcast = new MenuState(MenuState.Type.SelectCharacter);
            for (int i = 0; i < playersConnected.Length; i += 1) // Loop over and broadcast the sprites of all connected players
            {
                if (playersConnected[i])
                {
                    spriteBroadcast.PlayerId = i;
                    spriteBroadcast.EventDetail = (int)characterSelections[i];
                    ((ProjectSkyCrane)ScreenManager.Game).RawServer.broadcastMSC(spriteBroadcast);
                }                
            }
            return;
        }

        /// <summary>
        /// Have the host broadcast which characters are and aren't locked.
        /// </summary>
        private void HostBroadcastLocks()
        {
            MenuState lockedBroadcast = new MenuState(MenuState.Type.LockCharacter);
            for (int i = 0; i < playersConnected.Length; i += 1) // Loop over and broadcast the locks of all connected players
            {
                if (playersConnected[i])
                {
                    lockedBroadcast.PlayerId = i;
                    if (characterSelectionsLocked[i])
                    {
                        lockedBroadcast.EventDetail = (int)MenuState.LockCharacterDetails.Locked;
                    }
                    else
                    {
                        lockedBroadcast.EventDetail = (int)MenuState.LockCharacterDetails.Unlocked;
                    }
                    ((ProjectSkyCrane)ScreenManager.Game).RawServer.broadcastMSC(lockedBroadcast);
                }
            }
            return;
        }

        /// <summary>
        /// Have the host broadcast that the game has started.
        /// </summary>
        private void HostBroadcastGameStart()
        {
            MenuState startBroadcast = new MenuState(MenuState.Type.GameStart);
            for (int i = 0; i < playersConnected.Length; i += 1) // Loop over and inform everyone the game is beginning
            {
                if (playersConnected[i])
                {
                    ((ProjectSkyCrane)ScreenManager.Game).RawServer.broadcastMSC(startBroadcast);
                }
            }
            return;
        }


        #endregion

        #region Draw and Update

        /// <summary>
        /// Run a regular update loop on the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (multiplayer) // Don't handle these cases in single player
            {
                if (host) // Have the host search for incoming client packets and respond to them
                {
                    List<Tuple<ConnectionID, MenuState>> serverStates = ((ProjectSkyCrane)ScreenManager.Game).RawServer.getMSC();
                    for (int i = 0; i < serverStates.Count; i++)
                    {
                        switch (serverStates[i].Item2.MenuType)
                        {
                            case MenuState.Type.Connect: // Deal with connection and disconnections from the server
                                if (serverStates[i].Item2.EventDetail == (int)MenuState.ConnectionDetails.IdReqest) // Someone is requesting an id
                                {
                                    int newId;
                                    if (connectionToPlayerIdHash.ContainsKey(serverStates[i].Item1.ID)) // We've already seen this player and they haven't disconnected, resend their current id
                                    {
                                        newId = connectionToPlayerIdHash[serverStates[i].Item1.ID];
                                    }
                                    else // Send the player a new id
                                    {
                                        newId = hostIds.Dequeue();
                                        playersConnected[newId] = true;
                                        connectionToPlayerIdHash.Add(serverStates[i].Item1.ID, newId);
                                        playerIdToConnectionHash.Add(newId, serverStates[i].Item1);
                                    }
                                    MenuState connectResponse = new MenuState(MenuState.Type.Connect, newId, (int)MenuState.ConnectionDetails.IdReqest); // Inform connector of their Id
                                    ((ProjectSkyCrane)ScreenManager.Game).RawServer.signalMSC(connectResponse, serverStates[i].Item1);
                                    HostBroadcastConnected();
                                    HostBroadcastSprites();
                                    HostBroadcastLocks();
                                }
                                else if (serverStates[i].Item2.EventDetail == (int)MenuState.ConnectionDetails.Disconnected) // Someone is disconnecting
                                {
                                    if (connectionToPlayerIdHash.ContainsKey(serverStates[i].Item1.ID)) // This is a known player based on their connection
                                    {
                                        int requestingId = connectionToPlayerIdHash[serverStates[i].Item1.ID];
                                        if (requestingId == serverStates[i].Item2.PlayerId) // Prevent spoofing and sillyness
                                        {
                                            playersConnected[requestingId] = false;
                                            connectionToPlayerIdHash.Remove(serverStates[i].Item1.ID); // Remove connections from both hashes
                                            playerIdToConnectionHash.Remove(requestingId);
                                            hostIds.Enqueue(requestingId); // Allow the id to be re-used
                                            HostBroadcastConnected();
                                        }
                                    }
                                }
                                break;
                            case MenuState.Type.SelectCharacter:
                                if (connectionToPlayerIdHash.ContainsKey(serverStates[i].Item1.ID)) // This is a known player based on their connection
                                {
                                    int requestingId = connectionToPlayerIdHash[serverStates[i].Item1.ID];
                                    if (requestingId == serverStates[i].Item2.PlayerId) // Prevent spoofing and sillyness
                                    {
                                        characterSelections[requestingId] = (PlayerCharacter.Type)serverStates[i].Item2.EventDetail;
                                        HostBroadcastSprites();
                                    }
                                }
                                break;
                            case MenuState.Type.LockCharacter:
                                if (connectionToPlayerIdHash.ContainsKey(serverStates[i].Item1.ID)) // Check for a known player based on connection
                                {
                                    int requestingId = connectionToPlayerIdHash[serverStates[i].Item1.ID];
                                    if (requestingId == serverStates[i].Item2.PlayerId) // Prevent spoofing and sillyness
                                    {
                                        if (serverStates[i].Item2.EventDetail == (int)MenuState.LockCharacterDetails.Locked) // Locking a character in
                                        {
                                            characterSelectionsLocked[requestingId] = true;
                                        }
                                        else if (serverStates[i].Item2.EventDetail == (int)MenuState.LockCharacterDetails.Unlocked) // Unlocking a character
                                        {
                                            characterSelectionsLocked[requestingId] = false;
                                        }
                                        HostBroadcastLocks();
                                    }
                                }
                                break;
                            case MenuState.Type.GameStart: // Don't do anything in this case, it's clearly a silly request
                                break;
                            default:
                                throw new ArgumentException();
                        }
                    }
                }
                else // Handle authoritative updates from the server if we are a client
                {
                    List<MenuState> clientStates = ((ProjectSkyCrane)ScreenManager.Game).RawClient.rcvMenuState();
                    for (int i = 0; i < clientStates.Count; i++)
                    {
                        switch (clientStates[i].MenuType)
                        {
                            case MenuState.Type.Connect: // Handle connection events
                                if (clientStates[i].EventDetail == (int)MenuState.ConnectionDetails.IdReqest) // Assign our own player id
                                {
                                    playerId = clientStates[i].PlayerId;
                                }
                                else if (clientStates[i].EventDetail == (int)MenuState.ConnectionDetails.Connected) // Assign a connected player
                                {
                                    playersConnected[clientStates[i].PlayerId] = true;
                                }
                                else if (clientStates[i].EventDetail == (int)MenuState.ConnectionDetails.Disconnected) // Note down that a player is no longer in the session
                                {
                                    playersConnected[clientStates[i].PlayerId] = false;
                                    // TODO: Handle the server disconnecting
                                }
                                break;
                            case MenuState.Type.SelectCharacter: // Handle character selection
                                characterSelections[clientStates[i].PlayerId] = (PlayerCharacter.Type)clientStates[i].EventDetail;
                                break;
                            case MenuState.Type.LockCharacter: // Handle character locking
                                if (clientStates[i].EventDetail == (int)MenuState.LockCharacterDetails.Locked)
                                {
                                    characterSelectionsLocked[clientStates[i].PlayerId] = true;
                                }
                                else if (clientStates[i].EventDetail == (int)MenuState.LockCharacterDetails.Unlocked)
                                {
                                    characterSelectionsLocked[clientStates[i].PlayerId] = false;
                                }
                                break;
                            case MenuState.Type.GameStart:
                                LoadingScreen.Load(ScreenManager, false, PlayerIndex.One, new GameplayScreen(host, multiplayer, NumConnectedPlayers(), playerId, characterSelections, null));
                                break;
                            default:
                                throw new ArgumentException();
                        }
                    }
                }
            }
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            return;
        }

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

            Vector2 slotMessageSize; // Get the size of the redundant slot messages
            if (multiplayer) // Insert slot messages
            {
                slotMessageSize = ScreenManager.Font.MeasureString(openSlotMessage);
            }
            else
            {
                slotMessageSize = ScreenManager.Font.MeasureString(closedSlotMessage);
            }

            for (int i = 0; i < ProjectSkyCrane.MAX_PLAYERS; i += 1) // Draw the individual characters
            {
                int row = i / PLAYERS_PER_ROW;
                int column = i % PLAYERS_PER_ROW;

                if (playersConnected[i]) // Draw a connected player
                {
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
                    spriteBatch.Draw(characters[(int)characterSelections[i]], new Rectangle(centerColumn, centerRow, 192, 192),
                        null, drawColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                    int selectBase = centerRow + 192;

                    if (i == playerId)
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
                else
                {
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
            }

            spriteBatch.End();
 	        base.Draw(gameTime);
            return;
        }

        #endregion
    }
}
