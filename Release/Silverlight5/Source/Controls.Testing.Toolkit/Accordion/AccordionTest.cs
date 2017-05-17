// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Data;

[assembly: SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "accordion", Scope = "member", Target = "System.Windows.Controls.Testing.AccordionTest.#GetAccordionItem(System.Windows.Controls.Accordion,System.Object)", Justification = "Temporary changes.")]
[assembly: SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "item", Scope = "member", Target = "System.Windows.Controls.Testing.AccordionTest.#GetAccordionItem(System.Windows.Controls.Accordion,System.Object)", Justification = "Temporary changes.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "item", Scope = "member", Target = "System.Windows.Controls.Testing.AccordionTest.#ShouldCreateAccordionItems()", Justification = "Temporary changes.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "item", Scope = "member", Target = "System.Windows.Controls.Testing.AccordionTest.#ShouldUpdateItemsWithExpandDirection()", Justification = "Temporary changes.")]

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Accordion unit tests.
    /// </summary>
    [TestClass]
    [Tag("Accordion")]
    public class AccordionTest : ItemsControlTest
    {
        /// <summary>
        /// Gets a default instance of FrameworkElement (or a derived type) to
        /// test.
        /// </summary>
        /// <value></value>
        public override FrameworkElement DefaultFrameworkElementToTest
        {
            get
            {
                return new Accordion();
            }
        }

        /// <summary>
        /// Gets a default instance of ItemsControl (or a derived type) to test.
        /// </summary>
        /// <value></value>
        public override ItemsControl DefaultItemsControlToTest
        {
            get { return new Accordion(); }
        }

        /// <summary>
        /// Gets instances of ItemsControl (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<ItemsControl> ItemsControlsToTest
        {
            get { return AccordionsToTest.OfType<ItemsControl>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenItemsControl (or derived types) to test.
        /// </summary>
        /// <value></value>
        public override IEnumerable<IOverriddenItemsControl> OverriddenItemsControlsToTest
        {
            get { yield break; }
        }

        /// <summary>
        /// Gets instances of Accordion (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<Accordion> AccordionsToTest
        {
            get
            {
                yield return DefaultAccordionToTest;
            }
        }

        /// <summary>
        /// Gets the default accordion to test.
        /// </summary>
        /// <value>The default accordion to test.</value>
        public virtual Accordion DefaultAccordionToTest
        {
            get
            {
                Accordion accordion = new Accordion();
                accordion.Items.Add("item 1");
                accordion.Items.Add("item 2");
                accordion.Items.Add("item 3");
                accordion.Items.Add(new Border() { Width = 30, Height = 30, Background = new SolidColorBrush(Colors.Blue) });
                accordion.Items.Add(new AccordionItem() { Content = "item 5", Header = "header of item 5" });
                return accordion;
            }
        }

        #region DependencyProperties
        /// <summary>
        /// Gets the ExpandDirection dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Accordion, ExpandDirection> ExpandDirectionProperty { get; private set; }

        /// <summary>
        /// Gets the SelectionMode dependency property test.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "Based on WPF MultiSelector.")]
        protected DependencyPropertyTest<Accordion, AccordionSelectionMode> SelectionModeProperty { get; private set; }

        /// <summary>
        /// Gets the SelectedItem dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Accordion, object> SelectedItemProperty { get; private set; }

        /// <summary>
        /// Gets the SelectedIndex dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Accordion, int> SelectedIndexProperty { get; private set; }

        /// <summary>
        /// Gets the SelectionSequence dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Accordion, SelectionSequence> SelectionSequenceProperty { get; private set; }

        /// <summary>
        /// Gets the SelectedItems dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Accordion, IList> SelectedItemsProperty { get; private set; }

        /// <summary>
        /// Gets the SelectedIndices dependency property test.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "Frameworks use the term indices.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Set out by Accordion")]
        protected DependencyPropertyTest<Accordion, IList<int>> SelectedIndicesProperty { get; private set; }

        /// <summary>
        /// Gets the ItemContainerStyle dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Accordion, Style> ItemContainerStyleProperty { get; private set; }

        /// <summary>
        /// Gets the ContentTemplate dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Accordion, DataTemplate> ContentTemplateProperty { get; private set; }
        #endregion DependencyProperties

        /// <summary>
        /// Initializes a new instance of the <see cref="AccordionTest"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Testing a complex control.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Testing a complex control.")]
        public AccordionTest()
        {
            ForegroundProperty.DefaultValue = new SolidColorBrush(Colors.Black);
            PaddingProperty.DefaultValue = new Thickness(0);

            BorderBrushProperty.DefaultValue = new SolidColorBrush(Color.FromArgb(0xff, 0xea, 0xea, 0xea));
            BorderThicknessProperty.DefaultValue = new Thickness(1);

            HorizontalAlignmentProperty.DefaultValue = HorizontalAlignment.Left;
            VerticalAlignmentProperty.DefaultValue = VerticalAlignment.Top;
            HorizontalContentAlignmentProperty.DefaultValue = HorizontalAlignment.Left;
            VerticalContentAlignmentProperty.DefaultValue = VerticalAlignment.Top;
            ItemsPanelProperty.DefaultValue = new ItemsPanelTemplate();

            Func<Accordion> initializer = () => new Accordion();

            ExpandDirectionProperty = new DependencyPropertyTest<Accordion, ExpandDirection>(this, "ExpandDirection")
                                          {
                                              Property = Accordion.ExpandDirectionProperty,
                                              Initializer = initializer,
                                              DefaultValue = ExpandDirection.Down,
                                              OtherValues =
                                                  new[] { ExpandDirection.Left, ExpandDirection.Right, ExpandDirection.Up },
                                              InvalidValues = new Dictionary<ExpandDirection, Type>
                                                                  {
                                                                      { (ExpandDirection)99, typeof(ArgumentOutOfRangeException) },
                                                                      { (ExpandDirection)66, typeof(ArgumentOutOfRangeException) }
                                                                  }
                                          };

            SelectionModeProperty = new DependencyPropertyTest<Accordion, AccordionSelectionMode>(this, "SelectionMode")
                                             {
                                                 Property = Accordion.SelectionModeProperty,
                                                 Initializer = initializer,
                                                 DefaultValue = AccordionSelectionMode.One,
                                                 OtherValues =
                                                     new[]
                                                         {
                                                             AccordionSelectionMode.OneOrMore, AccordionSelectionMode.ZeroOrMore,
                                                             AccordionSelectionMode.ZeroOrOne
                                                         },
                                                 InvalidValues = new Dictionary<AccordionSelectionMode, Type>
                                                                     {
                                                                         { (AccordionSelectionMode)99, typeof(ArgumentOutOfRangeException) },
                                                                         { (AccordionSelectionMode)66, typeof(ArgumentOutOfRangeException) }
                                                                     }
                                             };
            SelectedItemProperty = new DependencyPropertyTest<Accordion, object>(this, "SelectedItem")
                                        {
                                            Property = Accordion.SelectedItemProperty,
                                            Initializer = initializer,
                                            DefaultValue = null,
                                            OtherValues = new object[] { new object() }
                                        };

            SelectedIndexProperty = new DependencyPropertyTest<Accordion, int>(this, "SelectedIndex")
                                        {
                                            Property = Accordion.SelectedIndexProperty,
                                            Initializer = initializer,
                                            DefaultValue = -1
                                        };

            SelectionSequenceProperty = new DependencyPropertyTest<Accordion, SelectionSequence>(this, "SelectionSequence")
                                            {
                                                Property = Accordion.SelectionSequenceProperty,
                                                Initializer = initializer,
                                                DefaultValue = SelectionSequence.Simultaneous,
                                                OtherValues = new[] { SelectionSequence.CollapseBeforeExpand },
                                                InvalidValues = new Dictionary<SelectionSequence, Type>
                                                                    {
                                                                        { (SelectionSequence)99, typeof(ArgumentOutOfRangeException) },
                                                                        { (SelectionSequence)66, typeof(ArgumentOutOfRangeException) }
                                                                    }
                                            };

            SelectedItemsProperty = new DependencyPropertyTest<Accordion, IList>(this, "SelectedItems")
                                        {
                                            Property = Accordion.SelectedItemsProperty,
                                            Initializer = initializer,
                                            DefaultValue = new ObservableCollection<object>()
                                        };
            SelectedIndicesProperty = new DependencyPropertyTest<Accordion, IList<int>>(this, "SelectedIndices")
                                          {
                                              Property = Accordion.SelectedIndicesProperty,
                                              Initializer = initializer,
                                              DefaultValue = new ObservableCollection<int>()
                                          };
            ItemContainerStyleProperty = new DependencyPropertyTest<Accordion, Style>(this, "ItemContainerStyle")
                                        {
                                            Property = Accordion.ItemContainerStyleProperty,
                                            Initializer = initializer,
                                            DefaultValue = null,
                                            OtherValues = new[] 
                                            {
                                                new Style(typeof(HeaderedItemsControl)), 
                                                new Style(typeof(ItemsControl)), 
                                                new Style(typeof(Control)) 
                                            }
                                        };
            ContentTemplateProperty = new DependencyPropertyTest<Accordion, DataTemplate>(this, "ContentTemplate")
                                        {
                                            Property = Accordion.ContentTemplateProperty,
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

            // Default ItemsSource tests would all fail because they are not allowed 
            // to change the SelectedItemCollections in the default SelectionMode.
            ItemsSourceProperty.Initializer = () => new Accordion()
                                                        {
                                                                SelectionMode = AccordionSelectionMode.ZeroOrMore
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

            // Remove certain tests that can't run because they compare on an instance
            tests.Remove(tests.Where(t => t.Name == ItemsPanelProperty.CheckDefaultValueTest.Name).First());
            tests.Remove(tests.Where(t => t.Name == ItemsPanelProperty.ClearValueResetsDefaultTest.Name).First());

            // ExpandDirection tests
            tests.Add(ExpandDirectionProperty.CheckDefaultValueTest);
            tests.Add(ExpandDirectionProperty.ChangeSetValueTest);

            // SelectionMode tests
            tests.Add(SelectionModeProperty.CheckDefaultValueTest);
            tests.Add(SelectionModeProperty.ChangeSetValueTest);
            
            // SelectedItem tests
            tests.Add(SelectedItemProperty.CheckDefaultValueTest);

            // SelectedIndex tests
            tests.Add(SelectedIndexProperty.CheckDefaultValueTest);

            // SelectionSequence tests
            tests.Add(SelectionSequenceProperty.CheckDefaultValueTest);
            tests.Add(SelectionSequenceProperty.ChangeSetValueTest);

            // ContentTemplate tests
            tests.Add(ContentTemplateProperty.CheckDefaultValueTest);
            tests.Add(ContentTemplateProperty.ChangeClrSetterTest);
            tests.Add(ContentTemplateProperty.ChangeSetValueTest);
            tests.Add(ContentTemplateProperty.SetNullTest);
            tests.Add(ContentTemplateProperty.ClearValueResetsDefaultTest);
            tests.Add(ContentTemplateProperty.CanBeStyledTest);
            tests.Add(ContentTemplateProperty.TemplateBindTest);

            return tests;
        }

        #region Control contract
        /// <summary>
        /// Verify the control's style typed properties.
        /// </summary>
        public override void StyleTypedPropertiesAreDefined()
        {
            Assert.AreEqual(2, DefaultAccordionToTest.GetType().GetStyleTypedProperties().Count);
        }

        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's template visual states.")]
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> visualStates = new Accordion().GetType().GetVisualStates();
            Assert.AreEqual(6, visualStates.Count);

            Assert.AreEqual<string>("CommonStates", visualStates["Normal"]);
            Assert.AreEqual<string>("CommonStates", visualStates["MouseOver"]);
            Assert.AreEqual<string>("CommonStates", visualStates["Pressed"]);
            Assert.AreEqual<string>("CommonStates", visualStates["Disabled"]);

            Assert.AreEqual<string>("FocusStates", visualStates["Focused"]);
            Assert.AreEqual<string>("FocusStates", visualStates["Unfocused"]);
        }
        #endregion

        /// <summary>
        /// Tests that accordion items are of type accordionItem.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that accordion items are of type accordionItem.")]
        public virtual void ShouldCreateAccordionItems()
        {
            Accordion accordion = DefaultAccordionToTest;

            TestAsync(
                accordion,
                () =>
                {
                    foreach (object item in accordion.Items)
                    {
                        Assert.IsNotNull(accordion.ItemContainerGenerator.ContainerFromItem(item) as AccordionItem);
                    }
                });
        }

        /// <summary>
        /// Tests that accordion items have the correct expanddirection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that accordion items have the correct expanddirection.")]
        public virtual void ShouldUpdateItemsWithExpandDirection()
        {
            Accordion accordion = DefaultAccordionToTest;
            accordion.ExpandDirection = ExpandDirection.Left;

            TestAsync(
                accordion,
                () =>
                {
                    foreach (object item in accordion.Items)
                    {
                        AccordionItem accordionItem = (AccordionItem)accordion.ItemContainerGenerator.ContainerFromItem(item);
                        Assert.AreEqual(ExpandDirection.Left, accordionItem.ExpandDirection);
                    }
                },
                () => accordion.Items.Add("new item"),
                () => Assert.AreEqual(ExpandDirection.Left, ((AccordionItem)accordion.ItemContainerGenerator.ContainerFromItem("new item")).ExpandDirection),
                () => accordion.ExpandDirection = ExpandDirection.Right,
                () =>
                {
                    foreach (object item in accordion.Items)
                    {
                        AccordionItem accordionItem = (AccordionItem)accordion.ItemContainerGenerator.ContainerFromItem(item);
                        Assert.AreEqual(ExpandDirection.Right, accordionItem.ExpandDirection);
                    }
                });
        }

        #region SingleSelection tests
        /// <summary>
        /// Tests that only a valid item can be selected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that only a valid item can be selected.")]
        public virtual void ShouldOnlyAllowValidItemToBeSelected()
        {
            Accordion accordion = DefaultAccordionToTest;
            accordion.SelectionMode = AccordionSelectionMode.One;

            int count = 0;
            accordion.SelectionChanged += (sender, e) => count += 1;

            TestAsync(
                accordion,
                () => accordion.SelectedItem = "item 2",
                () => Assert.AreEqual(1, count),
                () => accordion.SelectedItem = "item not in accordion.",
                () => Assert.AreEqual(1, count, "should not have raised event because the item is invalid."),
                () => Assert.AreEqual("item 2", accordion.SelectedItem),
                () => accordion.SelectedItem = "item 3",
                () => Assert.AreEqual(2, count),
                () => Assert.AreEqual("item 3", accordion.SelectedItem));
        }

        /// <summary>
        /// Tests that changing the selection will expand the new accordionItem and collapse the old accordionItem.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that changing the selection will expand the new accordionItem and collapse the old accordionItem.")]
        public virtual void ShouldExpandAndCollapseWhenSelectedItemChanges()
        {
            Accordion accordion = DefaultAccordionToTest;
            accordion.SelectionMode = AccordionSelectionMode.One;
            accordion.SelectedItem = "item 1";

            TestAsync(
                accordion,
                () => Assert.IsTrue(GetAccordionItem(accordion, "item 1").IsSelected),
                () => accordion.SelectedItem = "item 2",
                () => Assert.IsTrue(GetAccordionItem(accordion, "item 2").IsSelected),
                () => Assert.IsFalse(GetAccordionItem(accordion, "item 1").IsSelected));
        }

        /// <summary>
        /// Tests that accordionItem gets selected when expanded.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that accordionItem gets selected when expanded.")]
        public virtual void ShouldSelectAccordionItemWhenExpanded()
        {
            Accordion accordion = DefaultAccordionToTest;
            accordion.SelectionMode = AccordionSelectionMode.One;

            TestAsync(
                accordion,
                () => GetAccordionItem(accordion, "item 2").IsSelected = true,
                () => Assert.AreEqual("item 2", accordion.SelectedItem),
                () => GetAccordionItem(accordion, "item 3").IsSelected = true,
                () => Assert.AreEqual("item 3", accordion.SelectedItem),
                () => Assert.IsFalse(GetAccordionItem(accordion, "item 2").IsSelected));
        }

        /// <summary>
        /// Tests that accordionItem gets deselected when collapsed.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that accordionItem gets deselected when collapsed.")]
        public virtual void ShouldDeselectAccordionItemWhenCollapsed()
        {
            Accordion accordion = DefaultAccordionToTest;
            accordion.SelectionMode = AccordionSelectionMode.OneOrMore;

            TestAsync(
                accordion,
                () => accordion.SelectedItem = "item 2",
                () => GetAccordionItem(accordion, "item 2").IsSelected = false,
                () => Assert.AreEqual("item 1", accordion.SelectedItem),
                () => Assert.IsFalse(GetAccordionItem(accordion, "item 2").IsSelected),
                () => Assert.IsTrue(GetAccordionItem(accordion, "item 1").IsSelected),
                () => accordion.SelectionMode = AccordionSelectionMode.ZeroOrOne,
                () => GetAccordionItem(accordion, "item 1").IsSelected = false,
                () => Assert.IsNull(accordion.SelectedItem));
        }

        /// <summary>
        /// Tests that last accordionItem can not be deselected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [ExpectedException(typeof(InvalidOperationException))]
        [Description("Tests that last accordionItem can not be deselected.")]
        public virtual void ShouldLockLastAccordionItem()
        {
            Accordion accordion = DefaultAccordionToTest;
            accordion.SelectionMode = AccordionSelectionMode.One;

            TestAsync(
                    accordion,
                    () => accordion.SelectedItem = "item 2",
                    () => Assert.IsTrue(GetAccordionItem(accordion, "item 2").IsLocked),
                    () => accordion.SelectedItem = "item 1",
                    () => Assert.IsFalse(GetAccordionItem(accordion, "item 2").IsLocked),
                    () => GetAccordionItem(accordion, "item 1").IsSelected = false); // will throw
        }

        /// <summary>
        /// Tests IsMinimumOneSelected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests IsMinimumOneSelected")]
        public virtual void ShouldSupportIsMinimumOneSelected()
        {
            Accordion accordion = DefaultAccordionToTest;
            accordion.SelectionMode = AccordionSelectionMode.One;

            TestAsync(
                accordion,
                () => Assert.AreEqual("item 1", accordion.SelectedItem),
                () => accordion.SelectedItem = "item 2",
                () => accordion.SelectionMode = AccordionSelectionMode.ZeroOrOne,
                () => Assert.AreEqual("item 2", accordion.SelectedItem),
                () => accordion.SelectedItem = null,
                () => Assert.IsNull(accordion.SelectedItem),
                () => accordion.SelectionMode = AccordionSelectionMode.One,
                () => Assert.AreEqual("item 1", accordion.SelectedItem));
        }

        /// <summary>
        /// Tests that IsMinimumOnSelected works when there are no items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that IsMinimumOnSelected works when there are no items.")]
        public virtual void ShouldSupportIsMinimumOneSelectedWithNoItems()
        {
            Accordion accordion = new Accordion();
            accordion.SelectionMode = AccordionSelectionMode.One;

            TestAsync(
                accordion,
                () => Assert.IsNull(accordion.SelectedItem),
                () => accordion.SelectionMode = AccordionSelectionMode.ZeroOrOne,
                () => Assert.IsNull(accordion.SelectedItem),
                () => accordion.SelectionMode = AccordionSelectionMode.One,
                () => Assert.IsNull(accordion.SelectedItem));
        }

        /// <summary>
        /// Tests that we can not unselect the first accordionItem when not allowed.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [ExpectedException(typeof(InvalidOperationException))]
        [Description("Tests that we can not unselect the first accordionItem when not allowed.")]
        public virtual void ShouldThrowExceptionWhenUnselectingItemWhenLocked()
        {
            Accordion accordion = DefaultAccordionToTest;
            accordion.SelectionMode = AccordionSelectionMode.One;

            TestAsync(
                accordion,
                () => GetAccordionItem(accordion, "item 1").IsSelected = false);
        }

        /// <summary>
        /// Tests that SelectedIndex and SelectedItem are synchronized.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that SelectedIndex and SelectedItem are synchronized.")]
        public virtual void ShouldSynchronizeSelectedIndexAndSelectedItem()
        {
            Accordion acc = new Accordion();

            acc.Items.Add("item 0");
            acc.Items.Add("item 1");
            acc.Items.Add("item 2");
            acc.Items.Add("item 3");

            TestAsync(
                acc,
                () => Assert.AreEqual(0, acc.SelectedIndex),
                () => acc.SelectedItem = "item 2",
                () => Assert.AreEqual(2, acc.SelectedIndex),
                () => acc.SelectedIndex = 1,
                () => Assert.AreEqual("item 1", acc.SelectedItem),
                () => acc.SelectedIndex = 10,   // invalid value
                () => Assert.AreEqual(1, acc.SelectedIndex));
        }

        /// <summary>
        /// Tests that accordion deals with setting selecteditem to a non-item.
        /// </summary>
        [TestMethod]
        [Bug("655796, Accordion causes StackOverflowException when setting SelectedItem to a value that is not present", Fixed = true)]
        [Description("Tests that accordion deals with setting selecteditem to a non-item.")]
        public virtual void ShouldHandleSettingSelectedItemToInvalidItem()
        {
            Accordion acc = new Accordion();
            acc.SelectedItem = new object();

            Assert.IsNull(acc.SelectedItem);
            Assert.AreEqual(-1, acc.SelectedIndex);

            // make sure one is selected.
            acc.SelectionMode = AccordionSelectionMode.OneOrMore;
            acc.Items.Add("1");
            acc.Items.Add("2");

            acc.SelectedItem = "2";
            Assert.AreEqual(1, acc.SelectedIndex);
            acc.SelectedItem = new object();
            // should revert to old value
            Assert.AreEqual("2", acc.SelectedItem);
            Assert.AreEqual(1, acc.SelectedIndex);

            acc.SelectedItem = null;
            // should revert to old value
            Assert.AreEqual("2", acc.SelectedItem);
            Assert.AreEqual(1, acc.SelectedIndex);
        }

        /// <summary>
        /// Tests that accordion deals with setting selecteditem to a non-item from itemssource.
        /// </summary>
        [TestMethod]
        [Description("Tests that accordion deals with setting selecteditem to a non-item from itemssource.")]
        public virtual void ShouldHandleSettingSelectedItemToInvalidItemUsingItemsSource()
        {
            Accordion acc = new Accordion();
            acc.ItemsSource = "hello world".Split(' ');

            acc.SelectedItem = "invalid";

            Assert.AreEqual("hello", acc.SelectedItem);
        }

        /// <summary>
        /// Tests that accordion deals with setting selectedindex to an invalid value.
        /// </summary>
        [TestMethod]
        [Bug("655810 Accordion causes StackOverflowException when setting SelectedIndex to a value that is not present", Fixed = true)]
        [Description("Tests that accordion deals with setting selectedindex to an invalid value.")]
        public virtual void ShouldHandleSettingSelectedIndexToInvalidValue()
        {
            Accordion acc = new Accordion();
            acc.SelectedIndex = 0;

            Assert.IsNull(acc.SelectedItem);
            Assert.AreEqual(-1, acc.SelectedIndex);

            // make sure one is selected.
            acc.SelectionMode = AccordionSelectionMode.OneOrMore;
            acc.Items.Add("1");
            acc.Items.Add("2");

            acc.SelectedIndex = 1;
            acc.SelectedIndex = -1;
            // should revert to old value
            Assert.AreEqual("2", acc.SelectedItem);
            Assert.AreEqual(1, acc.SelectedIndex);

            acc.SelectedIndex = 15;
            // should revert to old value
            Assert.AreEqual("2", acc.SelectedItem);
            Assert.AreEqual(1, acc.SelectedIndex);
        }
        #endregion SingleSelection tests

        #region Multi select tests
        /// <summary>
        /// Tests that collectionchanges are allowed during processing.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Ignore]
        [Description("Tests that collectionchanges are allowed during processing.")]
        public virtual void ShouldScheduleCollectionChangesAfterEventFinish()
        {
            // TODO: this test is invalid now we changed logic
            Accordion acc = DefaultAccordionToTest;
            acc.SelectionMode = AccordionSelectionMode.One;

            TestAsync(
                acc,
                () => acc.SelectedItems.Add("item 2"),
                () => Assert.AreEqual(acc.Items[1], acc.SelectedItems[0]),
                () => acc.SelectedIndices.Add(0),
                () => Assert.AreEqual(acc.Items[0], acc.SelectedItems[0]),
                () => Assert.AreEqual(0, acc.SelectedIndices[0]));
        }

        /// <summary>
        /// Tests that accordionItems get selected by the expanded event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that accordionItems get selected by the expanded event.")]
        public virtual void ShouldSelectAccordionItemsWhenIsExpandedChanges()
        {
            Accordion accordion = DefaultAccordionToTest;
            accordion.SelectionMode = AccordionSelectionMode.OneOrMore;

            TestAsync(
                accordion,
                () => Assert.IsNotNull(accordion.SelectedItem),
                () => Assert.AreEqual(1, accordion.SelectedItems.Count),
                () => GetAccordionItem(accordion, "item 2").IsSelected = true,
                () => GetAccordionItem(accordion, "item 3").IsSelected = true,
                () => Assert.AreEqual("item 3", accordion.SelectedItem),
                () => Assert.AreEqual(3, accordion.SelectedItems.Count));
        }

        /// <summary>
        /// Tests that accordionItems get selected by adding them to the selected items collection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that accordionItems get selected by adding them to the selected items collection.")]
        public virtual void ShouldSelectAccordionItemWhenAddedToSelectedCollections()
        {
            Accordion accordion = DefaultAccordionToTest;
            accordion.SelectionMode = AccordionSelectionMode.ZeroOrMore;

            TestAsync(
                accordion,
                () => accordion.SelectedItems.Add("item 2"),
                () => Assert.IsTrue(GetAccordionItem(accordion, "item 2").IsSelected),
                () => accordion.SelectedIndices.Add(2),
                () => Assert.IsTrue(GetAccordionItem(accordion, "item 3").IsSelected),
                () => accordion.SelectedIndices.Remove(1),
                () => Assert.IsFalse(GetAccordionItem(accordion, "item 2").IsSelected),
                () => accordion.SelectedItems.Remove("item 3"),
                () => Assert.IsFalse(GetAccordionItem(accordion, "item 3").IsSelected));
        }

        /// <summary>
        /// Tests that adding items will not break multiselectionmode rules.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Ignore]
        [Description("Tests that adding items will not break multiselectionmode rules.")]
        public virtual void ShouldSelectNewItemWhenAddingItemsThroughSelectedCollections()
        {
            // todo: test is invalid given new logic.

            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.ZeroOrMore;
            acc.Items.Add("0");
            acc.Items.Add("1");
            acc.Items.Add("2");
            acc.Items.Add("3");
            acc.Items.Add("4");

            TestAsync(
                acc,
                () => acc.SelectedItems.Add("1"),
                () => Assert.IsTrue(GetAccordionItem(acc, "1").IsSelected),
                () => acc.SelectedIndices.Add(4),
                () => Assert.IsTrue(GetAccordionItem(acc, "4").IsSelected),
                () => acc.SelectionMode = AccordionSelectionMode.One,
                () => Assert.AreEqual(acc.SelectedItems[0], acc.Items[acc.SelectedIndices[0]]),
                () => acc.SelectedItems.Add("0"),
                () => Assert.IsTrue(GetAccordionItem(acc, "0").IsSelected),
                () => Assert.IsFalse(GetAccordionItem(acc, "1").IsSelected),
                () => acc.SelectedIndices.Add(2),
                () => Assert.IsTrue(GetAccordionItem(acc, "2").IsSelected),
                () => Assert.IsFalse(GetAccordionItem(acc, "0").IsSelected));
        }

        /// <summary>
        /// Tests that IsMaximumOneSelected deselects items when necessary.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that IsMaximumOneSelected deselects items when necessary.")]
        public virtual void ShouldDeselectItemsWhenIsMaximumOneSelectedIsSet()
        {
            Accordion accordion = DefaultAccordionToTest;
            accordion.SelectionMode = AccordionSelectionMode.OneOrMore;

            TestAsync(
                accordion,
                () => GetAccordionItem(accordion, "item 2").IsSelected = true,
                () => GetAccordionItem(accordion, "item 3").IsSelected = true,
                () => GetAccordionItem(accordion, "item 1").IsSelected = false,
                () => Assert.AreEqual(2, accordion.SelectedItems.Count),
                () => accordion.SelectionMode = AccordionSelectionMode.One,
                () => Assert.AreEqual(1, accordion.SelectedItems.Count),
                () => Assert.AreEqual("item 3", accordion.SelectedItem));
        }

        /// <summary>
        /// Tests that the selectedindices follow selecteditems.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that the selectedindices follow selecteditems.")]
        public virtual void ShouldSynchronizeSelectedIndexesWithSelectedItems()
        {
            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.ZeroOrMore;
            acc.Items.Add("0");
            acc.Items.Add("1");
            acc.Items.Add("2");
            acc.Items.Add("3");
            acc.Items.Add("4");
            acc.Items.Add("5");

            TestAsync(
                acc,
                () => Assert.AreEqual(0, acc.SelectedItems.Count),
                () => Assert.AreEqual(0, acc.SelectedIndices.Count()),
                () => acc.SelectedItems.Add("2"),
                () => acc.SelectedItems.Add("5"),
                () => Assert.AreEqual(2, acc.SelectedItems.Count),
                () => Assert.AreEqual(2, acc.SelectedIndices.Count()),
                () => Assert.AreEqual("2", acc.SelectedItems[0]),
                () => Assert.AreEqual("5", acc.SelectedItems[1]),
                () => Assert.AreEqual(2, acc.SelectedIndices[0]),
                () => Assert.AreEqual(5, acc.SelectedIndices[1]),
                () => acc.SelectedItems.Remove("2"),
                () => Assert.AreEqual(1, acc.SelectedItems.Count),
                () => Assert.AreEqual(1, acc.SelectedIndices.Count),
                () => Assert.IsTrue(acc.SelectedIndices[0] == 5));
        }

        /// <summary>
        /// Tests that selecteditems follow selectedindices.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that selecteditems follow selectedindices.")]
        public virtual void ShouldSynchronizeSelectedItemsWithSelectedIndexes()
        {
            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.ZeroOrMore;
            acc.Items.Add("0");
            acc.Items.Add("1");
            acc.Items.Add("2");
            acc.Items.Add("3");
            acc.Items.Add("4");
            acc.Items.Add("5");

            TestAsync(
                acc,
                () => Assert.AreEqual(0, acc.SelectedItems.Count),
                () => Assert.AreEqual(0, acc.SelectedIndices.Count()),
                () => acc.SelectedIndices.Add(1),
                () => acc.SelectedIndices.Add(4),
                () => Assert.AreEqual(2, acc.SelectedItems.Count),
                () => Assert.AreEqual("1", acc.SelectedItems[0]),
                () => Assert.AreEqual("4", acc.SelectedItems[1]));
        }

        /// <summary>
        /// Tests that state is correct after SelectedCollections threw exception.
        /// </summary>
        [TestMethod]
        [Ignore]
        [Description("Tests that state is correct after SelectedCollections threw exception.")]
        public virtual void ShouldKeepIntegrityAfterSelectedCollectionsThrow()
        {
            // ignored because the goal can not be to keep the collection in tact.

            Accordion acc = DefaultAccordionToTest;
            acc.SelectionMode = AccordionSelectionMode.One;
            acc.SelectedItem = "item 1";

            Assert.IsTrue(acc.SelectedItems[0].ToString() == "item 1");

            // adding another item to the collection should change selected item, items and indices
            // since it will change the selecteditems collection as well, this is an invalid 
            // change. The control should throw an exception.

            try
            {
                acc.SelectedItems.Add("item 3");
            }
            catch (InvalidOperationException)
            {
            }

            Thread.Sleep(40);
            Assert.IsTrue(acc.SelectedItem.ToString() == "item 1");
            Assert.IsTrue(acc.SelectedIndex == 0);
            Assert.IsTrue(acc.SelectedItems.Count == 1 && acc.SelectedItems[0].ToString() == "item 1");
            Assert.IsTrue(acc.SelectedIndices.Count == 1 && acc.SelectedIndices.ToString() == "item 1");
        }
        #endregion Multi select tests

        #region same value types selection concerns
        /// <summary>
        /// Creates an accordion with multiple items of the same value ("item").
        /// </summary>
        /// <param name="selectionMode">The selection mode.</param>
        /// <returns>An accordion with 3 items "item".</returns>
        private static Accordion CreateAccordionWithMultipleItems(AccordionSelectionMode selectionMode)
        {
            Accordion acc = new Accordion
                                {
                                        SelectionMode = selectionMode
                                };
            acc.Items.Add("item");
            acc.Items.Add("item");
            acc.Items.Add("item");
            return acc;
        }

        /// <summary>
        /// Tests that a particular container can be selected amidst similar items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that a particular container can be selected amidst similar items.")]
        public virtual void ShouldBeAbleToSelectSpecificContainer()
        {
            Accordion acc = CreateAccordionWithMultipleItems(AccordionSelectionMode.ZeroOrMore);
 
            TestAsync(
                    acc,
                    () => ((AccordionItem)acc.ItemContainerGenerator.ContainerFromIndex(2)).IsSelected = true,
                    () => Assert.IsFalse(((AccordionItem)acc.ItemContainerGenerator.ContainerFromIndex(0)).IsSelected),
                    () => Assert.IsFalse(((AccordionItem)acc.ItemContainerGenerator.ContainerFromIndex(1)).IsSelected),
                    () => Assert.IsTrue(((AccordionItem)acc.ItemContainerGenerator.ContainerFromIndex(2)).IsSelected));
        }

        /// <summary>
        /// Tests selecting and deselecting items without containers.
        /// </summary>
        [TestMethod]
        [Description("Tests selecting and deselecting items without containers.")]
        public virtual void BreadthTestSelectionOfSimilarItems()
        {
            Accordion acc = CreateAccordionWithMultipleItems(AccordionSelectionMode.ZeroOrMore);

            acc.SelectedItem = "item";
            Assert.IsTrue(acc.SelectedIndex == 0);
            acc.SelectedItem = null;
            Assert.IsTrue(acc.SelectedIndex == -1);
            Assert.IsTrue(acc.SelectedItems.Count == 0 && acc.SelectedIndices.Count == 0);
            
            acc.SelectedIndex = 1;
            Assert.IsTrue(acc.SelectedItem.ToString() == "item");
            Assert.IsTrue(acc.SelectedIndex == 1);

            acc.SelectedIndices.Add(2);
            Assert.IsTrue(acc.SelectedItems.Count == 1);

            acc.SelectedIndices.Remove(1);
            Assert.IsTrue(acc.SelectedItems.Count == 1);
            Assert.IsTrue(acc.SelectedIndex == 2);

            acc.SelectedIndices.Add(1);
            Assert.IsTrue(acc.SelectedIndices.Count == 2);
            Assert.IsTrue(acc.SelectedIndex == 1);

            acc.SelectedIndices.Remove(1);
            Assert.IsTrue(acc.SelectedIndex == -1);
            Assert.IsTrue(acc.SelectedIndices.Count == 1);
        }

        /// <summary>
        /// Tests selecting in One mode with same items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests selecting in One mode with same items.")]
        public virtual void ShouldKeepSelectionModeIntactWithSameItems()
        {
            Accordion acc = CreateAccordionWithMultipleItems(AccordionSelectionMode.One);

            TestAsync(
                acc,
                () => GetAccordionItem(acc, 1).IsSelected = true,
                () => GetAccordionItem(acc, 2).IsSelected = true,
                () => Assert.IsTrue(acc.SelectedItems.Count == 1),
                () => Assert.IsTrue(acc.SelectedIndices.Count == 1),
                () => Assert.IsTrue(acc.SelectedIndex == 2),
                () => Assert.IsFalse(GetAccordionItem(acc, 1).IsSelected));
        }

        /// <summary>
        /// Tests that items are unselected when switching modes.
        /// </summary>
        [TestMethod]
        [Description("Tests that items are unselected when switching modes.")]
        public virtual void ShouldDeselectWhenChangingModes()
        {
            Accordion acc = CreateAccordionWithMultipleItems(AccordionSelectionMode.ZeroOrMore);

            acc.SelectedIndices.Add(0);
            acc.SelectedIndices.Add(1);
            acc.SelectedIndices.Add(2);

            Assert.IsTrue(acc.SelectedIndices.Count == 3);

            acc.SelectionMode = AccordionSelectionMode.One;

            Assert.IsTrue(acc.SelectedIndices.Count == 1);
        }
        
        /// <summary>
        /// Tests removing an index of the same item.
        /// </summary>
        [TestMethod]
        [Description("Tests removing an index of the same item.")]
        public virtual void ShouldBeAbleToRemoveIndexWithTheSameItemInCollection()
        {
            Accordion acc = CreateAccordionWithMultipleItems(AccordionSelectionMode.ZeroOrMore);
            acc.Items.Add("different item");

            acc.SelectedIndices.Add(1);
            acc.SelectedIndices.Add(2);

            acc.SelectedItem = "different item";
            Assert.IsTrue(acc.SelectedItem.ToString() == "different item");
            Assert.IsTrue(acc.SelectedItems.Contains("item"));

            acc.SelectedIndices.Remove(1);
            Assert.IsTrue(acc.SelectedItems.Contains("item"));
            Assert.IsTrue(acc.SelectedIndices.Count == 2);
        }

        /// <summary>
        /// Tests removing an item that indexed multiple times.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "Follows naming in Accordion.")]
        [TestMethod]
        [Description("Tests removing an item that indexed multiple times.")]
        public virtual void ShouldBeAbleToRemoveAnItemWithTheMultipleIndicesInCollection()
        {
            Accordion acc = CreateAccordionWithMultipleItems(AccordionSelectionMode.ZeroOrMore);
            acc.SelectedIndices.Add(1);
            acc.SelectedIndices.Add(2);
            acc.Items.Add("different item");

            acc.SelectedItem = "different item";

            acc.SelectedItems.Remove("item");
            Assert.IsTrue(acc.SelectedItem.ToString() == "different item");
            Assert.IsTrue(acc.SelectedIndices.Count == 1 && acc.SelectedIndices[0] == 3);
        }
        #endregion

        #region General selection concerns

        /// <summary>
        /// Tests that the first item is selected when adding items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that the first item is selected when adding items.")]
        public virtual void ShouldSelectFirstItemWhenAddingItems()
        {
            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.One;

            TestAsync(
                acc,
                () => acc.Items.Add("item 1"),
                () => acc.Items.Add("item 2"),
                () => acc.Items.Add("item 3"),
                () => Assert.IsTrue(GetAccordionItem(acc, "item 1").IsSelected),
                () => Assert.AreEqual(1, acc.SelectedItems.Count),
                () => Assert.AreEqual("item 1", acc.SelectedItems[0]),
                () => Assert.IsTrue(acc.SelectedItem.ToString() == "item 1"));
        }

        /// <summary>
        /// Tests that the first item is selected when setting an itemssource.
        /// </summary>
        [TestMethod]
        [Description("Tests that the first item is selected when setting an itemssource.")]
        public virtual void ShouldSelectFirstItemWhenSettingItemsSource()
        {
            Accordion acc = new Accordion();
            acc.ItemsSource = "hello world".Split(' ');

            Assert.IsTrue(acc.SelectedItem.Equals("hello"));
            Assert.IsTrue(acc.SelectedIndices.Count == 1);
            Assert.IsTrue(acc.SelectedItems.Count == 1);
        }

        /// <summary>
        /// Tests that no item is selected when adding items and SelectionMode allows zero.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that no item is selected when adding items and SelectionMode allows zero.")]
        public virtual void ShouldNotAutomaticallySelectItemWhenZeroAllowed()
        {
            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.ZeroOrOne;

            TestAsync(
                acc,
                () => acc.Items.Add("item 1"),
                () => acc.Items.Add("item 2"),
                () => Assert.AreEqual(0, acc.SelectedItems.Count),
                () => Assert.IsNull(acc.SelectedItem));
        }

        /// <summary>
        /// Tests that SelectedItems can be cleared.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Bug("655812 Accordion throws NotSupportedException Unsupported collection action 'Reset'. for common user scenario of clearing SelectedItems", Fixed = true)]
        [Description("Tests that SelectedItems can be cleared.")]
        public virtual void ShouldHandleClearingSelectedItems()
        {
            Accordion acc = new Accordion();

            acc.SelectedItems.Clear();

            TestAsync(
                    acc,
                    () => acc.SelectedItems.Clear(),
                    () => acc.Items.Add("1"),
                    () => acc.Items.Add("2"),
                    () => acc.Items.Add("3"),
                    () => acc.SelectionMode = AccordionSelectionMode.OneOrMore, // will force select first item
                    () => acc.SelectedItems.Add("2"),
                    () => acc.SelectedItems.Add("3"),
                    () => Assert.AreEqual(3, acc.SelectedIndices.Count),
                    () => acc.SelectionMode = AccordionSelectionMode.ZeroOrMore, // will allow us to clear
                    () => acc.SelectedItems.Clear(),
                    () => Assert.IsTrue(acc.SelectedItems.Count == 0),
                    () => Assert.IsTrue(acc.SelectedIndices.Count == 0),
                    () => Assert.IsTrue(acc.SelectedItem == null));
        }

        /// <summary>
        /// Tests that SelectedIndices can be cleared.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "As decided on.")]
        [TestMethod]
        [Asynchronous]
        [Bug("655812 Accordion throws NotSupportedException Unsupported collection action 'Reset'. for common user scenario of clearing SelectedItems", Fixed = true)]
        [Description("Tests that SelectedIndices can be cleared.")]
        public virtual void ShouldHandleClearingSelectedIndices()
        {
            Accordion acc = new Accordion();

            TestAsync(
                acc,
                () => acc.SelectedIndices.Clear(),
                    () => acc.Items.Add("1"),
                    () => acc.Items.Add("2"),
                    () => acc.Items.Add("3"),
                    () => acc.SelectionMode = AccordionSelectionMode.OneOrMore, // will force select first item
                    () => acc.SelectedIndices.Add(1),
                    () => acc.SelectedIndices.Add(2),
                    () => Assert.AreEqual(3, acc.SelectedIndices.Count),
                    () => acc.SelectionMode = AccordionSelectionMode.ZeroOrMore, // will allow us to clear
                    () => acc.SelectedIndices.Clear(),
                    () => Assert.IsTrue(acc.SelectedItems.Count == 0),
                    () => Assert.IsTrue(acc.SelectedIndices.Count == 0),
                    () => Assert.IsTrue(acc.SelectedItem == null));
        }

        /// <summary>
        /// Tests that Items can be cleared.
        /// </summary>
        [TestMethod]
        [Description("Tests that Items can be cleared.")]
        public virtual void ShouldHandleClearingItems()
        {
            Accordion acc = DefaultAccordionToTest;

            acc.SelectedItem = "item 3";

            Assert.IsTrue(acc.SelectedIndex > -1);

            acc.Items.Clear();

            Assert.IsTrue(acc.SelectedIndex == -1);
            Assert.IsTrue(acc.SelectedItem == null);
        }

        /// <summary>
        /// Tests that SelectedItems property can be cleared.
        /// </summary>
        /// <remarks>Does not use the ExpectedException attribute because I want
        /// to test the revert logic.</remarks>
        [TestMethod]
        [Bug("655813 Accordion causes StackOverflowException when attempting to change SelectedItems property", Fixed = true)]
        [Description("Tests that SelectedItems property can be cleared.")]
        public virtual void ShouldHandleClearingTheSelectedItemsProperty()
        {
            Accordion a = new Accordion();
            IList original = a.SelectedItems;
            bool exceptionThrown = false;

            try
            {
                a.SetValue(Accordion.SelectedItemsProperty, null);
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
            Assert.IsTrue(a.SelectedItems.Count == 0);
            Assert.IsTrue(original == a.SelectedItems);

            // reset test
            exceptionThrown = false;
            try
            {
                a.ClearValue(Accordion.SelectedItemsProperty);
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
            Assert.IsTrue(a.SelectedItems.Count == 0);
            Assert.IsTrue(original == a.SelectedItems);
        }

        /// <summary>
        /// Tests that SelectedIndices property can be cleared.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "As decided on.")]
        [TestMethod]
        [Description("Tests that SelectedIndices property can be cleared.")]
        public virtual void ShouldHandleClearingTheSelectedIndicesProperty()
        {
            Accordion a = new Accordion();
            IList<int> original = a.SelectedIndices;
            bool exceptionThrown = false;

            try
            {
                a.SetValue(Accordion.SelectedIndicesProperty, null);
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
            Assert.IsTrue(a.SelectedIndices.Count == 0);
            Assert.IsTrue(original == a.SelectedIndices);

            // reset test
            exceptionThrown = false;
            try
            {
                a.ClearValue(Accordion.SelectedIndicesProperty);
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
            Assert.IsTrue(a.SelectedIndices.Count == 0);
            Assert.IsTrue(original == a.SelectedIndices);
        }

        /// <summary>
        /// Tests that clearing the items collection also clears selecteditems.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that clearing the items collection also clears selecteditems.")]
        public virtual void ShouldClearSelectedItemsWhenClearingItems()
        {
            Accordion acc = DefaultAccordionToTest;
            acc.SelectionMode = AccordionSelectionMode.ZeroOrMore;

            acc.SelectedItems.Add("item 3");

            TestAsync(
                acc,
                () => acc.Items.Clear(),
                () => Assert.IsTrue(acc.SelectedItems.Count == 0),
                () => Assert.IsTrue(acc.SelectedIndices.Count == 0));
        }

        /// <summary>
        /// Tests that clearing the items collection in a minimum selection mode also clears selecteditems.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that clearing the items collection in a minimum selection mode also clears selecteditems.")]
        public virtual void ShouldClearSelectedItemsWhenClearingItemsInMinimumOneSelectedMode()
        {
            Accordion acc = DefaultAccordionToTest;
            acc.SelectionMode = AccordionSelectionMode.OneOrMore;

            acc.SelectedItems.Add("item 3");

            TestAsync(
                acc,
                () => acc.Items.Clear(),
                () => Assert.IsTrue(acc.SelectedItems.Count == 0),
                () => Assert.IsTrue(acc.SelectedIndices.Count == 0));
        }

        /// <summary>
        /// Tests that removing an item will also remove it from the Selected collections.
        /// </summary>
        [TestMethod]
        [Description("Tests that removing an item will also remove it from the Selected collections.")]
        public virtual void ShouldDeselectPriorToRemoving()
        {
            Accordion acc = DefaultAccordionToTest;
            // item 1 was preselected by the selectionmode

            acc.SelectionMode = AccordionSelectionMode.ZeroOrMore;

            acc.SelectedItems.Add("item 3");
            Assert.IsTrue(acc.SelectedIndices.Count == 2);
            acc.Items.Remove("item 3");
            Assert.IsFalse(acc.SelectedItems.Contains("item 3"));
            Assert.IsTrue(acc.SelectedIndices.Count == 1);
            Assert.IsFalse(acc.SelectedIndices.Contains(2));
        }

        /// <summary>
        /// Tests that items can be removed from selected items collection.
        /// </summary>
        [TestMethod]
        [Description("Tests that items can be removed from selected items collection.")]
        public virtual void ShouldBeAbleToRemoveItemsFromSelectedItems()
        {
            Accordion acc = DefaultAccordionToTest;
            // item 1 was preselected by the selectionmode

            acc.SelectionMode = AccordionSelectionMode.ZeroOrMore;

            acc.SelectedItems.Add("item 3");
            Assert.IsTrue(acc.SelectedIndices.Count == 2);
            acc.SelectedItems.Remove("item 3");
            Assert.IsFalse(acc.SelectedItems.Contains("item 3"));
            Assert.IsTrue(acc.SelectedIndices.Count == 1);
            Assert.IsFalse(acc.SelectedIndices.Contains(2));
        }

        /// <summary>
        /// Tests that removing an item will keep SelectedIndices in order.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that removing an item will keep SelectedIndices in order.")]
        public virtual void ShouldDeselectPriorToRemovingEvenInMinimumOneMode()
        {
            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.ZeroOrMore;
            acc.Items.Add("0");
            acc.Items.Add("1");
            acc.Items.Add("2");
            acc.Items.Add("3");
            acc.Items.Add("4");

            TestAsync(
                acc,
                () => acc.SelectedItems.Add("2"),
                () => acc.SelectedItems.Add("4"),
                () => acc.SelectionMode = AccordionSelectionMode.OneOrMore,
                () => Assert.IsTrue(acc.SelectedIndices.Count == 2),
                () => acc.Items.Remove("2"),
                () => Assert.IsFalse(acc.SelectedItems.Contains("2")),
                () => Assert.IsTrue(acc.SelectedIndices.Count == 1),
                () => Assert.IsTrue(acc.SelectedIndices[0] == 3),
                () => Assert.IsTrue(acc.SelectedIndex == 3),
                () => acc.Items.Remove("4"),
                () => Assert.IsTrue(acc.SelectedIndex == 0),
                () => Assert.IsTrue(acc.SelectedIndices.Count == 1 && acc.SelectedIndices[0] == 0));
        }

        /// <summary>
        /// Tests removing an item from the SelectedItems throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Description("Tests removing an item from the SelectedItems throws exception.")]
        public virtual void ShouldThrowExceptionWhenRemovingItemFromSelectedItems()
        {
            Accordion acc = DefaultAccordionToTest;
            acc.SelectionMode = AccordionSelectionMode.One;

            acc.SelectedItems.Remove("item 1");
        }

        /// <summary>
        /// Tests removing an item from the SelectedIndices throws exception.
        /// </summary>
        [TestMethod]
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "As decided on.")]
        [ExpectedException(typeof(InvalidOperationException))]
        [Description("Tests removing an item from the SelectedIndices throws exception.")]
        public virtual void ShouldThrowExceptionWhenRemovingItemFromSelectedIndices()
        {
            Accordion acc = DefaultAccordionToTest;
            acc.SelectionMode = AccordionSelectionMode.One;

            acc.SelectedIndices.Remove(0);
        }

        /// <summary>
        /// Tests clearing SelectedItems throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Description("Tests clearing SelectedItems throws exception.")]
        public virtual void ShouldThrowExceptionWhenClearingSelectedItems()
        {
            Accordion acc = DefaultAccordionToTest;
            acc.SelectionMode = AccordionSelectionMode.One;

            acc.SelectedItems.Clear();
        }

        /// <summary>
        /// Tests clearing SelectedIndices throws exception.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "As decided on.")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Description("Tests clearing SelectedIndices throws exception.")]
        public virtual void ShouldThrowExceptionWhenClearingSelectedIndices()
        {
            Accordion acc = DefaultAccordionToTest;
            acc.SelectionMode = AccordionSelectionMode.One;

            acc.SelectedIndices.Clear();
        }

        /// <summary>
        /// Tests SelectedIndices when changing selecteditem.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "As decided on.")]
        [TestMethod]
        [Description("Tests SelectedIndices when changing selecteditem.")]
        public virtual void ShouldMaintainCorrectSelectedIndices()
        {
            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.ZeroOrMore;
            acc.Items.Add("0");
            acc.Items.Add("1");
            acc.Items.Add("2");

            acc.SelectedIndices.Add(0);
            acc.SelectedIndices.Add(1);

            acc.Items.Insert(0, "before 0");
            Assert.IsTrue(acc.SelectedIndices.Count == 2);
            Assert.IsTrue(acc.SelectedIndices[0] == 1);
            Assert.IsTrue(acc.SelectedIndices[1] == 2);

            acc.Items.Insert(2, "after 0");
            Assert.IsTrue(acc.SelectedIndices.Count == 2);
            Assert.IsTrue(acc.SelectedIndices[0] == 1);
            Assert.IsTrue(acc.SelectedIndices[1] == 3);

            acc.Items.Remove("after 0");
            Assert.IsTrue(acc.SelectedIndices.Count == 2);
            Assert.IsTrue(acc.SelectedIndices[0] == 1);
            Assert.IsTrue(acc.SelectedIndices[1] == 2);

            acc.Items.RemoveAt(0);
            Assert.IsTrue(acc.SelectedIndices.Count == 2);
            Assert.IsTrue(acc.SelectedIndices[0] == 0);
            Assert.IsTrue(acc.SelectedIndices[1] == 1);

            acc.Items.RemoveAt(0);
            Assert.IsTrue(acc.SelectedIndices.Count == 1);
            Assert.IsTrue(acc.SelectedIndices[0] == 0);
            
            Assert.IsTrue(acc.SelectedItem.ToString() == "1");
        }

        /// <summary>
        /// Tests that SelectedIndex gets synchronized during Items adding and removing.
        /// </summary>
        [TestMethod]
        [Description("Tests that SelectedIndex gets synchronized during Items adding and removing.")]
        public virtual void ShouldSynchronizeSelectedIndexDuringItemsManipulation()
        {
            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.ZeroOrMore;
            acc.Items.Add("0");
            acc.Items.Add("1");
            acc.Items.Add("2");
            acc.Items.Add("3");
            acc.Items.Add("4");
            acc.Items.Add("5");

            acc.SelectedItem = "2";
            Assert.IsTrue(acc.SelectedIndex == 2);

            acc.Items.Insert(0, "first");
            Assert.IsTrue(acc.SelectedIndex == 3);

            acc.Items.RemoveAt(1);
            Assert.IsTrue(acc.SelectedIndex == 2);

            acc.Items.Clear();
            Assert.IsTrue(acc.SelectedIndex == -1);
        }

        /// <summary>
        /// Tests items with the same value.
        /// </summary>
        [TestMethod]
        [Description("Tests items with the same value.")]
        public virtual void ShouldAllowWorkingWithTheSameObjects()
        {
            // the string has several of the same letters.
            string test = "Hello World";
            Accordion acc = new Accordion()
                                {
                                    ItemsSource = test.ToCharArray()
                                };

            acc.SelectedItem = 'l';
        }

        /// <summary>
        /// Tests collections in a randomized way.
        /// </summary>
        [TestMethod]
        [Description("Tests collections in a randomized way.")]
        public virtual void RandomizedCollectionIntegrityTest()
        {
            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.ZeroOrMore;
            acc.Items.Add("same item");
            acc.Items.Add("same item");
            acc.Items.Add("same item");
            acc.Items.Add("same item");
            acc.Items.Add("same item");
            acc.Items.Add("different item");
            acc.Items.Add("different item2");
            acc.Items.Add("different item3");
            acc.Items.Add(new AccordionItem());
            acc.Items.Add(new AccordionItem());
            acc.Items.Add(new AccordionItem());
            acc.Items.Add(new AccordionItem());
            acc.Items.Add(new AccordionItem());

            Random r = new Random(121212);
            List<Action> actions = new List<Action>();
            actions.Add(() =>
                            {
                                int index = r.Next(acc.Items.Count);
                                if (!acc.SelectedIndices.Contains(index))
                                {
                                    acc.SelectedIndices.Add(index);
                                }
                            });
            actions.Add(() =>
                            {
                                if (acc.SelectedIndices.Count > 0)
                                {
                                    int index = acc.SelectedIndices[r.Next(acc.SelectedIndices.Count)];
                                    acc.SelectedIndices.Remove(index);
                                }
                            });
            actions.Add(() =>
                            {
                                object item = acc.Items[r.Next(acc.Items.Count)];
                                if (!acc.SelectedItems.Contains(item))
                                {
                                    acc.SelectedItems.Add(item);
                                }
                            });
            actions.Add(() =>
                            {
                                if (acc.SelectedItems.Count > 0)
                                {
                                    acc.SelectedItems.Remove(acc.SelectedItems[r.Next(acc.SelectedItems.Count)]);
                                }
                            });
            actions.Add(() => acc.SelectedItem = acc.Items[r.Next(acc.Items.Count)]);
            actions.Add(() => acc.SelectedIndex = r.Next(acc.Items.Count));

            for (int i = 0; i < 5000; i++)
            {
                // the below writeline makes it easy to set a breakpoint at a 
                // specific iteration
                Diagnostics.Debug.WriteLine(i);

                actions[r.Next(actions.Count)].Invoke();
                Assert.IsTrue((acc.SelectedIndex == -1 && acc.SelectedItem == null) || acc.Items[acc.SelectedIndex].Equals(acc.SelectedItem));
                foreach (int selectedIndex in acc.SelectedIndices)
                {
                    Assert.IsTrue(acc.SelectedItems.Contains(acc.Items[selectedIndex]));
                }
            }
        }

        /// <summary>
        /// Tests that a selectedindex can be removed when it is the last in the collection.
        /// </summary>
        [TestMethod]
        [Description("Tests that a selectedindex can be removed when it is the last in the collection.")]
        public virtual void ShouldBeAbleToRemoveIndexWithOnlyOneInCollection()
        {
            Accordion acc = CreateAccordionWithMultipleItems(AccordionSelectionMode.ZeroOrMore);
            acc.SelectedIndex = 1;
            acc.SelectedIndex = -1;
            acc.SelectedIndex = 2;
            Assert.IsTrue(acc.SelectedIndices.Count == 1);
            acc.SelectedIndices.Remove(2);
        }
        #endregion

        #region Layout tests
        /// <summary>
        /// Tests that height and width are spaced correctly.
        /// </summary>
        [Asynchronous]
        [Description("Tests that height and width are spaced correctly.")]
        public virtual void ShouldDivideSpaceBetweenExpandedExpenders()
        {
            Accordion accordion = DefaultAccordionToTest;
            accordion.Height = 500;
            accordion.SelectionMode = AccordionSelectionMode.ZeroOrMore;
            accordion.SelectedItem = null;

            ScrollViewer sv = null;

            TestAsync(
                accordion,
                () => sv = (ScrollViewer)((Grid)VisualTreeHelper.GetChild(accordion, 0)).FindName("ScrollViewer"),
                () => GetAccordionItem(accordion, "item 1").IsSelected = true,
                () => GetAccordionItem(accordion, "item 2").IsSelected = true,
                () => EnqueueVisualDelay(2000),
                () => Assert.IsTrue(GetAccordionItem(accordion, "item 1").ActualHeight == GetAccordionItem(accordion, "item 2").ActualHeight),
                () => Assert.IsTrue(GetAccordionItem(accordion, "item 1").ActualHeight > 100));
        }

        /// <summary>
        /// Tests that items are resized when accordion is resized.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Test is to be rewritten and is ignored at the moment.")]
        [TestMethod]
        [Asynchronous]
        [Ignore]
        [Description("Tests that items are resized when accordion is resized.")]
        public virtual void ShouldResizeItemsWhenAccordionIsResized()
        {
            // todo: This test should be rewritten once RX comes online.
            // the below is too fragile, so I do not want to enable this test at this point.

            Accordion acc = new Accordion();
            AccordionItem item1 = new AccordionItem();
            AccordionItem item2 = new AccordionItem();
            acc.Height = 200;
            acc.SelectionMode = AccordionSelectionMode.ZeroOrMore;
            acc.Items.Add(item1);
            acc.Items.Add(item2);

            item1.IsSelected = true;
            item2.IsSelected = true;

            TestAsync(
                acc,
                () => Thread.Sleep(10),  // make sure the expand animation has finished
                () => Thread.Sleep(10),
                () => Thread.Sleep(10),
                () => Assert.IsTrue(item1.ActualHeight > 30 && item1.ActualHeight < 105),
                () => acc.Height = 400,
                () => Thread.Sleep(10),  // make sure the expand animation has finished
                () => Thread.Sleep(10),
                () => Thread.Sleep(10),
                () => Thread.Sleep(10),
                () => Thread.Sleep(10),
                () => Assert.IsTrue(item1.ActualHeight > 105 && item1.ActualHeight < 220),
                () => acc.Height = 200,
                () => Thread.Sleep(10),  // make sure the expand animation has finished
                () => Thread.Sleep(10),
                () => Thread.Sleep(10),
                () => Thread.Sleep(10),
                () => Thread.Sleep(10),
                () => Thread.Sleep(10),
                () => Thread.Sleep(10),
                () => Thread.Sleep(10),
                () => Thread.Sleep(10),
                () => Assert.IsTrue(item1.ActualHeight > 30 && item1.ActualHeight < 105));
        }
        #endregion Layout tests

        #region Template tests
        /// <summary>
        /// Tests that a header and contenttemplate is used for generated items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that a header and contenttemplate is used for generated items.")]
        public virtual void ShouldApplyTemplatesWhenGeneratingItems()
        {
            Accordion acc = new Accordion();
            DataTemplate itemTemplate = new XamlBuilder<DataTemplate> { Name = "itemtemplate" } .Load();
            DataTemplate contentTemplate = new XamlBuilder<DataTemplate> { Name = "contenttemplate" } .Load();

            acc.ItemTemplate = itemTemplate;
            acc.ContentTemplate = contentTemplate;

            acc.Items.Add("item 1");

            TestAsync(
                acc,
                () => Assert.AreEqual(itemTemplate, GetAccordionItem(acc, "item 1").HeaderTemplate),
                () => Assert.AreEqual(contentTemplate, GetAccordionItem(acc, "item 1").ContentTemplate));
        }

        /// <summary>
        /// Tests that a header and contenttemplate are ignored for accordion items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that a header and contenttemplate are ignored for accordion items.")]
        public virtual void ShouldIgnoreTemplatesSpecificallySet()
        {
            Accordion acc = new Accordion();
            DataTemplate itemTemplate = new XamlBuilder<DataTemplate> { Name = "itemtemplate" } .Load();
            DataTemplate contentTemplate = new XamlBuilder<DataTemplate> { Name = "contenttemplate" } .Load();

            acc.ItemTemplate = itemTemplate;
            acc.ContentTemplate = contentTemplate;

            DataTemplate customHeaderTemplate = new XamlBuilder<DataTemplate> { Name = "customheadertemplate" } .Load();
            DataTemplate customContentTemplate = new XamlBuilder<DataTemplate> { Name = "customcontenttemplate" } .Load();

            AccordionItem withTemplates = new AccordionItem()
            {
                HeaderTemplate = customHeaderTemplate,
                ContentTemplate = customContentTemplate
            };
            AccordionItem withoutTemplates = new AccordionItem();

            TestAsync(
                acc,
                () => acc.Items.Add(withTemplates),
                () => acc.Items.Add(withoutTemplates),
                () => Assert.AreNotEqual(itemTemplate, ((AccordionItem)acc.Items[0]).HeaderTemplate),
                () => Assert.AreNotEqual(contentTemplate, ((AccordionItem)acc.Items[0]).ContentTemplate),
                () => Assert.AreEqual(itemTemplate, ((AccordionItem)acc.Items[1]).HeaderTemplate),
                () => Assert.AreEqual(contentTemplate, ((AccordionItem)acc.Items[1]).ContentTemplate),
                () => Assert.AreEqual(customHeaderTemplate, ((AccordionItem)acc.Items[0]).HeaderTemplate),
                () => Assert.AreEqual(customContentTemplate, ((AccordionItem)acc.Items[0]).ContentTemplate));
        }

        /// <summary>
        /// Tests displaymemberbinding used for both content and headertemplate.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests displaymemberbinding used for both content and headertemplate.")]
        public virtual void ShouldUseDisplayMemberBindingForBothContentAndHeader()
        {
            Person a = new Person() { FirstName = "aFirst", LastName = "aLast" };

            Accordion acc = new Accordion();
            acc.DisplayMemberPath = "LastName";

            AccordionItem item = null;
            TestAsync(
                acc,
                () => acc.Items.Add(a),
                () => item = GetAccordionItem(acc, a),
                () => Assert.IsNotNull(item.HeaderTemplate),
                () => Assert.AreSame(item.HeaderTemplate, item.ContentTemplate));
        }

        /// <summary>
        /// Tests that setting templates takes precedence over displaymemberpath.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that setting templates takes precedence over displaymemberpath.")]
        public virtual void ShouldOverrideDisplayMemberPathIfPossible()
        {
            Person a = new Person() { FirstName = "aFirst", LastName = "aLast" };

            Accordion acc = new Accordion();
            acc.DisplayMemberPath = "LastName";
            DataTemplate contentTemplate = new XamlBuilder<DataTemplate> { Name = "contenttemplate" } .Load();

            acc.ContentTemplate = contentTemplate;

            AccordionItem item = null;
            TestAsync(
                acc,
                () => acc.Items.Add(a),
                () => item = GetAccordionItem(acc, a),
                () => Assert.AreEqual(contentTemplate, item.ContentTemplate));
        }
        #endregion Template tests

        #region Binding tests
        /// <summary>
        /// Tests correct flowing of datacontext in manually added items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests correct flowing of datacontext in manually added items.")]
        public virtual void ShouldSetCorrectDataContextWhenManuallyAddingItems()
        {
            Accordion acc = new Accordion();
            acc.Items.Add("item 1");
            acc.Items.Add("item 2");
            acc.Items.Add("item 3");

            AccordionItem generated = null;

            TestAsync(
                acc,
                () => generated = (AccordionItem)acc.ItemContainerGenerator.ContainerFromIndex(1),
                () => Assert.IsTrue(generated.Header.ToString() == "item 2"));
        }

        /// <summary>
        /// Tests correct flowing of datacontext when binding to list.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests correct flowing of datacontext when binding to list.")]
        public virtual void ShouldSetCorrectDataContextWhenBinding()
        {
            Accordion acc = new Accordion();

            List<string> MyCollection = new List<string>();
            MyCollection.Add("AAAA");
            MyCollection.Add("BBBB");
            MyCollection.Add("CCCC");

            acc.DataContext = MyCollection;
            acc.SetBinding(Accordion.ItemsSourceProperty, new Binding());
            
            AccordionItem generated = null;

            TestAsync(
                    acc,
                    () => generated = (AccordionItem) acc.ItemContainerGenerator.ContainerFromIndex(1),
                    () => Assert.IsTrue(generated.Header.ToString() == "BBBB"));
        }

        /// <summary>
        /// Tests correct flowing of datacontext when setting ItemsSource.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests correct flowing of datacontext when setting ItemsSource.")]
        public virtual void ShouldSetCorrectDataContextWhenSettingItemsSource()
        {
            Accordion acc = new Accordion();

            List<string> MyCollection = new List<string>();
            MyCollection.Add("AAAA");
            MyCollection.Add("BBBB");
            MyCollection.Add("CCCC");

            acc.ItemsSource = MyCollection;
            
            AccordionItem generated = null;

            TestAsync(
                    acc,
                    () => generated = (AccordionItem)acc.ItemContainerGenerator.ContainerFromIndex(1),
                    () => Assert.IsTrue(generated.Header.ToString() == "BBBB"));
        }
        #endregion

        #region helper methods
        /// <summary>
        /// Gets the accordion item.
        /// </summary>
        /// <param name="accordion">The accordion.</param>
        /// <param name="item">The item that is wrapped by an AccordionItem.</param>
        /// <returns>The accordionItem that wraps the item.</returns>
        private static AccordionItem GetAccordionItem(Accordion accordion, object item)
        {
            return accordion.ItemContainerGenerator.ContainerFromItem(item) as AccordionItem;
        }

        /// <summary>
        /// Gets the accordion item belonging to an index.
        /// </summary>
        /// <param name="accordion">The accordion.</param>
        /// <param name="index">The index.</param>
        /// <returns>The accordionItem that wraps the item.</returns>
        private static AccordionItem GetAccordionItem(Accordion accordion, int index)
        {
            return accordion.ItemContainerGenerator.ContainerFromIndex(index) as AccordionItem;
        }
        #endregion helper methods
    }
}
