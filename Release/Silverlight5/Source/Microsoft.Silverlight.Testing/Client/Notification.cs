// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A notification type for use in the Notifications ItemsControl.
    /// </summary>
    [TemplatePart(Name = CloseButtonName, Type = typeof(ButtonBase))]
    public class Notification : ContentControl
    {
        /// <summary>
        /// Name of the template part.
        /// </summary>
        private const string CloseButtonName = "CloseButton";

        #region public string Title
        /// <summary>
        /// Gets or sets the title of the notification.
        /// </summary>
        public string Title
        {
            get { return GetValue(TitleProperty) as string; }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Identifies the Title dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(string),
                typeof(Notification),
                new PropertyMetadata(string.Empty));
        #endregion public string Title

        #region public Visibility CloseButtonVisibility
        /// <summary>
        /// Gets or sets the visibility of the close button.
        /// </summary>
        public Visibility CloseButtonVisibility
        {
            get { return (Visibility)GetValue(CloseButtonVisibilityProperty); }
            set { SetValue(CloseButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the CloseButtonVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty CloseButtonVisibilityProperty =
            DependencyProperty.Register(
                "CloseButtonVisibility",
                typeof(Visibility),
                typeof(Notification),
                new PropertyMetadata(Visibility.Visible));
        #endregion public Visibility CloseButtonVisibility

        /// <summary>
        /// Backing field for the close button.
        /// </summary>
        private ButtonBase _close;

        /// <summary>
        /// Closed event that connects to the close button of the notification.
        /// </summary>
        public event RoutedEventHandler Closed;

        /// <summary>
        /// Initializes a new instance of the Notification class.
        /// </summary>
        public Notification()
            : base()
        {
            DefaultStyleKey = typeof(Notification);
        }

        /// <summary>
        /// Fires the Closed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnClosed(RoutedEventArgs e)
        {
            RoutedEventHandler handler = Closed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Overrides the on apply template method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (_close != null)
            {
                _close.Click -= OnCloseClick;
            }

            base.OnApplyTemplate();

            _close = GetTemplateChild(CloseButtonName) as ButtonBase;
            if (_close != null)
            {
                _close.Click += OnCloseClick;
            }
        }

        /// <summary>
        /// Connects to the Click event of the Close button.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            OnClosed(e);
        }
    }
}