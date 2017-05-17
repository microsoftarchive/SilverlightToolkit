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
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// NumericUpDown unit tests.
    /// </summary>
    [TestClass]
    public partial class NumericUpDownTest : UpDownBaseTest<double>
    {
        #region UpDownBase<T>s to test
        /// <summary>
        /// Gets a default instance of UpDownBase&lt;T&gt; (or a derived type) to test.
        /// </summary>
        public override UpDownBase<double> DefaultUpDownBaseTToTest 
        {
            get { return DefaultNumericUpDownToTest; }
        }

        /// <summary>
        /// Gets instances of UpDownBase&lt;T&gt; (or derived types) to test.
        /// </summary>
        public override IEnumerable<UpDownBase<double>> UpDownBaseTsToTest
        {
            get { return NumericUpDownsToTest.OfType<UpDownBase<double>>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenUpDownBase&lt;T&gt; (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenUpDownBase<double>> OverriddenUpDownBaseTsToTest 
        {
            get { return OverriddenNumericUpDownsToTest.OfType<IOverriddenUpDownBase<double>>(); }
        }
        #endregion UpDownBase<T>s to test

        #region NumericUpDowns to test
        /// <summary>
        /// Gets a default instance of NumericUpDown (or a derived type) to test.
        /// </summary>
        public virtual NumericUpDown DefaultNumericUpDownToTest
        {
            get { return new NumericUpDown(); }
        }

        /// <summary>
        /// Gets instances of NumericUpDown (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<NumericUpDown> NumericUpDownsToTest
        {
            get
            {
                yield return DefaultNumericUpDownToTest;

                NumericUpDown nud = new NumericUpDown()
                {
                    Value = 3.3333,
                    Maximum = -2.2222,
                    Minimum = -11.1111,
                    Increment = 1.4499,
                    DecimalPlaces = 2,
                    IsEditable = false,
                    ////IsCyclic = true,
                };

                yield return nud;
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenNumericUpDown (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<IOverriddenNumericUpDown> OverriddenNumericUpDownsToTest
        {
            get { yield break; }
        }
        #endregion NumericUpDowns to test

        #region DP tests
        /// <summary>
        /// Gets the Minimum dependency property test.
        /// </summary>
        protected DependencyPropertyTest<NumericUpDown, double> MinimumProperty { get; private set; }

        /// <summary>
        /// Gets the Maximum dependency property test.
        /// </summary>
        protected DependencyPropertyTest<NumericUpDown, double> MaximumProperty { get; private set; }

        /// <summary>
        /// Gets the Increment dependency property test.
        /// </summary>
        protected DependencyPropertyTest<NumericUpDown, double> IncrementProperty { get; private set; }

        /// <summary>
        /// Gets the DecimalPlaces dependency property test.
        /// </summary>
        protected DependencyPropertyTest<NumericUpDown, int> DecimalPlacesProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the NumericUpDownTest class.
        /// </summary>
        public NumericUpDownTest()
            : base()
        {
            // carefully construct DP tests below to avoid coercion
            Func<NumericUpDown> initializer = () => DefaultNumericUpDownToTest;

            BorderThicknessProperty.DefaultValue = new Thickness(1);

            BorderBrushProperty.DefaultValue = new LinearGradientBrush()
            {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1),
                GradientStops = new GradientStopCollection 
                { 
                    new GradientStop { Color = Color.FromArgb(0xFF, 0xA3, 0xAE, 0xB9), Offset = 0 },
                    new GradientStop { Color = Color.FromArgb(0xFF, 0x83, 0x99, 0xA9), Offset = 0.375 },
                    new GradientStop { Color = Color.FromArgb(0xFF, 0x71, 0x85, 0x97), Offset = 0.375 },
                    new GradientStop { Color = Color.FromArgb(0xFF, 0x61, 0x75, 0x84), Offset = 1 },
                }
            };

            ValueProperty.OtherValues = new double[] { 1, 99 };
            ValueProperty.InvalidValues = new Dictionary<double, Type>
            {
                { double.NaN, typeof(ArgumentException) },
                { double.NegativeInfinity, typeof(ArgumentException) },
                { double.PositiveInfinity, typeof(ArgumentException) },
                { (double)double.MinValue - 1, typeof(ArgumentException) },
                { (double)double.MaxValue + 1, typeof(ArgumentException) },
            };

            MinimumProperty = new DependencyPropertyTest<NumericUpDown, double>(this, "Minimum")
            {
                Property = NumericUpDown.MinimumProperty,
                Initializer = initializer,
                DefaultValue = 0,
                OtherValues = new double[] { /*(double)double.MinValue + 1, */ -1000000, -1, 2, 1000000, /*(double)double.MaxValue*/ },
                InvalidValues = new Dictionary<double, Type>
                {
                    { double.NaN, typeof(ArgumentException) },
                    { double.NegativeInfinity, typeof(ArgumentException) },
                    { double.PositiveInfinity, typeof(ArgumentException) },
                    { (double)double.MinValue - 1, typeof(ArgumentException) },
                    { (double)double.MaxValue + 1, typeof(ArgumentException) },
                }
            };

            MaximumProperty = new DependencyPropertyTest<NumericUpDown, double>(this, "Maximum")
            {
                Property = NumericUpDown.MaximumProperty,
                Initializer = initializer,
                DefaultValue = 100,
                OtherValues = new double[] { 0, 1, 1000000, /*(double)double.MaxValue*/ },
                InvalidValues = new Dictionary<double, Type>
                {
                    { double.NaN, typeof(ArgumentException) },
                    { double.NegativeInfinity, typeof(ArgumentException) },
                    { double.PositiveInfinity, typeof(ArgumentException) },
                    { (double)double.MinValue - 1, typeof(ArgumentException) },
                    { (double)double.MaxValue + 1, typeof(ArgumentException) },
                }
            };

            IncrementProperty = new DependencyPropertyTest<NumericUpDown, double>(this, "Increment")
            {
                Property = NumericUpDown.IncrementProperty,
                Initializer = initializer,
                DefaultValue = 1,
                OtherValues = new double[] { /*0*,/ 1, 1000000 /*, (double)double.MaxValue*/ },
                InvalidValues = new Dictionary<double, Type>
                { 
                    { double.NaN, typeof(ArgumentException) },
                    { double.NegativeInfinity, typeof(ArgumentException) },
                    { double.PositiveInfinity, typeof(ArgumentException) },
                    { (double)double.MinValue - 1, typeof(ArgumentException) },
                    { (double)double.MaxValue + 1, typeof(ArgumentException) },
                    { -1, typeof(ArgumentException) },
                    { 0, typeof(ArgumentException) },
                }
            };

            DecimalPlacesProperty = new DependencyPropertyTest<NumericUpDown, int>(this, "DecimalPlaces")
            {
                Property = NumericUpDown.DecimalPlacesProperty,
                Initializer = initializer,
                DefaultValue = 0,
                OtherValues = new int[] { 1, 8, 15 },
                InvalidValues = new Dictionary<int, Type>
                { 
                    { -1, typeof(ArgumentException) },
                    { 16, typeof(ArgumentException) },
                }
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

            // Value Property tests
            tests.Add(ValueProperty.InvalidValueFailsTest);
            tests.Add(ValueProperty.InvalidValueIsIgnoredTest);
            tests.Add(ValueProperty.DoesNotChangeVisualStateTest(1, 2));

            // Minimum Property tests
            tests.Add(MinimumProperty.CheckDefaultValueTest);
            tests.Add(MinimumProperty.ChangeClrSetterTest);
            tests.Add(MinimumProperty.ChangeSetValueTest);
            tests.Add(MinimumProperty.ClearValueResetsDefaultTest);
            tests.Add(MinimumProperty.CanBeStyledTest);
            tests.Add(MinimumProperty.TemplateBindTest);
            tests.Add(MinimumProperty.SetXamlAttributeTest.Bug("523623 - NumericUpDown - Cannot set Minimum/Maximum value via xaml - XamlParseException", true));
            tests.Add(MinimumProperty.SetXamlElementTest);
            tests.Add(MinimumProperty.InvalidValueFailsTest);
            tests.Add(MinimumProperty.InvalidValueIsIgnoredTest);
            tests.Add(MinimumProperty.DoesNotChangeVisualStateTest(1, 2));

            // Maximum Property tests
            tests.Add(MaximumProperty.CheckDefaultValueTest);
            tests.Add(MaximumProperty.ChangeClrSetterTest);
            tests.Add(MaximumProperty.ChangeSetValueTest);
            tests.Add(MaximumProperty.ClearValueResetsDefaultTest);
            tests.Add(MaximumProperty.CanBeStyledTest);
            tests.Add(MaximumProperty.TemplateBindTest);
            tests.Add(MaximumProperty.SetXamlAttributeTest.Bug("523623 - NumericUpDown - Cannot set Minimum/Maximum value via xaml - XamlParseException", true));
            tests.Add(MaximumProperty.SetXamlElementTest);
            tests.Add(MaximumProperty.InvalidValueFailsTest);
            tests.Add(MaximumProperty.InvalidValueIsIgnoredTest);
            tests.Add(MaximumProperty.DoesNotChangeVisualStateTest(1, 2));

            // Increment Property tests
            tests.Add(IncrementProperty.CheckDefaultValueTest);
            tests.Add(IncrementProperty.ChangeClrSetterTest);
            tests.Add(IncrementProperty.ChangeSetValueTest);
            ////tests.Add(IncrementProperty.ClearValueResetsDefaultTest);
            ////tests.Add(IncrementProperty.CanBeStyledTest);
            tests.Add(IncrementProperty.TemplateBindTest);
            tests.Add(IncrementProperty.SetXamlAttributeTest);
            tests.Add(IncrementProperty.SetXamlElementTest);
            tests.Add(IncrementProperty.InvalidValueFailsTest.Bug("528187 - NumericUpDown - The Increment property should larger than 0, not include 0", true));
            tests.Add(IncrementProperty.InvalidValueIsIgnoredTest);
            tests.Add(IncrementProperty.DoesNotChangeVisualStateTest(1, 2));

            // DecimalPlaces Property tests
            tests.Add(DecimalPlacesProperty.CheckDefaultValueTest);
            tests.Add(DecimalPlacesProperty.ChangeClrSetterTest);
            tests.Add(DecimalPlacesProperty.ChangeSetValueTest);
            tests.Add(DecimalPlacesProperty.ClearValueResetsDefaultTest);
            tests.Add(DecimalPlacesProperty.CanBeStyledTest);
            tests.Add(DecimalPlacesProperty.TemplateBindTest);
            tests.Add(DecimalPlacesProperty.SetXamlAttributeTest);
            tests.Add(DecimalPlacesProperty.SetXamlElementTest);
            tests.Add(DecimalPlacesProperty.InvalidValueFailsTest);
            tests.Add(DecimalPlacesProperty.InvalidValueIsIgnoredTest);
            tests.Add(DecimalPlacesProperty.DoesNotChangeVisualStateTest(1, 2));

            return tests;
        }
        #endregion

        #region control contract
        /// <summary>
        /// Verify the control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultControlToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(1, properties.Count, "Incorrect number of style typed property attributes!");
            Assert.AreEqual(typeof(ButtonSpinner), properties["SpinnerStyle"], "Failed to find expected style type property SpinnerStyle!");
        }
        #endregion

        #region coercion test
        /// <summary>
        /// Helper function to output a NumericUpDown.
        /// </summary>
        /// <param name="nud">NumericUpDown control to be output.</param>
        /// <returns>Output string of NumericUpDown control.</returns>
        private static string OutputNUD(NumericUpDown nud)
        {
            string format = "[DecimalPlaces:{0}, Min:{1}, Max:{2}, Val:{3}, Inc:{4}]";
            return string.Format(CultureInfo.InvariantCulture, format, nud.DecimalPlaces, nud.Minimum, nud.Maximum, nud.Value, nud.Increment);
        }

        /// <summary>
        /// Helper function to verify coercion.
        /// </summary>
        /// <param name="nud">NumericUpDown control whose properties are checked for coercion.</param>
        /// <param name="min">Minimal requested.</param>
        /// <param name="max">Maximum requested.</param>
        /// <param name="val">Value requested.</param>
        private static void VerifyCoercion(NumericUpDown nud, double min, double max, double val)
        {
            Assert.IsTrue(nud.Minimum == min, "Invalid Minimum: {0}\n{{{1},{2},{3}}}", OutputNUD(nud), min, max, val);
            Assert.IsTrue(nud.Maximum == Math.Max(min, max), "Invalid Maximum: {0}\n{{{1},{2},{3}}}", OutputNUD(nud), min, max, val);
            Assert.IsTrue(nud.Value >= min && nud.Value <= Math.Max(min, max), "Invalid Value: {0}\n{{{1},{2},{3}}}", OutputNUD(nud), min, max, val);
            ////VerifyDecimalPlaces(nud);
        }

        /////// <summary>
        /////// Helper function to verify DecimalPlaces.
        /////// </summary>
        /////// <param name="nud">NumericUpDown control whose properties are checked for DecimalPlaces.</param>
        ////private static void VerifyDecimalPlaces(NumericUpDown nud)
        ////{
        ////    Assert.IsTrue(
        ////        nud.DecimalPlaces >= 0 && nud.DecimalPlaces < 15, 
        ////        "Invalid DecimalPlaces: {0}", 
        ////        OutputNUD(nud));

        ////    Assert.AreEqual<double>(
        ////        Math.Round(nud.Minimum, nud.DecimalPlaces), 
        ////        nud.Minimum, 
        ////        "Invalid Minimum: {0}", 
        ////        OutputNUD(nud));
        ////    Assert.AreEqual<double>(
        ////        Math.Round(nud.Maximum, nud.DecimalPlaces), 
        ////        nud.Maximum, 
        ////        "Invalid Maximum: {0}", 
        ////        OutputNUD(nud));
        ////    Assert.AreEqual<double>(
        ////        Math.Round(nud.Value, nud.DecimalPlaces), 
        ////        nud.Value, 
        ////        "Invalid Value: {0}", 
        ////        OutputNUD(nud));
        ////    Assert.AreEqual<double>(
        ////        Math.Round(nud.Increment, nud.DecimalPlaces), 
        ////        nud.Increment, 
        ////        "Invalid Increment: {0}", 
        ////        OutputNUD(nud));
        ////}

        /// <summary>
        /// Helper function to generate Action for setting properties of a NumericUpDown object.
        /// </summary>
        /// <param name="nud">The NumericUpDown object whose properties are set.</param>
        /// <param name="min">NumericUpDown Minimal.</param>
        /// <param name="max">NumericUpDown Maximum.</param>
        /// <param name="val">NumericUpDown Value.</param>
        /// <param name="inc">NumericUpDown Increment.</param>
        /// <param name="decimalPlaces">NumericUpDown DecimalPlaces.</param>
        /// <returns>Action object to set those properties for a NumericUpDown object.</returns>
        private static Action SetNUD(NumericUpDown nud, double min, double max, double val, double inc, int decimalPlaces)
        {
            return () => 
                {
                    nud.Increment = inc;
                    nud.Value = val;
                    nud.Maximum = max;
                    nud.Minimum = min; 
                    nud.DecimalPlaces = decimalPlaces;
                };
        }

        /// <summary>
        /// Helper function to generate Action object for verifying properties of a NumericUpDown object.
        /// </summary>
        /// <param name="nud">The NumericUpDown object whose properties are verified.</param>
        /// <param name="min">NumericUpDown Minimal.</param>
        /// <param name="max">NumericUpDown Maximum.</param>
        /// <param name="val">NumericUpDown Value.</param>
        /// <param name="inc">NumericUpDown Increment.</param>
        /// <param name="decimalPlaces">NumericUpDown DecimalPlaces.</param>
        /// <returns>Action object to verify those properties for a NumericUpDown object.</returns>
        private static Action VerifyNUD(NumericUpDown nud, double min, double max, double val, double inc, int decimalPlaces)
        {
            return () => 
                {
                    Assert.AreEqual<double>(min, nud.Minimum, "Invalid Minimum: {0}", OutputNUD(nud));
                    Assert.AreEqual<double>(max, nud.Maximum, "Invalid Maximum: {0}", OutputNUD(nud));
                    Assert.AreEqual<double>(val, nud.Value, "Invalid Value: {0}", OutputNUD(nud));
                    Assert.AreEqual<double>(inc, nud.Increment, "Invalid Increment: {0}", OutputNUD(nud));
                    Assert.AreEqual<int>(decimalPlaces, nud.DecimalPlaces, "Invalid DecimalPlaces: {0}", OutputNUD(nud));
                };
        }

        /// <summary>
        /// Exhaustive test of coercion combinations for NumericUpDown.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Exhaustive test of coercion combinations for NumericUpDown.")]
        public void TestCoercionAsync()
        {
            NumericUpDown nud = (NumericUpDown)DefaultControlToTest;
            List<Action> actions = new List<Action>();

            // initialize and verify all parameters
            actions.Add(SetNUD(nud, -0.01, 9.99, -2, 1.11, 2));
            actions.Add(VerifyNUD(nud, -0.01, 9.99, -0.01, 1.11, 2));

            double[] doubles = new double[] { 100, 50, 0, -50, -100 };
            foreach (double val in doubles)
            {
                foreach (double max in doubles)
                {
                    foreach (double min in doubles)
                    {
                        double cmin = min;
                        double cmax = max;
                        double cval = val;

                        actions.Add(SetNUD(nud, cmin, cmax, cval, 1.11, 2));
                        actions.Add(() => VerifyCoercion(nud, cmin, cmax, cval));
                    }
                }
            }

            TestAsync(nud, actions.ToArray());
        }
        #endregion

        #region Maximum Minimum
        /// <summary>
        /// Tests that the control will set correct validSpinDirection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that the control will set correct validSpinDirection")]
        public virtual void ShouldDetermineValidSpinDirection()
        {
            NumericUpDown nud = new NumericUpDown();
            nud.Minimum = 5;
            nud.Maximum = 10;

            ButtonSpinner spinner = null;

            TestAsync(
                nud,
                () => spinner = ((Panel)VisualTreeHelper.GetChild(nud, 0)).FindName("Spinner") as ButtonSpinner,
                () => Assert.AreEqual(ValidSpinDirections.Increase, spinner.ValidSpinDirection),
                () => nud.Value = 8,
                () => Assert.IsTrue(spinner.ValidSpinDirection == (ValidSpinDirections.Increase | ValidSpinDirections.Decrease)),
                () => nud.Value = 10,
                () => Assert.AreEqual(ValidSpinDirections.Decrease, spinner.ValidSpinDirection),
                () => nud.Maximum = 11,
                () => Assert.IsTrue(spinner.ValidSpinDirection == (ValidSpinDirections.Increase | ValidSpinDirections.Decrease)),
                () => nud.Value = 5,
                () => nud.Minimum = 1,
                () => Assert.IsTrue(spinner.ValidSpinDirection == (ValidSpinDirections.Increase | ValidSpinDirections.Decrease)));
        }
        #endregion Maximum Minimum
    }
}