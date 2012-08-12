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
        public KeyboardState currentKeyboardState;
        public KeyboardState lastKeyboardState;
        public GamePadState currentGamePadState;
        public GamePadState lastGamePadState;
        public bool gamePadWasConnected;

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
            currentKeyboardState = new KeyboardState();
            currentGamePadState = new GamePadState();
            lastKeyboardState = new KeyboardState();
            lastGamePadState = new GamePadState();
            gamePadWasConnected = new bool();
            return;
        }

        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            lastKeyboardState = currentKeyboardState;
            lastGamePadState = currentGamePadState;
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // Keep track of whether a gamepad has ever been
            // connected, so we can detect if it is unplugged.
            if (currentGamePadState.IsConnected)
            {
                gamePadWasConnected = true;
            }
            return;
        }

        /// <summary>
        /// Helper for checking if a key was newly pressed during this update.
        /// </summary>
        public bool IsNewKeyPress(Keys key)
        {
            return (currentKeyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key));
        }

        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// </summary>
        public bool IsNewButtonPress(Buttons button)
        {
            return (currentGamePadState.IsButtonDown(button) && lastGamePadState.IsButtonUp(button));
        }


        /// <summary>
        /// Checks for a "menu select" input action.
        /// </summary>
        public bool IsMenuSelect()
        {
            return IsNewKeyPress(Keys.Enter) || IsNewButtonPress(Buttons.A) ||IsNewButtonPress(Buttons.Start);
        }

        /// <summary>
        /// Checks to see if a menu option is being toggled.
        /// </summary>
        public bool IsMenuToggle(out int toggleDirection)
        {
            toggleDirection = 0;
            if (IsNewKeyPress(Keys.Enter) || IsNewButtonPress(Buttons.A) || IsNewButtonPress(Buttons.Start) ||
                    IsNewKeyPress(Keys.Right) || IsNewButtonPress(Buttons.LeftThumbstickRight) || IsNewButtonPress(Buttons.DPadRight))
            {
                toggleDirection += 1;
            }
            if (IsNewKeyPress(Keys.Left) || IsNewButtonPress(Buttons.LeftThumbstickLeft) || IsNewButtonPress(Buttons.DPadLeft))
            {
                toggleDirection -= 1;
            }

            return toggleDirection != 0;
        }


        /// <summary>
        /// Checks for a "menu cancel" input action.
        /// </summary>
        public bool IsMenuCancel()
        {
            return IsNewKeyPress(Keys.Escape) || IsNewButtonPress(Buttons.B);
        }

        /// <summary>
        /// Checks for a "menu up" input action.
        /// </summary>
        public bool IsMenuUp()
        {
            return IsNewKeyPress(Keys.Up) || IsNewButtonPress(Buttons.DPadUp) ||  IsNewButtonPress(Buttons.LeftThumbstickUp);
        }

        /// <summary>
        /// Checks for a "menu down" input action.
        /// </summary>
        public bool IsMenuDown()
        {
            return IsNewKeyPress(Keys.Down) || IsNewButtonPress(Buttons.DPadDown) || IsNewButtonPress(Buttons.LeftThumbstickDown);
        }

        /// <summary>
        /// Checks for a "pause the game" input action.
        /// </summary>
        public bool IsPauseGame()
        {
            return IsNewKeyPress(Keys.Escape) || IsNewButtonPress(Buttons.Start);
        }

        /// <summary>
        /// Check to see if the backspace was pressed.
        /// </summary>
        public bool IsBackspace()
        {
            return IsNewKeyPress(Keys.Back);
        }

        /// <summary>
        /// Check to see if any typable input was given.
        /// </summary>
        public String TypeableInput()
        {
            StringBuilder returnString = new StringBuilder();
            for (int i = 0; i < TYPEABLE_KEYS.Length; i += 1)
            {
                if (IsNewKeyPress(TYPEABLE_KEYS[i]))
                {
                    returnString.Append(TYPEABLE_CHARS[i]);
                }
            }

            return returnString.ToString();
        }

    }
}
