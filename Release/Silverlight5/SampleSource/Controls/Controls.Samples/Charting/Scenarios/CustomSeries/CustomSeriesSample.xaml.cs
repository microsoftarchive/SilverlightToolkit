// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.DataVisualization.Charting;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Charting sample demonstrating how to create a custom series.
    /// </summary>
    [Sample("(2)Custom Series", DifficultyLevel.Advanced, "DataVisualization")]
    public partial class CustomSeriesSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the CustomSeriesSample class.
        /// </summary>
        public CustomSeriesSample()
        {
            InitializeComponent();

            // Use a custom function for a series
            FunctionSeries series = CustomFunctionChart.Series[1] as FunctionSeries;
            series.Function = x => 110 + 3 * Math.Sin(x);
        }

        /// <summary>
        /// Perform a regression against the particulate levels data.
        /// </summary>
        /// <param name="sender">The regression ComboBox.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by an event handler in XAML.")]
        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body", Justification = "Simplifies the sample.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Simplifies the sample.")]
        private void OnRegressionTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the options and the series
            ComboBox combo = sender as ComboBox;
            if (combo == null || ParticulateAnalysis == null)
            {
                return;
            }
            ScatterSeries dataSeries = ParticulateAnalysis.Series[0] as ScatterSeries;
            FunctionSeries regressionSeries = ParticulateAnalysis.Series[1] as FunctionSeries;
            if (dataSeries == null || regressionSeries == null)
            {
                return;
            }

            // Get the active DataPoints (this assumes the default template for
            // ScatterSeries)
            Canvas plotArea = VisualTreeHelper.GetChild(dataSeries, 0) as Canvas;
            if (plotArea == null)
            {
                return;
            }
            List<DataPoint> activePoints =
                plotArea
                .Children
                .OfType<DataPoint>()
                .ToList();

            // The dimensions were added linearly to the ComboBox
            int dimension = combo.SelectedIndex + 1;

            // Initialize a simple least squares analysis
            int i = 0;
            int j = 0;
            int k = 0;
            double[] y = new double[activePoints.Count];
            double[,] x = new double[activePoints.Count, dimension + 1];
            for (i = 0; i < activePoints.Count; i++)
            {
                DataPoint point = activePoints[i];
                double independentValue = Convert.ToDouble(point.IndependentValue, CultureInfo.InvariantCulture);

                for (j = 0; j <= dimension; j++)
                {
                    x[i, j] = Math.Pow(independentValue, j);
                }

                y[i] = Convert.ToDouble(point.DependentValue, CultureInfo.InvariantCulture);
            }

            // Create the equations
            double[][] matrix = new double[dimension + 1][];
            for (i = 0; i <= dimension; i++)
            {
                // Create the row
                matrix[i] = new double[dimension + 2];

                // indeterminate coefficients
                for (j = 0; j <= dimension; j++)
                {
                    matrix[i][j] = 0.0;
                    for (k = 0; k < activePoints.Count; k++)
                    {
                        matrix[i][j] += x[k, i] * x[k, j];
                    }
                }

                // determinate values
                for (k = 0; k < activePoints.Count; k++)
                {
                    matrix[i][dimension + 1] += x[k, i] * y[k];
                }
            }

            // Convert to row-echelon form
            i = 0;
            j = 0;
            while (i <= dimension && j <= dimension)
            {
                // Get the pivot in column j starting at row i
                int pivotRow = i;
                for (k = i; k <= dimension; k++)
                {
                    if (Math.Abs(matrix[k][j]) > Math.Abs(matrix[pivotRow][j]))
                    {
                        pivotRow = k;
                    }
                }
                double pivot = matrix[pivotRow][j];

                // If we have a pivot element
                if (pivot != 0)
                {
                    // Swap the current row with the pivot row
                    double[] temp = matrix[i];
                    matrix[i] = matrix[pivotRow];
                    matrix[pivotRow] = temp;
                    pivotRow = i;

                    // Normalize the pivot row to the pivot
                    double c = matrix[i][j];
                    for (k = 0; k <= dimension + 1; k++)
                    {
                        matrix[i][k] /= c;
                    }

                    // Clear out the pivot position from the remaining rows
                    for (k = i + 1; k <= dimension; k++)
                    {
                        c = matrix[k][j];
                        for (int m = i; m <= dimension + 1; m++)
                        {
                            matrix[k][m] -= c * matrix[i][m];
                        }
                    }

                    i++;
                }

                j++;
            }

            // Solve using substitution
            for (i = dimension - 1; i >= 0; i--)
            {
                for (j = dimension; j > i; j--)
                {
                    matrix[i][dimension + 1] -= matrix[i][j] * matrix[j][dimension + 1];
                    matrix[i][j] = 0;
                }
            }

            // Capture the coefficients
            double a0 = matrix[0][dimension + 1];
            double a1 = matrix[1][dimension + 1];
            double a2 = (dimension >= 2) ? matrix[2][dimension + 1] : double.NaN;
            double a3 = (dimension >= 3) ? matrix[3][dimension + 1] : double.NaN;
            double a4 = (dimension == 4) ? matrix[4][dimension + 1] : double.NaN;

            // Create the function
            Func<double, double> function = null;
            switch (dimension)
            {
                case 1:
                    function = z => a1 * z + a0;
                    break;
                case 2:
                    function = z => a2 * z * z + a1 * z + a0;
                    break;
                case 3:
                    function = z => a3 * z * z * z + a2 * z * z + a1 * z + a0;
                    break;
                case 4:
                    function = z => a4 * z * z * z * z + a3 * z * z * z + a2 * z * z + a1 * z + a0;
                    break;
            }

            // Create the title
            StackPanel title = new StackPanel { Orientation = Orientation.Horizontal };
            title.Children.Add(
                new TextBlock
                {
                    Text = "f(x) = ",
                    Margin = new Thickness(0, 4, 0, 0)
                });

            title.Children.Add(
                new TextBlock
                {
                    Text = a0.ToString("N3", CultureInfo.InvariantCulture),
                    Margin = new Thickness(0, 4, 0, 0)
                });
            AddTitleTerm(title, a1, 1);
            if (dimension >= 2)
            {
                AddTitleTerm(title, a2, 2);
            }
            if (dimension >= 3)
            {
                AddTitleTerm(title, a3, 3);
            }
            if (dimension == 4)
            {
                AddTitleTerm(title, a4, 4);
            }

            // Set the function and the title
            regressionSeries.Function = function;
            regressionSeries.Title = title;
        }

        /// <summary>
        /// Add a term to the title.
        /// </summary>
        /// <param name="title">The title container.</param>
        /// <param name="value">The value of the term.</param>
        /// <param name="exponent">The exponent of the term.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by a method called by an event handler in XAML.")]
        private static void AddTitleTerm(StackPanel title, double value, int exponent)
        {
            if (value == 0)
            {
                return;
            }

            title.Children.Add(
                new TextBlock
                {
                    Text = value >= 0 ? " + " : " - ",
                    Margin = new Thickness(0, 4, 0, 0)
                });
            title.Children.Add(
                new TextBlock
                {
                    Text = string.Format(CultureInfo.InvariantCulture, "{0:N3}x", Math.Abs(value)),
                    Margin = new Thickness(0, 4, 0, 0)
                });

            if (exponent > 1)
            {
                title.Children.Add(
                    new TextBlock
                    {
                        Text = exponent.ToString(CultureInfo.InvariantCulture),
                        FontSize = 8
                    });
            }
        }
    }
}