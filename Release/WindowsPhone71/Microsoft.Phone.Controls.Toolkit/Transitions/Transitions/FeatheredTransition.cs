// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Controls an
    /// <see cref="T:Microsoft.Phone.Controls.ITransition"/>
    /// in order to produce a feathered animation on a set of 
    /// <see cref="T:System.Windows.UIElement"/>.
    /// </summary>
    public class FeatheredTransition : Transition
    {
        /// <summary>
        /// The <see cref="T:Microsoft.Phone.Controls.TurnstileFeatherTransitionMode"/>.
        /// </summary>
        private TurnstileFeatherTransitionMode _mode;

        /// <summary>
        /// The time at which the transition should begin.
        /// </summary>
        private TimeSpan? _beginTime;

        /// <summary>
        /// Constructs a
        /// <see cref="T:Microsoft.Phone.Controls.FeatheredTransition"/>
        /// for a
        /// <see cref="T:System.Windows.UIElement"/>
        /// and a
        /// <see cref="T:System.Windows.Media.Animation.Storyboard"/>,
        /// based on a 
        /// <see cref="T:Microsoft.Phone.Controls.TurnstileFeatherTransitionMode"/>
        /// </summary>
        /// <param name="element">
        /// The <see cref="T:System.Windows.UIElement"/>.
        /// </param>
        /// <param name="storyboard">
        /// The <see cref="T:System.Windows.Media.Animation.Storyboard"/>.
        /// </param>
        /// <param name="mode">
        /// The <see cref="T:Microsoft.Phone.Controls.TurnstileFeatherTransitionMode"/>.
        /// </param>
        /// <param name="beginTime">
        /// The time at which the transition should begin.
        /// </param>
        public FeatheredTransition(UIElement element, Storyboard storyboard, TurnstileFeatherTransitionMode mode, TimeSpan? beginTime)
            : base(element, storyboard)
        {
            _mode = mode;
            _beginTime = beginTime;
        }

        /// <summary>
        /// Composes the
        /// <see cref="M:System.Windows.Media.Animation.Storyboard"/>
        /// and mirrors 
        /// <see cref="M:System.Windows.Media.Animation.Storyboard.Begin"/>.
        /// </summary>
        public override void Begin()
        {
            TurnstileFeatherEffect.ComposeStoryboard(Storyboard, _beginTime, _mode);
            base.Begin();
        }
    }
}
