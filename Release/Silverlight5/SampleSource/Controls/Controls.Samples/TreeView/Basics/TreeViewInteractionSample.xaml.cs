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
    /// Sample demonstrating TreeView user interaction.
    /// </summary>
    [Sample("(1)Interaction", DifficultyLevel.Basic, "TreeView")]
    public partial class TreeViewInteractionSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the TreeViewInteractionSample class.
        /// </summary>
        public TreeViewInteractionSample()
        {
            InitializeComponent();

            // Fill the tree with data
            TreeOfLife.ItemsSource = Taxonomy.Life;
        }

        /// <summary>
        /// Expand all of the TreeOfLife TreeViewItems.
        /// </summary>
        /// <param name="sender">Expand All Button.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Attached as an event handler in XAML")]
        private void OnExpandAll(object sender, RoutedEventArgs e)
        {
            // Use the TreeViewExtensions.ExpandAll helper
            TreeOfLife.ExpandAll();
        }
    }
}