// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Collections;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// A base class that contains methods used by both the line and area series.
    /// </summary>
    /// <typeparam name="T">The type of data point used by the series.</typeparam>
    public abstract class LineAreaBaseSeries<T> : DataPointSingleSeriesWithAxes
        where T : DataPoint, new()
    {
        /// <summary>
        /// Gets data points collection sorted by independent value.
        /// </summary>
        internal OrderedMultipleDictionary<IComparable, DataPoint> DataPointsByIndependentValue { get; private set; }

        /// <summary>
        /// Gets the independent axis as a range axis.
        /// </summary>
        public IAxis ActualIndependentAxis { get { return this.InternalActualIndependentAxis as IAxis; } }

        /// <summary>
        /// Gets the dependent axis as a range axis.
        /// </summary>
        public IRangeAxis ActualDependentRangeAxis { get { return this.InternalActualDependentAxis as IRangeAxis; } }

        /// <summary>
        /// Initializes a new instance of the LineAreaBaseSeries class.
        /// </summary>
        protected LineAreaBaseSeries()
        {
            DataPointsByIndependentValue =
                new OrderedMultipleDictionary<IComparable, DataPoint>(
                    false,
                    (left, right) =>
                        left.CompareTo(right),
                    (leftDataPoint, rightDataPoint) =>
                            RuntimeHelpers.GetHashCode(leftDataPoint).CompareTo(RuntimeHelpers.GetHashCode(rightDataPoint)));
        }

        /// <summary>
        /// Called after data points have been loaded from the items source.
        /// </summary>
        /// <param name="newDataPoints">New active data points.</param>
        /// <param name="oldDataPoints">Old inactive data points.</param>
        protected override void OnDataPointsChanged(IList<DataPoint> newDataPoints, IList<DataPoint> oldDataPoints)
        {
            base.OnDataPointsChanged(newDataPoints, oldDataPoints);

            if (ActualIndependentAxis is IRangeAxis)
            {
                foreach (DataPoint dataPoint in oldDataPoints)
                {
                    DataPointsByIndependentValue.Remove((IComparable)dataPoint.IndependentValue, dataPoint);
                }

                foreach (DataPoint dataPoint in newDataPoints)
                {
                    DataPointsByIndependentValue.Add((IComparable)dataPoint.IndependentValue, dataPoint);
                }
            }
        }

        /// <summary>
        /// This method executes after all data points have been updated.
        /// </summary>
        protected override void OnAfterUpdateDataPoints()
        {
            if (InternalActualDependentAxis != null && InternalActualIndependentAxis != null)
            {
                UpdateShape();
            }
        }

        /// <summary>
        /// Repositions line data point in the sorted collection if the actual 
        /// independent axis is a range axis.
        /// </summary>
        /// <param name="dataPoint">The data point that has changed.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataPointIndependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
            if (ActualIndependentAxis is IRangeAxis && !oldValue.Equals(newValue))
            {
                bool removed = DataPointsByIndependentValue.Remove((IComparable)oldValue, dataPoint);
                if (removed)
                {
                    DataPointsByIndependentValue.Add((IComparable)newValue, dataPoint);
                }
            }

            base.OnDataPointIndependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Creates a new line data point.
        /// </summary>
        /// <returns>A line data point.</returns>
        protected override DataPoint CreateDataPoint()
        {
            return new T();
        }

        /// <summary>
        /// Returns the style to use for all data points.
        /// </summary>
        /// <returns>The style to use for all data points.</returns>
        protected override IEnumerator<Style> GetStyleEnumeratorFromHost()
        {
            return SeriesHost.GetStylesWithTargetType(typeof(T), true);
        }

        /// <summary>
        /// Updates the point collection object.
        /// </summary>
        protected abstract void UpdateShape();
    }
}
