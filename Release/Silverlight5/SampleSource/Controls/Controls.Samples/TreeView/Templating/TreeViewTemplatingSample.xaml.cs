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
    /// Demonstrate different TreeView templates.
    /// </summary>
    [Sample("(5)Templating", DifficultyLevel.Basic, "TreeView")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Templating", Justification = "Correct spelling")]
    public partial class TreeViewTemplatingSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the TreeViewTemplatingSample class.
        /// </summary>
        public TreeViewTemplatingSample()
        {
            InitializeComponent();

            TreeDefault.ItemsSource = Taxonomy.Life;
            TreeCheckBox.ItemsSource = Taxonomy.Life;
            TreeFullRow.ItemsSource = Taxonomy.Life;
            TreeLines.ItemsSource = Taxonomy.Life;
        }

        /// <summary>
        /// Expand all of the TreeViewItems.
        /// </summary>
        /// <param name="sender">The Expand All Button.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "The event handler is declared in XAML.")]
        private void OnExpandAll(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                TreeView view = button.Tag as TreeView;
                if (view != null)
                {
                    view.ExpandAll();
                }
            }
        }
    }
}