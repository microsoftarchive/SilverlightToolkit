// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Data;
using Microsoft.Silverlight.Testing.Controls;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A derived TreeView control specific to the application needs for
    /// showing results in real-time.
    /// </summary>
    public class ResultsTreeView : TreeView
    {
        /// <summary>
        /// Overrides the item to allow for simple binding to the expanded
        /// property on the item.
        /// </summary>
        /// <returns>Returns a new container for an item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return GetSharedContainer();
        }

        /// <summary>
        /// This method returns the container with an expanded binding.
        /// </summary>
        /// <returns>Returns the container with an expanded binding.</returns>
        public static DependencyObject GetSharedContainer()
        {
            TreeViewItem tvi = new ResultsTreeViewItem();
            Binding eb = new Binding("IsExpanded");
            eb.Mode = BindingMode.OneWay;
            tvi.SetBinding(TreeViewItem.IsExpandedProperty, eb);

            Binding cb = new Binding("IsChecked");
            cb.Mode = BindingMode.TwoWay;
            tvi.SetBinding(TreeViewExtensions.IsCheckedProperty, cb);

            return tvi;
        }
    }
}