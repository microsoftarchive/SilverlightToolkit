// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System
{
    /// <summary>
    /// Represents a 2-tuple, or pair.
    /// </summary>
    /// <typeparam name="T1">The type of the first component.</typeparam>
    /// <typeparam name="T2">The type of the second component.</typeparam>
    /// <QualityBand>Experimental</QualityBand>
    internal sealed class Tuple<T1, T2>
    {
        /// <summary>
        /// Initializes a new instance of the Tuple class.
        /// </summary>
        /// <param name="value1">The value of the first component.</param>
        /// <param name="value2">The value of the second component.</param>
        public Tuple(T1 value1, T2 value2)
        {
            this.Item1 = value1;
            this.Item2 = value2;
        }

        /// <summary>
        /// Gets the value of the first component.
        /// </summary>
        public T1 Item1 { get; private set; }

        /// <summary>
        /// Gets the value of the second component.
        /// </summary>
        public T2 Item2 { get; private set; }
    }
}