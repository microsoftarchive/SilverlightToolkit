// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the Viewbox.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Viewbox", Justification = "Name of the control")]
    [Sample("Viewbox", DifficultyLevel.Basic)]
    [Category("Viewbox")]
    public partial class ViewboxSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the ViewboxSample class.
        /// </summary>
        public ViewboxSample()
        {
            InitializeComponent();

            // Add this line to validate namescope works for Viewbox.
            childButton.Content = "Content";

            // Add event handlers to update the interactive demo
            InteractiveHorizontalSlider.ValueChanged += (s, e) => UpdateInteractiveDemo();
            InteractiveVerticalSlider.ValueChanged += (s, e) => UpdateInteractiveDemo();
            StretchNone.Checked += (s, e) => UpdateInteractiveDemo();
            StretchFill.Checked += (s, e) => UpdateInteractiveDemo();
            StretchUniform.Checked += (s, e) => UpdateInteractiveDemo();
            StretchUniformToFill.Checked += (s, e) => UpdateInteractiveDemo();
            StretchDirectionUpOnly.Checked += (s, e) => UpdateInteractiveDemo();
            StretchDirectionDownOnly.Checked += (s, e) => UpdateInteractiveDemo();
            StretchDirectionBoth.Checked += (s, e) => UpdateInteractiveDemo();
        }

        /// <summary>
        /// Update the interactive Viewbox demo when any property changes.
        /// </summary>
        private void UpdateInteractiveDemo()
        {
            InteractiveWidthIndicator.Width = InteractiveViewbox.Width = InteractiveContainer.ActualWidth * InteractiveHorizontalSlider.Value / 100.0;
            InteractiveHeightIndicator.Height = InteractiveViewbox.Height = InteractiveContainer.ActualHeight * InteractiveVerticalSlider.Value / 100.0;
            InteractiveViewbox.Stretch =
                (StretchFill.IsChecked == true) ? Stretch.Fill :
                (StretchUniform.IsChecked == true) ? Stretch.Uniform :
                (StretchUniformToFill.IsChecked == true) ? Stretch.UniformToFill :
                Stretch.None;
            InteractiveViewbox.StretchDirection =
                (StretchDirectionUpOnly.IsChecked == true) ? StretchDirection.UpOnly :
                (StretchDirectionDownOnly.IsChecked == true) ? StretchDirection.DownOnly :
                StretchDirection.Both;
        }
    }
}