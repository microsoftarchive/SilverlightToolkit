// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the DatePicker.
    /// </summary>
    [Sample("TabControl", DifficultyLevel.Basic, "TabControl")]
    public partial class TabControlSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the DatePickerSample class.
        /// </summary>
        public TabControlSample()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handle button clicks to add new tab items.
        /// </summary>
        /// <param name="sender">The Button.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Event handler attached in XAML.")]
        private void OnAddTabItem(object sender, RoutedEventArgs e)
        {
            sampleTabs.Items.Add(
                new TabItem
                {
                    Header = "Dynamically Created TabItem",
                    Content = "Some Content"
                });
        }
    }
}