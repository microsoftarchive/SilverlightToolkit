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

        /// <summary>
        /// Gets the HeaderTemplate dependency property test.
        /// </summary>
        protected DependencyPropertyTest<Accordion, DataTemplate> HeaderTemplateProperty { get; private set; }

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
            HeaderTemplateProperty = new DependencyPropertyTest<Accordion, DataTemplate>(this, "HeaderTemplate")
                                       {
                                           Property = Accordion.HeaderTemplateProperty,
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

            // HeaderTemplate tests
            tests.Add(HeaderTemplateProperty.CheckDefaultValueTest);
            tests.Add(HeaderTemplateProperty.ChangeClrSetterTest);
            tests.Add(HeaderTemplateProperty.ChangeSetValueTest);
            tests.Add(HeaderTemplateProperty.SetNullTest);
            tests.Add(HeaderTemplateProperty.ClearValueResetsDefaultTest);
            tests.Add(HeaderTemplateProperty.CanBeStyledTest);
            tests.Add(HeaderTemplateProperty.TemplateBindTest);

            return tests;
        }

        #region Control contract
        /// <summary>
        /// Verify the control's style typed properties.
        /// </summary>
        public override void StyleTypedPropertiesAreDefined()
        {
            Assert.AreEqual(1, DefaultAccordionToTest.GetType().GetStyleTypedProperties().Count);
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
                        // TODO: IItemContainerGenerator work
                        ////Assert.IsNotNull(accordion.ItemContainerGenerator.ContainerFromItem(item) as AccordionItem);
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
                        // TODO: IItemContainerGenerator work
                        ////AccordionItem accordionItem = (AccordionItem)accordion.ItemContainerGenerator.ContainerFromItem(item);
                        ////Assert.AreEqual(ExpandDirection.Left, accordionItem.ExpandDirection);
                    }
                },
                () => accordion.Items.Add("new item"),
                // TODO: IItemContainerGenerator work
                ////() => Assert.AreEqual(ExpandDirection.Left, ((AccordionItem)accordion.ItemContainerGenerator.ContainerFromItem("new item")).ExpandDirection),
                () => accordion.ExpandDirection = ExpandDirection.Right,
                () =>
                {
                    foreach (object item in accordion.Items)
                    {
                        // TODO: IItemContainerGenerator work
                        ////AccordionItem accordionItem = (AccordionItem)accordion.ItemContainerGenerator.ContainerFromItem(item);
                        ////Assert.AreEqual(ExpandDirection.Right, accordionItem.ExpandDirection);
                    }
                });
        }

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
                () => Assert.AreEqual(1, acc.SelectedItems.Count),
                () => Assert.AreEqual("item 1", acc.SelectedItems[0]),
                () => Assert.IsTrue(acc.SelectedItem.ToString() == "item 1"));
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
        [Ignore]
        [Asynchronous]
        [Description("Tests that accordionItem gets deselected when collapsed.")]
        public virtual void ShouldDeselectAccordionItemWhenCollapsed()
        {
            Accordion accordion = DefaultAccordionToTest;
            accordion.SelectionMode = AccordionSelectionMode.One;

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
        #endregion SingleSelection tests

        #region Multi select tests
        /// <summary>
        /// Tests that collectionchanges are allowed during processing.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that collectionchanges are allowed during processing.")]
        public virtual void ShouldScheduleCollectionChangesAfterEventFinish()
        {
            Accordion acc = DefaultAccordionToTest;
            acc.SelectionMode = AccordionSelectionMode.One;

            TestAsync(
                acc,
                () => acc.SelectedItems.Add("item 2"),
                () => Thread.Sleep(20),
                () => Assert.AreEqual(acc.Items[1], acc.SelectedItems[0]),
                () => acc.SelectedIndices.Add(0),
                () => Thread.Sleep(20),
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
                () => System.Threading.Thread.Sleep(40),
                () => Assert.IsTrue(GetAccordionItem(accordion, "item 2").IsSelected),
                () => accordion.SelectedIndices.Add(2),
                () => System.Threading.Thread.Sleep(40),
                () => Assert.IsTrue(GetAccordionItem(accordion, "item 3").IsSelected),
                () => accordion.SelectedIndices.Remove(1),
                () => System.Threading.Thread.Sleep(40),
                () => Assert.IsFalse(GetAccordionItem(accordion, "item 2").IsSelected),
                () => accordion.SelectedItems.Remove("item 3"),
                () => System.Threading.Thread.Sleep(40),
                () => Assert.IsFalse(GetAccordionItem(accordion, "item 3").IsSelected));
        }

        /// <summary>
        /// Tests that adding items will not break multiselectionmode rules.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that adding items will not break multiselectionmode rules.")]
        public virtual void ShouldSelectNewItemWhenAddingItemsThroughSelectedCollections()
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
                () => acc.SelectedItems.Add("1"),
                () => System.Threading.Thread.Sleep(50),
                () => Assert.IsTrue(GetAccordionItem(acc, "1").IsSelected),
                () => acc.SelectedIndices.Add(4),
                () => System.Threading.Thread.Sleep(50),
                () => Assert.IsTrue(GetAccordionItem(acc, "4").IsSelected),
                () => acc.SelectionMode = AccordionSelectionMode.One,
                () => System.Threading.Thread.Sleep(50),
                () => Assert.AreEqual(acc.SelectedItems[0], acc.Items[acc.SelectedIndices[0]]),
                () => acc.SelectedItems.Add("0"),
                () => System.Threading.Thread.Sleep(50),
                () => Assert.IsTrue(GetAccordionItem(acc, "0").IsSelected),
                () => Assert.IsFalse(GetAccordionItem(acc, "1").IsSelected),
                () => acc.SelectedIndices.Add(2),
                () => System.Threading.Thread.Sleep(50),
                () => Assert.IsTrue(GetAccordionItem(acc, "2").IsSelected),
                () => Assert.IsFalse(GetAccordionItem(acc, "0").IsSelected));
        }

        /// <summary>
        /// Tests that deselecting the selected item will select first of the selected items.
        /// </summary>
        [TestMethod]
        [Ignore]
        [Asynchronous]
        [Description("Tests that deselecting the selected item will select first of the selected items")]
        public virtual void ShouldSelectFirstFromSelectedItemsWhenDeselecting()
        {
            // ignored because we changed behavior. Last item is now locked.
            Accordion accordion = DefaultAccordionToTest;
            accordion.SelectionMode = AccordionSelectionMode.OneOrMore;

            TestAsync(
                accordion,
                () => GetAccordionItem(accordion, "item 2").IsSelected = true,
                () => GetAccordionItem(accordion, "item 3").IsSelected = true,
                () => Assert.AreEqual(3, accordion.SelectedItems.Count),
                () => GetAccordionItem(accordion, "item 1").IsSelected = false,
                () => Assert.AreEqual("item 3", accordion.SelectedItem),
                () => GetAccordionItem(accordion, "item 3").IsSelected = false,
                () => GetAccordionItem(accordion, "item 2").IsSelected = false,
                () => Assert.AreEqual("item 1", accordion.SelectedItem),
                () => Assert.AreEqual(1, accordion.SelectedItems.Count),
                () => accordion.SelectionMode = AccordionSelectionMode.ZeroOrMore,
                () => GetAccordionItem(accordion, "item 1").IsSelected = false,
                () => Assert.IsNull(accordion.SelectedItem),
                () => Assert.AreEqual(0, accordion.SelectedItems.Count));
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
                () => Assert.AreEqual(5, acc.SelectedIndices[1]));
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
                () => Thread.Sleep(40),
                () => Assert.AreEqual(2, acc.SelectedItems.Count),
                () => Assert.AreEqual("1", acc.SelectedItems[0]),
                () => Assert.AreEqual("4", acc.SelectedItems[1]));
        }

        #endregion Multi select tests

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
            DataTemplate headerTemplate = new XamlBuilder<DataTemplate> { Name = "headertemplate" } .Load();
            DataTemplate contentTemplate = new XamlBuilder<DataTemplate> { Name = "contenttemplate" } .Load();

            acc.HeaderTemplate = headerTemplate;
            acc.ContentTemplate = contentTemplate;

            acc.Items.Add("item 1");

            TestAsync(
                acc,
                () => Assert.AreEqual(headerTemplate, GetAccordionItem(acc, "item 1").HeaderTemplate),
                () => Assert.AreEqual(contentTemplate, GetAccordionItem(acc, "item 1").ContentTemplate));
        }

        /// <summary>
        /// Tests that a header and contenttemplate are ignored for accordion items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that a header and contenttemplate are ignored for accordion items.")]
        public virtual void ShouldIgnoreTemplatesWhenNotGeneratingItems()
        {
            Accordion acc = new Accordion();
            DataTemplate headerTemplate = new XamlBuilder<DataTemplate> { Name = "headertemplate" } .Load();
            DataTemplate contentTemplate = new XamlBuilder<DataTemplate> { Name = "contenttemplate" } .Load();

            acc.HeaderTemplate = headerTemplate;
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
                () => Assert.AreNotEqual(headerTemplate, ((AccordionItem)acc.Items[0]).HeaderTemplate),
                () => Assert.AreNotEqual(contentTemplate, ((AccordionItem)acc.Items[0]).ContentTemplate),
                () => Assert.AreNotEqual(headerTemplate, ((AccordionItem)acc.Items[1]).HeaderTemplate),
                () => Assert.AreNotEqual(contentTemplate, ((AccordionItem)acc.Items[1]).ContentTemplate),
                () => Assert.AreEqual(customHeaderTemplate, ((AccordionItem)acc.Items[0]).HeaderTemplate),
                () => Assert.AreEqual(customContentTemplate, ((AccordionItem)acc.Items[0]).ContentTemplate),
                () => Assert.IsNull(withoutTemplates.HeaderTemplate),
                () => Assert.IsNull(withoutTemplates.ContentTemplate));
        }

        /// <summary>
        /// Tests that an exception is thrown when both item and contenttemplate are set.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        [Asynchronous]
        [Description("Tests that an exception is thrown when both item and contenttemplate are set.")]
        public virtual void ShouldNotAllowSettingBothContentAndItemTemplate()
        {
            Accordion acc = new Accordion();
            DataTemplate itemTemplate = new XamlBuilder<DataTemplate> { Name = "itemtemplate" } .Load();
            DataTemplate contentTemplate = new XamlBuilder<DataTemplate> { Name = "contenttemplate" } .Load();

            acc.ItemTemplate = itemTemplate;
            acc.ContentTemplate = contentTemplate;

            TestAsync(
                acc,
                () => acc.Items.Add("item 1"));
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
            DataTemplate headerTemplate = new XamlBuilder<DataTemplate> { Name = "headertemplate" } .Load();
            DataTemplate contentTemplate = new XamlBuilder<DataTemplate> { Name = "contenttemplate" } .Load();

            acc.HeaderTemplate = headerTemplate;
            acc.ContentTemplate = contentTemplate;

            AccordionItem item = null;
            TestAsync(
                acc,
                () => acc.Items.Add(a),
                () => item = GetAccordionItem(acc, a),
                () => Assert.AreEqual(headerTemplate, item.HeaderTemplate),
                () => Assert.AreEqual(contentTemplate, item.ContentTemplate));
        }
        #endregion Template tests

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
        #endregion helper methods
    }
}
