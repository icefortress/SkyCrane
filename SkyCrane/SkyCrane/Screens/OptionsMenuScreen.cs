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
        MenuEntry borderlessOnMenuEntry;
        MenuEntry resolutionMenuEntry;
        MenuEntry vsyncOnMenuEntry;
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

        // Current display settings options
        static OnOff fullScreenOn = OnOff.Off;
        static OnOff borderlessOn = OnOff.Off;
        static OnOff vsyncOn = OnOff.Off;

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
            borderlessOnMenuEntry = new MenuEntry(string.Empty, true, fullScreenOn == OnOff.Off);
            resolutionMenuEntry = new MenuEntry(string.Empty, false, fullScreenOn == OnOff.Off);
            vsyncOnMenuEntry = new MenuEntry(string.Empty, true);
            musicOnMenuEntry = new MenuEntry(string.Empty, true);
            musicVolumeMenuEntry = new MenuEntry(string.Empty, true, musicOn == OnOff.On);
            soundFXOnMenuEntry = new MenuEntry(string.Empty, true);
            soundFXVolumeMenuEntry = new MenuEntry(string.Empty, true, soundFXOn == OnOff.On);

            SetMenuEntryText();
            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            fullScreenOnMenuEntry.Selected += FullScreenOnMenuEntrySelected;
            borderlessOnMenuEntry.Selected += BorderlessOnMenuEntrySelected;
            resolutionMenuEntry.Selected += ResolutionMenuEntrySelected;
            vsyncOnMenuEntry.Selected += VsyncOnMenuEntrySelected;
            musicOnMenuEntry.Selected += MusicOnMenuEntrySelected;
            musicVolumeMenuEntry.Selected += MusicVolumeMenuEntrySelected;
            soundFXOnMenuEntry.Selected += SoundFXOnEntrySelected;
            soundFXVolumeMenuEntry.Selected += SoundFXVolumeMenuEntrySelected;
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(fullScreenOnMenuEntry);
            MenuEntries.Add(borderlessOnMenuEntry);
            MenuEntries.Add(resolutionMenuEntry);
            MenuEntries.Add(vsyncOnMenuEntry);
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
            borderlessOnMenuEntry.Text = "Borderless: " + borderlessOn;
            resolutionMenuEntry.Text = "Pick Resolution";
            vsyncOnMenuEntry.Text = "Vertical Sync: " + vsyncOn;
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
        void FullScreenOnMenuEntrySelected(object sender, PlayerInputEventArgs e)
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
                borderlessOnMenuEntry.Enabled = fullScreenOn == OnOff.Off;
                SetMenuEntryText();
            }
            return;
        }

        /// <summary>
        /// Event handler for when the borderless setting is toggled.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void BorderlessOnMenuEntrySelected(object sender, PlayerInputEventArgs e)
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
                if (borderlessOn == OnOff.On) // Sneakily adjust the border through the handle
                {
                    Form.FromHandle(((ProjectSkyCrane)ScreenManager.Game).Window.Handle).FindForm().FormBorderStyle = FormBorderStyle.None;
                }
                else
                {
                    Form.FromHandle(((ProjectSkyCrane)ScreenManager.Game).Window.Handle).FindForm().FormBorderStyle = FormBorderStyle.FixedSingle;
                }
                SetMenuEntryText();
            }
            return;
        }

        /// <summary>
        /// Event handler for when the resolution setting is toggled.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void ResolutionMenuEntrySelected(object sender, PlayerInputEventArgs e)
        {
            ScreenManager.AddScreen(new ResolutionsMenuScreen(), e.PlayerIndex);
            return;
        }

        /// <summary>
        /// Event handler for when the vsync setting is toggled.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void VsyncOnMenuEntrySelected(object sender, PlayerInputEventArgs e)
        {
            if (e.ToggleDirection != 0)
            {
                vsyncOn += e.ToggleDirection;
                if (vsyncOn < OnOff.Off)
                {
                    vsyncOn = OnOff.On;
                }
                else if (vsyncOn > OnOff.On)
                {
                    vsyncOn = OnOff.Off;
                }
                ((ProjectSkyCrane)ScreenManager.Game).GraphicsDeviceManager.SynchronizeWithVerticalRetrace = vsyncOn == OnOff.On;
                SetMenuEntryText();
            }
            return;
        }


        /// <summary>
        /// Event handler for when the music is turned on or off.
        /// </summary>
        void MusicOnMenuEntrySelected(object sender, PlayerInputEventArgs e)
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
        void MusicVolumeMenuEntrySelected(object sender, PlayerInputEventArgs e)
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
        void SoundFXOnEntrySelected(object sender, PlayerInputEventArgs e)
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
        void SoundFXVolumeMenuEntrySelected(object sender, PlayerInputEventArgs e)
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
