// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Microsoft.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// This control draws gridlines with the help of an axis.
    /// </summary>
    internal class GridLines : Canvas
    {
        #region public Axis Axis
        /// <summary>
        /// Gets the axis that the grid lines are connected to.
        /// </summary>
        public Axis Axis
        {
            get { return GetValue(AxisProperty) as Axis; }
            private set { SetValue(AxisProperty, value); }
        }

        /// <summary>
        /// Identifies the Axis dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisProperty =
            DependencyProperty.Register(
                "Axis",
                typeof(Axis),
                typeof(GridLines),
                new PropertyMetadata(null, OnAxisPropertyChanged));

        /// <summary>
        /// AxisProperty property changed handler.
        /// </summary>
        /// <param name="d">GridLines that changed its Axis.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridLines source = (GridLines)d;
            Axis oldValue = (Axis)e.OldValue;
            Axis newValue = (Axis)e.NewValue;
            source.OnAxisPropertyChanged(oldValue, newValue);
        }

        /// <summary>
        /// AxisProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected virtual void OnAxisPropertyChanged(Axis oldValue, Axis newValue)
        {
            Debug.Assert(newValue != null, "Don't set the axis property to null.");

            if (newValue != null)
            {
                newValue.Invalidated += OnAxisInvalidated;
            }

            if (oldValue != null)
            {
                oldValue.Invalidated -= OnAxisInvalidated;
            }
        }
        #endregion public Axis Axis

        /// <summary>
        /// Instantiates a new instance of the GridLines class.
        /// </summary>
        /// <param name="axis">The axis used by the GridLines.</param>
        public GridLines(Axis axis)
        {
            this.Axis = axis;
            this.SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
        }

        /// <summary>
        /// Redraws grid lines when the size of the control changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Invalidate();
        }

        /// <summary>
        /// Redraws grid lines when the axis is invalidated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void OnAxisInvalidated(object sender, RoutedEventArgs e)
        {
            Invalidate();
        }

        /// <summary>
        /// Draws the grid lines.
        /// </summary>
        internal void Invalidate()
        {
            IList<IComparable> intervals = Enumerable.ToList(Axis.GetLinesIntervals());
            IList<Line> lines = Children.Where(x => x is Line).Cast<Line>().ToList();

            while (lines.Count > intervals.Count)
            {
                Children.Remove(lines[lines.Count - 1]);
                lines.RemoveAt(lines.Count - 1);
            }

            while (lines.Count < intervals.Count)
            {
                lines.Add(new Line() { Style = Axis.GridLineStyle });
                Children.Add(lines[lines.Count - 1]);
            }

            for (int i = 0; i < intervals.Count; i++)
            {
                IComparable currentValue = intervals[i];

                double position = Axis.AxisType == AxisType.Category ?
                    Axis.GetDataPointCategoryByIndexInPixels((double)currentValue) :
                    Axis.GetPlotAreaCoordinate(currentValue);
                if (!double.IsNaN(position))
                {
                    Line line = lines[i];
                    if (Axis.Orientation == AxisOrientation.Vertical)
                    {
                        line.Y1 = line.Y2 = Math.Round(ActualHeight - position);
                        line.X1 = 0.0;
                        line.X2 = ActualWidth;
                    }
                    else if (Axis.Orientation == AxisOrientation.Horizontal)
                    {
                        line.X1 = line.X2 = Math.Round(position);
                        line.Y1 = 0.0;
                        line.Y2 = ActualHeight;
                    }
                }
            }
        }
    }
}