// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;

namespace System.Linq
{
    /// <summary>
    /// A set of extension methods for IObservable objects.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    internal static class ObservableExtensions
    {
        /// <summary>
        /// Converts an observable sequence of values into unit values.
        /// </summary>
        /// <typeparam name="T">The type of the observable sequence.</typeparam>
        /// <param name="that">The sequence to convert.</param>
        /// <returns>A sequence of unit values.</returns>
        public static IObservable<Unit> IgnoreAll<T>(this IObservable<T> that)
        {
            return that.Select(_ => new Unit());
        }

        /// <summary>
        /// Returns an observable that will return continuously at a certain
        /// interval.
        /// </summary>
        /// <param name="interval">The number of ticks between intervals.</param>
        /// <returns>A sequence of items separated by an interval.</returns>
        public static IObservable<Unit> Interval(long interval)
        {
            return new AnonymousObservable<Unit>(
                observer =>
                {
                    DispatcherTimer dispatcherTimer = new DispatcherTimer();

                    dispatcherTimer.Interval = new TimeSpan(interval);
                    EventHandler handler = (o, a) => observer.OnNext(new Unit());
                    dispatcherTimer.Tick += handler;
                    dispatcherTimer.Start();

                    return new AnonymousDisposable(
                        () =>
                        {
                            dispatcherTimer.Tick -= handler;
                            dispatcherTimer.Stop();
                        });
                });
        }
    }
}