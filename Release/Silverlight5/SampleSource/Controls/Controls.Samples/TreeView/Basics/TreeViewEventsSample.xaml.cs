// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample demonstrating TreeView and TreeViewItem events.
    /// </summary>
    [Sample("(3)Events", DifficultyLevel.Basic, "TreeView")]
    public partial class TreeViewEventsSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the TreeViewEventsSample class.
        /// </summary>
        public TreeViewEventsSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handle the TreeView.SelectedItemChanged event.
        /// </summary>
        /// <param name="sender">The TreeView.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "The event handler is declared in XAML.")]
        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Note: Our sample added actual TreeViewItems to the TreeView.Items
            // collection (instead of proding other CLR or business objects
            // directly to the Items or ItemsSource properties).  That means
            // e.OldValue and e.NewValue will be TreeViewItems instead of
            // strings, business objects, etc.
            TreeViewItem oldItem = e.OldValue as TreeViewItem;
            TreeViewItem newItem = e.NewValue as TreeViewItem;
            Log(string.Format(
                CultureInfo.CurrentUICulture,
                "TreeView: SelectedItemChanged from '{0}' to '{1}'",
                oldItem != null ? oldItem.Header as string : "(null)",
                newItem != null ? newItem.Header as string : "(null)"));
        }

        /// <summary>
        /// Handle the TreeViewItem.Selected event.
        /// </summary>
        /// <param name="sender">The TreeViewItem.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "The event handler is declared in XAML.")]
        private void OnSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            Log(item, "Selected");
        }

        /// <summary>
        /// Handle the TreeViewItem.Unselected event.
        /// </summary>
        /// <param name="sender">The TreeViewItem.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "The event handler is declared in XAML.")]
        private void OnUnselected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            Log(item, "Unselected");
        }

        /// <summary>
        /// Handle the TreeViewItem.Expanded event.
        /// </summary>
        /// <param name="sender">The TreeViewItem.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "The event handler is declared in XAML.")]
        private void OnExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            Log(item, "Expanded");
        }

        /// <summary>
        /// Handle the TreeViewItem.Collapsed event.
        /// </summary>
        /// <param name="sender">The TreeViewItem.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "The event handler is declared in XAML.")]
        private void OnCollapsed(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            Log(item, "Collapsed");
        }

        /// <summary>
        /// Add an event to the list of raised events on the demo.
        /// </summary>
        /// <param name="message">The message to log.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by an event handler declared in XAML.")]
        private void Log(string message)
        {
            // Add a new message to the event log
            message = string.Format(CultureInfo.CurrentUICulture, "[{0:hh:mm:ss}]  {1}", DateTime.Now, message);
            EventLog.Children.Add(new TextBlock { Text = message });

            // Scroll to the bottom of the event log
            EventViewer.ScrollToVerticalOffset(EventViewer.ExtentHeight);
        }

        /// <summary>
        /// Add an event to the list of raised events on the demo.
        /// </summary>
        /// <param name="item">The item that raised the event.</param>
        /// <param name="eventName">The name of the event to log.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by an event handler declared in XAML.")]
        private void Log(TreeViewItem item, string eventName)
        {
            Log(string.Format(
                CultureInfo.CurrentUICulture,
                "TreeViewItem '{0}': {1}",
                item != null ? item.Header as string : "(null)",
                eventName));
        }

        /// <summary>
        /// Clear the event log.
        /// </summary>
        /// <param name="sender">The Button.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "The event handler is declared in XAML.")]
        private void OnClearLog(object sender, RoutedEventArgs e)
        {
            EventLog.Children.Clear();
        }
    }
}