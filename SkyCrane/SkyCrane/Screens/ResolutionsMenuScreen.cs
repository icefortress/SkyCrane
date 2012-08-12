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
    class ResolutionsMenuScreen : MenuScreen
    {
        #region Fields

        // Display options
        static readonly string[] resolutions = { "800x600", "1024x768", 
            "1152x864", "1280x720", "1280x800", "1280x960", "1360x768",
            "1366x768", "1400x1050", "1440x900", "1680x1050", "1920x1080" };
        static readonly char[] resolutionDelimiters = { 'x' };

        static string currentResolution = "1280x720";

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public ResolutionsMenuScreen()
            : base("Resolutions")
        {

            for (int i = 0; i < resolutions.Length; i += 1) // Add an option for each resolution
            {
                MenuEntry resolution = new MenuEntry(resolutions[i]);
                resolution.Selected += ResolutionMenuEntrySelected;
                MenuEntries.Add(resolution);

                if (string.Compare(resolutions[i], currentResolution) == 0) // Select the most recent entry
                {
                    SelectedEntry = i;
                }
            }

            // Set up the back menu option
            MenuEntry back = new MenuEntry("Back");
            back.Selected += OnCancel;
            MenuEntries.Add(back);
            return;
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the resolution setting is toggled.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void ResolutionMenuEntrySelected(object sender, PlayerInputEventArgs e)
        {
            currentResolution = ((MenuEntry)sender).Text;
            string[] dimensions = currentResolution.Split(resolutionDelimiters);
            int width = int.Parse(dimensions[0]), height = int.Parse(dimensions[1]);
            Form gameWindow = Form.FromHandle(((ProjectSkyCrane)ScreenManager.Game).Window.Handle).FindForm();
            gameWindow.Size = new Size(width, height); // Set the window size
            if (Screen.PrimaryScreen.Bounds.Width == width) // Set the new X coordinates
            {
                gameWindow.Left = 0;
            }
            else
            {
                gameWindow.Left = (Screen.PrimaryScreen.Bounds.Width - width) / 2;
            }
            if (Screen.PrimaryScreen.Bounds.Height == height) // Set the new Y coordinates
            {
                gameWindow.Top = 0;
            }
            else
            {
                gameWindow.Top = (Screen.PrimaryScreen.Bounds.Height - height) / 2;
            }
            return;
        }
     
        #endregion
    }
}
