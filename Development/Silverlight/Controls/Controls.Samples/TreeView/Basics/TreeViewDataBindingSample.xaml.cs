// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls.Samples
{
    /// <summary>
    /// Sample demonstrating TreeView data-binding.
    /// </summary>
    [Sample("TreeView/(0)Basics/(4)Data Binding")]
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