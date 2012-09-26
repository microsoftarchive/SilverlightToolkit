// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls.Primitives;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Provides attached properties to feather FrameworkElements in
    /// and out during page transitions. The result is a 'turnstile feather' effect
    /// added to the select elements.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public sealed class TurnstileFeatherEffect : DependencyObject
    {
        /// <summary>
        /// The center of rotation on X for elements that are feathered.
        /// </summary>
        private const double FeatheringCenterOfRotationX = -0.2;

        /// <summary>
        /// The duration in milliseconds that each element takes
        /// to feather forward in.
        /// </summary>
        private const double ForwardInFeatheringDuration = 350.0;

        /// <summary>
        /// The initial angle position for an element 
        /// that feathers forward in.
        /// </summary>
        private const double ForwardInFeatheringAngle = -80.0;

        /// <summary>
        /// The delay in milliseconds between each element that 
        /// feathers forward in.
        /// </summary>
        private const double ForwardInFeatheringDelay = 40.0;

        /// <summary>
        /// The duration in milliseconds that each element takes
        /// to feather forward out.
        /// </summary>
        private const double ForwardOutFeatheringDuration = 250.0;

        /// <summary>
        /// The final angle position for an element 
        /// that feathers forward out.
        /// </summary>
        private const double ForwardOutFeatheringAngle = 50.0;

        /// <summary>
        /// The delay in milliseconds between each element that 
        /// feathers forward out.
        /// </summary>
        private const double ForwardOutFeatheringDelay = 50.0;

        /// <summary>
        /// The duration in milliseconds that each element takes
        /// to feather backward in.
        /// </summary>
        private const double BackwardInFeatheringDuration = 350.0;

        /// <summary>
        /// The initial angle position for an element 
        /// that feathers backward in.
        /// </summary>
        private const double BackwardInFeatheringAngle = 50.0;

        /// <summary>
        /// The delay in milliseconds between each element that 
        /// feathers backward in.
        /// </summary>
        private const double BackwardInFeatheringDelay = 50.0; 

        /// <summary>
        /// The duration in milliseconds that each element takes
        /// to feather backward out.
        /// </summary>
        private const double BackwardOutFeatheringDuration = 250.0;

        /// <summary>
        /// The initial angle position for an element 
        /// that feathers backward out.
        /// </summary>
        private const double BackwardOutFeatheringAngle = -80.0;

        /// <summary>
        /// The delay in milliseconds between each element that 
        /// feathers backward out.
        /// </summary>
        private const double BackwardOutFeatheringDelay = 40.0; 

        /// <summary>
        /// The easing function that defines the exponential inwards 
        /// interpolation of the storyboards.
        /// </summary>
        private static readonly ExponentialEase TurnstileFeatheringExponentialEaseIn = new ExponentialEase() { EasingMode = EasingMode.EaseIn, Exponent = 6 };

        /// <summary>
        /// The easing function that defines the exponential outwards
        /// interpolation of the storyboards.
        /// </summary>
        private static readonly ExponentialEase TurnstileFeatheringExponentialEaseOut = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 6 };

        /// <summary>
        /// The property path used to map the animation's target property
        /// to the RotationY property of the plane projection of a UI element.
        /// </summary>
        private static readonly PropertyPath RotationYPropertyPath = new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationY)");

        /// <summary>
        /// The property path used to map the animation's target property
        /// to the Opacity property of a UI element.
        /// </summary>
        private static readonly PropertyPath OpacityPropertyPath = new PropertyPath("(UIElement.Opacity)");

        /// <summary>
        /// A point with coordinate (0, 0).
        /// </summary>
        private static readonly Point Origin = new Point(0, 0);

        /// <summary>
        /// Private manager that represents a correlation between pages
        /// and the indexed elements it contains.
        /// </summary>
        private static Dictionary<PhoneApplicationPage, List<WeakReference>> _pagesToReferences = new Dictionary<PhoneApplicationPage, List<WeakReference>>();

        /// <summary>
        /// Identifies the set of framework elements that are targeted
        /// to be feathered.
        /// </summary>
        private static IList<WeakReference> _featheringTargets;

        /// <summary>
        /// Indicates whether the targeted framework elements need their
        /// projections and transforms to be restored.
        /// </summary>
        private static bool _pendingRestore;

        /// <summary>
        /// Default list of types that cannot be feathered.
        /// </summary>
        private static IList<Type> _nonPermittedTypes = new List<Type>() 
            { 
                typeof(PhoneApplicationFrame), 
                typeof(PhoneApplicationPage), 
                typeof(PivotItem),
                typeof(Panorama),
                typeof(PanoramaItem)
            };

        /// <summary>
        /// Default list of types that cannot be feathered.
        /// </summary>
        public static IList<Type> NonPermittedTypes
        {
            get { return _nonPermittedTypes; }
        }

        #region FeatheringIndex DependencyProperty

        /// <summary>
        /// Gets the feathering index of the specified dependency object.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The feathering index.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Standard pattern.")]
        public static int GetFeatheringIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(FeatheringIndexProperty);
        }

        /// <summary>
        /// Sets the feathering index of the specified dependency object.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The feathering index.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Standard pattern.")]
        public static void SetFeatheringIndex(DependencyObject obj, int value)
        {
            obj.SetValue(FeatheringIndexProperty, value);
        }

        /// <summary>
        /// Identifies the feathering index of the current element,
        /// which represents its place in the feathering order sequence.
        /// </summary>
        public static readonly DependencyProperty FeatheringIndexProperty =
            DependencyProperty.RegisterAttached("FeatheringIndex", typeof(int), typeof(TurnstileFeatherEffect), new PropertyMetadata(-1, OnFeatheringIndexPropertyChanged));

        /// <summary>
        /// Subscribes an element to the private manager.
        /// </summary>
        /// <param name="obj">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnFeatheringIndexPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement target = obj as FrameworkElement;

            if (target == null)
            {
                string message = string.Format(CultureInfo.InvariantCulture, "The dependency object must be of the type {0}.", typeof(FrameworkElement));
                throw new InvalidOperationException(message);
            }

            CheckForTypePermission(target);

            int index = (int)e.NewValue;

            if (index < 0)
            {
                // Dettach event handlers.
                if (TurnstileFeatherEffect.GetHasEventsAttached(target))
                {
                    target.SizeChanged -= Target_SizeChanged;
                    target.Unloaded -= Target_Unloaded;
                    TurnstileFeatherEffect.SetHasEventsAttached(target, false);
                }

                UnsubscribeFrameworkElement(target);
            }
            else
            {
                // Attach event handlers.
                if (!TurnstileFeatherEffect.GetHasEventsAttached(target))
                {
                    target.SizeChanged += Target_SizeChanged;
                    target.Unloaded += Target_Unloaded;
                    TurnstileFeatherEffect.SetHasEventsAttached(target, true);
                }

                SubscribeFrameworkElement(target);
            }
        }

        #endregion        

        #region ParentPage DependencyProperty

        /// <summary>
        /// Gets the parent page of the specified dependency object.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The page.</returns>
        private static PhoneApplicationPage GetParentPage(DependencyObject obj)
        {
            return (PhoneApplicationPage)obj.GetValue(ParentPageProperty);
        }

        /// <summary>
        /// Sets the parent page of the specified dependency object.
        /// </summary>
        /// <param name="obj">The depedency object.</param>
        /// <param name="value">The page.</param>
        private static void SetParentPage(DependencyObject obj, PhoneApplicationPage value)
        {
            obj.SetValue(ParentPageProperty, value);
        }

        /// <summary>
        /// Identifies the ParentPage dependency property.
        /// </summary>
        private static readonly DependencyProperty ParentPageProperty =
            DependencyProperty.RegisterAttached("ParentPage", typeof(PhoneApplicationPage), typeof(TurnstileFeatherEffect), new PropertyMetadata(null, OnParentPagePropertyChanged));

        /// <summary>
        /// Manages subscription to a page.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnParentPagePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement target = (FrameworkElement)obj;
            PhoneApplicationPage oldPage = (PhoneApplicationPage)e.OldValue;
            PhoneApplicationPage newPage = (PhoneApplicationPage)e.NewValue;
            List<WeakReference> references;

            if (newPage != null)
            {
                if (!_pagesToReferences.TryGetValue(newPage, out references))
                {
                    references = new List<WeakReference>();
                    _pagesToReferences.Add(newPage, references);
                }
                else
                {
                    WeakReferenceHelper.RemoveNullTargetReferences(references);
                }

                if (!WeakReferenceHelper.ContainsTarget(references, target))
                {
                    references.Add(new WeakReference(target));
                }

                references.Sort(SortReferencesByIndex);
            }
            else
            {
                if(_pagesToReferences.TryGetValue(oldPage, out references))
                {
                    WeakReferenceHelper.TryRemoveTarget(references, target);

                    if (references.Count == 0)
                    {
                        _pagesToReferences.Remove(oldPage);
                    }
                }
            }       
        }

        #endregion

        #region IsSubscribed DependencyProperty

        /// <summary>
        /// Gets whether the specified dependency object
        /// is subscribed to the private manager or not.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The value.</returns>        
        private static bool GetIsSubscribed(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSubscribedProperty);
        }

        /// <summary>
        /// Sets whether the specified dependency object
        /// is subscribed to the private manager or not.
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
            DependencyProperty.RegisterAttached("IsSubscribed", typeof(bool), typeof(TurnstileFeatherEffect), new PropertyMetadata(false));

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
            DependencyProperty.RegisterAttached("HasEventsAttached", typeof(bool), typeof(TurnstileFeatherEffect), new PropertyMetadata(false));

        #endregion

        #region OriginalProjection DependencyProperty

        /// <summary>
        /// Gets the original projection of the specified dependency object
        /// after the projection needed to apply the turnstile feather effect
        /// has been attached to it.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The original projection.</returns>
        private static Projection GetOriginalProjection(DependencyObject obj)
        {
            return (Projection)obj.GetValue(OriginalProjectionProperty);
        }

        /// <summary>
        /// Sets the original projection of the specified dependency object.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The original projection.</param>
        private static void SetOriginalProjection(DependencyObject obj, Projection value)
        {
            obj.SetValue(OriginalProjectionProperty, value);
        }

        /// <summary>
        /// Identifies the OriginalProjection dependency property.
        /// </summary>
        private static readonly DependencyProperty OriginalProjectionProperty =
            DependencyProperty.RegisterAttached("OriginalProjection", typeof(Projection), typeof(TurnstileFeatherEffect), new PropertyMetadata(null));

        #endregion

        #region OriginalRenderTransform DependencyProperty

        /// <summary>
        /// Gets the original render transform of the specified dependency 
        /// object after the transform needed to apply the turnstile feather 
        /// effect has been attached to it.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The original render transform.</returns>
        private static Transform GetOriginalRenderTransform(DependencyObject obj)
        {
            return (Transform)obj.GetValue(OriginalRenderTransformProperty);
        }

        /// <summary>
        /// Sets the original render transform of the specified 
        /// dependency object.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The original render transform.</param>
        private static void SetOriginalRenderTransform(DependencyObject obj, Transform value)
        {
            obj.SetValue(OriginalRenderTransformProperty, value);
        }

        /// <summary>
        /// Identifies the OriginalRenderTransform dependency property.
        /// </summary>
        private static readonly DependencyProperty OriginalRenderTransformProperty =
            DependencyProperty.RegisterAttached("OriginalRenderTransform", typeof(Transform), typeof(TurnstileFeatherEffect), new PropertyMetadata(null)); 

        #endregion

        #region OriginalOpacity DependencyProperty

        /// <summary>
        /// Gets the original opacity of the specified dependency 
        /// object before the turnstile feather effect is applied to it.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The original opacity.</returns>
        private static double GetOriginalOpacity(DependencyObject obj)
        {
            return (double)obj.GetValue(OriginalOpacityProperty);
        }

        /// <summary>
        /// Sets the original opacity of the specified 
        /// dependency object.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The original opacity.</param>
        private static void SetOriginalOpacity(DependencyObject obj, double value)
        {
            obj.SetValue(OriginalOpacityProperty, value);
        }

        /// <summary>
        /// Identifies the OriginalOpacity dependency property.
        /// </summary>
        private static readonly DependencyProperty OriginalOpacityProperty =
            DependencyProperty.RegisterAttached("OriginalOpacity", typeof(double), typeof(TurnstileFeatherEffect), new PropertyMetadata(0.0));

        #endregion

        /// <summary>
        /// Called when an element gets resized.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        /// <remarks>
        /// Ideally, the Loaded event should be handled instead of
        /// the SizeChanged event. However, the Loaded event does not occur
        /// by the time the TransitionFrame tries to animate a forward in transition.
        /// Handling the SizeChanged event instead guarantees that
        /// the newly created FrameworkElements can be subscribed in time
        /// before the transition begins.
        /// </remarks>
        private static void Target_SizeChanged(object sender, SizeChangedEventArgs e)
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
        /// Throws an exception if the object sent as parameter is of a type
        /// that is included in the list of non-permitted types.
        /// </summary>
        /// <param name="obj">The object.</param>
        private static void CheckForTypePermission(object obj)
        {
            Type type = obj.GetType();

            if(NonPermittedTypes.Contains(type))
            {
                string message = string.Format(CultureInfo.InvariantCulture, "Objects of the type {0} cannot be feathered.", type);
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Compares two weak references targeting dependency objects
        /// to sort them based on their feathering index.
        /// </summary>
        /// <param name="x">The first weak reference.</param>
        /// <param name="y">The second weak reference.</param>
        /// <returns>
        /// 0 if both weak references target dependency objects with
        /// the same feathering index.
        /// 1 if the first reference targets a dependency 
        /// object with a greater feathering index.
        /// -1 if the second reference targets a dependency 
        /// object with a greater feathering index.       
        /// </returns>
        private static int SortReferencesByIndex(WeakReference x, WeakReference y)
        {
            DependencyObject targetX = x.Target as DependencyObject;
            DependencyObject targetY = y.Target as DependencyObject;

            if (targetX == null)
            {
                if (targetY == null)
                {
                    // If x is null and y is null, 
                    // they're equal. 
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, 
                    // y is greater. 
                    return -1;
                }
            }
            else
            {
                // If x is not null and y is null,
                //  x is greater.
                if (targetY == null)
                {
                    return 1;
                }
                else
                {
                    int xIndex = GetFeatheringIndex(targetX);
                    int yIndex = GetFeatheringIndex(targetY);

                    return xIndex.CompareTo(yIndex);
                }
            }
        }

        /// <summary>
        /// Returns the set of weak references to the items 
        /// that must be animated.
        /// </summary>
        /// <returns>
        /// A set of weak references to items sorted by their feathering index.
        /// </returns>
        private static IList<WeakReference> GetTargetsToAnimate()
        {
            List<WeakReference> references;
            List<WeakReference> targets = new List<WeakReference>();
            PhoneApplicationPage page = null;
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;

            if (frame != null)
            {
                page = frame.Content as PhoneApplicationPage;
            }

            if (page == null)
            {
                return null;
            }

            if (!_pagesToReferences.TryGetValue(page, out references))
            {
                return null;
            }

            foreach (WeakReference r in references)
            {
                FrameworkElement target = r.Target as FrameworkElement;

                // If target is null, skip.
                if (target == null)
                {
                    continue;
                }

                // If target is not on the screen, skip.
                if (!IsOnScreen(target))
                {
                    continue;
                }
         
                ListBox listBox = r.Target as ListBox;
                LongListSelector longListSelector = r.Target as LongListSelector;
                Pivot pivot = r.Target as Pivot;

                if (listBox != null)
                {
                    // If the target is a ListBox, feather its items individually.
                    ItemsControlExtensions.GetItemsInViewPort(listBox, targets);
                }
                else if (longListSelector != null)
                {
                    // If the target is a LongListSelector, feather its items individually.
                    ListBox child = TemplatedVisualTreeExtensions.GetFirstLogicalChildByType<ListBox>(longListSelector, false);

                    if (child != null)
                    {
                        ItemsControlExtensions.GetItemsInViewPort(child, targets);
                    }
                }
                else if (pivot != null)
                {
                    // If the target is a Pivot, feather the title and the headers individually.
                    ContentPresenter title = TemplatedVisualTreeExtensions.GetFirstLogicalChildByType<ContentPresenter>(pivot, false);

                    if (title != null)
                    {
                        targets.Add(new WeakReference(title));
                    }

                    PivotHeadersControl headers = TemplatedVisualTreeExtensions.GetFirstLogicalChildByType<PivotHeadersControl>(pivot, false);

                    if (headers != null)
                    {
                        targets.Add(new WeakReference(headers));
                    }
                }
                else
                {
                    // Else, feather the target as a whole.
                    targets.Add(r);
                }
            }    

            return targets;
        }

        /// <summary>
        /// Subscribes an element to the private managers.
        /// </summary>
        /// <param name="target">The framework element.</param>
        private static void SubscribeFrameworkElement(FrameworkElement target)
        {
            if (!TurnstileFeatherEffect.GetIsSubscribed(target))
            {
                // Find the parent page.
                PhoneApplicationPage page = TemplatedVisualTreeExtensions.GetParentByType<PhoneApplicationPage>(target);

                if (page == null)
                {
                    return;
                }

                TurnstileFeatherEffect.SetParentPage(target, page);
                TurnstileFeatherEffect.SetIsSubscribed(target, true);
            }
        }

        /// <summary>
        /// Unsubscribes an element from the private manager.
        /// </summary>
        /// <param name="target">The framework element.</param>
        private static void UnsubscribeFrameworkElement(FrameworkElement target)
        {
            // If element is subscribed, unsubscribe.
            if (TurnstileFeatherEffect.GetIsSubscribed(target))
            {
                TurnstileFeatherEffect.SetParentPage(target, null);
                TurnstileFeatherEffect.SetIsSubscribed(target, false);
            }
        }
        
        /// <summary>
        /// Prepares a framework element to be feathered by adding a plane projection
        /// and a composite transform to it.
        /// </summary>
        /// <param name="root">The root visual.</param>
        /// <param name="element">The framework element.</param>
        private static bool TryAttachProjectionAndTransform(PhoneApplicationFrame root, FrameworkElement element)
        {
            GeneralTransform generalTransform;

            try
            {
                generalTransform = element.TransformToVisual(root);
            }
            catch (ArgumentException)
            {
                return false;
            }

            Point coordinates = generalTransform.Transform(Origin);
            double y = coordinates.Y + element.ActualHeight / 2.0;
            double offset = (root.ActualHeight / 2.0) - y;

            // Cache original projection and transform.
            TurnstileFeatherEffect.SetOriginalProjection(element, element.Projection);
            TurnstileFeatherEffect.SetOriginalRenderTransform(element, element.RenderTransform);

            // Attach projection.
            PlaneProjection projection = new PlaneProjection();
            projection.GlobalOffsetY = offset * -1.0;
            projection.CenterOfRotationX = FeatheringCenterOfRotationX;
            element.Projection = projection;

            // Attach transform.
            Transform originalTransform = element.RenderTransform;
            TranslateTransform translateTransform = new TranslateTransform();
            translateTransform.Y = offset;
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(originalTransform);
            transformGroup.Children.Add(translateTransform);
            element.RenderTransform = transformGroup;

            return true;
        }

        /// <summary>
        /// Restores the original projection and render transform of
        /// the targeted framework elements.
        /// </summary>
        private static void RestoreProjectionsAndTransforms()
        {
            if (_featheringTargets == null || !_pendingRestore)
            {
                return;
            }

            foreach (WeakReference r in _featheringTargets)
            {
                FrameworkElement element = r.Target as FrameworkElement;

                if (element == null)
                {
                    continue;
                }                

                Projection projection = TurnstileFeatherEffect.GetOriginalProjection(element);
                Transform transform = TurnstileFeatherEffect.GetOriginalRenderTransform(element);

                element.Projection = projection;
                element.RenderTransform = transform;
            }

            _pendingRestore = false;
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
            double width = root.ActualWidth;

            try
            {
                generalTransform = element.TransformToVisual(root);
            }
            catch (ArgumentException)
            {
                return false;
            }

            Rect bounds = new Rect(
                generalTransform.Transform(Origin),
                generalTransform.Transform(new Point(element.ActualWidth, element.ActualHeight)));

            bool isParentTransparent = false;
            IList<FrameworkElement> ancestors = element.GetVisualAncestors().ToList();

            if (ancestors != null)
            {
                for (int i = 0; i < ancestors.Count; i++)
                {
                    if (ancestors[i].Opacity <= 0.001)
                    {
                        isParentTransparent = true;
                        break;
                    }
                }
            }

            return (bounds.Bottom > 0) && (bounds.Top < height) 
                && (bounds.Right > 0) && (bounds.Left < width) 
                && !isParentTransparent;
        }

        /// <summary>
        /// Adds a set of animations corresponding to the 
        /// turnstile feather forward in effect.
        /// </summary>
        /// <param name="storyboard">
        /// The storyboard where the animations
        /// will be added.
        /// </param>
        private static void ComposeForwardInStoryboard(Storyboard storyboard)
        {     
            int counter = 0;
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;

            foreach (WeakReference r in _featheringTargets)
            {
                FrameworkElement element = (FrameworkElement)r.Target;
                double originalOpacity = element.Opacity;
                TurnstileFeatherEffect.SetOriginalOpacity(element, originalOpacity);

                // Hide the element until the storyboard is begun.
                element.Opacity = 0.0; 

                if(!TryAttachProjectionAndTransform(root, element))
                {
                    continue;
                }

                DoubleAnimation doubleAnimation = new DoubleAnimation()
                {
                    Duration = TimeSpan.FromMilliseconds(ForwardInFeatheringDuration),
                    From = ForwardInFeatheringAngle,
                    To = 0.0,
                    BeginTime = TimeSpan.FromMilliseconds(ForwardInFeatheringDelay * counter),
                    EasingFunction = TurnstileFeatheringExponentialEaseOut
                };

                Storyboard.SetTarget(doubleAnimation, element);
                Storyboard.SetTargetProperty(doubleAnimation, RotationYPropertyPath);
                storyboard.Children.Add(doubleAnimation);

                doubleAnimation = new DoubleAnimation()
                {
                    Duration = TimeSpan.Zero,
                    From = 0.0,
                    To = TurnstileFeatherEffect.GetOriginalOpacity(element),
                    BeginTime = TimeSpan.FromMilliseconds(ForwardInFeatheringDelay * counter)
                };

                Storyboard.SetTarget(doubleAnimation, element);
                Storyboard.SetTargetProperty(doubleAnimation, OpacityPropertyPath);
                storyboard.Children.Add(doubleAnimation);

                counter++;
            }
        }

        /// <summary>
        /// Adds a set of animations corresponding to the 
        /// turnstile feather forward out effect.
        /// </summary>
        /// <param name="storyboard">
        /// The storyboard where the animations
        /// will be added.
        /// </param>
        private static void ComposeForwardOutStoryboard(Storyboard storyboard)
        {
            int counter = 0;
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;

            foreach (WeakReference r in _featheringTargets)
            {
                FrameworkElement element = (FrameworkElement)r.Target;
                double originalOpacity = element.Opacity;
                TurnstileFeatherEffect.SetOriginalOpacity(element, originalOpacity);     

                if (!TryAttachProjectionAndTransform(root, element))
                {
                    continue;
                }

                DoubleAnimation doubleAnimation = new DoubleAnimation()
                {
                    Duration = TimeSpan.FromMilliseconds(ForwardOutFeatheringDuration),
                    From = 0.0,
                    To = ForwardOutFeatheringAngle,
                    BeginTime = TimeSpan.FromMilliseconds(ForwardOutFeatheringDelay * counter),
                    EasingFunction = TurnstileFeatheringExponentialEaseIn
                };         

                Storyboard.SetTarget(doubleAnimation, element);
                Storyboard.SetTargetProperty(doubleAnimation, RotationYPropertyPath);
                storyboard.Children.Add(doubleAnimation);

                doubleAnimation = new DoubleAnimation()
                {
                    Duration = TimeSpan.Zero,
                    From = originalOpacity,
                    To = 0.0,
                    BeginTime = TimeSpan.FromMilliseconds(ForwardOutFeatheringDelay * counter + ForwardOutFeatheringDuration)
                };

                Storyboard.SetTarget(doubleAnimation, element);
                Storyboard.SetTargetProperty(doubleAnimation, OpacityPropertyPath);
                storyboard.Children.Add(doubleAnimation);

                counter++;
            }
        }

        /// <summary>
        /// Adds a set of animations corresponding to the 
        /// turnstile feather backward in effect.
        /// </summary>
        /// <param name="storyboard">
        /// The storyboard where the animations
        /// will be added.
        /// </param>
        private static void ComposeBackwardInStoryboard(Storyboard storyboard)
        {
            int counter = 0;
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;

            foreach (WeakReference r in _featheringTargets)
            {
                FrameworkElement element = (FrameworkElement)r.Target;
                double originalOpacity = element.Opacity;
                TurnstileFeatherEffect.SetOriginalOpacity(element, originalOpacity);

                // Hide the element until the storyboard is begun.
                element.Opacity = 0.0; 

                if (!TryAttachProjectionAndTransform(root, element))
                {
                    continue;
                }

                DoubleAnimation doubleAnimation = new DoubleAnimation()
                {
                    Duration = TimeSpan.FromMilliseconds(BackwardInFeatheringDuration),
                    From = BackwardInFeatheringAngle,
                    To = 0.0,
                    BeginTime = TimeSpan.FromMilliseconds(BackwardInFeatheringDelay * counter),
                    EasingFunction = TurnstileFeatheringExponentialEaseOut
                };

                Storyboard.SetTarget(doubleAnimation, element);
                Storyboard.SetTargetProperty(doubleAnimation, RotationYPropertyPath);
                storyboard.Children.Add(doubleAnimation);

                doubleAnimation = new DoubleAnimation()
                {
                    Duration = TimeSpan.Zero,
                    From = 0.0,
                    To = originalOpacity,
                    BeginTime = TimeSpan.FromMilliseconds(BackwardInFeatheringDelay * counter)
                };

                Storyboard.SetTarget(doubleAnimation, element);
                Storyboard.SetTargetProperty(doubleAnimation, OpacityPropertyPath);
                storyboard.Children.Add(doubleAnimation);

                counter++;
            }
        }

        /// <summary>
        /// Adds a set of animations corresponding to the 
        /// turnstile feather backward out effect.
        /// </summary>
        /// <param name="storyboard">
        /// The storyboard where the animations
        /// will be added.
        /// </param>
        private static void ComposeBackwardOutStoryboard(Storyboard storyboard)
        {
            int counter = 0;
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;

            foreach (WeakReference r in _featheringTargets)
            {
                FrameworkElement element = (FrameworkElement)r.Target;
                double originalOpacity = element.Opacity;
                TurnstileFeatherEffect.SetOriginalOpacity(element, originalOpacity);                 

                if (!TryAttachProjectionAndTransform(root, element))
                {
                    continue;
                }

                DoubleAnimation doubleAnimation = new DoubleAnimation()
                {
                    Duration = TimeSpan.FromMilliseconds(BackwardOutFeatheringDuration),
                    From = 0.0,
                    To = BackwardOutFeatheringAngle,
                    BeginTime = TimeSpan.FromMilliseconds(BackwardOutFeatheringDelay * counter),
                    EasingFunction = TurnstileFeatheringExponentialEaseIn
                };

                Storyboard.SetTarget(doubleAnimation, element);
                Storyboard.SetTargetProperty(doubleAnimation, RotationYPropertyPath);
                storyboard.Children.Add(doubleAnimation);

                doubleAnimation = new DoubleAnimation()
                {
                    Duration = TimeSpan.Zero,
                    From = originalOpacity,
                    To = 0.0,
                    BeginTime = TimeSpan.FromMilliseconds((BackwardOutFeatheringDelay * counter) + BackwardOutFeatheringDuration)
                };

                Storyboard.SetTarget(doubleAnimation, element);
                Storyboard.SetTargetProperty(doubleAnimation, OpacityPropertyPath);
                storyboard.Children.Add(doubleAnimation);

                counter++;
            }
        }

        /// <summary>
        /// Adds a set of animations corresponding to the 
        /// turnstile feather effect.
        /// </summary>
        /// <param name="storyboard">
        /// The storyboard where the animations
        /// will be added.</param>
        /// <param name="beginTime">
        /// The time at which the storyboard should begin.</param>
        /// <param name="mode">
        /// The mode of the turnstile feather effect.
        /// </param>
        internal static void ComposeStoryboard(Storyboard storyboard, TimeSpan? beginTime, TurnstileFeatherTransitionMode mode)
        {
            RestoreProjectionsAndTransforms();

            _featheringTargets = GetTargetsToAnimate();

            if (_featheringTargets == null)
            {
                return;
            }

            _pendingRestore = true;

            switch (mode)
            {
                case TurnstileFeatherTransitionMode.ForwardIn:
                    ComposeForwardInStoryboard(storyboard);
                    break;
                case TurnstileFeatherTransitionMode.ForwardOut:
                    ComposeForwardOutStoryboard(storyboard);
                    break;
                case TurnstileFeatherTransitionMode.BackwardIn:
                    ComposeBackwardInStoryboard(storyboard);
                    break;
                case TurnstileFeatherTransitionMode.BackwardOut:
                    ComposeBackwardOutStoryboard(storyboard);
                    break;
                default:
                    break;
            }

            storyboard.BeginTime = beginTime;

            storyboard.Completed += (s, e) =>
            {
                foreach (WeakReference r in _featheringTargets)
                {
                    FrameworkElement element = (FrameworkElement)r.Target;
                    double originalOpacity = TurnstileFeatherEffect.GetOriginalOpacity(element);
                    element.Opacity = originalOpacity;
                }

                RestoreProjectionsAndTransforms();
            };
        }
    }
}