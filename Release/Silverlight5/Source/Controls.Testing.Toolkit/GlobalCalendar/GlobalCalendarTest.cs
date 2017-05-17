// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// GlobalCalendar unit tests.
    /// </summary>
    [TestClass]
    [Tag("GlobalCalendar")]
    public partial class GlobalCalendarTest : ControlTest
    {
        #region Controls to test
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get { return DefaultGlobalCalendarToTest; }
        }

        /// <summary>
        /// Gets instances of Control (or derived types) to test.
        /// </summary>
        public override IEnumerable<Control> ControlsToTest
        {
            get { return GlobalCalendarsToTest.OfType<Control>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenControl> OverriddenControlsToTest
        {
            get { yield break; }
        }
        #endregion Controls to test

        #region GlobalCalendars to test
        /// <summary>
        /// Gets a default instance of GlobalCalendar (or a derived type) to test.
        /// </summary>
        public virtual GlobalCalendar DefaultGlobalCalendarToTest
        {
            get { return new GlobalCalendar(); }
        }

        /// <summary>
        /// Gets instances of GlobalCalendar (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<GlobalCalendar> GlobalCalendarsToTest
        {
            get
            {
                yield return DefaultGlobalCalendarToTest;
                yield return new GlobalCalendar()
                {
                    CalendarInfo = new CultureCalendarInfo(new CultureInfo("th-TH"))
                };
            }
        }
        #endregion GlobalCalendars to test

        /// <summary>
        /// Gets the CalendarButtonStyle dependency property test.
        /// </summary>
        protected DependencyPropertyTest<GlobalCalendar, Style> CalendarButtonStyleProperty { get; private set; }

        /// <summary>
        /// Gets the CalendarDayButtonStyle dependency property test.
        /// </summary>
        protected DependencyPropertyTest<GlobalCalendar, Style> CalendarDayButtonStyleProperty { get; private set; }

        /// <summary>
        /// Gets the CalendarItemStyle dependency property test.
        /// </summary>
        protected DependencyPropertyTest<GlobalCalendar, Style> CalendarItemStyleProperty { get; private set; }

        /// <summary>
        /// Gets the IsTodayHighlighted dependency property test.
        /// </summary>
        protected DependencyPropertyTest<GlobalCalendar, bool> IsTodayHighlightedProperty { get; private set; }

        /// <summary>
        /// Gets the DisplayMode dependency property test.
        /// </summary>
        protected DependencyPropertyTest<GlobalCalendar, CalendarMode> DisplayModeProperty { get; private set; }

        /// <summary>
        /// Gets the FirstDayOfWeek dependency property test.
        /// </summary>
        protected DependencyPropertyTest<GlobalCalendar, DayOfWeek> FirstDayOfWeekProperty { get; private set; }

        /// <summary>
        /// Gets the SelectionMode dependency property test.
        /// </summary>
        protected DependencyPropertyTest<GlobalCalendar, CalendarSelectionMode> SelectionModeProperty { get; private set; }

        /// <summary>
        /// Gets the SelectedDate dependency property test.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "For Testing.")]
        protected DependencyPropertyTest<GlobalCalendar, DateTime?> SelectedDateProperty { get; private set; }

        /// <summary>
        /// Gets the DisplayDate dependency property test.
        /// </summary>
        protected DependencyPropertyTest<GlobalCalendar, DateTime> DisplayDateProperty { get; private set; }

        /// <summary>
        /// Gets the DisplayDateStart dependency property test.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "For Testing.")]
        protected DependencyPropertyTest<GlobalCalendar, DateTime?> DisplayDateStartProperty { get; private set; }

        /// <summary>
        /// Gets the DisplayDateEnd dependency property test.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "For Testing.")]
        protected DependencyPropertyTest<GlobalCalendar, DateTime?> DisplayDateEndProperty { get; private set; }

        /// <summary>
        /// Gets the CalendarDayButtonStyleSelector dependency property test.
        /// </summary>
        protected DependencyPropertyTest<GlobalCalendar, CalendarDayButtonStyleSelector> CalendarDayButtonStyleSelectorProperty { get; private set; }

        /// <summary>
        /// Gets the CalendarInfo dependency property test.
        /// </summary>
        protected DependencyPropertyTest<GlobalCalendar, CalendarInfo> CalendarInfoProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the GlobalCalendarTest class.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "There are a lot of types involevd with GlobalCalendar.")]
        public GlobalCalendarTest()
        {
            LinearGradientBrush backgroundBrush = new LinearGradientBrush
            {
                StartPoint = new Point(.5, 0),
                EndPoint = new Point(.5, 1)
            };
            backgroundBrush.GradientStops.Add(new GradientStop { Offset = 0, Color = Color.FromArgb(0xFF, 0xD3, 0xDE, 0xE8) });
            backgroundBrush.GradientStops.Add(new GradientStop { Offset = 0.16, Color = Color.FromArgb(0xFF, 0xD3, 0xDE, 0xE8) });
            backgroundBrush.GradientStops.Add(new GradientStop { Offset = 0.16, Color = Color.FromArgb(0xFF, 0xFC, 0xFC, 0xFD) });
            backgroundBrush.GradientStops.Add(new GradientStop { Offset = 1, Color = Colors.White });
            BackgroundProperty.DefaultValue = backgroundBrush;

            LinearGradientBrush borderBrush = new LinearGradientBrush
            {
                StartPoint = new Point(.5, 0),
                EndPoint = new Point(.5, 1)
            };
            borderBrush.GradientStops.Add(new GradientStop { Offset = 0, Color = Color.FromArgb(0xFF, 0xA3, 0xAE, 0xB9) });
            borderBrush.GradientStops.Add(new GradientStop { Offset = 0.375, Color = Color.FromArgb(0xFF, 0x83, 0x99, 0xA9) });
            borderBrush.GradientStops.Add(new GradientStop { Offset = 0.375, Color = Color.FromArgb(0xFF, 0x71, 0x85, 0x97) });
            borderBrush.GradientStops.Add(new GradientStop { Offset = 1, Color = Color.FromArgb(0xFF, 0x61, 0x75, 0x84) });
            BorderBrushProperty.DefaultValue = borderBrush;

            BorderThicknessProperty.DefaultValue = new Thickness(1);

            Func<GlobalCalendar> initializer = () => DefaultGlobalCalendarToTest;
            CalendarButtonStyleProperty = new DependencyPropertyTest<GlobalCalendar, Style>(this, "CalendarButtonStyle")
            {
                Property = GlobalCalendar.CalendarButtonStyleProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new Style[] { new Style { TargetType = typeof(GlobalCalendarButton) }, new Style { TargetType = typeof(Control) } },
                InvalidValues = new Dictionary<Style, Type> { { new Style { TargetType = typeof(CalendarButton) }, typeof(ArgumentException) } }
            };
            CalendarDayButtonStyleProperty = new DependencyPropertyTest<GlobalCalendar, Style>(this, "CalendarDayButtonStyle")
            {
                Property = GlobalCalendar.CalendarDayButtonStyleProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new Style[] { new Style { TargetType = typeof(GlobalCalendarDayButton) }, new Style { TargetType = typeof(Control) } },
                InvalidValues = new Dictionary<Style, Type> { { new Style { TargetType = typeof(CalendarButton) }, typeof(ArgumentException) } }
            };
            CalendarItemStyleProperty = new DependencyPropertyTest<GlobalCalendar, Style>(this, "CalendarItemStyle")
            {
                Property = GlobalCalendar.CalendarItemStyleProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new Style[] { new Style { TargetType = typeof(GlobalCalendarItem) }, new Style { TargetType = typeof(Control) } },
                InvalidValues = new Dictionary<Style, Type> { { new Style { TargetType = typeof(CalendarItem) }, typeof(ArgumentException) } }
            };
            IsTodayHighlightedProperty = new DependencyPropertyTest<GlobalCalendar, bool>(this, "IsTodayHighlighted")
            {
                Property = GlobalCalendar.IsTodayHighlightedProperty,
                Initializer = initializer,
                DefaultValue = true,
                OtherValues = new bool[] { false },
                InvalidValues = new Dictionary<bool, Type> { }
            };
            DisplayModeProperty = new DependencyPropertyTest<GlobalCalendar, CalendarMode>(this, "DisplayMode")
            {
                Property = GlobalCalendar.DisplayModeProperty,
                Initializer = initializer,
                DefaultValue = CalendarMode.Month,
                OtherValues = new CalendarMode[] { CalendarMode.Decade, CalendarMode.Year },
                InvalidValues = new Dictionary<CalendarMode, Type>
                    {
                        { (CalendarMode)(-1), typeof(ArgumentException) },
                        { (CalendarMode)3, typeof(ArgumentException) },
                        { (CalendarMode)100, typeof(ArgumentException) }
                    }
            };
            FirstDayOfWeekProperty = new DependencyPropertyTest<GlobalCalendar, DayOfWeek>(this, "FirstDayOfWeek")
            {
                Property = GlobalCalendar.FirstDayOfWeekProperty,
                Initializer = initializer,
                DefaultValue = DayOfWeek.Sunday,
                OtherValues = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday },
                InvalidValues = new Dictionary<DayOfWeek, Type>
                    {
                        { (DayOfWeek)(-1), typeof(ArgumentOutOfRangeException) },
                        { (DayOfWeek)7, typeof(ArgumentOutOfRangeException) },
                        { (DayOfWeek)100, typeof(ArgumentOutOfRangeException) }
                    }
            };
            SelectionModeProperty = new DependencyPropertyTest<GlobalCalendar, CalendarSelectionMode>(this, "SelectionMode")
            {
                Property = GlobalCalendar.SelectionModeProperty,
                Initializer = initializer,
                DefaultValue = CalendarSelectionMode.SingleDate,
                OtherValues = new CalendarSelectionMode[] { CalendarSelectionMode.MultipleRange, CalendarSelectionMode.None, CalendarSelectionMode.SingleRange },
                InvalidValues = new Dictionary<CalendarSelectionMode, Type>
                    {
                        { (CalendarSelectionMode)(-1), typeof(ArgumentOutOfRangeException) },
                        { (CalendarSelectionMode)4, typeof(ArgumentOutOfRangeException) },
                        { (CalendarSelectionMode)100, typeof(ArgumentOutOfRangeException) }
                    }
            };
            SelectedDateProperty = new DependencyPropertyTest<GlobalCalendar, DateTime?>(this, "SelectedDate")
            {
                Property = GlobalCalendar.SelectedDateProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new DateTime?[] { DateTime.Today, DateTime.Today.AddDays(7), DateTime.Today.AddDays(-7) },
                InvalidValues = new Dictionary<DateTime?, Type> { }
            };
            DisplayDateProperty = new DependencyPropertyTest<GlobalCalendar, DateTime>(this, "DisplayDate")
            {
                Property = GlobalCalendar.DisplayDateProperty,
                Initializer = initializer,
                DefaultValue = DateTime.Today,
                OtherValues = new DateTime[] { DateTime.Today.AddDays(31), DateTime.Today.AddDays(-31) },
                InvalidValues = new Dictionary<DateTime, Type> { }
            };
            DisplayDateStartProperty = new DependencyPropertyTest<GlobalCalendar, DateTime?>(this, "DisplayDateStart")
            {
                Property = GlobalCalendar.DisplayDateStartProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new DateTime?[] { DateTime.Today, DateTime.Today.AddDays(31), DateTime.Today.AddDays(-31) },
                InvalidValues = new Dictionary<DateTime?, Type> { }
            };
            DisplayDateEndProperty = new DependencyPropertyTest<GlobalCalendar, DateTime?>(this, "DisplayDateEnd")
            {
                Property = GlobalCalendar.DisplayDateEndProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new DateTime?[] { DateTime.Today, DateTime.Today.AddDays(31), DateTime.Today.AddDays(-31) },
                InvalidValues = new Dictionary<DateTime?, Type> { }
            };
            CalendarDayButtonStyleSelectorProperty = new DependencyPropertyTest<GlobalCalendar, CalendarDayButtonStyleSelector>(this, "CalendarDayButtonStyleSelector")
            {
                Property = GlobalCalendar.CalendarDayButtonStyleSelectorProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new CalendarDayButtonStyleSelector[] { null },
                InvalidValues = new Dictionary<CalendarDayButtonStyleSelector, Type> { }
            };
            CalendarInfoProperty = new DependencyPropertyTest<GlobalCalendar, CalendarInfo>(this, "CalendarInfo")
            {
                Property = GlobalCalendar.CalendarInfoProperty,
                Initializer = initializer,
                DefaultValue = new GregorianCalendarInfo(CultureInfo.CurrentCulture),
                OtherValues = new CalendarInfo[] { new CultureCalendarInfo(new CultureInfo("en-US")), new CultureCalendarInfo(new CultureInfo("th-TH")) },
                InvalidValues = new Dictionary<CalendarInfo, Type> { }
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

            // CalendarButtonStyleProperty tests
            tests.Add(CalendarButtonStyleProperty.CheckDefaultValueTest);
            tests.Add(CalendarButtonStyleProperty.ChangeClrSetterTest);
            tests.Add(CalendarButtonStyleProperty.ChangeSetValueTest);
            tests.Add(CalendarButtonStyleProperty.SetNullTest);
            tests.Add(CalendarButtonStyleProperty.ClearValueResetsDefaultTest);
            tests.Add(CalendarButtonStyleProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.Add(CalendarButtonStyleProperty.CanBeStyledTest);
            tests.Add(CalendarButtonStyleProperty.TemplateBindTest);
            tests.Add(CalendarButtonStyleProperty.BindingTest);

            // CalendarDayButtonStyleProperty tests
            tests.Add(CalendarDayButtonStyleProperty.CheckDefaultValueTest);
            tests.Add(CalendarDayButtonStyleProperty.ChangeClrSetterTest);
            tests.Add(CalendarDayButtonStyleProperty.ChangeSetValueTest);
            tests.Add(CalendarDayButtonStyleProperty.SetNullTest);
            tests.Add(CalendarDayButtonStyleProperty.ClearValueResetsDefaultTest);
            tests.Add(CalendarDayButtonStyleProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.Add(CalendarDayButtonStyleProperty.CanBeStyledTest);
            tests.Add(CalendarDayButtonStyleProperty.TemplateBindTest);
            tests.Add(CalendarDayButtonStyleProperty.BindingTest);

            // CalendarItemStyleProperty tests
            tests.Add(CalendarItemStyleProperty.CheckDefaultValueTest);
            tests.Add(CalendarItemStyleProperty.ChangeClrSetterTest);
            tests.Add(CalendarItemStyleProperty.ChangeSetValueTest);
            tests.Add(CalendarItemStyleProperty.SetNullTest);
            tests.Add(CalendarItemStyleProperty.ClearValueResetsDefaultTest);
            tests.Add(CalendarItemStyleProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.Add(CalendarItemStyleProperty.CanBeStyledTest);
            tests.Add(CalendarItemStyleProperty.TemplateBindTest);
            tests.Add(CalendarItemStyleProperty.BindingTest);

            // IsTodayHighlightedProperty tests
            tests.Add(IsTodayHighlightedProperty.CheckDefaultValueTest);
            tests.Add(IsTodayHighlightedProperty.ChangeClrSetterTest);
            tests.Add(IsTodayHighlightedProperty.ChangeSetValueTest);
            tests.Add(IsTodayHighlightedProperty.ClearValueResetsDefaultTest);
            tests.Add(IsTodayHighlightedProperty.InvalidValueFailsTest);
            tests.Add(IsTodayHighlightedProperty.InvalidValueIsIgnoredTest);
            tests.Add(IsTodayHighlightedProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.Add(IsTodayHighlightedProperty.CanBeStyledTest);
            tests.Add(IsTodayHighlightedProperty.TemplateBindTest);
            tests.Add(IsTodayHighlightedProperty.BindingTest);
            tests.Add(IsTodayHighlightedProperty.SetXamlAttributeTest);

            // DisplayModeProperty tests
            tests.Add(DisplayModeProperty.CheckDefaultValueTest);
            tests.Add(DisplayModeProperty.ChangeClrSetterTest);
            tests.Add(DisplayModeProperty.ChangeSetValueTest);
            tests.Add(DisplayModeProperty.ClearValueResetsDefaultTest);
            tests.Add(DisplayModeProperty.InvalidValueFailsTest);
            tests.Add(DisplayModeProperty.InvalidValueIsIgnoredTest);
            tests.Add(DisplayModeProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.Add(DisplayModeProperty.CanBeStyledTest);
            tests.Add(DisplayModeProperty.TemplateBindTest);
            tests.Add(DisplayModeProperty.BindingTest);
            tests.Add(DisplayModeProperty.SetXamlAttributeTest);
            tests.Add(DisplayModeProperty.SetXamlElementTest);

            // FirstDayOfWeekProperty tests
            tests.Add(FirstDayOfWeekProperty.CheckDefaultValueTest);
            tests.Add(FirstDayOfWeekProperty.ChangeClrSetterTest);
            tests.Add(FirstDayOfWeekProperty.ChangeSetValueTest);
            tests.Add(FirstDayOfWeekProperty.ClearValueResetsDefaultTest);
            tests.Add(FirstDayOfWeekProperty.InvalidValueFailsTest);
            tests.Add(FirstDayOfWeekProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.Add(FirstDayOfWeekProperty.TemplateBindTest);
            tests.Add(FirstDayOfWeekProperty.BindingTest);
            tests.Add(FirstDayOfWeekProperty.SetXamlAttributeTest);

            // SelectionModeProperty tests
            tests.Add(SelectionModeProperty.CheckDefaultValueTest);
            tests.Add(SelectionModeProperty.ChangeClrSetterTest);
            tests.Add(SelectionModeProperty.ChangeSetValueTest);
            tests.Add(SelectionModeProperty.ClearValueResetsDefaultTest);
            tests.Add(SelectionModeProperty.InvalidValueFailsTest);
            tests.Add(SelectionModeProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.Add(SelectionModeProperty.CanBeStyledTest);
            tests.Add(SelectionModeProperty.TemplateBindTest);
            tests.Add(SelectionModeProperty.BindingTest);
            tests.Add(SelectionModeProperty.SetXamlAttributeTest);
            tests.Add(SelectionModeProperty.SetXamlElementTest);

            // SelectedDateProperty tests
            tests.Add(SelectedDateProperty.CheckDefaultValueTest);
            tests.Add(SelectedDateProperty.ChangeClrSetterTest);
            tests.Add(SelectedDateProperty.ChangeSetValueTest);
            tests.Add(SelectedDateProperty.SetNullTest);
            tests.Add(SelectedDateProperty.ClearValueResetsDefaultTest);
            tests.Add(SelectedDateProperty.InvalidValueFailsTest);
            tests.Add(SelectedDateProperty.InvalidValueIsIgnoredTest);
            tests.Add(SelectedDateProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.Add(SelectedDateProperty.CanBeStyledTest);
            tests.Add(SelectedDateProperty.TemplateBindTest);
            tests.Add(SelectedDateProperty.BindingTest);

            // DisplayDateProperty tests
            tests.Add(DisplayDateProperty.CheckDefaultValueTest);
            tests.Add(DisplayDateProperty.ChangeClrSetterTest);
            tests.Add(DisplayDateProperty.ChangeSetValueTest);
            tests.Add(DisplayDateProperty.InvalidValueFailsTest);
            tests.Add(DisplayDateProperty.InvalidValueIsIgnoredTest);
            tests.Add(DisplayDateProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.Add(DisplayDateProperty.TemplateBindTest);
            tests.Add(DisplayDateProperty.BindingTest);

            // DisplayDateStartProperty tests
            tests.Add(DisplayDateStartProperty.CheckDefaultValueTest);
            tests.Add(DisplayDateStartProperty.ChangeClrSetterTest);
            tests.Add(DisplayDateStartProperty.ChangeSetValueTest);
            tests.Add(DisplayDateStartProperty.SetNullTest);
            tests.Add(DisplayDateStartProperty.ClearValueResetsDefaultTest);
            tests.Add(DisplayDateStartProperty.InvalidValueFailsTest);
            tests.Add(DisplayDateStartProperty.InvalidValueIsIgnoredTest);
            tests.Add(DisplayDateStartProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.Add(DisplayDateStartProperty.CanBeStyledTest);
            tests.Add(DisplayDateStartProperty.TemplateBindTest);
            tests.Add(DisplayDateStartProperty.BindingTest);

            // DisplayDateEndProperty tests
            tests.Add(DisplayDateEndProperty.CheckDefaultValueTest);
            tests.Add(DisplayDateEndProperty.ChangeClrSetterTest);
            tests.Add(DisplayDateEndProperty.ChangeSetValueTest);
            tests.Add(DisplayDateEndProperty.SetNullTest);
            tests.Add(DisplayDateEndProperty.ClearValueResetsDefaultTest);
            tests.Add(DisplayDateEndProperty.InvalidValueFailsTest);
            tests.Add(DisplayDateEndProperty.InvalidValueIsIgnoredTest);
            tests.Add(DisplayDateEndProperty.InvalidValueDoesNotChangeVisualStateTest);
            tests.Add(DisplayDateEndProperty.CanBeStyledTest);
            tests.Add(DisplayDateEndProperty.TemplateBindTest);
            tests.Add(DisplayDateEndProperty.BindingTest);

            // CalendarDayButtonStyleProperty tests
            tests.Add(CalendarDayButtonStyleProperty.CheckDefaultValueTest);
            tests.Add(CalendarDayButtonStyleProperty.ChangeClrSetterTest);
            tests.Add(CalendarDayButtonStyleProperty.ChangeSetValueTest);
            tests.Add(CalendarDayButtonStyleProperty.SetNullTest);
            tests.Add(CalendarDayButtonStyleProperty.ClearValueResetsDefaultTest);
            tests.Add(CalendarDayButtonStyleProperty.CanBeStyledTest);
            tests.Add(CalendarDayButtonStyleProperty.BindingTest);
            tests.Add(CalendarDayButtonStyleProperty.TemplateBindTest);

            // CalendarInfoProperty tests
            tests.Add(CalendarInfoProperty.ChangeClrSetterTest);
            tests.Add(CalendarInfoProperty.ChangeSetValueTest);
            tests.Add(CalendarInfoProperty.SetNullTest);
            tests.Add(CalendarInfoProperty.CanBeStyledTest);
            tests.Add(CalendarInfoProperty.BindingTest);
            tests.Add(CalendarInfoProperty.TemplateBindTest);

            return tests;
        }

        #region Control contract
        /// <summary>
        /// Verify the control's template parts.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's template parts.")]
        public override void TemplatePartsAreDefined()
        {
            IDictionary<string, Type> parts = DefaultGlobalCalendarToTest.GetType().GetTemplateParts();
            Assert.AreEqual(2, parts.Count, "Incorrect number of parts");
            Assert.AreEqual(typeof(Panel), parts["Root"], "Failed to find expected template part Root!");
            Assert.AreEqual(typeof(GlobalCalendarItem), parts["CalendarItem"], "Failed to find expected template part CalendarItem!");
        }

        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's template visual states.")]
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> states = DefaultGlobalCalendarToTest.GetType().GetVisualStates();
            Assert.AreEqual(3, states.Count, "Incorrect number of template states");
            Assert.AreEqual("ValidationStates", states["Valid"], "Failed to find expected state Valid!");
            Assert.AreEqual("ValidationStates", states["InvalidFocused"], "Failed to find expected state InvalidFocused!");
            Assert.AreEqual("ValidationStates", states["InvalidUnfocused"], "Failed to find expected state InvalidUnfocused!");
        }

        /// <summary>
        /// Verify the control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultGlobalCalendarToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(3, properties.Count, "Incorrect number of style typed property attributes!");
            Assert.AreEqual(typeof(GlobalCalendarButton), properties["CalendarButtonStyle"], "Failed to find expected style type property CalendarButtonStyle!");
            Assert.AreEqual(typeof(GlobalCalendarDayButton), properties["CalendarDayButtonStyle"], "Failed to find expected style type property CalendarDayButtonStyle!");
            Assert.AreEqual(typeof(GlobalCalendarItem), properties["CalendarItemStyle"], "Failed to find expected style type property CalendarItemStyle!");
        }
        #endregion Control contract

        /// <summary>
        /// Compare two dates.
        /// </summary>
        /// <param name="first">The first date.</param>
        /// <param name="second">The second date.</param>
        /// <returns>A date indicating whether the dates are equal.</returns>
        private static bool CompareDates(DateTime first, DateTime second)
        {
            return GlobalCalendarTestExtensions.CompareDates(first, second);
        }

        #region Basic Tests
        /// <summary>
        /// Create a GlobalCalendar control.
        /// </summary>
        [TestMethod]
        [Description("Create a GlobalCalendar control.")]
        public void Create()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            Assert.IsNotNull(calendar);
        }

        /// <summary>
        /// Create a GlobalCalendar in XAML markup.
        /// </summary>
        [TestMethod]
        [Description("Create a GlobalCalendar in XAML markup.")]
        [Asynchronous]
        public void CreateInXaml()
        {
            object result = XamlReader.Load(
                "<controls:GlobalCalendar " +
                    "xmlns=\"http://schemas.microsoft.com/client/2007\" " +
                    "xmlns:controls=\"clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Toolkit\" " +
                    "SelectedDate=\"04/30/2008\" " +
                    "DisplayDateStart=\"04/30/2020\" " +
                    "DisplayDateEnd=\"04/30/2010\" " +
                    "DisplayDate=\"02/02/2000\" />");
            Assert.IsInstanceOfType(result, typeof(GlobalCalendar));

            GlobalCalendar calendar = result as GlobalCalendar;
            TestAsync(
                calendar,
                () => CompareDates(calendar.SelectedDate.Value, new DateTime(2008, 4, 30)),
                () => CompareDates(calendar.DisplayDateStart.Value, new DateTime(2008, 4, 30)),
                () => CompareDates(calendar.DisplayDate, new DateTime(2008, 4, 30)),
                () => CompareDates(calendar.DisplayDateEnd.Value, new DateTime(2010, 4, 30)));
        }

        /// <summary>
        /// Ensure all default values are correct.
        /// </summary>
        [TestMethod]
        [Description("Ensure all default values are correct.")]
        [Asynchronous]
        public void CheckDefaultValues()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            Assert.IsTrue(CompareDates(DateTime.Today, calendar.DisplayDate));
            Assert.IsNull(calendar.DisplayDateStart);
            Assert.IsNull(calendar.DisplayDateEnd);
            Assert.AreEqual(calendar.FirstDayOfWeek, GlobalCalendarTestExtensions.GetCurrentDateFormat().FirstDayOfWeek);
            Assert.IsTrue(calendar.IsTodayHighlighted);
            Assert.IsNull(calendar.SelectedDate);
            Assert.IsTrue(calendar.SelectedDates.Count == 0);
            Assert.IsTrue(calendar.BlackoutDates.Count == 0);
            Assert.IsTrue(calendar.IsEnabled);
            Assert.IsTrue(calendar.DisplayMode == CalendarMode.Month);
            Assert.IsTrue(calendar.SelectionMode == CalendarSelectionMode.SingleDate);

            GlobalCalendarItem item = null;

            TestAsync(
                calendar,
                () => item = calendar.GetCalendarItem(),
                () => Assert.IsTrue(item.GetPreviousButton().IsEnabled),
                () => Assert.IsTrue(item.GetNextButton().IsEnabled),
                () => Assert.IsTrue(item.GetHeaderButton().IsEnabled),
                () => Assert.IsTrue(item.GetHeaderButton().Content.ToString() == calendar.DisplayDate.ToString("Y", GlobalCalendarTestExtensions.GetCurrentDateFormat())));
        }

        /// <summary>
        /// Ensure Nullable Properties can be set to null.
        /// </summary>
        [TestMethod]
        [Description("Ensure Nullable Properties can be set to null.")]
        [Asynchronous]
        public void ArePropertiesNullable()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            DateTime value = DateTime.Today;

            calendar.SelectedDate = null;
            Assert.IsNull(calendar.SelectedDate);

            calendar.SelectedDate = value;
            Assert.IsTrue(CompareDates(value, calendar.SelectedDate.Value));

            calendar.SelectedDate = null;
            Assert.IsNull(calendar.SelectedDate);

            calendar.DisplayDateStart = null;
            Assert.IsNull(calendar.DisplayDateStart);

            calendar.DisplayDateStart = value;
            Assert.IsTrue(CompareDates(value, calendar.DisplayDateStart.Value));

            calendar.DisplayDateStart = null;
            Assert.IsNull(calendar.DisplayDateStart);

            calendar.DisplayDateEnd = null;
            Assert.IsNull(calendar.DisplayDateEnd);

            calendar.DisplayDateEnd = value;
            Assert.IsTrue(CompareDates(value, calendar.DisplayDateEnd.Value));

            calendar.DisplayDateEnd = null;
            Assert.IsNull(calendar.DisplayDateEnd);

            GlobalCalendarItem item = null;

            TestAsync(
                calendar,
                () => item = calendar.GetCalendarItem(),
                () => Assert.IsTrue(item.GetPreviousButton().IsEnabled),
                () => Assert.IsTrue(item.GetNextButton().IsEnabled),
                () => Assert.IsTrue(item.GetHeaderButton().IsEnabled),
                () => Assert.IsTrue(item.GetHeaderButton().Content.ToString() == calendar.DisplayDate.ToString("Y", GlobalCalendarTestExtensions.GetCurrentDateFormat())));
        }

        /// <summary>
        /// Ensure DateTime Properties can be set to DateTime.MaxValue.
        /// </summary>
        [TestMethod]
        [Description("Ensure DateTime Properties can be set to DateTime.MaxValue")]
        [Asynchronous]
        public void SetToMaxValue()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.DisplayDate = DateTime.MaxValue;
            Assert.IsTrue(CompareDates(DateTime.MaxValue, calendar.DisplayDate));

            calendar.DisplayDateEnd = DateTime.MinValue;
            calendar.DisplayDateStart = DateTime.MaxValue;
            Assert.IsTrue(CompareDates(calendar.DisplayDateStart.Value, DateTime.MaxValue));
            Assert.IsTrue(CompareDates(calendar.DisplayDateEnd.Value, DateTime.MaxValue));

            calendar.SelectedDate = DateTime.MaxValue;
            Assert.IsTrue(CompareDates(DateTime.MaxValue, (DateTime)calendar.SelectedDate));

            Assert.IsTrue(calendar.SelectedDates.Count == 1);
            Assert.IsTrue(CompareDates(calendar.SelectedDates[0], calendar.SelectedDate.Value));

            calendar.SelectedDates.Clear();
            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.MaxValue));
            Assert.IsTrue(CompareDates(calendar.BlackoutDates[0].End, DateTime.MaxValue));
            Assert.IsTrue(CompareDates(calendar.BlackoutDates[0].Start, DateTime.MaxValue));

            GlobalCalendarItem item = null;

            TestAsync(
                calendar,
                () => item = calendar.GetCalendarItem(),
                () => Assert.IsFalse(item.GetPreviousButton().IsEnabled),
                () => Assert.IsFalse(item.GetNextButton().IsEnabled),
                () => Assert.IsTrue(item.GetHeaderButton().IsEnabled),
                () => Assert.IsTrue(item.GetHeaderButton().Content.ToString() == calendar.DisplayDate.ToString("Y", GlobalCalendarTestExtensions.GetCurrentDateFormat())));
        }

        /// <summary>
        /// Ensure DateTime Properties can be set to DateTime.MinValue.
        /// </summary>
        [TestMethod]
        [Description("Ensure DateTime Properties can be set to DateTime.MinValue")]
        [Asynchronous]
        public void SetToMinValue()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.DisplayDate = DateTime.MinValue;
            Assert.IsTrue(CompareDates(calendar.DisplayDate, DateTime.MinValue));

            calendar.DisplayDateStart = DateTime.MinValue;
            calendar.DisplayDateEnd = DateTime.MinValue;
            Assert.IsTrue(CompareDates(calendar.DisplayDateStart.Value, DateTime.MinValue));
            Assert.IsTrue(CompareDates(calendar.DisplayDateEnd.Value, DateTime.MinValue));

            calendar.SelectedDate = DateTime.MinValue;
            Assert.IsTrue(CompareDates(DateTime.MinValue, (DateTime)calendar.SelectedDate));

            Assert.IsTrue(calendar.SelectedDates.Count == 1);
            Assert.IsTrue(CompareDates(calendar.SelectedDates[0], calendar.SelectedDate.Value));

            calendar.SelectedDates.Clear();
            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue));
            Assert.IsTrue(CompareDates(calendar.BlackoutDates[0].End, DateTime.MinValue));
            Assert.IsTrue(CompareDates(calendar.BlackoutDates[0].Start, DateTime.MinValue));

            GlobalCalendarItem item = null;

            TestAsync(
                calendar,
                () => item = calendar.GetCalendarItem(),
                () => Assert.IsFalse(item.GetPreviousButton().IsEnabled),
                () => Assert.IsFalse(item.GetNextButton().IsEnabled),
                () => Assert.IsTrue(item.GetHeaderButton().IsEnabled),
                () => Assert.IsTrue(item.GetHeaderButton().Content.ToString() == calendar.DisplayDate.ToString("Y", GlobalCalendarTestExtensions.GetCurrentDateFormat())));
        }
        #endregion Basic Tests

        #region Events
        /// <summary>
        /// Ensure DateSelected event is fired.
        /// </summary>
        [TestMethod]
        [Description("Ensure DateSelected event is fired.")]
        public void DateSelectedEvent()
        {
            bool handled = false;
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(delegate
            {
                handled = true;
            });
            DateTime value = new DateTime(2000, 10, 10);
            calendar.SelectedDate = value;
            Assert.IsTrue(handled);
            Assert.AreEqual(calendar.ToString(), value.ToString());
        }

        /// <summary>
        /// Ensure DisplayDateChanged event is fired.
        /// </summary>
        [TestMethod]
        [Description("Ensure DisplayDateChanged event is fired.")]
        public void DisplayDateChangedEvent()
        {
            bool handled = false;
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.DisplayDateChanged += new EventHandler<GlobalCalendarDateChangedEventArgs>(delegate
            {
                handled = true;
            });
            DateTime value = new DateTime(2000, 10, 10);
            calendar.DisplayDate = value;
            Assert.IsTrue(handled);
        }
        #endregion Events

        #region BlackoutDates
        /// <summary>
        /// Set the date of SelectedDateProperty.
        /// </summary>
        [TestMethod]
        [Description("Set the date of SelectedDateProperty.")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BlackoutDatesSingleDay()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.BlackoutDates.AddDatesInPast();
            calendar.SelectedDate = DateTime.Today.AddDays(-1);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [Description("TODO: Inherited code: Requires comment.")]
        public void BlackoutDatesRange()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.Today, DateTime.Today.AddDays(10)));

            calendar.SelectedDate = DateTime.Today.AddDays(-1);
            Assert.IsTrue(CompareDates(calendar.SelectedDate.Value, DateTime.Today.AddDays(-1)));
            Assert.IsTrue(CompareDates(calendar.SelectedDate.Value, calendar.SelectedDates[0]));

            calendar.SelectedDate = DateTime.Today.AddDays(11);
            Assert.IsTrue(CompareDates(calendar.SelectedDate.Value, DateTime.Today.AddDays(11)));
            Assert.IsTrue(CompareDates(calendar.SelectedDate.Value, calendar.SelectedDates[0]));

            calendar.SelectedDate = DateTime.Today.AddDays(5);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Description("TODO: Inherited code: Requires comment.")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetBlackoutDatesRange()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.SelectedDate = DateTime.Today.AddDays(5);
            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.Today, DateTime.Today.AddDays(10)));
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Description("TODO: Inherited code: Requires comment.")]
        public void SetBlackoutDatesRangeDisplayStart()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.DisplayDateStart = DateTime.Today.AddDays(-5);
            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.Today.AddDays(-10), DateTime.Today.AddDays(10)));
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Description("TODO: Inherited code: Requires comment.")]
        public void SetBlackoutDatesRangeDisplayEnd()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.DisplayDateEnd = DateTime.Today.AddDays(5);
            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.Today, DateTime.Today.AddDays(10)));
        }
        #endregion BlackoutDates

        #region DisplayDate
        /// <summary>
        /// Set the date of DisplayDateProperty.
        /// </summary>
        [TestMethod]
        [Description("Set the date of DisplayDateProperty.")]
        public void DisplayDatePropertySetValue()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            DateTime value = DateTime.Today.AddMonths(3);
            calendar.DisplayDate = value;
            Assert.IsTrue(CompareDates(calendar.DisplayDate, value));
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Description("TODO: Inherited code: Requires comment.")]
        public void DisplayDateStartEnd()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.DisplayDateStart = new DateTime(2005, 12, 30);

            DateTime value = new DateTime(2005, 12, 15);
            calendar.DisplayDate = value;
            Assert.IsTrue(CompareDates(calendar.DisplayDate, calendar.DisplayDateStart.Value));

            value = new DateTime(2005, 12, 30);
            calendar.DisplayDate = value;
            Assert.IsTrue(CompareDates(calendar.DisplayDate, value));

            value = DateTime.MaxValue;
            calendar.DisplayDate = value;
            Assert.IsTrue(CompareDates(calendar.DisplayDate, value));

            calendar.DisplayDateEnd = new DateTime(2010, 12, 30);
            Assert.IsTrue(CompareDates(calendar.DisplayDate, calendar.DisplayDateEnd.Value));
        }
        #endregion DisplayDate

        #region DisplayDateRange
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Description("TODO: Inherited code: Requires comment.")]
        public void DisplayDateRangeEnd()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            DateTime value = new DateTime(2000, 1, 30);

            calendar.DisplayDate = value;
            calendar.DisplayDateEnd = value;
            calendar.DisplayDateStart = value;
            Assert.IsTrue(CompareDates(calendar.DisplayDateStart.Value, value));
            Assert.IsTrue(CompareDates(calendar.DisplayDateEnd.Value, value));

            value = value.AddMonths(2);
            calendar.DisplayDateStart = value;
            Assert.IsTrue(CompareDates(calendar.DisplayDateStart.Value, value));
            Assert.IsTrue(CompareDates(calendar.DisplayDateEnd.Value, value));
            Assert.IsTrue(CompareDates(calendar.DisplayDate, value));
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Description("TODO: Inherited code: Requires comment.")]
        public void DisplayDateRangeEndSelectedDate()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;

            calendar.SelectedDate = DateTime.MaxValue;
            Assert.IsTrue(CompareDates((DateTime)calendar.SelectedDate, DateTime.MaxValue));

            calendar.DisplayDateEnd = DateTime.MaxValue.AddDays(-1);
            Assert.IsTrue(CompareDates((DateTime)calendar.DisplayDateEnd, DateTime.MaxValue));
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Description("TODO: Inherited code: Requires comment.")]
        public void DisplayDateRangeStartOutOfRangeExceptionSelectedDate()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;

            calendar.SelectedDate = DateTime.MinValue;
            Assert.IsTrue(CompareDates((DateTime)calendar.SelectedDate, DateTime.MinValue));

            calendar.DisplayDateStart = DateTime.MinValue.AddDays(1);
            Assert.IsTrue(CompareDates((DateTime)calendar.DisplayDateStart, DateTime.MinValue));
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Description("Ensure ArgumentOutOfRangeException is thrown.")]
        public void DisplayDateRangeEndBlackoutDayStart()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.Today, DateTime.Today));
            calendar.DisplayDateEnd = DateTime.Today.AddDays(-1);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Description("Ensure ArgumentOutOfRangeException is thrown.")]
        public void DisplayDateRangeEndBlackoutDayEnd()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.Today, DateTime.Today.AddDays(4)));
            calendar.DisplayDateEnd = DateTime.Today;
        }
        #endregion DisplayDateRange

        #region DisplayMode
        /// <summary>
        /// Ensure ArgumentOutOfRangeException is thrown.
        /// </summary>
        [TestMethod]
        [Description("Ensure ArgumentOutOfRangeException is thrown.")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DisplayModeOutOfRangeException()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.DisplayMode = (CalendarMode)4;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("TODO: Inherited code: Requires comment.")]
        public void DisplayModeYearToDecade()
        {
            GlobalCalendar calendar = new GlobalCalendar { DisplayMode = CalendarMode.Year };
            GlobalCalendarItem month = null;

            TestAsync(
                25,
                calendar,
                () => month = calendar.GetCalendarItem(),
                () => Assert.IsTrue(month.GetMonthView().Visibility == Visibility.Collapsed),
                () => Assert.IsTrue(month.GetYearView().Visibility == Visibility.Visible),
                () => Assert.IsTrue(calendar.DisplayMode == CalendarMode.Year),
                () => month.GetHeaderButton().ClickViaPeer(),
                ////() => month.HeaderButton_Click(month.GetHeaderButton(), new RoutedEventArgs()),
                () => Assert.IsTrue(calendar.DisplayMode == CalendarMode.Decade));
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("TODO: Inherited code: Requires comment.")]
        public void DisplayModeDecadeToMonth()
        {
            GlobalCalendar calendar = new GlobalCalendar
            {
                DisplayDate = new DateTime(2000, 1, 1),
                DisplayDateStart = new DateTime(2000, 1, 1),
                DisplayDateEnd = new DateTime(2000, 1, 1),
                DisplayMode = CalendarMode.Decade
            };
            GlobalCalendarItem month = null;

            TestAsync(
                calendar,
                () => month = calendar.GetCalendarItem(),
                () => Assert.IsTrue(month.GetMonthView().Visibility == Visibility.Collapsed),
                () => Assert.IsTrue(month.GetYearView().Visibility == Visibility.Visible),
                () => Assert.IsTrue(calendar.DisplayMode == CalendarMode.Decade),
                () => Assert.IsFalse(month.GetHeaderButton().IsEnabled),
                () => Assert.IsFalse(month.GetPreviousButton().IsEnabled),
                () => Assert.IsFalse(month.GetNextButton().IsEnabled),
                () => calendar.DisplayMode = CalendarMode.Month,
                () => Assert.IsTrue(month.GetHeaderButton().IsEnabled),
                () => Assert.IsFalse(month.GetPreviousButton().IsEnabled),
                () => Assert.IsFalse(month.GetNextButton().IsEnabled),
                () => Assert.IsTrue(calendar.DisplayMode == CalendarMode.Month),
                () => Assert.IsTrue(month.GetMonthView().Visibility == Visibility.Visible),
                () => Assert.IsTrue(month.GetYearView().Visibility == Visibility.Collapsed),
                () => calendar.DisplayMode = CalendarMode.Year,
                () => Assert.IsTrue(month.GetMonthView().Visibility == Visibility.Collapsed),
                () => Assert.IsTrue(month.GetYearView().Visibility == Visibility.Visible));
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("TODO: Inherited code: Requires comment.")]
        public void DisplayModeDecadeMinValue()
        {
            GlobalCalendar calendar = new GlobalCalendar
            {
                DisplayDate = DateTime.MinValue,
                DisplayMode = CalendarMode.Decade
            };

            TestAsync(
                calendar,
                () => Assert.IsFalse(calendar.GetCalendarItem().GetPreviousButton().IsEnabled),
                () => Assert.IsTrue(calendar.GetCalendarItem().GetNextButton().IsEnabled));
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("TODO: Inherited code: Requires comment.")]
        public void DisplayModeDecadeMaxValue()
        {
            GlobalCalendar calendar = new GlobalCalendar
            {
                DisplayDate = DateTime.MaxValue,
                DisplayMode = CalendarMode.Decade
            };

            TestAsync(
                calendar,
                () => Assert.IsFalse(calendar.GetCalendarItem().GetNextButton().IsEnabled),
                () => Assert.IsTrue(calendar.GetCalendarItem().GetPreviousButton().IsEnabled));
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("TODO: Inherited code: Requires comment.")]
        public void DisplayModeOneWeek()
        {
            GlobalCalendar calendar = new GlobalCalendar
            {
                DisplayDateStart = new DateTime(2000, 2, 2),
                DisplayDateEnd = new DateTime(2000, 2, 5),
                DisplayMode = CalendarMode.Year
            };

            TestAsync(
                calendar,
                () => Assert.IsTrue(((GlobalCalendarButton)calendar.GetCalendarItem().GetYearView().Children[1]).IsEnabled),
                () => calendar.DisplayMode = CalendarMode.Decade,
                () => Assert.IsTrue(((GlobalCalendarButton)calendar.GetCalendarItem().GetYearView().Children[1]).IsEnabled));
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("TODO: Inherited code: Requires comment.")]
        public void DisplayModeVisibleDayCount()
        {
            GlobalCalendar calendar = new GlobalCalendar
            {
                DisplayDateStart = new DateTime(2008, 6, 15),
                DisplayDateEnd = new DateTime(2008, 6, 29)
            };

            TestAsync(
                calendar,
                () => calendar.DisplayMode = CalendarMode.Year,
                () => calendar.DisplayMode = CalendarMode.Month,
                () => Assert.IsFalse((calendar.FindDayButtonFromDay(new DateTime(2008, 6, 14))).IsEnabled),
                () => Assert.IsFalse((calendar.FindDayButtonFromDay(new DateTime(2008, 6, 30))).IsEnabled));
        }
        #endregion DisplayMode

        #region FirstDayOfWeek
        /// <summary>
        /// Set the date of FirstDayOfWeekProperty.
        /// </summary>
        [TestMethod]
        [Description("Set the date of FirstDayOfWeekProperty.")]
        public void FirstDayOfWeekPropertySetValue()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            DayOfWeek value = DayOfWeek.Thursday;

            calendar.FirstDayOfWeek = value;
            Assert.AreEqual(value, calendar.GetValue(GlobalCalendar.FirstDayOfWeekProperty));
            Assert.AreEqual(value, calendar.FirstDayOfWeek);

            value = (DayOfWeek)3;
            calendar.FirstDayOfWeek = value;
            Assert.AreEqual(value, calendar.FirstDayOfWeek);
        }

        /// <summary>
        /// Ensure ArgumentOutOfRangeException is thrown.
        /// </summary>
        [TestMethod]
        [Description("Ensure ArgumentOutOfRangeException is thrown.")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FirstDayOfWeekOutOfRangeException()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.FirstDayOfWeek = (DayOfWeek)7;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("TODO: Inherited code: Requires comment.")]
        public void DisplayModeDisplayDate()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            TestAsync(
                calendar,
                () => calendar.DisplayMode = CalendarMode.Year,
                () => Assert.IsTrue(calendar.GetCalendarItem().GetHeaderButton().Content.ToString() == calendar.DisplayDate.Year.ToString(CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("TODO: Inherited code: Requires comment.")]
        public void DisplayModeDecadeDisplayDate()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            TestAsync(
                calendar,
                () => calendar.DisplayMode = CalendarMode.Decade,
                () => Assert.IsTrue(calendar.DisplayMode == CalendarMode.Decade));
        }
        #endregion FirstDayOfWeek

        #region Selection helpers
        /// <summary>
        /// The days added to the SelectedDates collection.
        /// </summary>
        private IList _selectedDatesChangedAddedDays;

        /// <summary>
        /// The days removed from the SelectedDates collection.
        /// </summary>
        private IList _selectedDateChangedRemovedDays;

        /// <summary>
        /// The number of times the SelectedDatesChanged event has been fired.
        /// </summary>
        private int _selectedDatesChangedCount;

        /// <summary>
        /// Handle the SelectedDatesChanged event.
        /// </summary>
        /// <param name="sender">The calendar.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedDatesChangedAddedDays = e.AddedItems;
            _selectedDateChangedRemovedDays = e.RemovedItems;
            _selectedDatesChangedCount++;
        }

        /// <summary>
        /// Clear the variables used to track the SelectedDatesChanged event.
        /// </summary>
        private void ResetSelectedDatesChanged()
        {
            if (_selectedDatesChangedAddedDays != null)
            {
                _selectedDatesChangedAddedDays.Clear();
            }

            if (_selectedDateChangedRemovedDays != null)
            {
                _selectedDateChangedRemovedDays.Clear();
            }

            _selectedDatesChangedCount = 0;
        }
        #endregion Selection helpers

        #region Selection
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Description("TODO: Inherited code: Requires comment.")]
        public void SelectedDateSingle()
        {
            ResetSelectedDatesChanged();
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(OnSelectedDatesChanged);
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.SelectedDate = DateTime.Today;
            Assert.IsTrue(CompareDates(calendar.SelectedDate.Value, DateTime.Today));
            Assert.IsTrue(calendar.SelectedDates.Count == 1);
            Assert.IsTrue(CompareDates(calendar.SelectedDates[0], DateTime.Today));
            Assert.IsTrue(_selectedDatesChangedCount == 1);
            Assert.IsTrue(_selectedDatesChangedAddedDays.Count == 1);
            Assert.IsTrue(_selectedDateChangedRemovedDays.Count == 0);
            ResetSelectedDatesChanged();

            calendar.SelectedDate = DateTime.Today;
            Assert.IsTrue(CompareDates(calendar.SelectedDate.Value, DateTime.Today));
            Assert.IsTrue(calendar.SelectedDates.Count == 1);
            Assert.IsTrue(CompareDates(calendar.SelectedDates[0], DateTime.Today));
            Assert.IsTrue(_selectedDatesChangedCount == 0);

            calendar.SelectedDate = new DateTime();
            calendar.ClearValue(GlobalCalendar.SelectedDateProperty);
            Assert.AreEqual(DependencyProperty.UnsetValue, calendar.ReadLocalValue(GlobalCalendar.SelectedDateProperty));

            calendar.SelectionMode = CalendarSelectionMode.None;
            Assert.IsTrue(calendar.SelectedDates.Count == 0);
            Assert.IsNull(calendar.SelectedDate);

            calendar.SelectionMode = CalendarSelectionMode.SingleDate;

            calendar.SelectedDates.Add(DateTime.Today.AddDays(1));
            Assert.IsTrue(CompareDates(calendar.SelectedDate.Value, DateTime.Today.AddDays(1)));
            Assert.IsTrue(calendar.SelectedDates.Count == 1);

            calendar.SelectedDates.Add(DateTime.Today.AddDays(2));
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Description("TODO: Inherited code: Requires comment.")]
        public void SelectedDateSingleRange()
        {
            ResetSelectedDatesChanged();
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(OnSelectedDatesChanged);
            calendar.SelectionMode = CalendarSelectionMode.SingleRange;
            calendar.SelectedDate = DateTime.Today;
            Assert.IsTrue(CompareDates(calendar.SelectedDate.Value, DateTime.Today));
            Assert.IsTrue(calendar.SelectedDates.Count == 1);
            Assert.IsTrue(CompareDates(calendar.SelectedDates[0], DateTime.Today));
            Assert.IsTrue(_selectedDatesChangedCount == 1);
            Assert.IsTrue(_selectedDatesChangedAddedDays.Count == 1);
            Assert.IsTrue(_selectedDateChangedRemovedDays.Count == 0);
            ResetSelectedDatesChanged();

            calendar.SelectedDates.Clear();
            Assert.IsNull(calendar.SelectedDate);

            ResetSelectedDatesChanged();
            calendar.SelectedDates.AddRange(DateTime.Today, DateTime.Today.AddDays(10));
            Assert.IsTrue(CompareDates(calendar.SelectedDate.Value, DateTime.Today));
            Assert.IsTrue(calendar.SelectedDates.Count == 11);

            ResetSelectedDatesChanged();
            calendar.SelectedDates.AddRange(DateTime.Today, DateTime.Today.AddDays(10));
            Assert.IsTrue(calendar.SelectedDates.Count == 11);
            Assert.IsTrue(_selectedDatesChangedCount == 0);

            calendar.SelectedDates.AddRange(DateTime.Today.AddDays(-20), DateTime.Today);
            Assert.IsTrue(CompareDates(calendar.SelectedDate.Value, DateTime.Today.AddDays(-20)));
            Assert.IsTrue(calendar.SelectedDates.Count == 21);
            Assert.IsTrue(_selectedDatesChangedCount == 1);
            Assert.IsTrue(_selectedDatesChangedAddedDays.Count == 21);
            Assert.IsTrue(_selectedDateChangedRemovedDays.Count == 11);
            ResetSelectedDatesChanged();

            calendar.SelectedDates.Add(DateTime.Today.AddDays(100));
            Assert.IsTrue(CompareDates(calendar.SelectedDate.Value, DateTime.Today.AddDays(100)));
            Assert.IsTrue(calendar.SelectedDates.Count == 1);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Description("TODO: Inherited code: Requires comment.")]
        public void SelectedDateMultipleRange()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.SelectionMode = CalendarSelectionMode.MultipleRange;
            calendar.SelectedDate = DateTime.Today;
            Assert.IsTrue(CompareDates(calendar.SelectedDate.Value, DateTime.Today));
            Assert.IsTrue(calendar.SelectedDates.Count == 1);
            Assert.IsTrue(CompareDates(calendar.SelectedDates[0], DateTime.Today));

            calendar.SelectedDates.Clear();
            Assert.IsNull(calendar.SelectedDate);

            calendar.SelectedDates.AddRange(DateTime.Today, DateTime.Today.AddDays(10));
            Assert.IsTrue(CompareDates(calendar.SelectedDate.Value, DateTime.Today));
            Assert.IsTrue(calendar.SelectedDates.Count == 11);

            calendar.SelectedDates.Add(DateTime.Today);
            Assert.IsTrue(calendar.SelectedDates.Count == 11);
        }
        #endregion Selection

        #region SelectionMode
        /// <summary>
        /// Verify SelectionMode property.
        /// </summary>
        [TestMethod]
        [Description("Verify SelectionMode property.")]
        [Asynchronous]
        public void SelectionMode()
        {
            GlobalCalendar calendar = new GlobalCalendar { SelectionMode = CalendarSelectionMode.SingleRange };

            calendar.SelectedDates.AddRange(DateTime.Today, DateTime.Today.AddDays(10));
            Assert.IsTrue(calendar.SelectedDates.Count == 11);

            TestAsync(
                calendar,
                () => calendar.SelectionMode = CalendarSelectionMode.MultipleRange,
                () => Assert.IsTrue(calendar.SelectedDates.Count == 0));
        }

        /// <summary>
        /// Ensure ArgumentOutOfRangeException is thrown.
        /// </summary>
        [TestMethod]
        [Description("Ensure ArgumentOutOfRangeException is thrown.")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SelectionModeOutOfRangeException()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.SelectionMode = (CalendarSelectionMode)7;
        }
        #endregion SelectionMode

        #region Mouse Selection
        #region NoSelection
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Description("Verify selection is not possible is None Mode.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SelectDayNoneInvalidOperationException()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.SelectionMode = CalendarSelectionMode.None;
            calendar.SelectedDate = new DateTime(2000, 2, 2);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Description("Check if an exception is thrown if SelectedDates are updated in None Selection Mode")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddNone()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.SelectionMode = CalendarSelectionMode.None;
            calendar.SelectedDates.Add(new DateTime());
        }
        #endregion NoSelection

        #region SingleSelection
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        [TestMethod]
        [Description("Check if an exception is thrown if SelectedDates have 2 items in Single SelectionMode.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddSingle()
        {
            GlobalCalendar calendar = new GlobalCalendar();
            calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            calendar.SelectedDates.Add(DateTime.Today);
            calendar.SelectedDates.Add(DateTime.Today.AddDays(1));
        }
        #endregion SingleSelection
        #endregion Mouse Selection
    }
}