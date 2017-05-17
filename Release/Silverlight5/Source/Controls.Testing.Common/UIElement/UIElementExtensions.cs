// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Controls.Internals;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// A set of extension methods for UIElement.
    /// </summary>
    internal static partial class UIElementExtensions
    {
        /// <summary>
        /// Returns an observable that represents the LayoutUpdated
        /// event.
        /// </summary>
        /// <param name="that">The instance to create an observable
        /// for.</param>
        /// <returns>An observable that represents the LayoutUpdated 
        /// event.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "It's necessary to nest generic types.")]
        public static IObservable<IEvent<EventArgs>> GetLayoutUpdated(this FrameworkElement that)
        {
            return Observable.FromEvent<EventArgs>(that, "LayoutUpdated");
        }

        /// <summary>
        /// Returns an observable that represents the next LayoutUpdated
        /// event.
        /// </summary>
        /// <param name="that">The instance to create an observable
        /// for.</param>
        /// <returns>An observable that represents the LayoutUpdated 
        /// event.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "It's necessary to nest generic types.")]
        public static IObservable<IEvent<EventArgs>> GetNextLayoutUpdated(this FrameworkElement that)
        {
            return that.GetLayoutUpdated().Take(1);
        }

        /// <summary>
        /// Returns an observable that represents the SizeChanged event.
        /// </summary>
        /// <param name="that">The instance to create an observable
        /// for.</param>
        /// <returns>An observable that represents the SizeChanged event.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "It's necessary to nest generic types.")]
        public static IObservable<IEvent<SizeChangedEventArgs>> GetSizeChanged(this FrameworkElement that)
        {
            return Observable.FromEvent<SizeChangedEventArgs>(that, "SizeChanged"); 
        }

        /// <summary>
        /// Returns an observable that represents the next SizeChanged
        /// event.
        /// </summary>
        /// <param name="that">The instance to create an observable
        /// for.</param>
        /// <returns>An observable that represents the SizeChanged 
        /// event.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "It's necessary to nest generic types.")]
        public static IObservable<IEvent<SizeChangedEventArgs>> GetNextSizeChanged(this FrameworkElement that)
        {
            return that.GetSizeChanged().Take(1);
        }

        /// <summary>
        /// An observable sequence that returns when a dispatcher invokes.
        /// </summary>
        /// <param name="that">The framework element.</param>
        /// <returns>An observable sequence that returns when a dispatcher 
        /// invokes.</returns>
        public static IObservable<Unit> GetBeginInvoke(this FrameworkElement that)
        {
            return new AnonymousObservable<Unit>(
                observer =>
                {
                    bool hasDisposed = false;
                    IDisposable dispose = new AnonymousDisposable(() => hasDisposed = true);

                    that.Dispatcher.BeginInvoke(
                        new Action(
                        () =>
                        {
                            if (!hasDisposed)
                            {
                                hasDisposed = true;
                                observer.OnNext(new Unit());
                                observer.OnCompleted();
                            }
                        }));

                    return dispose;
                });
        }

        /// <summary>
        /// An observable sequence that returns when the size changes, the
        /// layout updates, or a dispatcher task invokes.
        /// </summary>
        /// <param name="that">The framework element.</param>
        /// <returns>An observable sequence that returns when the size changes, 
        /// the layout updates, or a dispatcher task invokes.</returns>
        public static IObservable<Unit> AfterNextLayoutUpdate(this FrameworkElement that)
        {
            return
                Observable.Merge(
                    that.GetNextSizeChanged().IgnoreAll(),
                    that.GetNextLayoutUpdated().IgnoreAll(),
                    that.GetBeginInvoke().Delay(TimeSpan.FromMilliseconds(2000)).ObserveOnDispatcher())
                    .Take(1);
        }

        /// <summary>
        /// Returns an observable that wraps the KeyUp event and
        /// returns even if one of the events involved is handled.
        /// </summary>
        /// <param name="that">The element to listen to.</param>
        /// <returns>An observable that that wraps the KeyUp event 
        /// and returns even if one of the events involved is handled.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Rx observables based on events are implemented this way.")]
        public static IObservable<IEvent<KeyEventArgs>> GetKeyUpAlways(this UIElement that)
        {
            return new AnonymousObservable<IEvent<KeyEventArgs>>(
                (observer) =>
                {
                    KeyEventHandler handler = (o, a) => observer.OnNext(Event.Create(o, a));
                    that.AddHandler(UIElement.KeyUpEvent, handler, true);

                    return new AnonymousDisposable(() => that.RemoveHandler(UIElement.KeyUpEvent, handler));
                });
        }

        /// <summary>
        /// Gets an observable sequence based on the GotFocus event.
        /// </summary>
        /// <param name="that">The element to listen to.</param>
        /// <returns>An observable sequence based on the GotFocus event.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Rx observables based on events are implemented this way.")]
        public static IObservable<IEvent<RoutedEventArgs>> GetGotFocus(this UIElement that)
        {
            return Observable.FromEvent<RoutedEventArgs>(that, "GotFocus");
        }

        /// <summary>
        /// Gets an observable sequence based on the MouseLeftButtonDown event.
        /// </summary>
        /// <param name="that">The element.</param>
        /// <returns>An observable sequence based on the MouseLeftButtonDown event.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Rx observables based on events are implemented this way.")]
        public static IObservable<IEvent<MouseButtonEventArgs>> GetMouseLeftButtonDown(this UIElement that)
        {
            return Observable.FromEvent<MouseButtonEventArgs>(that, "MouseLeftButtonDown");
        }

        /// <summary>
        /// Gets an observable sequence based on the MouseLeftButtonUp event.
        /// </summary>
        /// <param name="that">The element.</param>
        /// <returns>An observable sequence based on the MouseLeftButtonUp event.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Rx observables based on events are implemented this way.")]
        public static IObservable<IEvent<MouseButtonEventArgs>> GetMouseLeftButtonUp(this UIElement that)
        {
            return Observable.FromEvent<MouseButtonEventArgs>(that, "MouseLeftButtonUp");
        }
    }
}