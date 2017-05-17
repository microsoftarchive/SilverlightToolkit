// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Threading;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit testing TimeUpDown.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Tests are allowed to be coupled.")]
    [TestClass]
    [Tag("TimeInput")]
    public class TimeUpDownTest : UpDownBaseTest<DateTime?>
    {
        #region UpDownBasesToTest
        /// <summary>
        /// Gets a default instance of UpDownBase&lt;T&gt; (or a derived type) to test.
        /// </summary>
        public override UpDownBase<DateTime?> DefaultUpDownBaseTToTest
        {
            get { return DefaultTimeUpDownToTest; }
        }

        /// <summary>
        /// Gets instances of UpDownBase&lt;T&gt; (or derived types) to test.
        /// </summary>
        public override IEnumerable<UpDownBase<DateTime?>> UpDownBaseTsToTest
        {
            get { return TimeUpDownsToTest.OfType<UpDownBase<DateTime?>>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenUpDownBase&lt;T&gt; (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenUpDownBase<DateTime?>> OverriddenUpDownBaseTsToTest
        {
            get { return OverriddenTimeUpDownsToTest.OfType<IOverriddenUpDownBase<DateTime?>>(); }
        }
        #endregion UpDownBasesToTest

        #region TimeUpDowns To Test
        /// <summary>
        /// Gets the default TimeUpDown (or a derived type) to test.
        /// </summary>
        public virtual TimeUpDown DefaultTimeUpDownToTest
        {
            get { return new TimeUpDown() { Culture = new CultureInfo("en-US") }; }
        }

        /// <summary>
        /// Gets instances of TimeUpDown (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<TimeUpDown> TimeUpDownsToTest
        {
            get
            {
                yield return DefaultTimeUpDownToTest;

                TimeUpDown tud = new TimeUpDown();

                yield return tud;
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenTimeUpDown (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<IOverriddenControl> OverriddenTimeUpDownsToTest
        {
            get { yield break; }
        }
        #endregion TimeUpDowns To Test

        #region Dependency properties
        /// <summary>
        /// Gets the minimum property.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Value is nullable DateTime.")]
        protected DependencyPropertyTest<TimeUpDown, DateTime?> MinimumProperty { get; private set; }

        /// <summary>
        /// Gets the maximum property.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Value is nullable DateTime.")]
        protected DependencyPropertyTest<TimeUpDown, DateTime?> MaximumProperty { get; private set; }

        /// <summary>
        /// Gets the time parsers property.
        /// </summary>
        protected DependencyPropertyTest<TimeUpDown, TimeParserCollection> TimeParsersProperty { get; private set; }

        /// <summary>
        /// Gets the display format property.
        /// </summary>
        protected DependencyPropertyTest<TimeUpDown, ITimeFormat> FormatProperty { get; private set; }

        /// <summary>
        /// Gets the culture property.
        /// </summary>
        protected DependencyPropertyTest<TimeUpDown, CultureInfo> CultureProperty { get; private set; }

        /// <summary>
        /// Gets the time globalization info property.
        /// </summary>
        protected DependencyPropertyTest<TimeUpDown, TimeGlobalizationInfo> TimeGlobalizationInfoProperty { get; private set; }

        /// <summary>
        /// Gets the hint content property.
        /// </summary>
        /// <value>The hint content property.</value>
        protected DependencyPropertyTest<TimeUpDown, object> HintContentProperty { get; private set; }
        #endregion Dependency properties

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeUpDownTest"/> class.
        /// </summary>
        public TimeUpDownTest()
        {
            Func<TimeUpDown> initializer = () => new TimeUpDown();
            BackgroundProperty.DefaultValue = new SolidColorBrush(Colors.Transparent);
            ValueProperty.OtherValues = new[] { new DateTime?(), new DateTime(2000, 12, 3), new DateTime(1900, 1, 1) };

            MinimumProperty = new DependencyPropertyTest<TimeUpDown, DateTime?>(this, "Minimum")
            {
                Property = TimeUpDown.MinimumProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new DateTime?(), new DateTime(2000, 12, 3), new DateTime(1900, 1, 1) },
            };

            MaximumProperty = new DependencyPropertyTest<TimeUpDown, DateTime?>(this, "Maximum")
            {
                Property = TimeUpDown.MaximumProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new DateTime?(), new DateTime(2000, 12, 3), new DateTime(1900, 1, 1) },
            };

            TimeParsersProperty = new DependencyPropertyTest<TimeUpDown, TimeParserCollection>(this, "TimeParsers")
            {
                Property = TimeUpDown.TimeParsersProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new TimeParserCollection(), null }
            };

            FormatProperty = new DependencyPropertyTest<TimeUpDown, ITimeFormat>(this, "Format")
            {
                Property = TimeUpDown.FormatProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new ITimeFormat[] { new ShortTimeFormat(), new LongTimeFormat(), new CustomTimeFormat("hh:ss") },
            };

            CultureProperty = new DependencyPropertyTest<TimeUpDown, CultureInfo>(this, "Culture")
            {
                Property = TimeUpDown.CultureProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new CultureInfo("en-US"), new CultureInfo("nl-NL") }
            };

            TimeGlobalizationInfoProperty = new DependencyPropertyTest<TimeUpDown, TimeGlobalizationInfo>(this, "TimeGlobalizationInfo")
            {
                Property = TimeUpDown.TimeGlobalizationInfoProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new TimeGlobalizationInfo() }
            };

            HintContentProperty = new DependencyPropertyTest<TimeUpDown, object>(this, "TimeHintContent")
            {
                Property = TimeUpDown.TimeHintContentProperty,
                Initializer = initializer,
                DefaultValue = String.Empty,
                OtherValues = new object[] { "bad time", new Border() }
            };

            // setting new defaults
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
            BorderThicknessProperty.DefaultValue = new Thickness(1);
        }

        /// <summary>
        /// Gets the dependency property tests.
        /// </summary>
        /// <returns>A list of DP tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());

            DependencyPropertyTestMethod buggedTest = tests.FirstOrDefault(a => a.Name == ValueProperty.SetXamlElementTest.Name);
            if (buggedTest != null)
            {
                buggedTest.Bug("TODO: xaml parser.");
            }

            // TODO: styles that are templatebound fail. Find out why
            tests.Where(method => method.PropertyName == "SpinnerStyle").ToList().ForEach(method => tests.Remove(method));

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
            tests.Add(CultureProperty.SetXamlAttributeTest);
            tests.Add(CultureProperty.SetXamlElementTest.Bug("TODO: framework substitutes x:Null, which is not correct for CultureInfo"));

            // TimeGlobalizationInfoProperty tests
            tests.Add(TimeGlobalizationInfoProperty.CheckDefaultValueTest);
            tests.Add(TimeGlobalizationInfoProperty.ChangeClrSetterTest);
            tests.Add(TimeGlobalizationInfoProperty.ChangeSetValueTest);
            tests.Add(TimeGlobalizationInfoProperty.ClearValueResetsDefaultTest);
            tests.Add(TimeGlobalizationInfoProperty.CanBeStyledTest);
            tests.Add(TimeGlobalizationInfoProperty.TemplateBindTest);

            // TimeHintContentProperty tests
            tests.Add(HintContentProperty.CheckDefaultValueTest);
            return tests;
        }

        #region Contract tests
        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's template visual states.")]
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> visualStates = DefaultTimeUpDownToTest.GetType().GetVisualStates();
            Assert.AreEqual(12, visualStates.Count);

            Assert.AreEqual("CommonStates", visualStates["Normal"]);
            Assert.AreEqual("CommonStates", visualStates["MouseOver"]);
            Assert.AreEqual("CommonStates", visualStates["Pressed"]);
            Assert.AreEqual("CommonStates", visualStates["Disabled"]);

            Assert.AreEqual("FocusStates", visualStates["Focused"]);
            Assert.AreEqual("FocusStates", visualStates["Unfocused"]);

            Assert.AreEqual("TimeHintStates", visualStates["TimeHintOpenedUp"]);
            Assert.AreEqual("TimeHintStates", visualStates["TimeHintOpenedDown"]);
            Assert.AreEqual("TimeHintStates", visualStates["TimeHintClosed"]);

            Assert.AreEqual("ParsingStates", visualStates["ValidTime"]);
            Assert.AreEqual("ParsingStates", visualStates["InvalidTime"]);
            Assert.AreEqual("ParsingStates", visualStates["EmptyTime"]);
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
            Assert.AreEqual(typeof(ButtonSpinner), properties["SpinnerStyle"], "Failed to find expected style type property SpinnerStyle!");
        }

        /// <summary>
        /// Verifies the Control's TemplateParts.
        /// </summary>
        [TestMethod]
        [Description("Verifies the Control's TemplateParts.")]
        public override void TemplatePartsAreDefined()
        {
            IDictionary<string, Type> templateParts = DefaultControlToTest.GetType().GetTemplateParts();
            Assert.AreEqual(3, templateParts.Count);
            Assert.AreSame(typeof(TextBox), templateParts["Text"]);
            Assert.AreSame(typeof(Spinner), templateParts["Spinner"]);
            Assert.AreSame(typeof(Popup), templateParts["TimeHintPopup"]);
        }
        #endregion Contract tests

        #region TimeTypeConverter
        /// <summary>
        /// Tests that TimeTypeConverter is used to set value.
        /// </summary>
        [TestMethod]
        [Description("Tests that TimeTypeConverter is used to set value.")]
        public virtual void ShouldInvokeTimeTypeConverterWhenPassingNull()
        {
            XamlBuilder<ContentControl> builder = CreateWrappedTimeUpDownXaml();
            XamlBuilder tudBuilder = builder.Children[0];
            tudBuilder.AttributeProperties = new Dictionary<string, string> { { "Value", "" } };

            ContentControl wrapper = builder.Load();
            TimeUpDown tud = (TimeUpDown)wrapper.Content;

            Assert.AreEqual(null, tud.Value);
        }

        /// <summary>
        /// Tests that TimeTypeConverter is used to set value.
        /// </summary>
        [TestMethod]
        [Description("Tests that TimeTypeConverter is used to set value.")]
        public virtual void ShouldInvokeTimeTypeConverterWhenPassingNonNull()
        {
            XamlBuilder<ContentControl> builder = CreateWrappedTimeUpDownXaml();
            XamlBuilder tudBuilder = builder.Children[0];
            tudBuilder.AttributeProperties = new Dictionary<string, string> { { "Value", "01/01/2009 14:00:00" } };

            ContentControl wrapper = builder.Load();
            TimeUpDown tud = (TimeUpDown)wrapper.Content;

            Assert.AreEqual(new DateTime(2009, 1, 1, 14, 0, 0), tud.Value);
        }
        #endregion TimeTypeConverter

        #region Culture Type Converter
        /// <summary>
        /// Tests that CultureTypeConverter is used to set value.
        /// </summary>
        [TestMethod]
        [Description("Tests that CultureTypeConverter is used to set value.")]
        public virtual void ShouldInvokeCultureTypeConverterWhenPassingNonNull()
        {
            XamlBuilder<ContentControl> builder = CreateWrappedTimeUpDownXaml();
            XamlBuilder tudBuilder = builder.Children[0];
            tudBuilder.AttributeProperties = new Dictionary<string, string> { { "Culture", "nl-NL" } };

            ContentControl wrapper = builder.Load();
            TimeUpDown tud = (TimeUpDown)wrapper.Content;

            Assert.AreEqual(new CultureInfo("nl-NL"), tud.Culture);
        }
        #endregion Culture Type Converter

        #region TimeParsers
        /// <summary>
        /// Tests that custom formatters are used.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that text can be parsed correctly.")]
        public virtual void ShouldUseCustomParser()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Culture = new CultureInfo("en-US");
            tud.Format = new CustomTimeFormat("hh.mm:ss");
            tud.TimeParsers = new TimeParserCollection { new CustomTimeParser() };

            TestAsync(
                tud,
                () => tud.ApplyText("3 uur 40"), // short
                () => Assert.AreEqual(new DateTime(1, 1, 1, 15, 40, 0).TimeOfDay, tud.Value.Value.TimeOfDay));
        }
        #endregion TimeParsers

        #region Coercion
        /// <summary>
        /// Breadth test on interaction between minimum maximum and value.
        /// </summary>
        [TestMethod]
        [Description("Breadth test on interaction between minimum maximum and value")]
        public virtual void ShouldCoerceMinimumMaximumAndValue()
        {
            TimeUpDown tud = DefaultTimeUpDownToTest;

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
            TimeUpDown tud = new TimeUpDown();

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
            TimeUpDown tud = DefaultTimeUpDownToTest;

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
        /// Tests that last valid value is used when unable to parse.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that last valid value is used when unable to parse.")]
        public virtual void ShouldRevertToLastValidValueWhenUnableToParse()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Culture = new CultureInfo("en-US");
            tud.Format = new CustomTimeFormat("hh.mm:ss");
            tud.Value = new DateTime(2099, 1, 3, 14, 30, 0);

            TestAsync(
                tud,
                () => tud.ApplyText("unparsableText"),
                () => Assert.AreEqual(new DateTime(2099, 1, 3, 14, 30, 0), tud.Value.Value));
        }

        /// <summary>
        /// Tests that last valid value is used when value is set outside of valid range.
        /// </summary>
        [TestMethod]
        [Description("Tests that last valid value is used when value is set outside of valid range.")]
        public virtual void ShouldRevertToLastValidValueWhenSetOutsideRange()
        {
            TimeUpDown tud = DefaultTimeUpDownToTest;

            tud.Value = new DateTime(2100, 1, 1, 15, 0, 0);
            tud.Minimum = new DateTime(2100, 1, 1, 14, 0, 0);
            tud.Maximum = new DateTime(2100, 1, 1, 16, 0, 0);

            tud.Value = new DateTime(2100, 1, 1, 10, 0, 0);
            Assert.AreEqual(new DateTime(2100, 1, 1, 15, 0, 0), tud.Value.Value);
        }
        #endregion

        #region Culture determination
        /// <summary>
        /// Tests that culture determination is correct.
        /// </summary>
        [TestMethod]
        [Description("Tests that culture determination is correct.")]
        public virtual void ShouldDetermineActualCulture()
        {
            TimeUpDown tud = DefaultTimeUpDownToTest;
            tud.Culture = new CultureInfo("nl");
            CultureInfo currentUI = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("it-IT");
            CultureInfo current = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

            try
            {
                Assert.AreEqual("nl", tud.ActualCulture.ToString());
                tud.Culture = null;
                Assert.AreEqual("fr-FR", tud.ActualCulture.ToString());
                Thread.CurrentThread.CurrentCulture = new CultureInfo("fr");
                Assert.AreEqual("it-IT", tud.ActualCulture.ToString());
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("it");
                Assert.AreEqual("en-US", tud.ActualCulture.ToString());
            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = currentUI;
                Thread.CurrentThread.CurrentCulture = current;
            }
        }
        #endregion

        #region Value formatting
        /// <summary>
        /// Tests that correct formatting is used.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that correct formatting is used.")]
        public virtual void ShouldBeAbleToFormatADateTime()
        {
            TimeUpDown tud = DefaultTimeUpDownToTest;
            tud.Format = new CustomTimeFormat("hh.mm:ss");
            TextBox displayBox = null;

            TestAsync(
                tud,
                () => displayBox = ((Panel)VisualTreeHelper.GetChild(tud, 0)).FindName("Text") as TextBox,
                () => tud.Value = new DateTime(1900, 1, 1, 9, 5, 10),
                () => tud.Format = new LongTimeFormat(),
                () => Assert.AreEqual("9:05:10 AM", displayBox.Text),
                () => tud.Format = new ShortTimeFormat(),
                () => Assert.AreEqual("9:05 AM", displayBox.Text),
                () => tud.Format = new CustomTimeFormat("hh.mm:ss"),
                () => Assert.AreEqual("09.05:10", displayBox.Text),
                () => tud.Culture = new CultureInfo("nl-NL"),
                () => tud.Format = new LongTimeFormat(),
                () => Assert.IsTrue(displayBox.Text.EndsWith("9:05:10", StringComparison.OrdinalIgnoreCase)),
                () => tud.Format = new ShortTimeFormat(),
                () => Assert.IsTrue(displayBox.Text.EndsWith("9:05", StringComparison.OrdinalIgnoreCase)),
                () => tud.Format = new CustomTimeFormat("hh.mm:ss"),
                () => Assert.AreEqual("09.05:10", displayBox.Text));
        }
        #endregion

        #region Value parsing
        /// <summary>
        /// Tests that text can be parsed correctly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that text can be parsed correctly.")]
        public virtual void ShouldParseText()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Culture = new CultureInfo("en-US");
            tud.TimeParsers = new TimeParserCollection();
            tud.TimeParsers.Add(new CustomTimeParser());
            tud.Format = new CustomTimeFormat("hh.mm:ss");

            DateTime testDate = new DateTime(1900, 1, 1, 9, 5, 10);

            TestAsync(
                tud,
                () => tud.ApplyText("09.05:10"), // custom format string
                () => Assert.AreEqual(new DateTime(1, 1, 1, 9, 5, 10).TimeOfDay, tud.Value.Value.TimeOfDay),
                () => tud.ApplyText("10:06:11 AM"), // long
                () => Assert.AreEqual(new DateTime(1, 1, 1, 10, 6, 11).TimeOfDay, tud.Value.Value.TimeOfDay),
                () => tud.ApplyText("10:07 AM"), // short
                () => Assert.AreEqual(new DateTime(1, 1, 1, 10, 7, 0).TimeOfDay, tud.Value.Value.TimeOfDay),
                () => tud.ApplyText("3 uur 40"), // short
                () => Assert.AreEqual(new DateTime(1, 1, 1, 15, 40, 0).TimeOfDay, tud.Value.Value.TimeOfDay));
        }

        /// <summary>
        /// Tests that date of value is not changed when parsing text.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that date of value is not changed when parsing text.")]
        public virtual void ShouldIgnoreParsedDate()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Culture = new CultureInfo("en-US");
            tud.Format = new LongTimeFormat();
            tud.Value = new DateTime(2001, 2, 3, 14, 15, 16);

            TestAsync(
                tud,
                () => tud.ApplyText("10:10:10 AM"),
                () => Assert.AreEqual(new DateTime(2001, 2, 3, 10, 10, 10), tud.Value.Value));
        }

        /// <summary>
        /// Tests custom parsing hook.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests custom parsing hook.")]
        public virtual void ShouldAllowCustomParsing()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Culture = new CultureInfo("en-US");

            tud.Parsing += (sender, e) =>
            {
                e.Value = new DateTime(2000, 2, 2, 9, 10, 11);
                e.Handled = true;
            };

            TestAsync(
                tud,
                () => tud.ApplyText("random text"),
                () => Assert.AreEqual("9:10 AM", tud.DisplayTextBox.Text));
        }

        /// <summary>
        /// Tests that all events regarding parsing are called correctly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that all events regarding parsing are called correctly.")]
        public virtual void ShouldRaiseParsingAndParseEvents()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Culture = new CultureInfo("en-US");
            tud.Value = new DateTime(2000, 2, 2, 9, 10, 11);

            bool parsingCalled = false;
            bool parseErrorCalled = false;

            tud.Parsing += (sender, e) => parsingCalled = true;
            tud.ParseError += (sender, e) => parseErrorCalled = true;

            TestAsync(
            tud,
            () => tud.ApplyText("random text"),
            () => Assert.IsTrue(parsingCalled),
            () => Assert.IsTrue(parseErrorCalled),
            () => Assert.AreEqual("9:10 AM", tud.DisplayTextBox.Text));
        }

        /// <summary>
        /// Tests that current date is used if no date is supplied.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that current date is used if no date is supplied.")]
        public virtual void ShouldUseCurrentDateAsFallback()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Culture = new CultureInfo("en-US");
            tud.Format = new ShortTimeFormat();

            TestAsync(
                tud,
                () => tud.ApplyText("09:05 AM"),
                () => Assert.AreEqual(DateTime.Now.Date, tud.Value.Value.Date));
        }

        /////// <summary>
        /////// Tests that date is used if date is supplied.
        /////// </summary>
        ////[TestMethod]
        ////[Asynchronous]
        ////[Description("Tests that date is used if date is supplied.")]
        ////public virtual void ShouldUseDateIfSupplied()
        ////{
        ////    OverriddenTimeUpDown tud = new OverriddenTimeUpDown
        ////                                   {
        ////                                       Culture = new CultureInfo("en-US"),
        ////                                       Format = new ShortTimeFormat()
        ////                                   };

        ////    DateTimeFormatInfo info = tud.ActualCulture.DateTimeFormat;

        ////    TestAsync(
        ////        tud,
        ////        () => tud.ApplyText(String.Format(
        ////            CultureInfo.InvariantCulture, 
        ////            "{0} {1}", 
        ////            new DateTime(2002, 2, 5).ToString(info.ShortDatePattern),
        ////            new DateTime(2002, 2, 5, 3, 14, 0).ToShortTimeString())), // custom format string
        ////        () => Assert.AreEqual(new DateTime(2002, 2, 5, 3, 14, 0), tud.Value.Value));
        ////}

        /// <summary>
        /// Tests that value can be cleared correctly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that value can be cleared correctly.")]
        public virtual void ShouldBeAbleToClearValue()
        {
            TimeUpDown tud = new TimeUpDown();

            bool executed = false;

            tud.LayoutUpdated += (sender, args) =>
                                     {
                                         if (executed)
                                         {
                                             return;
                                         }
                                         tud.Value = new DateTime();
                                         tud.ClearValue(TimeUpDown.ValueProperty);
                                         Assert.IsTrue((DependencyProperty.UnsetValue == tud.ReadLocalValue(TimeUpDown.ValueProperty)));
                                         executed = true;
                                     };

            EnqueueCallback(() => TestPanel.Children.Add(tud));
            EnqueueCallback(tud.UpdateLayout);
            EnqueueConditional(() => executed);
            EnqueueTestComplete();
        }
        #endregion

        #region Spinning
        /// <summary>
        /// Tests that caret position is correct after spin.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("RequiresFocus")]
        [Description("Tests that caret position is correct after spin.")]
        public virtual void ShouldMaintainCursorLocationAfterSpin()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Format = new CustomTimeFormat("h:mm:ss");

            TestAsync(
                tud,
                () => tud.DisplayTextBox.Focus(),
                () => tud.Value = new DateTime(1900, 1, 1, 9, 5, 10),
                () => tud.SetCursor(3),
                () => tud.Increment(),
                () => Assert.AreEqual("9:06:10", tud.DisplayTextBox.Text),
                () => Assert.AreEqual(3, tud.DisplayTextBox.SelectionStart),
                () => tud.Decrement(),
                () => Assert.AreEqual(3, tud.DisplayTextBox.SelectionStart),
                () => tud.SetCursor(0),
                () => tud.Increment(), // hours always spin 1 hour timespans
                () => Assert.AreEqual("10:05:10", tud.DisplayTextBox.Text),
                () => Assert.AreEqual(0, tud.DisplayTextBox.SelectionStart),
                () => tud.Decrement(),
                () => Assert.AreEqual(0, tud.DisplayTextBox.SelectionStart));
        }

        /// <summary>
        /// Tests that spinning will first parse value.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that spinning will first parse value.")]
        public virtual void ShouldParseBeforeSpin()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Format = new CustomTimeFormat("h:mm:ss");
            RepeatButton button = null;
            RepeatButtonAutomationPeer peer = null;
            IInvokeProvider invokeProvider = null;
            TestAsync(
                tud, 
                () => tud.DisplayTextBox.Focus(),
                () => tud.ApplyText("4:03:03"),
                () => tud.SetCursor(3),
                () => button = (RepeatButton) tud.GetVisualDescendents().Where(o => o.GetType().Equals(typeof(RepeatButton))).First(),
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(button) as RepeatButtonAutomationPeer,
                () => invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider,
                () => invokeProvider.Invoke(),
                () => Assert.IsTrue(tud.DisplayTextBox.Text == "4:04:03"),
                () => button = (RepeatButton)tud.GetVisualDescendents().Where(o => o.GetType().Equals(typeof(RepeatButton))).Skip(1).First(),
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(button) as RepeatButtonAutomationPeer,
                () => invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider,
                () => tud.ApplyText("4:03:03"),
                () => tud.SetCursor(3),
                () => invokeProvider.Invoke(),
                () => Assert.IsTrue(tud.DisplayTextBox.Text == "4:02:03"));
        }

        /// <summary>
        /// Tests localized spinning.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Test.")]
        [TestMethod]
        [Asynchronous]
        [Tag("RequiresFocus")]
        [Description("Tests localized spinning.")]
        public virtual void ShouldSpinAccordingToCursorPosition()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Format = new CustomTimeFormat("HH:mm:ss");
            tud.Culture = new CultureInfo("nl-NL"); // no designators used
            TestAsync(
                tud,
                () => tud.DisplayTextBox.Focus(),
                () => tud.Value = new DateTime(1900, 1, 1, 10, 5, 10),
                () => tud.SetCursor(0),
                () => tud.Increment(),
                () => Assert.AreEqual("11:05:10", tud.DisplayTextBox.Text),
                () => tud.Decrement(),
                () => tud.SetCursor(1),
                () => tud.Increment(),
                () => Assert.AreEqual("11:05:10", tud.DisplayTextBox.Text),
                () => tud.Decrement(),
                () => tud.SetCursor(2),
                () => tud.Increment(),
                () => tud.Increment(),
                () => Assert.AreEqual("12:05:10", tud.DisplayTextBox.Text),
                () => tud.Decrement(),
                () => tud.SetCursor(3),
                () => tud.Increment(),
                () => Assert.AreEqual("11:15:10", tud.DisplayTextBox.Text),
                () => tud.Decrement(),
                () => tud.SetCursor(4),
                () => tud.Increment(),
                () => Assert.AreEqual("11:06:10", tud.DisplayTextBox.Text),
                () => tud.Decrement(),
                () => tud.SetCursor(5),
                () => tud.Increment(),
                () => Assert.AreEqual("11:06:10", tud.DisplayTextBox.Text),
                () => tud.Decrement(),
                () => tud.SetCursor(6),
                () => tud.Increment(),
                () => Assert.AreEqual("11:05:20", tud.DisplayTextBox.Text),
                () => tud.Decrement(),
                () => tud.SetCursor(7),
                () => tud.Increment(),
                () => Assert.AreEqual("11:05:11", tud.DisplayTextBox.Text),
                () => tud.Decrement(),
                () => tud.SetCursor(8),
                () => tud.Increment(),
                () => Assert.AreEqual("11:05:11", tud.DisplayTextBox.Text));
        }

        /// <summary>
        /// Tests that spinning below minimum will cycle to maximum.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("RequiresFocus")]
        [Description("Tests that spinning below minimum will cycle to maximum.")]
        public virtual void ShouldSetToMaximumWhenSpinningBelowMinimum()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Format = new CustomTimeFormat("HH:mm:ss");
            tud.Value = new DateTime(2100, 1, 1, 15, 0, 0);
            tud.Minimum = new DateTime(2100, 1, 1, 14, 0, 0);
            tud.Maximum = new DateTime(2100, 1, 1, 16, 0, 0);
            tud.IsCyclic = true;

            TestAsync(
                tud,
                () => tud.DisplayTextBox.Focus(),
                () => tud.SetCursor(3),
                () => tud.Decrement(6),
                () => Assert.AreEqual(tud.Minimum.Value, tud.Value.Value),
                () => tud.Decrement(),
                () => Assert.AreEqual(tud.Maximum.Value, tud.Value.Value),
                () => tud.Increment(),
                () => Assert.AreEqual(tud.Minimum.Value, tud.Value.Value));
        }

        /// <summary>
        /// Tests that spinning is disabled when contextual spin would go past borders.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("RequiresFocus")]
        [Description("Tests that spinning is disabled when contextual spin would go past borders.")]
        public virtual void ShouldDisableSpinIfSpinWouldExceedRange()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown
            {
                Format = new CustomTimeFormat("HH:mm:ss"),
                Value = new DateTime(2100, 1, 1, 15, 0, 0),
                Minimum = new DateTime(2100, 1, 1, 14, 0, 0),
                Maximum = new DateTime(2100, 1, 1, 15, 30, 0),
                IsCyclic = false
            };

            ButtonSpinner spinner = null;
            TestAsync(
                tud,
                () => tud.DisplayTextBox.Focus(),
                () => tud.SetCursor(4),
                () => spinner = ((Panel)VisualTreeHelper.GetChild(tud, 0)).FindName("Spinner") as ButtonSpinner,
                () => Assert.IsTrue(spinner.ValidSpinDirection == (ValidSpinDirections.Increase | ValidSpinDirections.Decrease)),
                () => tud.SetCursor(1), // at one hour position
                () => Assert.IsTrue(spinner.ValidSpinDirection == ValidSpinDirections.Decrease, "changing caret position should re-evaluate spin directions"),
                () => tud.Minimum = new DateTime(2100, 1, 1, 14, 30, 0),
                () => Assert.IsTrue(spinner.ValidSpinDirection == ValidSpinDirections.None, "changing minimum should re-evaluate spin directions"),
                () => tud.Maximum = new DateTime(2100, 1, 1, 16, 30, 0),
                () => Assert.IsTrue(spinner.ValidSpinDirection == ValidSpinDirections.Increase, "changing maximum should re-evaluate spin directions"),
                () => tud.Minimum = null,
                () => tud.Maximum = null,
                () => tud.Value = DateTime.Now.Date,
                () => Assert.IsTrue(spinner.ValidSpinDirection == ValidSpinDirections.Increase, "no minimum, but at beginning of day"),
                () => tud.Value = DateTime.Now.Date.Subtract(TimeSpan.FromSeconds(1)),
                () => Assert.IsTrue(spinner.ValidSpinDirection == ValidSpinDirections.Decrease, "no maximum, but at end of day"));
        }

        /// <summary>
        /// Tests spinning at DateTime boundary.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("RequiresFocus")]
        [Description("Tests spinning at DateTime boundary.")]
        public virtual void ShouldHandleSpinAtDateTimeBoundaries()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown
            {
                Format = new CustomTimeFormat("HH:mm:ss"),
                Value = DateTime.MinValue,
            };

            TestAsync(
                tud,
                () => tud.DisplayTextBox.Focus(),
                () => tud.SetCursor(0),
                () => tud.Decrement(),
                () => Assert.AreEqual(DateTime.MinValue.Date, tud.Value.Value.Date),
                () => Assert.IsTrue(tud.Value > DateTime.MinValue),
                () => tud.Value = DateTime.MaxValue,
                () => tud.Increment(),
                () => Assert.AreEqual(DateTime.MaxValue.Date, tud.Value.Value.Date),
                () => Assert.IsTrue(tud.Value < DateTime.MaxValue));
        }

        /// <summary>
        /// Tests that caret does not jump unnecessarily.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Complex test.")]
        [TestMethod]
        [Asynchronous]
        [Tag("RequiresFocus")]
        [Description("Tests that caret does not jump unnecessarily.")]
        public virtual void ShouldNotMoveCaretPositionUnnecessarily()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown
            {
                Format = new CustomTimeFormat("h:mm:ss tt"),
            };

            TestAsync(
                tud,
                () => tud.DisplayTextBox.Focus(),
                () => tud.ApplyText("4:03:02 AM"),
                () => tud.SetCursor(1),
                () => tud.Decrement(),
                () => Assert.AreEqual(1, tud.GetCursor()),
                () => tud.Increment(),
                () => Assert.AreEqual(1, tud.GetCursor()),
                () => tud.SetCursor(4),
                () => tud.Decrement(),
                () => Assert.AreEqual(4, tud.GetCursor()),
                () => tud.Increment(),
                () => Assert.AreEqual(4, tud.GetCursor()),
                () => tud.SetCursor(7),
                () => tud.Decrement(),
                () => Assert.AreEqual(7, tud.GetCursor()),
                () => tud.Increment(),
                () => Assert.AreEqual(7, tud.GetCursor()),
                () => tud.SetCursor(8),
                () => tud.Decrement(),  
                () => Assert.AreEqual(8, tud.GetCursor()),
                () => tud.Increment(),
                () => Assert.AreEqual(8, tud.GetCursor()),
                () => tud.SetCursor(9),
                () => tud.Decrement(),
                () => Assert.AreEqual(9, tud.GetCursor()),
                () => tud.Increment(),
                () => Assert.AreEqual(9, tud.GetCursor()),
                () => tud.SetCursor(10),
                () => tud.Decrement(),
                () => Assert.AreEqual(10, tud.GetCursor()),
                () => tud.Increment(),
                () => Assert.AreEqual(10, tud.GetCursor()));
        }

        /// <summary>
        /// Tests that caret is not positioned in front of a TimeSeparator.
        /// </summary>
        /// <remarks>Ignored because this might be the exact behavior that we DO want.</remarks>
        [TestMethod]
        [Asynchronous]
        [Tag("RequiresFocus")]
        [Ignore]
        [Description("Tests that caret is not positioned in front of a TimeSeparator.")]
        public virtual void ShouldNotPositionCaretInFrontOfTimeSeparator()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown
                                           {
                                               Format = new CustomTimeFormat("h:mm:ss tt"),
                                               Value = new DateTime(1900, 1, 1, 11, 1, 1)
                                           };

            TestAsync(
                tud, 
                () => tud.DisplayTextBox.Focus(),
                () => tud.SetCursor(2),
                () => tud.Decrement(),
                () => Assert.AreEqual(0, tud.GetCursor()));
        }

        /// <summary>
        /// Tests spinning hours special behavior.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("RequiresFocus")]
        [Ignore]
        [Description("Tests spinning hours special behavior.")]
        public virtual void ShouldUseDesignatorSpinOrOneHourSpin()
        {
            // TODO: remove test if decision has been made on current behavior

            // testing hour behavior that was changed (27 march 2009)
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Format = new CustomTimeFormat("HH:mm:ss");
            tud.Culture = new CultureInfo("nl-NL"); // culture without designator

            TestAsync(
                tud,
                () => tud.DisplayTextBox.Focus(),
                () => tud.Value = new DateTime(1900, 1, 1, 10, 5, 10),
                () => tud.SetCursor(0),
                () => tud.Increment(),
                () => Assert.AreEqual("20:05:10", tud.DisplayTextBox.Text),
                () => tud.Increment(),
                () => Assert.AreEqual("21:05:10", tud.DisplayTextBox.Text),
                () => tud.SetCursor(0),
                () => tud.Decrement(2),
                () => Assert.AreEqual("01:05:10", tud.DisplayTextBox.Text),
                () => tud.Culture = new CultureInfo("en-US"),
                () => tud.Format = new ShortTimeFormat(),
                () => tud.Value = new DateTime(1900, 1, 1, 10, 5, 10),
                () => tud.SetCursor(0),
                () => tud.Increment(),
                () => Assert.AreEqual("10:05 PM", tud.DisplayTextBox.Text),
                () => tud.SetCursor(0),
                () => tud.Decrement(),
                () => Assert.AreEqual("10:05 AM", tud.DisplayTextBox.Text));
        }

        /// <summary>
        /// Tests that can spin with TimeDesignators in front.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("RequiresFocus")]
        [Description("Tests that can spin with TimeDesignators in front.")]
        public virtual void ShouldBeAbleToSpinTimeDesignatorsInFront()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Format = new CustomTimeFormat("tt h:mm");
            tud.Culture = new CultureInfo("en-US");
            tud.Value = new DateTime(1900, 1, 1, 8, 3, 3);

            TestAsync(
                    tud,
                    () => tud.DisplayTextBox.Focus(),
                    () => Assert.AreEqual("AM 8:03", tud.DisplayTextBox.Text),
                    () => tud.SetCursor(0),
                    () => tud.Increment(),
                    () => Assert.AreEqual("PM 8:03", tud.DisplayTextBox.Text));
        }
        #endregion Spinning

        #region Balloon hint
        /// <summary>
        /// Tests balloon hint logic.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests balloon hint logic.")]
        public virtual void ShouldShowBalloonTextCorrectly()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Format = new CustomTimeFormat("h:mm:ss");
            tud.Minimum = new DateTime(1900, 1, 1, 4, 30, 0);
            tud.Maximum = new DateTime(1900, 1, 1, 18, 40, 0);
            tud.Value = new DateTime(1900, 1, 1, 12, 0, 0);

            TestAsync(
                tud,
                () => tud.SetText("121314"),
                () => Assert.IsFalse(tud.TimeHintContent.ToString().Contains("<")),
                () => tud.SetText("330"),
                () => Assert.IsTrue(tud.TimeHintContent.ToString().Contains("<")),
                () => tud.SetText("230"),
                () => Assert.IsTrue(tud.TimeHintContent.ToString().Contains("<")),
                () => tud.SetText("1300"),
                () => Assert.IsFalse(tud.TimeHintContent.ToString().Contains("<")));
        }

        /// <summary>
        /// Tests that balloon gets shown on top.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that balloon gets shown on top.")]
        [Ignore]
        public virtual void ShouldShowBalloonOnTop()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Format = new CustomTimeFormat("h:mm:ss");
            tud.Minimum = new DateTime(1900, 1, 1, 4, 30, 0);
            tud.Maximum = new DateTime(1900, 1, 1, 18, 40, 0);
            tud.Value = new DateTime(1900, 1, 1, 12, 0, 0);
            tud.Height = 30;
            bool isLoaded = false;
            tud.Loaded += delegate { isLoaded = true; };

            FrameworkElement visualHintElement = null;

            tud.VerticalAlignment = VerticalAlignment.Center;
            
            EnqueueCallback(() => TestPanel.Children.Add(tud));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => visualHintElement = tud.HintPopup.Child as FrameworkElement);

            // will open up balloon
            EnqueueCallback(() => tud.SetText("asdfasdf"));
            // allow the hint to animate
            EnqueueDelay(500);
            EnqueueCallback(() => Assert.IsTrue(visualHintElement.ActualHeight > 20));
            EnqueueCallback(() => Assert.IsTrue(tud.HintPopup.VerticalAlignment == VerticalAlignment.Top));
            EnqueueCallback(() => Assert.IsTrue(((TranslateTransform)(((TransformGroup)visualHintElement.RenderTransform).Children[3])).Y < -20));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests that balloon gets shown on bottom if there is no space.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that balloon gets shown on bottom if there is no space.")]
        [Ignore]
        public virtual void ShouldShowBalloonOnBottomIfNecessary()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Format = new CustomTimeFormat("h:mm:ss");
            tud.Minimum = new DateTime(1900, 1, 1, 4, 30, 0);
            tud.Maximum = new DateTime(1900, 1, 1, 18, 40, 0);
            tud.Value = new DateTime(1900, 1, 1, 12, 0, 0);
            tud.Height = 30;
            tud.Width = 120;

            bool isLoaded = false;
            tud.Loaded += delegate { isLoaded = true; };

            FrameworkElement visualHintElement = null;

            tud.VerticalAlignment = VerticalAlignment.Top;

            EnqueueCallback(() => TestPanel.Children.Add(tud));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => visualHintElement = tud.HintPopup.Child as FrameworkElement);

            // will open up balloon
            EnqueueCallback(() => tud.SetText("asdfasdf"));
            // allow the hint to animate
            EnqueueDelay(500);
            EnqueueCallback(() => Assert.IsTrue(visualHintElement.ActualHeight > 20));
            EnqueueCallback(() => Assert.IsTrue(tud.HintPopup.VerticalAlignment == VerticalAlignment.Bottom));
            EnqueueCallback(() => Assert.IsTrue(((TranslateTransform)(((TransformGroup)visualHintElement.RenderTransform).Children[3])).Y == 0));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests that balloon does not get shown if there is no space.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that balloon does not get shown if there is no space.")]
        [Ignore]
        public virtual void ShouldNotShowBalloonIfNoRoom()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Format = new CustomTimeFormat("h:mm:ss");
            tud.Minimum = new DateTime(1900, 1, 1, 4, 30, 0);
            tud.Maximum = new DateTime(1900, 1, 1, 18, 40, 0);
            tud.Value = new DateTime(1900, 1, 1, 12, 0, 0);

            bool isLoaded = false;
            tud.Loaded += delegate { isLoaded = true; };

            FrameworkElement visualHintElement = null;

            tud.VerticalAlignment = VerticalAlignment.Center;
            tud.Height = TestPanel.ActualHeight - 4;

            EnqueueCallback(() => TestPanel.Children.Add(tud));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => visualHintElement = tud.HintPopup.Child as FrameworkElement);

            // will open up balloon
            EnqueueCallback(() => tud.SetText("asdfasdf"));
            // allow the hint to animate
            EnqueueDelay(500);
            EnqueueCallback(() => Assert.IsTrue(visualHintElement.ActualHeight < 5));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests that balloon hint uses custom parsing logic.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that balloon hint uses custom parsing logic.")]
        public virtual void ShouldAllowCustomParsingWhenDeterminingHint()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.Format = new CustomTimeFormat("HH:mm:ss");

            tud.Parsing += (sender, args) =>
                               {
                                   args.Value = new DateTime(1900, 1, 1, 11, 12, 13);
                                   args.Handled = true;
                               };

            TestAsync(
                    tud,
                    () => tud.SetText("asdf"),
                    () => Assert.AreEqual("<11:12:13>", tud.TimeHintContent.ToString()));
        }

        /// <summary>
        /// Tests that balloon hint coerces correctly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that balloon hint coerces correctly.")]
        public virtual void ShouldCoerceInsideBalloonHint()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();

            tud.Value = new DateTime(2100, 1, 1, 15, 0, 0);
            tud.Minimum = new DateTime(2100, 1, 1, 14, 0, 0);
            tud.Maximum = new DateTime(2100, 1, 1, 16, 0, 0);
            tud.Format = new CustomTimeFormat("HH:mm:ss");

            TestAsync(
                    tud,
                    () => tud.SetText("11:00:00"),
                    () => Assert.AreEqual("<14:00:00>", tud.TimeHintContent.ToString()),
                    () => tud.SetText("17:00:00"),
                    () => Assert.AreEqual("<16:00:00>", tud.TimeHintContent.ToString()));
        }

        /// <summary>
        /// Tests that balloon hint takes custom parsers into account.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that balloon hint takes custom parsers into account.")]
        public virtual void ShouldUseCustomParsersWhenDeterminingHint()
        {
            OverriddenTimeUpDown tud = new OverriddenTimeUpDown();
            tud.TimeParsers = new TimeParserCollection { new CustomTimeParser() };
            tud.Format = new CustomTimeFormat("HH:mm:ss");

            TestAsync(
                    tud,
                    () => tud.SetText("3 uur 40"),
                    () => Assert.AreEqual("15:40:00", tud.TimeHintContent.ToString()));
        }

        /// <summary>
        /// Tests that setting TimeHintContent throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Description("Tests that setting TimeHintContent throws exception.")]
        public virtual void ShouldThrowExceptionWhenSettingBalloonContent()
        {
            TimeUpDown tud = new TimeUpDown();
            tud.SetValue(TimeUpDown.TimeHintContentProperty, new object());
        }
        #endregion Balloon hint

        #region Bugs
        /// <summary>
        /// Tests that TimeUpDown works within a Popup control.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that TimeUpDown works within a Popup control.")]
        public void WorksInsideAPopup()
        {
            Popup popup = new Popup();
            popup.Child = new TimeUpDown();
            TestAsync(
                popup,
                () => popup.IsOpen = true);
        }
        #endregion Bugs

        #region Xaml building helpers
        /// <summary>
        /// Creates a XamlBuilder for a ContentControl which has a TimeUpdown
        /// builder as child.
        /// </summary>
        /// <returns>XamlBuilder for the ContentControl.</returns>
        private static XamlBuilder<ContentControl> CreateWrappedTimeUpDownXaml()
        {
            // might consider creating extension methods to work with child easier.
            XamlBuilder<ContentControl> builder = new XamlBuilder<ContentControl>
            {
                ExplicitNamespaces = new Dictionary<string, string>(),
                Children = new List<XamlBuilder>()
                    {
                        new XamlBuilder 
                        { 
                            ElementType = typeof(TimeUpDown),
                        }
                    }
            };
            builder.ExplicitNamespaces["controls"] = XamlBuilder.GetNamespace(typeof(TimeUpDown));

            return builder;
        }
        #endregion
    }
}