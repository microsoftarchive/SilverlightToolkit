// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Specifies different types of axes.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum AxisType
    {
        /// <summary>
        /// An axis that has its type set by the first series to use it.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// An axis which aggregates and presents a list of categories.
        /// </summary>
        Category = 1,

        /// <summary>
        /// An axis which has uniform increments.
        /// </summary>
        Linear = 2,

        /// <summary>
        /// An axis which binds to any set of date/time values.
        /// </summary>
        DateTime = 3
    }
}