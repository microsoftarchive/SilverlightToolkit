// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// An observable collection of axes.
    /// </summary>
    internal class AxisCollection : NoResetObservableCollection<Axis>
    {
        /// <summary>
        /// Initializes a new instance of the AxisCollection class.
        /// </summary>
        public AxisCollection()
        {
        }

        /// <summary>
        /// Ensures that a maximum of one horizontal and one vertical axis are 
        /// inserted into the collection.
        /// </summary>
        /// <param name="index">The index at which to insert the axis.</param>
        /// <param name="item">The axis to insert.</param>
        protected override void InsertItem(int index, Axis item)
        {
            if (Enumerable.Any(this, axis => axis.Orientation == item.Orientation))
            {
                if (item.Orientation == AxisOrientation.Horizontal)
                {
                    throw new InvalidOperationException(Properties.Resources.AxisCollection_CannotHaveMoreThanOneHorizontalAxis);
                }
                else if (item.Orientation == AxisOrientation.Vertical)
                {
                    throw new InvalidOperationException(Properties.Resources.AxisCollection_CannotHaveMoreThanOneVerticalAxis);
                }
            }
            base.InsertItem(index, item);
        }
    }
}