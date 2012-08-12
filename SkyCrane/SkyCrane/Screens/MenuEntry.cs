#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkyCrane.GameStateManager;
#endregion

namespace SkyCrane.Screens
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    class MenuEntry
    {
        #region Fields

        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        string text;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        float selectionFade;

        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        Vector2 position;

        /// <summary>
        /// Whether or not a particular menu entry is a toggleable one.
        /// </summary>
        bool toggleable;

        /// <summary>
        /// Whether or not this menu entry is enabled.
        /// </summary>
        bool enabled;

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }


        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Gets or sets whether this menu entry is toggleable.
        /// </summary>
        public Boolean Toggleable
        {
            get { return toggleable; }
            set { toggleable = value; }
        }

        /// <summary>
        /// Gets or sets whether this menu entry is enabled.
        /// </summary>
        public Boolean Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerInputEventArgs> Selected;

        /// <summary>
        /// Event raised when input gets typed;
        /// </summary>
        public event EventHandler<PlayerInputEventArgs> Typed;

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(bool menuAccepted, bool menuCancelled, int toggleDirection)
        {
            if (Selected != null)
            {
                Selected(this, new PlayerInputEventArgs(menuAccepted, menuCancelled, toggleDirection));
            }
            return;
        }

        /// <summary>
        /// Method for raising the Typed event.
        /// </summary>
        protected internal virtual void OnInputTyped(bool typingAccepted, bool typingCancelled, bool typingBackspace, String keysTyped)
        {
            if (Typed != null)
            {
                Typed(this, new PlayerInputEventArgs(false, false, 0, typingAccepted, typingCancelled, typingBackspace, keysTyped));
            }
            return;
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(string text, bool toggleable = false, bool enabled = true)
        {
            this.text = text;
            this.toggleable = toggleable;
            this.enabled = enabled;
            return;
        }


        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
            {
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            }
            else
            {
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
            }
            return;
        }


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {

            // Draw the selected entry in yellow, otherwise white.
            Color color;
            if (enabled) // Disabled entries are always gray
            {
                if (isSelected) // Unselected entries are always white
                {
                    if (screen.TypingInput) // Typing input is green, non-typing is yellow
                    {
                        color = Color.LimeGreen;
                    }
                    else
                    {
                        color = Color.Yellow;
                    }
                }
                else
                {
                    color = Color.White;
                }
            }
            else
            {
                color = Color.Gray;
            }
            
            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;
            
            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f * selectionFade;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, text, position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
            return;
        }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }

        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth(MenuScreen screen)
        {
            return (int)screen.ScreenManager.Font.MeasureString(Text).X;
        }

        #endregion
    }
}
