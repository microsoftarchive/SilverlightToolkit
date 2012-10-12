// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Represents a collection of items that can be expanded or collapsed.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    [TemplateVisualState(Name = CollapsedState, GroupName = ExpansionStates)]
    [TemplateVisualState(Name = ExpandedState, GroupName = ExpansionStates)]
    [TemplateVisualState(Name = ExpandableState, GroupName = ExpandabilityStates)]
    [TemplateVisualState(Name = NonExpandableState, GroupName = ExpandabilityStates)]
    [TemplatePart(Name = Presenter, Type = typeof(ItemsPresenter))]
    [TemplatePart(Name = ExpanderPanel, Type = typeof(Grid))]
    [TemplatePart(Name = ExpandedStateAnimation, Type = typeof(DoubleAnimation))]
    [TemplatePart(Name = CollapsedToExpandedKeyFrame, Type = typeof(EasingDoubleKeyFrame))]
    [TemplatePart(Name = ExpandedToCollapsedKeyFrame, Type = typeof(EasingDoubleKeyFrame))]
    public class ExpanderView : HeaderedItemsControl
    {
        /// <summary>
        /// Expansion visual states.
        /// </summary>
        public const string ExpansionStates = "ExpansionStates";

        /// <summary>
        /// Expandability visual states.
        /// </summary>
        public const string ExpandabilityStates = "ExpandabilityStates";

        /// <summary>
        /// Collapsed visual state.
        /// </summary>
        public const string CollapsedState = "Collapsed";

        /// <summary>
        /// Expanded visual state.
        /// </summary>
        public const string ExpandedState = "Expanded";

        /// <summary>
        /// Expandable visual state.
        /// </summary>
        public const string ExpandableState = "Expandable";

        /// <summary>
        /// NonExpandable visual state.
        /// </summary>
        public const string NonExpandableState = "NonExpandable";

        /// <summary>
        /// Presenter template part name.
        /// </summary>
        private const string Presenter = "Presenter";

        /// <summary>
        /// Expander Panel template part name.
        /// </summary>
        private const string ExpanderPanel = "ExpanderPanel";

        /// <summary>
        /// Expanded State Animation template part name.
        /// </summary>
        private const string ExpandedStateAnimation = "ExpandedStateAnimation";

        /// <summary>
        /// Collapsed to Expanded Key Frame template part name.
        /// </summary>
        private const string CollapsedToExpandedKeyFrame = "CollapsedToExpandedKeyFrame";

        /// <summary>
        /// Expanded to Collapsed Key Frame template part name.
        /// </summary>
        private const string ExpandedToCollapsedKeyFrame = "ExpandedToCollapsedKeyFrame";

        /// <summary>
        /// Presenter template part.
        /// </summary>
        private ItemsPresenter _presenter;

        /// <summary>
        /// Canvas template part
        /// </summary>
        private Canvas _itemsCanvas;

        /// <summary>
        /// Expander Panel template part.
        /// </summary>
        private Grid _expanderPanel;

        /// <summary>
        /// Expanded State Animation template part.
        /// </summary>
        private DoubleAnimation _expandedStateAnimation;

        /// <summary>
        /// Collapsed to Expanded Key Frame template part.
        /// </summary>
        private EasingDoubleKeyFrame _collapsedToExpandedFrame;

        /// <summary>
        /// Expanded to Collapsed Key Frame template part.
        /// </summary>
        private EasingDoubleKeyFrame _expandedToCollapsedFrame;

        /// <summary>
        /// Step between the keytimes of drop-down animations
        /// to create a feather effect.
        /// </summary>
        private const int KeyTimeStep = 35;

        /// <summary>
        /// Initial key time for drop-down animations.
        /// </summary>
        private const int InitialKeyTime = 225;

        /// <summary>
        /// Final key time for drop-down animations.
        /// </summary>
        private const int FinalKeyTime = 250;

        /// <summary>
        /// Occurs when the Expander View opens to display its content.
        /// </summary>
        public event RoutedEventHandler Expanded;

        /// <summary>
        /// Occurs when the Expander View closes to hide its content.
        /// </summary>
        public event RoutedEventHandler Collapsed;

        #region Expander DependencyProperty

        /// <summary>
        /// Gets or sets the expander object.
        /// </summary>
        public object Expander
        {
            get { return (object)GetValue(ExpanderProperty); }
            set { SetValue(ExpanderProperty, value); }
        }

        /// <summary>
        /// Identifies the Expander dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpanderProperty =
            DependencyProperty.Register("Expander", typeof(object), typeof(ExpanderView), new PropertyMetadata(null, OnExpanderPropertyChanged));


        /// <summary>
        /// ExpanderProperty changed handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The event information.</param>
        private static void OnExpanderPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ExpanderView source = (ExpanderView)obj;
            source.OnExpanderChanged(e.OldValue, e.NewValue);
        }

        #endregion

        #region ExpanderTemplate DependencyProperty

        /// <summary>
        /// Gets or sets the data template that defines
        /// the expander.
        /// </summary>
        public DataTemplate ExpanderTemplate
        {
            get { return (DataTemplate)GetValue(ExpanderTemplateProperty); }
            set { SetValue(ExpanderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the ExpanderTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpanderTemplateProperty =
            DependencyProperty.Register("ExpanderTemplate", typeof(DataTemplate), typeof(ExpanderView), new PropertyMetadata(null, OnExpanderTemplatePropertyChanged));

        /// <summary>
        /// ExpanderTemplateProperty changed handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The event information.</param>
        private static void OnExpanderTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ExpanderView source = (ExpanderView)obj;
            source.OnExpanderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        #endregion

        #region NonExpandableHeader

        /// <summary>
        /// Gets or sets the header object that is displayed
        /// when the Expander View is non-expandable.
        /// </summary>
        public object NonExpandableHeader
        {
            get { return (object)GetValue(NonExpandableHeaderProperty); }
            set { SetValue(NonExpandableHeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the NonExpandableHeader dependency property.
        /// </summary>
        public static readonly DependencyProperty NonExpandableHeaderProperty =
            DependencyProperty.Register("NonExpandableHeader", typeof(object), typeof(ExpanderView), new PropertyMetadata(null, OnNonExpandableHeaderPropertyChanged));

        /// <summary>
        /// NonExpandableHeaderProperty changed handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The event information.</param>
        private static void OnNonExpandableHeaderPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ExpanderView source = (ExpanderView)obj;
            source.OnNonExpandableHeaderChanged(e.OldValue, e.NewValue);
        }

        #endregion

        #region NonExpandableHeadeTemplate DependencyProperty

        /// <summary>
        /// Gets or sets the data template that defines
        /// the non-expandable header.
        /// </summary>
        public DataTemplate NonExpandableHeaderTemplate
        {
            get { return (DataTemplate)GetValue(NonExpandableHeaderTemplateProperty); }
            set { SetValue(NonExpandableHeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the NonExpandableHeaderTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty NonExpandableHeaderTemplateProperty =
            DependencyProperty.Register("NonExpandableHeaderTemplate", typeof(DataTemplate), typeof(ExpanderView), new PropertyMetadata(null, OnNonExpandableHeaderTemplatePropertyChanged));

        /// <summary>
        /// NonExpandableHeaderTemplate changed handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The event information.</param>
        private static void OnNonExpandableHeaderTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ExpanderView source = (ExpanderView)obj;
            source.OnNonExpandableHeaderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        #endregion

        #region IsExpanded DependencyProperty

        /// <summary>
        /// Gets or sets the flag that indicates whether the
        /// Expander View is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set
            {
                if (!IsNonExpandable)
                {
                    SetValue(IsExpandedProperty, value);
                }
                else
                {
                    throw new InvalidOperationException(Properties.Resources.InvalidExpanderViewOperation);
                }
            }
        }

        /// <summary>
        /// Identifies the IsExpanded dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ExpanderView), new PropertyMetadata(false, OnIsExpandedPropertyChanged));

        /// <summary>
        /// IsExpandedProperty changed handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The event information.</param>
        private static void OnIsExpandedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ExpanderView source = (ExpanderView)obj;

            RoutedEventArgs args = new RoutedEventArgs();
            if ((bool)e.NewValue)
            {
                source.OnExpanded(args);
            }
            else
            {
                source.OnCollapsed(args);
            }

            source.UpdateVisualState(true);
        }

        #endregion

        #region HasItems DependencyProperty

        /// <summary>
        /// Gets or sets the flag that indicates whether the
        /// Expander View has items.
        /// </summary>
        public bool HasItems
        {
            get { return (bool)GetValue(HasItemsProperty); }
            set { SetValue(HasItemsProperty, value); }
        }

        /// <summary>
        /// Identifies the HasItems dependency property.
        /// </summary>
        public static readonly DependencyProperty HasItemsProperty =
            DependencyProperty.Register("HasItems", typeof(bool), typeof(ExpanderView), new PropertyMetadata(false, null));

        #endregion

        #region IsNonExpandable

        /// <summary>
        /// Gets or sets the flag that indicates whether the
        /// Expander View is non-expandable.
        /// </summary>
        public bool IsNonExpandable
        {
            get { return (bool)GetValue(IsNonExpandableProperty); }
            set { SetValue(IsNonExpandableProperty, value); }
        }

        /// <summary>
        /// Identifies the NonExpandable dependency property.
        /// </summary>
        public static readonly DependencyProperty IsNonExpandableProperty =
            DependencyProperty.Register("IsNonExpandable", typeof(bool), typeof(ExpanderView), new PropertyMetadata(false, OnIsNonExpandablePropertyChanged));

        /// <summary>
        /// IsNonExpandableProperty changed handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The event information.</param>
        private static void OnIsNonExpandablePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ExpanderView source = (ExpanderView)obj;

            if ((bool)e.NewValue)
            {
                if (source.IsExpanded)
                {
                    source.IsExpanded = false;
                }
            }

            source.UpdateVisualState(true);
        }

        #endregion

        /// <summary>
        /// Gets the template parts and sets event handlers.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (_expanderPanel != null)
            {
                _expanderPanel.Tap -= OnExpanderPanelTap;
            }

            base.OnApplyTemplate();

            _expanderPanel = base.GetTemplateChild(ExpanderPanel) as Grid;
            _expandedToCollapsedFrame = base.GetTemplateChild(ExpandedToCollapsedKeyFrame) as EasingDoubleKeyFrame;
            _collapsedToExpandedFrame = base.GetTemplateChild(CollapsedToExpandedKeyFrame) as EasingDoubleKeyFrame;
            _itemsCanvas = base.GetTemplateChild("ItemsCanvas") as Canvas;

            VisualState expandedState = (base.GetTemplateChild(ExpandedState) as VisualState);
            if (expandedState != null)
            {
                _expandedStateAnimation = expandedState.Storyboard.Children[0] as DoubleAnimation;
            }
            
            _presenter = base.GetTemplateChild(Presenter) as ItemsPresenter;
            if (_presenter != null)
            {
                _presenter.SizeChanged += OnPresenterSizeChanged;
            }
            
            if (_expanderPanel != null)
            {
                _expanderPanel.Tap += OnExpanderPanelTap;
            }

            UpdateVisualState(false);
        }

        /// <summary>
        /// Initializes a new instance of the ExpanderView class.
        /// </summary>
        public ExpanderView()
        {
            DefaultStyleKey = typeof(ExpanderView);
            SizeChanged += OnSizeChanged;
        }

        /// <summary>
        /// Recalculates the size of the presenter to match its parent.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event args</param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_presenter != null)
            {
                UIElement parent = VisualTreeHelper.GetParent(_presenter) as UIElement;
                while (!(parent is ExpanderView))
                {
                    parent = VisualTreeHelper.GetParent(parent) as UIElement;
                }
                GeneralTransform gt = parent.TransformToVisual(_presenter);
                Point childToParentCoordinates = gt.Transform(new Point(0, 0));
                _presenter.Width = parent.RenderSize.Width + childToParentCoordinates.X;
            }
        }

        /// <summary>
        /// Recalculates size of canvas based on the size change for the presenter.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event args</param>
        private void OnPresenterSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (null != _itemsCanvas && null != _presenter && IsExpanded)
            {
                // Already expanded, so we need to update the height of the canvas directly.
                _itemsCanvas.Height = _presenter.DesiredSize.Height;
            }
        }

        /// <summary>
        /// Updates the visual state.
        /// </summary>
        /// <param name="useTransitions">
        /// Indicates whether visual transitions should be used.
        /// </param>
        internal virtual void UpdateVisualState(bool useTransitions)
        {
            string isExpandedState, isNonExpandableState;

            if (_presenter != null)
            {
                if (_expandedStateAnimation != null)
                {
                    _expandedStateAnimation.To = _presenter.DesiredSize.Height;
                }

                if (_collapsedToExpandedFrame != null)
                {
                    _collapsedToExpandedFrame.Value = _presenter.DesiredSize.Height;
                }

                if (_expandedToCollapsedFrame != null)
                {
                    _expandedToCollapsedFrame.Value = _presenter.DesiredSize.Height;
                }
            }

            // Handle the Expansion states
            if (IsExpanded)
            {
                isExpandedState = ExpandedState;
                if (useTransitions)
                {
                    AnimateContainerDropDown();
                }
            }
            else
            {
                isExpandedState = CollapsedState;
            }

            VisualStateManager.GoToState(this, isExpandedState, useTransitions);

            // Handle the Expandability states.
            if (IsNonExpandable)
            {
                isNonExpandableState = NonExpandableState;   
            }
            else
            {
                isNonExpandableState = ExpandableState;  
            }

            VisualStateManager.GoToState(this, isNonExpandableState, useTransitions);
        }

        /// <summary>
        /// Raises a routed event.
        /// </summary>
        /// <param name="handler">Event handler.</param>
        /// <param name="args">Event arguments.</param>
        private void RaiseEvent(RoutedEventHandler handler, RoutedEventArgs args)
        {
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Provides the feathered animation for items
        /// when the Expander View goes from collapsed to expanded.
        /// </summary>
        internal void AnimateContainerDropDown()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                FrameworkElement container = ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;

                if (container == null)
                {
                    break;
                }

                Storyboard itemDropDown = new Storyboard();
                IEasingFunction quadraticEase = new QuadraticEase { EasingMode = EasingMode.EaseOut };
                int initialKeyTime = InitialKeyTime + (KeyTimeStep * i);
                int finalKeyTime = FinalKeyTime + (KeyTimeStep * i);

                TranslateTransform translation = new TranslateTransform();
                container.RenderTransform = translation;

                DoubleAnimationUsingKeyFrames transAnimation = new DoubleAnimationUsingKeyFrames();

                EasingDoubleKeyFrame transKeyFrame_1 = new EasingDoubleKeyFrame();
                transKeyFrame_1.EasingFunction = quadraticEase;
                transKeyFrame_1.KeyTime = TimeSpan.FromMilliseconds(0.0);
                transKeyFrame_1.Value = -150.0;

                EasingDoubleKeyFrame transKeyFrame_2 = new EasingDoubleKeyFrame();
                transKeyFrame_2.EasingFunction = quadraticEase;
                transKeyFrame_2.KeyTime = TimeSpan.FromMilliseconds(initialKeyTime);
                transKeyFrame_2.Value = 0.0;

                EasingDoubleKeyFrame transKeyFrame_3 = new EasingDoubleKeyFrame();
                transKeyFrame_3.EasingFunction = quadraticEase;
                transKeyFrame_3.KeyTime = TimeSpan.FromMilliseconds(finalKeyTime);
                transKeyFrame_3.Value = 0.0;

                transAnimation.KeyFrames.Add(transKeyFrame_1);
                transAnimation.KeyFrames.Add(transKeyFrame_2);
                transAnimation.KeyFrames.Add(transKeyFrame_3);

                Storyboard.SetTarget(transAnimation, translation);
                Storyboard.SetTargetProperty(transAnimation, new PropertyPath(TranslateTransform.YProperty));
                itemDropDown.Children.Add(transAnimation);

                DoubleAnimationUsingKeyFrames opacityAnimation = new DoubleAnimationUsingKeyFrames();

                EasingDoubleKeyFrame opacityKeyFrame_1 = new EasingDoubleKeyFrame();
                opacityKeyFrame_1.EasingFunction = quadraticEase;
                opacityKeyFrame_1.KeyTime = TimeSpan.FromMilliseconds(0.0);
                opacityKeyFrame_1.Value = 0.0;

                EasingDoubleKeyFrame opacityKeyFrame_2 = new EasingDoubleKeyFrame();
                opacityKeyFrame_2.EasingFunction = quadraticEase;
                opacityKeyFrame_2.KeyTime = TimeSpan.FromMilliseconds(initialKeyTime - 150);
                opacityKeyFrame_2.Value = 0.0;

                EasingDoubleKeyFrame opacityKeyFrame_3 = new EasingDoubleKeyFrame();
                opacityKeyFrame_3.EasingFunction = quadraticEase;
                opacityKeyFrame_3.KeyTime = TimeSpan.FromMilliseconds(finalKeyTime);
                opacityKeyFrame_3.Value = 1.0;

                opacityAnimation.KeyFrames.Add(opacityKeyFrame_1);
                opacityAnimation.KeyFrames.Add(opacityKeyFrame_2);
                opacityAnimation.KeyFrames.Add(opacityKeyFrame_3);

                Storyboard.SetTarget(opacityAnimation, container);
                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(FrameworkElement.OpacityProperty));
                itemDropDown.Children.Add(opacityAnimation);

                itemDropDown.Begin();
            }
        }

        #region ItemsControl overriden methods

        /// <summary>
        /// Updates the HasItems property.
        /// </summary>
        /// <param name="e">The event information.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            HasItems = Items.Count > 0;
        }

        #endregion

        #region Input events

        /// <summary>
        /// Toggles the IsExpanded property.
        /// </summary>
        /// <param name="sender">The Expander Panel that triggers the event.</param>
        /// <param name="e">The event information.</param>
        private void OnExpanderPanelTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!IsNonExpandable)
            {
                IsExpanded = !IsExpanded;
            }
        }

        #endregion

        #region Event overrides

        /// <summary>
        /// Called when the value of th Expander property changes.
        /// </summary>
        /// <param name="oldExpander">
        /// The old value of the Expander property.
        /// </param>
        /// <param name="newExpander">
        /// The new value of the Expander property.
        /// </param>
        protected virtual void OnExpanderChanged(object oldExpander, object newExpander)
        {
        }

        /// <summary>
        /// Called when the value of the ExpanderTemplate property changes.
        /// </summary>
        /// <param name="oldTemplate">
        /// The old value of the ExpanderTemplate property.
        /// </param>
        /// <param name="newTemplate">
        /// The new value of the ExpanderTemplate property.
        /// </param>
        protected virtual void OnExpanderTemplateChanged(DataTemplate oldTemplate, DataTemplate newTemplate)
        {
        }

        /// <summary>
        /// Called when the value of the NonExpandableHeader property changes.
        /// </summary>
        /// <param name="oldHeader">
        /// The old value of the NonExpandableHeader property.
        /// </param>
        /// <param name="newHeader">
        /// The new value of the NonExpandableHeader property.
        /// </param>
        protected virtual void OnNonExpandableHeaderChanged(object oldHeader, object newHeader)
        {
        }

        /// <summary>
        /// Called when the value of the NonExpandableHeaderTemplate 
        /// property changes.
        /// </summary>
        /// <param name="oldTemplate">
        /// The old value of the NonExpandableHeaderTemplate property.
        /// </param>
        /// <param name="newTemplate">
        /// The new value of the NonExpandableHeaderTemplate property.
        /// </param>
        protected virtual void OnNonExpandableHeaderTemplateChanged(DataTemplate oldTemplate, DataTemplate newTemplate)
        {
        }

        /// <summary>
        /// Raises an Expanded event when the IsExpanded property
        /// changes from false to true.
        /// </summary>
        /// <param name="e">The event information.</param>
        protected virtual void OnExpanded(RoutedEventArgs e)
        {
            RaiseEvent(Expanded, e);
        }

        /// <summary>
        /// Raises a Collapsed event when the IsExpanded property
        /// changes from true to false.
        /// </summary>
        /// <param name="e">The event information.</param>
        protected virtual void OnCollapsed(RoutedEventArgs e)
        {
            RaiseEvent(Collapsed, e);
        }

        #endregion
    }

    /// <summary>
    /// Represents the visual states of an Expander View
    /// related to its current expansion.
    /// </summary>
    internal enum ExpansionStates
    {
        /// <summary>
        /// Collapsed visual state value.
        /// </summary>
        Collapsed = 0,

        /// <summary>
        /// Expanded visual state value.
        /// </summary>
        Expanded = 1,
    }

    /// <summary>
    /// Represents the visual states of an Expander View
    /// related to its expandability.
    /// </summary>
    internal enum ExpandabilityStates
    {
        /// <summary>
        /// Expandable visual state value.
        /// </summary>
        Expandable = 0,

        /// <summary>
        /// NonExpandable visual state value.
        /// </summary>
        NonExpandable = 1,
    }
}
