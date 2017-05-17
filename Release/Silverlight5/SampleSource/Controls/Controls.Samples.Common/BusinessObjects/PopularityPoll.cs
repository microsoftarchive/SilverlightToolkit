// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// A popularity poll for something, taken on a given date.
    /// </summary>
    public class PopularityPoll
    {
        /// <summary>
        /// Gets or sets the date on which the poll was taken.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the percentage of support the thing had.
        /// </summary>
        public double Percent { get; set; }
    }
}