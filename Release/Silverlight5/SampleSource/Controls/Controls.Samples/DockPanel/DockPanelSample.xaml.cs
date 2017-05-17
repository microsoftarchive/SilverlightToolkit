// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the DockPanel.
    /// </summary>
    [Sample("DockPanel", DifficultyLevel.Basic, "DockPanel")]
    public partial class DockPanelSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the DockPanelSample class.
        /// </summary>
        public DockPanelSample()
        {
            InitializeComponent();

            // Update the toolbar demo when a button is toggled
            btnBold.Checked += (s, e) => UpdateToolbarDemo();
            btnBold.Unchecked += (s, e) => UpdateToolbarDemo();
            btnItalic.Checked += (s, e) => UpdateToolbarDemo();
            btnItalic.Unchecked += (s, e) => UpdateToolbarDemo();
            btnUnderline.Checked += (s, e) => UpdateToolbarDemo();
            btnUnderline.Unchecked += (s, e) => UpdateToolbarDemo();
        }

        /// <summary>
        /// Update the toolbar demo when a font property changes.
        /// </summary>
        private void UpdateToolbarDemo()
        {
            UsageText.FontWeight = (btnBold.IsChecked == true) ?
                FontWeights.Bold :
                FontWeights.Normal;
            UsageText.FontStyle = (btnItalic.IsChecked == true) ?
                FontStyles.Italic :
                FontStyles.Normal;
            UsageText.TextDecorations = (btnUnderline.IsChecked == true) ?
                TextDecorations.Underline :
                null;
        }
    }
}