// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Phone.Controls.Primitives
{
    /// <summary>
    /// Provides an attachable property that clips a FrameworkElement 
    /// to its bounds, e.g. clip the element when it is translated outside 
    /// of the panel it is placed in.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class ClipToBounds : DependencyObject
    {
        /// <summary>
        /// Gets the IsEnabled dependency property from an object.
        /// </summary>
        /// <param name="obj">The object to get the value from.</param>
        /// <returns>The property's value.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Standard pattern.")]
        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        /// <summary>
        /// Sets the IsEnabled dependency property on an object.
        /// </summary>
        /// <param name="obj">The object to set the value on.</param>
        /// <param name="value">The value to set.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Standard pattern.")]
        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        /// <summary>
        /// Identifies the IsEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ClipToBounds), new PropertyMetadata(false, OnIsEnabledPropertyChanged));

        /// <summary>
        /// Attaches or detaches the SizeChanged event handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The event information.</param>
        private static void OnIsEnabledPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement target = obj as FrameworkElement;

            if(target != null)
            {
                if ((bool)e.NewValue)
                {
                    target.SizeChanged += OnSizeChanged;
                }
                else
                {
                    target.SizeChanged -= OnSizeChanged;
                }
            }
        }

        /// <summary>
        /// Clips the associated object to a rectangular geometry determined
        /// by its actual width and height.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private static void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement source = sender as FrameworkElement;

            if (source != null)
            {
                RectangleGeometry geometry = new RectangleGeometry();
                geometry.Rect = new Rect(0, 0, source.ActualWidth, source.ActualHeight);
                source.Clip = geometry;
            }
        }        
    }
}
