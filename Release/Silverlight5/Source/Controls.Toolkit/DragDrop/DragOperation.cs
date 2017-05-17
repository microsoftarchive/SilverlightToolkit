// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Internals;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#if SILVERLIGHT
using SW = Microsoft.Windows;
#else
using SW = System.Windows;
#endif

namespace Microsoft.Windows
{
    /// <summary>
    /// An asynchronous drag operation.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    internal class DragOperation : IObservable<SW.DragDropEffects>
    {
        /// <summary>
        /// The interval at which to pulse DragOver events when the mouse
        /// isn't moving.
        /// </summary>
        private const int MouseOverPulseIntervalInMilliseconds = 250;

        /// <summary>
        /// Gets or sets a value indicating whether a drag operation is in 
        /// process.
        /// </summary>
        private static bool IsDragging { get; set; }

        /// <summary>
        /// Information retrieved when the drag operation began.
        /// </summary>
        private SW.DragEventArgs _dragStartEventArgs;

        /// <summary>
        /// The effects authorized by the drag source.
        /// </summary>
        private SW.DragDropEffects _allowedEffects = SW.DragDropEffects.None;

        /// <summary>
        /// Information about the last drag event.
        /// </summary>
        private SW.DragEventArgs _lastDragEventArgs = null;

        /// <summary>
        /// Information about the last give feedback event.
        /// </summary>
        private SW.GiveFeedbackEventArgs _lastGiveFeedbackEventArgs;

        /// <summary>
        /// An event that is raised when a drag operations is started.
        /// </summary>
        private ISubject<IEvent<SW.DragEventArgs>> _dragStarted = new Subject<IEvent<SW.DragEventArgs>>();

        /// <summary>
        /// An event that is raised when a drag operations is starting.
        /// </summary>
        private ISubject<IEvent<SW.DragEventArgs>> _dragStarting = new Subject<IEvent<SW.DragEventArgs>>();

        /// <summary>
        /// An event that is raised when the drag source should be queried 
        /// whether to continue the drag operation.
        /// </summary>
        private ISubject<IEvent<SW.QueryContinueDragEventArgs>> _dragSourceQueryContinueDrag = new Subject<IEvent<SW.QueryContinueDragEventArgs>>();

        /// <summary>
        /// An event that is raised when the drag operation is completed.
        /// </summary>
        private ISubject<SW.DragDropEffects> _dragCompleted = new Subject<SW.DragDropEffects>();

        /// <summary>
        /// The source of the data being dragged.
        /// </summary>
        private DependencyObject _dragSource;

        /// <summary>
        /// The state of the input keys relevant to drag operations.
        /// </summary>
        private SW.DragDropKeyStates _keyStates;

        /// <summary>
        /// The element currently being dragged over.
        /// </summary>
        private DependencyObject _currentDragOverElement;

        /// <summary>
        /// A value indicating whether the escape key is pressed.
        /// </summary>
        private bool _escapePressed;

        /// <summary>
        /// Gets or sets the state of the input keys relevant to drag 
        /// operations.
        /// </summary>
        private SW.DragDropKeyStates KeyStates
        {
            get
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    _keyStates |= SW.DragDropKeyStates.ControlKey;
                }
                else
                {
                    _keyStates &= ~SW.DragDropKeyStates.ControlKey;
                }
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    _keyStates |= SW.DragDropKeyStates.ShiftKey;
                }
                else
                {
                    _keyStates &= ~SW.DragDropKeyStates.ShiftKey;
                }
                if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                {
                    _keyStates |= SW.DragDropKeyStates.AltKey;
                }
                else
                {
                    _keyStates &= ~SW.DragDropKeyStates.AltKey;
                }

                return _keyStates;
            }
            set
            {
                _keyStates = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DragOperation class.
        /// </summary>
        /// <param name="dragSource">The source of the drag operation.</param>
        /// <param name="data">The data associated with the drag operation.
        /// </param>
        /// <param name="allowedEffects">The allowed effects of the drag 
        /// operation.
        /// </param>
        /// <param name="initialKeyState">The initial state of the keys relevant 
        /// to drag operations.</param>
        public DragOperation(DependencyObject dragSource, object data, SW.DragDropEffects allowedEffects, SW.DragDropKeyStates initialKeyState)
        {
            SW.IDataObject dataObject = data as SW.IDataObject;
            if (dataObject == null)
            {
                dataObject = new DataObject(data);
            }

            _allowedEffects = allowedEffects;
            KeyStates = initialKeyState;

            _dragSource = dragSource;
            SW.DragEventArgs dragStartEventArgs =
                new SW.DragEventArgs()
                {
                    OriginalSource = dragSource,
                    AllowedEffects = allowedEffects,
                    Effects = allowedEffects,
                    Data = dataObject
                };

            _lastDragEventArgs = dragStartEventArgs;
            dragStartEventArgs.AllowedEffects = allowedEffects;
            dragStartEventArgs.Data = dataObject;

            this._dragStartEventArgs = dragStartEventArgs;

            IObservable<IEvent<SW.QueryContinueDragEventArgs>> drop =
                from dragStartedEvent in _dragStarting
                from dropEvent in 
                    _dragSourceQueryContinueDrag
                        .Where(
                            ev => ev.EventArgs.Handled 
                                && ev.EventArgs.Action == SW.DragAction.Drop)
                        .Take(1)
                        .TakeUntil(_dragCompleted)
                select dropEvent;

            IObservable<IEvent<SW.QueryContinueDragEventArgs>> sourceQueryContinue =
                from dragStartedEvent in _dragStarting
                from queryContinueDragEvent in 
                    _dragSourceQueryContinueDrag
                        .Where(
                            ev => 
                                ev.EventArgs.Handled 
                                && (ev.EventArgs.Action == SW.DragAction.Drop || ev.EventArgs.Action == SW.DragAction.Cancel))
                        .Take(1)
                        .TakeUntil(_dragCompleted)
                select queryContinueDragEvent;

            sourceQueryContinue
                .Where(queryContinueDragEvent => queryContinueDragEvent.EventArgs.Action == SW.DragAction.Drop)
                .ObserveOnDispatcher()
                .Subscribe(queryContinueDragEvent => OnTargetDrop());

            sourceQueryContinue
                .Where(queryContinueDragEvent => queryContinueDragEvent.EventArgs.Action == SW.DragAction.Cancel)
                .ObserveOnDispatcher()
                .Subscribe(queryContinueDragEvent => OnCancel());

            _dragCompleted.Subscribe(_ => IsDragging = false);
        }

        /// <summary>
        /// This method starts the drag operation and sends the results to 
        /// an observer.
        /// </summary>
        /// <param name="observer">The observer listening for the result of
        /// the drag operation.</param>
        /// <returns>A disposable object used to detach from a drag operation.
        /// </returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Use of Rx makes complexity seem artificially high to static analyzer.  It's necessary to initialize events in the constructor.")]
        public IDisposable Subscribe(IObserver<SW.DragDropEffects> observer)
        {
            if (IsDragging)
            {
                throw new InvalidOperationException("A drag operation is already in progress.");
            }
            IsDragging = true;

            // Always execute a QueryContinueDrag first.
            _dragStarting.Take(1)
                .ObserveOnDispatcher()
                .Subscribe(
                    dragEventArgs =>
                    {
                        SW.QueryContinueDragEventArgs args = OnDragSourceQueryContinueDrag();
                        if (!(args.Handled && args.Action == SW.DragAction.Cancel))
                        {
                            OnDragSourceGiveFeedback(_lastDragEventArgs);
                            _dragStarted.OnNext(dragEventArgs);
                        }
                    });

            IObservable<SW.DragDropKeyStates> keyStatesChanged =
                from dragStarted in _dragStarting
                where Application.Current != null
                from keyState in
                    Application.Current.RootVisual
                        .GetKeyStateChangedOnSelfAndSiblingsAlways(KeyStates)
                        .TakeUntil(_dragCompleted)
                select keyState;

            keyStatesChanged.Subscribe(keyState => KeyStates = keyState);

            IObservable<bool> escapePressedChanged =
                from dragStated in _dragStarting
                where Application.Current != null
                from escapePressed in
                    Application.Current.RootVisual
                        .GetEscapePressedChangedOnSelfAndSiblingsAlways()
                        .TakeUntil(_dragCompleted)
                select escapePressed;

            escapePressedChanged.Subscribe(pressed => _escapePressed = pressed);
            
            Observable
                .Merge(
                    keyStatesChanged.IgnoreAll(),
                    escapePressedChanged.IgnoreAll())
                .TakeUntil(_dragCompleted)
                .ObserveOnDispatcher()
                .Subscribe(_ => RaiseDragSourceEvents(_lastDragEventArgs));

            IObservable<IEvent<SW.DragEventArgs>> dragOver =
                from dragStartedEvent in _dragStarted
                let mouseMoveDragOver = GetMouseMoveDragOver()
                from dragMouseOverEvent in mouseMoveDragOver.TakeUntil(_dragCompleted)
                from pulseDragMouseOverEvent in
                    Observable.Merge(
                        Observable.Return(dragMouseOverEvent),
                        Observable
                            .Interval(TimeSpan.FromMilliseconds(MouseOverPulseIntervalInMilliseconds))
                            .ObserveOnDispatcher()
                            .Select(_ =>
                            {
                                UIElement originalSource = dragMouseOverEvent.EventArgs.OriginalSource as UIElement;

                                if (originalSource != null)
                                {
                                    Point pointWithinOriginalSource = dragMouseOverEvent.EventArgs.GetPosition((UIElement)dragMouseOverEvent.EventArgs.OriginalSource);
                                    // if point is no longer within original source find the element
                                    // in the tree that contains the point and make it the original source (and sender)
                                    // or the next dragover event.
                                    if (!new Rect(new Point(0, 0), originalSource.GetSize()).Contains(pointWithinOriginalSource))
                                    {
                                        originalSource = GetDragOverOriginalSource(dragMouseOverEvent.EventArgs);
                                    }
                                }

                                return Event.Create(originalSource, new SW.DragEventArgs(dragMouseOverEvent.EventArgs) { OriginalSource = originalSource });
                            })
                            .TakeUntil(
                                Observable.Merge(
                                    mouseMoveDragOver.IgnoreAll(),
                                    _dragCompleted.IgnoreAll())))
                where IsDragging
                select pulseDragMouseOverEvent;

            dragOver
                .ObserveOnDispatcher()
                .Subscribe(dragOverEvent => OnDragOver(dragOverEvent.Sender, dragOverEvent.EventArgs));
            
            _dragStarting.OnNext(Event.Create(_dragSource, _dragStartEventArgs));
            _dragCompleted
                .ObserveOnDispatcher()
                .Subscribe(observer);

            return new AnonymousDisposable(() => { Debug.Assert(false, "Should never detach from DragOperation."); });
        }

        /// <summary>
        /// Retrieves the original source of a new DragOver event.  Attempts to
        /// determine the original source by finding the the deepest element
        /// in the tree that the mouse is over.
        /// </summary>
        /// <param name="args">Information about the drag event.</param>
        /// <returns>The original source of a new DragOver event.</returns>
        private static UIElement GetDragOverOriginalSource(SW.DragEventArgs args)
        {
            UIElement originalSource = args.OriginalSource as UIElement;
            // Use the previous original source and go to its root.
            // Note: this won't work if a popup appears on top of the
            // original source but it will work if the previous original source
            // is inside of a popup.
            UIElement rootVisual = originalSource.GetVisualAncestors().OfType<UIElement>().LastOrDefault();
            if (rootVisual != null)
            {
                // If the original source disappears (ex. a popup disappears),
                // use the root visual.
                rootVisual = Application.Current.RootVisual;

                originalSource =
                    FunctionalProgramming.TraverseBreadthFirst(
                        rootVisual,
                        node => node.GetVisualChildren().OfType<UIElement>(),
                        node => new Rect(new Point(0, 0), node.GetSize()).Contains(args.GetPosition(node)))
                        .LastOrDefault();
            }
            return originalSource;
        }

        /// <summary>
        /// Raises a routed drag event and stores information about the drag
        /// event.
        /// </summary>
        /// <param name="element">An element that accepts a drop.</param>
        /// <param name="eventArgs">Information about the drag event.</param>
        /// <param name="raiseAction">An action that raises the specific drag event.
        /// </param>
        /// <returns>Information about the drag event that may have been
        /// modified by handlers.</returns>
        private SW.DragEventArgs RaiseRoutedDragEvent(DependencyObject element, SW.DragEventArgs eventArgs, Action<DependencyObject, SW.DragEventArgs> raiseAction)
        {
            SW.DragEventArgs dragEventArgs =
                new SW.DragEventArgs(eventArgs)
                {
                    Effects = eventArgs.AllowedEffects,
                    OriginalSource = element
                };

            if ((bool)element.GetValue(UIElement.AllowDropProperty))
            {
                element.RaiseRoutedEvent(
                    dragEventArgs,
                    (obj, args) =>
                    {
                        raiseAction(obj, args);
                    });
            }
            else
            {
                dragEventArgs.Effects = DragDropEffects.None;
                dragEventArgs.Handled = true;
            }

            _lastDragEventArgs = dragEventArgs;

            return dragEventArgs;
        }

        /// <summary>
        /// Raises the GiveFeedback and QueryContinueDrag events on the drag
        /// source.
        /// </summary>
        /// <param name="args">Information about the last drag event.</param>
        private void RaiseDragSourceEvents(SW.DragEventArgs args)
        {
            OnDragSourceGiveFeedback(args);

            OnDragSourceQueryContinueDrag();
        }

        /// <summary>
        /// Returns the allowed effects by analyzing the state of the keys 
        /// pressed.  If the control key is pressed the user is requesting a 
        /// copy. If copy is available the effect will be only copy, if not 
        /// available the effect will be None.
        /// </summary>
        /// <param name="allowedEffects">The allowed effects.</param>
        /// <returns>The effects of the drag operation.</returns>
        private SW.DragDropEffects GetDragDropEffects(SW.DragDropEffects allowedEffects)
        {
            if ((KeyStates & SW.DragDropKeyStates.ControlKey) == SW.DragDropKeyStates.ControlKey)
            {
                if ((KeyStates & SW.DragDropKeyStates.ShiftKey) == SW.DragDropKeyStates.ShiftKey)
                {
                    if ((allowedEffects & SW.DragDropEffects.Link) == SW.DragDropEffects.Link)
                    {
                        return SW.DragDropEffects.Link;
                    }
                    else
                    {
                        return SW.DragDropEffects.None;
                    }
                }
                else if ((allowedEffects & SW.DragDropEffects.Copy) == SW.DragDropEffects.Copy)
                {
                    return SW.DragDropEffects.Copy;
                }
                else
                {
                    return SW.DragDropEffects.None;
                }
            }
            else if ((allowedEffects & SW.DragDropEffects.Move) == SW.DragDropEffects.Move)
            {
                return SW.DragDropEffects.Move;
            }
            else
            {
                return allowedEffects;
            }
        }

        /// <summary>
        /// Raises the GiveFeedback event on the drag source.
        /// </summary>
        /// <param name="args">Information about the GiveFeedback event.
        /// </param>
        private void OnDragSourceGiveFeedback(SW.DragEventArgs args)
        {
            SW.DragDropEffects effects = _allowedEffects;

            if (args.Handled)
            {
                effects = _allowedEffects & args.Effects;
            }
            else
            {
                effects = GetDragDropEffects(_allowedEffects);
            }

            _lastGiveFeedbackEventArgs =
                new SW.GiveFeedbackEventArgs()
                {
                    Effects = effects,
                    OriginalSource = _dragSource,
                };

            _dragSource.RaiseRoutedEvent(
                _lastGiveFeedbackEventArgs,
                (accept, e) => accept.OnGiveFeedback(e));
        }

        /// <summary>
        /// Raises the QueryContinueDragEvent on the drag source.
        /// </summary>
        /// <returns>Information about the QueryContinueDrag event.</returns>
        private SW.QueryContinueDragEventArgs OnDragSourceQueryContinueDrag()
        {
            SW.QueryContinueDragEventArgs queryContinueDragEventArgs =
                new SW.QueryContinueDragEventArgs()
                {
                    Action = SW.DragAction.Continue,
                    EscapePressed = _escapePressed,
                    KeyStates = KeyStates,
                    OriginalSource = _dragSource
                };

            DependencyObject dragSource = _dragSource;
            if (dragSource != null)
            {
                queryContinueDragEventArgs =
                    dragSource.RaiseRoutedEvent(
                        queryContinueDragEventArgs,
                        (acc, e) => acc.OnQueryContinueDrag(e));
            }

            if (!queryContinueDragEventArgs.Handled)
            {
                if (queryContinueDragEventArgs.EscapePressed)
                {
                    queryContinueDragEventArgs.Action = SW.DragAction.Cancel;
                    queryContinueDragEventArgs.Handled = true;
                }
                else if ((queryContinueDragEventArgs.KeyStates & SW.DragDropKeyStates.LeftMouseButton) != SW.DragDropKeyStates.LeftMouseButton)
                {
                    queryContinueDragEventArgs.Action = (_lastGiveFeedbackEventArgs == null || _lastGiveFeedbackEventArgs.Effects != SW.DragDropEffects.None) ? SW.DragAction.Drop : SW.DragAction.Cancel;
                    queryContinueDragEventArgs.Handled = true;
                }
            }

            _dragSourceQueryContinueDrag.OnNext(
                 Event.Create(
                    _dragSource,
                    queryContinueDragEventArgs));

            return queryContinueDragEventArgs;
        }

        /// <summary>
        /// This method is invoked when the drag operation is cancelled.
        /// </summary>
        private void OnCancel()
        {
            OnDragLeave();
            _dragCompleted.OnNext(SW.DragDropEffects.None);
        }

        /// <summary>
        /// This method raises the Drop event.
        /// </summary>
        private void OnTargetDrop()
        {
            UIElement dropTarget = _lastDragEventArgs.OriginalSource as UIElement;
            if (dropTarget != null && dropTarget.AllowDrop)
            {
                SW.DragEventArgs dropEventArgs =
                    RaiseRoutedDragEvent(dropTarget, _lastDragEventArgs, (acc, ev) => acc.OnDrop(ev));

                // Regardless of whether event args is handled and what the allowed effects are
                // WPF drag and drop always returns the effects after a drop event.
                _dragCompleted.OnNext(dropEventArgs.Effects);
            }
            else
            {
                _dragCompleted.OnNext(SW.DragDropEffects.None);
            }
        }

        /// <summary>
        /// Gets an event raised when an item is dragged over any object in the 
        /// visual tree.
        /// </summary>
        /// <returns>An event raised when an item is dragged over any object in the 
        /// visual tree.</returns>
        private IObservable<IEvent<SW.DragEventArgs>> GetMouseMoveDragOver()
        {
            return
                Application.Current.RootVisual
                    .GetMouseMoveOnSelfAndSiblings()
                    .TakeUntil(_dragCompleted)
                    .Select(mouseMoveEvent =>
                        Tuple.Create(
                            ((UIElement)mouseMoveEvent.EventArgs.OriginalSource),
                            mouseMoveEvent.EventArgs))
                    .Select(
                        acceptDropSourceAndEventArgs =>
                            Event.Create(
                                acceptDropSourceAndEventArgs.Item1,
                                new SW.DragEventArgs(_dragStartEventArgs)
                                {
                                    OriginalSource = acceptDropSourceAndEventArgs.Item1,
                                    MouseEventArgs = acceptDropSourceAndEventArgs.Item2
                                }));
        }

        /// <summary>
        /// Raises the DragEnter event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">Information about the event.</param>
        private void OnDragEnter(object sender, SW.DragEventArgs eventArgs)
        {
            OnDragLeave();

            _currentDragOverElement = eventArgs.OriginalSource as DependencyObject;
            DependencyObject acceptDrop = (DependencyObject)sender;
            eventArgs =
                RaiseRoutedDragEvent(
                    acceptDrop,
                    eventArgs,
                    (ancestor, args) => ancestor.OnDragEnter(args));

            RaiseDragSourceEvents(eventArgs);
        }

        /// <summary>
        /// Raises the DragOver event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">Information about the event.</param>
        private void OnDragOver(object sender, SW.DragEventArgs eventArgs)
        {
            UIElement target = eventArgs.OriginalSource as UIElement;
            if (_currentDragOverElement != target)
            {
                OnDragEnter(sender, new SW.DragEventArgs(eventArgs));
            }

            DependencyObject acceptDrop = (DependencyObject)sender;
            eventArgs =
               RaiseRoutedDragEvent(
                   acceptDrop,
                   eventArgs,
                   (ancestor, args) => ancestor.OnDragOver(args));

            RaiseDragSourceEvents(eventArgs);
        }

        /// <summary>
        /// Raises the DragLeave event.
        /// </summary>
        private void OnDragLeave()
        {
            if (_currentDragOverElement != null)
            {
                SW.DragEventArgs eventArgs = new SW.DragEventArgs(_lastDragEventArgs) { OriginalSource = _currentDragOverElement };

                DependencyObject acceptDrop = (DependencyObject)_currentDragOverElement;
                eventArgs =
                    RaiseRoutedDragEvent(
                        acceptDrop,
                        eventArgs,
                        (ancestor, args) => ancestor.OnDragLeave(args));
            }
        }
    }
}