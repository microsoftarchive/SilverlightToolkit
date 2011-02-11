// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// A progress bar implementation for a smoother appearance of the 
    /// indeterminate states, with the added behavior that after the behavior
    /// is no longer needed, it smoothly fades out the dots for a less jarring
    /// experience. No exposed Value property.
    /// </summary>
    /// <remarks>
    /// Important - this control is not intended for regular progress 
    /// bar use, but only indeterminate. As a result, only an IsIndeterminate 
    /// set of visual states are defined in the nested progress bar template. 
    /// Use the standard ProgressBar control in the platform for determinate 
    /// scenarios as there is no performance benefit in determinate mode.
    /// </remarks>
    [TemplateVisualState(GroupName = VisualStateGroupName, Name = NormalState)]
    [TemplateVisualState(GroupName = VisualStateGroupName, Name = HiddenState)]
    public class PerformanceProgressBar : Control
    {
        #region Visual States
        private const string VisualStateGroupName = "VisibilityStates";
        private const string NormalState = "Normal";
        private const string HiddenState = "Hidden";
        #endregion

        /// <summary>
        /// The visual state group reference used to wait until the hidden state
        /// has fully transitioned to flip the underlying progress bar's
        /// indeterminate value to False.
        /// </summary>
        private VisualStateGroup _visualStateGroup;

        #region public bool ActualIsIndeterminate
        /// <summary>
        /// Gets or sets the value indicating whether the actual indeterminate
        /// property should be reflecting a particular value.
        /// </summary>
        public bool ActualIsIndeterminate
        {
            get { return (bool)GetValue(ActualIsIndeterminateProperty); }
            set { SetValue(ActualIsIndeterminateProperty, value); }
        }

        /// <summary>
        /// Identifies the ActualIsIndeterminate dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualIsIndeterminateProperty =
            DependencyProperty.Register(
                "ActualIsIndeterminate",
                typeof(bool),
                typeof(PerformanceProgressBar),
                new PropertyMetadata(false));
        #endregion public bool ActualIsIndeterminate

        #region public bool IsIndeterminate
        /// <summary>
        /// Gets or sets a value indicating whether the control is in the
        /// indeterminate state.
        /// </summary>
        public bool IsIndeterminate
        {
            get { return (bool)GetValue(IsIndeterminateProperty); }
            set { SetValue(IsIndeterminateProperty, value); }
        }

        /// <summary>
        /// Identifies the IsIndeterminate dependency property.
        /// </summary>
        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register(
                "IsIndeterminate",
                typeof(bool),
                typeof(PerformanceProgressBar),
                new PropertyMetadata(false, OnIsIndeterminatePropertyChanged));

        /// <summary>
        /// IsIndeterminateProperty property changed handler.
        /// </summary>
        /// <param name="d">PerformanceProgressBar that changed its IsIndeterminate.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsIndeterminatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PerformanceProgressBar source = d as PerformanceProgressBar;
            if (source != null)
            {
                source.OnIsIndeterminateChanged((bool)e.NewValue);
            }
        }
        #endregion public bool IsIndeterminate

        /// <summary>
        /// Initializes a new instance of the PerformanceProgressBar type.
        /// </summary>
        public PerformanceProgressBar()
            : base()
        {
            DefaultStyleKey = typeof(PerformanceProgressBar);
        }

        /// <summary>
        /// Applies the template and extracts both a visual state and a template
        /// part.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (_visualStateGroup != null)
            {
                _visualStateGroup.CurrentStateChanged -= OnVisualStateChanged;
            }

            base.OnApplyTemplate();

            _visualStateGroup = VisualStates.TryGetVisualStateGroup(this, VisualStateGroupName);
            if (_visualStateGroup != null)
            {
                _visualStateGroup.CurrentStateChanged += OnVisualStateChanged;
            }

            UpdateVisualStates(false);
        }

        private void OnVisualStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            // Turn off the visuals, the transition to hidden is complete.
            if (ActualIsIndeterminate && e != null && e.NewState != null && e.NewState.Name == HiddenState && !IsIndeterminate)
            {
                ActualIsIndeterminate = false;
            }
        }

        private void OnIsIndeterminateChanged(bool newValue)
        {
            if (newValue)
            {
                ActualIsIndeterminate = true;
            }
            else if (ActualIsIndeterminate && _visualStateGroup == null)
            {
                ActualIsIndeterminate = false;
            }
            // else: visual state changed handler will take care of this.

            UpdateVisualStates(true);
        }

        private void UpdateVisualStates(bool useTransitions)
        {
            VisualStateManager.GoToState(
                this, 
                IsIndeterminate ? NormalState : HiddenState, 
                useTransitions);
        }

        private static T FindFirstChildOfType<T>(DependencyObject root) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);
            while (0 < queue.Count)
            {
                var current = queue.Dequeue();
                for (var i = VisualTreeHelper.GetChildrenCount(current) - 1; 0 <= i; i--)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typedChild = child as T;
                    if (null != typedChild)
                    {
                        return typedChild;
                    }
                    queue.Enqueue(child);
                }
            }

            return null;
        }
    }
}
