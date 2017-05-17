// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A class that contains notification information.
    /// </summary>
    public class Notifications : ItemsControl
    {
        /// <summary>
        /// Initialize a new intance of the Notifications type.
        /// </summary>
        public Notifications()
        {
            DefaultStyleKey = typeof(Notifications);
        }

        /// <summary>
        /// Overrides the on apply template method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Adds a notification object to the set of notifications. Connects the
        /// close button handler to automatically remove the notification from
        /// the visual tree.
        /// </summary>
        /// <param name="notification">A notification instance.</param>
        public void AddNotification(Notification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            notification.Closed += OnNotificationClosed;
            Items.Add(notification);
        }

        /// <summary>
        /// Handles the Closed event from the notification and removes the
        /// child.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnNotificationClosed(object sender, RoutedEventArgs e)
        {
            Notification n = sender as Notification;
            if (n != null)
            {
                Items.Remove(n);
            }
        }
    }
}