// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Controls.DataVisualization.Charting;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Contains methods that generate sample Charts for any Series type.
    /// </summary>
    public static class SampleGenerators
    {
        /// <summary>
        /// Stores a shared ObservableCollection for use by the dynamic collection scenario.
        /// </summary>
        private static ObservableCollection<int> _dynamicCollectionItemsSource = new ObservableCollection<int>();

        /// <summary>
        /// Stores a shared List for use by the dynamic data items scenario.
        /// </summary>
        private static List<Pet> _dynamicDataItemsSource = new List<Pet>
        {
            new Pet { Species = "Dogs" },
            new Pet { Species = "Cats" },
            new Pet { Species = "Birds" },
            new Pet { Species = "Fish" },
        };

        /// <summary>
        /// Stores a shared List for use by the dynamic date items scenario.
        /// </summary>
        private static List<Pair> _dynamicDateItemsSource = new List<Pair>
        {
            new Pair { First = new DateTime(2008, 10, 11), Second = 0.0 },
            new Pair { First = new DateTime(2008, 10, 12), Second = 0.0 },
            new Pair { First = new DateTime(2008, 10, 13), Second = 0.0 },
            new Pair { First = new DateTime(2008, 10, 14), Second = 0.0 },
            new Pair { First = new DateTime(2008, 10, 15), Second = 0.0 },
            new Pair { First = new DateTime(2008, 10, 16), Second = 0.0 },
        };

        /// <summary>
        /// Stores a shared random number generator.
        /// </summary>
        private static Random _random = new Random();

        /// <summary>
        /// Initializes static members of the SampleGenerators class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Need to do additional initialization.")]
        static SampleGenerators()
        {
            // Create a timer to update the dynamic data regularly
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(2);
            dispatcherTimer.Tick += delegate
            {
                // Update _dynamicCollectionItemsSource
                _dynamicCollectionItemsSource.Add(_random.Next(1, 11));
                if (10 < _dynamicCollectionItemsSource.Count)
                {
                    _dynamicCollectionItemsSource.RemoveAt(0);
                }

                // Update _dynamicDataItemsSource
                foreach (Pet pet in _dynamicDataItemsSource)
                {
                    pet.Count = _random.Next(1, 20);
                }

                // Update _dynamicDateItemsSource
                foreach (Pair pair in _dynamicDateItemsSource)
                {
                    pair.Second = _random.NextDouble() * 20;
                }
            };
            dispatcherTimer.Start();
        }

        /// <summary>
        /// Generates numeric Series samples.
        /// </summary>
        /// <param name="panel">Panel to add the generated Charts to.</param>
        /// <param name="seriesConstructor">Function that returns a Series instance for each sample.</param>
        /// <param name="includeIndependentValueBinding">True if an IndependentValueBinding should be created.</param>
        public static void GenerateNumericSeriesSamples(Panel panel, Func<Series> seriesConstructor, bool includeIndependentValueBinding)
        {
            Binding independentValueBinding = includeIndependentValueBinding ? new Binding() : null;
            Scenario[] scenarios = new Scenario[]
            {
                new Scenario { Title = "One Int", ItemsSource = new int[] { 3 }, IndependentValueBinding = independentValueBinding },
                new Scenario { Title = "Three Ints", ItemsSource = new int[] { 4, 2, 7 }, IndependentValueBinding = independentValueBinding },
                new Scenario { Title = "Seven Doubles", ItemsSource = new double[] { 4.4, 6.2, 9.7, 7.0, 2.1, 8.3, 5.5 }, IndependentValueBinding = independentValueBinding },
                new Scenario { Title = "No Points", ItemsSource = new int[0], IndependentValueBinding = independentValueBinding },
                new Scenario { Title = "Some Points with Value 0", ItemsSource = new int[] { 0, 1, 0, 2, 0 }, IndependentValueBinding = independentValueBinding },
                new Scenario { Title = "Some Negative-Value Points", ItemsSource = new double[] { 2.1, 3.7, -2.5, -4.6, 1.0 }, IndependentValueBinding = independentValueBinding },
                new Scenario { Title = "High-Value Points", ItemsSource = new int[] { 2000000000, 1200000000, 2100000000, 1000000000, 2000000000 }, IndependentValueBinding = independentValueBinding },
                // new Scenario { Title = "100 Points", ItemsSource = Enumerable.Range(1, 100), IndependentValueBinding = independentValueBinding },
                new Scenario { Title = "Dynamic Collection", ItemsSource = _dynamicCollectionItemsSource, IndependentValueBinding = independentValueBinding },
            };
            GenerateSeriesSamples(panel, seriesConstructor, scenarios, null);
        }

        /// <summary>
        /// Generates value Series samples.
        /// </summary>
        /// <param name="panel">Panel to add the generated Charts to.</param>
        /// <param name="seriesConstructor">Function that returns a Series instance for each sample.</param>
        public static void GenerateValueSeriesSamples(Panel panel, Func<Series> seriesConstructor)
        {
            Scenario[] scenarios = new Scenario[]
            {
                new Scenario { Title = "Pet Counts (No Names)", ItemsSource = pets, DependentValueBinding = new Binding("Count") },
                new Scenario { Title = "Dynamic Data Items (No Names)", ItemsSource = _dynamicDataItemsSource, DependentValueBinding = new Binding("Count") },
            };
            GenerateSeriesSamples(panel, seriesConstructor, scenarios, null);
        }

        /// <summary>
        /// Generates category/value Series samples.
        /// </summary>
        /// <param name="panel">Panel to add the generated Charts to.</param>
        /// <param name="seriesConstructor">Function that returns a Series instance for each sample.</param>
        public static void GenerateCategoryValueSeriesSamples(Panel panel, Func<Series> seriesConstructor)
        {
            Scenario[] scenarios = new Scenario[]
            {
                new Scenario { Title = "Pet Counts (Names)", ItemsSource = pets, DependentValueBinding = new Binding("Count"), IndependentValueBinding = new Binding("Species") },
                new Scenario { Title = "Dynamic Data Items (Names)", ItemsSource = _dynamicDataItemsSource, DependentValueBinding = new Binding("Count"), IndependentValueBinding = new Binding("Species") },
            };
            GenerateSeriesSamples(panel, seriesConstructor, scenarios, null);
        }

        /// <summary>
        /// Generates value/value Series samples.
        /// </summary>
        /// <param name="panel">Panel to add the generated Charts to.</param>
        /// <param name="seriesConstructor">Function that returns a Series instance for each sample.</param>
        public static void GenerateValueValueSeriesSamples(Panel panel, Func<Series> seriesConstructor)
        {
            List<Point> circle = new List<Point>();
            for (double i = 0; i < 2 * Math.PI; i += 0.1)
            {
                circle.Add(new Point(Math.Sin(i), Math.Cos(i)));
            }
            Scenario[] scenarios = new Scenario[]
            {
                new Scenario { Title = "Circle", ItemsSource = circle, DependentValueBinding = new Binding("X"), IndependentValueBinding = new Binding("Y") },
            };
            GenerateSeriesSamples(panel, seriesConstructor, scenarios, null);
        }

        /// <summary>
        /// Generates value/value Series samples.
        /// </summary>
        /// <param name="panel">Panel to add the generated Charts to.</param>
        /// <param name="seriesConstructor">Function that returns a Series instance for each sample.</param>
        public static void GenerateDateTimeValueSeriesSamples(Panel panel, Func<Series> seriesConstructor)
        {
            Scenario[] scenarios = new Scenario[]
            {
                new Scenario { Title = "Value by Date", ItemsSource = _dynamicDateItemsSource, DependentValueBinding = new Binding("Second"), IndependentValueBinding = new Binding("First") },
            };
            Action<Chart> chartModifier = (chart) =>
            {
                IAxis dateAxis = new DateTimeAxis { Orientation = AxisOrientation.X };
                chart.Axes.Add(dateAxis);
                IAxis valueAxis = new LinearAxis { Orientation = AxisOrientation.Y, Minimum = 0, Maximum = 20, ShowGridLines = true };
                chart.Axes.Add(valueAxis);
            };
            GenerateSeriesSamples(panel, seriesConstructor, scenarios, chartModifier);
        }

        /// <summary>
        /// Generates multiple value Series samples.
        /// </summary>
        /// <param name="panel">Panel to add the generated Charts to.</param>
        /// <param name="seriesConstructor">Function that returns a Series instance for each sample.</param>
        /// <param name="includeIndependentValueBinding">True if an IndependentValueBinding should be created.</param>
        public static void GenerateMultipleValueSeriesSamples(Panel panel, Func<Series> seriesConstructor, bool includeIndependentValueBinding)
        {
            Binding independentValueBinding = includeIndependentValueBinding ? new Binding() : null;
            double[] items = new double[] { 3.1, 1.6, 4.9, 0.8, 2.2 };
            List<IEnumerable> itemsRepeated = new List<IEnumerable>();
            for (int i = 0; i < 30; i++)
            {
                itemsRepeated.Add(items);
            }
            Scenario[] scenarios = new Scenario[]
            {
                new Scenario { Title = "Three Series", ItemsSources = new IEnumerable[] { new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 }, new int[] { 7, 8, 9 } }, IndependentValueBinding = independentValueBinding },
                // new Scenario { Title = "Thirty Series", ItemsSources = itemsRepeated, IndependentValueBinding = independentValueBinding },
            };
            GenerateSeriesSamples(panel, seriesConstructor, scenarios, null);
        }

        /// <summary>
        /// Generates various Series samples.
        /// </summary>
        /// <param name="panel">Panel to add the generated Charts to.</param>
        /// <param name="seriesConstructor">Function that returns a Series instance for each sample.</param>
        /// <param name="scenarios">Collection of scenarios to generate.</param>
        /// <param name="chartModifier">Function that applies any necessary modifications to the Chart.</param>
        private static void GenerateSeriesSamples(Panel panel, Func<Series> seriesConstructor, IEnumerable<Scenario> scenarios, Action<Chart> chartModifier)
        {
            Style wrapperStyle = Application.Current.Resources["WrapperStyle"] as Style;

            // For each scenario...
            foreach (Scenario scenario in scenarios)
            {
                // Create the sample Chart
                Chart chart = new Chart { Title = scenario.Title, MaxWidth = 500, MaxHeight = 270 };
                foreach (IEnumerable itemsSource in scenario.ItemsSources)
                {
                    DataPointSeries series = seriesConstructor() as DataPointSeries;
                    series.ItemsSource = itemsSource;
                    series.DependentValueBinding = scenario.DependentValueBinding;
                    series.IndependentValueBinding = scenario.IndependentValueBinding;
                    chart.Series.Add(series);
                }
                if (null != chartModifier)
                {
                    chartModifier(chart);
                }

                // Wrap the Chart in a suitably formatted Grid
                Grid grid = new Grid { Style = wrapperStyle };
                grid.Children.Add(chart);
                panel.Children.Add(grid);
            }
        }

        /// <summary>
        /// Collection of Pet objects for use by Chart samples.
        /// </summary>
        private static Pet[] pets = new Pet[]
        {
            new Pet { Species = "Dogs", Count = 5 },
            new Pet { Species = "Cats", Count = 4 },
            new Pet { Species = "Birds", Count = 6 },
            new Pet { Species = "Fish", Count = 9 }
        };

        /// <summary>
        /// Class representing an automatically generated Chart sample.
        /// </summary>
        private class Scenario
        {
            /// <summary>
            /// Gets or sets the title of the scenario.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Sets the ItemsSource for the Chart's Series.
            /// </summary>
            public IEnumerable ItemsSource
            {
                set
                {
                    ItemsSources = new IEnumerable[] { value };
                }
            }

            /// <summary>
            /// Gets or sets the ItemsSources for the Chart's Series.
            /// </summary>
            public IEnumerable<IEnumerable> ItemsSources { get; set; }

            /// <summary>
            /// Gets or sets the DependentValueBinding for the Chart's Series.
            /// </summary>
            public Binding DependentValueBinding { get; set; }

            /// <summary>
            /// Gets or sets the IndependentValueBinding for the Chart's Series.
            /// </summary>
            public Binding IndependentValueBinding { get; set; }
        }
    }
}