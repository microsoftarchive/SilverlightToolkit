// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// A Control-derived class used by the Rating control to
    /// uniquely identify the objects used by the collections
    /// that contain the items displayed by the control.
    /// </summary>
    /// <remarks>
    /// This control exists to allow for custom ControlTemplates
    /// to be created to override the default visual style of the
    /// Rating control items. 
    /// </remarks>
    public class RatingItem : Control
    {
        /// <summary>
        /// Initializes a new instance of the RatingItem type.
        /// </summary>
        public RatingItem()
        {
            DefaultStyleKey = typeof(RatingItem);
        }

        #region public double StrokeThickness
        /// <summary>
        /// Gets or sets the value indicating the thickness of a stroke
        /// around the path object.
        /// </summary>
        /// <remarks>
        /// A control element was neccessary to allow for control templating,
        /// however the default implementation uses a path, this property was
        /// created because a good substitute for StrokeThickeness of a path,
        /// which is a double type, does not exist in the default Control class.
        /// </remarks>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the StrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThinkness", typeof(double), typeof(RatingItem), null);
        #endregion
    }
}
