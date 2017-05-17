// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// DomainUpDown unit tests.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Test class")]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposing in method ShouldGrabFocusOnSpin.")]
    [TestClass]
    public partial class DomainUpDownTest : UpDownBaseTest<object>
    {
        #region UpDownBase<T>s to test
        /// <summary>
        /// Gets a default instance of UpDownBase&lt;T&gt; (or a derived type) to test.
        /// </summary>
        /// <value></value>
        public override UpDownBase<object> DefaultUpDownBaseTToTest
        {
            get { return DefaultDomainUpDownToTest; }
        }

        /// <summary>
        /// Gets instances of UpDownBase&lt;T&gt; (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<UpDownBase<object>> UpDownBaseTsToTest
        {
            get { return DomainUpDownsToTest.OfType<UpDownBase<object>>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenUpDownBase&lt;T&gt; (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<IOverriddenUpDownBase<object>> OverriddenUpDownBaseTsToTest
        {
            get { return OverriddenDomainUpDownsToTest.OfType<IOverriddenUpDownBase<object>>(); }
        }
        #endregion UpDownBase<T>s to test

        #region DomainUpDowns to test

        /// <summary>
        /// Gets the default DomainUpDown (or a derived type) to test.
        /// </summary>
        public virtual DomainUpDown DefaultDomainUpDownToTest
        {
            get { return new DomainUpDown(); }
        }

        /// <summary>
        /// Gets instances of DomainUpDown (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<DomainUpDown> DomainUpDownsToTest
        {
            get
            {
                yield return DefaultDomainUpDownToTest;

                DomainUpDown dud = new DomainUpDown()
                                       {
                                       };

                yield return dud;
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenDomainUpDown (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<IOverriddenControl> OverriddenDomainUpDownsToTest
        {
            get { yield break; }
        }

        #endregion

        #region DependencyProperties
        /// <summary>
        /// Gets the ItemsSource dependency property test.
        /// </summary>
        protected DependencyPropertyTest<DomainUpDown, IEnumerable> ItemsSourceProperty { get; private set; }

        /// <summary>
        /// Gets the CurrentItemIndex dependency property test.
        /// </summary>
        protected DependencyPropertyTest<DomainUpDown, int> CurrentIndexProperty { get; private set; }

        /// <summary>
        /// Gets the is cyclic property.
        /// </summary>
        /// <value>The is cyclic property.</value>
        protected DependencyPropertyTest<DomainUpDown, bool> IsCyclicProperty { get; private set; }

        /// <summary>
        /// Gets the item template property.
        /// </summary>
        /// <value>The item template property.</value>
        protected DependencyPropertyTest<DomainUpDown, DataTemplate> ItemTemplateProperty { get; private set; }

        /// <summary>
        /// Gets the accepts invalid input property.
        /// </summary>
        /// <value>The accepts invalid input property.</value>
        protected DependencyPropertyTest<DomainUpDown, InvalidInputAction> AcceptsInvalidInputProperty { get; private set; }

        /// <summary>
        /// Gets the fallback item property.
        /// </summary>
        /// <value>The fallback item property.</value>
        protected DependencyPropertyTest<DomainUpDown, object> FallbackItemProperty { get; private set; }

        #endregion DependencyProperties

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainUpDownTest"/> class.
        /// </summary>
        public DomainUpDownTest()
        {
            Func<DomainUpDown> initializer = () => DefaultDomainUpDownToTest;
            Func<DomainUpDown> initializerWithItemsSource = () =>
                                                                {
                                                                    DomainUpDown dud = DefaultDomainUpDownToTest;
                                                                    dud.ItemsSource = new object[] { 1, 2, 3, 4 };
                                                                    return dud;
                                                                };

            ItemsSourceProperty =
                new DependencyPropertyTest<DomainUpDown, IEnumerable>(this, "ItemsSource")
                    {
                        Property = DomainUpDown.ItemsSourceProperty,
                        Initializer = initializer,
                        DefaultValue = null,
                        OtherValues = new List<IEnumerable>()
                                          {
                                              new object[]
                                                  {
                                                      new Rectangle(),
                                                      "test string"
                                                  },
                                              null
                                          }
                    };

            CurrentIndexProperty =
                new DependencyPropertyTest<DomainUpDown, int>(this, "CurrentIndex")
                    {
                        Property = DomainUpDown.CurrentIndexProperty,
                        Initializer = initializerWithItemsSource,
                        DefaultValue = 0,   // since we have an items collection, default value is not valid and is coerced
                        OtherValues = new int[] { 1, 2 }
                    };

            IsCyclicProperty =
                new DependencyPropertyTest<DomainUpDown, bool>(this, "IsCyclic")
                    {
                        Property = DomainUpDown.IsCyclicProperty,
                        Initializer = initializer,
                        DefaultValue = false,
                        OtherValues = new bool[] { true }
                    };

            ItemTemplateProperty =
                new DependencyPropertyTest<DomainUpDown, DataTemplate>(this, "ItemTemplate")
                    {
                        Property = DomainUpDown.ItemTemplateProperty,
                        Initializer = initializer,
                        DefaultValue = null,
                        OtherValues = new[]
                                          {
                                              new DataTemplate(),
                                                new XamlBuilder<DataTemplate>().Load(),
                                                (new XamlBuilder<DataTemplate> { Name = "Template" }).Load(),
                                                (new XamlBuilder<DataTemplate> { Name = "Template", Children = new List<XamlBuilder> { new XamlBuilder<StackPanel>() } }).Load()
                                          }
                    };

            AcceptsInvalidInputProperty =
                new DependencyPropertyTest<DomainUpDown, InvalidInputAction>(this, "InvalidInputAction")
                    {
                        Property = DomainUpDown.InvalidInputActionProperty,
                        Initializer = initializer,
                        DefaultValue = InvalidInputAction.UseFallbackItem,
                        OtherValues = new[] { InvalidInputAction.TextBoxCannotLoseFocus,  }
                    };

            FallbackItemProperty =
                new DependencyPropertyTest<DomainUpDown, object>(this, "FallbackItem")
                    {
                        Property = DomainUpDown.FallbackItemProperty,
                        Initializer = initializerWithItemsSource,
                        DefaultValue = null,
                        OtherValues = new object[] { 1, 2, 3, 4 }
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

            BackgroundProperty.DefaultValue = new SolidColorBrush(Colors.Transparent);

            BorderThicknessProperty.DefaultValue = new Thickness(1);

            HorizontalContentAlignmentProperty.DefaultValue = HorizontalAlignment.Left;

            PaddingProperty.DefaultValue = new Thickness(0);

            Func<UpDownBase<object>> initializerWithItemsSourceUpDownBase = () =>
            {
                DomainUpDown dud = DefaultDomainUpDownToTest;
                dud.ItemsSource = new object[] { 1, 2, 3, 4 };
                return dud;
            };

            ValueProperty.Initializer = initializerWithItemsSourceUpDownBase;
            ValueProperty.DefaultValue = 1;
            ValueProperty.OtherValues = new object[] { 1, 2 };
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // Get the base Control dependency property tests
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());

            tests.Add(ItemsSourceProperty.CheckDefaultValueTest);
            tests.Add(ItemsSourceProperty.ChangeClrSetterTest);
            tests.Add(ItemsSourceProperty.ChangeSetValueTest);
            tests.Add(ItemsSourceProperty.ClearValueResetsDefaultTest);
            tests.Add(ItemsSourceProperty.SetXamlAttributeTest);
            tests.Add(ItemsSourceProperty.SetXamlElementTest);

            tests.Add(CurrentIndexProperty.ChangeClrSetterTest);
            tests.Add(CurrentIndexProperty.ChangeSetValueTest);
            tests.Add(CurrentIndexProperty.SetXamlAttributeTest.Bug("TODO: find out why this fails."));
            tests.Add(CurrentIndexProperty.SetXamlElementTest.Bug("TODO: find out why this fails."));

            tests.Add(IsCyclicProperty.ChangeClrSetterTest);
            tests.Add(IsCyclicProperty.ChangeSetValueTest);
            tests.Add(IsCyclicProperty.ClearValueResetsDefaultTest);

            tests.Add(ItemTemplateProperty.CheckDefaultValueTest);
            tests.Add(ItemTemplateProperty.ChangeClrSetterTest);
            tests.Add(ItemTemplateProperty.ChangeSetValueTest);
            tests.Add(ItemTemplateProperty.SetNullTest);
            tests.Add(ItemTemplateProperty.ClearValueResetsDefaultTest);
            tests.Add(ItemTemplateProperty.CanBeStyledTest);
            tests.Add(ItemTemplateProperty.TemplateBindTest);

            tests.Add(AcceptsInvalidInputProperty.CheckDefaultValueTest);
            tests.Add(AcceptsInvalidInputProperty.ChangeClrSetterTest);

            tests.Add(FallbackItemProperty.CheckDefaultValueTest);
            tests.Add(FallbackItemProperty.ChangeClrSetterTest);

            DependencyPropertyTestMethod buggedTestA = tests.FirstOrDefault(a => a.Name == ValueProperty.SetXamlAttributeTest.Name);
            if (buggedTestA != null)
            {
                buggedTestA.Bug("Find out why this fails for DomainUpDown and not for UpDownBase.");
            }
            DependencyPropertyTestMethod buggedTestB = tests.FirstOrDefault(a => a.Name == ValueProperty.SetXamlElementTest.Name);
            if (buggedTestB != null)
            {
                buggedTestB.Bug("Find out why this fails for DomainUpDown and not for UpDownBase.");
            }

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
            IDictionary<string, string> visualStates = DefaultDomainUpDownToTest.GetType().GetVisualStates();
            Assert.AreEqual(13, visualStates.Count);

            Assert.AreEqual<string>("CommonStates", visualStates["Normal"]);
            Assert.AreEqual<string>("CommonStates", visualStates["MouseOver"]);
            Assert.AreEqual<string>("CommonStates", visualStates["Pressed"]);
            Assert.AreEqual<string>("CommonStates", visualStates["Disabled"]);

            Assert.AreEqual<string>("FocusStates", visualStates["Focused"]);
            Assert.AreEqual<string>("FocusStates", visualStates["Unfocused"]);

            Assert.AreEqual<string>("InteractionModeStates", visualStates["Edit"]);
            Assert.AreEqual<string>("InteractionModeStates", visualStates["Display"]);

            Assert.AreEqual<string>("DomainStates", visualStates["InvalidDomain"]);
            Assert.AreEqual<string>("DomainStates", visualStates["ValidDomain"]);

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
            Assert.AreEqual(2, templateParts.Count);
            Assert.AreSame(typeof(TextBox), templateParts["Text"]);
            Assert.AreSame(typeof(Spinner), templateParts["Spinner"]);
        }
        #endregion Contract tests

        /// <summary>
        /// Bind DomainUpDown to a collection of Items should result in a selected object.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Bind DomainUpDown to a collection of Items.")]
        public virtual void ShouldSelectAValueWhenBindingToAnItemsSource()
        {
            DomainUpDown dud = DefaultDomainUpDownToTest;
            IEnumerable domains = new object[] { "1", "2", "3", "4" };

            TestAsync(
                dud,
                () => Assert.IsNull(dud.Value),
                () => dud.ItemsSource = domains,
                () => Assert.AreEqual("1", dud.Value));
        }

        /// <summary>
        /// Tests that changing CurrentIndex also changes selected item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that changing CurrentIndex also changes selected item.")]
        public virtual void ShouldChangeValueWhenChangingIndex()
        {
            DomainUpDown dud = DefaultDomainUpDownToTest;
            IEnumerable domains = new object[] { "1", "2", "3", "4" };
            dud.ItemsSource = domains;

            TestAsync(
                dud,
                () => Assert.AreEqual(0, dud.CurrentIndex),
                () => Assert.AreEqual("1", dud.Value),
                () => dud.CurrentIndex = 2,
                () => Assert.AreEqual("3", dud.Value));
        }

        /// <summary>
        /// Should report current index of -1 on empty domain.
        /// </summary>
        [TestMethod]
        [Description("Should report current index of -1 on empty domain.")]
        public virtual void ShouldReportEmptyDomain()
        {
            DomainUpDown dud = DefaultDomainUpDownToTest;
            ObservableCollection<object> domains = new ObservableCollection<object> { "1" };
            dud.ItemsSource = domains;

            Assert.AreEqual(0, dud.CurrentIndex);
            domains.RemoveAt(0);
            Assert.AreEqual(-1, dud.CurrentIndex);
        }

        /// <summary>
        /// Tests that changing the Value also changes the CurrentIndex.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that changing the Value also changes the CurrentIndex.")]
        public virtual void ShouldChangeCurrentIndexWhenChangingValue()
        {
            DomainUpDown dud = DefaultDomainUpDownToTest;
            IEnumerable domains = new object[] { "1", "2", "3", "4" };
            dud.ItemsSource = domains;

            TestAsync(
                dud,
                () => Assert.AreEqual("1", dud.Value),
                () => dud.Value = domains.OfType<object>().ElementAt(3),
                () => Assert.AreEqual(3, dud.CurrentIndex));
        }

        /// <summary>
        /// Tests that changing the itemssource also changes the Value, keeping the selected index in consideration.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that changing the itemssource also changes the Value, keeping the current index in consideration.")]
        public virtual void ShouldUseCurrentIndexWhenChangingItemsSource()
        {
            DomainUpDown dud = DefaultDomainUpDownToTest;
            IEnumerable domains = new object[] { "1", "2", "3", "4" };
            IEnumerable domains2 = new object[] { "a", "b", "c", "d" };
            dud.ItemsSource = domains;
            dud.CurrentIndex = 2;

            TestAsync(
                dud,
                () => Assert.AreEqual(domains.OfType<object>().ElementAt(2), dud.Value),
                () => dud.ItemsSource = domains2,
                () => Assert.AreEqual(2, dud.CurrentIndex),
                () => Assert.AreEqual("c", dud.Value));
        }

        /// <summary>
        /// Tests that setting currentindex when there are no items is cached.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that setting currentindex when there are no items is cached.")]
        public virtual void ShouldUseOldInvalidCurrentIndexWhenAddingItems()
        {
            DomainUpDown dud = DefaultDomainUpDownToTest;
            dud.CurrentIndex = 2;

            TestAsync(
                dud,
                () => Assert.AreEqual(-1, dud.CurrentIndex),
                () => dud.Items.Add("1"),
                () => dud.Items.Add("2"),
                () => dud.Items.Add("3"),
                () => Assert.AreEqual(2, dud.CurrentIndex),
                () => Assert.AreEqual("3", dud.Value));            
        }

        /// <summary>
        /// Tests that setting currentindex when there is no itemssource set is cached.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that setting currentindex when there is no itemssource set is cached.")]
        public virtual void ShouldUseOldInvalidCurrentIndexWhenSettingItemsSource()
        {
            DomainUpDown dud = DefaultDomainUpDownToTest;
            dud.CurrentIndex = 2;
            IEnumerable domains = new object[] { "1", "2", "3", "4" };

            TestAsync(
                dud,
                () => Assert.AreEqual(-1, dud.CurrentIndex),
                () => dud.ItemsSource = domains,
                () => Assert.AreEqual(2, dud.CurrentIndex),
                () => Assert.AreEqual("3", dud.Value));
        }

        /// <summary>
        /// Tests that first currentindex is cached and second will throw exception.
        /// </summary>
        [TestMethod]
        [Description("Tests that first currentindex is cached and second will throw exception.")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public virtual void ShouldThrowExceptionWhenBadIndexIsSet()
        {
            DomainUpDown dud = DefaultDomainUpDownToTest;
            dud.CurrentIndex = 2;
            dud.CurrentIndex = 1;
        }

        /// <summary>
        /// Tests that first currentindex will not throw exception.
        /// </summary>
        [TestMethod]
        [Description("Tests that first currentindex will not throw exception.")]
        public virtual void ShouldNotThrowExceptionOnFirstBadIndex()
        {
            DomainUpDown dud = DefaultDomainUpDownToTest;
            dud.CurrentIndex = 2;
        }

        /// <summary>
        /// Tests that setting a value when no items are loaded will be cached.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that setting a value when no items are loaded will be cached.")]
        public virtual void ShouldUseOldValueWhenSettingItemsSource()
        {
            DomainUpDown dud = DefaultDomainUpDownToTest;
            dud.Value = "3";
            IEnumerable domains = new object[] { "1", "2", "3", "4" };

            TestAsync(
                dud,
                () => Assert.AreEqual(-1, dud.CurrentIndex),
                () => Assert.AreEqual(null, dud.Value),
                () => dud.ItemsSource = domains,
                () => Assert.AreEqual(2, dud.CurrentIndex),
                () => Assert.AreEqual("3", dud.Value));
        }

        /// <summary>
        /// Tests that setting a value when no items are loaded will be cached.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that setting a value when no items are loaded will be cached.")]
        public virtual void ShouldUseOldValueWhenAddingItems()
        {
            DomainUpDown dud = DefaultDomainUpDownToTest;
            dud.Value = "3";

            TestAsync(
                dud,
                () => Assert.AreEqual(-1, dud.CurrentIndex),
                () => Assert.AreEqual(null, dud.Value),
                () => dud.Items.Add("1"),
                () => dud.Items.Add("2"),
                () => dud.Items.Add("3"),
                () => Assert.AreEqual(2, dud.CurrentIndex),
                () => Assert.AreEqual("3", dud.Value));
        }

        /// <summary>
        /// Tests that changing the itemssource so that both currentindex and value is not valid, first item is selected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that changing the itemssource so that both currentindex and value is not valid, first item is selected.")]
        public virtual void ShouldSelectFirstItemWhenValueAndIndexNotValid()
        {
            DomainUpDown dud = DefaultDomainUpDownToTest;
            IEnumerable domains = new object[] { "1", "2", "3", "4" };
            IEnumerable domains2 = new object[] { "a" };
            dud.ItemsSource = domains;
            dud.CurrentIndex = 2;

            TestAsync(
                dud,
                () => Assert.AreEqual("3", dud.Value),
                () => dud.ItemsSource = domains2,
                () => Assert.AreEqual("a", dud.Value),
                () => Assert.AreEqual(0, dud.CurrentIndex));
        }

        /// <summary>
        /// Tests that changing the itemssource keeps the current value in consideration.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that changing the itemssource keeps the current value in consideration.")]
        public virtual void ShouldUseCurrentValueWhenChangingItemsSource()
        {
            DomainUpDown dud = DefaultDomainUpDownToTest;
            IEnumerable domains = new object[] { "1", "2", "3", "4" };
            IEnumerable domains2 = new object[] { "a", "b", "c", "3" };
            dud.ItemsSource = domains;
            dud.CurrentIndex = 2;   // so value "3" 

            TestAsync(
                dud,
                () => Assert.AreEqual("3", dud.Value),
                () => Assert.AreEqual(2, dud.CurrentIndex),
                () => dud.ItemsSource = domains2,
                () => Assert.AreEqual("3", dud.Value),
                () => Assert.AreEqual(3, dud.CurrentIndex));
        }

        /// <summary>
        /// Initialize through xaml should set items.
        /// </summary>
        [TestMethod]
        [Description("Initialize through xaml should set items.")]
        public virtual void ShouldBeAbleToInitializeThroughXamlAndSettingItems()
        {
            DomainUpDownItemsInitialization test = new DomainUpDownItemsInitialization();
            DomainUpDown dud = test.dud;
            Assert.IsNotNull(dud);

            Assert.AreEqual(2, dud.Items.Count);

            dud.Items.Add("another string");

            Assert.AreEqual(3, dud.Items.Count);
        }

        /// <summary>
        /// Setting ItemsSource should redirect Items.
        /// </summary>
        [TestMethod]
        [Description("Setting ItemsSource should redirect Items.")]
        public virtual void ShouldRedirectItemsWhenItemsSourceIsSet()
        {
            DomainUpDownItemsInitialization test = new DomainUpDownItemsInitialization();
            DomainUpDown dud = test.FindName("dud") as DomainUpDown;
            Assert.IsNotNull(dud);

            IEnumerable domainSource = new object[] { 1, 2, 3, 4, 5, 6, 7 };

            dud.ItemsSource = domainSource;

            Assert.AreEqual(7, dud.Items.Count);
        }

        /// <summary>
        /// Adding Items when ItemsSource is set should throw exception.
        /// </summary>
        [TestMethod]
        [Description("Adding Items when ItemsSource is set should throw exception.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public virtual void ShouldThrowExceptionOnItemsManipulationWhenItemsSourceIsSet()
        {
            DomainUpDown dud = DefaultDomainUpDownToTest;
            IEnumerable domains = new object[] { "1", "2", "3", "4" };
            dud.ItemsSource = domains;

            dud.Items.Add("item should not be able to be added when itemssource is set");
        }

        /// <summary>
        /// Changing ItemsSource to null allows manipulation of items collection.
        /// </summary>
        [TestMethod]
        [Description("Changing ItemsSource to null allows manipulation of items collection.")]
        public virtual void ShouldResetItemsWhenItemsSourceIsSetToNull()
        {
            DomainUpDown dud = DefaultDomainUpDownToTest;
            IEnumerable domains = new object[] { "1", "2", "3", "4" };
            dud.Items.Add("1");
            dud.ItemsSource = domains;

            dud.ItemsSource = null;

            dud.Items.Add("first Item");
            dud.Items.Add("second Item");

            Assert.AreEqual(2, dud.Items.Count);
        }

        /// <summary>
        /// Pre select first item when adding through items.
        /// </summary>
        [TestMethod]
        [Description("Pre select first item when adding through items.")]
        public virtual void ShouldSelectAValueWhenAddingItems()
        {
            DomainUpDown dud = new DomainUpDown();
            dud.Items.Add("1");
            dud.Items.Add("2");

            Assert.AreEqual("1", dud.Value);
        }

        /// <summary>
        /// Remove value when removed from itemssource.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Remove selected item when removed from itemssource.")]
        public virtual void ShouldRemoveValueIfNoLongerInItemsSource()
        {
            DomainUpDown dud = new DomainUpDown();
            ObservableCollection<int> domain = new ObservableCollection<int>() { 1, 2, 3, 4 };
            dud.ItemsSource = domain;
            dud.Value = 3;

            TestAsync(
                dud,
                () => Assert.AreEqual(3, dud.Value),
                () => domain.Remove(3),
                () => Assert.AreEqual(1, dud.Value));
        }

        /// <summary>
        /// Remove value when removed from items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Remove selected item when removed from items.")]
        public virtual void ShouldRemoveValueIfNoLongerInItems()
        {
            DomainUpDown dud = new DomainUpDown();
            dud.Items.Add(1);
            dud.Items.Add(2);
            dud.Items.Add(3);
            dud.Value = 3;

            int oldvalue = -1, newvalue = -1;

            dud.ValueChanged += (s, e) =>
            {
                oldvalue = (int)e.OldValue;
                newvalue = (int)e.NewValue;
            };

            TestAsync(
                dud,
                () => Assert.AreEqual(3, dud.Value),
                () => dud.Items.Remove(3),
                () => Assert.AreEqual(3, oldvalue),
                () => Assert.AreEqual(1, newvalue),
                () => Assert.AreEqual(1, dud.Value));
        }

        /// <summary>
        /// Changing the Value should raise the ValueChanged event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Changing the Value should raise the ValueChanged event.")]
        public virtual void ShouldRaiseValueChangedEvent()
        {
            DomainUpDown dud = new DomainUpDown();
            dud.Items.Add("1");
            dud.Items.Add("2");
            dud.Items.Add("3");

            object oldItem = null, newItem = null;
            dud.ValueChanged += (sender, e) =>
                                           {
                                               oldItem = e.OldValue;
                                               newItem = e.NewValue;
                                           };

            TestAsync(
                dud,
                () => dud.CurrentIndex = 1,
                () => Assert.AreEqual("1", oldItem),
                () => Assert.AreEqual("2", newItem),
                () => dud.CurrentIndex = 2,
                () => Assert.AreEqual("2", oldItem),
                () => Assert.AreEqual("3", newItem),
                () => dud.Value = "1",
                () => Assert.AreEqual("3", oldItem),
                () => Assert.AreEqual("1", newItem));
        }

        /// <summary>
        /// Test Incrementing and decrement.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test Incrementing and decrement.")]
        public virtual void ShouldIncrementAndDecrement()
        {
            OverriddenDomainUpDown dud = new OverriddenDomainUpDown();
            dud.Items.Add(1);
            dud.Items.Add(2);
            dud.Items.Add(3);
            dud.Items.Add(4);
            dud.IsCyclic = false;

            TestAsync(
                dud,
                () => Assert.AreEqual(1, dud.Value),
                () => dud.Decrement(),
                () => Assert.AreEqual(2, dud.Value),
                () => Assert.AreEqual(1, dud.CurrentIndex),
                () => dud.Decrement(),
                () => dud.Decrement(),
                () => Assert.AreEqual(4, dud.Value),
                () => dud.Decrement(),
                () => Assert.AreEqual(4, dud.Value),
                () => dud.Increment(),
                () => dud.Increment(),
                () => dud.Increment(),
                () => dud.Increment(),
                () => Assert.AreEqual(1, dud.Value),
                () => dud.Increment(),
                () => Assert.AreEqual(1, dud.Value));
        }

        /// <summary>
        /// Test Increment and decrement with cyclic set to true.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Test Increment and decrement with cyclic set to true.")]
        public virtual void ShouldIncrementIndefinitelyWhenCyclic()
        {
            OverriddenDomainUpDown dud = new OverriddenDomainUpDown();
            dud.Items.Add(1);
            dud.Items.Add(2);
            dud.Items.Add(3);
            dud.Items.Add(4);
            dud.IsCyclic = true;

            TestAsync(
                dud,
                () => Assert.AreEqual(1, dud.Value),
                () => dud.Decrement(),
                () => Assert.AreEqual(2, dud.Value),
                () => Assert.AreEqual(1, dud.CurrentIndex),
                () => dud.Decrement(),
                () => dud.Decrement(),
                () => Assert.AreEqual(4, dud.Value),
                () => dud.Decrement(),
                () => dud.Decrement(),
                () => Assert.AreEqual(2, dud.Value),
                () => dud.Increment(),
                () => dud.Increment(),
                () => dud.Increment(),
                () => dud.Increment(),
                () => dud.Increment(),
                () => Assert.AreEqual(1, dud.Value),
                () => dud.Increment(),
                () => Assert.AreEqual(4, dud.Value));
        }

        /// <summary>
        /// Setting the text should select item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Setting the text should select item.")]
        public virtual void ShouldSelectItemWhenTextChanges()
        {
            OverriddenDomainUpDown dud = new OverriddenDomainUpDown();
            dud.Items.Add("a");
            dud.Items.Add("b");
            dud.Items.Add("c");

            TestAsync(
                dud,
                () => dud.ApplyText("b"),
                () => Assert.AreEqual(1, dud.CurrentIndex));
        }

        /// <summary>
        /// Setting invalid text should not change value.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Setting invalid text should not change value")]
        public virtual void ShouldNotReactToInvalidText()
        {
            OverriddenDomainUpDown dud = new OverriddenDomainUpDown();
            dud.Items.Add("a");
            dud.Items.Add("b");
            dud.Items.Add("c");

            TestAsync(
                dud,
                () => dud.ApplyText("b"),
                () => dud.ApplyText("d"),
                () => Assert.AreEqual(1, dud.CurrentIndex));
        }

        /// <summary>
        /// Setting invalid value should not change value.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Setting invalid value should not change value.")]
        public virtual void ShouldNotReactToInvalidValue()
        {
            OverriddenDomainUpDown dud = new OverriddenDomainUpDown();
            dud.Items.Add("a");
            dud.Items.Add("b");
            dud.Items.Add("c");

            TestAsync(
                dud,
                () => dud.CurrentIndex = 1,
                () => dud.Value = "d",
                () => Assert.AreEqual("b", dud.Value));
        }

        /// <summary>
        /// Setting invalid value should take into account the fallback and acceptsInvalidInput settings.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Setting invalid value should take into account the fallback and acceptsInvalidInput settings")]
        public virtual void ShouldHandleInvalidTextCorrectly()
        {
            OverriddenDomainUpDown dud = new OverriddenDomainUpDown();
            dud.Items.Add("a");
            dud.Items.Add("b");
            dud.Items.Add("c");
            dud.Items.Add("fallback");

            dud.InvalidInputAction = InvalidInputAction.UseFallbackItem;
            dud.FallbackItem = "fallback";

            TestAsync(
                dud,
                () => dud.CurrentIndex = 1,
                () => dud.ApplyText("does not exist"),
                () => Assert.AreEqual("fallback", dud.Value),
                () => dud.FallbackItem = "fallback does not exist",
                () => dud.CurrentIndex = 1,
                () => dud.ApplyText("does not exist"),
                () => Assert.AreEqual("b", dud.Value));
        }

        /// <summary>
        /// Should return to display mode after valid input was parsed.
        /// </summary>
        [TestMethod]
        [Description("Should return to display mode after valid input was parsed.")]
        public virtual void ShouldReturnToDisplayAfterValidValue()
        {
            OverriddenDomainUpDown dud = new OverriddenDomainUpDown();
            dud.Items.Add("a");
            dud.Items.Add("b");
            dud.Items.Add("c");
            dud.Items.Add("fallback");

            dud.InvalidInputAction = InvalidInputAction.UseFallbackItem;
            dud.FallbackItem = "fallback";

            dud.CurrentIndex = 1;
            dud.ApplyText("b");
            Assert.AreEqual(false, dud.IsEditing);
        }

        /// <summary>
        /// Should return to display mode after invalid input was parsed.
        /// </summary>
        [TestMethod]
        [Description("Should return to display mode after invalid input was parsed.")]
        public virtual void ShouldReturnToDisplayAfterInvalidValue()
        {
            OverriddenDomainUpDown dud = new OverriddenDomainUpDown();
            dud.Items.Add("a");
            dud.Items.Add("b");
            dud.Items.Add("c");
            dud.Items.Add("fallback");

            dud.InvalidInputAction = InvalidInputAction.UseFallbackItem;
            dud.FallbackItem = "fallback";

            dud.CurrentIndex = 1;
            dud.ApplyText("does not exist");
            Assert.AreEqual(false, dud.IsEditing);
        }

        /// <summary>
        /// Should remain in edit mode after invalid input was parsed
        /// with TextBoxCannotLoseFocus action.
        /// </summary>
        [TestMethod]
        [Description("Should remain in edit mode after invalid input was parsed with TextBoxCannotLoseFocus action.")]
        public virtual void ShouldRemainInEditModeAfterInvalidInputAndTextBoxCannotLoseFocusAction()
        {
            OverriddenDomainUpDown dud = new OverriddenDomainUpDown();
            dud.Items.Add("a");
            dud.Items.Add("b");
            dud.Items.Add("c");
            dud.Items.Add("fallback");

            dud.InvalidInputAction = InvalidInputAction.TextBoxCannotLoseFocus;
            dud.FallbackItem = "fallback";

            dud.CurrentIndex = 1;
            dud.ApplyText("does not exist");
            Assert.AreEqual(true, dud.IsEditing);
        }

        /// <summary>
        /// Should not allow index at -1 when there are items.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Wish to test behavior after the exception.")]
        [TestMethod]
        [Description("Should not allow index at -1 when there are items.")]
        public virtual void ShouldSelectFirstItemWhenIndexSetToMinus1()
        {
            DomainUpDown dud = DefaultDomainUpDownToTest;
            dud.Items.Add("a");
            dud.Items.Add("b");

            Assert.AreEqual(0, dud.CurrentIndex);
            try
            {
                dud.CurrentIndex = -1;
            }
            catch (Exception)
            {
                // swallow exception so we can finish test.
            }
            // should be coerced to 0
            Assert.AreEqual(0, dud.CurrentIndex);
        }

        /// <summary>
        /// Tests that the button spinner gets indications about valid directions.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that the button spinner gets indications about valid directions.")]
        public virtual void ShouldDisallowSpinsWhenDomainBordersAreReached()
        {
            DomainUpDown dud = new DomainUpDown();
            dud.Items.Add("a");
            dud.Items.Add("b");
            dud.Items.Add("c");

            ButtonSpinner spinner = null;

            // dud changes increment decrement logic

            TestAsync(
                dud,
                () => spinner = ((Panel)VisualTreeHelper.GetChild(dud, 0)).FindName("Spinner") as ButtonSpinner,
                () => Assert.AreEqual(ValidSpinDirections.Decrease, spinner.ValidSpinDirection),
                () => dud.Value = "b",
                () => Assert.IsTrue(spinner.ValidSpinDirection == (ValidSpinDirections.Increase | ValidSpinDirections.Decrease)),
                () => dud.Value = "c",
                () => Assert.AreEqual(ValidSpinDirections.Increase, spinner.ValidSpinDirection),
                () => dud.IsCyclic = true,
                () => Assert.IsTrue(spinner.ValidSpinDirection == (ValidSpinDirections.Increase | ValidSpinDirections.Decrease)));
        }

        /// <summary>
        /// Tests that focus is set after a spin.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag("RequiresFocus")]
        [Description("Tests that focus is set after a spin.")]
        public virtual void ShouldGrabFocusWhenSpinning()
        {
            OverriddenDomainUpDown dud = new OverriddenDomainUpDown();
            dud.Items.Add("a");
            dud.Items.Add("b");
            dud.Items.Add("c");
            dud.CurrentIndex = 1;

            // using timer so that the tests never get blocked
            Timer timer = new Timer(state => Assert.Fail("Did not get focus in time. This test has been reported to fail sometimes on first run."), new object(), 2000, 0);
            
            Button b = new Button();

            bool isLoaded = false;
            dud.Loaded += delegate { isLoaded = true; };

            bool buttonHasFocus = false;
            b.GotFocus += delegate
                              {
                                  buttonHasFocus = true;
                                  // potentially null 
                                  if (timer != null)
                                  {
                                      // reset the timer for another period.
                                      timer.Change(2000, 0);
                                  }
                              };

            EnqueueCallback(() => TestPanel.Children.Add(dud));
            EnqueueCallback(() => TestPanel.Children.Add(b));
            EnqueueConditional(() => isLoaded);
            EnqueueCallback(() => b.Focus());
            EnqueueConditional(() => buttonHasFocus);
            EnqueueCallback(() => buttonHasFocus = false);
            EnqueueCallback(() => Assert.IsTrue(Input.FocusManager.GetFocusedElement() == b));
            EnqueueCallback(() => dud.Increment());
            EnqueueVisualDelay(500);
            EnqueueCallback(() => Assert.IsTrue(Input.FocusManager.GetFocusedElement() != b));
            EnqueueCallback(() => Assert.IsTrue(dud.IsEditing == false));
            EnqueueCallback(() => b.Focus());
            EnqueueConditional(() => buttonHasFocus);
            EnqueueCallback(() => timer.Dispose());     // get rid of the timer.
            EnqueueCallback(() => timer = null);
            EnqueueCallback(() => dud.Decrement());
            EnqueueVisualDelay(500);
            EnqueueCallback(() => Assert.IsTrue(Input.FocusManager.GetFocusedElement() != b));
            EnqueueCallback(() => Assert.IsTrue(dud.IsEditing == false));

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests that style for ButtonSpinner is templatebound.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that style for ButtonSpinner is templatebound.")]
        public virtual void ShouldStyleButtonSpinner()
        {
            OverriddenDomainUpDown dud = new OverriddenDomainUpDown();

            Style s = new Style(typeof(Spinner));
            s.Setters.Add(new Setter(FrameworkElement.TagProperty, "flowed down"));

            TestAsync(
                dud,
                () => dud.SpinnerStyle = s,
                () => Assert.AreEqual("flowed down", dud.Spinner.Tag));
        }

        /// <summary>
        /// Tests ValueMemberBinding.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests ValueMemberBinding.")]
        public virtual void ShouldUseValueMemberBinding()
        {
            OverriddenDomainUpDown dud = new OverriddenDomainUpDown();
            dud.ValueMemberBinding = new Binding("Value");
            dud.ItemsSource = new[]
                                  {
                                          new KeyValuePair<string, string>("a", "b"),
                                          new KeyValuePair<string, string>("c", "d"),
                                  };

            TestAsync(
                dud,
                () => dud.ApplyText("d"),
                () => Assert.AreEqual(1, dud.CurrentIndex));
        }

        /// <summary>
        /// Tests ValueMemberPath.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests ValueMemberPath.")]
        public virtual void ShouldUseValueMemberPath()
        {
            OverriddenDomainUpDown dud = new OverriddenDomainUpDown();
            dud.ValueMemberPath = "Value";
            dud.ItemsSource = new[]
                                  {
                                          new KeyValuePair<string, string>("a", "b"),
                                          new KeyValuePair<string, string>("c", "d"),
                                  };

            TestAsync(
                dud,
                () => dud.ApplyText("d"),
                () => Assert.AreEqual(1, dud.CurrentIndex));
        }

        /// <summary>
        /// Tests that automation peer does not get null values.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that automation peer does not get null values.")]
        public virtual void ShouldNotGiveAutomationPeerNullValues()
        {
            DomainUpDown dud = new DomainUpDown();
            DomainUpDownAutomationPeer peer = null;
            IValueProvider valueProvider = null;

            TestAsync(
                    dud,
                    () => peer = (DomainUpDownAutomationPeer)FrameworkElementAutomationPeer.CreatePeerForElement(dud),
                    () => valueProvider = (IValueProvider) peer.GetPattern(PatternInterface.Value),
                    () => dud.ItemsSource = Enumerable.Range(0, 20).ToList(),
                    () => Assert.AreEqual("0", valueProvider.Value),
                    () => dud.ItemsSource = null);  // should not throw
        }

        /// <summary>
        /// Tests that ItemsSource can be set to zero.
        /// </summary>
        [TestMethod]
        [Description("Tests that ItemsSource can be set to zero.")]
        public virtual void ShouldHandleSettingItemsSourceToZeroCountCollection()
        {
            DomainUpDown dud = new DomainUpDown();
            dud.ItemsSource = new object[0];

            Assert.AreEqual(-1, dud.CurrentIndex);
            Assert.IsNull(dud.Value);
        }

        /// <summary>
        /// Tests that DomainUpDown is not keeping a strong reference to ItemsSource.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "Testing the effect of GC on the control.")]
        [TestMethod]
        [Description("Tests that DomainUpDown is not keeping a strong reference to ItemsSource.")]
        public virtual void ShouldNotKeepStrongReferenceToItemsSourceThroughCollectionChangedEvent()
        {
            ObservableCollection<int> collection = new ObservableCollection<int>();
            WeakReference weakReference = new WeakReference(new DomainUpDown());
            ((DomainUpDown)weakReference.Target).ItemsSource = collection;
            GC.Collect();
            Assert.IsNull(weakReference.Target);
        }
    }
}
