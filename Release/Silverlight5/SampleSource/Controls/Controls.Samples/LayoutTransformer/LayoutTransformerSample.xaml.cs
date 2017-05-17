// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating LayoutTransformer.
    /// </summary>
    [Sample("Interactive Sample", DifficultyLevel.Basic, "LayoutTransformer")]
    public partial class LayoutTransformerSample : UserControl
    {
        /// <summary>
        /// Specifies the size of large fonts.
        /// </summary>
        private const int SampleFontSizeLarge = 64;

        /// <summary>
        /// Specifies the size of small fonts.
        /// </summary>
        private const int SampleFontSizeSmall = 28;

        /// <summary>
        /// Indicates whether component initialization has finished.
        /// </summary>
        private bool _initializeComponentFinished;

        /// <summary>
        /// Initializes a new instance of the LayoutTransformerSample class.
        /// </summary>
        public LayoutTransformerSample()
        {
            InitializeComponent();
            _initializeComponentFinished = true;
            Slider_ValueChanged(null, null);
            Text_Click(null, null);
            Loaded += delegate
            {
                // Sample browser-specific layout change.
                SampleHelpers.ChangeSampleAlignmentToStretch(this);
            };
        }

        /// <summary>
        /// Handles the ValueChanged event for any Slider.
        /// </summary>
        /// <param name="sender">Source of event.</param>
        /// <param name="e">Event arguments.</param>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_initializeComponentFinished)
            {
                scaleTransform.ScaleX = scaleXSlider.Value;
                scaleXDisplay.Text = Math.Round(scaleXSlider.Value, 2).ToString(CultureInfo.CurrentCulture);
                scaleTransform.ScaleY = scaleYSlider.Value;
                scaleYDisplay.Text = Math.Round(scaleYSlider.Value, 2).ToString(CultureInfo.CurrentCulture);
                skewTransform.AngleX = skewXSlider.Value;
                skewXDisplay.Text = Math.Round(skewXSlider.Value, 2).ToString(CultureInfo.CurrentCulture);
                skewTransform.AngleY = skewYSlider.Value;
                skewYDisplay.Text = Math.Round(skewYSlider.Value, 2).ToString(CultureInfo.CurrentCulture);
                rotateTransform.Angle = rotateSlider.Value;
                rotateDisplay.Text = Math.Round(rotateSlider.Value, 2).ToString(CultureInfo.CurrentCulture);
                layoutTransform.ApplyLayoutTransform();
            }
        }

        /// <summary>
        /// Handles the Click event for the Empty Button.
        /// </summary>
        /// <param name="sender">Source of event.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Referenced in XAML.")]
        private void Empty_Click(object sender, RoutedEventArgs e)
        {
            layoutTransform.Content = null;
        }

        /// <summary>
        /// Handles the Click event for the Text Button.
        /// </summary>
        /// <param name="sender">Source of event.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Referenced in XAML.")]
        private void Text_Click(object sender, RoutedEventArgs e)
        {
            layoutTransform.Content = new TextBlock { Text = "TextBlock", FontSize = SampleFontSizeLarge };
        }

        /// <summary>
        /// Handles the Click event for the Button Button.
        /// </summary>
        /// <param name="sender">Source of event.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Referenced in XAML.")]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            layoutTransform.Content = new Button { Content = "Button", FontSize = SampleFontSizeLarge };
        }

        /// <summary>
        /// Handles the Click event for the Image Button.
        /// </summary>
        /// <param name="sender">Source of event.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Referenced in XAML.")]
        private void Image_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (Stream resourceStream = typeof(SharedResources).Assembly.GetManifestResourceStream("System.Windows.Controls.Samples.Images.Dock.jpg"))
            {
                if (resourceStream != null)
                {
                    bitmapImage.SetSource(resourceStream);
                }
            }
            layoutTransform.Content = new Image { Source = bitmapImage };
        }

        /// <summary>
        /// Handles the Click event for the Empty Button.
        /// </summary>
        /// <param name="sender">Source of event.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Referenced in XAML.")]
        private void ListBox_Click(object sender, RoutedEventArgs e)
        {
            var listBox = new ListBox();
            foreach (var item in "This-is a-fully-functional-ListBox!".Split('-'))
            {
                listBox.Items.Add(new ListBoxItem { Content = item, FontSize = SampleFontSizeSmall });
            }
            layoutTransform.Content = listBox;
        }
    }
}