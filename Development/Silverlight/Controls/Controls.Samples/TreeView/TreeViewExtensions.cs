// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls.Samples
{
    /// <summary>
    /// TreeView and TreeViewItem extension methods.
    /// </summary>
    public static class TreeViewExtensions
    {
        /// <summary>
        /// Expand all the items in a TreeView.
        /// </summary>
        /// <param name="view">The TreeView.</param>
        public static void ExpandAll(this TreeView view)
        {
            if (view == null || view.Items == null)
            {
                return;
            }

            for (int i = 0; i < view.Items.Count; i++)
            {
                TreeViewItem item = view.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
                ExpandAll(item);
            }
        }

        /// <summary>
        /// Expand all the items in a TreeViewItem.
        /// </summary>
        /// <param name="item">The TreeViewItem.</param>
        public static void ExpandAll(this TreeViewItem item)
        {
            if (item == null)
            {
                return;
            }

            bool justExpanded = !item.IsExpanded;
            item.IsExpanded = true;
            if (justExpanded)
            {
                item.Dispatcher.BeginInvoke(() => ExpandAll(item));
            }
            else
            {
                for (int i = 0; i < item.Items.Count; i++)
                {
                    TreeViewItem child = item.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
                    ExpandAll(child);
                }
            }
        }
    }
}