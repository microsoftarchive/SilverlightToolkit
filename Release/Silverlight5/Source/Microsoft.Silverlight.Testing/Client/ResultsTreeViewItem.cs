// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing.Controls;

namespace Microsoft.Silverlight.Testing.Client
{
    /// <summary>
    /// A derived TreeViewItem for IsExpanded binding.
    /// </summary>
    public class ResultsTreeViewItem : TreeViewItem
    {
        /// <summary>
        /// Overrides the item to allow for simple binding to the expanded
        /// property on the item.
        /// </summary>
        /// <returns>Returns a new container for an item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return ResultsTreeView.GetSharedContainer();
        }

        /// <summary>
        /// Overrides the key down event to allow toggling the space.
        /// </summary>
        /// <param name="e">The key event arguments data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Flip the checkbox value
            if (e.Key == Key.Space)
            {
                bool? currentValue = (bool?)GetValue(TreeViewExtensions.IsCheckedProperty);
                SetValue(TreeViewExtensions.IsCheckedProperty, currentValue == null ? true : !(bool)currentValue);

                e.Handled = true;
            }

            base.OnKeyDown(e);
        }
    }
}