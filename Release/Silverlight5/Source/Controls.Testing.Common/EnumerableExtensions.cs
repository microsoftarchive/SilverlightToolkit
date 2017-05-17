// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Extension methods for the IEnumerable class.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Only returns a new item if it has a different type than the previous
        /// item.
        /// </summary>
        /// <typeparam name="T">The type of the items in the sequence.</typeparam>
        /// <param name="that">The sequence of items.</param>
        /// <returns>A filtered sequence of items.</returns>
        public static IEnumerable<T> HoldUntilChanges<T>(this IEnumerable<T> that)
        {
            IEnumerator<T> enumerator = that.GetEnumerator();
            if (enumerator.MoveNext())
            {
                T previous = enumerator.Current;
                yield return enumerator.Current;

                while (enumerator.MoveNext())
                {
                    if (!object.Equals(previous, enumerator.Current))
                    {
                        previous = enumerator.Current;
                        yield return enumerator.Current;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a value indicating whether the items in the left sequence
        /// and the right sequence are equal.
        /// </summary>
        /// <typeparam name="T">The type of the items in the sequence.</typeparam>
        /// <param name="left">The left sequence.</param>
        /// <param name="right">The right sequence.</param>
        /// <returns>A value indicating whether the items in the left sequence
        /// and the right sequence are equal.</returns>
        public static bool ItemsEqual<T>(this IEnumerable<T> left, IEnumerable<T> right)
        {
            IEnumerator leftEnumerator = left.GetEnumerator();
            IEnumerator rightEnumerator = right.GetEnumerator();

            while (leftEnumerator.MoveNext() && rightEnumerator.MoveNext())
            {
                if (leftEnumerator.Current != rightEnumerator.Current)
                {
                    return false;
                }
            }
            return true;
        }
    }
}