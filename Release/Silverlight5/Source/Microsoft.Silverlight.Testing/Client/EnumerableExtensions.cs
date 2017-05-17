// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Silverlight.Testing.Controls
{
    /// <summary>
    /// A set of extension methods for the sequence class.
    /// </summary>;dsf
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Produces a sequence of items using a seed value and iteration 
        /// method.
        /// </summary>
        /// <typeparam name="T">The type of the sequence.</typeparam>
        /// <param name="value">The initial value.</param>
        /// <param name="next">The iteration function.</param>
        /// <returns>A sequence of items using a seed value and iteration 
        /// method.</returns>
        public static IEnumerable<T> Iterate<T>(T value, Func<T, T> next)
        {
            do
            {
                yield return value;
                value = next(value);
            }
            while (true);
        }

        /// <summary>
        /// Prepend an item to a sequence.
        /// </summary>
        /// <typeparam name="T">The type of the sequence.</typeparam>
        /// <param name="that">The sequence to append the item to.</param>
        /// <param name="value">The item to append to the sequence.</param>
        /// <returns>A new sequence.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is linked into multiple projects.")]
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> that, T value)
        {
            if (that == null)
            {
                throw new ArgumentNullException("that");
            }

            yield return value;
            foreach (T item in that)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Accepts two sequences and applies a function to the corresponding 
        /// values in the two sequences.
        /// </summary>
        /// <typeparam name="T0">The type of the first sequence.</typeparam>
        /// <typeparam name="T1">The type of the second sequence.</typeparam>
        /// <typeparam name="R">The return type of the function.</typeparam>
        /// <param name="enumerable0">The first sequence.</param>
        /// <param name="enumerable1">The second sequence.</param>
        /// <param name="func">The function to apply to the corresponding values
        /// from the two sequences.</param>
        /// <returns>A sequence of transformed values from both sequences.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Code is linked into multiple projects.")]
        public static IEnumerable<R> Zip<T0, T1, R>(IEnumerable<T0> enumerable0, IEnumerable<T1> enumerable1, Func<T0, T1, R> func)
        {
            IEnumerator<T0> enumerator0 = enumerable0.GetEnumerator();
            IEnumerator<T1> enumerator1 = enumerable1.GetEnumerator();
            while (enumerator0.MoveNext() && enumerator1.MoveNext())
            {
                yield return func(enumerator0.Current, enumerator1.Current);
            }
        }
    }
}