// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Internals;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
#if SILVERLIGHT
using SW = Microsoft.Windows;
#else
using SW = System.Windows;
#endif

namespace System.Windows.Controls
{
    /// <summary>
    /// A control that enabled drag and drop operations on an ItemsControl.
    /// </summary>
    /// <typeparam name="TItemsControlType">The type of the items control.</typeparam>
    /// <typeparam name="TItemContainerType">The type of the item container.</typeparam>
    /// <QualityBand>Experimental</QualityBand>
#if SILVERLIGHT
    public abstract class DragDropTarget<TItemsControlType, TItemContainerType> : ContentControl, SW.IAcceptDrop
#else
    public abstract class DragDropTarget<TItemsControlType, TItemContainerType> : ContentControl
#endif
        where TItemsControlType : UIElement
        where TItemContainerType : UIElement
    {
        /// <summary>
        /// The name of the DragPopup template part.
        /// </summary>
        protected const string DragPopupName = "DragPopup";

        /// <summary>
        /// The name of the DragContainer template part.
        /// </summary>
        protected const string DragContainerName = "DragContainer";

        /// <summary>
        /// The name of the DragDecorator template part.
        /// </summary>
        protected const string DragDecoratorName = "DragDecorator";

        /// <summary>
        /// The name of the insertion indicator template part.
        /// </summary>
        protected const string InsertionIndicatorName = "InsertionIndicator";

        /// <summary>
        /// The name of the insertion indicator container.
        /// </summary>
        protected const string InsertionIndicatorContainerName = "InsertionIndicatorContainer";

        /// <summary>
        /// The size of the mouse cursor.
        /// </summary>
        private readonly Size mouseCursorSize = new Size(15.0, 15.0);

        /// <summary>
        /// Information about an ongoing item drag event.
        /// </summary>
        private static ItemDragEventArgs _currentItemDragEventArgs;

        /// <summary>
        /// The state of the keys relevant to drag and drop operation.
        /// </summary>
        private SW.DragDropKeyStates _keyStates;

        /// <summary>
        /// The effects specified in the last give feed back event.
        /// </summary>
        private SW.DragDropEffects _lastGiveFeedbackEffects;

        /// <summary>
        /// An event raised when the ItemsControl is changed.
        /// </summary>
        private Subject<TItemsControlType> _itemsControlChanged = new Subject<TItemsControlType>();

        /// <summary>
        /// An event raised when an item drag is started.
        /// </summary>
        private Subject<IEvent<ItemDragEventArgs>> _itemDragStarted = new Subject<IEvent<ItemDragEventArgs>>();

        /// <summary>
        /// An event raised when an item drag is completed.
        /// </summary>
        private Subject<IEvent<ItemDragEventArgs>> _itemDragCompleted = new Subject<IEvent<ItemDragEventArgs>>();

        /// <summary>
        /// Gets information about an ongoing item drag event.
        /// </summary>
        private static ItemDragEventArgs CurrentItemDragEventArgs
        {
            get { return (_currentItemDragEventArgs != null) ? new ItemDragEventArgs(_currentItemDragEventArgs) : null; }
        }

        /// <summary>
        /// A value indicating whether an item was dropped on the drag source.
        /// </summary>
        private bool _itemWasDroppedOnSource;

        /// <summary>
        /// Gets or sets the popup used to move the drag decorator with the 
        /// mouse.
        /// </summary>
        private Popup _dragPopup;

        /// <summary>
        /// Gets or sets the canvas used to move the drag decorator with the
        /// mouse.
        /// </summary>
        private Canvas _dragContainer;

        /// <summary>
        /// Gets or sets the drag decorator that moves with the mouse during a 
        /// drag operation.
        /// </summary>
        private DragDecorator _dragDecorator;

        /// <summary>
        /// Gets or sets the insertion indicator path used to indicate where an item
        /// will be inserted.
        /// </summary>
        private Path _insertionIndicator;

        /// <summary>
        /// Gets or sets the insertion indicator container.
        /// </summary>
        private Canvas _insertionIndicatorContainer;

        /// <summary>
        /// A drop target insertion index that is adjusted when items are being
        /// moved within the source to the location of one of the selected items.  
        /// It is necessary to adjust the insertion index in this case because the selected items 
        /// are removed from the items control before being added again, and when added the
        /// insertion index will be null because it wont be able to be retrieved using the
        /// visual tree.
        /// </summary>
        private int? _adjustedDropTargetInsertionIndex = null;

        #region public SW.DragDropEffects AllowedSourceEffects
        /// <summary>
        /// Gets or sets the allowed effects when this DragDropTarget is the drag source.
        /// </summary>
        public SW.DragDropEffects AllowedSourceEffects
        {
            get { return (SW.DragDropEffects)GetValue(AllowedSourceEffectsProperty); }
            set { SetValue(AllowedSourceEffectsProperty, value); }
        }

        /// <summary>
        /// Identifies the AllowedSourceEffects dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Dependency Property should be inherited by derived classes.")]
        public static readonly System.Windows.DependencyProperty AllowedSourceEffectsProperty =
            DependencyProperty.Register(
                "AllowedSourceEffects",
                typeof(SW.DragDropEffects),
                typeof(DragDropTarget<TItemsControlType, TItemContainerType>),
                new PropertyMetadata(SW.DragDropEffects.Link | SW.DragDropEffects.Move | SW.DragDropEffects.Scroll));
        #endregion public SW.DragDropEffects AllowedSourceEffects

        #region public event ItemDragStarting
        /// <summary>
        /// A list of ItemDragStarting event handlers.
        /// </summary>
        private List<EventHandler<ItemDragEventArgs>> _itemDragStarting = new List<EventHandler<ItemDragEventArgs>>();

        /// <summary>
        /// An event raised when a drag operation is starting on an item.
        /// </summary>
        public event EventHandler<ItemDragEventArgs> ItemDragStarting
        {
            add
            {
                _itemDragStarting.Add(value);
            }
            remove
            {
                _itemDragStarting.Remove(value);
            }
        }

        /// <summary>
        /// A method that raises the item drag starting event.
        /// </summary>
        /// <param name="eventArgs">Information about the drag starting event.
        /// </param>
        protected virtual void OnItemDragStarting(ItemDragEventArgs eventArgs)
        {
            UIElement dragSource = eventArgs.DragSource as UIElement;

            ItemDragEventArgs copy = new ItemDragEventArgs(eventArgs);
            foreach (EventHandler<ItemDragEventArgs> handler in this._itemDragStarting)
            {
                handler(this, copy);
                if (copy.Handled)
                {
                    eventArgs = copy;
                    break;
                }
            }

            if (!eventArgs.Cancel && !SW.DragDrop.IsDragInProgress)
            {
                _itemDragStarted.OnNext(Event.Create(this, eventArgs));
                SW.GiveFeedbackEventHandler giveFeedbackHandler =
                    (_, a) =>
                    {
                        _lastGiveFeedbackEffects = a.Effects;
                    };

                EventHandler<DragDropCompletedEventArgs> handler = null;
                handler =
                    (_, args) =>
                    {
                        if (dragSource != null)
                        {
                            dragSource.RemoveHandler(
                                SW.DragDrop.GiveFeedbackEvent,
                                giveFeedbackHandler);
                        }
                        SW.DragDrop.DragDropCompleted -= handler;

                        _itemDragCompleted.OnNext(
                            Event.Create(
                                this,
                                new ItemDragEventArgs(eventArgs)
                                {
                                    Effects = (args.Effects == SW.DragDropEffects.Scroll || args.Effects == SW.DragDropEffects.None) ? args.Effects : _lastGiveFeedbackEffects
                                }));
                    };

                if (dragSource != null)
                {
                    dragSource.AddHandler(
                        SW.DragDrop.GiveFeedbackEvent,
                        giveFeedbackHandler,
                        true);
                }

                SW.DragDrop.DragDropCompleted += handler;

                SW.DragDrop.DoDragDrop(
                    eventArgs.DragSource,
                    eventArgs,
                    eventArgs.AllowedEffects,
                    SW.DragDropKeyStates.LeftMouseButton);
            }
        }
        #endregion public event ItemDragStarting

        #region public event ItemDroppedOnTarget
        /// <summary>
        /// A list of ItemDragCompleted event handlers.
        /// </summary>
        private List<EventHandler<ItemDragEventArgs>> _itemDroppedOnTargetHandlers = new List<EventHandler<ItemDragEventArgs>>();

        /// <summary>
        /// This event is raised when an item is dropped on a target.
        /// </summary>
        public event EventHandler<ItemDragEventArgs> ItemDroppedOnTarget
        {
            add
            {
                _itemDroppedOnTargetHandlers.Add(value);
            }
            remove
            {
                _itemDroppedOnTargetHandlers.Remove(value);
            }
        }

        /// <summary>
        /// Raises the ItemDragCompleted event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        protected virtual void OnItemDroppedOnTarget(ItemDragEventArgs args)
        {
            foreach (EventHandler<ItemDragEventArgs> handler in _itemDroppedOnTargetHandlers)
            {
                handler(this, args);
                if (args.Handled)
                {
                    break;
                }
            }

            if (!args.Handled)
            {
                if ((args.Effects & SW.DragDropEffects.Move) == SW.DragDropEffects.Move && !args.DataRemovedFromDragSource)
                {
                    TItemsControlType itemsControl = args.DragSource as TItemsControlType;
                    RemoveDataFromItemsControl(itemsControl, args.Data);
                    args.Handled = true;
                }
            }
        }
        #endregion public event ItemDroppedOnTarget

        #region public event ItemDroppedOnSource
        /// <summary>
        /// A list of event handles for the ItemDroppedOnSource event.
        /// </summary>
        private IList<SW.DragEventHandler> _itemDroppedOnSource = new List<SW.DragEventHandler>();

        /// <summary>
        /// An event raised when the an item is dropped onto the adorner.
        /// </summary>
        public event SW.DragEventHandler ItemDroppedOnSource
        {
            add
            {
                _itemDroppedOnSource.Add(value);
            }
            remove
            {
                _itemDroppedOnSource.Remove(value);
            }
        }

        /// <summary>
        /// Raises the ItemDroppedOnSource event.
        /// </summary>
        /// <param name="args">Information about the ItemDroppedOnSource event.
        /// </param>
        public virtual void OnItemDroppedOnSource(SW.DragEventArgs args)
        {
            foreach (SW.DragEventHandler handler in _itemDroppedOnSource)
            {
                handler(this, args);
                if (args.Handled)
                {
                    return;
                }
            }

            ItemDragEventArgs itemDragEventArgs = (ItemDragEventArgs)args.Data.GetData();
            SelectionCollection selectionCollection = SelectionCollection.ToSelectionCollection(itemDragEventArgs.Data);

            // Don't bother to do anything unless data is a selection collection
            // and all items have indexes.  Otherwise the result would be 
            // non-deterministic.
            if (selectionCollection.All(selection => selection.Index.HasValue))
            {
                // Grabbing the drop target insertion index.
                TItemsControlType dropTarget = GetDropTarget(args);
                if (dropTarget != null)
                {
                    try
                    {
                        int insertionIndex = GetDropTargetInsertionIndexOverride(dropTarget, args);

                        if ((_lastGiveFeedbackEffects & SW.DragDropEffects.Move) == SW.DragDropEffects.Move)
                        {
                            // If the insertion index is equal to one of the selected indexes, we must
                            // adjust and override the insertion index because when OnDropOverride is called the items
                            // will be out of the items control and the next call to GetDropTargetInsertionIndex
                            // will return -1.
                            if (selectionCollection.Any(selection => selection.Index == insertionIndex || selection.Index + 1 == insertionIndex))
                            {
                                // Adjusting the drop target insertion index
                                insertionIndex -= selectionCollection.Where(selection => selection.Index.Value < insertionIndex).Count();
                                _adjustedDropTargetInsertionIndex = insertionIndex;
                            }

                            // Removing data from drag source to prepare for adding it again.
                            if (!itemDragEventArgs.DataRemovedFromDragSource)
                            {
                                itemDragEventArgs.RemoveDataFromDragSource();
                            }
                        }
                        OnDropOverride(args);
                    }
                    finally
                    {
                        _adjustedDropTargetInsertionIndex = null;
                    }
                }
            }
        }
        #endregion public event ItemDroppedOnSource

        #region public event IndicatingInsertionLocation
        /// <summary>
        /// A list of IndicatingInsertionLocation handlers.
        /// </summary>
        private List<EventHandler<IndicatingInsertionLocationEventArgs<TItemsControlType>>> _IndicatingInsertionLocation = new List<EventHandler<IndicatingInsertionLocationEventArgs<TItemsControlType>>>();

        /// <summary>
        /// A method which raises the IndicatingInsertionLocation event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        protected virtual void OnIndicatingInsertionLocation(IndicatingInsertionLocationEventArgs<TItemsControlType> args)
        {
            foreach (EventHandler<IndicatingInsertionLocationEventArgs<TItemsControlType>> handler in _IndicatingInsertionLocation)
            {
                handler(this, args);
                if (args.Handled)
                {
                    break;
                }
            }

            if (_insertionIndicatorContainer != null && _insertionIndicator != null)
            {
                UIElement element = this.Content as UIElement;
                if (element != null)
                {
                    GeneralTransform generalTransform = element.SafeTransformToVisual(this);
                    Point origin;
                    if (generalTransform != null && generalTransform.TryTransform(new Point(0, 0), out origin))
                    {
                        _insertionIndicatorContainer.Clip = new RectangleGeometry { Rect = new Rect(origin, element.GetSize()) };
                    }
                }

                if (args.Handled)
                {
                    _insertionIndicator.Data = args.InsertionIndicatorGeometry;
                }
                else
                {
                    if (args.InsertionIndex.HasValue)
                    {
                        _insertionIndicator.Data = GetInsertionIndicatorGeometry(args.DropTarget, args.InsertionIndex.Value, args.DragEventArgs);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the orientation of the items host in the items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The orientation of the items host in the items control.
        /// </returns>
        internal Orientation? GetOrientation(TItemsControlType itemsControl)
        {
            Panel host = GetItemsHost(itemsControl);

            VirtualizingStackPanel virtualizingStackPanel = host as VirtualizingStackPanel;
            if (virtualizingStackPanel != null)
            {
                return virtualizingStackPanel.Orientation;
            }

            StackPanel stackPanel = host as StackPanel;
            if (stackPanel != null)
            {
                return stackPanel.Orientation;
            }

            WrapPanel wrapPanel = host as WrapPanel;
            if (wrapPanel != null)
            {
                return wrapPanel.Orientation;
            }

            return null;
        }

        #endregion public event IndicatingInsertionLocation

#if SILVERLIGHT
        /// <summary>
        /// Raises the DragEnter event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        void SW.IAcceptDrop.OnDragEnter(SW.DragEventArgs args)
        {
            this.OnDragEnter(args);
        }

        /// <summary>
        /// Raises the DragOver event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        void SW.IAcceptDrop.OnDragOver(SW.DragEventArgs args)
        {
            this.OnDragOver(args);
        }

        /// <summary>
        /// Raises the DragLeave event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        void SW.IAcceptDrop.OnDragLeave(SW.DragEventArgs args)
        {
            this.OnDragLeave(args);
        }

        /// <summary>
        /// Raises the Drop event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        void SW.IAcceptDrop.OnDrop(SW.DragEventArgs args)
        {
            OnDrop(args);
        }

        /// <summary>
        /// Raises the GiveFeedback event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        void SW.IAcceptDrop.OnGiveFeedback(SW.GiveFeedbackEventArgs args)
        {
            this.OnGiveFeedback(args);
        }

        /// <summary>
        /// Raises the QueryContinueDrag event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        void SW.IAcceptDrop.OnQueryContinueDrag(SW.QueryContinueDragEventArgs args)
        {
            this.OnQueryContinueDrag(args);
        }
#endif

        #region public event DragEnter
        /// <summary>
        /// Raises the DragEnter event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        protected virtual void OnDragEnter(SW.DragEventArgs args)
        {
            foreach (SW.DragEventHandler handler in _dragEnter)
            {
                handler(this, args);
                if (args.Handled)
                {
                    return;
                }
            }

            OnDragEvent(args);
        }

        /// <summary>
        /// A list of event handles for the DragEnter event.
        /// </summary>
        private IList<SW.DragEventHandler> _dragEnter = new List<SW.DragEventHandler>();

        /// <summary>
        /// An event raised when the an item is dragged into the adorner.
        /// </summary>
        public new event SW.DragEventHandler DragEnter
        {
            add
            {
                _dragEnter.Add(value);
            }
            remove
            {
                _dragEnter.Remove(value);
            }
        }
        #endregion public event DragEnter

        #region public event DragOver
        /// <summary>
        /// Raises the DragOver event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        protected virtual void OnDragOver(SW.DragEventArgs args)
        {
            foreach (SW.DragEventHandler handler in _dragOver)
            {
                handler(this, args);
                if (args.Handled)
                {
                    return;
                }
            }

            OnDragEvent(args);
        }

        /// <summary>
        /// A list of event handles for the DragOver event.
        /// </summary>
        private IList<SW.DragEventHandler> _dragOver = new List<SW.DragEventHandler>();

        /// <summary>
        /// An event raised when the an item is dragged over the adorner.
        /// </summary>
        public new event SW.DragEventHandler DragOver
        {
            add
            {
                _dragOver.Add(value);
            }
            remove
            {
                _dragOver.Remove(value);
            }
        }
        #endregion

        #region public event DragLeave
        /// <summary>
        /// Raises the DragLeave event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        protected virtual void OnDragLeave(SW.DragEventArgs args)
        {
            foreach (SW.DragEventHandler handler in _dragLeave)
            {
                handler(this, args);
                if (args.Handled)
                {
                    return;
                }
            }

            OnDragEvent(args);
        }

        /// <summary>
        /// A list of event handles for the DragLeave event.
        /// </summary>
        private IList<SW.DragEventHandler> _dragLeave = new List<SW.DragEventHandler>();

        /// <summary>
        /// An event raised when the an item is dragged out of the adorner.
        /// </summary>
        public new event SW.DragEventHandler DragLeave
        {
            add
            {
                _dragLeave.Add(value);
            }
            remove
            {
                _dragLeave.Remove(value);
            }
        }
        #endregion public event DragLeave

        #region public event Drop
        /// <summary>
        /// A list of event handles for the Drop event.
        /// </summary>
        private IList<SW.DragEventHandler> _drop = new List<SW.DragEventHandler>();

        /// <summary>
        /// An event raised when the an item is dropped onto the adorner.
        /// </summary>
        public new event SW.DragEventHandler Drop
        {
            add
            {
                _drop.Add(value);
            }
            remove
            {
                _drop.Remove(value);
            }
        }

        /// <summary>
        /// An method that invokes the Drop event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        protected virtual void OnDrop(SW.DragEventArgs args)
        {
            foreach (SW.DragEventHandler handler in _drop)
            {
                handler(this, args);
                if (args.Handled)
                {
                    return;
                }
            }

            if (IsDragWithinDragSource(args))
            {
                _itemWasDroppedOnSource = true;

                OnItemDroppedOnSource(args);
            }
            else
            {
                OnDropOverride(args);
            }
        }
        #endregion public event Drop

        #region public event GiveFeedback
        /// <summary>
        /// Raises the GiveFeedback event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        protected virtual void OnGiveFeedback(SW.GiveFeedbackEventArgs args)
        {
            foreach (SW.GiveFeedbackEventHandler handler in _giveFeedback)
            {
                handler(this, args);
                if (args.Handled)
                {
                    return;
                }
            }

            if (_dragDecorator != null)
            {
                _dragDecorator.Effects = args.Effects;
            }
        }

        /// <summary>
        /// A list of event handles for the GiveFeedback event.
        /// </summary>
        private IList<SW.GiveFeedbackEventHandler> _giveFeedback = new List<SW.GiveFeedbackEventHandler>();

        /// <summary>
        /// An event raised when the feedback about the drag operations is 
        /// requested from the adorner.
        /// </summary>
        public event SW.GiveFeedbackEventHandler GiveFeedback
        {
            add
            {
                _giveFeedback.Add(value);
            }
            remove
            {
                _giveFeedback.Remove(value);
            }
        }
        #endregion

        #region public event QueryContinueDrag
        /// <summary>
        /// Raises the QueryContinueDrag event.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        protected virtual void OnQueryContinueDrag(SW.QueryContinueDragEventArgs args)
        {
            foreach (SW.QueryContinueDragEventHandler handler in _queryContinueDrag)
            {
                handler(this, args);
                if (args.Handled)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// A list of event handles for the QueryContinueDrag event.
        /// </summary>
        private IList<SW.QueryContinueDragEventHandler> _queryContinueDrag = new List<SW.QueryContinueDragEventHandler>();

        /// <summary>
        /// An event raised when the adorner is queries regarding the drag 
        /// operation.
        /// </summary>
        public event SW.QueryContinueDragEventHandler QueryContinueDrag
        {
            add
            {
                _queryContinueDrag.Add(value);
            }
            remove
            {
                _queryContinueDrag.Remove(value);
            }
        }

        #endregion public event QueryContinueDrag

        /// <summary>
        /// Initializes a new instance of the ItemsControlDragAdorner class.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Use of Rx makes code appear more complex than it is to static analyzer.")]
        protected DragDropTarget()
        {
            // Defer attaching to KeyStatesChanged event until load so that
            // RootVisual is instantiated.
            this.Loaded += delegate { GetKeyStatesChanged().Subscribe(keyStates => this._keyStates = keyStates); };

            SW.DragEventHandler raiseIndicatingInsertionLocationEvent =
                (_, args) =>
                {
                    // Only raise indicating inseration location event if there are effects 
                    if (args.Effects != SW.DragDropEffects.Scroll && args.Effects != SW.DragDropEffects.None)
                    {
                        TItemsControlType dropTarget = GetDropTarget(args);
                        OnIndicatingInsertionLocation(
                            new IndicatingInsertionLocationEventArgs<TItemsControlType>()
                            {
                                DragEventArgs = args,
                                DropTarget = dropTarget,
                                InsertionIndex = GetDropTargetInsertionIndex(dropTarget, args),
                                OriginalSource = this
                            });
                    }
                };

            SW.DragEventHandler hideInsertionIndicator =
                delegate
                {
                    if (_insertionIndicator != null)
                    {
                        _insertionIndicator.Data = null;
                    }
                };

            this.AddHandler(
                SW.DragDrop.DragEnterEvent,
                raiseIndicatingInsertionLocationEvent,
                true);

            this.AddHandler(
                SW.DragDrop.DragOverEvent,
                raiseIndicatingInsertionLocationEvent,
                true);

            this.AddHandler(
                SW.DragDrop.DragLeaveEvent,
                hideInsertionIndicator,
                true);

            this.AddHandler(
                SW.DragDrop.DropEvent,
                hideInsertionIndicator,
                true);

            GetItemDragStarting().Subscribe(ev => OnItemDragStarting(ev.EventArgs));

            _itemDragStarted.Subscribe(ev => OnItemDragStarted(ev.EventArgs));

            IObservable<IEvent<MouseEventArgs>> draggingObservable =
                from dragStarted in _itemDragStarted
                from mouseMove in
                    Application.Current.RootVisual
                        .GetMouseMoveOnSelfAndSiblings()
                        .TakeUntil(_itemDragCompleted)
                select mouseMove;

            draggingObservable.Subscribe(ev => OnDragging(ev.EventArgs));

            _itemDragCompleted.Subscribe(ev => InternalOnItemDragCompleted(ev.EventArgs));
        }

        /// <summary>
        /// Returns a value indicating whether an item is being dragged within
        /// the drag source.
        /// </summary>
        /// <param name="args">Information about the drag event.</param>
        /// <returns>A value indicating whether an item is being dragged within
        /// the drag source.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "The ExtendedRoutedEventArgs type doesn't existing WPF.")]
        protected bool IsDragWithinDragSource(SW.DragEventArgs args)
        {
            // We can only tell if an item is being dragged within the drag source if the drag is originating from another drag drop target.
            object data = args.Data.GetData();

            ItemDragEventArgs currentItemDragEventArgs = data as ItemDragEventArgs;
            if (currentItemDragEventArgs != null)
            {
                TItemsControlType dropTarget = GetDropTarget(args);
                return (currentItemDragEventArgs.DragSource == dropTarget);
            }
            return false;
        }

        /// <summary>
        /// Gets the adjusted insertion index within a drop target given information about a drag event.
        /// </summary>
        /// <param name="dropTarget">The drop target.</param>
        /// <param name="args">Information about a drag event.</param>
        /// <returns>The insertion index within the drop target.</returns>
        protected int GetDropTargetInsertionIndex(TItemsControlType dropTarget, SW.DragEventArgs args)
        {
            if (_adjustedDropTargetInsertionIndex != null)
            {
                return _adjustedDropTargetInsertionIndex.Value;
            }
            else
            {
                return GetDropTargetInsertionIndexOverride(dropTarget, args);
            }
        }

        /// <summary>
        /// Gets an event that is raised whenever the drag and drop key states
        /// are changed.
        /// </summary>
        /// <returns>An event that is raised whenever the drag and drop key 
        /// states are changed.</returns>
        private IObservable<SW.DragDropKeyStates> GetKeyStatesChanged()
        {
            return
                Observable.Merge(
                // before drag starts listen only root visual
                    from _ in
                        Observable.Merge(
                            Observable.Return(new Unit()),
                            _itemDragCompleted.IgnoreAll())
                    from keyStates in
                        Observable
                            .Return(Application.Current)
                            .ObserveOnDispatcher()
                            .SelectMany(app => app.RootVisual.GetKeyStateChangedAlways(_keyStates))
                            .TakeUntil(_itemDragStarted)         
                    select keyStates,
                // after drag starts listen to root visual and all siblings
                    from _ in _itemDragStarted
                    from keyStates in
                        Observable
                            .Return(Application.Current)
                            .ObserveOnDispatcher()
                            .SelectMany(app => app.RootVisual.GetKeyStateChangedOnSelfAndSiblingsAlways(_keyStates))
                            .TakeUntil(_itemDragCompleted)
                    select keyStates);
        }

        /// <summary>
        /// Retrieves the offset of a mouse position relative to an element.
        /// </summary>
        /// <param name="args">The mouse event arguments.</param>
        /// <param name="source">The element to use as the origin.</param>
        /// <returns>The offset of a mouse position relative to an element.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception should be swallowed regardless")]
        private static Point GetOffset(MouseEventArgs args, UIElement source)
        {
            return args.GetSafePosition(source);
        }

        /// <summary>
        /// Returns an observable that raises whenever a drag operation begins
        /// on an item.
        /// </summary>
        /// <returns>An observable that raises whenever a drag operation begins
        /// on an item.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Use of Rx makes code appear more complex than it is to static analyzer.")]
        private IObservable<IEvent<ItemDragEventArgs>> GetItemDragStarting()
        {
            return
                from mouseLeftButtonDown in this.GetMouseLeftButtonDownAlways().ObserveOnDispatcher()
                let originalSource = (UIElement)mouseLeftButtonDown.EventArgs.OriginalSource
                let offset = GetOffset(mouseLeftButtonDown.EventArgs, originalSource)
                from dragStarted in
                    Application.Current.RootVisual
                        .GetMouseMove()
                        .SkipWhile(moveEvent =>
                        {
                            Point point = GetOffset(moveEvent.EventArgs, originalSource);
                            return Math.Abs(point.X - offset.X) < SW.SystemParameters.MinimumHorizontalDragDistance && Math.Abs(point.Y - offset.Y) < SW.SystemParameters.MinimumVerticalDragDistance;
                        })
                        .Take(1)
                        .TakeUntil(Application.Current.RootVisual.GetMouseLeftButtonUpOnSelfAndSiblingsAlways())
                let itemsControl = GetItemsControlAncestor(originalSource)
                where itemsControl != null
                let itemContainer = GetItemContainerAncestor(itemsControl, originalSource)
                where itemContainer != null
                let itemIndex = IndexFromContainer(itemsControl, itemContainer)
                let data = ItemFromContainer(itemsControl, itemContainer)
                where data != null
                select
                    Event.Create(
                        this,
                        new ItemDragEventArgs
                        {
                            DragDecoratorContentMouseOffset = offset,
                            Data = new SelectionCollection() { new Selection(itemIndex, data) },
                            DragSource = itemsControl,
                            AllowedEffects = this.ReadLocalValue(AllowedSourceEffectsProperty) == DependencyProperty.UnsetValue ? GetAllowedEffects(itemsControl) : (SW.DragDropEffects)this.GetValue(AllowedSourceEffectsProperty),
                            DragDecoratorContent =
                                new Image
                                {
                                    Source = new WriteableBitmap(itemContainer, new TranslateTransform())
                                },
                            Handled = false,
                            RemoveDataFromDragSourceAction = (args) => RemoveDataFromItemsControl(itemsControl, args.Data)
                        });
        }

        /// <summary>
        /// Returns the allowed effects for an item drag operation.  Excludes
        /// move if an item cannot be removed from the items control.
        /// </summary>
        /// <param name="itemsControl">The items control to examine to 
        /// determine the allowed effects.</param>
        /// <returns>The allowed effects for an item drag operation.</returns>
        private SW.DragDropEffects GetAllowedEffects(TItemsControlType itemsControl)
        {
            SW.DragDropEffects result = (SW.DragDropEffects.Link | SW.DragDropEffects.Move | SW.DragDropEffects.Scroll);
            if (!CanRemove(itemsControl))
            {
                result = result & ~SW.DragDropEffects.Move;
            }

            return result;
        }

        /// <summary>
        /// This method is invoked when the template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._dragPopup = GetTemplateChild(DragPopupName) as Popup;
            this._dragContainer = GetTemplateChild(DragContainerName) as Canvas;
            this._dragDecorator = GetTemplateChild(DragDecoratorName) as DragDecorator;
            this._insertionIndicator = GetTemplateChild(InsertionIndicatorName) as Path;
            this._insertionIndicatorContainer = GetTemplateChild(InsertionIndicatorContainerName) as Canvas;
        }

        /// <summary>
        /// This method moves the drag decorator with the mouse when the mouse
        /// position changes during a drag operation.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        private void OnDragging(MouseEventArgs args)
        {
            Point mouseLocation = args.GetSafePosition(Application.Current.RootVisual);
            if (double.IsNaN(mouseLocation.X))
            {
                mouseLocation = new Point(0, 0);
            }
            Canvas.SetLeft(_dragDecorator, mouseLocation.X - _currentItemDragEventArgs.DragDecoratorContentMouseOffset.X);
            Canvas.SetTop(_dragDecorator, mouseLocation.Y - _currentItemDragEventArgs.DragDecoratorContentMouseOffset.Y);
        }

        /// <summary>
        /// This method initializes graphical elements when an item drag 
        /// operation begins.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        private void OnItemDragStarted(ItemDragEventArgs args)
        {
            _currentItemDragEventArgs = args;
            _itemWasDroppedOnSource = false;

            GeneralTransform transform = Application.Current.RootVisual.SafeTransformToVisual(_dragPopup);
            Point offset = new Point(0, 0);
            if (transform != null)
            {
                offset = transform.Transform(new Point(0, 0));
            }

            _dragPopup.HorizontalOffset = offset.X;
            _dragPopup.VerticalOffset = offset.Y;

            _dragDecorator.IconPosition = new Point(args.DragDecoratorContentMouseOffset.X + mouseCursorSize.Width, args.DragDecoratorContentMouseOffset.Y + mouseCursorSize.Height);

            Size rootVisualSize = Application.Current.RootVisual.GetSize();

            _dragContainer.Width = rootVisualSize.Width;
            _dragContainer.Height = rootVisualSize.Height;
            _dragDecorator.Visibility = Visibility.Visible;
            _dragDecorator.Content = args.DragDecoratorContent;
            _dragPopup.IsOpen = true;
        }

        #region public event ItemDragCompleted
        /// <summary>
        /// A list of event handles for the ItemDragCompleted event.
        /// </summary>
        private IList<EventHandler<ItemDragEventArgs>> _itemDragCompletedHandlers = new List<EventHandler<ItemDragEventArgs>>();

        /// <summary>
        /// An event raised when the an item drag is completed.
        /// </summary>
        public event EventHandler<ItemDragEventArgs> ItemDragCompleted
        {
            add
            {
                _itemDragCompletedHandlers.Add(value);
            }
            remove
            {
                _itemDragCompletedHandlers.Remove(value);
            }
        }

        /// <summary>
        /// This method hides graphical elements when a drag operation 
        /// completes.  
        /// </summary>
        /// <param name="args">Information about the event.</param>
        private void InternalOnItemDragCompleted(ItemDragEventArgs args)
        {
            _currentItemDragEventArgs = null;
            _dragPopup.IsOpen = false;
            _dragDecorator.Content = null;
            _dragDecorator.Visibility = Visibility.Collapsed;

            if (!_itemWasDroppedOnSource)
            {
                OnItemDroppedOnTarget(new ItemDragEventArgs(args));
            }

            OnItemDragCompleted(args);
        }

        /// <summary>
        /// This method is invoked when an item drag is completed.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        protected virtual void OnItemDragCompleted(ItemDragEventArgs args)
        {
            foreach (EventHandler<ItemDragEventArgs> handler in _itemDragCompletedHandlers)
            {
                handler(this, args);
                if (args.Handled)
                {
                    break;
                }
            }
        }

        #endregion

        /// <summary>
        /// Updates the drag event information whenever a drag event occurs.
        /// </summary>
        /// <param name="args">Information about the event.</param>
        protected virtual void OnDragEvent(SW.DragEventArgs args)
        {
            DependencyObject originalSource = (DependencyObject)args.OriginalSource;
            TItemsControlType dropTarget = GetDropTarget(args);

            SW.DragDropEffects effects = args.AllowedEffects;
            SelectionCollection selectionCollection = GetSelectionCollection(args.Data.GetData());

            // Prevent a move, link, or copy if the items can't be added or if 
            // we're dragging over the drag source and there are no indexes 
            // available.
            if (dropTarget == null
                || selectionCollection.SelectedItems.Any(data => !CanAddItem(dropTarget, data))
                || (_currentItemDragEventArgs != null && _currentItemDragEventArgs.DragSource == dropTarget && selectionCollection.Any(selection => !selection.Index.HasValue)))
            {
                effects &= ~(SW.DragDropEffects.Move | SW.DragDropEffects.Link | SW.DragDropEffects.Copy);
            }

            TItemsControlType itemsControl = GetItemsControlAncestor(originalSource);
            if (itemsControl != null && CanScroll(itemsControl) && (effects & SW.DragDropEffects.Scroll) == SW.DragDropEffects.Scroll)
            {
                if (itemsControl != null)
                {
                    TItemContainerType hoveredItemContainer = GetItemContainerAncestor(itemsControl, originalSource);
                    if (hoveredItemContainer != null)
                    {
                        ScrollIntoView(itemsControl, hoveredItemContainer);
                    }
                }
            }

            if (!args.Handled && effects != args.AllowedEffects)
            {
                args.Effects = effects;
                args.Handled = true;
            }
        }

        /// <summary>
        /// Gets a selection collection from the data in a drag operation.
        /// </summary>
        /// <param name="data">The data being transferred by the drag
        /// operation.</param>
        /// <returns>A selection collection containing the data.</returns>
        internal static SelectionCollection GetSelectionCollection(object data)
        {
            ItemDragEventArgs args = data as ItemDragEventArgs;
            if (args != null)
            {
                data = args.Data;
            }

            return SelectionCollection.ToSelectionCollection(data);
        }

        /// <summary>
        /// Adds data to the drop target.
        /// </summary>
        /// <param name="args">Information about the Drop event.</param>
        protected virtual void OnDropOverride(SW.DragEventArgs args)
        {
            if ((args.AllowedEffects & SW.DragDropEffects.Link) == SW.DragDropEffects.Link
                || (args.AllowedEffects & SW.DragDropEffects.Move) == SW.DragDropEffects.Move)
            {
                object data = args.Data.GetData();

                SelectionCollection selectionCollection = GetSelectionCollection(data);

                TItemsControlType dropTarget = GetDropTarget(args);
                if (dropTarget != null && selectionCollection.All(selection => CanAddItem(dropTarget, selection.Item)))
                {
                    if ((args.Effects & SW.DragDropEffects.Move) == SW.DragDropEffects.Move)
                    {
                        args.Effects = SW.DragDropEffects.Move;
                    }
                    else
                    {
                        args.Effects = SW.DragDropEffects.Link;
                    }

                    int? index = GetDropTargetInsertionIndex(dropTarget, args);

                    if (index != null)
                    {
                        // If the data is coming from a DragDropTarget and a Move
                        // operation is in progress we remove the data before the
                        // the source before adding it to ourselves.  This allows
                        // us to transfer UIElement's which would otherwise be
                        // in the visual tree twice if we added them first.
                        ItemDragEventArgs itemDragEventArgs = data as ItemDragEventArgs;
                        if (args.Effects == SW.DragDropEffects.Move && itemDragEventArgs != null && !itemDragEventArgs.DataRemovedFromDragSource)
                        {
                            itemDragEventArgs.RemoveDataFromDragSource();
                        }

                        foreach (Selection selection in selectionCollection.Reverse())
                        {
                            InsertItem(dropTarget, index.Value, selection.Item);
                        }
                    }
                }
                else
                {
                    args.Effects = SW.DragDropEffects.None;
                }

                if (args.Effects != args.AllowedEffects)
                {
                    args.Handled = true;
                }
            }
        }

        /// <summary>
        /// Removes data from an ItemsControl.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to remove from the ItemsControl.</param>
        private void RemoveDataFromItemsControl(TItemsControlType itemsControl, object data)
        {
            SelectionCollection selectionCollection = GetSelectionCollection(data);
            IEnumerable<Selection> selectionsWithIndexes = selectionCollection.Where(selection => selection.Index.HasValue).OrderByDescending(selection => selection.Index);
            foreach (Selection selection in selectionsWithIndexes)
            {
                RemoveItemAtIndex(itemsControl, selection.Index.Value);
            }

            IEnumerable<Selection> selectionsWithoutIndexes = selectionCollection.Where(selection => !selection.Index.HasValue);
            foreach (Selection selection in selectionsWithoutIndexes)
            {
                RemoveItem(itemsControl, selection.Item);
            }
        }

        #region Methods for sub-class to implement

        /// <summary>
        /// Inserts an item into an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="data">The data to be inserted.</param>
        protected abstract void InsertItem(TItemsControlType itemsControl, int index, object data);

        /// <summary>
        /// Adds an item to an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be inserted.</param>
        protected abstract void AddItem(TItemsControlType itemsControl, object data);

        /// <summary>
        /// Removes an item from an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be removed.</param>
        protected abstract void RemoveItem(TItemsControlType itemsControl, object data);

        /// <summary>
        /// Removes an item from an items control by index.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index of the item to be removed.</param>
        protected virtual void RemoveItemAtIndex(TItemsControlType itemsControl, int index)
        {
            throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.DragDropTarget_RemoveItemAtIndex_RemovalByIndexNotSupported);
        }

        // TODO: Make int? to accomodate possibility that there is no concept of items?

        /// <summary>
        /// Gets the number of items in an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The number of items in the items control.</returns>
        protected abstract int GetItemCount(TItemsControlType itemsControl);

        /// <summary>
        /// Retrieves the item container at a given index.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to retrieve the container.
        /// </param>
        /// <returns>The item container at a given index.</returns>
        protected abstract TItemContainerType ContainerFromIndex(TItemsControlType itemsControl, int index);

        /// <summary>
        /// Retrieves the items host for a given items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The items host for a given items control.</returns>
        protected abstract Panel GetItemsHost(TItemsControlType itemsControl);

        /// <summary>
        /// Retrieves the index of an item container.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>The index of an item container.</returns>
        protected abstract int? IndexFromContainer(TItemsControlType itemsControl, TItemContainerType itemContainer);

        /// <summary>
        /// Gets the item from an item container.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>The data contained by the item container.</returns>
        protected abstract object ItemFromContainer(TItemsControlType itemsControl, TItemContainerType itemContainer);

        /// <summary>
        /// Returns a value indicating whether an item can be removed from the
        /// items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>A value indicating whether an item can be removed from the
        /// items control.</returns>
        protected abstract bool CanRemove(TItemsControlType itemsControl);

        /// <summary>
        /// Returns a value indicating whether an item can be added to the
        /// items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be added.</param>
        /// <returns>A value indicating whether an item can be added to the
        /// items control.</returns>
        protected abstract bool CanAddItem(TItemsControlType itemsControl, object data);

        /// <summary>
        /// Returns a value indicating whether a container belongs to an items 
        /// control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>A value indicating whether a container belongs to an items 
        /// control.</returns>
        protected abstract bool IsItemContainerOfItemsControl(TItemsControlType itemsControl, DependencyObject itemContainer);

        /// <summary>
        /// Returns the items control ancestor of a dependency object.
        /// </summary>
        /// <param name="dependencyObject">The dependency object to retrieve the
        /// element for.</param>
        /// <returns>The items control ancestor of the dependency object.
        /// </returns>
        protected virtual TItemsControlType GetItemsControlAncestor(DependencyObject dependencyObject)
        {
            return (TItemsControlType)this.Content;
        }

        /// <summary>
        /// Returns the item container ancestor of a dependency object.
        /// </summary>
        /// <param name="itemsControl">The items control that contains the
        /// item container.</param>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The item container ancestor of the dependency object.
        /// </returns>
        public virtual TItemContainerType GetItemContainerAncestor(TItemsControlType itemsControl, DependencyObject dependencyObject)
        {
            return
                dependencyObject
                    .GetVisualAncestors()
                    .Prepend(dependencyObject)
                    .OfType<TItemContainerType>()
                    .Where(ancestor => IsItemContainerOfItemsControl(itemsControl, ancestor))
                    .FirstOrDefault();
        }

        /// <summary>
        /// Returns a geometry to use for the insertion indicator given 
        /// an item container, the orientation of the items host panel, and a 
        /// value indicating whether to insert before or after the item 
        /// container.
        /// </summary>
        /// <param name="dropTarget">The drop target.</param>
        /// <param name="insertionIndex">The index at which to insert the item.
        /// </param>
        /// <param name="dragEventArgs">Information about the drag event.
        /// </param>
        /// <returns>The geometry to use for the insertion indicator given 
        /// information about an IndicatingInsertionLocation event.</returns>
        protected virtual Geometry GetInsertionIndicatorGeometry(TItemsControlType dropTarget, int insertionIndex, SW.DragEventArgs dragEventArgs)
        {
            int count = GetItemCount(dropTarget);

            if (count > 0)
            {
                TItemContainerType itemContainer;
                bool insertBefore;
                if (insertionIndex == count)
                {
                    itemContainer = ContainerFromIndex(dropTarget, insertionIndex - 1);
                    insertBefore = false;
                }
                else
                {
                    itemContainer = ContainerFromIndex(dropTarget, insertionIndex);
                    insertBefore = true;
                }

                if (itemContainer != null)
                {
                    Orientation? orientation = GetOrientation(dropTarget);
                    if (orientation != null)
                    {
                        GeneralTransform gt = itemContainer.SafeTransformToVisual(this);
                        if (gt != null)
                        {
                            if (orientation == Orientation.Vertical)
                            {
                                Point offset = gt.Transform(new Point(0, insertBefore ? 0 : itemContainer.GetSize().Height));

                                if (offset.Y <= _insertionIndicatorContainer.GetSize().Height)
                                {
                                    return
                                        new LineGeometry
                                        {
                                            StartPoint = offset,
                                            EndPoint = new Point(offset.X + itemContainer.GetSize().Width, offset.Y)
                                        };
                                }
                            }
                            else
                            {
                                Point offset = gt.Transform(new Point(insertBefore ? 0 : itemContainer.GetSize().Width, 0));

                                if (offset.X <= _insertionIndicatorContainer.GetSize().Height)
                                {
                                    return
                                        new LineGeometry
                                        {
                                            StartPoint = offset,
                                            EndPoint = new Point(offset.X, offset.Y + itemContainer.GetSize().Height)
                                        };
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a value indicating whether a given items control
        /// can scroll.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The value indicating whether the given items control
        /// can scroll.</returns>
        protected virtual bool CanScroll(TItemsControlType itemsControl)
        {
            return false;
        }

        /// <summary>
        /// Scrolls a given item container into the view.
        /// </summary>
        /// <param name="itemsControl">The items control that contains
        /// the item container.</param>
        /// <param name="itemContainer">The item container to scroll into
        /// view.</param>
        protected virtual void ScrollIntoView(TItemsControlType itemsControl, TItemContainerType itemContainer)
        {
        }

        /// <summary>
        /// Retrieves the drop target of a drag event.
        /// </summary>
        /// <param name="args">Information about the drag event.</param>
        /// <returns>The drop target of a drag event.</returns>
        protected virtual TItemsControlType GetDropTarget(SW.DragEventArgs args)
        {
            return GetItemsControlAncestor((DependencyObject)args.OriginalSource);
        }

        /// <summary>
        /// Gets the insertion index within a drop target given information about a drag event.
        /// </summary>
        /// <param name="dropTarget">The drop target.</param>
        /// <param name="args">Information about a drag event.</param>
        /// <returns>The insertion index within the drop target.</returns>
        protected virtual int GetDropTargetInsertionIndexOverride(TItemsControlType dropTarget, SW.DragEventArgs args)
        {
            TItemContainerType targetItemContainer = GetItemContainerAncestor(dropTarget, (DependencyObject)args.OriginalSource);
            int? insertionIndex = null;
            Orientation? orientation = GetOrientation(dropTarget);

            if (orientation != null)
            {
                if (targetItemContainer != null)
                {
                    if (orientation != null)
                    {
                        bool insertBefore = true;
                        Point relativePoint = args.GetPosition(targetItemContainer);
                        insertBefore =
                                (orientation == Orientation.Horizontal
                                    && relativePoint.X < targetItemContainer.GetSize().Width / 2.0)
                                || (orientation == Orientation.Vertical
                                    && relativePoint.Y < targetItemContainer.GetSize().Height / 2.0);

                        insertionIndex = IndexFromContainer(dropTarget, targetItemContainer);
                        if (insertionIndex != null && !insertBefore)
                        {
                            insertionIndex++;
                        }
                    }
                    else
                    {
                        insertionIndex = IndexFromContainer(dropTarget, targetItemContainer);
                    }
                }

                if (insertionIndex != null)
                {
                    return insertionIndex.Value;
                }
            }

            return GetItemCount(dropTarget);
        }
        #endregion Methods for sub-class to implement
    }
}
