// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Interface used to test virtual members of NumericUpDown.
    /// </summary>
    public interface IOverriddenNumericUpDown : IOverriddenUpDownBase<double>
    {
        /// <summary>
        /// Gets the OnMinimumChanged test actions.
        /// </summary>
        OverriddenMethod<double, double> MinimumChangedActions { get; }

        /// <summary>
        /// Gets the OnMaximumChanged test actions.
        /// </summary>
        OverriddenMethod<double, double> MaximumChangedActions { get; }

        /// <summary>
        /// Gets the OnIncrementChanged test actions.
        /// </summary>
        OverriddenMethod<double, double> IncrementChangedActions { get; }

        /// <summary>
        /// Gets the OnDecimalPlacesChanged test actions.
        /// </summary>
        OverriddenMethod<int, int> DecimalPlacesChangedActions { get; }
    }
}