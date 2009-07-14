// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides the base implementation for controls that contain a single 
    /// content element and a header.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A HeaderedContentControl extends the ContentControl class and adds a 
    /// Header property for displaying label content at the top of the control. 
    /// The Header property accepts an object, which means it can contain any 
    /// kind of content. You can also set a data template to specify how the 
    /// label content should display by using the HeaderTemplate property.
    /// </para>
    /// <para>
    /// A HeaderedContentControl has a limited default style. You can create a 
    /// HeaderedContentControl, but its appearance will be very simple. 
    /// If you want to enhance the appearance of the control, you must create 
    /// a new ControlTemplate for the control. A HeaderedContentControl is 
    /// useful for creating custom controls because it provides a model for 
    /// a content control with a header.
    /// </para>
    /// <para>
    /// Dependency properties for this control might be set by the control�s 
    /// default style. If a property is set by a default style, the property 
    /// might change from its default value when the control appears in the 
    /// application. 
    /// </para>
    /// </remarks>
    /// <QualityBand>Stable</QualityBand>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Headered", Justification = "Consistency with WPF")]
    public partial class HeaderedContentControl : ContentControl
    {
        #region public object Header
        /// <summary>
        /// Gets or sets the content for the header of the control. 
        /// </summary>
        /// <remarks>
        /// This property is used to add content to the header of the 
        /// HeaderedContentControl. You can apply a data template to the 
        /// header by using the HeaderTemplate property. 
        /// </remarks>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
                DependencyProperty.Register(
                        "Header",
                        typeof(object),
                        typeof(HeaderedContentControl),
                        new PropertyMetadata(OnHeaderPropertyChanged));

        /// <summary>
        /// HeaderProperty property changed handler.
        /// </summary>
        /// <param name="d">HeaderedContentControl whose Header property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs, which contains the old and new value.</param>
        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedContentControl ctrl = (HeaderedContentControl)d;
            ctrl.OnHeaderChanged(e.OldValue, e.NewValue);
        }
        #endregion public object Header

        #region public DataTemplate HeaderTemplate
        /// <summary>
        /// Gets or sets the template that is used to display the content of 
        /// the control's header. 
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the HeaderTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
                DependencyProperty.Register(
                        "HeaderTemplate",
                        typeof(DataTemplate),
                        typeof(HeaderedContentControl),
                        new PropertyMetadata(OnHeaderTemplatePropertyChanged));

        /// <summary>
        /// HeaderTemplateProperty property changed handler.
        /// </summary>
        /// <param name="d">HeaderedContentControl whose HeaderTemplate property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs, which contains the old and new value.</param>
        private static void OnHeaderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedContentControl ctrl = (HeaderedContentControl)d;
            ctrl.OnHeaderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }
        #endregion public DataTemplate HeaderTemplate

        /// <summary>
        /// Default DependencyObject constructor.
        /// </summary>
        public HeaderedContentControl()
        {
            DefaultStyleKey = typeof(HeaderedContentControl);
        }

        /// <summary>
        /// Called when the value of the Header property changes.
        /// </summary>
        /// <param name="oldHeader">The old value of the Header property.</param>
        /// <param name="newHeader">The new value of the Header property.</param>
        protected virtual void OnHeaderChanged(object oldHeader, object newHeader)
        {
        }

        /// <summary>
        /// Called when the value of the HeaderTemplate property changes.
        /// </summary>
        /// <param name="oldHeaderTemplate">The old value of the HeaderTemplate property.</param>
        /// <param name="newHeaderTemplate">The new value of the HeaderTemplate property.</param>
        protected virtual void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
        {
        }
    }
}
