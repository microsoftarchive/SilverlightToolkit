// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Windows.Controls.DataVisualization
{
    /// <summary>
    /// This class contains general purpose functions to manipulate the generic
    /// IEnumerable type.
    /// </summary>
    internal static class EnumerableFunctions
    {
        /// <summary>
        /// Returns whether a stream is empty.
        /// </summary>
        /// <typeparam name="T">The type of the stream.</typeparam>
        /// <param name="that">The stream.</param>
        /// <returns>Whether the stream is empty or not.</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> that)
        {
            IEnumerator<T> enumerator = that.GetEnumerator();
            return !enumerator.MoveNext();
        }

        /////// <summary>
        /////// Performs a projection with the index of the item in the stream.
        /////// </summary>
        /////// <typeparam name="T">The type of the stream.</typeparam>
        /////// <typeparam name="R">The type of the returned stream.</typeparam>
        /////// <param name="that">The stream to apply the projection to.</param>
        /////// <param name="func">The function to apply to each item.</param>
        /////// <returns>A stream of the returned values.</returns>
        ////public static IEnumerable<R> SelectWithIndex<T, R>(this IEnumerable<T> that, Func<T, int, R> func)
        ////{
        ////    int counter = 0;

        ////    foreach (T item in that)
        ////    {
        ////        yield return func(item, counter);
        ////        counter++;
        ////    }
        ////}

        /// <summary>
        /// Accepts two streams and applies a function to the corresponding 
        /// values in the two streams.
        /// </summary>
        /// <typeparam name="T0">The type of the first stream.</typeparam>
        /// <typeparam name="T1">The type of the second stream.</typeparam>
        /// <typeparam name="R">The return type of the function.</typeparam>
        /// <param name="enumerable0">The first stream.</param>
        /// <param name="enumerable1">The second stream.</param>
        /// <param name="func">The function to apply to the corresponding values
        /// from the two streams.</param>
        /// <returns>A stream of transformed values from both streams.</returns>
        public static IEnumerable<R> Zip<T0, T1, R>(IEnumerable<T0> enumerable0, IEnumerable<T1> enumerable1, Func<T0, T1, R> func)
        {
            IEnumerator<T0> enumerator0 = enumerable0.GetEnumerator();
            IEnumerator<T1> enumerator1 = enumerable1.GetEnumerator();
            while (enumerator0.MoveNext() && enumerator1.MoveNext())
            {
                yield return func(enumerator0.Current, enumerator1.Current);
            }
        }

        /////// <summary>
        /////// Creates a stream of values by accepting an initial value, an 
        /////// iteration function, and apply the iteration function recursively.
        /////// </summary>
        /////// <typeparam name="T">The type of the stream.</typeparam>
        /////// <param name="value">The initial value.</param>
        /////// <param name="nextFunction">The function to apply to the value.
        /////// </param>
        /////// <returns>A stream of the iterated values.</returns>
        ////public static IEnumerable<T> Iterate<T>(T value, Func<T, T> nextFunction)
        ////{
        ////    yield return value;
        ////    while (true)
        ////    {
        ////        value = nextFunction(value);
        ////        yield return value;
        ////    }
        ////}

        /// <summary>
        /// Returns the index of an item in a stream.
        /// </summary>
        /// <typeparam name="T">The type of the stream.</typeparam>
        /// <param name="that">The stream.</param>
        /// <param name="value">The item to search for.</param>
        /// <returns>The index of the item or -1 if not found.</returns>
        public static int IndexOf<T>(this IEnumerable<T> that, T value)
        {
            int index = 0;
            foreach (T item in that)
            {
                if (object.ReferenceEquals(value, item) || value.Equals(item))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of an item in a stream.
        /// </summary>
        /// <typeparam name="T">The type of the stream.</typeparam>
        /// <param name="that">The stream.</param>
        /// <param name="func">The item to search for.</param>
        /// <returns>The index of the item or -1 if not found.</returns>
        public static int? IndexOf<T>(this IEnumerable<T> that, Func<T, bool> func)
        {
            int index = 0;
            foreach (T item in that)
            {
                if (func(item))
                {
                    return index;
                }
                index++;
            }
            return new int?();
        }

        /// <summary>
        /// Executes an action for each item and a stream, passing in the index
        /// of that item to the action procedure.
        /// </summary>
        /// <typeparam name="T">The type of the stream.</typeparam>
        /// <param name="that">The stream.</param>
        /// <param name="action">A function that accepts a stream item and its
        /// index in the stream.</param>
        public static void ForEachWithIndex<T>(this IEnumerable<T> that, Action<T, int> action)
        {
            int index = 0;
            foreach (T item in that)
            {
                action(item, index);
                index++;
            }
        }

        /////// <summary>
        /////// Accepts two streams and returns a stream of pairs containing one 
        /////// item from each stream at a corresponding index.
        /////// </summary>
        /////// <typeparam name="T">The type of the streams.</typeparam>
        /////// <param name="stream0">The first stream.</param>
        /////// <param name="stream1">The second stream.</param>
        /////// <returns>A stream of pairs.</returns>
        ////public static IEnumerable<Pair<T>> ZipPair<T>(IEnumerable<T> stream0, IEnumerable<T> stream1)
        ////{
        ////    IEnumerator<T> enumerable0 = stream0.GetEnumerator();
        ////    IEnumerator<T> enumerable1 = stream1.GetEnumerator();
        ////    while (enumerable0.MoveNext() && enumerable1.MoveNext())
        ////    {
        ////        yield return new Pair<T>(enumerable0.Current, enumerable1.Current);
        ////    }
        ////}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="K"></typeparam>
        ///// <typeparam name="R"></typeparam>
        ///// <param name="that"></param>
        ///// <param name="keySelector"></param>
        ///// <param name="itemSelector"></param>
        ///// <returns></returns>
        ////public static Dictionary<KeyType, ItemType> ToTransformedDictionary<InputType, KeyType, ItemType>(this IEnumerable<InputType> that, Func<InputType, KeyType> keySelector, Func<InputType, ItemType> itemSelector)
        ////{
        ////    Dictionary<KeyType, ItemType> dictionary = new Dictionary<KeyType, ItemType>();
        ////    foreach (InputType item in that)
        ////    {
        ////        dictionary[keySelector(item)] = itemSelector(item);
        ////    }
        ////    return dictionary;
        ////}
    }
}