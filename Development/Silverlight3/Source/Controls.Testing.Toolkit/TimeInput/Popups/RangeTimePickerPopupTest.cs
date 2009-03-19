// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls.Primitives;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for RangeTimePickerPopup.
    /// </summary>
    [TestClass]
    [Tag("TimeInput")]
    public class RangeTimePickerPopupTest : TimePickerPopupBaseTest
    {
        #region Get instances to test
        /// <summary>
        /// Gets the default time picker popup to test.
        /// </summary>
        /// <value>The default time picker popup to test.</value>
        public override TimePickerPopup DefaultTimePickerPopupToTest
        {
            get { return new RangeTimePickerPopup(); }
        }

        /// <summary>
        /// Gets the time picker popups to test.
        /// </summary>
        /// <value>The time picker popups to test.</value>
        public override IEnumerable<TimePickerPopup> TimePickerPopupInstancesToTest
        {
            get { yield return new RangeTimePickerPopup(); }
        }

        /// <summary>
        /// Gets the overridden time picker popups to test.
        /// </summary>
        /// <value>The overridden time picker popups to test.</value>
        public override IEnumerable<IOverriddenControl> OverriddenTimePickerPopupInstancesToTest
        {
            get { yield break; }
        } 
        #endregion

        #region Dependency property tests
        /// <summary>
        /// Gets the time up down style property.
        /// </summary>
        protected DependencyPropertyTest<RangeTimePickerPopup, Style> SliderStyleProperty { get; private set; }

        /// <summary>
        /// Gets the spinner property.
        /// </summary>
        protected DependencyPropertyTest<RangeTimePickerPopup, DataTemplate> LabelItemTemplateProperty { get; private set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeTimePickerPopupTest"/> class.
        /// </summary>
        public RangeTimePickerPopupTest()
        {
            Func<RangeTimePickerPopup> initializer = () => new RangeTimePickerPopup();

            SliderStyleProperty = new DependencyPropertyTest<RangeTimePickerPopup, Style>(this, "SliderStyle")
            {
                Property = RangeTimePickerPopup.SliderStyleProperty,
                Initializer = initializer,
                OtherValues = new[] { new Style(typeof(RangeBase)), new Style(typeof(Control)) }
            };

            LabelItemTemplateProperty = new DependencyPropertyTest<RangeTimePickerPopup, DataTemplate>(this, "LabelItemTemplate")
            {
                Property = RangeTimePickerPopup.LabelItemTemplateProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new DataTemplate[]
                {
                    new DataTemplate(),
                    new XamlBuilder<DataTemplate>().Load(),
                    (new XamlBuilder<DataTemplate> { Name = "Template" }).Load(),
                    (new XamlBuilder<DataTemplate> { Name = "Template", Children = new List<XamlBuilder> { new XamlBuilder<StackPanel>() } }).Load()
                }
            };
        }

        /// <summary>
        /// Gets the dependency property tests.
        /// </summary>
        /// <returns>A list of DP tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            IList<DependencyPropertyTestMethod> tests = base.GetDependencyPropertyTests().ToList();

            PopupMinutesIntervalProperty.DefaultValue = 5;
            BackgroundProperty.DefaultValue = new SolidColorBrush(Colors.White);

            // SliderStyleProperty tests
            tests.Add(SliderStyleProperty.ChangeClrSetterTest);
            tests.Add(SliderStyleProperty.ChangeSetValueTest);
            tests.Add(SliderStyleProperty.SetNullTest);
            tests.Add(SliderStyleProperty.ClearValueResetsDefaultTest.Bug("Style with templatebinding throws catastrophic error"));
            tests.Add(SliderStyleProperty.CanBeStyledTest);
            tests.Add(SliderStyleProperty.TemplateBindTest);
            tests.Add(SliderStyleProperty.SetXamlAttributeTest.Bug("TODO: XAML Parser?"));
            tests.Add(SliderStyleProperty.SetXamlElementTest.Bug("TODO: XAML Parser?"));

            // LabelItemTemplateProperty tests
            tests.Add(LabelItemTemplateProperty.ChangeClrSetterTest);
            tests.Add(LabelItemTemplateProperty.ChangeSetValueTest);
            tests.Add(LabelItemTemplateProperty.SetNullTest);
            tests.Add(LabelItemTemplateProperty.CanBeStyledTest);
            tests.Add(LabelItemTemplateProperty.TemplateBindTest);

            return tests;
        }

        #region control contracts
        /// <summary>
        /// Verify the control's template parts.
        /// </summary>
        public override void TemplatePartsAreDefined()
        {
            IDictionary<string, Type> parts = DefaultTimePickerPopupToTest.GetType().GetTemplateParts();
            Assert.AreEqual(8, parts.Count);
            Assert.AreEqual(typeof(RangeBase), parts["HoursSlider"], "Failed to find expected part HoursSlider!");
            Assert.AreEqual(typeof(RangeBase), parts["MinutesSlider"], "Failed to find expected part MinutesSlider!");
            Assert.AreEqual(typeof(RangeBase), parts["SecondsSlider"], "Failed to find expected part SecondsSlider!");
            Assert.AreEqual(typeof(Panel), parts["HoursPanel"], "Failed to find expected part HoursPanel!");
            Assert.AreEqual(typeof(Panel), parts["MinutesPanel"], "Failed to find expected part MinutesPanel!");
            Assert.AreEqual(typeof(Panel), parts["SecondsPanel"], "Failed to find expected part SecondsPanel!");
            Assert.AreEqual(typeof(ButtonBase), parts["Commit"], "Failed to find expected part Commit!");
            Assert.AreEqual(typeof(ButtonBase), parts["Cancel"], "Failed to find expected part Cancel!");
        }

        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> states = DefaultTimePickerPopupToTest.GetType().GetVisualStates();
            Assert.AreEqual(12, states.Count, "Incorrect number of template states");
            Assert.AreEqual("CommonStates", states["Normal"], "Failed to find expected state Normal!");
            Assert.AreEqual("CommonStates", states["MouseOver"], "Failed to find expected state MouseOver!");
            Assert.AreEqual("CommonStates", states["Pressed"], "Failed to find expected state Pressed!");
            Assert.AreEqual("CommonStates", states["Disabled"], "Failed to find expected state Disabled!");
            Assert.AreEqual("FocusStates", states["Focused"], "Failed to find expected state Focused!");
            Assert.AreEqual("FocusStates", states["Unfocused"], "Failed to find expected state Unfocused!");
            Assert.AreEqual("ContainedByPickerStates", states["Contained"], "Failed to find expected state Contained!");
            Assert.AreEqual("ContainedByPickerStates", states["NotContained"], "Failed to find expected state NotContained!");
            Assert.AreEqual("PopupModeStates", states["AllowSecondsAndDesignatorsSelection"], "Failed to find expected state AllowSecondsAndDesignatorsSelection!");
            Assert.AreEqual("PopupModeStates", states["AllowTimeDesignatorsSelection"], "Failed to find expected state AllowTimeDesignatorsSelection!");
            Assert.AreEqual("PopupModeStates", states["AllowSecondsSelection"], "Failed to find expected state AllowSecondsSelection!");
            Assert.AreEqual("PopupModeStates", states["HoursAndMinutesOnly"], "Failed to find expected state HoursAndMinutesOnly!");
        }

        /// <summary>
        /// Verify the control's style typed properties.
        /// </summary>
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> styleTypedProperties = DefaultTimePickerPopupToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(1, styleTypedProperties.Count, "Incorrect number of StyleType properties");
            Assert.AreEqual(typeof(RangeBase), styleTypedProperties["SliderStyle"], "Failed to find expected style type ListBoxStyle");
        }
        #endregion control contracts

        /// <summary>
        /// Tests that sliders respond to Value changes.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that sliders respond to Value changes.")]
        public virtual void ShouldRepresentValueInSliders()
        {
            OverriddenRangeTimePickerPopup stp = new OverriddenRangeTimePickerPopup();

            TestAsync(
                stp,
                () => stp.Value = new DateTime(1900, 1, 1, 14, 12, 14),
                () => Assert.IsTrue(stp.OverriddenHoursSlider.Value == 14),
                () => Assert.IsTrue(stp.OverriddenMinutesSlider.Value == 12),
                () => Assert.IsTrue(stp.OverriddenSecondsSlider.Value == 14),
                () => stp.Value = new DateTime(1900, 1, 1, 8, 10, 20),
                () => Assert.IsTrue(stp.OverriddenHoursSlider.Value == 8),
                () => Assert.IsTrue(stp.OverriddenMinutesSlider.Value == 10),
                () => Assert.IsTrue(stp.OverriddenSecondsSlider.Value == 20));
        }

        /// <summary>
        /// Tests that Value responds to slider changes.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that Value responds to slider changes.")]
        public virtual void ShouldUpdateValueWithSliders()
        {
            OverriddenRangeTimePickerPopup stp = new OverriddenRangeTimePickerPopup
            {
                PopupSecondsInterval = 1,
                PopupMinutesInterval = 1
            };

            DateTime value = DateTime.Now;
            int valueCount = 0;
            stp.ValueChanged += (sender, e) =>
                                    {
                                        valueCount ++;
                                        value = e.NewValue.Value;
                                    };

            TestAsync(
                stp,
                () => stp.OverriddenHoursSlider.Value = 8,
                () => stp.OverriddenMinutesSlider.Value = 10,
                () => stp.OverriddenSecondsSlider.Value = 12,
                () => Assert.AreEqual(3, valueCount),
                () => Assert.AreEqual(new DateTime(1900, 1, 1, 8, 10, 12).TimeOfDay, value.TimeOfDay),
                () => stp.OverriddenHoursSlider.Value = 14,
                () => Assert.AreEqual(4, valueCount),
                () => Assert.AreEqual(new DateTime(1900, 1, 1, 14, 10, 12).TimeOfDay, value.TimeOfDay));
        }

        /// <summary>
        /// Tests that boundaries are respected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that boundaries are respected.")]
        public virtual void ShouldRespectMinimumAndMaximum()
        {
            OverriddenRangeTimePickerPopup stp = new OverriddenRangeTimePickerPopup
                                                      {
                                                          PopupSecondsInterval = 1, 
                                                          PopupMinutesInterval = 1
                                                      };

            TestAsync(
                stp,
                () => stp.Minimum = new DateTime(1900, 1, 1, 5, 5, 5),
                () => stp.Maximum = new DateTime(1900, 1, 1, 15, 15, 15),
                () => stp.OverriddenHoursSlider.Value = 4,
                () => Assert.AreEqual(stp.Minimum.Value.TimeOfDay, stp.Value.Value.TimeOfDay),
                () => stp.OverriddenHoursSlider.Value = 16,
                () => Assert.AreEqual(stp.Maximum.Value.TimeOfDay, stp.Value.Value.TimeOfDay),
                () => Assert.AreEqual(15, stp.OverriddenHoursSlider.Value),
                () => Assert.AreEqual(15, stp.OverriddenMinutesSlider.Value),
                () => Assert.AreEqual(15, stp.OverriddenSecondsSlider.Value));
        }

        /// <summary>
        /// Tests that labels are generated.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that labels are generated.")]
        [Ignore]
        public virtual void ShouldGenerateFormattedLabels()
        {
            OverriddenRangeTimePickerPopup stp = new OverriddenRangeTimePickerPopup
            {
                PopupSecondsInterval = 1,
                PopupMinutesInterval = 1
            };

            TestAsync(
                stp,
                () => Assert.IsTrue(stp.OverriddenHourContainer.Children.Count > 0),
                () => stp.Culture = new CultureInfo("en-US"),
                () => stp.Format = new LongTimeFormat(),
                () => Assert.AreEqual("10 PM", ((ContentPresenter)stp.OverriddenHourContainer.Children[0]).Content));
        }

        /// <summary>
        /// Tests that value is being snapped correctly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that value is being snapped correctly.")]
        public virtual void ShouldSnapValueToInterval()
        {
            OverriddenRangeTimePickerPopup stp = new OverriddenRangeTimePickerPopup
            {
                PopupSecondsInterval = 5,
                PopupMinutesInterval = 5
            };

            TestAsync(
                stp,
                () => stp.OverriddenHoursSlider.Value = 5,
                () => stp.OverriddenMinutesSlider.Value = 26,
                () => stp.OverriddenSecondsSlider.Value = 26,
                () => Assert.AreEqual(new DateTime(1900, 1, 1, 5, 25, 25).TimeOfDay, stp.Value.Value.TimeOfDay),
                () => Assert.AreEqual(25, stp.OverriddenMinutesSlider.Value),
                () => Assert.AreEqual(25, stp.OverriddenSecondsSlider.Value));
        }
    }
}
