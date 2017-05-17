// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// The TestVisualStateManager class is used to track state change
    /// notifications.
    /// </summary>
    public class TestVisualStateManager : VisualStateManager
    {
        /// <summary>
        /// Gets a list of states that have been changed.
        /// </summary>
        public IList<string> ChangedStates { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TestVisualStateManager class.
        /// </summary>
        public TestVisualStateManager()
        {
            ChangedStates = new List<string>();
        }

        /// <summary>
        /// Change the visual state.
        /// </summary>
        /// <param name="control">Control whose state changed.</param>
        /// <param name="templateRoot">Root of the control's template.</param>
        /// <param name="stateName">New state name.</param>
        /// <param name="group">Visual state group.</param>
        /// <param name="state">Visual state.</param>
        /// <param name="useTransitions">
        /// A value indicating whether to animate transitions.
        /// </param>
        /// <returns>A value indicating whether the state was changed.</returns>
        protected override bool GoToStateCore(Control control, FrameworkElement templateRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
        {
            ChangedStates.Add(stateName);
            return false;
        }
    }
}