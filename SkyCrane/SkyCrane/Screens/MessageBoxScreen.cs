#region File Description
//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SkyCrane.GameStateManager;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace SkyCrane.Screens
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    class MessageBoxScreen : GameScreen
    {

        #region Fields

        // Message variables
        bool includeUsageText;
        string message;
        string baseMessage;
        const string okText = "Ok: [ENTER] or";
        const string cancelText = "Cancel: [ESC] or";

        // Buttons and textures
        Texture2D gradientTexture;
        Texture2D aButtonTexture;
        Texture2D bButtonTexture;

        // Sound effecs
        SoundEffect okSoundEffect;
        SoundEffect cancelSoundEffect;

        #endregion

        #region Events

        public event EventHandler<PlayerInputEventArgs> Accepted;
        public event EventHandler<PlayerInputEventArgs> Cancelled;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message) : this(message, true) { }

        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message, bool includeUsageText)
        {
            this.includeUsageText = includeUsageText;
            baseMessage = message;
            if (includeUsageText)
            {
                this.message = baseMessage + "\n" + okText + "\n" + cancelText;
            }
            else
            {
                this.message = message;
            }
            IsPopup = true;
            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
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
            ContentManager content = ScreenManager.Game.Content;

            // Load textures
            gradientTexture = content.Load<Texture2D>("Menus/gradient");
            aButtonTexture = content.Load<Texture2D>("XBox Buttons/button_a");
            bButtonTexture = content.Load<Texture2D>("XBox Buttons/button_b");

            // Load sounds
            okSoundEffect = content.Load<SoundEffect>("SoundFX/menu_select");
            cancelSoundEffect = content.Load<SoundEffect>("SoundFX/menu_cancel");

            return;
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;

            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.

            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                okSoundEffect.Play();

                // Raise the accepted event, then exit the message box.
                if (Accepted != null)
                {
                    Accepted(this, new PlayerInputEventArgs(playerIndex, true));
                }
                
                ExitScreen();
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                cancelSoundEffect.Play();

                // Raise the cancelled event, then exit the message box.
                if (Cancelled != null)
                {
                    Cancelled(this, new PlayerInputEventArgs(playerIndex, false, true));
                }

                ExitScreen();
            }
            return;
        }


        #endregion

        #region Draw

        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textSize.Y + vPad * 2);

            Vector2 baseSize = font.MeasureString(baseMessage);
            Vector2 okSize = font.MeasureString(okText);
            Vector2 cancelSize = font.MeasureString(cancelText);

            // Fade the popup alpha during transitions.
            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin();

            // Draw the background rectangle.
            spriteBatch.Draw(gradientTexture, backgroundRectangle, color);
            
            // Draw the message box text.
            spriteBatch.DrawString(font, message, textPosition, color);

            if (includeUsageText) // If there is usage text, draw accompanying buttons
            {
                spriteBatch.Draw(aButtonTexture, new Rectangle((int)(textPosition.X + okSize.X),
                    (int)(textPosition.Y + baseSize.Y - 14), 64, 64), color);
                spriteBatch.Draw(bButtonTexture, new Rectangle((int)(textPosition.X + cancelSize.X),
                    (int)(textPosition.Y + baseSize.Y + okSize.Y - 14), 64, 64), color);
            }

            spriteBatch.End();

            return;
        }


        #endregion
    }
}
