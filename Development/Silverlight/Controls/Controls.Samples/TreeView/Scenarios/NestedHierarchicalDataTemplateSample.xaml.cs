// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;

namespace Microsoft.Windows.Controls.Samples
{
    /// <summary>
    /// The NestedHierarchicalDataTemplate sample page shows how to nest
    /// HierarchicalDataTemplate templates with each HierarchicalDataTemplate
    /// having its own unique template.
    /// </summary>
    [Sample("TreeView/(1)Scenarios/(1)Nested HierarchicalDataTemplate")]
    public partial class NestedHierarchicalDataTemplateSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the
        /// NestedHierarchicalDataTemplateSample class.
        /// </summary>
        public NestedHierarchicalDataTemplateSample()
        {
            InitializeComponent();
            ArtistTree.ItemsSource = Artist.AllArtists;
        }
    }
}