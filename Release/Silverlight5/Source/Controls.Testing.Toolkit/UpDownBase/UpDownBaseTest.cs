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
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// UpDownBase unit tests.
    /// </summary>
    public abstract class UpDownBaseTest : ControlTest
    {
        #region Controls to test
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return DefaultUpDownBaseToTest; }
        }

        /// <summary>
        /// Gets instances of Control (or derived types) to test.
        /// </summary>
        public override IEnumerable<Control> ControlsToTest
        {
            get { return UpDownBasesToTest.OfType<Control>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenControl> OverriddenControlsToTest
        {
            get { return OverriddenUpDownBasesToTest.OfType<IOverriddenControl>(); }
        }
        #endregion Controls to test

        #region UpDownBases to test
        /// <summary>
        /// Gets a default instance of UpDownBase (or a derived type) to test.
        /// </summary>
        public abstract UpDownBase DefaultUpDownBaseToTest { get; }

        /// <summary>
        /// Gets instances of UpDownBase (or derived types) to test.
        /// </summary>
        public abstract IEnumerable<UpDownBase> UpDownBasesToTest { get; }

        /// <summary>
        /// Gets instances of IOverriddenUpDownBase (or derived types) to test.
        /// </summary>
        public abstract IEnumerable<IOverriddenUpDownBase> OverriddenUpDownBasesToTest { get; }
        #endregion UpDownBases to test

        /// <summary>
        /// Gets the SpinnerStyle dependency property test.
        /// </summary>
        protected DependencyPropertyTest<UpDownBase, Style> SpinnerStyleProperty { get; private set; }

         /// <summary>
        /// Initializes a new instance of the UpDownBaseTest class.
        /// </summary>
        protected UpDownBaseTest()
            : base()
        {
            Func<UpDownBase> initializer = () => DefaultUpDownBaseToTest;
            SpinnerStyleProperty = new DependencyPropertyTest<UpDownBase, Style>(this, "SpinnerStyle")
            {
                Property = UpDownBase.SpinnerStyleProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new Style[] { new Style(typeof(ButtonSpinner)) }
            };
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // Get the base Control dependency property tests
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());

            // SpinnerStyleProperty tests
            tests.Add(SpinnerStyleProperty.CheckDefaultValueTest);
            tests.Add(SpinnerStyleProperty.ChangeClrSetterTest);
            tests.Add(SpinnerStyleProperty.ChangeSetValueTest);
            tests.Add(SpinnerStyleProperty.SetNullTest);
            tests.Add(SpinnerStyleProperty.ClearValueResetsDefaultTest);
            tests.Add(SpinnerStyleProperty.CanBeStyledTest);
            tests.Add(SpinnerStyleProperty.TemplateBindTest);
            tests.Add(SpinnerStyleProperty.SetXamlAttributeTest.Bug("TODO: XAML Parser?"));
            tests.Add(SpinnerStyleProperty.SetXamlElementTest.Bug("TODO: XAML Parser?"));

            return tests;
        }

        #region control contract
        /// <summary>
        /// Verifies the Control's TemplateParts.
        /// </summary>
        [TestMethod]
        [Description("Verifies the Control's TemplateParts.")]
        public override void TemplatePartsAreDefined()
        {
            IDictionary<string, Type> templateParts = DefaultControlToTest.GetType().GetTemplateParts();
            Assert.AreEqual(2, templateParts.Count);
            Assert.AreSame(typeof(TextBox), templateParts["Text"]);
            Assert.AreSame(typeof(Spinner), templateParts["Spinner"]);
        }

        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's template visual states.")]
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> visualStates = DefaultControlToTest.GetType().GetVisualStates();
            Assert.AreEqual(9, visualStates.Count);

            Assert.AreEqual<string>("CommonStates", visualStates["Normal"]);
            Assert.AreEqual<string>("CommonStates", visualStates["MouseOver"]);
            Assert.AreEqual<string>("CommonStates", visualStates["Pressed"]);
            Assert.AreEqual<string>("CommonStates", visualStates["Disabled"]);

            Assert.AreEqual<string>("FocusStates", visualStates["Focused"]);
            Assert.AreEqual<string>("FocusStates", visualStates["Unfocused"]);

            Assert.AreEqual<string>("ValidationStates", visualStates["Valid"], "Failed to find expected state Valid!");
            Assert.AreEqual<string>("ValidationStates", visualStates["InvalidFocused"], "Failed to find expected state InvalidFocused!");
            Assert.AreEqual<string>("ValidationStates", visualStates["InvalidUnfocused"], "Failed to find expected state InvalidUnfocused!");
        }

        /// <summary>
        /// Verify the control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultControlToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(1, properties.Count, "Incorrect number of style typed property attributes!");
            Assert.AreEqual(typeof(Spinner), properties["SpinnerStyle"], "Failed to find expected style type property SpinnerStyle!");
        }
        #endregion
    }
}