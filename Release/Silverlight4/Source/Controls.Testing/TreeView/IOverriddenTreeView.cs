// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Interface used to test virtual members of TreeView.
    /// </summary>
    public interface IOverriddenTreeView : IOverriddenItemsControl
    {
        /// <summary>
        /// Gets the OnSelectedItemChanged test actions.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for testing")]
        OverriddenMethod<RoutedPropertyChangedEventArgs<object>> SelectedItemChangedActions { get; }
    }
}