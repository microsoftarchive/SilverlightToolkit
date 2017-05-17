// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the HeaderedItemsControl.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Headered", Justification = "Name of the control")]
    [Sample("(4)HeaderedItemsControl", DifficultyLevel.Basic, "TreeView")]
    public partial class HeaderedItemsControlSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the HeaderedItemsControlSample class.
        /// </summary>
        public HeaderedItemsControlSample()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        /// <summary>
        /// Load the demonstration.
        /// </summary>
        /// <param name="sender">Sample page.</param>
        /// <param name="e">Event arguments.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            MicrosoftExecutives.ItemsSource = Employee.Executives;
        }
    }
}