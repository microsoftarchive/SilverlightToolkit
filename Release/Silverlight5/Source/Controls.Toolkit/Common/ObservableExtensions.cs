// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls.Internals;
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
    }
}