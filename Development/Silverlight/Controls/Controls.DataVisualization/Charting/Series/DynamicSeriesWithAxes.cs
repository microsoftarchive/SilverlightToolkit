// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents a dynamic series that uses axes to display data points.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public abstract class DynamicSeriesWithAxes : DynamicSeries, IAxisInformationProvider
    {
        #region protected Axis DependentAxis
        /// <summary>
        /// Gets or sets the value of the Dependent axis.
        /// </summary>
        internal Axis DependentAxis
        {
            get { return GetValue(DependentAxisProperty) as Axis; }
            set { SetValue(DependentAxisProperty, value); }
        }

        /// <summary>
        /// Identifies the DependentAxis dependency property.
        /// </summary>
        private static readonly DependencyProperty DependentAxisProperty =
            DependencyProperty.Register(
                "DependentAxis",
                typeof(Axis),
                typeof(DynamicSeries),
                null);
        #endregion protected Axis DependentAxis

        #region protected Axis IndependentAxis
        /// <summary>
        /// Gets or sets the value of the Independent axis.
        /// </summary>
        internal Axis IndependentAxis
        {
            get { return GetValue(IndependentAxisProperty) as Axis; }
            set { SetValue(IndependentAxisProperty, value); }
        }

        /// <summary>
        /// Identifies the IndependentAxis dependency property.
        /// </summary>
        private static readonly DependencyProperty IndependentAxisProperty =
            DependencyProperty.Register(
                "IndependentAxis",
                typeof(Axis),
                typeof(DynamicSeries),
                null);
        #endregion protected Axis IndependentAxis

        /// <summary>
        /// Updates the axes when a new data point is added.
        /// </summary>
        /// <param name="dataPoint">The data point to add.</param>
        protected override void AddDataPoint(DataPoint dataPoint)
        {
            base.AddDataPoint(dataPoint);

            if (!LoadingDataPoints)
            {
                UpdateDependentAxis();
                UpdateIndependentAxis();
            }
        }

        /// <summary>
        /// Updates the dependent and independent axes when a data point is
        /// removed.
        /// </summary>
        /// <param name="dataPoint">The data point to remove.</param>
        protected override void RemoveDataPoint(DataPoint dataPoint)
        {
            base.RemoveDataPoint(dataPoint);

            if (!LoadingDataPoints)
            {
                UpdateDependentAxis();
                UpdateIndependentAxis();
            }
        }

        /// <summary>
        /// Update the axes when the specified data point's ActualDependentValue property changes.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataPointActualDependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
            UpdateDependentAxis();
            UpdateDataPoint(dataPoint);
            base.OnDataPointActualDependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Update the axes when the specified data point's DependentValue property changes.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataPointDependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
            if ((null != DependentAxis) && ((DependentAxis.AxisType == AxisType.Linear) || (DependentAxis.AxisType == AxisType.DateTime)))
            {
                dataPoint.BeginAnimation(DataPoint.ActualDependentValueProperty, "ActualDependentValue", newValue, this.TransitionDuration);
            }
            else
            {
                dataPoint.ActualDependentValue = newValue;
            }
            base.OnDataPointDependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Update axes when the specified data point's effective dependent value changes.
        /// </summary>
        private void UpdateDependentAxis()
        {
            if (DependentAxis != null)
            {
                switch (DependentAxis.AxisType)
                {
                    case AxisType.Category:
                        DependentAxis.Update(this, GetCategories(DependentAxis));
                        break;
                    case AxisType.DateTime:
                        DependentAxis.Update(this, GetDateTimeRange(DependentAxis));
                        break;
                    case AxisType.Linear:
                        DependentAxis.Update(this, GetNumericRange(DependentAxis));
                        break;
                }
            }
        }

        /// <summary>
        /// Update axes when the specified data point's actual independent value changes.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataPointActualIndependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
            UpdateIndependentAxis();
            UpdateDataPoint(dataPoint);
            base.OnDataPointActualIndependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Update axes when the specified data point's independent value changes.
        /// </summary>
        /// <param name="dataPoint">The data point.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataPointIndependentValueChanged(DataPoint dataPoint, object oldValue, object newValue)
        {
            if ((null != IndependentAxis) && ((IndependentAxis.AxisType == AxisType.Linear) || (IndependentAxis.AxisType == AxisType.DateTime)))
            {
                dataPoint.BeginAnimation(DataPoint.ActualIndependentValueProperty, "ActualIndependentValue", newValue, this.TransitionDuration);
            }
            else
            {
                dataPoint.ActualIndependentValue = newValue;
            }
            base.OnDataPointIndependentValueChanged(dataPoint, oldValue, newValue);
        }

        /// <summary>
        /// Update axes when a data point's effective independent value changes.
        /// </summary>
        private void UpdateIndependentAxis()
        {
            if (IndependentAxis != null)
            {
                switch (IndependentAxis.AxisType)
                {
                    case AxisType.Category:
                        IndependentAxis.Update(this, GetCategories(IndependentAxis));
                        break;
                    case AxisType.DateTime:
                        IndependentAxis.Update(this, GetDateTimeRange(IndependentAxis));
                        break;
                    case AxisType.Linear:
                        IndependentAxis.Update(this, GetNumericRange(IndependentAxis));
                        break;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the DynamicSeriesWithAxes class.
        /// </summary>
        internal DynamicSeriesWithAxes()
        {
        }

        /// <summary>
        /// Acquires axes after data points have loaded.
        /// </summary>
        protected override void OnDataPointsLoaded()
        {
            GetAxes();

            UpdateIndependentAxis();
            UpdateDependentAxis();

            base.OnDataPointsLoaded();
        }

        /// <summary>
        /// Method called to get series to acquire the axes it needs.  Acquires
        /// no axes by default.
        /// </summary>
        private void GetAxes()
        {
            if (SeriesHost != null)
            {
                DataPoint firstDataPoint = ActiveDataPoints.FirstOrDefault();
                if (firstDataPoint == null)
                {
                    return;
                }

                double dependentValue;
                if (!ValueHelper.TryConvert(firstDataPoint.DependentValue, out dependentValue))
                {
                    throw new InvalidOperationException(Properties.Resources.DynamicSeriesWithAxes_DependentValueMustBeNumeric);
                }

                GetAxes(firstDataPoint);
            }
        }

        /// <summary>
        /// Method called to get series to acquire the axes it needs.  Acquires
        /// no axes by default.
        /// </summary>
        /// <param name="firstDataPoint">The first data point.</param>
        protected abstract void GetAxes(DataPoint firstDataPoint);

        /// <summary>
        /// Acquires a category axis.
        /// </summary>
        /// <param name="orientation">The desired orientation of the axis.
        /// </param>
        /// <param name="axisInitializationAction">A function that initializes 
        /// a newly created axis.</param>
        /// <param name="axisPropertyAccessor">A function that returns the 
        /// current value of the property used to store the axis.</param>
        /// <param name="axisPropertySetter">A function that accepts an axis
        /// value and assigns it to the property intended to store a reference
        /// to it.</param>
        protected void GetCategoryAxis(
                    AxisOrientation orientation,
                    Action<Axis> axisInitializationAction,
                    Func<Axis> axisPropertyAccessor,
                    Action<Axis> axisPropertySetter)
        {
            Type categoryType = typeof(object);

            Axis currentIndependentAxis = axisPropertyAccessor() as Axis;

            // Only acquire new independent axis if necessary
            if (currentIndependentAxis == null
                || !currentIndependentAxis.CategoryType.IsAssignableFrom(categoryType)
                || currentIndependentAxis.Orientation != orientation)
            {
                Axis categoryAxis =
                    SeriesHost.Axes.OfType<Axis>()
                        .Where(axis => (axis.AxisType == AxisType.Category || axis.AxisType == AxisType.Auto)
                            && axis.CategoryType.IsAssignableFrom(categoryType)
                            && axis.Orientation == orientation)
                        .FirstOrDefault();

                if (categoryAxis == null)
                {
                    categoryAxis = new Axis { AxisType = AxisType.Category, CategoryType = categoryType };
                    categoryAxis.Orientation = orientation;
                    categoryAxis.RemoveIfUnused = true;
                    axisInitializationAction(categoryAxis);
                }
                else if (categoryAxis.AxisType == AxisType.Auto)
                {
                    categoryAxis.AxisType = AxisType.Category;
                }

                // Unregister with current axis if it has one.
                if (axisPropertyAccessor() != null)
                {
                    axisPropertyAccessor().Invalidated -= OnAxisInvalidated;
                    SeriesHost.UnregisterWithAxis(this, axisPropertyAccessor());
                }

                // Set axis to be its independent axis
                axisPropertySetter(categoryAxis);
                categoryAxis.Invalidated += OnAxisInvalidated;

                // Register new axis with series host
                SeriesHost.RegisterWithAxis(this, axisPropertyAccessor());
            }
        }

        /// <summary>
        /// Acquires a range axis.
        /// </summary>
        /// <param name="firstDataPoint">A data point used to determine
        /// where a date or numeric axis is required.</param>
        /// <param name="orientation">The desired orientation of the axis.
        /// </param>
        /// <param name="axisInitializationAction">A function that initializes 
        /// a newly created axis.</param>
        /// <param name="axisPropertyAccessor">A function that returns the 
        /// current value of the property used to store the axis.</param>
        /// <param name="axisPropertySetter">A function that accepts an axis
        /// value and assigns it to the property intended to store a reference
        /// to it.</param>
        /// <param name="dataPointAxisValueGetter">A function that accepts a
        /// Control and returns the value that will be plot on the axis.
        /// </param>
        /// <param name="notDateTimeOrNumericExceptionMessage">An exception
        /// message to use if the type of the value retrieved from the 
        /// data point is neither a double or a date.</param>
        protected void GetRangeAxis(
                    DataPoint firstDataPoint,
                    AxisOrientation orientation,
                    Action<Axis> axisInitializationAction,
                    Func<Axis> axisPropertyAccessor,
                    Action<Axis> axisPropertySetter,
                    Func<DataPoint, object> dataPointAxisValueGetter,
                    string notDateTimeOrNumericExceptionMessage)
        {
            // Acquiring independent axes
            Axis axis;
            bool isAxisNumeric = true;
            double doubleValue = 0.0;
            DateTime dateTimeValue;
            if (ValueHelper.TryConvert(dataPointAxisValueGetter(firstDataPoint), out doubleValue))
            {
                isAxisNumeric = true;
            }
            else if (ValueHelper.TryConvert(dataPointAxisValueGetter(firstDataPoint), out dateTimeValue))
            {
                isAxisNumeric = false;
            }
            else
            {
                throw new InvalidOperationException(notDateTimeOrNumericExceptionMessage);
            }
            if (axisPropertyAccessor() != null && axisPropertyAccessor().AxisType == AxisType.Auto)
            {
                if (isAxisNumeric)
                {
                    axisPropertyAccessor().AxisType = AxisType.Linear;
                }
                else
                {
                    axisPropertyAccessor().AxisType = AxisType.DateTime;
                }
            }
            // If current axis is not suitable...
            if (axisPropertyAccessor() == null || (isAxisNumeric && !(axisPropertyAccessor().AxisType == AxisType.Linear))
                || !(axisPropertyAccessor().AxisType == AxisType.DateTime))
            {
                // Attempt to find suitable axis
                IEnumerable<Axis> axesWithCorrectOrientation =
                    SeriesHost.Axes.Cast<Axis>().Where(currentAxis => currentAxis.Orientation == orientation);
                if (isAxisNumeric)
                {
                    axis = axesWithCorrectOrientation.Where(currentAxis => currentAxis.AxisType == AxisType.Linear || currentAxis.AxisType == AxisType.Auto).FirstOrDefault();
                }
                else
                {
                    axis = axesWithCorrectOrientation.Where(currentAxis => currentAxis.AxisType == AxisType.DateTime || currentAxis.AxisType == AxisType.Auto).FirstOrDefault();
                }

                if (axis == null)
                {
                    if (isAxisNumeric)
                    {
                        axis = new Axis();
                        axis.AxisType = AxisType.Linear;
                    }
                    else
                    {
                        axis = new Axis();
                        axis.AxisType = AxisType.DateTime;
                    }
                    axisInitializationAction(axis);
                    axis.RemoveIfUnused = true;
                    axis.Orientation = orientation;
                }
                else
                {
                    if (isAxisNumeric)
                    {
                        axis.AxisType = AxisType.Linear;
                    }
                    else
                    {
                        axis.AxisType = AxisType.DateTime;
                    }
                }

                // Unregister with current axis if it has one.
                if (axisPropertyAccessor() != null)
                {
                    axisPropertyAccessor().Invalidated -= OnAxisInvalidated;
                    SeriesHost.UnregisterWithAxis(this, axisPropertyAccessor());
                }

                axisPropertySetter(axis);
                axis.Invalidated += OnAxisInvalidated;
                
                SeriesHost.RegisterWithAxis(this, axis);
            }
        }

        /// <summary>
        /// Updates data points when the axis is invalidated.
        /// </summary>
        /// <param name="sender">The axis that was invalidated.</param>
        /// <param name="e">The event data.</param>
        private void OnAxisInvalidated(object sender, EventArgs e)
        {
            if (DependentAxis != null && IndependentAxis != null && PlotArea != null)
            {
                UpdateAllDataPoints();
            }
        }

        /// <summary>
        /// If data is found returns the minimum and maximum dependent numeric 
        /// values.
        /// </summary>
        /// <param name="axis">Axis that needs the data.</param>
        /// <returns>The range of values or empty if no data is present.
        /// </returns>
        public Range<double> GetNumericRange(Axis axis)
        {
            if (axis == null)
            {
                throw new ArgumentNullException("axis");
            }

            Func<DataPoint, double> selector = null;
            if (axis == IndependentAxis)
            {
                selector = (dataPoint) => ValueHelper.ToDouble(dataPoint.ActualIndependentValue);
            }
            else if (axis == DependentAxis)
            {
                selector = (dataPoint) => ValueHelper.ToDouble(dataPoint.ActualDependentValue);
            }
            return (null != selector) ? ActiveDataPoints.Select(selector).GetRange() : Range<double>.Empty;
        }

        /// <summary>
        /// If data is found returns the minimum and maximum dependent date 
        /// values.
        /// </summary>
        /// <param name="axis">Axis that needs the data.</param>
        /// <returns>The range of values or empty if no data is present.
        /// </returns>
        public Range<DateTime> GetDateTimeRange(Axis axis)
        {
            if (axis == null)
            {
                throw new ArgumentNullException("axis");
            }

            Func<DataPoint, DateTime> selector = null;
            if (axis == IndependentAxis)
            {
                selector = (dataPoint) => ValueHelper.ToDateTime(dataPoint.ActualIndependentValue);
            }
            else if (axis == DependentAxis)
            {
                selector = (dataPoint) => ValueHelper.ToDateTime(dataPoint.ActualDependentValue);
            }
            return (null != selector) ? ActiveDataPoints.Select(selector).GetRange() : Range<DateTime>.Empty;
        }

        /// <summary>
        /// Returns the minimum difference between two values on a given axis.
        /// </summary>
        /// <param name="axis">The axis to retrieve the minimum difference 
        /// for.</param>
        /// <returns>The minimum difference between two values on a given axis.
        /// </returns>
        public double GetMinimumValueDelta(Axis axis)
        {
            if (axis == null)
            {
                throw new ArgumentNullException("axis");
            }

            Func<DataPoint, double> selector = null;
            if (axis == IndependentAxis)
            {
                selector = (dataPoint) => (double) dataPoint.IndependentValue;
            }
            else if (axis == DependentAxis)
            {
                selector = (dataPoint) => (double) dataPoint.DependentValue;
            }
            var smallestAndSecondSmallest = ActiveDataPoints.Select(selector).OrderBy(x => x).Take(2).ToList();
            return smallestAndSecondSmallest[1] - smallestAndSecondSmallest[0];
        }

        /// <summary>
        /// Returns the minimum time span between two dates on a given axis.
        /// </summary>
        /// <param name="axis">The axis to retrieve the minimum time span 
        /// for.</param>
        /// <returns>The minimum time span between two dates on a given axis.
        /// </returns>
        public TimeSpan GetMinimumTimeSpanDelta(Axis axis)
        {
            if (axis == null)
            {
                throw new ArgumentNullException("axis");
            }

            Func<DataPoint, DateTime> selector = null;
            if (axis == IndependentAxis)
            {
                selector = (dataPoint) => (DateTime)dataPoint.ActualIndependentValue;
            }
            else if (axis == DependentAxis)
            {
                selector = (dataPoint) => (DateTime)dataPoint.ActualDependentValue;
            }
            var smallestAndSecondSmallest = ActiveDataPoints.Select(selector).OrderBy(x => x).Take(2).ToList();
            return smallestAndSecondSmallest[1] - smallestAndSecondSmallest[0];
        }

        /// <summary>
        /// Returns a list of categories used by the series.
        /// </summary>
        /// <param name="axis">The axis for which to retrieve the categories.
        /// </param>
        /// <returns>A list of categories used by the series.</returns>
        public IEnumerable<object> GetCategories(Axis axis)
        {
            if (axis == null)
            {
                throw new ArgumentNullException("axis");
            }

            Func<DataPoint, object> selector = null;
            if (axis == IndependentAxis)
            {
                if (IndependentValueBinding == null)
                {
                    return Enumerable.Range(1, ActiveDataPointCount).Cast<object>();
                }
                selector = (dataPoint) => dataPoint.IndependentValue ?? dataPoint.DependentValue;
            }
            else if (axis == DependentAxis)
            {
                selector = (dataPoint) => dataPoint.DependentValue;
            }

            return ActiveDataPoints.Select(selector).Distinct();
        }

        /// <summary>
        /// Called when the value of the SeriesHost property changes.
        /// </summary>
        /// <param name="oldValue">The value to be replaced.</param>
        /// <param name="newValue">The new series host value.</param>
        protected override void OnSeriesHostPropertyChanged(ISeriesHost oldValue, ISeriesHost newValue)
        {
            // Unregister from old Parent
            if (oldValue != null)
            {
                if (IndependentAxis != null)
                {
                    oldValue.UnregisterWithAxis(this, IndependentAxis);
                    IndependentAxis = null;
                }
                if (DependentAxis != null)
                {
                    oldValue.UnregisterWithAxis(this, DependentAxis);
                    DependentAxis = null;
                }
            }

            // Register with new Parent
            if (newValue != null)
            {
                if (IndependentAxis != null)
                {
                    newValue.RegisterWithAxis(this, IndependentAxis);
                }
                if (DependentAxis != null)
                {
                    newValue.RegisterWithAxis(this, DependentAxis);
                }
            }

            base.OnSeriesHostPropertyChanged(oldValue, newValue);
        }
    }
}