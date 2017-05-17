// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Tests for TimePickerPopup.
    /// </summary>
    public abstract class TimePickerPopupBaseTest : ControlTest
    {
        #region Instances to test
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        /// <value></value>
        public override Control DefaultControlToTest
        {
            get { return DefaultTimePickerPopupToTest; }
        }

        /// <summary>
        /// Gets instances of Control (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<Control> ControlsToTest
        {
            get { return (IEnumerable<Control>)TimePickerPopupInstancesToTest; }
        }

        /// <summary>
        /// Gets instances of IOverriddenControl (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<IOverriddenControl> OverriddenControlsToTest
        {
            get { return OverriddenTimePickerPopupInstancesToTest; }
        }

        /// <summary>
        /// Gets the default time picker popup to test.
        /// </summary>
        /// <value>The default time picker popup to test.</value>
        public abstract TimePickerPopup DefaultTimePickerPopupToTest { get; }

        /// <summary>
        /// Gets the time picker popups to test.
        /// </summary>
        /// <value>The time picker popups to test.</value>
        public abstract IEnumerable<TimePickerPopup> TimePickerPopupInstancesToTest { get; }

        /// <summary>
        /// Gets the overridden time picker popups to test.
        /// </summary>
        /// <value>The overridden time picker popups to test.</value>
        public abstract IEnumerable<IOverriddenControl> OverriddenTimePickerPopupInstancesToTest { get; }
        #endregion Instances to test

        #region Dependency properties
        /// <summary>
        /// Gets the value property.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Value is nullable DateTime.")]
        protected DependencyPropertyTest<TimePickerPopup, DateTime?> ValueProperty { get; private set; }

        /// <summary>
        /// Gets the minimum property.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Value is nullable DateTime.")]
        protected DependencyPropertyTest<TimePickerPopup, DateTime?> MinimumProperty { get; private set; }

        /// <summary>
        /// Gets the maximum property.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Value is nullable DateTime.")]
        protected DependencyPropertyTest<TimePickerPopup, DateTime?> MaximumProperty { get; private set; }

        /// <summary>
        /// Gets the display format property.
        /// </summary>
        protected DependencyPropertyTest<TimePickerPopup, ITimeFormat> FormatProperty { get; private set; }

        /// <summary>
        /// Gets the culture property.
        /// </summary>
        protected DependencyPropertyTest<TimePickerPopup, CultureInfo> CultureProperty { get; private set; }

        /// <summary>
        /// Gets the time globalization info property.
        /// </summary>
        protected DependencyPropertyTest<TimePickerPopup, TimeGlobalizationInfo> TimeGlobalizationInfoProperty { get; private set; }

        /// <summary>
        /// Gets the popup seconds interval property.
        /// </summary>
        protected DependencyPropertyTest<TimePickerPopup, int> PopupSecondsIntervalProperty { get; private set; }

        /// <summary>
        /// Gets the popup minutes interval property.
        /// </summary>
        protected DependencyPropertyTest<TimePickerPopup, int> PopupMinutesIntervalProperty { get; private set; }

        /// <summary>
        /// Gets the popup time selection mode property.
        /// </summary>
        protected DependencyPropertyTest<TimePickerPopup, PopupTimeSelectionMode> PopupTimeSelectionModeProperty { get; private set; }

        #endregion Dependency properties

        /// <summary>
        /// Initializes a new instance of the <see cref="TimePickerPopupBaseTest"/> class.
        /// </summary>
        protected TimePickerPopupBaseTest()
        {
            Func<TimePickerPopup> initializer = () => DefaultTimePickerPopupToTest;

            ValueProperty = new DependencyPropertyTest<TimePickerPopup, DateTime?>(this, "Value")
            {
                Property = TimePickerPopup.ValueProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new DateTime?(), new DateTime(2000, 12, 3), new DateTime(1900, 1, 1) },
            };

            MinimumProperty = new DependencyPropertyTest<TimePickerPopup, DateTime?>(this, "Minimum")
            {
                Property = TimePickerPopup.MinimumProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new DateTime?(), new DateTime(2000, 12, 3), new DateTime(1900, 1, 1) },
            };

            MaximumProperty = new DependencyPropertyTest<TimePickerPopup, DateTime?>(this, "Maximum")
            {
                Property = TimePickerPopup.MaximumProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new DateTime?(), new DateTime(2000, 12, 3), new DateTime(1900, 1, 1) },
            };

            FormatProperty = new DependencyPropertyTest<TimePickerPopup, ITimeFormat>(this, "Format")
            {
                Property = TimePickerPopup.FormatProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new ITimeFormat[] { new ShortTimeFormat(), new LongTimeFormat(), new CustomTimeFormat("hh:ss") },
            };

            CultureProperty = new DependencyPropertyTest<TimePickerPopup, CultureInfo>(this, "Culture")
            {
                Property = TimePickerPopup.CultureProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new CultureInfo("en-US"), new CultureInfo("nl-NL") }
            };

            TimeGlobalizationInfoProperty = new DependencyPropertyTest<TimePickerPopup, TimeGlobalizationInfo>(this, "TimeGlobalizationInfo")
            {
                Property = TimePickerPopup.TimeGlobalizationInfoProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new TimeGlobalizationInfo() }
            };

            PopupSecondsIntervalProperty = new DependencyPropertyTest<TimePickerPopup, int>(this, "PopupSecondsInterval")
            {
                Property = TimePickerPopup.PopupSecondsIntervalProperty,
                Initializer = initializer,
                DefaultValue = 0,
                OtherValues = new[] { 10, 20, 59 },
                InvalidValues = new Dictionary<int, Type>()
                                    {
                                        { -10, typeof(ArgumentOutOfRangeException) },
                                        { -1, typeof(ArgumentOutOfRangeException) },
                                        { -21311, typeof(ArgumentOutOfRangeException) },
                                    }
            };

            PopupMinutesIntervalProperty = new DependencyPropertyTest<TimePickerPopup, int>(this, "PopupMinutesInterval")
            {
                Property = TimePickerPopup.PopupMinutesIntervalProperty,
                Initializer = initializer,
                DefaultValue = 30,
                OtherValues = new[] { 10, 20, 59 },
                InvalidValues = new Dictionary<int, Type>()
                                    {
                                        { -10, typeof(ArgumentOutOfRangeException) },
                                        { -1, typeof(ArgumentOutOfRangeException) },
                                        { -21311, typeof(ArgumentOutOfRangeException) },
                                    }
            };
            // todo: the enum values that are commented out will be included after mix.
            PopupTimeSelectionModeProperty = new DependencyPropertyTest<TimePickerPopup, PopupTimeSelectionMode>(this, "PopupTimeSelectionMode")
            {
                Property = TimePickerPopup.PopupTimeSelectionModeProperty,
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

            // TimePickerFormatProperty tests
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
            tests.Add(PopupTimeSelectionModeProperty.InvalidValueFailsTest);
            tests.Add(PopupTimeSelectionModeProperty.InvalidValueIsIgnoredTest);
            // tests.Add(PopupTimeSelectionModeProperty.ClearValueResetsDefaultTest);
            // tests.Add(PopupTimeSelectionModeProperty.CanBeStyledTest);
            // tests.Add(PopupTimeSelectionModeProperty.TemplateBindTest);
            tests.Add(PopupTimeSelectionModeProperty.SetXamlAttributeTest);
            tests.Add(PopupTimeSelectionModeProperty.SetXamlElementTest);

            return tests;
        }

        #region Coercion
        /// <summary>
        /// Breadth test on interaction between minimum maximum and value.
        /// </summary>
        [TestMethod]
        [Description("Breadth test on interaction between minimum maximum and value")]
        public virtual void ShouldCoerceMinimumMaximumAndValue()
        {
            TimePickerPopup tpp = DefaultTimePickerPopupToTest;

            tpp.Value = new DateTime(2100, 1, 1, 14, 0, 0);
            tpp.Minimum = new DateTime(2100, 1, 1, 14, 0, 0);
            tpp.Maximum = new DateTime(2100, 1, 1, 16, 0, 0);

            tpp.Minimum = tpp.Minimum.Value.AddMinutes(30.0);
            Assert.AreEqual(new DateTime(2100, 1, 1, 14, 30, 0), tpp.Minimum.Value);
            Assert.AreEqual(tpp.Minimum, tpp.Value);
            tpp.Value = tpp.Maximum.Value;
            tpp.Maximum = tpp.Maximum.Value.AddMinutes(-30.0);
            Assert.AreEqual(new DateTime(2100, 1, 1, 15, 30, 0), tpp.Maximum.Value);
            Assert.AreEqual(tpp.Maximum, tpp.Value);
        }

        /// <summary>
        /// Tests setting ranges.
        /// </summary>
        [TestMethod]
        [Description("Tests setting ranges.")]
        public virtual void ShouldBeAbleToSetInvalidBoundaries()
        {
            TimePickerPopup tpp = DefaultTimePickerPopupToTest;

            tpp.Value = new DateTime(2100, 1, 1, 14, 0, 0);
            tpp.Minimum = new DateTime(2100, 1, 1, 14, 0, 0);
            tpp.Maximum = new DateTime(2100, 1, 1, 16, 0, 0);

            tpp.Maximum = new DateTime(2100, 1, 1, 11, 0, 0);

            Assert.AreEqual(new DateTime(2100, 1, 1, 11, 0, 0), tpp.Minimum);
            Assert.AreEqual(new DateTime(2100, 1, 1, 11, 0, 0), tpp.Maximum);
            Assert.AreEqual(new DateTime(2100, 1, 1, 11, 0, 0), tpp.Value);

            tpp.Minimum = new DateTime(2100, 1, 1, 13, 0, 0);
            Assert.AreEqual(new DateTime(2100, 1, 1, 13, 0, 0), tpp.Minimum);
            Assert.AreEqual(new DateTime(2100, 1, 1, 13, 0, 0), tpp.Maximum);
            Assert.AreEqual(new DateTime(2100, 1, 1, 13, 0, 0), tpp.Value);
        }

        /// <summary>
        /// Tests that min, max and value do not care for date part.
        /// </summary>
        [TestMethod]
        [Description("Tests that min, max and value do not care for date part.")]
        public virtual void ShouldDisregardDatePartWhenCoercing()
        {
            TimePickerPopup tpp = DefaultTimePickerPopupToTest;

            tpp.Value = new DateTime(2100, 1, 1, 15, 0, 0);
            tpp.Minimum = new DateTime(2100, 1, 1, 14, 0, 0);
            tpp.Maximum = new DateTime(2100, 1, 1, 16, 0, 0);

            tpp.Value = tpp.Value.Value.AddDays(2);
            tpp.Maximum = tpp.Maximum.Value.AddMinutes(-90);
            Assert.AreEqual(new DateTime(2100, 1, 3, 14, 30, 0), tpp.Value.Value);
            tpp.Value = tpp.Value.Value.AddYears(-1);
            Assert.AreEqual(new DateTime(2099, 1, 3, 14, 30, 0), tpp.Value.Value);
        }

        /// <summary>
        /// Tests that last valid value is used when value is set outside of valid range.
        /// </summary>
        [TestMethod]
        [Description("Tests that last valid value is used when value is set outside of valid range.")]
        public virtual void ShouldRevertToLastValidValueWhenSetOutsideRange()
        {
            TimePickerPopup tpp = DefaultTimePickerPopupToTest;

            tpp.Value = new DateTime(2100, 1, 1, 15, 0, 0);
            tpp.Minimum = new DateTime(2100, 1, 1, 14, 0, 0);
            tpp.Maximum = new DateTime(2100, 1, 1, 16, 0, 0);

            tpp.Value = new DateTime(2100, 1, 1, 10, 0, 0);
            Assert.AreEqual(new DateTime(2100, 1, 1, 15, 0, 0), tpp.Value.Value);
        }
        #endregion

        /// <summary>
        /// Tests that visual state changes when Container is set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that visual state changes when Container is set.")]
        public virtual void ShouldChangeVisualStateWhenContainerIsSet()
        {
            TimePicker tip = new TimePicker();
            TimePickerPopup tpp = DefaultTimePickerPopupToTest;

            TestVisualStateManager vsmTpp = new TestVisualStateManager();

            bool tipIsLoaded = false;
            tip.Loaded += delegate { tipIsLoaded = true; };

            bool tppIsLoaded = false;
            tpp.Loaded += delegate { tppIsLoaded = true; };

            // Add the element to the test surface and wait until it's loaded
            EnqueueCallback(() => TestPanel.Children.Add(tip));
            EnqueueCallback(() => TestPanel.Children.Add(tpp));
            EnqueueConditional(() => tipIsLoaded);
            EnqueueConditional(() => tppIsLoaded);

            EnqueueCallback(() =>
                                {
                                    FrameworkElement root2 = tpp.GetVisualDescendents().First() as FrameworkElement;
                                    VisualStateManager.SetCustomVisualStateManager(root2, vsmTpp);
                                });
            EnqueueCallback(() =>
                                {
                                    TestPanel.Children.Remove(tpp);
                                    tip.Popup = tpp;
                                });
            EnqueueCallback(() => Assert.IsTrue(vsmTpp.ChangedStates.Contains("Contained")));

            EnqueueTestComplete();
        }
    }
}