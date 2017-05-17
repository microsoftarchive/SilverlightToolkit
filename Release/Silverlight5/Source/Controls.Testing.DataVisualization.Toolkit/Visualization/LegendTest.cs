// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
#endif

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the Legend class.
    /// </summary>
    [TestClass]
    public partial class LegendTest : ControlTest
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return new Legend(); }
        }

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
        /// Initializes a new instance of the LegendTest class.
        /// </summary>
        public LegendTest()
        {
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
        /// Verify the Control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the Control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultControlToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(2, properties.Count);
            Assert.AreEqual(typeof(Title), properties["TitleStyle"]);
            Assert.AreEqual(typeof(ContentPresenter), properties["ItemContainerStyle"]);
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        [TestMethod]
        [Description("Creates a new instance.")]
        public void NewInstance()
        {
            Legend legend = new Legend();
            Assert.IsNotNull(legend);
        }

        /// <summary>
        /// Verifies the initial values of all properties.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies the initial values of all properties.")]
        public void InitialValues()
        {
            Legend legend = new Legend();
            TestAsync(
                legend,
                () => Assert.IsNotNull(legend.Items),
                () => Assert.IsNull(legend.Header),
                () => Assert.IsNotNull(legend.TitleStyle));
        }

        /// <summary>
        /// Assigns a string to the Title property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a string to the Title property.")]
        public void TitleChangeString()
        {
            Legend legend = new Legend();
            string title = "String Title";
            legend.Header = title;
            Assert.AreSame(title, legend.Header);
        }

        /// <summary>
        /// Assigns an object to the Title property.
        /// </summary>
        [TestMethod]
        [Description("Assigns an object to the Title property.")]
        public void TitleChangeObject()
        {
            Legend legend = new Legend();
            object title = new object();
            legend.Header = title;
            Assert.AreSame(title, legend.Header);
        }

        /// <summary>
        /// Assigns a Button to the Title property.
        /// </summary>
        [TestMethod]
        [Description("Assigns a Button to the Title property.")]
        public void TitleChangeButton()
        {
            Legend legend = new Legend();
            Button title = new Button { Content = "Button Title" };
            legend.Header = title;
            Assert.AreSame(title, legend.Header);
        }

        /// <summary>
        /// Changes the TitleStyle property.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Changes the TitleStyle property.")]
        public void TitleStyleChange()
        {
            Legend legend = new Legend();
            // Change Legend's Template because Silverlight only allows setting Style properties once.
            legend.Template = new ControlTemplate();
            Style style = new Style(typeof(Title));
            TestAsync(
                legend,
                () => legend.TitleStyle = style,
                () => Assert.AreSame(style, legend.TitleStyle));
        }

        /// <summary>
        /// Completely tests the Legend's Automatic visibility-switching code.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Completely tests the Legend's Automatic visibility-switching code.")]
        public void AutomaticVisibilitySwitching()
        {
            Button button0 = new Button();
            Button button1 = new Button();
            Legend legend = new Legend();
            TestAsync(
                legend,
                // Initial Visible state helps designers
                () => Assert.AreEqual(Visibility.Visible, legend.Visibility),
                () => legend.Header = "Title",
                () => Assert.AreEqual(Visibility.Visible, legend.Visibility),
                () => legend.Header = null,
                () => Assert.AreEqual(Visibility.Collapsed, legend.Visibility),
                () => legend.Items.Clear(),
                () => Assert.AreEqual(Visibility.Collapsed, legend.Visibility),
                () => legend.Items.Add(button0),
                () => Assert.AreEqual(Visibility.Visible, legend.Visibility),
                () => legend.Items.Add(button1),
                () => Assert.AreEqual(Visibility.Visible, legend.Visibility),
                () => legend.Items.RemoveAt(0),
                () => Assert.AreEqual(Visibility.Visible, legend.Visibility),
                () => legend.Items.RemoveAt(0),
                () => Assert.AreEqual(Visibility.Collapsed, legend.Visibility),
                () => legend.Items.Add(button0),
                () => Assert.AreEqual(Visibility.Visible, legend.Visibility),
                () => legend.Items.Clear(),
                () => Assert.AreEqual(Visibility.Collapsed, legend.Visibility),
                () => legend.Items.Add(button0),
                () => Assert.AreEqual(Visibility.Visible, legend.Visibility));
        }

        /// <summary>
        /// Verifies the Control's TemplateParts.
        /// </summary>
        [TestMethod]
        [Description("Verifies the Control's TemplateParts.")]
        public override void TemplatePartsAreDefined()
        {
            IDictionary<string, Type> templateParts = DefaultControlToTest.GetType().GetTemplateParts();
            Assert.AreEqual(0, templateParts.Count);
        }
    }
}