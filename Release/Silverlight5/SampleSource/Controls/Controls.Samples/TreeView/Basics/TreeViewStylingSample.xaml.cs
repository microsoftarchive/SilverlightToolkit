// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample demonstrating TreeView styling.
    /// </summary>
    [Sample("(5)Styling", DifficultyLevel.Basic, "TreeView")]
    public partial class TreeViewStylingSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the TreeViewStylingSample class.
        /// </summary>
        public TreeViewStylingSample()
        {
            InitializeComponent();

            // Fill the tree with data
            TreeOfLife.ItemsSource = Taxonomy.Life;
        }
    }
}