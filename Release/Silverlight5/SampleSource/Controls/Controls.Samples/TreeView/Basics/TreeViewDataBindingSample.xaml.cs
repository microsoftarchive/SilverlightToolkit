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
    /// Sample demonstrating TreeView data-binding.
    /// </summary>
    [Sample("(4)Data Binding", DifficultyLevel.Basic, "TreeView")]
    public partial class TreeViewDataBindingSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the TreeViewDataBindingSample class.
        /// </summary>
        public TreeViewDataBindingSample()
        {
            InitializeComponent();

            // Fill the tree with data
            TreeOfLife.ItemsSource = Taxonomy.Life;
        }
    }
}