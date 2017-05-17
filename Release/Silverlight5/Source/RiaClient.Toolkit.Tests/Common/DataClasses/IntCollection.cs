//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="IntCollection.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.ObjectModel;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// A collection of integers.
    /// </summary>
    public class IntCollection : Collection<int>
    {
        /// <summary>
        /// Constructs a new <see cref="IntCollection"/> with 10 ints.
        /// </summary>
        public IntCollection()
            : this(10)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="IntCollection"/> with <paramref name="numInts"/> ints.
        /// </summary>
        /// <param name="numInts">The number of ints to put into the collection.</param>
        public IntCollection(int numInts)
        {
            for (int i = 0; i < numInts; i++)
            {
                this.Add(i);
            }
        }
    }
}
