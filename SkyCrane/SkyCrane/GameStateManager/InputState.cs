#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Text;

namespace SkyCrane.GameStateManager
{
    /// <summary>
    /// Helper for various types of input.
    /// Tracks current and previous states of devices.
    /// Allows complex input actions ("naviagte menu", "pause game").
    /// </summary>
    public class InputState
    {

        public const int MAX_INPUTS = 4;

        public readonly KeyboardState[] currentKeyboardStates;
        public readonly KeyboardState[] lastKeyboardStates;
        public readonly GamePadState[] currentGamePadStates;
        public readonly GamePadState[] lastGamePadStates;
        public readonly bool[] gamePadWasConnected;

        /// <summary>
        /// Typeable keys.
        /// </summary>
        public static readonly Keys[] TYPEABLE_KEYS = { Keys.A, Keys.B, Keys.C,
            Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
            Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q,
            Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X,
            Keys.Y, Keys.Z, Keys.NumPad0, Keys.NumPad1, Keys.NumPad2,
            Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6,
            Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.D0, Keys.D1,
            Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8,
            Keys.D9, Keys.OemPeriod, Keys.Decimal, Keys.Divide, Keys.OemQuestion,
            Keys.OemMinus, Keys.Subtract };
        
        /// <summary>
        /// CharacteDivider mappings for the above array.
        /// </summary>
        public static readonly String TYPEABLE_CHARS = "abcdefghijklmnopqrstuvwxyz01234567890123456789..//--";

        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            currentKeyboardStates = new KeyboardState[MAX_INPUTS];
            currentGamePadStates = new GamePadState[MAX_INPUTS];
            lastKeyboardStates = new KeyboardState[MAX_INPUTS];
            lastGamePadStates = new GamePadState[MAX_INPUTS];
            gamePadWasConnected = new bool[MAX_INPUTS];
            return;
        }

        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < MAX_INPUTS; i++)
            {
                lastKeyboardStates[i] = currentKeyboardStates[i];
                lastGamePadStates[i] = currentGamePadStates[i];
                currentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                currentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

                // Keep track of whether a gamepad has ever been
                // connected, so we can detect if it is unplugged.
                if (currentGamePadStates[i].IsConnected)
                {
                    gamePadWasConnected[i] = true;
                }
            }
            return;
        }

        /// <summary>
        /// Helper for checking if a key was newly pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (currentKeyboardStates[i].IsKeyDown(key) &&
                        lastKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
        }

        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer,
                                                     out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;
                int i = (int)playerIndex;
                return (currentGamePadStates[i].IsButtonDown(button) &&
                        lastGamePadStates[i].IsButtonUp(button));
            }
            else
            {
                // Accept input from any player.
                return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
            }
        }


        /// <summary>
        /// Checks for a "menu select" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuSelect(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            return //IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks to see if a menu option is being toggled.
        /// </summary>
        /// <param name="controllingPlayer">The controlling player is checked for input.</param>
        /// <param name="playerIndex">Returned player that pressed the button (on null input).</param>
        /// <param name="menuDirection">The returned direction that the option was being toggled in.</param>
        /// <returns>True an option is being toggled.</returns>
        public bool IsMenuToggle(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex, out int toggleDirection)
        {
            toggleDirection = 0;
            if (//IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                    IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                    IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) ||
                    IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex) ||
                    IsNewKeyPress(Keys.Right, controllingPlayer, out playerIndex) ||
                    IsNewButtonPress(Buttons.LeftThumbstickRight, controllingPlayer, out playerIndex) ||
                    IsNewButtonPress(Buttons.DPadRight, controllingPlayer, out playerIndex))
            {
                toggleDirection += 1;
            }
            if (IsNewKeyPress(Keys.Left, controllingPlayer, out playerIndex) ||
                    IsNewButtonPress(Buttons.LeftThumbstickLeft, controllingPlayer, out playerIndex) ||
                    IsNewButtonPress(Buttons.DPadLeft, controllingPlayer, out playerIndex))
            {
                toggleDirection -= 1;
            }

            return toggleDirection != 0;
        }


        /// <summary>
        /// Checks for a "menu cancel" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuCancel(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex);
                   //IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "menu up" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "menu down" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Checks for a "pause the game" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsPauseGame(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   //IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Check to see if the backspace was pressed.
        /// </summary>
        public bool IsBackspace(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Back, controllingPlayer, out playerIndex);
        }

        /// <summary>
        /// Check to see if any typable input was given.
        /// </summary>
        public String TypeableInput(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            playerIndex = controllingPlayer.Value;
            StringBuilder returnString = new StringBuilder();
            for (int i = 0; i < TYPEABLE_KEYS.Length; i += 1)
            {
                if (IsNewKeyPress(TYPEABLE_KEYS[i], controllingPlayer, out playerIndex))
                {
                    returnString.Append(TYPEABLE_CHARS[i]);
                }
            }

            return returnString.ToString();
        }

    }
}
