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
    /// Sample demonstrating TreeView selection.
    /// </summary>
    [Sample("(2)Selection", DifficultyLevel.Basic, "TreeView")]
    public partial class TreeViewSelectionSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the TreeViewSelectionSample class.
        /// </summary>
        public TreeViewSelectionSample()
        {
            InitializeComponent();

            // Fill the tree with data
            TreeOfLife.ItemsSource = Taxonomy.Life;
        }

        /// <summary>
        /// Handle the TreeView.SelectedItemChanged event.
        /// </summary>
        /// <param name="sender">The TreeView.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "The event handler is declared in XAML.")]
        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView view = sender as TreeView;
            SelectedItem.Content = view.SelectedItem;
            SelectedValue.Content = view.SelectedValue;
        }
    }
}