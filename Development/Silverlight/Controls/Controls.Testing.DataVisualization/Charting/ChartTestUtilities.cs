// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Controls;
using Microsoft.Windows.Controls.DataVisualization.Charting;

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// Collection of utilities for use when testing charts.
    /// </summary>
    public static class ChartTestUtilities
    {
        /// <summary>
        /// Returns a list of data points within a series.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>A list of data points in the series.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This function will only work with a series.")]
        public static IList<DataPoint> GetDataPointsForSeries(Series series)
        {
            return series.GetVisualChildren().OfType<DataPoint>().Where(dataPoint => dataPoint.Parent.GetType() == typeof(Canvas)).ToList();
        }
    }
}