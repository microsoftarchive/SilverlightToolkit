// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System
{
    /// <summary>
    /// Provides static methods for creating tuple objects.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    internal static class Tuple
    {
        /// <summary>
        /// Creates a new 2-tuple, or pair.
        /// </summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
        /// <param name="value1">The value of the first component of the tuple.</param>
        /// <param name="value2">The value of the second component of the tuple.</param>
        /// <returns>A 2-tuple whose value is (item1, item2).</returns>
        public static Tuple<T1, T2> Create<T1, T2>(T1 value1, T2 value2)
        {
            return new Tuple<T1, T2>(value1, value2);
        }
    }
}