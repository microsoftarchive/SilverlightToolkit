// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
#endif

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the Axis class.
    /// </summary>
    public abstract class AxisBase : ControlTest
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
        /// Initializes a new instance of the AxisTest class.
        /// </summary>
        protected AxisBase()
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
        /// Verify the Control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the Control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultControlToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(4, properties.Count);
            Assert.AreEqual(typeof(Line), properties["TickMarkStyle"]);
            Assert.AreEqual(typeof(TextBlock), properties["LabelStyle"]);
            Assert.AreEqual(typeof(Title), properties["TitleStyle"]);
            Assert.AreEqual(typeof(Line), properties["GridLineStyle"]);
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        [TestMethod]
        [Description("Creates a new instance.")]
        public void NewInstance()
        {
            IAxis axis = (IAxis) DefaultControlToTest;
            Assert.IsNotNull(axis);
        }

        /// <summary>
        /// Changes the value of the orientation property.
        /// </summary>
        [TestMethod]
        [Description("Changes the value of the AxisOrientation property.")]
        public void OrientationChangeValue()
        {
            IAxis axis = (IAxis) DefaultControlToTest;
            AxisOrientation orientation = AxisOrientation.X;
            axis.Orientation = orientation;
            Assert.AreEqual(orientation, axis.Orientation);
        }

        /// <summary>
        /// Adds a new point to a Category Axis, causing the Axis to update.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Adds a new point to a Category Axis, causing the Axis to update.")]
        [Bug("527206, Axis exception when adding new category", Fixed = true)]
        public void AddNewPointToCategoryAxis()
        {
            Chart chart = new Chart();
            DataPointSeries series = new ColumnSeries();
            series.IndependentValueBinding = new Binding();
            ObservableCollection<int> itemsSource = new ObservableCollection<int>();
            itemsSource.Add(5);
            series.ItemsSource = itemsSource;
            TestAsync(
                chart,
                () => chart.Series.Add(series),
                () => itemsSource.Add(10));
        }
    }
}