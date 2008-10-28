// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Base class for controls that represents controls that can spin.
    /// </summary>
    /// <remarks>
    /// Spinner abstract class defines and implements common and focused visual state groups.
    /// Spinner abstract class defines and implements Spin event and OnSpin method.
    /// </remarks>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]

    [TemplateVisualState(Name = "Focused", GroupName = "FocusedStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusedStates")]
    public abstract partial class Spinner : Control
    {
        /// <summary>
        /// Occurs when spinning is initiated by the end-user.
        /// </summary>
        public event EventHandler<SpinEventArgs> Spin;

        /// <summary>
        /// Initializes a new instance of the Spinner class.
        /// </summary>
        protected Spinner()
        {
        }

        /// <summary>
        /// Raises the OnSpin event when spinning is initiated by the end-user.
        /// </summary>
        /// <param name="e">Spin event args.</param>
        protected virtual void OnSpin(SpinEventArgs e)
        {
            EventHandler<SpinEventArgs> handler = Spin;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
