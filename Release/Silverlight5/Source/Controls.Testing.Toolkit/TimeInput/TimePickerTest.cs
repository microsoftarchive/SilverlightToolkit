// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Tests for TimePicker.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Testing a complex class.")]
    [TestClass]
    [Tag("TimeInput")]
    public class TimePickerTest : PickerTest
    {
        /// <summary>
        /// Helper function to load UI from xaml file.
        /// </summary>
        /// <param name="resourceFile">Resource file name.</param>
        /// <returns>Instantiated object from Xaml.</returns>
        private ResourceDictionary LoadXamlResource(string resourceFile)
        {
            StreamReader streamReader = null;
            Stream stream = null;
            string resourceName = null;

            string[] names = this.GetType().Assembly.GetManifestResourceNames();
            foreach (string resource in names)
            {
                if (resource.Contains(resourceFile))
                {
                    resourceName = resource;
                }
            }
            Assert.IsNotNull(resourceName, "Could not find resource " + resourceFile);
            stream = this.GetType().Assembly.GetManifestResourceStream(resourceName);
            streamReader = new StreamReader(stream);
            string xamlString = streamReader.ReadToEnd();

            return (ResourceDictionary)XamlReader.Load(xamlString);
        }

        /// <summary>
        /// The name of the resources file.
        /// </summary>
        private const string DefaultStylesResource = "TimeInputStyles";

        // todo: use when harness is opened up

        ///// <summary>
        ///// Gets the current test method name.
        ///// </summary>
        ///// <returns>Test method name.</returns>
        // public static string CurrentTestName
        // {
        //    get
        //    {
        //        UnitTestHarness harness = (UnitTestHarness)UnitTestSystem.CurrentHarness;
        //        MethodContainer container = (MethodContainer)harness.DispatcherStack.CurrentCompositeWorkItem;
        //        return container.MethodInfo.Name;
        //    }
        // }

        #region Getting controls to test
        /// <summary>
        /// Gets the default picker to test.
        /// </summary>
        /// <value>The default picker to test.</value>
        public override Picker DefaultPickerToTest
        {
            get { return DefaultTimePickerToTest; }
        }

        /// <summary>
        /// Gets the pickers to test.
        /// </summary>
        /// <value>The pickers to test.</value>
        public override IEnumerable<Picker> PickersToTest
        {
            get { return (IEnumerable<Picker>)TimePickersToTest; }
        }

        /// <summary>
        /// Gets the overridden pickers to test.
        /// </summary>
        /// <value>The overridden pickers to test.</value>
        public override IEnumerable<IOverriddenControl> OverriddenPickersToTest
        {
            get { return OverriddenTimePickersToTest; }
        }

        /// <summary>
        /// Gets the default time picker to test.
        /// </summary>
        /// <value>The default time picker to test.</value>
        public virtual TimePicker DefaultTimePickerToTest
        {
            get { return new TimePicker(); }
        }

        /// <summary>
        /// Gets the time pickers to test.
        /// </summary>
        /// <value>The time pickers to test.</value>
        public virtual IEnumerable<TimePicker> TimePickersToTest
        {
            get { yield return DefaultTimePickerToTest; }
        }

        /// <summary>
        /// Gets the overridden time pickers to test.
        /// </summary>
        /// <value>The overridden time pickers to test.</value>
        public virtual IEnumerable<IOverriddenControl> OverriddenTimePickersToTest
        {
            get { yield break; }
        } 
        #endregion Getting controls to test

        #region Dependency properties
        /// <summary>
        /// Gets the value property.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Value is nullable DateTime.")]
        protected DependencyPropertyTest<TimePicker, DateTime?> ValueProperty { get; private set; }
        
        /// <summary>
        /// Gets the minimum property.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Value is nullable DateTime.")]
        protected DependencyPropertyTest<TimePicker, DateTime?> MinimumProperty { get; private set; }

        /// <summary>
        /// Gets the maximum property.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Value is nullable DateTime.")]
        protected DependencyPropertyTest<TimePicker, DateTime?> MaximumProperty { get; private set; }

        /// <summary>
        /// Gets the time up down style property.
        /// </summary>
        protected DependencyPropertyTest<TimePicker, Style> TimeUpDownStyleProperty { get; private set; }

        /// <summary>
        /// Gets the spinner property.
        /// </summary>
        protected DependencyPropertyTest<TimePicker, Style> SpinnerStyleProperty { get; private set; }

        /// <summary>
        /// Gets the time parsers property.
        /// </summary>
        protected DependencyPropertyTest<TimePicker, TimeParserCollection> TimeParsersProperty { get; private set; }
        
        /// <summary>
        /// Gets the display format property.
        /// </summary>
        protected DependencyPropertyTest<TimePicker, ITimeFormat> FormatProperty { get; private set; }
        
        /// <summary>
        /// Gets the culture property.
        /// </summary>
        protected DependencyPropertyTest<TimePicker, CultureInfo> CultureProperty { get; private set; }

        /// <summary>
        /// Gets the time globalization info property.
        /// </summary>
        protected DependencyPropertyTest<TimePicker, TimeGlobalizationInfo> TimeGlobalizationInfoProperty { get; private set; }

        /// <summary>
        /// Gets the popup seconds interval property.
        /// </summary>
        protected DependencyPropertyTest<TimePicker, int> PopupSecondsIntervalProperty { get; private set; }

        /// <summary>
        /// Gets the popup minutes interval property.
        /// </summary>
        protected DependencyPropertyTest<TimePicker, int> PopupMinutesIntervalProperty { get; private set; }

        /// <summary>
        /// Gets the popup time selection mode property.
        /// </summary>
        protected DependencyPropertyTest<TimePicker, PopupTimeSelectionMode> PopupTimeSelectionModeProperty { get; private set; }
        #endregion Dependency properties

        /// <summary>
        /// Initializes a new instance of the <see cref="TimePickerTest"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Testing a complex class.")]
        public TimePickerTest()
        {
            LinearGradientBrush brush = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 0.0),
                EndPoint = new Point(0.5, 1.0)
            };
            brush.GradientStops.Add(
                new GradientStop
                {
                    Color = System.Windows.Media.Color.FromArgb(0xFF, 0xA3, 0xAE, 0xB9),
                    Offset = 0.0
                });
            brush.GradientStops.Add(
                new GradientStop
                {
                    Color = System.Windows.Media.Color.FromArgb(0xFF, 0x83, 0x99, 0xA9),
                    Offset = 0.375
                });
            brush.GradientStops.Add(
                new GradientStop
                {
                    Color = System.Windows.Media.Color.FromArgb(0xFF, 0x71, 0x85, 0x97),
                    Offset = 0.375
                });
            brush.GradientStops.Add(
                new GradientStop
                {
                    Color = System.Windows.Media.Color.FromArgb(0xFF, 0x61, 0x75, 0x84),
                    Offset = 1.0
                });
            BorderBrushProperty.DefaultValue = brush;
            BorderThicknessProperty.DefaultValue = new Thickness(1);
            BackgroundProperty.DefaultValue = new SolidColorBrush(Colors.Transparent);

            Func<TimePicker> initializer = () => DefaultTimePickerToTest;

            ValueProperty = new DependencyPropertyTest<TimePicker, DateTime?>(this, "Value")
            {
                Property = TimePicker.ValueProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new DateTime?(), new DateTime(2000, 12, 3), new DateTime(1900, 1, 1) },
            };

            MinimumProperty = new DependencyPropertyTest<TimePicker, DateTime?>(this, "Minimum")
            {
                Property = TimePicker.MinimumProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new DateTime?(), new DateTime(2000, 12, 3), new DateTime(1900, 1, 1) },
            };

            MaximumProperty = new DependencyPropertyTest<TimePicker, DateTime?>(this, "Maximum")
            {
                Property = TimePicker.MaximumProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new DateTime?(), new DateTime(2000, 12, 3), new DateTime(1900, 1, 1) },
            };

            TimeUpDownStyleProperty = new DependencyPropertyTest<TimePicker, Style>(this, "TimeUpDownStyle")
            {
                Property = TimePicker.TimeUpDownStyleProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new Style(typeof(TimeUpDown)) }
            };

            SpinnerStyleProperty = new DependencyPropertyTest<TimePicker, Style>(this, "SpinnerStyle")
            {
                Property = TimePicker.SpinnerStyleProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new Style(typeof(ButtonSpinner)), new Style(typeof(Control)) }
            };

            TimeParsersProperty = new DependencyPropertyTest<TimePicker, TimeParserCollection>(this, "TimeParsers")
            {
                Property = TimePicker.TimeParsersProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new TimeParserCollection(), null }
            };

            FormatProperty = new DependencyPropertyTest<TimePicker, ITimeFormat>(this, "Format")
            {
                Property = TimePicker.FormatProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new ITimeFormat[] { new ShortTimeFormat(), new LongTimeFormat(), new CustomTimeFormat("hh:ss") },
            };

            CultureProperty = new DependencyPropertyTest<TimePicker, CultureInfo>(this, "Culture")
            {
                Property = TimePicker.CultureProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new CultureInfo("en-US"), new CultureInfo("nl-NL") }
            };

            TimeGlobalizationInfoProperty = new DependencyPropertyTest<TimePicker, TimeGlobalizationInfo>(this, "TimeGlobalizationInfo")
            {
                Property = TimePicker.TimeGlobalizationInfoProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new TimeGlobalizationInfo() }
            };

            PopupSecondsIntervalProperty = new DependencyPropertyTest<TimePicker, int>(this, "PopupSecondsInterval")
            {
                Property = TimePicker.PopupSecondsIntervalProperty,
                Initializer = initializer,
                DefaultValue = default(int),
                OtherValues = new[] { 1, 2, 30 },
                InvalidValues = new Dictionary<int, Type>()
                                    {
                                        { -10, typeof(ArgumentOutOfRangeException) },
                                        { -1, typeof(ArgumentOutOfRangeException) },
                                        { 60, typeof(ArgumentOutOfRangeException) },
                                        { 2360, typeof(ArgumentOutOfRangeException) },
                                        { -21311, typeof(ArgumentOutOfRangeException) },
                                    }
            };

            PopupMinutesIntervalProperty = new DependencyPropertyTest<TimePicker, int>(this, "PopupMinutesInterval")
            {
                Property = TimePicker.PopupMinutesIntervalProperty,
                Initializer = initializer,
                DefaultValue = default(int),
                OtherValues = new[] { 1, 2, 30 },
                InvalidValues = new Dictionary<int, Type>()
                                    {
                                        { -10, typeof(ArgumentOutOfRangeException) },
                                        { -1, typeof(ArgumentOutOfRangeException) },
                                        { 60, typeof(ArgumentOutOfRangeException) },
                                        { 12344, typeof(ArgumentOutOfRangeException) },
                                        { -21311, typeof(ArgumentOutOfRangeException) },
                                    }
            };

            // todo: the enum values that are commented out will be included after mix.
            PopupTimeSelectionModeProperty = new DependencyPropertyTest<TimePicker, PopupTimeSelectionMode>(this, "PopupTimeSelectionMode")
            {
                Property = TimePicker.PopupTimeSelectionModeProperty,
                Initializer = initializer,
                DefaultValue = PopupTimeSelectionMode.HoursAndMinutesOnly,
                // OtherValues = new[] { PopupTimeSelectionMode.AllowTimeDesignatorsSelection, PopupTimeSelectionMode.AllowSecondsAndDesignatorsSelection, PopupTimeSelectionMode.AllowSecondsSelection },
                InvalidValues = new Dictionary<PopupTimeSelectionMode, Type>()
                                    {
                                        { (PopupTimeSelectionMode)12, typeof(ArgumentOutOfRangeException) },
                                        { (PopupTimeSelectionMode)23, typeof(ArgumentOutOfRangeException) },
                                        { (PopupTimeSelectionMode)66, typeof(ArgumentOutOfRangeException) },
                                    }
            };
        }

        /// <summary>
        /// Gets the dependency property tests.
        /// </summary>
        /// <returns>A list of DP tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());

            // ValueProperty tests
            tests.Add(ValueProperty.CheckDefaultValueTest);
            tests.Add(ValueProperty.ChangeClrSetterTest);
            tests.Add(ValueProperty.ChangeSetValueTest);
            tests.Add(ValueProperty.ClearValueResetsDefaultTest);
            tests.Add(ValueProperty.CanBeStyledTest);
            tests.Add(ValueProperty.TemplateBindTest);
            tests.Add(ValueProperty.SetXamlAttributeTest);
            tests.Add(ValueProperty.SetXamlElementTest.Bug("TODO: xaml parser."));

            // MinimumProperty tests
            tests.Add(MinimumProperty.CheckDefaultValueTest);
            tests.Add(MinimumProperty.ChangeClrSetterTest);
            tests.Add(MinimumProperty.ChangeSetValueTest);
            tests.Add(MinimumProperty.ClearValueResetsDefaultTest);
            tests.Add(MinimumProperty.CanBeStyledTest);
            tests.Add(MinimumProperty.TemplateBindTest);
            tests.Add(MinimumProperty.SetXamlAttributeTest);
            tests.Add(MinimumProperty.SetXamlElementTest.Bug("TODO: xaml parser."));

            // MaximumProperty tests
            tests.Add(MaximumProperty.CheckDefaultValueTest);
            tests.Add(MaximumProperty.ChangeClrSetterTest);
            tests.Add(MaximumProperty.ChangeSetValueTest);
            tests.Add(MaximumProperty.ClearValueResetsDefaultTest);
            tests.Add(MaximumProperty.CanBeStyledTest);
            tests.Add(MaximumProperty.TemplateBindTest);
            tests.Add(MaximumProperty.SetXamlAttributeTest);
            tests.Add(MaximumProperty.SetXamlElementTest.Bug("TODO: xaml parser."));

            // TimeUpDownStyleProperty tests
            tests.Add(TimeUpDownStyleProperty.CheckDefaultValueTest);
            tests.Add(TimeUpDownStyleProperty.ChangeClrSetterTest);
            tests.Add(TimeUpDownStyleProperty.ChangeSetValueTest);
            tests.Add(TimeUpDownStyleProperty.SetNullTest);
            tests.Add(TimeUpDownStyleProperty.ClearValueResetsDefaultTest);
            tests.Add(TimeUpDownStyleProperty.CanBeStyledTest);
            tests.Add(TimeUpDownStyleProperty.TemplateBindTest);
            tests.Add(TimeUpDownStyleProperty.SetXamlAttributeTest.Bug("TODO: XAML Parser?"));
            tests.Add(TimeUpDownStyleProperty.SetXamlElementTest.Bug("TODO: XAML Parser?"));

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

            // TimeParsersProperty tests
            tests.Add(TimeParsersProperty.CheckDefaultValueTest);
            tests.Add(TimeParsersProperty.ChangeClrSetterTest);
            tests.Add(TimeParsersProperty.ChangeSetValueTest);
            tests.Add(TimeParsersProperty.ClearValueResetsDefaultTest);
            tests.Add(TimeParsersProperty.SetNullTest);
            tests.Add(TimeParsersProperty.CanBeStyledTest);
            tests.Add(TimeParsersProperty.TemplateBindTest);
            tests.Add(TimeParsersProperty.SetXamlAttributeTest.Bug("TODO: xaml parser?"));
            tests.Add(TimeParsersProperty.SetXamlElementTest.Bug("TODO: xaml parser?"));

            // FormatProperty tests
            tests.Add(FormatProperty.CheckDefaultValueTest);
            tests.Add(FormatProperty.ChangeClrSetterTest);
            tests.Add(FormatProperty.ChangeSetValueTest);
            tests.Add(FormatProperty.ClearValueResetsDefaultTest);
            tests.Add(FormatProperty.CanBeStyledTest);
            tests.Add(FormatProperty.TemplateBindTest);

            // CultureProperty tests
            tests.Add(CultureProperty.CheckDefaultValueTest);
            tests.Add(CultureProperty.ChangeClrSetterTest);
            tests.Add(CultureProperty.ChangeSetValueTest);
            tests.Add(CultureProperty.ClearValueResetsDefaultTest);
            tests.Add(CultureProperty.CanBeStyledTest);
            tests.Add(CultureProperty.TemplateBindTest);
            tests.Add(CultureProperty.SetXamlAttributeTest.Bug("TODO: framework substitutes x:Null, which is not correct for CultureInfo"));
            tests.Add(CultureProperty.SetXamlElementTest.Bug("TODO: framework substitutes x:Null, which is not correct for CultureInfo"));

            // TimeGlobalizationInfoProperty tests
            tests.Add(TimeGlobalizationInfoProperty.CheckDefaultValueTest);
            tests.Add(TimeGlobalizationInfoProperty.ChangeClrSetterTest);
            tests.Add(TimeGlobalizationInfoProperty.ChangeSetValueTest);
            tests.Add(TimeGlobalizationInfoProperty.ClearValueResetsDefaultTest);
            tests.Add(TimeGlobalizationInfoProperty.CanBeStyledTest); 
            tests.Add(TimeGlobalizationInfoProperty.TemplateBindTest);

            // PopupSecondsIntervalProperty tests
            tests.Add(PopupSecondsIntervalProperty.CheckDefaultValueTest);
            tests.Add(PopupSecondsIntervalProperty.ChangeClrSetterTest);
            tests.Add(PopupSecondsIntervalProperty.ChangeSetValueTest);
            tests.Add(PopupSecondsIntervalProperty.InvalidValueFailsTest);
            tests.Add(PopupSecondsIntervalProperty.InvalidValueIsIgnoredTest);
            tests.Add(PopupSecondsIntervalProperty.ClearValueResetsDefaultTest);
            tests.Add(PopupSecondsIntervalProperty.CanBeStyledTest);
            tests.Add(PopupSecondsIntervalProperty.TemplateBindTest);
            tests.Add(PopupSecondsIntervalProperty.SetXamlAttributeTest);
            tests.Add(PopupSecondsIntervalProperty.SetXamlElementTest);

            // PopupMinutesIntervalProperty tests
            // tests.Add(PopupMinutesIntervalProperty.CheckDefaultValueTest);
            tests.Add(PopupMinutesIntervalProperty.ChangeClrSetterTest);
            tests.Add(PopupMinutesIntervalProperty.ChangeSetValueTest);
            tests.Add(PopupMinutesIntervalProperty.InvalidValueFailsTest);
            tests.Add(PopupMinutesIntervalProperty.InvalidValueIsIgnoredTest);
            tests.Add(PopupMinutesIntervalProperty.ClearValueResetsDefaultTest);
            tests.Add(PopupMinutesIntervalProperty.CanBeStyledTest);
            tests.Add(PopupMinutesIntervalProperty.TemplateBindTest);
            tests.Add(PopupMinutesIntervalProperty.SetXamlAttributeTest);
            tests.Add(PopupMinutesIntervalProperty.SetXamlElementTest);

            // PopupTimeSelectionModeProperty tests
            tests.Add(PopupTimeSelectionModeProperty.CheckDefaultValueTest);
            tests.Add(PopupTimeSelectionModeProperty.ChangeClrSetterTest);
            tests.Add(PopupTimeSelectionModeProperty.ChangeSetValueTest);
            tests.Add(PopupTimeSelectionModeProperty.InvalidValueIsIgnoredTest);
            // tests.Add(PopupTimeSelectionModeProperty.ClearValueResetsDefaultTest);
            // tests.Add(PopupTimeSelectionModeProperty.CanBeStyledTest);
            // tests.Add(PopupTimeSelectionModeProperty.TemplateBindTest);
            tests.Add(PopupTimeSelectionModeProperty.SetXamlAttributeTest);
            tests.Add(PopupTimeSelectionModeProperty.SetXamlElementTest);

            return tests;
        }

        #region control contracts
        /// <summary>
        /// Verify the control's template parts.
        /// </summary>
        public override void TemplatePartsAreDefined()
        {
            IDictionary<string, Type> parts = DefaultTimePickerToTest.GetType().GetTemplateParts();
            Assert.AreEqual(4, parts.Count);
            Assert.AreEqual(typeof(TimeUpDown), parts["TimeUpDown"], "Failed to find expected part TimeUpDown!");
            Assert.AreEqual(typeof(Popup), parts["Popup"], "Failed to find expected part Popup!");
            Assert.AreEqual(typeof(ToggleButton), parts["DropDownToggle"], "Failed to find expected part DropDownToggle!");
            Assert.AreEqual(typeof(ContentControl), parts["PopupPlaceHolder"], "Failed to find expected part PopupPlaceHolder!");
        }

        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> states = DefaultTimePickerToTest.GetType().GetVisualStates();
            Assert.AreEqual(8, states.Count, "Incorrect number of template states");
            Assert.AreEqual("CommonStates", states["Normal"], "Failed to find expected state Normal!");
            Assert.AreEqual("CommonStates", states["MouseOver"], "Failed to find expected state MouseOver!");
            Assert.AreEqual("CommonStates", states["Pressed"], "Failed to find expected state Pressed!");
            Assert.AreEqual("CommonStates", states["Disabled"], "Failed to find expected state Disabled!");
            Assert.AreEqual("FocusStates", states["Focused"], "Failed to find expected state Focused!");
            Assert.AreEqual("FocusStates", states["Unfocused"], "Failed to find expected state Unfocused!");
            Assert.AreEqual("PopupStates", states["PopupOpened"], "Failed to find expected state PopupOpened!");
            Assert.AreEqual("PopupStates", states["PopupClosed"], "Failed to find expected state PopupClosed!");
        }

        /// <summary>
        /// Verify the control's style typed properties.
        /// </summary>
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> styleTypedProperties = DefaultTimePickerToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(2, styleTypedProperties.Count, "Incorrect number of StyleType properties");
            Assert.AreEqual(typeof(TimeUpDown), styleTypedProperties["TimeUpDownStyle"], "Failed to find expected style type TimeUpDownStyle");
            Assert.AreEqual(typeof(ButtonSpinner), styleTypedProperties["SpinnerStyle"], "Failed to find expected style type SpinnerStyle");
        }
        #endregion control contracts

        #region Coercion
        /// <summary>
        /// Breadth test on interaction between minimum maximum and value.
        /// </summary>
        [TestMethod]
        [Description("Breadth test on interaction between minimum maximum and value")]
        public virtual void ShouldCoerceMinimumMaximumAndValue()
        {
            TimePicker tud = new TimePicker();

            tud.Value = new DateTime(2100, 1, 1, 14, 0, 0);
            tud.Minimum = new DateTime(2100, 1, 1, 14, 0, 0);
            tud.Maximum = new DateTime(2100, 1, 1, 16, 0, 0);

            int valueChangedCount = 0;
            tud.ValueChanged += (sender, e) => valueChangedCount++;

            tud.Minimum = tud.Minimum.Value.AddMinutes(30.0);
            Assert.AreEqual(new DateTime(2100, 1, 1, 14, 30, 0), tud.Minimum.Value);
            Assert.AreEqual(tud.Minimum, tud.Value);
            tud.Value = tud.Maximum.Value;
            tud.Maximum = tud.Maximum.Value.AddMinutes(-30.0);
            Assert.AreEqual(new DateTime(2100, 1, 1, 15, 30, 0), tud.Maximum.Value);
            Assert.AreEqual(tud.Maximum, tud.Value);
            Assert.AreEqual(3, valueChangedCount, "3 changes in value, should have raised event 3 times.");
        }

        /// <summary>
        /// Tests setting ranges.
        /// </summary>
        [TestMethod]
        [Description("Tests setting ranges.")]
        public virtual void ShouldBeAbleToSetInvalidBoundaries()
        {
            TimePicker tud = new TimePicker();

            tud.Value = new DateTime(2100, 1, 1, 14, 0, 0);
            tud.Minimum = new DateTime(2100, 1, 1, 14, 0, 0);
            tud.Maximum = new DateTime(2100, 1, 1, 16, 0, 0);

            tud.Maximum = new DateTime(2100, 1, 1, 11, 0, 0);

            Assert.AreEqual(new DateTime(2100, 1, 1, 11, 0, 0), tud.Minimum);
            Assert.AreEqual(new DateTime(2100, 1, 1, 11, 0, 0), tud.Maximum);
            Assert.AreEqual(new DateTime(2100, 1, 1, 11, 0, 0), tud.Value);

            tud.Minimum = new DateTime(2100, 1, 1, 13, 0, 0);
            Assert.AreEqual(new DateTime(2100, 1, 1, 13, 0, 0), tud.Minimum);
            Assert.AreEqual(new DateTime(2100, 1, 1, 13, 0, 0), tud.Maximum);
            Assert.AreEqual(new DateTime(2100, 1, 1, 13, 0, 0), tud.Value);
        }

        /// <summary>
        /// Tests that min, max and value do not care for date part.
        /// </summary>
        [TestMethod]
        [Description("Tests that min, max and value do not care for date part.")]
        public virtual void ShouldDisregardDatePartWhenCoercing()
        {
            TimePicker tud = new TimePicker();

            tud.Value = new DateTime(2100, 1, 1, 15, 0, 0);
            tud.Minimum = new DateTime(2100, 1, 1, 14, 0, 0);
            tud.Maximum = new DateTime(2100, 1, 1, 16, 0, 0);

            tud.Value = tud.Value.Value.AddDays(2);
            tud.Maximum = tud.Maximum.Value.AddMinutes(-90);
            Assert.AreEqual(new DateTime(2100, 1, 3, 14, 30, 0), tud.Value.Value);
            tud.Value = tud.Value.Value.AddYears(-1);
            Assert.AreEqual(new DateTime(2099, 1, 3, 14, 30, 0), tud.Value.Value);
        }

        /// <summary>
        /// Tests that last valid value is used when value is set outside of valid range.
        /// </summary>
        [TestMethod]
        [Description("Tests that last valid value is used when value is set outside of valid range.")]
        public virtual void ShouldRevertToLastValidValueWhenSetOutsideRange()
        {
            TimePicker tud = new TimePicker();

            tud.Value = new DateTime(2100, 1, 1, 15, 0, 0);
            tud.Minimum = new DateTime(2100, 1, 1, 14, 0, 0);
            tud.Maximum = new DateTime(2100, 1, 1, 16, 0, 0);

            tud.Value = new DateTime(2100, 1, 1, 10, 0, 0);
            Assert.AreEqual(new DateTime(2100, 1, 1, 15, 0, 0), tud.Value.Value);
        }
        #endregion

        #region interaction with TimeUpDown
        /// <summary>
        /// Tests that Value gets transferred correctly to TimeUpDown.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that Value gets transferred correctly to TimeUpDown.")]
        public virtual void ShouldBindValueToTimeUpDownValue()
        {
            TimePicker tip = new TimePicker();
            TimeUpDown tud = null;

            TestAsync(
                tip,
                () => tud = ((Panel)VisualTreeHelper.GetChild(tip, 0)).FindName("TimeUpDown") as TimeUpDown,
                () => tip.Value = new DateTime(1900, 1, 1, 9, 5, 10),
                () => Assert.AreEqual(tip.Value, tud.Value));
        }

        /// <summary>
        /// Tests that setting a value before template was applied on TimePicker flows down into TimeUpDown.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that setting a value on TimePicker flows down into TimeUpDown.")]
        public virtual void ShouldInitializeValueInTimeUpDownCorrectlyBeforeTemplateIsApplied()
        {
            OverriddenTimePicker tip = new OverriddenTimePicker(true)
            {
                Culture = new CultureInfo("en-US"),
                Value = new DateTime(1900, 1, 1, 11, 11, 11)
            };
            OverriddenTimeUpDown tud = null;

            TestAsync(
                tip,
                () => tud = tip.OverriddenTimeUpDown,
                () => Assert.AreEqual("11:11 AM", tud.DisplayTextBox.Text));
        }

        /// <summary>
        /// Tests that Value from TimeUpDown gets flowed up to TimePicker.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that Value from TimeUpDown gets flowed up to TimePicker.")]
        public virtual void ShouldFlowUpValueFromTimeUpDown()
        {
            TimePicker tip = new TimePicker();
            TimeUpDown tud = null;

            TestAsync(
                tip,
                () => tud = ((Panel)VisualTreeHelper.GetChild(tip, 0)).FindName("TimeUpDown") as TimeUpDown,
                () => tud.Value = new DateTime(1900, 1, 1, 9, 5, 10),
                () => Assert.AreEqual(new DateTime(1900, 1, 1, 9, 5, 10), tip.Value));
        }

        /// <summary>
        /// Tests that Value binding does not get removed after setting value directly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that Value binding does not get removed after setting value directly.")]
        public virtual void ShouldKeepBindingBetweenValueIntact()
        {
            TimePicker tip = new TimePicker();
            TimeUpDown tud = null;

            TestAsync(
                tip,
                () => tud = ((Panel)VisualTreeHelper.GetChild(tip, 0)).FindName("TimeUpDown") as TimeUpDown,
                () => tud.Value = new DateTime(1900, 1, 1, 9, 5, 10),
                () => Assert.AreEqual(tip.Value, tud.Value),
                () => tip.Value = new DateTime(2000, 1, 1, 11, 6, 8),
                () => Assert.AreEqual(new DateTime(2000, 1, 1, 11, 6, 8), tud.Value));
        }

        /// <summary>
        /// Tests that minimum and maximum are flowed to TimeUpDown.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that minimum and maximum are flowed to TimeUpDown.")]
        public virtual void ShouldBindMinimumAndMaximumToTimeUpDown()
        {
            TimePicker tip = new TimePicker();
            tip.Minimum = new DateTime(1950, 1, 1, 3, 0, 0);
            tip.Minimum = new DateTime(1940, 1, 1, 18, 0, 0);
            TimeUpDown tud = null;

            TestAsync(
                tip,
                () => tud = ((Panel)VisualTreeHelper.GetChild(tip, 0)).FindName("TimeUpDown") as TimeUpDown,
                () => Assert.AreEqual(tip.Minimum, tud.Minimum),
                () => Assert.AreEqual(tip.Maximum, tud.Maximum),
                () => tip.Minimum = new DateTime(1950, 1, 1, 10, 0, 0),
                () => tip.Maximum = new DateTime(1950, 1, 1, 14, 0, 0),
                () => Assert.AreEqual(tip.Minimum, tud.Minimum),
                () => Assert.AreEqual(tip.Maximum, tud.Maximum));
        }

        /// <summary>
        /// Tests that TimeParsers are flowed to TimeUpDown.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that TimeParsers are flowed to TimeUpDown.")]
        public virtual void ShouldBindParsersToTimeUpDown()
        {
            TimePicker tip = new TimePicker();
            TimeUpDown tud = null;

            TimeParserCollection parsers = new TimeParserCollection();
            parsers.Add(new CustomTimeParser());

            TestAsync(
                tip,
                () => tud = ((Panel)VisualTreeHelper.GetChild(tip, 0)).FindName("TimeUpDown") as TimeUpDown,
                () => tip.TimeParsers = parsers,
                () => Assert.AreEqual(tip.TimeParsers, tud.TimeParsers));
        }

        /// <summary>
        /// Tests that Format is flowed to TimeUpDown.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that Format is flowed to TimeUpDown.")]
        public virtual void ShouldBindFormatToTimeUpDown()
        {
            TimePicker tip = new TimePicker();
            TimeUpDown tud = null;

            TestAsync(
                tip,
                () => tud = ((Panel)VisualTreeHelper.GetChild(tip, 0)).FindName("TimeUpDown") as TimeUpDown,
                () => tip.Format = new CustomTimeFormat("mm:hh"),
                () => Assert.AreEqual(tip.Format, tud.Format));
        }

        /// <summary>
        /// Tests that Culture is flowed to TimeUpDown.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that Culture is flowed to TimeUpDown.")]
        public virtual void ShouldBindCultureToTimeUpDown()
        {
            TimePicker tip = new TimePicker();
            TimeUpDown tud = null;

            TestAsync(
                tip,
                () => tud = ((Panel)VisualTreeHelper.GetChild(tip, 0)).FindName("TimeUpDown") as TimeUpDown,
                () => tip.Culture = new CultureInfo("nl-NL"),
                () => Assert.AreEqual(tip.Culture, tud.Culture));
        }

        /// <summary>
        /// Tests that TimeGlobalizationInfo if flowed to TimeUpDown.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that TimeGlobalizationInfo if flowed to TimeUpDown.")]
        public virtual void ShouldBindTimeGlobalizationInfoToTimeUpDown()
        {
            TimePicker tip = new TimePicker();
            TimeUpDown tud = null;

            TestAsync(
                tip,
                () => tud = ((Panel)VisualTreeHelper.GetChild(tip, 0)).FindName("TimeUpDown") as TimeUpDown,
                () => tip.TimeGlobalizationInfo = new TimeGlobalizationInfo(),
                () => Assert.AreEqual(tip.TimeGlobalizationInfo, tud.TimeGlobalizationInfo));
        }

        /// <summary>
        /// Tests that ParseError is raised.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that ParseError is raised.")]
        public virtual void ShouldAllowParseErrorEventFromTimeUpDown()
        {
            OverriddenTimePicker tip = new OverriddenTimePicker(true);

            bool parseErrorRaised = false;

            OverriddenTimeUpDown tud = null;
            tip.ParseError += (sender, e) => parseErrorRaised = true;

            TestAsync(
                tip,
                () => tud = tip.OverriddenTimeUpDown, 
                () => tud.ApplyText("non parsable text should raise exception"),
                () => Assert.IsTrue(parseErrorRaised));
        }

        /// <summary>
        /// Tests custom parsing hook.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests custom parsing hook.")]
        public virtual void ShouldAllowCustomParsingFromTimeUpDown()
        {
            OverriddenTimePicker tip = new OverriddenTimePicker(true);
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();

            tip.Parsing += (sender, e) =>
            {
                e.Value = new DateTime(2000, 2, 2, 9, 10, 11);
                e.Handled = true;
            };

            TestAsync(
                tip,
                () => tud = tip.OverriddenTimeUpDown, 
                () => tud.Culture = new CultureInfo("en-US"),
                () => tud.ApplyText("random text"),
                () => Assert.AreEqual("9:10 AM", tud.DisplayTextBox.Text));
        }

        /// <summary>
        /// Tests that TimeUpDownStyle is applied.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that TimeUpDownStyle is applied.")]
        public virtual void ShouldApplyTimeUpDownStyle()
        {
            TimePicker tip = new TimePicker();
            TimeUpDown tud = null;

            Style style = new Style(typeof(TimeUpDown));
            style.Setters.Add(new Setter(TimeUpDown.TagProperty, "style is applied"));

            tip.TimeUpDownStyle = style;

            TestAsync(
                tip,
                () => tud = ((Panel)VisualTreeHelper.GetChild(tip, 0)).FindName("TimeUpDown") as TimeUpDown,
                () => Assert.AreEqual("style is applied", tud.Tag));
        }

        /// <summary>
        /// Tests that SpinnerStyle is applied.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that SpinnerStyle is applied.")]
        public virtual void ShouldApplySpinnerStyle()
        {
            TimePicker tip = new TimePicker();
            TimeUpDown tud = null;

            Style style = new Style(typeof(Spinner));
            style.Setters.Add(new Setter(Spinner.TagProperty, "style is applied"));

            TestAsync(
                tip,
                () => tip.SpinnerStyle = style,
                () => tud = ((Panel)VisualTreeHelper.GetChild(tip, 0)).FindName("TimeUpDown") as TimeUpDown,
                () => Assert.AreEqual(style, tud.SpinnerStyle));
        }

        /// <summary>
        /// Tests that value can be cleared correctly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Bug("bug 653791, TimePicker.Value property does not respond to DependencyProperty.ClearValue correctly. Postponed")]
        [Description("Tests that value can be cleared correctly.")]
        public virtual void ShouldBeAbleToClearValue()
        {
            TimePicker tip = new TimePicker();

            bool executed = false;
            bool loaded = false;

            tip.Loaded += (sender, args) => loaded = true;

            tip.LayoutUpdated += (sender, args) =>
            {
                if (executed)
                {
                    return;
                }
                executed = true;
                tip.Value = new DateTime();
                tip.ClearValue(TimePicker.ValueProperty);
                Assert.IsTrue(DependencyProperty.UnsetValue == tip.ReadLocalValue(TimePicker.ValueProperty));
            };

            EnqueueCallback(() => TestPanel.Children.Add(tip));
            EnqueueConditional(() => loaded);
            EnqueueCallback(() => executed = false);
            EnqueueCallback(tip.UpdateLayout);
            EnqueueConditional(() => executed);
            EnqueueTestComplete();
        }
        #endregion interaction with TimeUpDown

        #region data flow with popups
        /// <summary>
        /// Tests that interval defaults are pulled up from popup.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that interval defaults are pulled up from popup.")]
        public virtual void ShouldGetDefaultIntervalFromPopup()
        {
            TimePicker tip = new TimePicker();

            TestAsync(
                tip, 
                () => tip.IsDropDownOpen = true,
                () => Assert.AreEqual(0, tip.PopupSecondsInterval),
                () => Assert.AreEqual(30, tip.PopupMinutesInterval));
        }

        /// <summary>
        /// Tests that setting intervals on picker overwrites popupdefaults.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that setting intervals on picker overwrites popupdefaults.")]
        public virtual void ShouldOverwriteDefaultIntervalsFromPopup()
        {
            TimePicker tip = new TimePicker();

            tip.PopupSecondsInterval = 20;
            tip.PopupMinutesInterval = 30;

            TestAsync(
                tip, 
                () => Assert.AreEqual(20, tip.PopupSecondsInterval),
                () => Assert.AreEqual(30, tip.PopupMinutesInterval));
        }

        /// <summary>
        /// Tests that Value property is flowed to popup.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that Value property is flowed to popup.")]
        public virtual void ShouldBindValueToPopup()
        {
            OverriddenTimePicker tip = new OverriddenTimePicker();
            tip.Maximum = new DateTime(2000, 2, 2, 23, 1, 1);
            tip.Minimum = new DateTime(2002, 3, 3, 1, 0, 0);
            TimePickerPopup popup = null;

            tip.Value = new DateTime(1900, 1, 1, 12, 3, 2);
            TestAsync(
                tip,
                () => popup = tip.ActualTimePickerPopup as TimePickerPopup,
                () => tip.IsDropDownOpen = true,
                () => Assert.AreEqual(new DateTime(1900, 1, 1, 12, 3, 2), tip.Value),
                () => Assert.AreEqual(new DateTime(1900, 1, 1, 12, 0, 0), popup.Value),
                () => tip.Value = new DateTime(2000, 2, 2, 2, 2, 2),
                () => Assert.AreEqual(new DateTime(2000, 2, 2, 2, 2, 2), popup.Value));
        }

        /// <summary>
        /// Tests that Minimum property is flowed to popup.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that Minimum property is flowed to popup.")]
        public virtual void ShouldBindMinimumToPopup()
        {
            OverriddenTimePicker tip = new OverriddenTimePicker();
            tip.Maximum = new DateTime(2000, 2, 2, 16, 1, 1);
            tip.Minimum = new DateTime(2002, 3, 3, 2, 2, 2);
            TimePickerPopup popup = null;

            TestAsync(
                tip,
                () => popup = tip.ActualTimePickerPopup as TimePickerPopup,
                () => tip.IsDropDownOpen = true,
                () => Assert.AreEqual(tip.Minimum, popup.Minimum),
                () => tip.Minimum = new DateTime(2000, 2, 2, 2, 2, 2),
                () => Assert.AreEqual(new DateTime(2000, 2, 2, 2, 2, 2), popup.Minimum),
                () => popup.Minimum = new DateTime(2010, 3, 3, 3, 3, 3),
                () => Assert.AreEqual(new DateTime(2010, 3, 3, 3, 3, 3), tip.Minimum, "Twoway manual syc"));
        }

        /// <summary>
        /// Tests that Maximum property is flowed to popup.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that Maximum property is flowed to popup.")]
        public virtual void ShouldBindMaximumToPopup()
        {
            OverriddenTimePicker tip = new OverriddenTimePicker();

            tip.Maximum = new DateTime(1900, 1, 1, 12, 3, 2);
            TimePickerPopup popup = null;
            TestAsync(
                tip,
                () => popup = tip.ActualTimePickerPopup as TimePickerPopup,
                () => tip.IsDropDownOpen = true,
                () => Assert.AreEqual(tip.Maximum, popup.Maximum),
                () => tip.Maximum = new DateTime(2000, 2, 2, 2, 2, 2),
                () => Assert.AreEqual(new DateTime(2000, 2, 2, 2, 2, 2), popup.Maximum),
                () => popup.Maximum = new DateTime(2010, 3, 3, 3, 3, 3),
                () => Assert.AreEqual(new DateTime(2010, 3, 3, 3, 3, 3), tip.Maximum, "Twoway manual syc"));
        }

        /// <summary>
        /// Tests that Culture property is flowed to popup.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that Culture property is flowed to popup.")]
        public virtual void ShouldBindCultureToPopup()
        {
            OverriddenTimePicker tip = new OverriddenTimePicker();

            tip.Culture = new CultureInfo("nl-NL");
            TimePickerPopup popup = null;
            TestAsync(
                tip,
                () => popup = tip.ActualTimePickerPopup as TimePickerPopup,
                () => tip.IsDropDownOpen = true,
                () => Assert.AreSame(tip.Culture, popup.Culture),
                () => tip.Culture = new CultureInfo("en-US"),
                () => Assert.AreSame(tip.Culture, popup.Culture),
                () => popup.Culture = new CultureInfo("nl-NL"),
                () => Assert.AreSame(popup.Culture, tip.Culture, "Twoway manual syc"));
        }

        /// <summary>
        /// Tests that Format property is flowed to popup.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that Format property is flowed to popup.")]
        public virtual void ShouldBindFormatToPopup()
        {
            OverriddenTimePicker tip = new OverriddenTimePicker();

            TimePickerPopup popup = null;
            TestAsync(
                tip,
                () => popup = tip.ActualTimePickerPopup as TimePickerPopup,
                () => tip.IsDropDownOpen = true,
                () => Assert.AreSame(tip.Format, popup.Format),
                () => tip.Format = new LongTimeFormat(),
                () => Assert.AreEqual(tip.Format, popup.Format),
                () => popup.Format = new CustomTimeFormat("test"),
                () => Assert.IsTrue(tip.Format is CustomTimeFormat, "Twoway manual syc"));
        }

        /// <summary>
        /// Tests that TimeGlobalizationInfo property is flowed to popup.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that Culture property is flowed to popup.")]
        public virtual void ShouldBindTimeGlobalizationInfoToPopup()
        {
            OverriddenTimePicker tip = new OverriddenTimePicker();

            tip.TimeGlobalizationInfo = new TimeGlobalizationInfo();
            TimePickerPopup popup = null;
            TestAsync(
                tip,
                () => popup = tip.ActualTimePickerPopup as TimePickerPopup,
                () => tip.IsDropDownOpen = true,
                () => Assert.AreSame(tip.TimeGlobalizationInfo, popup.TimeGlobalizationInfo),
                () => tip.TimeGlobalizationInfo = new TimeGlobalizationInfo(),
                () => Assert.AreSame(tip.TimeGlobalizationInfo, popup.TimeGlobalizationInfo),
                () => popup.TimeGlobalizationInfo = new TimeGlobalizationInfo(),
                () => Assert.AreSame(popup.TimeGlobalizationInfo, tip.TimeGlobalizationInfo, "Twoway manual syc"));
        }

        /// <summary>
        /// Tests that PopupSecondsInterval property is flowed to popup.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that Culture property is flowed to popup.")]
        public virtual void ShouldBindPopupSecondsIntervalToPopup()
        {
            OverriddenTimePicker tip = new OverriddenTimePicker();

            tip.PopupSecondsInterval = 53;
            TimePickerPopup popup = null;
            TestAsync(
                tip,
                () => popup = tip.ActualTimePickerPopup as TimePickerPopup,
                () => tip.IsDropDownOpen = true,
                () => Assert.AreEqual(tip.PopupSecondsInterval, popup.PopupSecondsInterval),
                () => tip.PopupSecondsInterval = 16,
                () => Assert.AreEqual(tip.PopupSecondsInterval, popup.PopupSecondsInterval),
                () => popup.PopupSecondsInterval = 33,
                () => Assert.AreEqual(33, tip.PopupSecondsInterval, "Twoway manual syc"));
        }

        /// <summary>
        /// Tests that PopupMinutesInterval property is flowed to popup.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that Culture property is flowed to popup.")]
        public virtual void ShouldBindPopupMinutesIntervalToPopup()
        {
            OverriddenTimePicker tip = new OverriddenTimePicker();

            tip.PopupMinutesInterval = 53;
            TimePickerPopup popup = null;
            TestAsync(
                tip,
                () => popup = tip.ActualTimePickerPopup as TimePickerPopup,
                () => tip.IsDropDownOpen = true,
                () => Assert.AreEqual(tip.PopupMinutesInterval, popup.PopupMinutesInterval),
                () => tip.PopupMinutesInterval = 16,
                () => Assert.AreEqual(tip.PopupMinutesInterval, popup.PopupMinutesInterval),
                () => popup.PopupMinutesInterval = 33,
                () => Assert.AreEqual(33, tip.PopupMinutesInterval, "Twoway manual syc"));
        }

        /// <summary>
        /// Tests that PopupTimeSelection binds to the popup.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that PopupTimeSelection binds to the popup.")]
        public virtual void ShouldBindPopupTimeSelectionToPopup()
        {
            OverriddenTimePicker tip = new OverriddenTimePicker();
            // todo: the enum values that are commented out will be included after mix.
            tip.Popup = new RangeTimePickerPopup();
            tip.PopupTimeSelectionMode = PopupTimeSelectionMode.AllowSecondsSelection;
            TimePickerPopup popup = null;
            TestAsync(
                tip,
                () => popup = tip.ActualTimePickerPopup as TimePickerPopup,
                () => tip.IsDropDownOpen = true,
                () => Assert.AreEqual(PopupTimeSelectionMode.AllowSecondsSelection, popup.PopupTimeSelectionMode),
                // () => tip.PopupTimeSelectionMode = PopupTimeSelectionMode.AllowSecondsAndDesignatorsSelection,
                // () => Assert.AreEqual(PopupTimeSelectionMode.AllowSecondsAndDesignatorsSelection, popup.PopupTimeSelectionMode),
                () => popup.PopupTimeSelectionMode = PopupTimeSelectionMode.HoursAndMinutesOnly,
                () => Assert.AreEqual(PopupTimeSelectionMode.HoursAndMinutesOnly, tip.PopupTimeSelectionMode, "Twoway manual syc"));
        }

        /// <summary>
        /// Tests that registering a popup will check PopupTimeSelectionMode.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that registering a popup will check PopupTimeSelectionMode.")]
        public virtual void ShouldCoercePopupTimeSelectionModeWhenRegisteringPopup()
        {
            OverriddenTimePicker tip = new OverriddenTimePicker();
            tip.PopupTimeSelectionMode = PopupTimeSelectionMode.AllowSecondsSelection;

            TestAsync(
                    tip, // this will register a popup through PopupTemplate, and silently coerce
                    () => Assert.AreEqual(PopupTimeSelectionMode.HoursAndMinutesOnly, tip.PopupTimeSelectionMode));
        }

        /// <summary>
        /// Tests that setting PopupTimeSelectionMode works when instantiating with PopupTemplate.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that setting PopupTimeSelectionMode works when instantiating default.")]
        public virtual void ShouldAllowSettingPopupTimeSelectionModeWithPopupTemplate()
        {
            TimePicker tip = new TimePicker();
            tip.PopupTimeSelectionMode = PopupTimeSelectionMode.AllowSecondsSelection;
            tip.Style = LoadXamlResource(DefaultStylesResource)["ShouldAllowSettingPopupTimeSelectionModeWithPopupTemplate"] as Style;
            Assert.IsNotNull(tip.Style);

            TimePicker tip2 = new TimePicker();
            tip2.Style = LoadXamlResource(DefaultStylesResource)["ShouldAllowSettingPopupTimeSelectionModeWithPopupTemplateB"] as Style;
            Assert.IsNotNull(tip2.Style);

            TestAsync(
                tip,
                () => Assert.AreEqual(PopupTimeSelectionMode.AllowSecondsSelection, tip.PopupTimeSelectionMode),
                () => Assert.AreEqual(PopupTimeSelectionMode.AllowSecondsSelection, tip2.PopupTimeSelectionMode));
        }

        /// <summary>
        /// Tests that setting PopupTimeSelectionMode works when instantiating with Popup.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that setting PopupTimeSelectionMode works when instantiating with Popup.")]
        public virtual void ShouldAllowSettingPopupTimeSelectionModeWithPopup()
        {
            ContentControl cc = new ContentControl();
            cc.Style = LoadXamlResource(DefaultStylesResource)["ShouldAllowSettingPopupTimeSelectionModeWithPopup"] as Style;

            Assert.IsNotNull(cc.Style);

            TimePicker tp = null;

            TestAsync(
                cc,
                () => tp = VisualTreeHelper.GetChild(cc, 0) as TimePicker,
                () => Assert.AreEqual(PopupTimeSelectionMode.AllowSecondsSelection, tp.PopupTimeSelectionMode));
        }
        #endregion data flow with popups

        #region instantiation popups
        /// <summary>
        /// Tests that an exception is thrown when the Popup is styled.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [ExpectedException(typeof(ArgumentException))]
        [Description("Tests that a nice exception is thrown when the Popup is styled.")]
        public virtual void ShouldThrowExceptionWhenPopupIsStyled()
        {
            TimePicker tip = new TimePicker();
            Style s = new Style(typeof(TimePicker));
            s.Setters.Add(new Setter(TimePicker.PopupProperty, new RangeTimePickerPopup()));

            EnqueueCallback(() => tip.Style = s);
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests that Popup can not be set from Xaml.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [ExpectedException(typeof(ArgumentException))]
        [Description("Tests that Popup can not be set from Xaml.")]
        public virtual void ShouldThrowExceptionWhenPopupIsStyledFromXaml()
        {
            TimePicker tip = new TimePicker();
            // todo: use currentmethodname when ignite has opened that up.
            Style s = (Style)LoadXamlResource(DefaultStylesResource)["ShouldThrowExceptionWhenPopupIsStyledFromXaml"];

            Assert.IsNotNull(s);

            EnqueueCallback(() => tip.Style = s);
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests that PopupTemplate can be set from Xaml.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that PopupTemplate can be set from xaml.")]
        public virtual void ShouldAllowSettingPopupTemplateFromXaml()
        {
            OverriddenTimePicker tip = new OverriddenTimePicker(false);
            // todo: use currentmethodname when ignite has opened that up.
            tip.Style = LoadXamlResource(DefaultStylesResource)["ShouldAllowSettingPopupTemplateFromXaml"] as Style;

            Assert.IsNotNull(tip.Style);

            TestAsync(
                tip,
                () => Assert.IsTrue(tip.ActualTimePickerPopup is RangeTimePickerPopup));
        }

        /// <summary>
        /// Tests Popup versus PopupTemplate.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests Popup versus PopupTemplate.")]
        public virtual void ShouldLetPopupWinFromPopupTemplate()
        {
            OverriddenTimePicker tip = new OverriddenTimePicker();

            TestAsync(
                    tip,
                    () => Assert.IsInstanceOfType(tip.ActualTimePickerPopup, typeof(ListTimePickerPopup)),
                    () => tip.Popup = new RangeTimePickerPopup()
                                          {
                                                  Tag = "1"
                                          },
                    () => Assert.AreEqual("1", tip.ActualTimePickerPopup.Tag),
                    () => tip.Popup = null,
                    () => Assert.IsInstanceOfType(tip.ActualTimePickerPopup, typeof(ListTimePickerPopup)));
        }

        /// <summary>
        /// Tests that clearing the popup brings back the original.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that clearing the popup brings back the original.")]
        public virtual void ShouldRevertToOriginalPopupAfterClearing()
        {
            TimePicker tp = new TimePicker();

            TimePickerPopup original = null;

            TestAsync(
                    tp,
                    () => original = tp.Popup,
                    () => tp.Popup = new ListTimePickerPopup(),
                    () => tp.ClearValue(TimePicker.PopupProperty),
                    () => Assert.AreEqual(original, tp.Popup));
        }
        #endregion
    }
}