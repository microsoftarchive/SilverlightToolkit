// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Interface used to test virtual members of TreeViewItem.
    /// </summary>
    public interface IOverriddenTreeViewItem : IOverriddenHeaderedItemsControl
    {
        /// <summary>
        /// Gets the OnExpanded test actions.
        /// </summary>
        OverriddenMethod<RoutedEventArgs> ExpandedActions { get; }

        /// <summary>
        /// Gets the OnCollapsed test actions.
        /// </summary>
        OverriddenMethod<RoutedEventArgs> CollapsedActions { get; }

        /// <summary>
        /// Gets the OnSelected test actions.
        /// </summary>
        OverriddenMethod<RoutedEventArgs> SelectedActions { get; }

        /// <summary>
        /// Gets the OnUnselected test actions.
        /// </summary>
        OverriddenMethod<RoutedEventArgs> UnselectedActions { get; }
    }
}