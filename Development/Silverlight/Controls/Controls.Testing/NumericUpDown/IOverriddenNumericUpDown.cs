// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// Interface used to test virtual members of NumericUpDown.
    /// </summary>
    public interface IOverriddenNumericUpDown : IOverriddenUpDownBase<decimal>
    {
        /// <summary>
        /// Gets the OnMinimumChanged test actions.
        /// </summary>
        OverriddenMethod<decimal, decimal> MinimumChangedActions { get; }

        /// <summary>
        /// Gets the OnMaximumChanged test actions.
        /// </summary>
        OverriddenMethod<decimal, decimal> MaximumChangedActions { get; }

        /// <summary>
        /// Gets the OnIncrementChanged test actions.
        /// </summary>
        OverriddenMethod<decimal, decimal> IncrementChangedActions { get; }

        /// <summary>
        /// Gets the OnDecimalPlacesChanged test actions.
        /// </summary>
        OverriddenMethod<int, int> DecimalPlacesChangedActions { get; }
    }
}