// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Interface used to test virtual members of UpDownBase&lt;T&gt;.
    /// </summary>
    /// <typeparam name="T">Type of Value property.</typeparam>
    public interface IOverriddenUpDownBase<T> : IOverriddenUpDownBase
    {
        /// <summary>
        /// Gets the OnValueChanged test actions.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        OverriddenMethod<RoutedPropertyChangedEventArgs<T>> ValueChangedActions { get; }

        /// <summary>
        /// Gets the OnValueChanging test actions.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        OverriddenMethod<RoutedPropertyChangingEventArgs<T>> ValueChangingActions { get; }

        /// <summary>
        /// Gets the OnIsEditableChanged test actions.
        /// </summary>
        OverriddenMethod<bool, bool> IsEditableChangedActions { get; }

        /////// <summary>
        /////// Gets the OnIsCyclicChanged test actions.
        /////// </summary>
        ////OverriddenMethod<bool, bool> IsCyclicChangedActions { get; }

        /// <summary>
        /// Gets the OnParseError test actions.
        /// </summary>
        OverriddenMethod<UpDownParseErrorEventArgs> ParseErrorActions { get; }

        /// <summary>
        /// Gets the OnSpin test actions.
        /// </summary>
        OverriddenMethod<SpinEventArgs> SpinActions { get; }

        /// <summary>
        /// Gets the ApplyValue test actions.
        /// </summary>
        OverriddenMethod<string> ApplyValueActions { get; }

        /// <summary>
        /// Gets the FormatValue test actions.
        /// </summary>
        OverriddenMethod<string> FormatValueActions { get; }

        /// <summary>
        /// Gets the OnIncrement test actions.
        /// </summary>
        OverriddenMethod IncrementActions { get; }

        /// <summary>
        /// Gets the OnDecrement test actions.
        /// </summary>
        OverriddenMethod DecrementActions { get; }
    }
}