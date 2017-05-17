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

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for ListTimePickerPopup.
    /// </summary>
    [TestClass]
    [Tag("TimeInput")]
    public class ListTimePickerPopupTest : TimePickerPopupBaseTest
    {
        #region Get instances to test
        /// <summary>
        /// Gets the default time picker popup to test.
        /// </summary>
        /// <value>The default time picker popup to test.</value>
        public override TimePickerPopup DefaultTimePickerPopupToTest
        {
            get { return new ListTimePickerPopup(); }
        }

        /// <summary>
        /// Gets the time picker popups to test.
        /// </summary>
        /// <value>The time picker popups to test.</value>
        public override IEnumerable<TimePickerPopup> TimePickerPopupInstancesToTest
        {
            get { yield return DefaultTimePickerPopupToTest; }
        }

        /// <summary>
        /// Gets the overridden time picker popups to test.
        /// </summary>
        /// <value>The overridden time picker popups to test.</value>
        public override IEnumerable<IOverriddenControl> OverriddenTimePickerPopupInstancesToTest
        {
            get { yield break; }
        } 
        #endregion Get instances to test

        #region Dependency property tests
        /// <summary>
        /// Gets the time up down style property.
        /// </summary>
        protected DependencyPropertyTest<ListTimePickerPopup, Style> ListBoxStyleProperty { get; private set; }

        /// <summary>
        /// Gets the spinner property.
        /// </summary>
        protected DependencyPropertyTest<ListTimePickerPopup, Style> ListBoxItemStyleProperty { get; private set; }
        #endregion Dependency property tests

        /// <summary>
        /// Initializes a new instance of the <see cref="ListTimePickerPopupTest"/> class.
        /// </summary>
        public ListTimePickerPopupTest()
        {
            Func<ListTimePickerPopup> initializer = () => new ListTimePickerPopup();

            ListBoxStyleProperty = new DependencyPropertyTest<ListTimePickerPopup, Style>(this, "ListBoxStyle")
            {
                Property = ListTimePickerPopup.ListBoxStyleProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new Style(typeof(ListBox)), new Style(typeof(Control)) }
            };

            ListBoxItemStyleProperty = new DependencyPropertyTest<ListTimePickerPopup, Style>(this, "ListBoxItemStyle")
            {
                Property = ListTimePickerPopup.ListBoxItemStyleProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new[] { new Style(typeof(ListBoxItem)), new Style(typeof(Control)) }
            };
        }

        /// <summary>
        /// Gets the dependency property tests.
        /// </summary>
        /// <returns>A list of DP tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            IList<DependencyPropertyTestMethod> tests = base.GetDependencyPropertyTests().ToList();

            // ListBoxStyleProperty tests
            tests.Add(ListBoxStyleProperty.CheckDefaultValueTest);
            tests.Add(ListBoxStyleProperty.ChangeClrSetterTest);
            tests.Add(ListBoxStyleProperty.ChangeSetValueTest);
            tests.Add(ListBoxStyleProperty.SetNullTest);
            tests.Add(ListBoxStyleProperty.ClearValueResetsDefaultTest);
            tests.Add(ListBoxStyleProperty.CanBeStyledTest);
            tests.Add(ListBoxStyleProperty.TemplateBindTest);
            tests.Add(ListBoxStyleProperty.SetXamlAttributeTest.Bug("TODO: XAML Parser?"));
            tests.Add(ListBoxStyleProperty.SetXamlElementTest.Bug("TODO: XAML Parser?"));

            // ListBoxItemStyleProperty tests
            tests.Add(ListBoxItemStyleProperty.CheckDefaultValueTest);
            tests.Add(ListBoxItemStyleProperty.ChangeClrSetterTest);
            tests.Add(ListBoxItemStyleProperty.ChangeSetValueTest);
            tests.Add(ListBoxItemStyleProperty.SetNullTest);
            tests.Add(ListBoxItemStyleProperty.ClearValueResetsDefaultTest);
            tests.Add(ListBoxItemStyleProperty.CanBeStyledTest);
            tests.Add(ListBoxItemStyleProperty.TemplateBindTest);
            tests.Add(ListBoxItemStyleProperty.SetXamlAttributeTest.Bug("TODO: XAML Parser?"));
            tests.Add(ListBoxItemStyleProperty.SetXamlElementTest.Bug("TODO: XAML Parser?"));

            return tests;
        }

        #region control contracts
        /// <summary>
        /// Verify the control's template parts.
        /// </summary>
        public override void TemplatePartsAreDefined()
        {
            IDictionary<string, Type> parts = DefaultTimePickerPopupToTest.GetType().GetTemplateParts();
            Assert.AreEqual(1, parts.Count);
            Assert.AreEqual(typeof(ListBox), parts["ListBox"], "Failed to find expected part ListBox!");
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
            Assert.AreEqual(2, styleTypedProperties.Count, "Incorrect number of StyleType properties");
            Assert.AreEqual(typeof(ListBox), styleTypedProperties["ListBoxStyle"], "Failed to find expected style type ListBoxStyle");
            Assert.AreEqual(typeof(ListBoxItem), styleTypedProperties["ListBoxItemStyle"], "Failed to find expected style type ListBoxItemStyle");
        }
        #endregion control contracts

        /// <summary>
        /// Tests that TimeItems are being generated.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that TimeItems are being generated.")]
        public virtual void ShouldGenerateTimeItems()
        {
            ListTimePickerPopup ltp = new ListTimePickerPopup();
            
            TestAsync(
                ltp,
                () => Assert.IsTrue(ltp.TimeItemsSelection.Items.Count > 0));
        }

        /// <summary>
        /// Tests that TimeItems honor minimum.
        /// </summary>
        [TestMethod]
        [Description("Tests that TimeItems honor minimum.")]
        public virtual void ShouldTakeMinimumIntoAccount()
        {
            ListTimePickerPopup ltp = new ListTimePickerPopup()
                                          {
                                              PopupMinutesInterval = 15,
                                              Culture = new CultureInfo("nl-NL"),
                                              Format = new LongTimeFormat()
                                          };

            ltp.Minimum = new DateTime(2000, 1, 1, 2, 20, 30);
            // some settings on mac will do double digits
            Assert.IsTrue(ltp.TimeItemsSelection.Items[0].Key.EndsWith("2:30:00", StringComparison.OrdinalIgnoreCase));

            ltp.Minimum = new DateTime(2000, 1, 1, 5, 0, 0);
            // some settings on mac will do double digits
            Assert.IsTrue(ltp.TimeItemsSelection.Items[0].Key.EndsWith("5:00:00", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Tests that TimeItems honor maximum.
        /// </summary>
        [TestMethod]
        [Description("Tests that TimeItems honor maximum.")]
        public virtual void ShouldTakeMaximumIntoAccount()
        {
            ListTimePickerPopup ltp = new ListTimePickerPopup()
                                          {
                                              PopupMinutesInterval = 15,
                                              Culture = new CultureInfo("nl-NL"),
                                              Format = new LongTimeFormat()
                                          };

            ltp.Maximum = new DateTime(2000, 1, 1, 2, 20, 30);
            // some settings on mac will do double digits
            Assert.IsTrue(ltp.TimeItemsSelection.Items[ltp.TimeItemsSelection.Items.Count - 1].Key.EndsWith("2:15:00", StringComparison.OrdinalIgnoreCase));

            ltp.Maximum = new DateTime(2000, 1, 1, 5, 0, 0);
            // some settings on mac will do double digits
            Assert.IsTrue(ltp.TimeItemsSelection.Items[ltp.TimeItemsSelection.Items.Count - 1].Key.EndsWith("5:00:00", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Tests that correct item gets preselected.
        /// </summary>
        [TestMethod]
        [Description("Tests that correct item gets preselected.")]
        public virtual void ShouldSelectCorrectItemOnValueChanges()
        {
            ListTimePickerPopup ltp = new ListTimePickerPopup()
            {
                PopupMinutesInterval = 15,
                Culture = new CultureInfo("nl-NL"),
                Format = new LongTimeFormat()
            };

            ltp.Value = new DateTime(1900, 2, 2, 13, 0, 0);
            Assert.AreEqual("13:00:00", ltp.TimeItemsSelection.SelectedItem.Key);

            ltp.Value = new DateTime(2000, 3, 3, 15, 5, 0);
            Assert.AreEqual("15:00:00", ltp.TimeItemsSelection.SelectedItem.Key);
        }

        /// <summary>
        /// Tests that DatePart from value is used to generate items.
        /// </summary>
        [TestMethod]
        [Description("Tests that DatePart from value is used to generate items.")]
        public virtual void ShouldUseDatePartFromValueToGenerateItems()
        {
            ListTimePickerPopup ltp = new ListTimePickerPopup()
            {
                PopupMinutesInterval = 15,
                Culture = new CultureInfo("nl-NL"),
                Format = new LongTimeFormat()
            };

            ltp.Value = new DateTime(1900, 2, 2, 13, 0, 0);
            Assert.IsTrue(ltp.TimeItemsSelection.Items[0].Value.Value.Date == new DateTime(1900, 2, 2));

            ltp.Value = new DateTime(2000, 3, 4, 3, 0, 0);
            Assert.IsTrue(ltp.TimeItemsSelection.Items[0].Value.Value.Date == new DateTime(2000, 3, 4));
        }

        /// <summary>
        /// Tests that TimeItems are formatted.
        /// </summary>
        [TestMethod]
        [Description("Tests that TimeItems are formatted.")]
        public virtual void ShouldFormatTimeItems()
        {
            ListTimePickerPopup ltp = new ListTimePickerPopup()
                                          {
                                              PopupMinutesInterval = 15,
                                              Culture = new CultureInfo("nl-NL"),
                                              Format = new ShortTimeFormat()
                                          };
            // timeformats are different for mac than win
            Assert.IsTrue(ltp.TimeItemsSelection.Items[0].Key.EndsWith("0:00", StringComparison.OrdinalIgnoreCase));

            ltp.Culture = new CultureInfo("en-US");
            Assert.AreEqual("12:00 AM", ltp.TimeItemsSelection.Items[0].Key);

            ltp.Format = new CustomTimeFormat("HH:mm:ss");
            Assert.AreEqual("00:00:00", ltp.TimeItemsSelection.Items[0].Key);
        }

        /// <summary>
        /// Tests that popup throws exception on not supported selection enum.
        /// </summary>
        [TestMethod]
        [Description("Tests that popup throws exception on not supported selection enum.")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public virtual void ShouldThrowExceptionWhenTimeSelectionModeIsNotSupported()
        {
            ListTimePickerPopup ltp = new ListTimePickerPopup();
            ltp.PopupTimeSelectionMode = PopupTimeSelectionMode.AllowSecondsSelection;
        }

        /// <summary>
        /// Tests that TimeItemsSelection is property read-only.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [Description("Tests that TimeItemsSelection is property read-only.")]
        public virtual void ShouldThrowExceptionWhenSettingTimeItemsSelection()
        {
            ListTimePickerPopup ltp = new ListTimePickerPopup();
            ltp.SetValue(ListTimePickerPopup.TimeItemsSelectionProperty, new ItemSelectionHelper<KeyValuePair<string, DateTime?>>());
        }
    }
}
