// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.DataVisualization.Charting;
#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
#endif

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the Control class.
    /// </summary>
    public abstract partial class DataPointBase : ControlTest
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
        /// Initializes a new instance of the DataPointBase class.
        /// </summary>
        protected DataPointBase()
        {
            BackgroundProperty.DefaultValue = new SolidColorBrush(Colors.Orange);
            BorderBrushProperty.DefaultValue = new SolidColorBrush(Colors.Black);
            BorderThicknessProperty.DefaultValue = new Thickness(1);
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
        /// Verify the Control Template's visual states.
        /// </summary>
        [TestMethod]
        [Description("Verify the Control Template's visual states.")]
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> visualStates = DefaultControlToTest.GetType().GetVisualStates();
            Assert.AreEqual(6, visualStates.Count);

            Assert.AreEqual<string>("CommonStates", visualStates["Normal"]);
            Assert.AreEqual<string>("CommonStates", visualStates["MouseOver"]);
            Assert.AreEqual<string>("SelectionStates", visualStates["Unselected"]);
            Assert.AreEqual<string>("SelectionStates", visualStates["Selected"]);
            Assert.AreEqual<string>("RevealStates", visualStates["Shown"]);
            Assert.AreEqual<string>("RevealStates", visualStates["Hidden"]);
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        [TestMethod]
        [Description("Creates a new instance.")]
        public void NewInstance()
        {
            DataPoint dataPoint = DefaultControlToTest as DataPoint;
            Assert.IsNotNull(dataPoint);
        }

        /// <summary>
        /// Verifies the initial values of all properties.
        /// </summary>
        [TestMethod]
        [Description("Verifies the initial values of all properties.")]
        public virtual void InitialValues()
        {
            DataPoint dataPoint = DefaultControlToTest as DataPoint;
            Assert.IsNull(dataPoint.IndependentValue);
            Assert.IsNull(dataPoint.DependentValue);
            Assert.AreEqual(0.0, dataPoint.ActualDependentValue);
            Assert.AreEqual(null, dataPoint.ActualIndependentValue);
            Assert.IsNull(dataPoint.DependentValueStringFormat);
            Assert.IsNull(dataPoint.FormattedDependentValue);
            Assert.IsNull(dataPoint.IndependentValueStringFormat);
            Assert.IsNull(dataPoint.FormattedIndependentValue);
        }

        /// <summary>
        /// Assigns a string to the IndependentValue property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a string to the IndependentValue property.")]
        public void IndependentValueChangeString()
        {
            DataPoint dataPoint = DefaultControlToTest as DataPoint;
            string independentValue = "Independent Value";
            dataPoint.IndependentValue = independentValue;
            Assert.AreSame(independentValue, dataPoint.IndependentValue);
        }

        /// <summary>
        /// Assigns a double to the IndependentValue property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a double to the IndependentValue property.")]
        public void IndependentValueChangeDouble()
        {
            DataPoint dataPoint = DefaultControlToTest as DataPoint;
            double independentValue = 1.2;
            dataPoint.IndependentValue = independentValue;
            Assert.AreEqual(independentValue, dataPoint.IndependentValue);
        }

        /// <summary>
        /// Changes the IndependentValueStringFormat property.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Changes the IndependentValueStringFormat property.")]
        public void IndependentValueStringFormatChange()
        {
            DataPoint dataPoint = DefaultControlToTest as DataPoint;
            double value = 0.12345;
            TestAsync(
                dataPoint,
                () => dataPoint.IndependentValue = value,
                () => Assert.AreEqual(value, dataPoint.IndependentValue),
                () => Assert.AreEqual(value.ToString(CultureInfo.CurrentCulture), dataPoint.FormattedIndependentValue),
                () => dataPoint.IndependentValueStringFormat = "{0:0.##}",
                () => Assert.AreEqual(value.ToString("0.##", CultureInfo.CurrentCulture), dataPoint.FormattedIndependentValue));
        }

        /// <summary>
        /// Assigns a double to the ActualIndependentValue property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a double to the ActualIndependentValue property.")]
        public void ActualIndependentValueChangeDouble()
        {
            DataPoint dataPoint = DefaultControlToTest as DataPoint;
            double actualIndependentValue = 1.2;
            dataPoint.ActualIndependentValue = actualIndependentValue;
            Assert.AreEqual(actualIndependentValue, dataPoint.ActualIndependentValue);
        }

        /// <summary>
        /// Assigns a double to the DependentValue property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a double to the DependentValue property.")]
        public void DependentValueChangeDouble()
        {
            DataPoint dataPoint = DefaultControlToTest as DataPoint;
            double dependentValue = 1.2;
            dataPoint.DependentValue = dependentValue;
            Assert.AreEqual(dependentValue, dataPoint.DependentValue);
        }

        /// <summary>
        /// Changes the DependentValueStringFormat property.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Changes the DependentValueStringFormat property.")]
        public void DependentValueStringFormatChange()
        {
            DataPoint dataPoint = DefaultControlToTest as DataPoint;
            double value = 0.12345;
            TestAsync(
                dataPoint,
                () => dataPoint.DependentValue = value,
                () => Assert.AreEqual(value, dataPoint.DependentValue),
                () => Assert.AreEqual(value.ToString(CultureInfo.CurrentCulture), dataPoint.FormattedDependentValue),
                () => dataPoint.DependentValueStringFormat = "{0:0.##}",
                () => Assert.AreEqual(value.ToString("0.##", CultureInfo.CurrentCulture), dataPoint.FormattedDependentValue));
        }

        /// <summary>
        /// Assigns a double to the ActualDependentValue property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a double to the ActualDependentValue property.")]
        public void ActualDependentValueChangeDouble()
        {
            DataPoint dataPoint = DefaultControlToTest as DataPoint;
            double actualDependentValue = 1.2;
            dataPoint.ActualDependentValue = actualDependentValue;
            Assert.AreEqual(actualDependentValue, dataPoint.ActualDependentValue);
        }
    }
}