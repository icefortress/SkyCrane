#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using Microsoft.Xna.Framework;
using System.Windows.Forms;
using System.Drawing;
#endregion

namespace SkyCrane.Screens
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry fullScreenOnMenuEntry;
        MenuEntry resolutionMenuEntry;
        MenuEntry borderlessMenuEntry;
        MenuEntry musicOnMenuEntry;
        MenuEntry musicVolumeMenuEntry;
        MenuEntry soundFXOnMenuEntry;
        MenuEntry soundFXVolumeMenuEntry;

        /// <summary>
        /// Enumeration representing on/off options.
        /// </summary>
        enum OnOff
        {
            Off,
            On
        }

        // Display options
        static readonly string[] resolutions = { "800x600", "1024x768", 
            "1152x864", "1280x720", "1280x800", "1280x960", "1920x1080" };
        static readonly char[] resolutionDelimiters = { 'x' };

        // Current display settings options
        static OnOff fullScreenOn = OnOff.Off;
        static int resolution = 0;
        static OnOff borderlessOn = OnOff.Off;

        // Volume level options
        const int MIN_VOLUME = 0;
        const int MAX_VOLUME = 10;
        const int VOLUME_DELTA = 1;

        // Current music settings
        static OnOff musicOn = OnOff.On;
        static int musicVolume = MAX_VOLUME;

        // Current sound FX settings
        static OnOff soundFXOn = OnOff.On;
        static int soundFXVolume = MAX_VOLUME;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {

            // Create our menu entries
            fullScreenOnMenuEntry = new MenuEntry(string.Empty, true);
            resolutionMenuEntry = new MenuEntry(string.Empty, false, fullScreenOn == OnOff.Off);
            borderlessMenuEntry = new MenuEntry(string.Empty, true, fullScreenOn == OnOff.Off);
            musicOnMenuEntry = new MenuEntry(string.Empty, true);
            musicVolumeMenuEntry = new MenuEntry(string.Empty, true, musicOn == OnOff.On);
            soundFXOnMenuEntry = new MenuEntry(string.Empty, true);
            soundFXVolumeMenuEntry = new MenuEntry(string.Empty, true, soundFXOn == OnOff.On);

            SetMenuEntryText();
            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            fullScreenOnMenuEntry.Selected += FullScreenOnMenuEntrySelected;
            resolutionMenuEntry.Selected += ResolutionMenuEntrySelected;
            borderlessMenuEntry.Selected += BorderlessMenuEntrySelected;
            musicOnMenuEntry.Selected += MusicOnMenuEntrySelected;
            musicVolumeMenuEntry.Selected += MusicVolumeMenuEntrySelected;
            soundFXOnMenuEntry.Selected += SoundFXOnEntrySelected;
            soundFXVolumeMenuEntry.Selected += SoundFXVolumeMenuEntrySelected;
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(fullScreenOnMenuEntry);
            MenuEntries.Add(borderlessMenuEntry);
            MenuEntries.Add(resolutionMenuEntry);
            MenuEntries.Add(musicOnMenuEntry);
            MenuEntries.Add(musicVolumeMenuEntry);
            MenuEntries.Add(soundFXOnMenuEntry);
            MenuEntries.Add(soundFXVolumeMenuEntry);
            MenuEntries.Add(back);
            return;
        }

        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            fullScreenOnMenuEntry.Text = "Fullscreen: " + fullScreenOn;
            borderlessMenuEntry.Text = "Borderless: " + borderlessOn;
            resolutionMenuEntry.Text = "Pick Resolution";
            musicOnMenuEntry.Text = "Music: " + musicOn;
            musicVolumeMenuEntry.Text = "Music Volume: " + musicVolume;
            soundFXOnMenuEntry.Text = "SoundFX: " + soundFXOn;
            soundFXVolumeMenuEntry.Text = "SoundFX Volume: " + soundFXVolume;
            return;
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the fullscreen setting is toggled.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void FullScreenOnMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (e.ToggleDirection != 0)
            {
                ((ProjectSkyCrane)ScreenManager.Game).GraphicsDeviceManager.ToggleFullScreen();
                fullScreenOn += e.ToggleDirection;
                if (fullScreenOn < OnOff.Off)
                {
                    fullScreenOn = OnOff.On;
                }
                else if (fullScreenOn > OnOff.On)
                {
                    fullScreenOn = OnOff.Off;
                }
                resolutionMenuEntry.Enabled = fullScreenOn == OnOff.Off;
                borderlessMenuEntry.Enabled = fullScreenOn == OnOff.Off;
                SetMenuEntryText();
            }
            return;
        }

        /// <summary>
        /// Event handler for when the resolution setting is toggled.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void ResolutionMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new ResolutionsMenuScreen(), e.PlayerIndex);
            return;
        }

        /// <summary>
        /// Event handler for when the borderless setting is toggled.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void BorderlessMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (e.ToggleDirection != 0)
            {
                borderlessOn += e.ToggleDirection;
                if (borderlessOn < OnOff.Off)
                {
                    borderlessOn = OnOff.On;
                }
                else if (borderlessOn > OnOff.On)
                {
                    borderlessOn = OnOff.Off;
                }

                Form gameWindow = Form.FromHandle(((ProjectSkyCrane)ScreenManager.Game).Window.Handle).FindForm();
                if (borderlessOn == OnOff.On) // Sneakily adjust the border through the handle
                {
                    gameWindow.FormBorderStyle = FormBorderStyle.None;
                }
                else
                {
                    gameWindow.FormBorderStyle = FormBorderStyle.FixedSingle;
                }
                SetMenuEntryText();
            }
            return;
        }

        /// <summary>
        /// Event handler for when the music is turned on or off.
        /// </summary>
        void MusicOnMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (e.ToggleDirection != 0)
            {
                musicOn += e.ToggleDirection;
                if (musicOn < OnOff.Off)
                {
                    musicOn = OnOff.On;
                }
                else if (musicOn > OnOff.On)
                {
                    musicOn = OnOff.Off;
                }
                musicVolumeMenuEntry.Enabled = musicOn == OnOff.On;
                SetMenuEntryText();
            }
            return;
        }

        /// <summary>
        /// Event handler for when the music volume is turned up or down.
        /// </summary>
        void MusicVolumeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (e.ToggleDirection != 0)
            {
                musicVolume += e.ToggleDirection * VOLUME_DELTA;
                if (musicVolume > MAX_VOLUME)
                {
                    musicVolume = MIN_VOLUME;
                }
                else if (musicVolume < MIN_VOLUME)
                {
                    musicVolume = MAX_VOLUME;
                }
                SetMenuEntryText();
            }
            return;
        }

        /// <summary>
        /// Event handler for when the SFX are turned on or off.
        /// </summary>
        void SoundFXOnEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (e.ToggleDirection != 0)
            {
                soundFXOn += e.ToggleDirection;
                if (soundFXOn < OnOff.Off)
                {
                    soundFXOn = OnOff.On;
                }
                else if (soundFXOn > OnOff.On)
                {
                    soundFXOn = OnOff.Off;
                }
                soundFXVolumeMenuEntry.Enabled = soundFXOn == OnOff.On;
                SetMenuEntryText();
            }
            return;
        }

        /// <summary>
        /// Event handler for when the SFX volume is turned up or down.
        /// </summary>
        void SoundFXVolumeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (e.ToggleDirection != 0)
            {
                soundFXVolume += e.ToggleDirection * VOLUME_DELTA;
                if (soundFXVolume > MAX_VOLUME)
                {
                    soundFXVolume = MIN_VOLUME;
                }
                else if (soundFXVolume < MIN_VOLUME)
                {
                    soundFXVolume = MAX_VOLUME;
                }
                SetMenuEntryText();
            }
            return;
        }

        #endregion
    }
}
