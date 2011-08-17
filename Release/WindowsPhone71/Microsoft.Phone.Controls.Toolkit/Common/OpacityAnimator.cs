// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media.Animation;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// A utility for animating a horizontal translation value.
    /// </summary>
    internal sealed class OpacityAnimator
    {
        /// <summary>
        /// Single static instance of a PropertyPath with string path "X".
        /// </summary>
        private static readonly PropertyPath OpacityPropertyPath = new PropertyPath("Opacity");

        /// <summary>
        /// The Storyboard instance for the animation.
        /// </summary>
        private readonly Storyboard _sbRunning = new Storyboard();

        /// <summary>
        /// The DoubleAnimation instance for a running animation.
        /// </summary>
        private readonly DoubleAnimation _daRunning = new DoubleAnimation();

        /// <summary>
        /// Flag to suppress the Completed event notification from happening.
        /// </summary>
        private bool _suppressChangeNotification;

        /// <summary>
        /// A one-time action for the current GoTo statement only. Cleared if
        /// GoTo is called before the action runs.
        /// </summary>
        private Action _oneTimeAction;

        /// <summary>
        /// Initializes a new instance of the TransformAnimator class.
        /// </summary>
        /// <param name="target">Target element.</param>
        public OpacityAnimator(UIElement target)
        {
            Debug.Assert(target != null);

            _sbRunning.Completed += OnCompleted;
            _sbRunning.Children.Add(_daRunning);
            Storyboard.SetTarget(_daRunning, target);
            Storyboard.SetTargetProperty(_daRunning, OpacityPropertyPath);
        }

        /// <summary>
        /// Targets a new opacity value over a specified duration.
        /// </summary>
        /// <param name="targetOpacity">The target opacity value.</param>
        /// <param name="duration">The duration for the animation.</param>
        public void GoTo(double targetOpacity, Duration duration)
        {
            GoTo(targetOpacity, duration, null, null);
        }

        /// <summary>
        /// Targets a new opacity value over a specified duration.
        /// </summary>
        /// <param name="targetOpacity">The target opacity value.</param>
        /// <param name="duration">The duration for the animation.</param>
        /// <param name="completionAction">A completion Action.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Keeping existing implementation.")]
        public void GoTo(double targetOpacity, Duration duration, Action completionAction)
        {
            GoTo(targetOpacity, duration, null, completionAction);
        }

        /// <summary>
        /// Targets a new opacity value over a specified duration.
        /// </summary>
        /// <param name="targetOpacity">The target opacity value.</param>
        /// <param name="duration">The duration for the animation.</param>
        /// <param name="easingFunction">An easing function value.</param>
        /// <param name="completionAction">A completion Action.</param>
        public void GoTo(double targetOpacity, Duration duration, IEasingFunction easingFunction, Action completionAction)
        {
            _daRunning.To = targetOpacity;
            _daRunning.Duration = duration;
            _daRunning.EasingFunction = easingFunction;
            _sbRunning.Begin();
            _suppressChangeNotification = true;
            _sbRunning.SeekAlignedToLastTick(TimeSpan.Zero);
            _oneTimeAction = completionAction;
        }

        /// <summary>
        /// Handles and passes on the Completed event.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCompleted(object sender, EventArgs e)
        {
            Action action = _oneTimeAction;
            if (action != null)
            {
                _oneTimeAction = null;
                action();
            }
            if (!_suppressChangeNotification)
            {
                _suppressChangeNotification = false;
            }
        }

        /// <summary>
        /// Ensures and creates if needed the animator for an element. Will also
        /// verify that a translate transform is present.
        /// </summary>
        /// <param name="targetElement">The target element.</param>
        /// <param name="animator">The animator reference.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Keeping existing implementation.")]
        public static void EnsureAnimator(UIElement targetElement, ref OpacityAnimator animator)
        {
            if (animator == null)
            {
                animator = new OpacityAnimator(targetElement);
            }
            if (animator == null)
            {
                throw new InvalidOperationException("The animation system could not be prepared for the target element.");
            }
        }
    }
}