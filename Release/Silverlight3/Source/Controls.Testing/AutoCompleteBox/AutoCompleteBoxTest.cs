// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// AutoCompleteBox control tests.
    /// </summary>
    [TestClass]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Test cases exercise many classes.")]
    public partial class AutoCompleteBoxTest : ControlTest
    {
        /// <summary>
        /// Gets a default instance of Control (or a derived type) to test.
        /// </summary>
        public override Control DefaultControlToTest
        {
            get
            {
                return new AutoCompleteBox();
            }
        }

        /// <summary>
        /// Gets instances of Control (or derived types) to test.
        /// </summary>
        public override IEnumerable<Control> ControlsToTest
        {
            get { yield break; }
        }

        /// <summary>
        /// Gets the overridden controls to test.
        /// </summary>
        public override IEnumerable<IOverriddenControl> OverriddenControlsToTest
        {
            get { yield break; }
        }

        /// <summary>
        /// Gets the IsTextCompletionEnabled dependency property test.
        /// </summary>
        protected DependencyPropertyTest<AutoCompleteBox, bool> IsTextCompletionEnabledProperty { get; private set; }

        /// <summary>
        /// Gets the IsDropDownOpen dependency property test.
        /// </summary>
        protected DependencyPropertyTest<AutoCompleteBox, bool> IsDropDownOpenProperty { get; private set; }

        /// <summary>
        /// Gets the ItemContainerStyle dependency property test.
        /// </summary>
        protected DependencyPropertyTest<AutoCompleteBox, Style> ItemContainerStyleProperty { get; private set; }

        /// <summary>
        /// Gets the ItemFilter dependency property test.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by DependencyPropertyTest implementation.")]
        protected DependencyPropertyTest<AutoCompleteBox, AutoCompleteFilterPredicate<object>> ItemFilterProperty { get; private set; }

        /// <summary>
        /// Gets the ItemsSource dependency property test.
        /// </summary>
        protected DependencyPropertyTest<AutoCompleteBox, IEnumerable> ItemsSourceProperty { get; private set; }

        /// <summary>
        /// Gets the ItemTemplate dependency property test.
        /// </summary>
        protected DependencyPropertyTest<AutoCompleteBox, DataTemplate> ItemTemplateProperty { get; private set; }

        /// <summary>
        /// Gets the MaxDropDownHeight dependency property test.
        /// </summary>
        protected DependencyPropertyTest<AutoCompleteBox, double> MaxDropDownHeightProperty { get; private set; }

        /// <summary>
        /// Gets the MinimumPopulateDelay dependency property test.
        /// </summary>
        protected DependencyPropertyTest<AutoCompleteBox, int> MinimumPopulateDelayProperty { get; private set; }

        /// <summary>
        /// Gets the MinimumPrefixLength dependency property test.
        /// </summary>
        protected DependencyPropertyTest<AutoCompleteBox, int> MinimumPrefixLengthProperty { get; private set; }

        /// <summary>
        /// Gets the FilterMode dependency property test.
        /// </summary>
        protected DependencyPropertyTest<AutoCompleteBox, AutoCompleteFilterMode> FilterModeProperty { get; private set; }

        /// <summary>
        /// Gets the SearchText dependency property test.
        /// </summary>
        protected DependencyPropertyTest<AutoCompleteBox, string> SearchTextProperty { get; private set; }

        /// <summary>
        /// Gets the SelectedItem dependency property test.
        /// </summary>
        protected DependencyPropertyTest<AutoCompleteBox, object> SelectedItemProperty { get; private set; }

        /// <summary>
        /// Gets the Text dependency property test.
        /// </summary>
        protected DependencyPropertyTest<AutoCompleteBox, string> TextProperty { get; private set; }

        /// <summary>
        /// Gets the TextBoxStyle dependency property test.
        /// </summary>
        protected DependencyPropertyTest<AutoCompleteBox, Style> TextBoxStyleProperty { get; private set; }

        /// <summary>
        /// Gets the TextFilter dependency property test.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by DependencyPropertyTest implementation.")]
        protected DependencyPropertyTest<AutoCompleteBox, AutoCompleteFilterPredicate<string>> TextFilterProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the AutoCompleteBoxTest class.
        /// </summary>
        public AutoCompleteBoxTest()
        {
            // Default control template values, originally from TextBox
            BackgroundProperty.DefaultValue = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xff, 0xff));
            ForegroundProperty.DefaultValue = new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0x00, 0x00));
            BorderThicknessProperty.DefaultValue = new Thickness(1);
            PaddingProperty.DefaultValue = new Thickness(2);

            LinearGradientBrush lgb = new LinearGradientBrush();
            lgb.StartPoint = new Point(0.5, 0);
            lgb.EndPoint = new Point(0.5, 1);
            lgb.GradientStops.Add(new GradientStop { Color = Color.FromArgb(0xff, 0xa3, 0xae, 0xb9), Offset = 0 });
            lgb.GradientStops.Add(new GradientStop { Color = Color.FromArgb(0xff, 0x83, 0x99, 0xa9), Offset = 0.375 });
            lgb.GradientStops.Add(new GradientStop { Color = Color.FromArgb(0xff, 0x71, 0x85, 0x97), Offset = 0.375 });
            lgb.GradientStops.Add(new GradientStop { Color = Color.FromArgb(0xff, 0x61, 0x75, 0x84), Offset = 1 });
            BorderBrushProperty.DefaultValue = lgb;

            // AutoCompleteBox dependency properties
            Func<AutoCompleteBox> initializer = () => new AutoCompleteBox();
            Func<AutoCompleteBox> initializerWithContent = () =>
                {
                    AutoCompleteBox autoCompleteBox = (AutoCompleteBox)DefaultControlToTest;
                    autoCompleteBox.ItemsSource = new string[] { "a", "aa", "aaa" };
                    autoCompleteBox.PopulateComplete();
                    return autoCompleteBox;
                };
            Func<AutoCompleteBox> initializerWithContentAndTextValue = () =>
                {
                    AutoCompleteBox acb = initializerWithContent();
                    acb.Text = "aa";
                    return acb;
                };
            IsDropDownOpenProperty = new DependencyPropertyTest<AutoCompleteBox, bool>(this, "IsDropDownOpen")
                {
                    Property = AutoCompleteBox.IsDropDownOpenProperty,
                    Initializer = initializerWithContentAndTextValue,
                    DefaultValue = false,
                    OtherValues = new bool[] { true },
                    InvalidValues = new Dictionary<bool, Type> { }
                };
            IsTextCompletionEnabledProperty = new DependencyPropertyTest<AutoCompleteBox, bool>(this, "IsTextCompletionEnabled")
                {
                    Property = AutoCompleteBox.IsTextCompletionEnabledProperty,
                    Initializer = initializer,
                    DefaultValue = false,
                    OtherValues = new bool[] { true },
                    InvalidValues = new Dictionary<bool, Type> { }
                };
            ItemContainerStyleProperty = new DependencyPropertyTest<AutoCompleteBox, Style>(this, "ItemContainerStyle")
            {
                Property = AutoCompleteBox.ItemContainerStyleProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new Style[] { new Style(typeof(AutoCompleteBox)), new Style(typeof(Control)) }
            };
            ItemFilterProperty = new DependencyPropertyTest<AutoCompleteBox, AutoCompleteFilterPredicate<object>>(this, "ItemFilter")
                {
                    Property = AutoCompleteBox.ItemFilterProperty,
                    Initializer = initializer,
                    DefaultValue = null,
                    OtherValues = new AutoCompleteFilterPredicate<object>[] { (s, i) => true },
                    InvalidValues = new Dictionary<AutoCompleteFilterPredicate<object>, Type> { }
                };
            ItemsSourceProperty = new DependencyPropertyTest<AutoCompleteBox, IEnumerable>(this, "ItemsSource")
                {
                    Property = AutoCompleteBox.ItemsSourceProperty,
                    Initializer = initializer,
                    DefaultValue = null,
                    OtherValues = new IEnumerable[] { new int[0], new int[] { 0 }, new int[] { 1, 2, 3 }, new string[] { "string", "strung" } },
                    InvalidValues = new Dictionary<IEnumerable, Type> { }
                };
            ItemTemplateProperty = new DependencyPropertyTest<AutoCompleteBox, DataTemplate>(this, "ItemTemplate")
            {
                Property = AutoCompleteBox.ItemTemplateProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new DataTemplate[] { new DataTemplate() }
            };
            MaxDropDownHeightProperty = new DependencyPropertyTest<AutoCompleteBox, double>(this, "MaxDropDownHeight")
                {
                    Property = AutoCompleteBox.MaxDropDownHeightProperty,
                    Initializer = initializer,
                    DefaultValue = double.PositiveInfinity,
                    OtherValues = new double[] { 0.0, 1.0, 100.0, 1000000.0 },
                    InvalidValues = new Dictionary<double, Type> { { -1.0, typeof(ArgumentException) } }
                };
            MinimumPopulateDelayProperty = new DependencyPropertyTest<AutoCompleteBox, int>(this, "MinimumPopulateDelay")
                {
                    Property = AutoCompleteBox.MinimumPopulateDelayProperty,
                    Initializer = initializer,
                    DefaultValue = 0,
                    OtherValues = new int[] { 100, 1000 },
                    InvalidValues = new Dictionary<int, Type> { { -1, typeof(ArgumentException) } }
                };
            MinimumPrefixLengthProperty = new DependencyPropertyTest<AutoCompleteBox, int>(this, "MinimumPrefixLength")
                {
                    Property = AutoCompleteBox.MinimumPrefixLengthProperty,
                    Initializer = initializer,
                    DefaultValue = 1,
                    OtherValues = new int[] { -1, 0, 100 },
                    InvalidValues = new Dictionary<int, Type> { { -2, typeof(ArgumentException) } }
                };
            FilterModeProperty = new DependencyPropertyTest<AutoCompleteBox, AutoCompleteFilterMode>(this, "FilterMode")
                {
                    Property = AutoCompleteBox.FilterModeProperty,
                    Initializer = initializer,
                    DefaultValue = AutoCompleteFilterMode.StartsWith,
                    OtherValues = new AutoCompleteFilterMode[] { AutoCompleteFilterMode.ContainsCaseSensitive, AutoCompleteFilterMode.Custom },
                    InvalidValues = new Dictionary<AutoCompleteFilterMode, Type> { { (AutoCompleteFilterMode)111, typeof(ArgumentException) } }
                };
            SearchTextProperty = new DependencyPropertyTest<AutoCompleteBox, string>(this, "SearchText")
            {
                Property = AutoCompleteBox.SearchTextProperty,
                Initializer = initializer,
                DefaultValue = "",
                OtherValues = new string[] { },
                InvalidValues = new Dictionary<string, Type> { }
            };
            SelectedItemProperty = new DependencyPropertyTest<AutoCompleteBox, object>(this, "SelectedItem")
                {
                    Property = AutoCompleteBox.SelectedItemProperty,
                    Initializer = initializer,
                    DefaultValue = null,
                    OtherValues = new object[] { "aa" },
                    InvalidValues = new Dictionary<object, Type> { { "bb", typeof(Exception) } }
                };
            TextProperty = new DependencyPropertyTest<AutoCompleteBox, string>(this, "Text")
                {
                    Property = AutoCompleteBox.TextProperty,
                    Initializer = initializerWithContent,
                    DefaultValue = "",
                    OtherValues = new string[] { "a", "aa", "bbb" },
                    InvalidValues = new Dictionary<string, Type> { }
                };
            TextBoxStyleProperty = new DependencyPropertyTest<AutoCompleteBox, Style>(this, "TextBoxStyle")
            {
                Property = AutoCompleteBox.TextBoxStyleProperty,
                Initializer = initializer,
                DefaultValue = null,
                OtherValues = new Style[] { new Style(typeof(TextBox)), new Style(typeof(Control)) }
            };
            TextFilterProperty = new DependencyPropertyTest<AutoCompleteBox, AutoCompleteFilterPredicate<string>>(this, "TextFilter")
            {
                Property = AutoCompleteBox.TextFilterProperty,
                Initializer = initializer,
                DefaultValue = null, // Actual value not available
                OtherValues = new AutoCompleteFilterPredicate<string>[] { (s, i) => true },
                InvalidValues = new Dictionary<AutoCompleteFilterPredicate<string>, Type> { }
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

            // IsDropDownOpenProperty tests
            tests.Add(IsDropDownOpenProperty.BindingTest);
            tests.Add(IsDropDownOpenProperty.CanBeStyledTest); // .Bug("532675,663508"));
            tests.Add(IsDropDownOpenProperty.ChangeClrSetterTest);
            tests.Add(IsDropDownOpenProperty.ChangeSetValueTest);
            tests.Add(IsDropDownOpenProperty.ChangesVisualStateTest(false, true, "PopupOpened"));
            tests.Add(IsDropDownOpenProperty.ChangesVisualStateTest(true, false, "PopupClosed"));
            tests.Add(IsDropDownOpenProperty.CheckDefaultValueTest);
            tests.Add(IsDropDownOpenProperty.ClearValueResetsDefaultTest);

            // This test should fail because the IsDropDownOpen logic requires
            // that the Text value is set before the IsDropDownOpen, or else
            // the property reverts.
            // tests.Add(IsDropDownOpenProperty.SetXamlAttributeTest);
            tests.Add(IsDropDownOpenProperty.SetXamlElementTest.Bug("XAML parser bug"));
            tests.Add(IsDropDownOpenProperty.TemplateBindTest);

            // IsTextCompletionEnabledProperty tests
            tests.Add(IsTextCompletionEnabledProperty.BindingTest);
            tests.Add(IsTextCompletionEnabledProperty.CanBeStyledTest);
            tests.Add(IsTextCompletionEnabledProperty.ChangeClrSetterTest);
            tests.Add(IsTextCompletionEnabledProperty.ChangeSetValueTest);
            tests.Add(IsTextCompletionEnabledProperty.CheckDefaultValueTest);
            tests.Add(IsTextCompletionEnabledProperty.ClearValueResetsDefaultTest);
            tests.Add(IsTextCompletionEnabledProperty.SetXamlAttributeTest);
            tests.Add(IsTextCompletionEnabledProperty.SetXamlElementTest.Bug("XAML parser bug"));
            tests.Add(IsTextCompletionEnabledProperty.TemplateBindTest);

            // ItemContainerStyleProperty tests
            tests.Add(ItemContainerStyleProperty.BindingTest);
            tests.Add(ItemContainerStyleProperty.CanBeStyledTest);
            tests.Add(ItemContainerStyleProperty.ChangeClrSetterTest);
            tests.Add(ItemContainerStyleProperty.ChangeSetValueTest);
            tests.Add(ItemContainerStyleProperty.CheckDefaultValueTest);
            tests.Add(ItemContainerStyleProperty.ClearValueResetsDefaultTest);
            tests.Add(ItemContainerStyleProperty.SetNullTest);
            // tests.Add(ItemContainerStyleProperty.SetXamlAttributeTest.Bug("XamlConverter cannot serialize"));
            // tests.Add(ItemContainerStyleProperty.SetXamlElementTest.Bug("XamlConverter cannot serialize"));
            tests.Add(ItemContainerStyleProperty.TemplateBindTest);

            // ItemFilterProperty tests
            tests.Add(ItemFilterProperty.BindingTest);
            tests.Add(ItemFilterProperty.CanBeStyledTest);
            tests.Add(ItemFilterProperty.ChangeClrSetterTest);
            tests.Add(ItemFilterProperty.ChangeSetValueTest);
            tests.Add(ItemFilterProperty.CheckDefaultValueTest);
            tests.Add(ItemFilterProperty.ClearValueResetsDefaultTest);
            tests.Add(ItemFilterProperty.SetNullTest);
            // tests.Add(ItemFilterProperty.SetXamlAttributeTest.Bug("XamlConverter cannot serialize"));
            // tests.Add(ItemFilterProperty.SetXamlElementTest.Bug("XamlConverter cannot serialize"));
            tests.Add(ItemFilterProperty.TemplateBindTest);

            // ItemsSourceProperty tests
            tests.Add(ItemsSourceProperty.BindingTest);
            // tests.Add(ItemsSourceProperty.CanBeStyledTest.Bug("Platform exception"));
            tests.Add(ItemsSourceProperty.ChangeClrSetterTest);
            tests.Add(ItemsSourceProperty.ChangeSetValueTest);
            tests.Add(ItemsSourceProperty.CheckDefaultValueTest);
            tests.Add(ItemsSourceProperty.ClearValueResetsDefaultTest);
            tests.Add(ItemsSourceProperty.IsContentPropertyTest.Bug("666336", true));
            tests.Add(ItemsSourceProperty.SetNullTest);
            tests.Add(ItemsSourceProperty.SetXamlAttributeTest);
            tests.Add(ItemsSourceProperty.SetXamlContentTest.Bug("666336", true));
            tests.Add(ItemsSourceProperty.SetXamlElementTest);
            tests.Add(ItemsSourceProperty.TemplateBindTest);

            // ItemTemplateProperty tests
            tests.Add(ItemTemplateProperty.BindingTest);
            tests.Add(ItemTemplateProperty.CanBeStyledTest);
            tests.Add(ItemTemplateProperty.ChangeClrSetterTest);
            tests.Add(ItemTemplateProperty.ChangeSetValueTest);
            tests.Add(ItemTemplateProperty.CheckDefaultValueTest);
            tests.Add(ItemTemplateProperty.ClearValueResetsDefaultTest);
            tests.Add(ItemTemplateProperty.SetNullTest);
            // tests.Add(ItemTemplateProperty.SetXamlAttributeTest.Bug("XamlConverter cannot serialize"));
            // tests.Add(ItemTemplateProperty.SetXamlElementTest.Bug("XamlConverter cannot serialize"));
            tests.Add(ItemTemplateProperty.TemplateBindTest);

            // MaxDropDownHeightProperty tests
            tests.Add(MaxDropDownHeightProperty.BindingTest);
            tests.Add(MaxDropDownHeightProperty.CanBeStyledTest);
            tests.Add(MaxDropDownHeightProperty.ChangeClrSetterTest);
            tests.Add(MaxDropDownHeightProperty.ChangeSetValueTest);
            tests.Add(MaxDropDownHeightProperty.CheckDefaultValueTest);
            tests.Add(MaxDropDownHeightProperty.ClearValueResetsDefaultTest);
            tests.Add(MaxDropDownHeightProperty.InvalidValueFailsTest);
            tests.Add(MaxDropDownHeightProperty.SetXamlAttributeTest);
            tests.Add(MaxDropDownHeightProperty.SetXamlElementTest);
            tests.Add(MaxDropDownHeightProperty.TemplateBindTest);

            // MinimumPopulateDelayProperty tests
            tests.Add(MinimumPopulateDelayProperty.BindingTest);
            tests.Add(MinimumPopulateDelayProperty.CanBeStyledTest);
            tests.Add(MinimumPopulateDelayProperty.ChangeClrSetterTest);
            tests.Add(MinimumPopulateDelayProperty.ChangeSetValueTest);
            tests.Add(MinimumPopulateDelayProperty.CheckDefaultValueTest);
            tests.Add(MinimumPopulateDelayProperty.ClearValueResetsDefaultTest);
            tests.Add(MinimumPopulateDelayProperty.InvalidValueFailsTest);
            tests.Add(MinimumPopulateDelayProperty.SetXamlAttributeTest);
            tests.Add(MinimumPopulateDelayProperty.SetXamlElementTest);
            tests.Add(MinimumPopulateDelayProperty.TemplateBindTest);

            // MinimumPrefixLengthProperty tests
            tests.Add(MinimumPrefixLengthProperty.BindingTest);
            tests.Add(MinimumPrefixLengthProperty.CanBeStyledTest);
            tests.Add(MinimumPrefixLengthProperty.ChangeClrSetterTest);
            tests.Add(MinimumPrefixLengthProperty.ChangeSetValueTest);
            tests.Add(MinimumPrefixLengthProperty.CheckDefaultValueTest);
            tests.Add(MinimumPrefixLengthProperty.ClearValueResetsDefaultTest);
            tests.Add(MinimumPrefixLengthProperty.InvalidValueFailsTest.Bug("665614", true));
            tests.Add(MinimumPrefixLengthProperty.SetXamlAttributeTest);
            tests.Add(MinimumPrefixLengthProperty.SetXamlElementTest);
            tests.Add(MinimumPrefixLengthProperty.TemplateBindTest);

            // FilterModeProperty tests
            tests.Add(FilterModeProperty.BindingTest);
            tests.Add(FilterModeProperty.CanBeStyledTest);
            tests.Add(FilterModeProperty.ChangeClrSetterTest);
            tests.Add(FilterModeProperty.ChangeSetValueTest);
            tests.Add(FilterModeProperty.CheckDefaultValueTest);
            tests.Add(FilterModeProperty.ClearValueResetsDefaultTest);
            tests.Add(FilterModeProperty.InvalidValueFailsTest);
            tests.Add(FilterModeProperty.SetXamlAttributeTest);
            tests.Add(FilterModeProperty.SetXamlElementTest);
            tests.Add(FilterModeProperty.TemplateBindTest);

            // SearchTextProperty tests
            tests.Add(SearchTextProperty.CheckDefaultValueTest);
            tests.Add(SearchTextProperty.IsReadOnlyTest);

            // SelectedItemProperty tests
            tests.Add(SelectedItemProperty.BindingTest);
            tests.Add(SelectedItemProperty.CanBeStyledTest);
            tests.Add(SelectedItemProperty.ChangeClrSetterTest);
            tests.Add(SelectedItemProperty.ChangeSetValueTest);
            tests.Add(SelectedItemProperty.CheckDefaultValueTest);
            tests.Add(SelectedItemProperty.ClearValueResetsDefaultTest);
            tests.Add(SelectedItemProperty.InvalidValueIsIgnoredTest.Bug("666396"));
            tests.Add(SelectedItemProperty.SetNullTest);
            tests.Add(SelectedItemProperty.SetXamlAttributeTest);
            tests.Add(SelectedItemProperty.SetXamlElementTest);
            tests.Add(SelectedItemProperty.TemplateBindTest);

            // TextProperty tests
            tests.Add(TextProperty.BindingTest);
            tests.Add(TextProperty.CanBeStyledTest);
            tests.Add(TextProperty.ChangeClrSetterTest);
            tests.Add(TextProperty.ChangeSetValueTest);
            tests.Add(TextProperty.CheckDefaultValueTest);
            tests.Add(TextProperty.ClearValueResetsDefaultTest);
            tests.Add(TextProperty.SetNullTest);
            tests.Add(TextProperty.SetXamlAttributeTest);
            tests.Add(TextProperty.SetXamlElementTest);
            tests.Add(TextProperty.TemplateBindTest);

            // TextBoxStyleProperty tests
            tests.Add(TextBoxStyleProperty.BindingTest);
            tests.Add(TextBoxStyleProperty.CanBeStyledTest);
            tests.Add(TextBoxStyleProperty.ChangeClrSetterTest);
            tests.Add(TextBoxStyleProperty.ChangeSetValueTest);
            tests.Add(TextBoxStyleProperty.CheckDefaultValueTest);
            tests.Add(TextBoxStyleProperty.ClearValueResetsDefaultTest);
            tests.Add(TextBoxStyleProperty.SetNullTest);
            // tests.Add(TextBoxStyleProperty.SetXamlAttributeTest.Bug("XamlConverter cannot serialize"));
            // tests.Add(TextBoxStyleProperty.SetXamlElementTest.Bug("XamlConverter cannot serialize"));
            tests.Add(TextBoxStyleProperty.TemplateBindTest);

            // TextFilterProperty tests
            tests.Add(TextFilterProperty.BindingTest);
            tests.Add(TextFilterProperty.CanBeStyledTest);
            tests.Add(TextFilterProperty.ChangeClrSetterTest);
            tests.Add(TextFilterProperty.ChangeSetValueTest);
            // tests.Add(TextFilterProperty.CheckDefaultValueTest.Bug("Unable to provide same instance as actual starting value"));
            // tests.Add(TextFilterProperty.ClearValueResetsDefaultTest.Bug("Unable to provide same instance as actual starting value"));
            tests.Add(TextFilterProperty.SetNullTest);
            // tests.Add(TextFilterProperty.SetXamlAttributeTest.Bug("XamlConverter cannot serialize"));
            // tests.Add(TextFilterProperty.SetXamlElementTest.Bug("XamlConverter cannot serialize"));
            tests.Add(TextFilterProperty.TemplateBindTest);

            return tests;
        }

        /// <summary>
        /// Creates an AutoCompleteBox control instance with a large set of 
        /// string data.
        /// </summary>
        /// <returns>Returns a new AutoCompleteBox with a set height and 
        /// ItemsSource.</returns>
        private AutoCompleteBox CreateSimpleStringAutoComplete()
        {
            AutoCompleteBox ac = (AutoCompleteBox)DefaultControlToTest;
            ac.ItemsSource = CreateSimpleStringArray();
            ac.Height = 32;
            return ac;
        }

        /// <summary>
        /// Creates a testable AutoCompleteBox instance.
        /// </summary>
        /// <returns>Returns a new AutoCompleteBox instance.</returns>
        private static OverriddenAutoCompleteBox GetDerivedAutoComplete()
        {
            return new OverriddenAutoCompleteBox()
            {
                ItemsSource = CreateSimpleStringArray(),
                Height = 32,
            };
        }

        /// <summary>
        /// Retrieves a defined predicate filter through a new AutoCompleteBox 
        /// control instance.
        /// </summary>
        /// <param name="mode">The FilterMode of interest.</param>
        /// <returns>Returns the predicate instance.</returns>
        private static AutoCompleteFilterPredicate<string> GetFilter(AutoCompleteFilterMode mode)
        {
            return new AutoCompleteBox { FilterMode = mode }
                .TextFilter;
        }

        /// <summary>
        /// Verifies the GetSelectionAdapter method is called.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies the GetSelectionAdapter method is called.")]
        public void VerifyGetSelectionAdapter()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            MethodCallMonitor getSelectionAdapterMonitor = control.GetSelectionAdapterActions.CreateMonitor();
            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => getSelectionAdapterMonitor.AssertCalled("The GetSelectionAdapter method was not called."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Verifies the OnFormatValue method is called.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies the OnFormatValue method is called.")]
        public void VerifyOnFormatValue()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            MethodCallMonitor formatValueMonitor = control.FormatValueActions.CreateMonitor();
            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.SearchText == "a");
            EnqueueCallback(() => formatValueMonitor.AssertCalled("The OnFormatValue method was not called."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Create a new SelectorSelectionAdapter.
        /// </summary>
        [TestMethod]
        [Description("Create a new SelectorSelectionAdapter.")]
        public void SelectionAdapterConstructor()
        {
            SelectorSelectionAdapter adapter = new SelectorSelectionAdapter();
            Assert.IsNull(adapter.SelectorControl, "A selection control was present.");
        }

        /// <summary>
        /// Validate all the standard filters.
        /// </summary>
        [TestMethod]
        [Description("Validate the standard filters.")]
        public void TestSearchFilters()
        {
            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.Contains)("am", "name"));
            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.Contains)("AME", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.Contains)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.ContainsCaseSensitive)("na", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.ContainsCaseSensitive)("AME", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.ContainsCaseSensitive)("hello", "name"));

            Assert.IsNull(GetFilter(AutoCompleteFilterMode.Custom));
            Assert.IsNull(GetFilter(AutoCompleteFilterMode.None));

            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.Equals)("na", "na"));
            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.Equals)("na", "NA"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.Equals)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.EqualsCaseSensitive)("na", "na"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.EqualsCaseSensitive)("na", "NA"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.EqualsCaseSensitive)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.StartsWith)("na", "name"));
            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.StartsWith)("NAM", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.StartsWith)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.StartsWithCaseSensitive)("na", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.StartsWithCaseSensitive)("NAM", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.StartsWithCaseSensitive)("hello", "name"));
        }

        /// <summary>
        /// Validate the ordinal filters.
        /// </summary>
        [TestMethod]
        [Description("Validate the ordinal filters.")]
        public void TestOrdinalSearchFilters()
        {
            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.ContainsOrdinal)("am", "name"));
            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.ContainsOrdinal)("AME", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.ContainsOrdinal)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.ContainsOrdinalCaseSensitive)("na", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.ContainsOrdinalCaseSensitive)("AME", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.ContainsOrdinalCaseSensitive)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.EqualsOrdinal)("na", "na"));
            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.EqualsOrdinal)("na", "NA"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.EqualsOrdinal)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.EqualsOrdinalCaseSensitive)("na", "na"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.EqualsOrdinalCaseSensitive)("na", "NA"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.EqualsOrdinalCaseSensitive)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.StartsWithOrdinal)("na", "name"));
            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.StartsWithOrdinal)("NAM", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.StartsWithOrdinal)("hello", "name"));

            Assert.IsTrue(GetFilter(AutoCompleteFilterMode.StartsWithOrdinalCaseSensitive)("na", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.StartsWithOrdinalCaseSensitive)("NAM", "name"));
            Assert.IsFalse(GetFilter(AutoCompleteFilterMode.StartsWithOrdinalCaseSensitive)("hello", "name"));
        }

        /// <summary>
        /// Tests that invalid search mode values throw.
        /// </summary>
        [TestMethod]
        [Description("Tests that invalid search mode values throw.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidFilterMode()
        {
            AutoCompleteBox control = new AutoCompleteBox();
            AutoCompleteFilterMode invalid = (AutoCompleteFilterMode)4321;
            control.SetValue(AutoCompleteBox.FilterModeProperty, invalid);
        }

        /// <summary>
        /// Attach to the standard, non-cancelable drop down events.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Justification = "We must check for string.Empty and not null.")]
        [TestMethod]
        [Asynchronous]
        [Priority(0)]
        [Timeout(DefaultTimeout)]
        [Description("Attach to the standard, non-cancelable drop down events.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void DropDownEvents()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            bool openEvent = false;
            bool closeEvent = false;
            MethodCallMonitor dropDownOpeningMonitor = control.DropDownOpeningActions.CreateMonitor();
            MethodCallMonitor dropDownOpenedMonitor = control.DropDownOpenedActions.CreateMonitor();
            MethodCallMonitor dropDownClosingMonitor = control.DropDownClosingActions.CreateMonitor();
            MethodCallMonitor dropDownClosedMonitor = control.DropDownClosedActions.CreateMonitor();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            control.DropDownOpened += (s, e) => openEvent = true;
            control.DropDownClosed += (s, e) => closeEvent = true;

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.SearchText == "a");
            EnqueueCallback(() => dropDownOpeningMonitor.AssertCalled("The OnDropDownOpening method was not called."));
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => Assert.IsTrue(openEvent, "The DropDownOpened event did not fire."));
            EnqueueCallback(() => dropDownOpenedMonitor.AssertCalled("The OnDropDownOpened method was not called."));

            EnqueueCallback(() => control.TextBox.Text = string.Empty);
            EnqueueConditional(() => control.Text == string.Empty);

            EnqueueCallback(() => dropDownClosingMonitor.AssertCalled("The OnDropDownClosing method was not called."));
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen));
            EnqueueConditional(() => closeEvent);

            EnqueueCallback(() => Assert.IsTrue(closeEvent, "The DropDownClosed event did not fire."));
            EnqueueCallback(() => dropDownClosedMonitor.AssertCalled("The OnDropDownClosed method was not called."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Right before calling PopulateComplete, clear the view.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Priority(0)]
        [Description("Right before calling PopulateComplete, clear the view.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void PopulateAndClearView()
        {
            OverriddenSelectionAdapter.Current = null;
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            bool populating = false;
            control.FilterMode = AutoCompleteFilterMode.None;
            control.Populating += (s, e) =>
            {
                e.Cancel = true;
                populating = true;
            };
            MethodCallMonitor populatingMonitor = control.PopulatingActions.CreateMonitor();
            MethodCallMonitor populatedMonitor = control.PopulatedActions.CreateMonitor();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "accounti");

            EnqueueConditional(() => control.Text == control.TextBox.Text);

            EnqueueCallback(() => Assert.IsTrue(populating, "The populating event did not fire."));
            EnqueueCallback(() => populatingMonitor.AssertCalled("The OnPopulating method was not called."));

            EnqueueCallback(() =>
                {
                    // Clear the view
                    OverriddenSelectionAdapter.Current.ItemsSource = null;

                    // Call populate
                    control.PopulateComplete();
                });

            EnqueueCallback(() => Assert.IsNotNull(OverriddenSelectionAdapter.Current.ItemsSource, "The ItemsSource is still null."));
            EnqueueCallback(() => populatedMonitor.AssertCalled("The OnPopulated method was not called."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Check that IsTextCompletionEnabled through the Text property works.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Priority(0)]
        [Description("Check that IsTextCompletionEnabled through the Text property works.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void TextCompletionViaTextProperty()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            control.IsTextCompletionEnabled = true;

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => Assert.AreEqual<string>(string.Empty, control.Text));
            EnqueueCallback(() => control.Text = "close");
            EnqueueCallback(() => Assert.IsNotNull(control.SelectedItem, "The SelectedItem was null. IsTextCompletionEnabled result did not match the item."));
            
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test the IsTextCompletionEnabled selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Priority(0)]
        [Description("Test the IsTextCompletionEnabled selection.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void TextCompletionSelection()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            control.IsTextCompletionEnabled = true;

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Focus());
            EnqueueCallback(() => 
                {
                    // Set text and move the caret to the end
                    control.TextBox.Text = "ac";
                    control.TextBox.SelectionStart = 2;
                });
            EnqueueConditional(() => control.SearchText == "ac");
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => Assert.IsTrue(control.TextBox.SelectionLength > 2, "The selection length did not increase."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Attach to the TextChanged event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Priority(0)]
        [Description("Attach to the TextChanged event.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void TextChangedEvent()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            bool textChanged = false;
            MethodCallMonitor textChangedMonitor = control.TextChangedActions.CreateMonitor();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            control.TextChanged += (s, e) => textChanged = true;

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.SearchText == "a");
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => Assert.IsTrue(textChanged, "The TextChanged event never fired."));
            EnqueueCallback(() => textChangedMonitor.AssertCalled("The OnTextChanged method was never called."));

            EnqueueCallback(() => textChanged = false);
            EnqueueCallback(() => control.Text = "conversati");
            EnqueueConditional(() => textChanged);
            EnqueueCallback(() => Assert.IsTrue(textChanged, "The TextChanged event never fired."));

            EnqueueCallback(() => textChanged = false);
            EnqueueCallback(() => control.Text = null);
            EnqueueConditional(() => textChanged);
            EnqueueCallback(() => Assert.IsTrue(textChanged, "The TextChanged event never fired."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Verify that the minimum prefix length property is working.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Justification = "Important test scenario.")]
        [TestMethod]
        [Asynchronous]
        [Description("Verify that the minimum prefix length property is working.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void VerifyMinimumPrefixLength()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            control.IsTextCompletionEnabled = false;

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.SearchText == "a");
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = string.Empty);
            EnqueueConditional(() => control.Text == string.Empty);

            EnqueueCallback(() => control.MinimumPrefixLength = 3);

            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.Text == "a");
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "acc");
            EnqueueConditional(() => control.Text == "acc");
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Verify that the population delay can be set back to 0.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Justification = "Important to test for the empty string.")]
        [TestMethod]
        [Description("MinimumPopulateDelayChangeToNull")]
        [Bug("Silverlight 29497", Fixed = true)]
        [Asynchronous]
        public void MinimumPopulateDelayChangeToNull()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            control.IsTextCompletionEnabled = false;

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.SearchText == "a");
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = string.Empty);
            EnqueueConditional(() => control.Text == string.Empty);

            EnqueueCallback(() => control.MinimumPopulateDelay = 1);

            EnqueueCallback(() =>
            {
                control.TextBox.Text = "acc";
            });
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => control.MinimumPopulateDelay = 0);
            EnqueueCallback(() => control.TextBox.Text = "accou");

            EnqueueTestComplete();
        }

        /// <summary>
        /// Verify that the population delay cannot be set to a negative delay.
        /// </summary>
        [TestMethod]
        [Description("Verify that the population delay cannot be set to a negative delay.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [ExpectedException(typeof(ArgumentException))]
        public void MinimumPopulateDelayChangeToNegative()
        {
            AutoCompleteBox ac = (AutoCompleteBox)DefaultControlToTest;
            ac.MinimumPopulateDelay = -100;
        }

        /// <summary>
        /// Verify that the population delay reverts after a negative set.
        /// </summary>
        [TestMethod]
        [Description("Verify that the population delay reverts after a negative set.")]
        public void MinimumPopulateDelayChangeToNegativeReverts()
        {
            AutoCompleteBox ac = (AutoCompleteBox)DefaultControlToTest;
            int currentDelay = ac.MinimumPopulateDelay;
            try
            {
                ac.MinimumPopulateDelay = -100;
            }
            catch (ArgumentException)
            {
            }
            Assert.AreEqual(currentDelay, ac.MinimumPopulateDelay);
        }

        /// <summary>
        /// Verify that the minimum populate delay is being used.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Justification = "Important test scenario.")]
        [TestMethod]
        [Asynchronous]
        [Description("Verify that the minimum populate delay is being used.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [Bug("535601", Fixed = true)]
        public void VerifyMinimumPopulateDelay()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            control.IsTextCompletionEnabled = false;
            DateTime started = DateTime.MinValue;

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.SearchText == "a");
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = string.Empty);
            EnqueueConditional(() => control.Text == string.Empty);

            EnqueueCallback(() => control.MinimumPopulateDelay = 500);
            EnqueueCallback(() => Assert.AreEqual(500, control.MinimumPopulateDelay, 0.1, "The MinimumPopulateDelay was not changed to 500ms."));

            EnqueueCallback(() => 
                {
                    control.TextBox.Text = "acc";
                    started = DateTime.Now;
                });
            EnqueueConditional(() => ((TimeSpan)(DateTime.Now - started)).TotalMilliseconds < 500);
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen));

            EnqueueConditional(() => ((TimeSpan)(DateTime.Now - started)).TotalMilliseconds > 500);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Set the ItemTemplate.
        /// </summary>
        [TestMethod]
        [Description("Set the ItemTemplate.")]
        public void SetItemTemplate()
        {
            AutoCompleteBox ac = new AutoCompleteBox();
            ac.ItemTemplate = new DataTemplate();
            Assert.IsNotNull(ac.ItemTemplate, "The ItemTemplate was not set.");
        }

        /// <summary>
        /// Set the ItemContainerStyle.
        /// </summary>
        [TestMethod]
        [Description("Set the ItemContainerStyle.")]
        public void SetItemContainerStyle()
        {
            AutoCompleteBox ac = new AutoCompleteBox();
            ac.ItemContainerStyle = new Style();
            Assert.IsNotNull(ac.ItemContainerStyle, "The ItemContainerStyle was not set.");
        }

        /// <summary>
        /// Initialize a new control with the Text property already set.
        /// </summary>
        [TestMethod]
        [Description("Initialize a new control with the Text property already set.")]
        public void StartingWithText()
        {
            AutoCompleteBox control = new AutoCompleteBox
            {
                Text = "Starting text."
            };
            control.ItemsSource = CreateSimpleStringArray();
        }

        /// <summary>
        /// Set the TextBoxStyle.
        /// </summary>
        [TestMethod]
        [Description("Set the TextBoxStyle.")]
        public void SetTextBoxStyle()
        {
            AutoCompleteBox ac = new AutoCompleteBox();
            ac.TextBoxStyle = new Style();
            Assert.IsNotNull(ac.TextBoxStyle, "The TextBoxStyle was not set.");
        }

        /// <summary>
        /// Cancels the drop down opening.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Cancels the drop down opening.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void CancelDropDownOpening()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            MethodCallMonitor dropDownOpeningMonitor = control.DropDownOpeningActions.CreateMonitor();
            MethodCallMonitor dropDownOpenedMonitor = control.DropDownOpenedActions.CreateMonitor();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };

            control.DropDownOpening += (s, e) => e.Cancel = true;

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.SearchText == "a");
            EnqueueCallback(() => dropDownOpeningMonitor.AssertCalled("The OnDropDownOpening method was not called."));
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen));
            EnqueueCallback(() => dropDownOpenedMonitor.AssertNotCalled("The OnDropDownOpened method was called."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Cancels the drop down closing.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Cancels the drop down closing.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void CancelDropDownClosing()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            MethodCallMonitor dropDownClosingMonitor = control.DropDownClosingActions.CreateMonitor();
            MethodCallMonitor dropDownClosedMonitor = control.DropDownClosedActions.CreateMonitor();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };

            control.DropDownClosing += (s, e) => e.Cancel = true;

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.SearchText == "a");
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => control.IsDropDownOpen = false);
            EnqueueCallback(() => dropDownClosingMonitor.AssertCalled("The OnDropDownClosing method was not called."));
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => dropDownClosedMonitor.AssertNotCalled("The OnDropDownClosed method was called."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Cancels the Population event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Priority(0)]
        [Description("Cancels the Population event.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void CancelPopulation()
        {
            OverriddenSelectionAdapter.Current = null;
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            bool populating = false;
            control.FilterMode = AutoCompleteFilterMode.None;
            control.Populating += (s, e) =>
                {
                    e.Cancel = true;
                    populating = true;
                };
            MethodCallMonitor populatingMonitor = control.PopulatingActions.CreateMonitor();
            MethodCallMonitor populatedMonitor = control.PopulatedActions.CreateMonitor();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "accounti");

            EnqueueConditional(() => control.Text == control.TextBox.Text);

            EnqueueCallback(() => Assert.IsTrue(populating, "The populating event did not fire."));
            EnqueueCallback(() => populatingMonitor.AssertCalled("The OnPopulating method was not called."));
            EnqueueCallback(() => populatedMonitor.AssertNotCalled("The OnPopulated method was called."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests using the Population event to change the ItemsSource.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests using the Population event to change the ItemsSource.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void Population()
        {
            string custom = "Custom!";
            string search = "accounti";
            OverriddenSelectionAdapter.Current = null;
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            OverriddenSelectionAdapter tsa = null;
            bool populated = false;
            bool populatedOk = false;
            control.FilterMode = AutoCompleteFilterMode.None;
            control.Populating += (s, e) =>
                {
                    control.ItemsSource = new string[] { custom };
                    Assert.AreEqual(search, e.Parameter, "The parameter value was incorrect.");
                };
            control.Populated += (s, e) =>
            {
                populated = true;
                ReadOnlyCollection<object> collection = e.Data as ReadOnlyCollection<object>;
                populatedOk = collection != null && collection.Count == 1;
            };
            MethodCallMonitor populatingMonitor = control.PopulatingActions.CreateMonitor();
            MethodCallMonitor populatedMonitor = control.PopulatedActions.CreateMonitor();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = search);
            EnqueueConditional(() => control.IsDropDownOpen);

            EnqueueCallback(() => Assert.IsTrue(populated, "The populated event did not fire."));
            EnqueueCallback(() => populatingMonitor.AssertCalled("The OnPopulating method was not called."));
            EnqueueCallback(() => Assert.IsTrue(populatedOk, "The populated event data was incorrect."));
            EnqueueCallback(() => populatedMonitor.AssertCalled("The OnPopulated method was not called."));

            EnqueueCallback(() => tsa = OverriddenSelectionAdapter.Current);
            EnqueueCallback(() => Assert.IsNotNull(tsa, "The testable selection adapter was not found."));
            EnqueueCallback(() => tsa.SelectFirst());
            EnqueueConditional(() => control.SelectedItem != null);
            EnqueueCallback(() => Assert.AreEqual(custom, control.SelectedItem));
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => 
                {
                    tsa.TestCommit();
                });
            EnqueueConditional(() => !control.IsDropDownOpen);
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test using a custom adapter.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test using a custom adapter.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void AdapterInXaml()
        {
            string xmlns = " xmlns=\"http://schemas.microsoft.com/client/2007\" " +
                " xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" " +
                " xmlns:testing=\"clr-namespace:System.Windows.Controls.Testing;assembly=System.Windows.Controls.Testing\" " +
                " xmlns:input=\"clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input\" ";
            string xmlChild = @"<Style " + xmlns + @" TargetType=""input:AutoCompleteBox"">
        <Setter Property=""Text"" Value=""Custom..."" />
        <Setter Property=""Template"">
            <Setter.Value>
                <ControlTemplate TargetType=""input:AutoCompleteBox"">
                    <Grid Margin=""{TemplateBinding Padding}"" Background=""{TemplateBinding Background}"">
                        <TextBox IsTabStop=""True"" x:Name=""Text"" Style=""{TemplateBinding TextBoxStyle}"" Margin=""0"" />
                        <Popup x:Name=""Popup"">
                            <Border x:Name=""PopupBorder"">
                                    <testing:XamlSelectionAdapter x:Name=""SelectionAdapter"" />
                            </Border>
                        </Popup>
                    </Grid>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>";
            Style customTemplate = XamlReader.Load(xmlChild) as Style;
            AutoCompleteBox control = new AutoCompleteBox
            {
                Style = customTemplate,
                ItemsSource = CreateSimpleStringArray()
            };

            bool isLoaded = false;
            OverriddenSelectionAdapter.Current = null;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueTestComplete();
        }

        /// <summary>
        /// Verify the simple observable collection add operation works.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Navigate up and down inside the drop down and validate the selected items.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void ObservableCollectionAdd()
        {
            string search = "Data";
            OverriddenSelectionAdapter.Current = null;
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            ObservableCollection<string> data = new ObservableCollection<string>
            {
                "Data1",
                "Data2",
            };
            control.ItemsSource = data;
            ObservableCollection<object> view = null;
            OverriddenSelectionAdapter tsa = null;

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(
                () => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."),
                () => control.TextBox.Text = search);
            EnqueueConditional(() => control.IsDropDownOpen);

            // Assert the 2 items, then add an item, and assert the new value
            EnqueueCallback(
                () => tsa = OverriddenSelectionAdapter.Current,
                () => Assert.IsNotNull(tsa, "The testable selection adapter was not found."),
                () => view = tsa.ItemsSource as ObservableCollection<object>,
                () => Assert.AreEqual(2, view.Count),
                () => data.Add("Data3"),
                () => Assert.AreEqual(3, view.Count));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Navigate up and down inside the drop down and validate the selected 
        /// items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Navigate up and down inside the drop down and validate the selected items.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void UpDownNavigation()
        {
            string search = "acc";
            bool selectionChanged = false;
            OverriddenSelectionAdapter.Current = null;
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            ObservableCollection<object> view = null;
            OverriddenSelectionAdapter tsa = null;

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            control.SelectionChanged += (s, e) => selectionChanged = true;
            EnqueueCallback(() => TestPanel.Children.Add(control));
            MethodCallMonitor selectionChangedMonitor = control.SelectionChangedActions.CreateMonitor();
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = search);
            EnqueueConditional(() => control.IsDropDownOpen);

            EnqueueCallback(() => tsa = OverriddenSelectionAdapter.Current);
            EnqueueCallback(() => Assert.IsNotNull(tsa, "The testable selection adapter was not found."));
            EnqueueCallback(() => tsa.SelectFirst());
            EnqueueConditional(() => control.SelectedItem != null);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => tsa.SelectNext());
            EnqueueCallback(() => 
                {
                    view = tsa.ItemsSource as ObservableCollection<object>;
                    Assert.IsNotNull(view, "The ObservableCollection could not be cast.");
                });
            EnqueueCallback(() => Assert.AreEqual(view[1], control.SelectedItem, "The SelectedItem was not the expected value."));
            EnqueueCallback(() => tsa.SelectNext());
            EnqueueCallback(() => Assert.AreEqual(view[2], control.SelectedItem, "The SelectedItem was not the expected value."));
            EnqueueCallback(() => tsa.SelectPrevious());
            EnqueueCallback(() => Assert.AreEqual(view[1], control.SelectedItem, "The SelectedItem was not the expected value."));
            EnqueueCallback(() =>
            {
                tsa.TestCommit();
            });
            EnqueueConditional(() => !control.IsDropDownOpen);
            EnqueueCallback(() => Assert.AreEqual(view[1], control.SelectedItem));
            EnqueueCallback(() => Assert.IsTrue(selectionChanged, "The SelectedItemChanged event handler never fired."));
            EnqueueCallback(() => selectionChangedMonitor.AssertCalled("The OnSelectionChanged method was not called."));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Cancels the selection adapter and verifies that the text value is 
        /// reverted.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Cancels the selection adapter and verifies that the text value is reverted.")]
        [Bug("Silverlight 29497", Fixed = true)]
        public void CancelSelection()
        {
            string custom = "Custom!";
            string search = "accounti";
            OverriddenSelectionAdapter.Current = null;
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();
            OverriddenSelectionAdapter tsa = null;
            bool populated = false;
            bool populatedOk = false;
            control.FilterMode = AutoCompleteFilterMode.None;
            control.Populating += (s, e) => control.ItemsSource = new string[] { custom };
            control.Populated += (s, e) =>
            {
                populated = true;
                ReadOnlyCollection<object> roc = e.Data as ReadOnlyCollection<object>;
                populatedOk = roc != null && roc.Count == 1;
            };
            MethodCallMonitor populatingMonitor = control.PopulatingActions.CreateMonitor();
            MethodCallMonitor populatedMonitor = control.PopulatedActions.CreateMonitor();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = search);
            EnqueueConditional(() => control.IsDropDownOpen);

            EnqueueCallback(() => Assert.IsTrue(populated, "The populated event did not fire."));
            EnqueueCallback(() => populatingMonitor.AssertCalled("The OnPopulating method was not called."));
            EnqueueCallback(() => Assert.IsTrue(populatedOk, "The populated event data was incorrect."));
            EnqueueCallback(() => populatedMonitor.AssertCalled("The OnPopulated method was not called."));

            EnqueueCallback(() => tsa = OverriddenSelectionAdapter.Current);
            EnqueueCallback(() => Assert.IsNotNull(tsa, "The testable selection adapter was not found."));
            EnqueueCallback(() => tsa.SelectFirst());
            EnqueueConditional(() => control.SelectedItem != null);
            EnqueueCallback(() => Assert.AreEqual(custom, control.SelectedItem));
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => tsa.TestCancel());
            EnqueueConditional(() => !control.IsDropDownOpen);
            EnqueueCallback(() => Assert.AreEqual(search, control.TextBox.Text, "The original value was not restored in the text box"));
            EnqueueCallback(() => Assert.AreEqual(search, control.Text, "The original value was not restored in the text property"));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Gain coverage by moving the focus from the control to another 
        /// control in the test panel.
        /// </summary>
        [TestMethod]
        [Description("Gain coverage by moving the focus from the control to another control in the test panel.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [Tag(Tags.RequiresFocus)]
        [Asynchronous]
        public void LostFocus()
        {
            // TODO: Evaluate improvements to remove the Sleep.

            Button button = new Button { Content = "This is a Button" };
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            TestPanel.Children.Add(button);

            bool lostFocus = false;
            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            control.LostFocus += (s, e) => lostFocus = true;
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.Focus());
            EnqueueDelay(TimeSpan.FromSeconds(0.1));
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen));
            EnqueueDelay(TimeSpan.FromSeconds(0.1));
            EnqueueCallback(() => button.Focus());
            EnqueueDelay(TimeSpan.FromSeconds(0.1));
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen));
            EnqueueCallback(() => Assert.IsTrue(lostFocus, "Focus was not lost. The Silverlight host may not have had focus itself."));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Verify that focus being lost with the drop down open will close it.
        /// </summary>
        [TestMethod]
        [Description("Verify that focus being lost with the drop down open will close it")]
        [Bug("Silverlight 29497", Fixed = true)]
        [Tag(Tags.RequiresFocus)]
        [Asynchronous]
        public void LostFocusWithDropDown()
        {
            // TODO: Evaluate improvements to remove the Sleep.

            Button button = new Button { Content = "This is a Button" };
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            TestPanel.Children.Add(button);

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.Focus());
            EnqueueCallback(() => control.TextBox.Text = "accounti");
            EnqueueConditional(() => control.SearchText == "accounti");
            EnqueueCallback(() => StringAssert.Equals(control.TextBox.Text, "accounting"));
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));
            EnqueueCallback(() => button.Focus());
            
            // This will prevent a timeout
            EnqueueDelay(TimeSpan.FromSeconds(0.1));
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen, "The drop down did not close. Please check that the focus did change."));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test changing the minimum prefix length.
        /// </summary>
        [TestMethod]
        [Description("Test changing the minimum prefix length.")]
        public void ChangeMinimumPrefixLength()
        {
            AutoCompleteBox ac = new AutoCompleteBox();
            ac.MinimumPrefixLength = 10;
            Assert.AreEqual(10, ac.MinimumPrefixLength);
            ac.MinimumPrefixLength = 1;
            Assert.AreEqual(1, ac.MinimumPrefixLength);
        }

        /// <summary>
        /// Test changing the minimum prefix length to a large negative value.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [Description("Test changing the minimum prefix length to a large negative value.")]
        public void ChangeMinimumPrefixLengthCoerce()
        {
            AutoCompleteBox ac = new AutoCompleteBox();
            ac.MinimumPrefixLength = 10;
            Assert.AreEqual(10, ac.MinimumPrefixLength);
            
            // This will throw the exception
            ac.MinimumPrefixLength = -99;
        }

        /// <summary>
        /// Change the maximum drop down height dependency property.
        /// </summary>
        [TestMethod]
        [Description("Change the maximum drop down height dependency property.")]
        public void ChangeMaxDropDownHeight()
        {
            AutoCompleteBox ac = new AutoCompleteBox();
            ac.MaxDropDownHeight = 60;
            Assert.AreEqual(60, ac.MaxDropDownHeight);
        }

        /// <summary>
        /// Change the maximum drop down height dependency property with an 
        /// invalid value.
        /// </summary>
        [TestMethod]
        [Description("Change the maximum drop down height dependency property with an invalid value.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [ExpectedException(typeof(ArgumentException))]
        public void ChangeMaxDropDownHeightInvalid()
        {
            AutoCompleteBox ac = new AutoCompleteBox();
            ac.MaxDropDownHeight = -10;
        }

        /// <summary>
        /// Validate that the IsTextCompletionEnabled property is updated in 
        /// the standard AutoCompleteBox scenario.
        /// </summary>
        [TestMethod]
        [Description("Validate that the IsTextCompletionEnabled property is updated in the standard AutoCompleteBox scenario.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [Asynchronous]
        public void TextCompletionValidation()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "accounti");
            EnqueueConditional(() => control.SearchText == "accounti");
            EnqueueCallback(() => StringAssert.Equals(control.TextBox.Text, "accounting"));
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueTestComplete();
        }

        /// <summary>
        /// A typical string search scenario.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Functional test.")]
        [TestMethod]
        [Asynchronous]
        [Description("A typical string search scenario.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [Timeout(10000)]
        public void StringSearch()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };

            // Add the element to the test surface and wait until it's loaded
            
            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueConditional(() => control.IsDropDownOpen);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "acc");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueConditional(() => control.IsDropDownOpen);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "zoo");
            EnqueueConditional(() => control.Text == control.TextBox.Text);

            EnqueueCallback(() => control.TextBox.Text = "accept");
            EnqueueConditional(() => control.Text == control.TextBox.Text);

            EnqueueCallback(() => control.TextBox.Text = "cook");
            EnqueueConditional(() => control.Text == control.TextBox.Text);

            // Remove the element from the test surface and finish the test
            EnqueueCallback(() => TestPanel.Children.Remove(control));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the custom ItemFilter workflow.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Functional test scenario.")]
        [TestMethod]
        [Asynchronous]
        [Description("Tests the custom ItemFilter workflow.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [Timeout(10000)]
        public void ItemSearch()
        {
            OverriddenAutoCompleteBox control = GetDerivedAutoComplete();

            bool isLoaded = false;
            control.Loaded += delegate { isLoaded = true; };

            control.FilterMode = AutoCompleteFilterMode.Custom;
            control.ItemFilter = (search, item) =>
                {
                    string s = item as string;
                    return s == null ? false : true;
                };

            // Just set to null briefly to exercise that code path
            AutoCompleteFilterPredicate<object> filter = control.ItemFilter;
            Assert.IsNotNull(filter, "The ItemFilter was null.");
            control.ItemFilter = null;
            Assert.IsNull(control.ItemFilter, "The ItemFilter should be null.");
            control.ItemFilter = filter;
            Assert.IsNotNull(control.ItemFilter, "The ItemFilter was null.");

            // Add the element to the test surface and wait until it's loaded

            EnqueueCallback(() => TestPanel.Children.Add(control));
            EnqueueConditional(() => isLoaded);

            EnqueueCallback(() => Assert.IsNotNull(control.TextBox, "The TextBox part could not be retrieved."));
            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueConditional(() => control.IsDropDownOpen);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "acc");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueConditional(() => control.IsDropDownOpen);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "a");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueConditional(() => control.IsDropDownOpen);
            EnqueueCallback(() => Assert.IsTrue(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            EnqueueConditional(() => !control.IsDropDownOpen);
            EnqueueCallback(() => Assert.IsFalse(control.IsDropDownOpen));

            EnqueueCallback(() => control.TextBox.Text = "zoo");
            EnqueueConditional(() => control.Text == control.TextBox.Text);
            
            EnqueueCallback(() => control.TextBox.Text = "accept");
            EnqueueConditional(() => control.Text == control.TextBox.Text);

            EnqueueCallback(() => control.TextBox.Text = "cook");
            EnqueueConditional(() => control.Text == control.TextBox.Text);

            // Remove the element from the test surface and finish the test
            EnqueueCallback(() => TestPanel.Children.Remove(control));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Verify that the ItemsSource changed handler works.
        /// </summary>
        [TestMethod]
        [Description("Verify that the ItemsSource changed handler works.")]
        public void ChangeItemsSource()
        {
            AutoCompleteBox ac = CreateSimpleStringAutoComplete();
            ac.ItemsSource = null;
            ac.ItemsSource = new List<object> { DateTime.Now, "hello", Guid.NewGuid() };
            ac.ItemsSource = CreateSimpleStringArray();
        }

        /// <summary>
        /// Validate the the SearchText property is read only.
        /// </summary>
        [TestMethod]
        [Description("Validate the the SearchText property is read only.")]
        [Bug("Silverlight 29497", Fixed = true)]
        [Asynchronous]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadOnlySearchText()
        {
            AutoCompleteBox ac = CreateSimpleStringAutoComplete();
            TestAsync(
                ac,
                () => ac.SetValue(AutoCompleteBox.SearchTextProperty, "a"));
        }

        /// <summary>
        /// Validate that the SelectedItem property is no longer read only.
        /// </summary>
        /// <remarks>This was a functional change after the December 2008
        /// Silverlight Toolkit release.</remarks>
        [TestMethod]
        [Description("Validate that the SelectedItem property is no longer read only.")]
        [Bug("568174 - AutoCompleteBox - Make SelectedItem property settable", Fixed = true)]
        public void SetSelectedItem()
        {
            AutoCompleteBox ac = CreateSimpleStringAutoComplete();
            ac.SetValue(AutoCompleteBox.SelectedItemProperty, "a");
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
            Assert.AreEqual(4, templateParts.Count);
            Assert.AreSame(typeof(Popup), templateParts["Popup"]);
            Assert.AreSame(typeof(TextBox), templateParts["Text"]);
            Assert.AreSame(typeof(Selector), templateParts["Selector"]);
            Assert.AreSame(typeof(ISelectionAdapter), templateParts["SelectionAdapter"]);

            // By default:
            // - Selection adapter is wrapped at runtime
            // Assert.AreSame(typeof(ISelectionAdapter), templateParts["SelectionAdapter"]);
        }

        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's template visual states.")]
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> visualStates = DefaultControlToTest.GetType().GetVisualStates();
            Assert.AreEqual(11, visualStates.Count);

            Assert.AreEqual<string>("CommonStates", visualStates["Normal"]);
            Assert.AreEqual<string>("CommonStates", visualStates["MouseOver"]);
            Assert.AreEqual<string>("CommonStates", visualStates["Pressed"]);
            Assert.AreEqual<string>("CommonStates", visualStates["Disabled"]);

            Assert.AreEqual<string>("FocusStates", visualStates["Focused"]);
            Assert.AreEqual<string>("FocusStates", visualStates["Unfocused"]);

            Assert.AreEqual<string>("ValidationStates", visualStates["Valid"]);
            Assert.AreEqual<string>("ValidationStates", visualStates["InvalidFocused"]);
            Assert.AreEqual<string>("ValidationStates", visualStates["InvalidUnfocused"]);

            // + Popups
        }

        /// <summary>
        /// Verify the control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultControlToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(2, properties.Count, "Incorrect number of style typed property attributes!");
            Assert.AreEqual(typeof(TextBox), properties["TextBoxStyle"], "Failed to find expected style type property TextStyle!");
            Assert.AreEqual(typeof(ListBox), properties["ItemContainerStyle"], "Failed to find expected style type property ContainerStyle!");
        }
        #endregion

        #region Sample data
        
        /// <summary>
        /// Creates a large list of strings for AutoCompleteBox testing.
        /// </summary>
        /// <returns>Returns a new List of string values.</returns>
        protected static IList<string> CreateSimpleStringArray()
        {
            return new List<string>
            {
            "a", 
            "abide", 
            "able", 
            "about", 
            "above", 
            "absence", 
            "absurd", 
            "accept", 
            "acceptance", 
            "accepted", 
            "accepting", 
            "access", 
            "accessed", 
            "accessible", 
            "accident", 
            "accidentally", 
            "accordance", 
            "account", 
            "accounting", 
            "accounts", 
            "accusation", 
            "accustomed", 
            "ache", 
            "across", 
            "act", 
            "active", 
            "actual", 
            "actually", 
            "ada", 
            "added", 
            "adding", 
            "addition", 
            "additional", 
            "additions", 
            "address", 
            "addressed", 
            "addresses", 
            "addressing", 
            "adjourn", 
            "adoption", 
            "advance", 
            "advantage", 
            "adventures", 
            "advice", 
            "advisable", 
            "advise", 
            "affair", 
            "affectionately", 
            "afford", 
            "afore", 
            "afraid", 
            "after", 
            "afterwards", 
            "again", 
            "against", 
            "age", 
            "aged", 
            "agent", 
            "ago", 
            "agony", 
            "agree", 
            "agreed", 
            "agreement", 
            "ah", 
            "ahem", 
            "air", 
            "airs", 
            "ak", 
            "alarm", 
            "alarmed", 
            "alas", 
            "alice", 
            "alive", 
            "all", 
            "allow", 
            "almost", 
            "alone", 
            "along", 
            "aloud", 
            "already", 
            "also", 
            "alteration", 
            "altered", 
            "alternate", 
            "alternately", 
            "altogether", 
            "always", 
            "am", 
            "ambition", 
            "among", 
            "an", 
            "ancient", 
            "and", 
            "anger", 
            "angrily", 
            "angry", 
            "animal", 
            "animals", 
            "ann", 
            "annoy", 
            "annoyed", 
            "another", 
            "answer", 
            "answered", 
            "answers", 
            "antipathies", 
            "anxious", 
            "anxiously", 
            "any", 
            "anyone", 
            "anything", 
            "anywhere", 
            "appealed", 
            "appear", 
            "appearance", 
            "appeared", 
            "appearing", 
            "appears", 
            "applause", 
            "apple", 
            "apples", 
            "applicable", 
            "apply", 
            "approach", 
            "arch", 
            "archbishop", 
            "arches", 
            "archive", 
            "are", 
            "argue", 
            "argued", 
            "argument", 
            "arguments", 
            "arise", 
            "arithmetic", 
            "arm", 
            "arms", 
            "around", 
            "arranged", 
            "array", 
            "arrived", 
            "arrow", 
            "arrum", 
            "as", 
            "ascii", 
            "ashamed", 
            "ask", 
            "askance", 
            "asked", 
            "asking", 
            "asleep", 
            "assembled", 
            "assistance", 
            "associated", 
            "at", 
            "ate", 
            "atheling", 
            "atom", 
            "attached", 
            "attempt", 
            "attempted", 
            "attempts", 
            "attended", 
            "attending", 
            "attends", 
            "audibly", 
            "australia", 
            "author", 
            "authority", 
            "available", 
            "avoid", 
            "away", 
            "awfully", 
            "axes", 
            "axis", 
            "b", 
            "baby", 
            "back", 
            "backs", 
            "bad", 
            "bag", 
            "baked", 
            "balanced", 
            "bank", 
            "banks", 
            "banquet", 
            "bark", 
            "barking", 
            "barley", 
            "barrowful", 
            "based", 
            "bat", 
            "bathing", 
            "bats", 
            "bawled", 
            "be", 
            "beak", 
            "bear", 
            "beast", 
            "beasts", 
            "beat", 
            "beating", 
            "beau", 
            "beauti", 
            "beautiful", 
            "beautifully", 
            "beautify", 
            "became", 
            "because", 
            "become", 
            "becoming", 
            "bed", 
            "beds", 
            "bee", 
            "been", 
            "before", 
            "beg", 
            "began", 
            "begged", 
            "begin", 
            "beginning", 
            "begins", 
            "begun", 
            "behead", 
            "beheaded", 
            "beheading", 
            "behind", 
            "being", 
            "believe", 
            "believed", 
            "bells", 
            "belong", 
            "belongs", 
            "beloved", 
            "below", 
            "belt", 
            "bend", 
            "bent", 
            "besides", 
            "best", 
            "better", 
            "between", 
            "bill", 
            "binary", 
            "bird", 
            "birds", 
            "birthday", 
            "bit", 
            "bite", 
            "bitter", 
            "blacking", 
            "blades", 
            "blame", 
            "blasts", 
            "bleeds", 
            "blew", 
            "blow", 
            "blown", 
            "blows", 
            "body", 
            "boldly", 
            "bone", 
            "bones", 
            "book", 
            "books", 
            "boon", 
            "boots", 
            "bore", 
            "both", 
            "bother", 
            "bottle", 
            "bottom", 
            "bough", 
            "bound", 
            "bowed", 
            "bowing", 
            "box", 
            "boxed", 
            "boy", 
            "brain", 
            "branch", 
            "branches", 
            "brandy", 
            "brass", 
            "brave", 
            "breach", 
            "bread", 
            "break", 
            "breath", 
            "breathe", 
            "breeze", 
            "bright", 
            "brightened", 
            "bring", 
            "bringing", 
            "bristling", 
            "broke", 
            "broken", 
            "brother", 
            "brought", 
            "brown", 
            "brush", 
            "brushing", 
            "burn", 
            "burning", 
            "burnt", 
            "burst", 
            "bursting", 
            "busily", 
            "business", 
            "business@pglaf", 
            "busy", 
            "but", 
            "butter", 
            "buttercup", 
            "buttered", 
            "butterfly", 
            "buttons", 
            "by", 
            "bye", 
            "c", 
            "cackled", 
            "cake", 
            "cakes", 
            "calculate", 
            "calculated", 
            "call", 
            "called", 
            "calling", 
            "calmly", 
            "came", 
            "camomile", 
            "can", 
            "canary", 
            "candle", 
            "cannot", 
            "canterbury", 
            "canvas", 
            "capering", 
            "capital", 
            "card", 
            "cardboard", 
            "cards", 
            "care", 
            "carefully", 
            "cares", 
            "carried", 
            "carrier", 
            "carroll", 
            "carry", 
            "carrying", 
            "cart", 
            "cartwheels", 
            "case", 
            "cat", 
            "catch", 
            "catching", 
            "caterpillar", 
            "cats", 
            "cattle", 
            "caucus", 
            "caught", 
            "cauldron", 
            "cause", 
            "caused", 
            "cautiously", 
            "cease", 
            "ceiling", 
            "centre", 
            "certain", 
            "certainly", 
            "chain", 
            "chains", 
            "chair", 
            "chance", 
            "chanced", 
            "change", 
            "changed", 
            "changes", 
            "changing", 
            "chapter", 
            "character", 
            "charge", 
            "charges", 
            "charitable", 
            "charities", 
            "chatte", 
            "cheap", 
            "cheated", 
            "check", 
            "checked", 
            "checks", 
            "cheeks", 
            "cheered", 
            "cheerfully", 
            "cherry", 
            "cheshire", 
            "chief", 
            "child", 
            "childhood", 
            "children", 
            "chimney", 
            "chimneys", 
            "chin", 
            "choice", 
            "choke", 
            "choked", 
            "choking", 
            "choose", 
            "choosing", 
            "chop", 
            "chorus", 
            "chose", 
            "christmas", 
            "chrysalis", 
            "chuckled", 
            "circle", 
            "circumstances", 
            "city", 
            "civil", 
            "claim", 
            "clamour", 
            "clapping", 
            "clasped", 
            "classics", 
            "claws", 
            "clean", 
            "clear", 
            "cleared", 
            "clearer", 
            "clearly", 
            "clever", 
            "climb", 
            "clinging", 
            "clock", 
            "close", 
            "closed", 
            "closely", 
            "closer", 
            "clubs", 
            "coast", 
            "coaxing", 
            "codes", 
            "coils", 
            "cold", 
            "collar", 
            "collected", 
            "collection", 
            "come", 
            "comes", 
            "comfits", 
            "comfort", 
            "comfortable", 
            "comfortably", 
            "coming", 
            "commercial", 
            "committed", 
            "common", 
            "commotion", 
            "company", 
            "compilation", 
            "complained", 
            "complaining", 
            "completely", 
            "compliance", 
            "comply", 
            "complying", 
            "compressed", 
            "computer", 
            "computers", 
            "concept", 
            "concerning", 
            "concert", 
            "concluded", 
            "conclusion", 
            "condemn", 
            "conduct", 
            "confirmation", 
            "confirmed", 
            "confused", 
            "confusing", 
            "confusion", 
            "conger", 
            "conqueror", 
            "conquest", 
            "consented", 
            "consequential", 
            "consider", 
            "considerable", 
            "considered", 
            "considering", 
            "constant", 
            "consultation", 
            "contact", 
            "contain", 
            "containing", 
            "contempt", 
            "contemptuous", 
            "contemptuously", 
            "content", 
            "continued", 
            "contract", 
            "contradicted", 
            "contributions", 
            "conversation", 
            "conversations", 
            "convert", 
            "cook", 
            "cool", 
            "copied", 
            "copies", 
            "copy", 
            "copying", 
            "copyright", 
            "corner", 
            "corners", 
            "corporation", 
            "corrupt", 
            "cost", 
            "costs", 
            "could", 
            "couldn", 
            "counting", 
            "countries", 
            "country", 
            "couple", 
            "couples", 
            "courage", 
            "course", 
            "court", 
            "courtiers", 
            "coward", 
            "crab", 
            "crash", 
            "crashed", 
            "crawled", 
            "crawling", 
            "crazy", 
            "created", 
            "creating", 
            "creation", 
            "creature", 
            "creatures", 
            "credit", 
            "creep", 
            "crept", 
            "cried", 
            "cries", 
            "crimson", 
            "critical", 
            "crocodile", 
            "croquet", 
            "croqueted", 
            "croqueting", 
            "cross", 
            "crossed", 
            "crossly", 
            "crouched", 
            "crowd", 
            "crowded", 
            "crown", 
            "crumbs", 
            "crust", 
            "cry", 
            "crying", 
            "cucumber", 
            "cunning", 
            "cup", 
            "cupboards", 
            "cur", 
            "curiosity", 
            "curious", 
            "curiouser", 
            "curled", 
            "curls", 
            "curly", 
            "currants", 
            "current", 
            "curtain", 
            "curtsey", 
            "curtseying", 
            "curving", 
            "cushion", 
            "custard", 
            "custody", 
            "cut", 
            "cutting", 
            };
        }
#endregion

        /// <summary>
        /// Verifies that the event handler for an INotifyCollectionChanged 
        /// collection won't keep the items source alive.
        /// </summary>
        [TestMethod]
        [Description("Verifies that the event handler for an INotifyCollectionChanged collection won't keep the items source alive.")]
        [Bug("634021: AutoCompleteBox should implement the WeakEvent pattern to avoid leaking control instances due to its ItemsSource property.", Fixed = true)]
        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "Deliberately calling method to verify scenario.")]
        public void CollectionChangedHandlerDoesNotKeepSeriesAlive()
        {
            ObservableCollection<int> collection = new ObservableCollection<int>();
            WeakReference weakReference = new WeakReference(DefaultControlToTest);
            ((AutoCompleteBox)weakReference.Target).ItemsSource = collection;
            GC.Collect();
            Assert.IsNull(weakReference.Target);
        }

        /// <summary>
        /// Verify that the DropDownClosed event does not fire during startup.
        /// </summary>
        [TestMethod]
        [Description("Verify that the DropDownClosed event does not fire during startup")]
        [Bug("630459: AutoCompleteBox DropDownClosed event should not fire during startup", Fixed = true)]
        [Asynchronous]
        public void NoDropDownClosedAtStartup()
        {
            OverriddenAutoCompleteBox acb = new OverriddenAutoCompleteBox();
            bool closedEventFired = false;
            bool isLoaded = false;
            acb.DropDownClosed += (o, e) => closedEventFired = true;
            MethodCallMonitor dropDownClosedMonitor = acb.DropDownClosedActions.CreateMonitor();
            acb.Loaded += (o, e) => isLoaded = true;

            EnqueueCallback(() => TestPanel.Children.Add(acb));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => Assert.IsFalse(closedEventFired, "The DropDownClosed event fired during startup."));
            EnqueueCallback(() => dropDownClosedMonitor.AssertNotCalled("The OnDropDownClosed method was called."));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Verify that the DropDownClosing event does not fire during startup.
        /// </summary>
        [TestMethod]
        [Description("Verify that the DropDownClosing event does not fire during startup")]
        [Asynchronous]
        public void NoDropDownClosingAtStartup()
        {
            OverriddenAutoCompleteBox acb = new OverriddenAutoCompleteBox();
            bool eventFired = false;
            bool isLoaded = false;
            acb.DropDownClosing += (o, e) => eventFired = true;
            MethodCallMonitor dropDownClosingMonitor = acb.DropDownClosingActions.CreateMonitor();
            acb.Loaded += (o, e) => isLoaded = true;

            EnqueueCallback(() => TestPanel.Children.Add(acb));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => Assert.IsFalse(eventFired, "The DropDownClosing event fired during startup."));
            EnqueueCallback(() => dropDownClosingMonitor.AssertNotCalled("The OnDropDownClosing method was called."));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Verify that the DropDownOpened event does not fire during startup.
        /// </summary>
        [TestMethod]
        [Description("Verify that the DropDownOpened event does not fire during startup")]
        [Asynchronous]
        public void NoDropDownOpenedAtStartup()
        {
            OverriddenAutoCompleteBox acb = new OverriddenAutoCompleteBox();
            bool eventFired = false;
            bool isLoaded = false;
            acb.DropDownOpened += (o, e) => eventFired = true;
            MethodCallMonitor dropDownOpenedMonitor = acb.DropDownOpenedActions.CreateMonitor();
            acb.Loaded += (o, e) => isLoaded = true;

            EnqueueCallback(() => TestPanel.Children.Add(acb));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => Assert.IsFalse(eventFired, "The DropDownOpened event fired during startup."));
            EnqueueCallback(() => dropDownOpenedMonitor.AssertNotCalled("The OnDropDownOpened method was called."));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Verify that the DropDownOpening event does not fire during startup.
        /// </summary>
        [TestMethod]
        [Description("Verify that the DropDownOpening event does not fire during startup")]
        [Asynchronous]
        public void NoDropDownOpeningAtStartup()
        {
            OverriddenAutoCompleteBox acb = new OverriddenAutoCompleteBox();
            bool eventFired = false;
            bool isLoaded = false;
            acb.DropDownOpening += (o, e) => eventFired = true;
            MethodCallMonitor dropDownOpeningMonitor = acb.DropDownOpeningActions.CreateMonitor();
            acb.Loaded += (o, e) => isLoaded = true;

            EnqueueCallback(() => TestPanel.Children.Add(acb));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => Assert.IsFalse(eventFired, "The DropDownOpening event fired during startup."));
            EnqueueCallback(() => dropDownOpeningMonitor.AssertNotCalled("The OnDropDownOpening method was called."));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Verifies that resources are properly accessible.
        /// </summary>
        [TestMethod]
        [Description("Verifies that resources are properly accessible.")]
        public void ResourcesFromParentAndToChildren()
        {
            Panel panel = XamlReader.Load(
                @"<Grid " +
                    "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                    "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                    "xmlns:local='clr-namespace:System.Windows.Controls.Testing;assembly=System.Windows.Controls.Testing' " +
                    "xmlns:input='clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input'>" +
                    "<Grid.Resources>" +
                        "<SolidColorBrush x:Key='SCBO' Color='Orange'/>" +
                    "</Grid.Resources>" +
                    "<input:AutoCompleteBox x:Name='ACB' Background='{StaticResource SCBO}'>" +
                        "<input:AutoCompleteBox.Resources>" +
                            "<SolidColorBrush x:Key='SCBP' Color='Purple'/>" +
                        "</input:AutoCompleteBox.Resources>" +
                        "<input:AutoCompleteBox.ItemsSource>" +
                            "<local:TestObjectCollection>" +
                                "<ContentControl x:Name='CC' Background='{StaticResource SCBO}' Foreground='{StaticResource SCBP}'/>" +
                            "</local:TestObjectCollection>" +
                        "</input:AutoCompleteBox.ItemsSource>" +
                    "</input:AutoCompleteBox>" +
                "</Grid>") as Panel;
            SolidColorBrush scbo = panel.Resources["SCBO"] as SolidColorBrush;
            AutoCompleteBox acb = panel.FindName("ACB") as AutoCompleteBox;
            SolidColorBrush scbp = acb.Resources["SCBP"] as SolidColorBrush;
            ContentControl cc = panel.FindName("CC") as ContentControl;
            if (null == cc)
            {
                cc = acb.ItemsSource.OfType<ContentControl>().Single();
            }
            Assert.AreEqual(scbo, acb.Background);
            Assert.AreEqual(scbo, cc.Background);
            Assert.AreEqual(scbp, cc.Foreground);
        }

        /// <summary>
        /// Verifies that binding to SelectedItem works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that binding to SelectedItem works properly.")]
        public void SelectedItemBinding()
        {
            OverriddenAutoCompleteBox acb = new OverriddenAutoCompleteBox();
            acb.ItemsSource = "words go here".Split();
            ContentControl cc = new ContentControl();
            Binding b = new Binding("SelectedItem");
            b.Source = acb;
            cc.SetBinding(ContentControl.ContentProperty, b);

            int loadedCount = 0;
            acb.Loaded += (o, e) => loadedCount++;
            cc.Loaded += (o, e) => loadedCount++;

            EnqueueCallback(() => TestPanel.Children.Add(acb));
            EnqueueCallback(() => TestPanel.Children.Add(cc));
            EnqueueConditional(() => 2 == loadedCount);

            EnqueueCallback(() => acb.TextBox.Text = "w");
            EnqueueConditional(() => acb.IsDropDownOpen);

            EnqueueCallback(() => OverriddenSelectionAdapter.Current.SelectFirst());
            EnqueueCallback(() => Assert.AreEqual("words", acb.SelectedItem));

            EnqueueCallback(() => OverriddenSelectionAdapter.Current.TestCommit());
            EnqueueConditional(() => !acb.IsDropDownOpen);

            EnqueueCallback(() => Assert.AreEqual("words", cc.Content));

            EnqueueTestComplete();
        }
    }
}
