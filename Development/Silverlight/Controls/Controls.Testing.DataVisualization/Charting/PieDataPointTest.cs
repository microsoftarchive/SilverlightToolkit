// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.Controls.DataVisualization.Charting;
using System.Windows;

namespace Microsoft.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the PieDataPointTest class.
    /// </summary>
    [TestClass]
    public partial class PieDataPointTest : DataPointBase
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return new PieDataPoint(); }
        }

        /// <summary>
        /// Initializes a new instance of the PieDataPointTest class.
        /// </summary>
        public PieDataPointTest()
        {
            BorderBrushProperty.DefaultValue = new SolidColorBrush(Colors.White);
        }

        /// <summary>
        /// Verifies the Control's TemplateParts.
        /// </summary>
        [TestMethod]
        [Description("Verifies the Control's TemplateParts.")]
        public override void TemplatePartsAreDefined()
        {
            IDictionary<string, Type> templateParts = DefaultControlToTest.GetType().GetTemplateParts();
            Assert.AreEqual(1, templateParts.Count);
            Assert.AreSame(typeof(UIElement), templateParts["Slice"]);
        }

        /// <summary>
        /// Verifies the initial values of all properties.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies the initial values of all properties.")]
        public override void InitialValues()
        {
            base.InitialValues();
            PieDataPoint dataPoint = DefaultControlToTest as PieDataPoint;
            Assert.AreEqual(0.0, dataPoint.Ratio);
            Assert.AreEqual(0.0, dataPoint.ActualRatio);
            Assert.AreEqual(0.0, dataPoint.OffsetRatio);
            Assert.AreEqual(0.0, dataPoint.ActualOffsetRatio);
            Assert.IsNull(dataPoint.Geometry);
            Assert.IsNull(dataPoint.GeometryHighlight);
            Assert.IsNull(dataPoint.GeometrySelection);
            TestAsync(
                dataPoint,
                () => Assert.AreEqual("{0:p2}", dataPoint.RatioStringFormat),
                () => Assert.AreEqual("0.00 %", dataPoint.FormattedRatio));
        }

        /// <summary>
        /// Assigns a double to the Ratio property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a double to the Ratio property.")]
        public void RatioChangeDouble()
        {
            PieDataPoint dataPoint = DefaultControlToTest as PieDataPoint;
            double ratio = 1.2;
            dataPoint.Ratio = ratio;
            Assert.AreEqual(ratio, dataPoint.Ratio);
        }

        /// <summary>
        /// Changes the RatioStringFormat property.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Changes the RatioStringFormat property.")]
        public void RatioStringFormatChange()
        {
            PieDataPoint dataPoint = DefaultControlToTest as PieDataPoint;
            double ratio = 0.12345;
            TestAsync(
                dataPoint,
                () => dataPoint.Ratio = ratio,
                () => Assert.AreEqual(ratio, dataPoint.Ratio),
                () => Assert.AreEqual("12.35 %", dataPoint.FormattedRatio),
                () => dataPoint.RatioStringFormat = "{0}",
                () => Assert.AreEqual("0.12345", dataPoint.FormattedRatio));
        }

        /// <summary>
        /// Assigns a double to the ActualRatio property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a double to the ActualRatio property.")]
        public void ActualRatioChangeDouble()
        {
            PieDataPoint dataPoint = DefaultControlToTest as PieDataPoint;
            double actualRatio = 1.2;
            dataPoint.ActualRatio = actualRatio;
            Assert.AreEqual(actualRatio, dataPoint.ActualRatio);
        }

        /// <summary>
        /// Assigns a double to the OffsetRatio property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a double to the OffsetRatio property.")]
        public void OffsetRatioChangeDouble()
        {
            PieDataPoint dataPoint = DefaultControlToTest as PieDataPoint;
            double offsetRatio = 1.2;
            dataPoint.OffsetRatio = offsetRatio;
            Assert.AreEqual(offsetRatio, dataPoint.OffsetRatio);
        }

        /// <summary>
        /// Assigns a double to the ActualOffsetRatio property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a double to the ActualOffsetRatio property.")]
        public void ActualOffsetRatioChangeDouble()
        {
            PieDataPoint dataPoint = DefaultControlToTest as PieDataPoint;
            double actualOffsetRatio = 1.2;
            dataPoint.ActualOffsetRatio = actualOffsetRatio;
            Assert.AreEqual(actualOffsetRatio, dataPoint.ActualOffsetRatio);
        }
    }
}