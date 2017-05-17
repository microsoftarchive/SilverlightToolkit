// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// Represents a control that builds on top of the standard platform Button,
    /// offering the ability to modify the corner radii or even use special
    /// button modes.
    /// </summary>
    public class AdvancedButton : Button
    {
        #region public Visibility SecondaryVisibility
        /// <summary>
        /// Gets or sets the visibility of a secondary set of visuals in the
        /// template.
        /// </summary>
        public Visibility SecondaryVisibility
        {
            get { return (Visibility)GetValue(SecondaryVisibilityProperty); }
            set { SetValue(SecondaryVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the SecondaryVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondaryVisibilityProperty =
            DependencyProperty.Register(
                "SecondaryVisibility",
                typeof(Visibility),
                typeof(AdvancedButton),
                new PropertyMetadata(Visibility.Visible));
        #endregion public Visibility SecondaryVisibility

        #region public CornerRadius CornerRadius
        /// <summary>
        /// Gets or sets the corner radius to use.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the CornerRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(
                "CornerRadius",
                typeof(CornerRadius),
                typeof(AdvancedButton),
                new PropertyMetadata(new CornerRadius(1)));
        #endregion public CornerRadius CornerRadius
    }
}