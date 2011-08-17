// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// An item container for a Multiselect List.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    [TemplateVisualState(Name = Closed, GroupName = SelectionEnabledStates)]
    [TemplateVisualState(Name = Exposed, GroupName = SelectionEnabledStates)]
    [TemplateVisualState(Name = Opened, GroupName = SelectionEnabledStates)]
    [TemplatePart(Name = OutterHintPanel, Type = typeof(Rectangle))]
    [TemplatePart(Name = InnerHintPanel, Type = typeof(Rectangle))]
    [TemplatePart(Name = OutterCover, Type = typeof(Grid))]
    [TemplatePart(Name = InfoPresenter, Type = typeof(ContentControl))]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public class MultiselectItem : ContentControl
    {
        /// <summary>
        /// Selection mode visual states.
        /// </summary>
        private const string SelectionEnabledStates = "SelectionEnabledStates";

        /// <summary>
        /// Closed visual state.
        /// </summary>
        private const string Closed = "Closed";

        /// <summary>
        /// Exposed visual state.
        /// </summary>
        private const string Exposed = "Exposed";

        /// <summary>
        /// Opened visual state.
        /// </summary>
        private const string Opened = "Opened";

        /// <summary>
        /// Select Box template part name.
        /// </summary>
        private const string SelectBox = "SelectBox";

        /// <summary>
        /// Outter Hint Panel template part name.
        /// </summary>
        private const string OutterHintPanel = "OutterHintPanel";

        /// <summary>
        /// Inner Hint Panel template part name.
        /// </summary>
        private const string InnerHintPanel = "InnerHintPanel";

        /// <summary>
        /// Outter Cover template part name.
        /// </summary>
        private const string OutterCover = "OutterCover";

        /// <summary>
        /// Item Info Presenter template part name.
        /// </summary>
        private const string InfoPresenter = "InfoPresenter";

        /// <summary>
        /// Limit for the manipulation delta in the X-axis.
        /// </summary>
        private const double _deltaLimitX = 0.0;

        /// <summary>
        /// Limit for the manipulation delta in the Y-axis.
        /// </summary>
        private const double _deltaLimitY = 0.4;

        /// <summary>
        /// Outter Hint Panel template part.
        /// </summary>
        private Rectangle _outterHintPanel;

        /// <summary>
        /// Inner Hint Panel template part.
        /// </summary>
        private Rectangle _innerHintPanel;

        /// <summary>
        /// Outter Cover template part.
        /// </summary>
        private Grid _outterCover;

        /// <summary>
        /// Item Info Presenter template part.
        /// </summary>
        private ContentControl _infoPresenter;

        /// <summary>
        /// Multiselect List that owns this Multiselect Item.
        /// </summary>
        private MultiselectList _parent;

        /// <summary>
        /// Manipulation delta in the x-axis.
        /// </summary>
        private double _manipulationDeltaX;

        /// <summary>
        /// Manipulation delta in the y-axis.
        /// </summary>
        private double _manipulationDeltaY;             

        /// <summary>
        /// Indicates that this Multiselect Item is a container 
        /// being reused for virtualization.
        /// </summary>
        internal bool _isBeingVirtualized;

        /// <summary>
        /// Flag that is used to prevent multiple selection changed
        /// events from being fired when all the items in the list are
        /// unselected. Instead, a single event is fired.
        /// </summary>
        internal bool _canTriggerSelectionChanged = true;

        /// <summary>
        /// Occurs when the multiselect item is selected.
        /// </summary>
        public event RoutedEventHandler Selected;

        /// <summary>
        /// Occurs when the multiselect item is unselected.
        /// </summary>
        public event RoutedEventHandler Unselected;

        #region IsSelected DependencyProperty

        /// <summary>
        /// Gets or sets the flag that indicates if the item 
        /// is currently selected.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Identifies the IsSelected dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MultiselectItem), new PropertyMetadata(false, OnIsSelectedPropertyChanged));

        /// <summary>
        /// Adds or removes the item to the selected items collection
        /// in the Multiselect List that owns it.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The event information.</param>
        private static void OnIsSelectedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            MultiselectItem item = (MultiselectItem)obj;
            RoutedEventArgs args = new RoutedEventArgs();
            bool isSelected = (bool)e.NewValue;

            if (isSelected)
            {
                item.OnSelected(args);
            }
            else
            {
                item.OnUnselected(args);
            }

            //Ignore items being selected/unselected during parent virtualization.
            if (item._parent != null && !item._isBeingVirtualized)
            {
                if (isSelected)
                {
                    item._parent.SelectedItems.Add(item.Content);

                    //Trigger a selection changed event for one added item.
                    if (item._canTriggerSelectionChanged)
                    {
                        item._parent.OnSelectionChanged(new object[0], new object[] { item.Content });
                    }
                }
                else
                {
                    item._parent.SelectedItems.Remove(item.Content);

                    //Trigger a selection changed event for one removed item.
                    if (item._canTriggerSelectionChanged)
                    {
                        item._parent.OnSelectionChanged(new object[] { item.Content }, new object[0]);
                    }
                }
            }
        }

        #endregion

        #region State DependencyProperty

        /// <summary>
        /// Gets or sets the visual state.
        /// </summary>
        internal SelectionEnabledState State
        {
            get { return (SelectionEnabledState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        /// <summary>
        /// Identifies the State dependency property.
        /// </summary>
        internal static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(SelectionEnabledState), typeof(MultiselectItem), new PropertyMetadata(SelectionEnabledState.Closed, null));

        #endregion

        #region HintPanelHeight DependencyProperty

        /// <summary>
        /// Gets or sets the height of the hint panel.
        /// </summary>
        public double HintPanelHeight
        {
            get { return (double)GetValue(HintPanelHeightProperty); }
            set { SetValue(HintPanelHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the HintPanelHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty HintPanelHeightProperty =
            DependencyProperty.Register("HintPanelHeight", typeof(double), typeof(MultiselectItem), new PropertyMetadata(Double.NaN, null));

        /// <summary>
        /// Sets the vertical alignment of the hint panels to stretch if the
        /// height is not manually set. If it is, the alignment is set to top.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The event information.</param>
        private static void OnHintPanelHeightPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            MultiselectItem source = (MultiselectItem)obj;

            if (source._outterHintPanel != null)
            {
                if (double.IsNaN((double)e.NewValue))
                {
                    source._outterHintPanel.VerticalAlignment = VerticalAlignment.Stretch;
                }
                else
                {
                    source._outterHintPanel.VerticalAlignment = VerticalAlignment.Top;
                }
            }

            if (source._innerHintPanel != null)
            {
                if (double.IsNaN(source.HintPanelHeight))
                {
                    source._innerHintPanel.VerticalAlignment = VerticalAlignment.Stretch;
                }
                else
                {
                    source._innerHintPanel.VerticalAlignment = VerticalAlignment.Top;
                }
            }
        }

        #endregion

        #region ContentInfo DependencyProperty

        /// <summary>
        /// Gets or sets the content information.
        /// </summary>
        public object ContentInfo
        {
            get { return (object)GetValue(ContentInfoProperty); }
            set { SetValue(ContentInfoProperty, value); }
        }

        /// <summary>
        /// Identifies the ContentInfo dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentInfoProperty =
            DependencyProperty.Register("ContentInfo", typeof(object), typeof(MultiselectItem), new PropertyMetadata(null, OnContentInfoPropertyChanged));

        /// <summary>
        /// ContentInfoProperty changed handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The event information.</param>
        private static void OnContentInfoPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            MultiselectItem source = (MultiselectItem)obj;
            source.OnContentInfoChanged(e.OldValue, e.NewValue);
        }

        #endregion

        #region ContentInfoTemplate DependencyProperty

        /// <summary>
        /// Gets or sets the data template that defines
        /// the content information.
        /// </summary>
        public DataTemplate ContentInfoTemplate
        {
            get { return (DataTemplate)GetValue(ContentInfoTemplateProperty); }
            set { SetValue(ContentInfoTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the ContentInfoTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentInfoTemplateProperty =
            DependencyProperty.Register("ContentInfoTemplate", typeof(DataTemplate), typeof(MultiselectItem), new PropertyMetadata(null, OnContentInfoTemplatePropertyChanged));

        /// <summary>
        /// ContentInfoTemplate changed handler.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="e">The event information.</param>
        private static void OnContentInfoTemplatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            MultiselectItem source = (MultiselectItem)obj;
            DataTemplate oldTemplate = e.OldValue as DataTemplate;
            DataTemplate newTemplate = e.NewValue as DataTemplate;
            source.OnContentInfoTemplateChanged(oldTemplate, newTemplate);
        }

        #endregion

        /// <summary>
        /// Gets the template parts and sets event handlers.
        /// </summary>
        public override void OnApplyTemplate()
        {
            _parent = ItemsControlExtensions.GetParentItemsControl<MultiselectList>(this);

            if (_innerHintPanel != null)
            {
                _innerHintPanel.ManipulationStarted -= HintPanel_ManipulationStarted;
                _innerHintPanel.ManipulationDelta -= HintPanel_ManipulationDelta;
                _innerHintPanel.ManipulationCompleted -= HintPanel_ManipulationCompleted;
            }

            if (_outterHintPanel != null)
            {
                _outterHintPanel.ManipulationStarted -= HintPanel_ManipulationStarted;
                _outterHintPanel.ManipulationDelta -= HintPanel_ManipulationDelta;
                _outterHintPanel.ManipulationCompleted -= HintPanel_ManipulationCompleted;
            }

            if (_outterCover != null)
            {
                _outterCover.Tap -= Cover_Tap;
            }

            _innerHintPanel = base.GetTemplateChild(InnerHintPanel) as Rectangle;
            _outterHintPanel = base.GetTemplateChild(OutterHintPanel) as Rectangle;
            _outterCover = base.GetTemplateChild(OutterCover) as Grid;
            _infoPresenter = base.GetTemplateChild(InfoPresenter) as ContentControl;
           
            base.OnApplyTemplate();

            if (_innerHintPanel != null)
            {
                _innerHintPanel.ManipulationStarted += HintPanel_ManipulationStarted;
                _innerHintPanel.ManipulationDelta += HintPanel_ManipulationDelta;
                _innerHintPanel.ManipulationCompleted += HintPanel_ManipulationCompleted;
            }

            if (_outterHintPanel != null)
            {
                _outterHintPanel.ManipulationStarted += HintPanel_ManipulationStarted;
                _outterHintPanel.ManipulationDelta += HintPanel_ManipulationDelta;
                _outterHintPanel.ManipulationCompleted += HintPanel_ManipulationCompleted;
            }


            if (_outterCover != null)
            {
                _outterCover.Tap += Cover_Tap;
            }

            if (ContentInfo == null && _parent != null)
            {
                if (_parent.ItemInfoTemplate != null)
                {
                    _infoPresenter.ContentTemplate = _parent.ItemInfoTemplate;
                    Binding infoBinding = new Binding();
                    this.SetBinding(ContentInfoProperty, infoBinding);
                }
            }

            if (_outterHintPanel != null)
            {
                if (double.IsNaN(HintPanelHeight))
                {
                    _outterHintPanel.VerticalAlignment = VerticalAlignment.Stretch;
                }
                else
                {
                    _outterHintPanel.VerticalAlignment = VerticalAlignment.Top;                   
                }
            }

            if (_innerHintPanel != null)
            {
                if (double.IsNaN(HintPanelHeight))
                {
                    _innerHintPanel.VerticalAlignment = VerticalAlignment.Stretch;
                }
                else
                {
                    _innerHintPanel.VerticalAlignment = VerticalAlignment.Top;
                }
            }

            UpdateVisualState(false);
        }

        /// <summary>
        /// Initializes a new instance of the MultiselectItem class.
        /// </summary>
        public MultiselectItem()
        {
            DefaultStyleKey = typeof(MultiselectItem);
        }

        /// <summary>
        /// Updates the visual state.
        /// </summary>
        /// <param name="useTransitions">
        /// Indicates whether visual transitions should be used.
        /// </param>
        internal void UpdateVisualState(bool useTransitions)
        {
            string state;

            switch (this.State)
            {
                case SelectionEnabledState.Closed:
                    state = Closed;
                    break;
                case SelectionEnabledState.Exposed:
                    state = Exposed;
                    break;
                case SelectionEnabledState.Opened:
                    state = Opened;
                    break;
                default:
                    state = Closed;
                    break;
            }

            VisualStateManager.GoToState(this, state, useTransitions);
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

        #region Event overrides

        /// <summary>
        /// Raises a Selected event when the IsSelected property
        /// changes from false to true.
        /// </summary>
        /// <param name="e">The event information.</param>
        protected virtual void OnSelected(RoutedEventArgs e)
        {
            if (_parent == null)
            {
                State = SelectionEnabledState.Opened;
                UpdateVisualState(true);
            }
            RaiseEvent(Selected, e);
        }

        /// <summary>
        /// Raises an Unselected event when the IsSelected property
        /// changes from true to false.
        /// </summary>
        /// <param name="e">The event information.</param>
        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            if (_parent == null)
            {
                State = SelectionEnabledState.Closed;
                UpdateVisualState(true);
            }
            RaiseEvent(Unselected, e);
        }

        /// <summary>
        /// Called when the value of the ContentInfo property changes.
        /// </summary>
        /// <param name="oldContentInfo">
        /// The old value of the ContentInfo property.
        /// </param>
        /// <param name="newContentInfo">
        /// The new value of the ContentInfo property.
        /// </param>
        protected virtual void OnContentInfoChanged(object oldContentInfo, object newContentInfo)
        {
        }

        /// <summary>
        /// Called when the value of the ContentInfoTemplate property chages.
        /// </summary>
        /// <param name="oldContentInfoTemplate">
        /// The old value of the ContentInfoTemplate property.
        /// </param>
        /// <param name="newContentInfoTemplate">
        /// The new value of the ContentInfoTemplate property.
        /// </param>
        protected virtual void OnContentInfoTemplateChanged(DataTemplate oldContentInfoTemplate, DataTemplate newContentInfoTemplate)
        {
        }

        #endregion

        #region Input events

        /// <summary>
        /// Triggers a visual transition to the Exposed visual state.
        /// </summary>
        /// <param name="sender">The Hint Panel that triggers the event.</param>
        /// <param name="e">The event information.</param>
        private void HintPanel_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            this.State = SelectionEnabledState.Exposed;
            UpdateVisualState(true);
        }

        /// <summary>
        /// Triggers a visual transition to the Closed visual state 
        /// if the manipulation delta goes out of bounds.
        /// </summary>
        /// <param name="sender">The Hint Panel that triggers the event.</param>
        /// <param name="e">The event information.</param>
        private void HintPanel_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            _manipulationDeltaX = e.DeltaManipulation.Translation.X;
            _manipulationDeltaY = e.DeltaManipulation.Translation.Y;

            if (_manipulationDeltaX < _deltaLimitX)
            {
                _manipulationDeltaX *= -1.0;
            }

            if (_manipulationDeltaY < _deltaLimitX)
            {
                _manipulationDeltaY *= -1.0;
            }

            if (_manipulationDeltaX > _deltaLimitX || _manipulationDeltaY >= _deltaLimitY)
            {
                this.State = SelectionEnabledState.Closed;
                UpdateVisualState(true);
            }
        }

        /// <summary>
        /// Selects this MultiselectItem if the manipulation delta 
        /// is within limits and fires an OnSelectionChanged event accordingly.
        /// Resets the deltas for both axises.
        /// </summary>
        /// <param name="sender">The Hint Panel that triggers the event.</param>
        /// <param name="e">The event information.</param>
        private void HintPanel_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (_manipulationDeltaX == _deltaLimitX && _manipulationDeltaY < _deltaLimitY)
            {
                this.IsSelected = true;
            }

            _manipulationDeltaX = 0.0;
            _manipulationDeltaY = 0.0;
        }

        /// <summary>
        /// Toogles the selection of a MultiselectItem
        /// and fires an OnSelectionChanged event accordingly.
        /// </summary>
        /// <param name="sender">The cover that triggers the event.</param>
        /// <param name="e">The event information.</param>
        private void Cover_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.IsSelected = !this.IsSelected;
        }    

        #endregion    
    }

    /// <summary>
    /// Represents the visual states of a Multiselect item.
    /// </summary>
    internal enum SelectionEnabledState
    {
        /// <summary>
        /// Closed visual state value.
        /// </summary>
        Closed = 0,

        /// <summary>
        /// Exposed visual state value.
        /// </summary>
        Exposed = 1,

        /// <summary>
        /// Opened visual state value.
        /// </summary>
        Opened = 2
    }
}
