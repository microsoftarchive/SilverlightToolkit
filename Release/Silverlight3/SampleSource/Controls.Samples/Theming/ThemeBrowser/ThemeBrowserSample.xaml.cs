// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Controls;
using System.Windows.Controls.Theming;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Controls.DataVisualization.Charting;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Theme Sample Browser.
    /// </summary>
    [Sample("Theme Browser", DifficultyLevel.Basic)]
    [Category("Theme Browser")]
    public partial class ThemeBrowserSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the ThemeBrowserSample class.
        /// </summary>
        public ThemeBrowserSample()
        {
            InitializeComponent();

            ThemesList.ItemsSource = ThemeCatalogItem.ThemesCatalog;
            ThemesList.SelectionChanged += OnThemeChanged;

            // Show the first theme
            ShowTheme(ThemeCatalogItem.TwilightBlue);
        }

        /// <summary>
        /// Fired when the selected item in the list changes.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnThemeChanged(object sender, SelectionChangedEventArgs e)
        {
            // Remove the current theme from the end of the DemoControls area
            DemoControls.Children.RemoveAt(1);

            // Show the new theme
            ShowTheme(ThemesList.SelectedItem as ThemeCatalogItem);
        }

        /// <summary>
        /// Show the selected theme.
        /// </summary>
        /// <param name="item">The selected theme.</param>
        private void ShowTheme(ThemeCatalogItem item)
        {
            AllControls demo = new AllControls();
            demo.Loaded += (obj, e) => System.Diagnostics.Debug.WriteLine("Loaded");
            demo.PreferredBackground = item.PreferredBackground;
            DemoControls.Children.Add(demo);

            Uri uri = new Uri(item.ThemeXamlPath, UriKind.Relative);
            demo.ApplyStyle(uri);
        }
    }
}