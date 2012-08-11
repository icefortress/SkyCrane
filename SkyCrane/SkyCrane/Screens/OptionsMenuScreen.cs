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

        // Current music options
        static OnOff musicOn = OnOff.On;
        static int musicVolume = 100;

        // Current sound FX options
        static OnOff soundFXOn = OnOff.On;
        static int soundFXVolume = 100;

        // Volume level properties
        const int MIN_VOLUME = 0;
        const int MAX_VOLUME = 100;
        const int VOLUME_DELTA = 10;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            musicOnMenuEntry = new MenuEntry(string.Empty, true);
            musicVolumeMenuEntry = new MenuEntry(string.Empty, true);
            soundFXOnMenuEntry = new MenuEntry(string.Empty, true);
            soundFXVolumeMenuEntry = new MenuEntry(string.Empty, true);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            musicOnMenuEntry.Selected += MusicOnMenuEntrySelected;
            musicVolumeMenuEntry.Selected += MusicVolumeMenuEntrySelected;
            soundFXOnMenuEntry.Selected += SoundFXOnEntrySelected;
            soundFXVolumeMenuEntry.Selected += SoundFXVolumeMenuEntrySelected;
            back.Selected += OnCancel;
            
            // Add entries to the menu.
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
            musicOnMenuEntry.Text = "Music: " + musicOn;
            musicVolumeMenuEntry.Text = "Music Volume: " + musicVolume;
            soundFXOnMenuEntry.Text = "SoundFX: " + soundFXOn;
            soundFXVolumeMenuEntry.Text = "SoundFX Volume: " + soundFXVolume;
            return;
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void MusicOnMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {

            // Toggle the music on or off
            musicOn += e.ToggleDirection;
            if (musicOn < OnOff.Off)
            {
                musicOn = OnOff.On;
            }
            else if (musicOn > OnOff.On)
            {
                musicOn = OnOff.Off;
            }

            // Toggle music on and off
            musicVolumeMenuEntry.Enabled = musicOn == OnOff.On;

            SetMenuEntryText();
        }

        /// <summary>
        /// Event handler for when the Language menu entry is selected.
        /// </summary>
        void MusicVolumeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            musicVolume += e.ToggleDirection * VOLUME_DELTA;
            if (musicVolume > MAX_VOLUME)
            {
                musicVolume = MIN_VOLUME;
            }

            SetMenuEntryText();
            return;
        }

        /// <summary>
        /// Event handler for when the Frobnicate menu entry is selected.
        /// </summary>
        void SoundFXOnEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            // Toggle the music on or off
            soundFXOn += e.ToggleDirection;
            if (soundFXOn < OnOff.Off)
            {
                soundFXOn = OnOff.On;
            }
            else if (soundFXOn > OnOff.On)
            {
                soundFXOn = OnOff.Off;
            }

            // Toggle music on and off
            soundFXVolumeMenuEntry.Enabled = soundFXOn == OnOff.On;

            SetMenuEntryText();
            return;
        }

        /// <summary>
        /// Event handler for when the Elf menu entry is selected.
        /// </summary>
        void SoundFXVolumeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            soundFXVolume += e.ToggleDirection * VOLUME_DELTA;
            if (soundFXVolume > MAX_VOLUME)
            {
                soundFXVolume = MIN_VOLUME;
            }

            SetMenuEntryText();
            return;
        }

        #endregion
    }
}
