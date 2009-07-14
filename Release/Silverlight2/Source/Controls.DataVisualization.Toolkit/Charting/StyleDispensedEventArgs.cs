// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;

namespace System.Windows.Controls.DataVisualization
{
    /// <summary>
    /// Information describing the style dispensed when a StyleDispensed event 
    /// is raised.
    /// </summary>
    internal class StyleDispensedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the StyleDispensedEventArgs class.
        /// </summary>
        /// <param name="index">The index of the style dispensed.</param>
        /// <param name="style">The style dispensed.</param>
        public StyleDispensedEventArgs(int index, Style style)
        {
            this.Style = style;
            this.Index = index;
        }

        /// <summary>
        /// Gets the index of the style dispensed.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Gets the style dispensed.
        /// </summary>
        public Style Style { get; private set; }

        /// <summary>
        /// Returns a value indicating whether two style dispenser arguments are 
        /// equal.
        /// </summary>
        /// <param name="obj">The other StyleDispenser object.</param>
        /// <returns>A value indicating whether two style dispenser arguments 
        /// are equal.</returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}