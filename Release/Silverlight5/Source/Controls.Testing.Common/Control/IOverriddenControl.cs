// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Input;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Interface used to test virtual members of Control.
    /// </summary>
    public interface IOverriddenControl : IOverriddenFrameworkElement
    {
        /// <summary>
        /// Gets the OnGotFocus test actions.
        /// </summary>
        OverriddenMethod<RoutedEventArgs> GotFocusActions { get; }

        /// <summary>
        /// Gets the OnLostFocus test actions.
        /// </summary>
        OverriddenMethod<RoutedEventArgs> LostFocusActions { get; }

        /// <summary>
        /// Gets the OnKeyDown test actions.
        /// </summary>
        OverriddenMethod<KeyEventArgs> KeyDownActions { get; }

        /// <summary>
        /// Gets the OnKeyUp test actions.
        /// </summary>
        OverriddenMethod<KeyEventArgs> KeyUpActions { get; }

        /// <summary>
        /// Gets the OnMouseEnter test actions.
        /// </summary>
        OverriddenMethod<MouseEventArgs> MouseEnterActions { get; }

        /// <summary>
        /// Gets the OnMouseLeave test actions.
        /// </summary>
        OverriddenMethod<MouseEventArgs> MouseLeaveActions { get; }

        /// <summary>
        /// Gets the OnMouseMove test actions.
        /// </summary>
        OverriddenMethod<MouseEventArgs> MouseMoveActions { get; }

        /// <summary>
        /// Gets the OnMouseLeftButtonDown test actions.
        /// </summary>
        OverriddenMethod<MouseButtonEventArgs> MouseLeftButtonDownActions { get; }

        /// <summary>
        /// Gets the OnMouseLeftButtonUp test actions.
        /// </summary>
        OverriddenMethod<MouseButtonEventArgs> MouseLeftButtonUpActions { get; }

        /// <summary>
        /// Gets the OnApplyTemplate test actions.
        /// </summary>
        OverriddenMethod ApplyTemplateActions { get; }
    }
}