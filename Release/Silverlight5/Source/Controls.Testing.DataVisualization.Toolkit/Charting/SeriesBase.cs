// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.DataVisualization.Charting;
#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
#endif

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for children of the Series class.
    /// </summary>
    public abstract partial class SeriesBase : ControlTest
    {
        /// <summary>
        /// Gets instances of Control (or derived types) to test.
        /// </summary>
        public override IEnumerable<Control> ControlsToTest
        {
            get { yield return DefaultControlToTest; }
        }

        /// <summary>
        /// Gets instances of IOverriddenControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenControl> OverriddenControlsToTest
        {
            get { yield break; }
        }

        /// <summary>
        /// Gets a default instance of DynamicSeries (or a derived type) to 
        /// test.
        /// </summary>
        public DataPointSeries DefaultSeriesToTest
        {
            get
            {
                return DefaultControlToTest as DataPointSeries;
            }
        }

        /// <summary>
        /// Initializes a new instance of the SeriesBase class.
        /// </summary>
        protected SeriesBase()
        {
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // Get the base Control dependency property tests
            return TagInherited(base.GetDependencyPropertyTests());
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        [TestMethod]
        [Description("Creates a new instance.")]
        public void NewInstance()
        {
            DataPointSeries series = DefaultSeriesToTest;
            Assert.IsNotNull(series);
        }

        /// <summary>
        /// Assigns a string to the Title property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a string to the Title property.")]
        public void TitleChangeString()
        {
            DataPointSeries series = DefaultSeriesToTest;
            string title = "String Title";
            series.Title = title;
            Assert.AreSame(title, series.Title);
        }

        /// <summary>
        /// Assigns an object to the Title property.
        /// </summary>
        [TestMethod]
        [Description("Assigns an object to the Title property.")]
        public void TitleChangeObject()
        {
            DataPointSeries series = DefaultSeriesToTest;
            object title = new object();
            series.Title = title;
            Assert.AreSame(title, series.Title);
        }

        /// <summary>
        /// Assigns a Button to the Title property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a Button to the Title property.")]
        public void TitleChangeButton()
        {
            DataPointSeries series = DefaultSeriesToTest;
            Button title = new Button { Content = "Button Title" };
            series.Title = title;
            Assert.AreSame(title, series.Title);
        }

        /// <summary>
        /// Assigns an empty collection to the ItemsSource property.
        /// </summary>
        [TestMethod]
        [Description("Assigns an empty collection to the ItemsSource property.")]
        public void ItemsSourceChangeEmpty()
        {
            DataPointSeries series = DefaultSeriesToTest;
            ObjectCollection objectCollection = new ObjectCollection();
            series.ItemsSource = objectCollection;
            Assert.AreSame(objectCollection, series.ItemsSource);
        }

        /// <summary>
        /// Assigns a collection of objects to the ItemsSource property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a collection of objects to the ItemsSource property.")]
        public void ItemsSourceChangeObjects()
        {
            DataPointSeries series = DefaultSeriesToTest;
            ObjectCollection objectCollection = new ObjectCollection();
            objectCollection.Add(new object());
            objectCollection.Add(new object());
            series.ItemsSource = objectCollection;
            Assert.AreSame(objectCollection, series.ItemsSource);
        }

        /// <summary>
        /// Changes the contents of the ItemsSource collection.
        /// </summary>
        [TestMethod]
        [Description("Changes the contents of the ItemsSource collection.")]
        [Priority(0)]
        public void ItemsSourceContentChange()
        {
            DataPointSeries series = DefaultSeriesToTest;
            ObservableCollection<int> observableCollection = new ObservableCollection<int>();
            observableCollection.Add(3);
            series.ItemsSource = observableCollection;
            Assert.AreSame(observableCollection, series.ItemsSource);
            observableCollection.Add(5);
            Assert.AreSame(observableCollection, series.ItemsSource);
            observableCollection.RemoveAt(0);
            Assert.AreSame(observableCollection, series.ItemsSource);
        }

        /// <summary>
        /// Assigns a collection of custom data objects to the ItemsSource property.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Assigns a collection of custom data objects to the ItemsSource property.")]
        public void ItemsSourceWithCustomObjects()
        {
            Chart chart = new Chart();
            DataPointSeries series = DefaultSeriesToTest;
            ObservableCollection<DataObject<int>> itemsSource = new ObservableCollection<DataObject<int>>();
            NotifyingDataObject<int> notifyingDataObject = new NotifyingDataObject<int> { Value = 5 };
            itemsSource.Add(notifyingDataObject);
            itemsSource.Add(new DataObject<int> { Value = 3 });
            series.ItemsSource = itemsSource;
            series.DependentValueBinding = new Binding("Value");
            series.IndependentValueBinding = new Binding("Value");
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => notifyingDataObject.Value++,
                () => itemsSource.RemoveAt(0));
        }

        /// <summary>
        /// Swaps the ItemsSource collection for a different one.
        /// </summary>
        [TestMethod]
        [Description("Swaps the ItemsSource collection for a different one.")]
        public void ItemsSourceSwap()
        {
            DataPointSeries series = DefaultSeriesToTest;
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();
            series.ItemsSource = observableCollection;
            Assert.AreSame(observableCollection, series.ItemsSource);
            observableCollection = new ObservableCollection<object>();
            series.ItemsSource = observableCollection;
            Assert.AreSame(observableCollection, series.ItemsSource);
        }

        /// <summary>
        /// Changes the Template for the Series.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Changes the Template for the Series.")]
        public void ChangeSeriesTemplate()
        {
            DataPointSeries series = DefaultSeriesToTest;
            series.ItemsSource = new double[] { 1.1, 2.2 };
            TestAsync(
                series,
                () => series.ApplyTemplate(),
                () =>
                {
                    ControlTemplate template = series.Template;
                    series.Template = null;
                    series.Template = template;
                },
                () => series.ApplyTemplate());
        }

        /// <summary>
        /// Changes the TransitionDuration property.
        /// </summary>
        [TestMethod]
        [Description("Changes the TransitionDuration property.")]
        public void TransitionDurationChange()
        {
            DataPointSeries series = DefaultSeriesToTest;
            TimeSpan duration = TimeSpan.FromSeconds(2);
            series.TransitionDuration = duration;
            Assert.AreEqual(duration, series.TransitionDuration);
        }

        /// <summary>
        /// Sets the SelectedItem property to a valid item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Sets the SelectedItem property to a valid item.")]
        public void SetSelectedItemValid()
        {
            Chart chart = new Chart();
            DataPointSeries series = DefaultSeriesToTest;
            series.IndependentValueBinding = new Binding();
            int[] itemsSource = new int[] { 1 };
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => series.ItemsSource = itemsSource,
                () => series.SelectedItem = 1,
                () => Assert.AreEqual(1, series.SelectedItem));
        }

        /// <summary>
        /// Sets the SelectedItem property to an invalid item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Sets the SelectedItem property to an invalid item.")]
        public void SetSelectedItemInvalid()
        {
            Chart chart = new Chart();
            DataPointSeries series = DefaultSeriesToTest;
            series.IndependentValueBinding = new Binding();
            int[] itemsSource = new int[] { 1 };
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => series.ItemsSource = itemsSource,
                () => series.SelectedItem = 2,
                () => Assert.IsNull(series.SelectedItem));
        }

        /// <summary>
        /// Clears the SelectedItem property.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Clears the SelectedItem property.")]
        public void ClearSelectedItem()
        {
            Chart chart = new Chart();
            DataPointSeries series = DefaultSeriesToTest;
            series.IndependentValueBinding = new Binding();
            int[] itemsSource = new int[] { 1 };
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => series.ItemsSource = itemsSource,
                () => series.SelectedItem = 1,
                () => Assert.AreEqual(1, series.SelectedItem),
                () => series.SelectedItem = null,
                () => Assert.IsNull(series.SelectedItem));
        }
    }
}
