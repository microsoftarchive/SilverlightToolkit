// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Automation.Peers;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Interface used to test virtual members of FrameworkElement.
    /// </summary>
    public interface IOverriddenFrameworkElement
    {
        /// <summary>
        /// Gets the MeasureOverride test actions.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for testing")]
        OverriddenMethod<Size, Size?> MeasureActions { get; }

        /// <summary>
        /// Gets the ArrangeOverride test actions.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for testing")]
        OverriddenMethod<Size, Size?> ArrangeActions { get; }

        /// <summary>
        /// Gets the OnCreateAutomationPeer test actions.
        /// </summary>
        OverriddenMethod<AutomationPeer> CreateAutomationPeerActions { get; }
    }
}