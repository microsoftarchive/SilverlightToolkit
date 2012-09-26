// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Provides turnstile feather <see cref="T:Microsoft.Phone.Controls.ITransition"/>s.
    /// </summary>
    public class TurnstileFeatherTransition : TransitionElement
    {
        /// <summary>
        /// The
        /// <see cref="T:System.Windows.DependencyProperty"/>
        /// for the
        /// <see cref="T:Microsoft.Phone.Controls.TurnstileTransitionMode"/>.
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(TurnstileFeatherTransitionMode), typeof(TurnstileFeatherTransition), null);

        /// <summary>
        /// The <see cref="T:Microsoft.Phone.Controls.TurnstileTransitionMode"/>.
        /// </summary>
        public TurnstileFeatherTransitionMode Mode
        {
            get
            {
                return (TurnstileFeatherTransitionMode)GetValue(ModeProperty);
            }
            set
            {
                SetValue(ModeProperty, value);
            }
        }

        /// <summary>
        /// The
        /// <see cref="T:System.Windows.DependencyProperty"/>
        /// for the time at which the transition should begin.
        /// </summary>
        public static readonly DependencyProperty BeginTimeProperty =
            DependencyProperty.Register("BeginTime", typeof(TimeSpan?), typeof(TurnstileFeatherTransition), new PropertyMetadata(TimeSpan.Zero)); 

        /// <summary>
        /// The time at which the transition should begin.
        /// </summary>
        public TimeSpan? BeginTime
        {
            get 
            { 
                return (TimeSpan?)GetValue(BeginTimeProperty); 
            }
            set 
            { 
                SetValue(BeginTimeProperty, value); 
            }
        }

        /// <summary>
        /// Creates a new
        /// <see cref="T:Microsoft.Phone.Controls.ITransition"/>
        /// for a
        /// <see cref="T:System.Windows.UIElement"/>.
        /// Saves and clears the existing
        /// <see cref="F:System.Windows.UIElement.ProjectionProperty"/>
        /// value before the start of the transition, then restores it after it is stopped or completed.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Windows.UIElement"/>.</param>
        /// <returns>The <see cref="T:Microsoft.Phone.Controls.ITransition"/>.</returns>
        public override ITransition GetTransition(UIElement element)
        {
            return Transitions.TurnstileFeather(element, Mode, BeginTime);
        }
    }
}
