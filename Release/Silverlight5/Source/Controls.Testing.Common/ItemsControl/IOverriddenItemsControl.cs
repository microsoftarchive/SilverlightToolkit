// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Specialized;
using System.Windows;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Interface used to test virtual members of ItemsControl.
    /// </summary>
    public interface IOverriddenItemsControl : IOverriddenControl
    {
        /// <summary>
        /// Gets the IsItemItsOwnContainerOverride test actions.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for testing")]
        OverriddenMethod<object, bool?> IsItemItsOwnContainerOverrideActions { get; }

        /// <summary>
        /// Gets the GetContainerForItemOverride test actions.
        /// </summary>
        OverriddenMethod<DependencyObject> GetContainerForItemOverrideActions { get; }

        /// <summary>
        /// Gets the PrepareContainerForItemOverride test actions.
        /// </summary>
        OverriddenMethod<DependencyObject, object> PrepareContainerForItemOverrideActions { get; }

        /// <summary>
        /// Gets the ClearContainerForItemOverride test actions.
        /// </summary>
        OverriddenMethod<DependencyObject, object> ClearContainerForItemOverrideActions { get; }

        /// <summary>
        /// Gets the OnItemsChanged test actions.
        /// </summary>
        OverriddenMethod<NotifyCollectionChangedEventArgs> OnItemsChangedActions { get; }
    }
}