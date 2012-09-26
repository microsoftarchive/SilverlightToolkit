// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Provides attached properties to make FrameworkElements inside
    /// PivotItems responsive to SelectionChanged events by Pivots.
    /// The result is a 'slide in' effect added to the selected elements.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public partial class SlideInEffect : DependencyObject
    {
        /// <summary>
        /// The proportional offset by which each line index
        /// gets translated on the X-axis.
        /// </summary>
        private const double ProportionalOffset = 50.0;

        /// <summary>
        /// The percentage of the total translation that plays
        /// using the exponential interpolation.
        /// </summary>
        private const double ExponentialInterpolationWeight = 0.90;

        /// <summary>
        /// Time in milliseconds at which the linear translation begins.
        /// </summary>
        private const double BeginTime = 350.0;

        /// <summary>
        /// Time in milliseconds at which the storyboard's interpolation
        /// changes from linear to exponential.
        /// </summary>
        private const double BreakTime = 420.0;

        /// <summary>
        /// Time in milliseconds at which the exponential translation ends.
        /// </summary>
        private const double EndTime = 1050.0;

        /// <summary>
        /// The easing function that defines the exponential interpolation
        /// of the storyboard.
        /// </summary>
        private static readonly ExponentialEase SlideInExponentialEase = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 5 };

        /// <summary>
        /// The property path used to map the animation's target property
        /// to the X property of a translate transform.
        /// </summary>
        private static readonly PropertyPath XPropertyPath = new PropertyPath("X");

        /// <summary>
        /// Identifies whether there was a SelectionChanged event
        /// triggered by a pivot.
        /// </summary>
        private static bool _selectionChanged;

        /// <summary>
        /// Identifies whether there was a ManipulationStarted event
        /// triggered by a pivot.
        /// </summary>
        private static bool _manipulatedStarted;

        /// <summary>
        /// Private manager that represents a correlation between Pivots
        /// and the number of indexed elements it contains.
        /// </summary>
        private static Dictionary<Pivot, int> _pivotsToElementCounters = new Dictionary<Pivot, int>();

        /// <summary>
        /// Private manager that represents a correlation between PivotItems 
        /// and the indexed elements it contains.
        /// </summary>
        private static Dictionary<PivotItem, List<FrameworkElement>> _pivotItemsToElements = new Dictionary<PivotItem, List<FrameworkElement>>();

        #region LineIndex DependencyProperty

        /// <summary>
        /// Gets the line index of the specified dependency object.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The line index.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Standard pattern.")]
        public static int GetLineIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(LineIndexProperty);
        }

        /// <summary>
        /// Sets the line index of the specified dependency object.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The line index.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Standard pattern.")]
        public static void SetLineIndex(DependencyObject obj, int value)
        {
            obj.SetValue(LineIndexProperty, value);
        }

        /// <summary>
        /// Identifies the line index of the current element,
        /// which is proportional to its initial offset before sliding in.
        /// </summary>
        public static readonly DependencyProperty LineIndexProperty =
            DependencyProperty.RegisterAttached("LineIndex", typeof(int), typeof(SlideInEffect), new PropertyMetadata(-1, OnLineIndexPropertyChanged));

        /// <summary>
        /// Modify the subscription of the dependency object 
        /// to the private managers based on the line index value.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnLineIndexPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement target = obj as FrameworkElement;

            if (target == null)
            {
                throw new InvalidOperationException("The dependency object must be a framework element.");
            }

            int index = (int)e.NewValue;

            if (index < 0)
            {
                // Dettach event handlers.
                if (SlideInEffect.GetHasEventsAttached(target))
                {
                    target.Loaded -= Target_Loaded;
                    target.Unloaded -= Target_Unloaded;
                    SlideInEffect.SetHasEventsAttached(target, false);
                }

                UnsubscribeFrameworkElement(target);
            }
            else
            {
                // Attach event handlers.
                if (!SlideInEffect.GetHasEventsAttached(target))
                {
                    target.Loaded += Target_Loaded;
                    target.Unloaded += Target_Unloaded;
                    SlideInEffect.SetHasEventsAttached(target, true);
                }

                SubscribeFrameworkElement(target);
            }
        }

        #endregion

        #region ParentPivot DependencyProperty

        /// <summary>
        /// Gets the parent pivot of the specified dependency object.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The pivot.</returns>
        private static Pivot GetParentPivot(DependencyObject obj)
        {
            return (Pivot)obj.GetValue(ParentPivotProperty);
        }

        /// <summary>
        /// Sets the parent pivot of the specified dependency object.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The pivot.</param>
        private static void SetParentPivot(DependencyObject obj, Pivot value)
        {
            obj.SetValue(ParentPivotProperty, value);
        }

        /// <summary>
        /// Identifies the ParentPivot dependency property.
        /// </summary>
        private static readonly DependencyProperty ParentPivotProperty =
            DependencyProperty.RegisterAttached("ParentPivot", typeof(Pivot), typeof(SlideInEffect), new PropertyMetadata(null, OnParentPivotPropertyChanged));

        /// <summary>
        /// Manages subscription to a pivot.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnParentPivotPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Pivot oldPivot = (Pivot)e.OldValue;
            Pivot newPivot = (Pivot)e.NewValue;

            if (newPivot != null)
            {
                if (!_pivotsToElementCounters.ContainsKey(newPivot))
                {
                    // Attach event handlers to the parent Pivot.
                    newPivot.SelectionChanged += Pivot_SelectionChanged;
                    newPivot.ManipulationStarted += Pivot_ManipulationStarted;
                    newPivot.ManipulationCompleted += Pivot_ManipulationCompleted;

                    _pivotsToElementCounters.Add(newPivot, 1);
                }
                else
                {
                    _pivotsToElementCounters[newPivot]++;
                }
            }
            else
            {
                if (_pivotsToElementCounters.ContainsKey(oldPivot))
                {
                    int count = --(_pivotsToElementCounters[oldPivot]);

                    if (count == 0)
                    {
                        // Dettach event handlers from the parent Pivot.
                        oldPivot.SelectionChanged -= Pivot_SelectionChanged;
                        oldPivot.ManipulationStarted -= Pivot_ManipulationStarted;
                        oldPivot.ManipulationCompleted -= Pivot_ManipulationCompleted;

                        _pivotsToElementCounters.Remove(oldPivot);
                    }
                }
            }
        }

        #endregion

        #region ParentPivotItem DependencyProperty

        /// <summary>
        /// Gets the parent pivot item of the specified dependency object.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The pivot item.</returns>
        private static PivotItem GetParentPivotItem(DependencyObject obj)
        {
            return (PivotItem)obj.GetValue(ParentPivotItemProperty);
        }

        /// <summary>
        /// Sets the parent pivot item of the specified dependency object.
        /// </summary>
        /// <param name="obj">The depedency object.</param>
        /// <param name="value">The pivot item.</param>
        private static void SetParentPivotItem(DependencyObject obj, PivotItem value)
        {
            obj.SetValue(ParentPivotItemProperty, value);
        }

        /// <summary>
        /// Identifies the ParentPivotItem dependency property.
        /// </summary>
        private static readonly DependencyProperty ParentPivotItemProperty =
            DependencyProperty.RegisterAttached("ParentPivotItem", typeof(PivotItem), typeof(SlideInEffect), new PropertyMetadata(null, OnParentPivotItemPropertyChanged));

        /// <summary>
        /// Manages subscription to a pivot item.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnParentPivotItemPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement target = (FrameworkElement)obj;
            PivotItem oldPivotItem = (PivotItem)e.OldValue;
            PivotItem newPivotItem = (PivotItem)e.NewValue;
            List<FrameworkElement> elements;

            if (newPivotItem != null)
            {        
                if(!_pivotItemsToElements.TryGetValue(newPivotItem, out elements))
                {
                    elements = new List<FrameworkElement>();
                    _pivotItemsToElements.Add(newPivotItem, elements);
                }

                elements.Add(target);
            }
            else
            {
                if(_pivotItemsToElements.TryGetValue(oldPivotItem, out elements))
                {
                    if (elements.Contains(target))
                    {
                        elements.Remove(target);
                    }

                    if (elements.Count == 0)
                    {
                        _pivotItemsToElements.Remove(oldPivotItem);
                    }
                }
            }
        }

        #endregion

        #region AttachedTransform DependencyProperty

        /// <summary>
        /// Gets the attached transform of the specified dependency object.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The attached transform.</returns>
        private static TranslateTransform GetAttachedTransform(DependencyObject obj)
        {
            return (TranslateTransform)obj.GetValue(AttachedTransformProperty);
        }

        /// <summary>
        /// Sets the attached transform of the specified dependency object.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The attached transform.</param>
        private static void SetAttachedTransform(DependencyObject obj, TranslateTransform value)
        {
            obj.SetValue(AttachedTransformProperty, value);
        }

        /// <summary>
        /// Identifies the AttachedTransform dependency property.
        /// </summary>
        private static readonly DependencyProperty AttachedTransformProperty =
            DependencyProperty.RegisterAttached("AttachedTransform", typeof(TranslateTransform), typeof(SlideInEffect), new PropertyMetadata(null));

        #endregion

        #region IsSubscribed DependencyProperty

        /// <summary>
        /// Gets whether the specified dependency object
        /// is subscribed to the private managers or not.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The value.</returns>
        private static bool GetIsSubscribed(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSubscribedProperty);
        }

        /// <summary>
        /// Sets whether the specified dependency object
        /// is subscribed to the private managers or not.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        private static void SetIsSubscribed(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSubscribedProperty, value);
        }

        /// <summary>
        /// Identifies the IsSubscribed dependency property.
        /// </summary>
        private static readonly DependencyProperty IsSubscribedProperty =
            DependencyProperty.RegisterAttached("IsSubscribed", typeof(bool), typeof(SlideInEffect), new PropertyMetadata(false, null));

        #endregion

        #region HasEventsAttached DependencyProperty

        /// <summary>
        /// Gets whether the specified dependency object
        /// has events attached to it or not.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The value.</returns>
        private static bool GetHasEventsAttached(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasEventsAttachedProperty);
        }

        /// <summary>
        /// Sets whether the specified dependency object
        /// has events attached to it or not.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        private static void SetHasEventsAttached(DependencyObject obj, bool value)
        {
            obj.SetValue(HasEventsAttachedProperty, value);
        }

        /// <summary>
        /// Identifies the HasEventsAttached dependency property.
        /// </summary>
        private static readonly DependencyProperty HasEventsAttachedProperty =
            DependencyProperty.RegisterAttached("HasEventsAttached", typeof(bool), typeof(SlideInEffect), new PropertyMetadata(false));

        #endregion

        /// <summary>
        /// Called when an element gets loaded.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private static void Target_Loaded(object sender, RoutedEventArgs e)
        {
            SubscribeFrameworkElement((FrameworkElement)sender);
        }

        /// <summary>
        /// Called when an element gets unloaded.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private static void Target_Unloaded(object sender, RoutedEventArgs e)
        {
            UnsubscribeFrameworkElement((FrameworkElement)sender);
        }

        /// <summary>
        /// Sets a flag indicating that a SelectionChanged event ocurred.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private static void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectionChanged = true;
        }

        /// <summary>
        /// Sets a flag indicating that a ManipulationStarted event ocurred.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private static void Pivot_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            _manipulatedStarted = true;
        }

        /// <summary>
        /// Animates the corresponding elements.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private static void Pivot_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (_selectionChanged && _manipulatedStarted)
            {
                Pivot pivot = (Pivot)sender;
                PivotItem pivotItem = pivot.ItemContainerGenerator.ContainerFromItem(pivot.SelectedItem) as PivotItem;

                if (pivotItem == null)
                {
                    return;
                }

                List<FrameworkElement> elements;

                if(!_pivotItemsToElements.TryGetValue(pivotItem, out elements))
                {
                    return;
                }

                Storyboard storyboard = new Storyboard();

                foreach (FrameworkElement target in elements)
                {
                    if (target != null)
                    {
                        if (IsOnScreen(target))
                        {
                            bool fromRight = (e.TotalManipulation.Translation.X <= 0);
                            ComposeStoryboard(target, fromRight, ref storyboard);
                        }
                    }
                }

                storyboard.Completed += (s1, e1) =>
                {
                    storyboard.Stop();
                };

                storyboard.Begin();
            }

            _selectionChanged = _manipulatedStarted = false;
        }

        /// <summary>
        /// Subscribes an element to the private managers.
        /// </summary>
        /// <param name="target">The framework element.</param>
        private static void SubscribeFrameworkElement(FrameworkElement target)
        {
            if (!SlideInEffect.GetIsSubscribed(target))
            {
                // Find the parent Pivot and PivotItem.
                Pivot pivot, pTemp;
                PivotItem pivotItem, iTemp;
                DependencyObject parent = VisualTreeHelper.GetParent(target);

                pTemp = null;
                pivotItem = iTemp = null;

                while ((pTemp == null) && (parent != null))
                {
                    pTemp = parent as Pivot;
                    iTemp = parent as PivotItem;

                    if (iTemp != null)
                    {
                        pivotItem = iTemp;
                    }

                    parent = VisualTreeHelper.GetParent(parent as DependencyObject);
                }

                if (parent == null || pivotItem == null)
                {
                    return;
                }
                else
                {
                    pivot = pTemp;
                }

                AttachTransform(target);
                SlideInEffect.SetParentPivot(target, pivot);
                SlideInEffect.SetParentPivotItem(target, pivotItem);
                SlideInEffect.SetIsSubscribed(target, true);
            }
        }

        /// <summary>
        /// Unsubscribes an element from the private managers.
        /// </summary>
        /// <param name="target">The framework element.</param>
        private static void UnsubscribeFrameworkElement(FrameworkElement target)
        {
            // If element is subscribed, unsubscribe.
            if (SlideInEffect.GetIsSubscribed(target))
            {
                SlideInEffect.SetParentPivot(target, null);
                SlideInEffect.SetParentPivotItem(target, null);
                SlideInEffect.SetIsSubscribed(target, false); 
            }                   
        }

        /// <summary>
        /// Adds an animation corresponding to an specific framework element.
        /// Thus, the storyboard can be composed piece by piece.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="leftToRight">
        /// Indicates whether the animation should go 
        /// from left to right or viceversa.
        /// </param>
        /// <param name="storyboard">A reference to the storyboard.</param>
        private static void ComposeStoryboard(FrameworkElement element, bool leftToRight, ref Storyboard storyboard)
        {
            double xPosition = SlideInEffect.GetLineIndex(element) * ProportionalOffset;
            double from = leftToRight ? xPosition : -xPosition;
            TranslateTransform translateTransform = SlideInEffect.GetAttachedTransform(element);
            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();

            LinearDoubleKeyFrame keyFrame1 = new LinearDoubleKeyFrame();
            keyFrame1.KeyTime = TimeSpan.Zero;
            keyFrame1.Value = from;
            animation.KeyFrames.Add(keyFrame1);

            LinearDoubleKeyFrame keyFrame2 = new LinearDoubleKeyFrame();
            keyFrame2.KeyTime = TimeSpan.FromMilliseconds(BeginTime);
            keyFrame2.Value = from;
            animation.KeyFrames.Add(keyFrame2);

            LinearDoubleKeyFrame keyFrame3 = new LinearDoubleKeyFrame();
            keyFrame3.KeyTime = TimeSpan.FromMilliseconds(BreakTime);
            keyFrame3.Value = from * ExponentialInterpolationWeight;
            animation.KeyFrames.Add(keyFrame3);

            EasingDoubleKeyFrame keyFrame4 = new EasingDoubleKeyFrame();
            keyFrame4.KeyTime = TimeSpan.FromMilliseconds(EndTime);
            keyFrame4.Value = 0.0;

            keyFrame4.EasingFunction = SlideInExponentialEase;
            animation.KeyFrames.Add(keyFrame4);

            Storyboard.SetTarget(animation, translateTransform);
            Storyboard.SetTargetProperty(animation, XPropertyPath);
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Indicates whether the specified framework element
        /// is within the bounds of the application's root visual.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <returns>
        /// True if the rectangular bounds of the framework element
        /// are completely outside the bounds of the application's root visual.
        /// </returns>
        private static bool IsOnScreen(FrameworkElement element)
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;

            if (root == null)
            {
                return false;
            }

            GeneralTransform generalTransform;
            double height = root.ActualHeight;                

            try
            {
                generalTransform = element.TransformToVisual(root);
            }
            catch (ArgumentException)
            {
                return false;
            }
                
            Rect bounds = new Rect(
                generalTransform.Transform(new Point(0, 0)), 
                generalTransform.Transform(new Point(element.ActualWidth, element.ActualHeight)));

            return (bounds.Bottom > 0 && bounds.Top < height);
        }

        /// <summary>
        /// Attach the translate transform that is used
        /// for the slide in effect to a framework element.
        /// </summary>
        /// <param name="element">The framework element.</param>
        private static void AttachTransform(FrameworkElement element)
        {
            Transform originalTransform = element.RenderTransform;
            TranslateTransform translateTransform = SlideInEffect.GetAttachedTransform(element);

            if (translateTransform == null)
            {
                translateTransform = new TranslateTransform() { X = 0 };

                if (originalTransform == null)
                {
                    element.RenderTransform = translateTransform;
                }
                else
                {
                    TransformGroup transformGroup = new TransformGroup();
                    transformGroup.Children.Add(originalTransform);
                    transformGroup.Children.Add(translateTransform);
                    element.RenderTransform = transformGroup;
                }

                SlideInEffect.SetAttachedTransform(element, translateTransform);
            }
        }
    }
}