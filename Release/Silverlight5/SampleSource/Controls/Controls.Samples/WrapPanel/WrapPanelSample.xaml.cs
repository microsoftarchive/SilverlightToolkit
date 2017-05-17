// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the WrapPanel.
    /// </summary>
    [Sample("WrapPanel", DifficultyLevel.Basic, "WrapPanel")]
    public partial class WrapPanelSample : UserControl
    {
        /// <summary>
        /// Gets or sets a value indicating whether the demonstration has
        /// already been loaded.
        /// </summary>
        private bool IsLoaded { get; set; }

        /// <summary>
        /// Initializes a new instance of the WrapPanelSample class.
        /// </summary>
        public WrapPanelSample()
        {
            InitializeComponent();

            Loaded += OnLoad;

            chkHorizontal.Checked += OnCheckChanged;
            chkHorizontal.Unchecked += OnCheckChanged;
        }

        /// <summary>
        /// Load the demonstration.
        /// </summary>
        /// <param name="sender">Sample page.</param>
        /// <param name="e">Event arguments.</param>
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            // Don't repopulate if the page has already been loaded.
            if (IsLoaded)
            {
                return;
            }
            IsLoaded = true;

            // Generate the text to wrap
            string lorem = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Suspendisse sed tellus non sapien laoreet accumsan. Phasellus rhoncus imperdiet pede. Morbi semper ipsum at leo. Nullam elit mi, dignissim et, vestibulum ut, laoreet quis, velit. Nulla aliquet risus sed arcu. Nunc vitae tortor in lectus tristique iaculis. Morbi elit. Quisque euismod mollis orci. Nullam cursus interdum eros. Curabitur tristique mi non nulla. Curabitur non nisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Nam aliquet, velit eu pretium placerat, massa lorem sollicitudin dolor, non ultricies nisi lorem bibendum enim. Pellentesque mollis egestas ipsum. Donec odio quam, tempus ut, iaculis molestie, viverra vitae, sapien.";
            for (int i = 0; i < 4; i++)
            {
                foreach (string word in lorem.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    ManualTextWrapping.Children.Add(new TextBlock
                    {
                        Text = word,
                        Margin = new Thickness(3)
                    });
                }
            }

            // Generate the color swatch
            int granularity = 50;
            for (int r = 0; r < 255; r += granularity)
            {
                for (int g = 0; g < 255; g += granularity)
                {
                    for (int b = 0; b < 255; b += granularity)
                    {
                        Swatch.Items.Add(new Rectangle
                        {
                            Width = 20,
                            Height = 20,
                            Margin = new Thickness(5),
                            Stroke = new SolidColorBrush(Colors.Black),
                            StrokeThickness = 1,
                            Fill = new SolidColorBrush(Color.FromArgb(255, (byte) r, (byte) g, (byte) b))
                        });
                    }
                }
            }

            // Set the thumbnails
            Thumbnails.ItemsSource = Photograph.GetPhotographs().OrderBy(p => p.Name);
        }

        /// <summary>
        /// Update the manual text layout sample to match the CheckBox.
        /// </summary>
        /// <param name="sender">The CheckBox that was changed.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCheckChanged(object sender, RoutedEventArgs e)
        {
            if (chkHorizontal.IsChecked == true)
            {
                ManualTextWrapping.Height = double.NaN;
                ManualTextWrapping.Orientation = Orientation.Horizontal;
            }
            else
            {
                ManualTextWrapping.Height = 600;
                ManualTextWrapping.Orientation = Orientation.Vertical;
            }
        }
    }
}