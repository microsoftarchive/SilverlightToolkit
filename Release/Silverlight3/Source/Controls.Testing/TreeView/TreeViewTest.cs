// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// TreeViewItem unit tests.
    /// </summary>
    [TestClass]
    [Tag("TreeView")]
    public partial class TreeViewTest : ItemsControlTest
    {
        #region ItemsControls to test
        /// <summary>
        /// Gets a default instance of ItemsControl (or a derived type) to test.
        /// </summary>
        public override ItemsControl DefaultItemsControlToTest
        {
            get { return DefaultTreeViewToTest; }
        }

        /// <summary>
        /// Gets instances of ItemsControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<ItemsControl> ItemsControlsToTest
        {
            get
            {
                return
                    TreeViewsToTest.OfType<ItemsControl>()
                    .Concat(TreeViewsToTest.Select(
                        control =>
                        {
                            control.ItemsSource = new string[] { "Item 1", "Item 2", "Item 3", "Item 4" };
                            return (ItemsControl)control;
                        }));
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenItemsControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenItemsControl> OverriddenItemsControlsToTest
        {
            get { return OverriddenTreeViewsToTest.OfType<IOverriddenItemsControl>(); }
        }
        #endregion ItemsControls to test

        #region TreeViews to test
        /// <summary>
        /// Gets a default instance of TreeView (or a derived type) to test.
        /// </summary>
        public virtual TreeView DefaultTreeViewToTest
        {
            get { return new TreeView(); }
        }

        /// <summary>
        /// Gets instances of TreeView (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<TreeView> TreeViewsToTest
        {
            get
            {
                yield return DefaultTreeViewToTest;

                Style itemContainerStyle = new Style(typeof(TreeViewItem));
                itemContainerStyle.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.Red)));
                itemContainerStyle.Setters.Add(new Setter(Control.FontWeightProperty, FontWeights.Bold));
                yield return new TreeView { ItemContainerStyle = itemContainerStyle };
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenTreeView (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<IOverriddenTreeView> OverriddenTreeViewsToTest
        {
            get { yield return new OverriddenTreeView(); }
        }
        #endregion TreeViews to test

        /// <summary>
        /// Gets the SelectedItem dependency property test.
        /// </summary>
        protected DependencyPropertyTest<TreeView, object> SelectedItemProperty { get; private set; }

        /// <summary>
        /// Gets the SelectedValue dependency property test.
        /// </summary>
        protected DependencyPropertyTest<TreeView, object> SelectedValueProperty { get; private set; }

        /// <summary>
        /// Gets the SelectedValuePath dependency property test.
        /// </summary>
        protected DependencyPropertyTest<TreeView, string> SelectedValuePathProperty { get; private set; }

        /// <summary>
        /// Gets the ItemContainerStyle dependency property test.
        /// </summary>
        protected DependencyPropertyTest<TreeView, Style> ItemContainerStyleProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TreeViewTest class.
        /// </summary>
        public TreeViewTest()
            : base()
        {
            BackgroundProperty.DefaultValue = new SolidColorBrush(Colors.White);
            BorderBrushProperty.DefaultValue = new SolidColorBrush(Colors.Black);
            BorderThicknessProperty.DefaultValue = new Thickness(1);
            PaddingProperty.DefaultValue = new Thickness(1);

            Func<TreeView> initializer = () => DefaultTreeViewToTest;
            SelectedItemProperty = new DependencyPropertyTest<TreeView, object>(this, "SelectedItem")
                {
                    Property = TreeView.SelectedItemProperty,
                    Initializer = initializer,
                    DefaultValue = null,
                    OtherValues = new object[] { new object() }
                };
            SelectedValueProperty = new DependencyPropertyTest<TreeView, object>(this, "SelectedValue")
                {
                    Property = TreeView.SelectedValueProperty,
                    Initializer = initializer,
                    DefaultValue = null,
                    OtherValues = new object[] { new object() }
                };
            SelectedValuePathProperty = new DependencyPropertyTest<TreeView, string>(this, "SelectedValuePath")
                {
                    Property = TreeView.SelectedValuePathProperty,
                    Initializer = initializer,
                    DefaultValue = "",
                    OtherValues = new string[] { "Value", null }
                };
            ItemContainerStyleProperty = new DependencyPropertyTest<TreeView, Style>(this, "ItemContainerStyle")
                {
                    Property = TreeView.ItemContainerStyleProperty,
                    Initializer = initializer,
                    DefaultValue = null,
                    OtherValues = new Style[] { new Style(typeof(HeaderedItemsControl)), new Style(typeof(ItemsControl)), new Style(typeof(Control)) }
                };

            HorizontalContentAlignmentProperty.DefaultValue = HorizontalAlignment.Left;
            VerticalContentAlignmentProperty.DefaultValue = VerticalAlignment.Top;
        }

        /// <summary>
        /// Get the dependency property tests.
        /// </summary>
        /// <returns>The dependency property tests.</returns>
        public override IEnumerable<DependencyPropertyTestMethod> GetDependencyPropertyTests()
        {
            // Get the base Control dependency property tests
            IList<DependencyPropertyTestMethod> tests = TagInherited(base.GetDependencyPropertyTests());

            // SelectedItemProperty tests
            tests.Add(SelectedItemProperty.CheckDefaultValueTest);
            tests.Add(SelectedItemProperty.IsReadOnlyTest);

            // SelectedValueProperty tests
            tests.Add(SelectedValueProperty.CheckDefaultValueTest);
            tests.Add(SelectedValueProperty.IsReadOnlyTest);

            // SelectedValuePathProperty tests
            tests.Add(SelectedValuePathProperty.BindingTest);
            tests.Add(SelectedValuePathProperty.CheckDefaultValueTest);
            tests.Add(SelectedValuePathProperty.ChangeClrSetterTest);
            tests.Add(SelectedValuePathProperty.ChangeSetValueTest);
            tests.Add(SelectedValuePathProperty.SetNullTest);
            tests.Add(SelectedValuePathProperty.ClearValueResetsDefaultTest);
            tests.Add(SelectedValuePathProperty.CanBeStyledTest);
            tests.Add(SelectedValuePathProperty.TemplateBindTest);
            tests.Add(SelectedValuePathProperty.SetXamlAttributeTest);
            tests.Add(SelectedValuePathProperty.SetXamlElementTest);

            // ItemContainerStyleProperty tests
            tests.Add(ItemContainerStyleProperty.BindingTest);
            tests.Add(ItemContainerStyleProperty.CheckDefaultValueTest);
            tests.Add(ItemContainerStyleProperty.ChangeClrSetterTest);
            tests.Add(ItemContainerStyleProperty.ChangeSetValueTest);
            tests.Add(ItemContainerStyleProperty.SetNullTest);
            tests.Add(ItemContainerStyleProperty.ClearValueResetsDefaultTest);
            tests.Add(ItemContainerStyleProperty.CanBeStyledTest);
            tests.Add(ItemContainerStyleProperty.TemplateBindTest);
            tests.Add(ItemContainerStyleProperty.SetXamlAttributeTest.Bug("TODO: XAML Parser?"));
            tests.Add(ItemContainerStyleProperty.SetXamlElementTest.Bug("TODO: XAML Parser?"));

            return tests;
        }

        #region Control contract
        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's template visual states.")]
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> states = DefaultTreeViewToTest.GetType().GetVisualStates();
            Assert.AreEqual(9, states.Count, "Incorrect number of template states");
            Assert.AreEqual("CommonStates", states["Normal"], "Failed to find expected state Normal!");
            Assert.AreEqual("CommonStates", states["MouseOver"], "Failed to find expected state MouseOver!");
            Assert.AreEqual("CommonStates", states["Pressed"], "Failed to find expected state Pressed!");
            Assert.AreEqual("CommonStates", states["Disabled"], "Failed to find expected state Disabled!");
            Assert.AreEqual("FocusStates", states["Focused"], "Failed to find expected state Focused!");
            Assert.AreEqual("FocusStates", states["Unfocused"], "Failed to find expected state Unfocused!");
            Assert.AreEqual<string>("ValidationStates", states["Valid"], "Failed to find expected state Valid!");
            Assert.AreEqual<string>("ValidationStates", states["InvalidFocused"], "Failed to find expected state InvalidFocused!");
            Assert.AreEqual<string>("ValidationStates", states["InvalidUnfocused"], "Failed to find expected state InvalidUnfocused!");
        }
        
        /// <summary>
        /// Verify the control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultTreeViewToTest.GetType().GetStyleTypedProperties();
            Assert.AreEqual(1, properties.Count, "Incorrect number of style typed property attributes!");
            Assert.AreEqual(typeof(TreeViewItem), properties["ItemContainerStyle"], "Failed to find expected style type property ItemContainerStyle!");
        }
        #endregion Control contract

        #region Items
        /// <summary>
        /// Ensure TreeViewItems are treated as their own containers.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure TreeViewItems are treated as their own containers.")]
        [Priority(0)]
        public virtual void TreeViewItemsAreNotWrapped()
        {
            TreeView view = new TreeView();
            TreeViewItem item = new TreeViewItem { Header = "Item!" };
            view.Items.Add(item);
            TestAsync(
                view,
                () => Assert.AreEqual(item, view.Items[0], "Unexpected value for item!"),
                () => Assert.AreEqual(item, view.ItemContainerGenerator.ContainerFromIndex(0), "Item should not have been wrapped!"));
        }

        /// <summary>
        /// Ensure objects are wrapped with TreeViewItems.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure objects are wrapped with TreeViewItems.")]
        [Priority(0)]
        [Bug("33637: ItemContainerGenerator.ItemFromContainer does not correctly return items", Fixed = true)]
        public virtual void ObjectsAreWrappedWithTreeViewItems()
        {
            TreeView view = new TreeView { ItemsSource = new int[] { 1, 2, 3 } };
            TestAsync(
                view,
                () => Assert.AreEqual(3, view.Items.Count, "Expected 3 Items in the TreeView"),
                () => Assert.AreEqual(1, view.Items[0], "Unexpected value for item 0!"),
                () => Assert.IsInstanceOfType(view.ItemContainerGenerator.ContainerFromIndex(0), typeof(TreeViewItem), "Item 0 was not a TreeViewItem!"),
                () => Assert.AreEqual(1, view.ItemContainerGenerator.ItemFromContainer(view.ItemContainerGenerator.ContainerFromIndex(0)), "Item 0 did not have the correct value!"));
        }

        /// <summary>
        /// Ensure items are properly cleared.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure items are properly cleared.")]
        public virtual void ClearItems()
        {
            TreeView view = new TreeView();
            TreeViewItem item = new TreeViewItem { Header = "Item!" };
            view.Items.Add(item);
            TestAsync(
                view,
                () => Assert.AreEqual(1, view.Items.Count, "Expected 1 item in the TreeView"),
                () => Assert.AreEqual(item, view.Items[0], "Unexpected value for item!"),
                () => view.Items.Clear(),
                () => Assert.AreEqual(0, view.Items.Count, "Expected 0 items in the TreeView"),
                () => view.ItemsSource = new int[] { 1, 2, 3 },
                () => Assert.IsNull(item.Parent, "Parent should be null!"));
        }
        #endregion Items

        #region OnItemsChanged
        /// <summary>
        /// Check if adding an item raises OnItemsChanged.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Check if adding an item raises OnItemsChanged.")]
        public virtual void OnItemsChangedAdd()
        {
            foreach (IOverriddenTreeView overriddenView in OverriddenTreeViewsToTest)
            {
                TreeView view = overriddenView as TreeView;
                TreeViewItem item = new TreeViewItem { Header = "Added" };

                overriddenView.OnItemsChangedActions.Test += (NotifyCollectionChangedEventArgs e) =>
                {
                    Assert.AreEqual(e.Action, NotifyCollectionChangedAction.Add, "Expected Add action!");
                    Assert.AreEqual(1, e.NewItems.Count, "Expected 1 new item!");
                    Assert.AreEqual(item, e.NewItems[0], "Expected added item!");
                };

                TestTaskAsync(
                    view,
                    () => view.Items.Add(item));
            }

            EnqueueTestComplete();
        }

        /// <summary>
        /// Add an item and ensure OnItemsChanged updates the selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Add an item and ensure OnItemsChanged updates the selection.")]
        [Priority(0)]
        public virtual void OnItemsChangedAddSelected()
        {
            foreach (IOverriddenTreeView overriddenView in OverriddenTreeViewsToTest)
            {
                TreeView view = overriddenView as TreeView;
                TreeViewItem first = new TreeViewItem { Header = "First" };
                view.Items.Add(first);
                TreeViewItem item = new TreeViewItem { Header = "Added", IsSelected = true };

                overriddenView.OnItemsChangedActions.Test += (NotifyCollectionChangedEventArgs e) =>
                {
                    Assert.IsFalse(first.IsSelected, "Original item should not be selected!");
                    Assert.IsTrue(item.IsSelected, "Added item should be selected!");
                };

                TestTaskAsync(
                    view,
                    () => first.IsSelected = true,
                    () => view.Items.Add(item));
            }

            EnqueueTestComplete();
        }

        /// <summary>
        /// Check if removing an item raises OnItemsChanged.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Remove an item when nothing was selected.")]
        public virtual void OnItemsChangedRemove()
        {
            foreach (IOverriddenTreeView overriddenView in OverriddenTreeViewsToTest)
            {
                TreeView view = overriddenView as TreeView;
                TreeViewItem item = new TreeViewItem { Header = "Removing" };
                view.Items.Add(item);

                overriddenView.OnItemsChangedActions.Test += (NotifyCollectionChangedEventArgs e) =>
                {
                    Assert.AreEqual(e.Action, NotifyCollectionChangedAction.Remove, "Expected Remove action!");
                    Assert.IsNull(e.NewItems, "Expected no new items!");
                    Assert.AreEqual(1, e.OldItems.Count, "Expected 1 old item!");
                    Assert.AreEqual(item, e.OldItems[0], "Expected removed item!");
                };

                TestTaskAsync(
                    view,
                    () => view.Items.Remove(item));
            }

            EnqueueTestComplete();
        }

        /// <summary>
        /// Remove the selected item and ensure OnItemsChanged selects the first
        /// item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Remove the selected item and ensure OnItemsChanged selects the first item.")]
        public virtual void OnItemsChangedRemoveSelected()
        {
            TreeView view = DefaultTreeViewToTest;
            TreeViewItem first = new TreeViewItem { Header = "First", IsSelected = true };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            view.Items.Add(second);

            TestAsync(
                view,
                () => view.Items.Remove(first),
                () => Assert.IsTrue(second.IsSelected, "Second should be selected!"));
        }

        /// <summary>
        /// Remove an unselected item and ensure OnItemsChanged is called.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Remove an unselected item and ensure OnItemsChanged is called.")]
        public virtual void OnItemsChangedRemoveUnselected()
        {
            TreeView view = DefaultTreeViewToTest;
            TreeViewItem first = new TreeViewItem { Header = "First", IsSelected = true };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            view.Items.Add(second);

            TestAsync(
                view,
                () => view.Items.Remove(second),
                () => Assert.IsTrue(first.IsSelected, "First should still be selected!"));
        }

        /// <summary>
        /// Clear the list and ensure OnItemsChanged is raisd with a rest.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Clear the list and ensure OnItemsChanged is raisd with a rest.")]
        public virtual void OnItemsChangedClear()
        {
            foreach (IOverriddenTreeView overriddenView in OverriddenTreeViewsToTest)
            {
                TreeView view = overriddenView as TreeView;
                TreeViewItem first = new TreeViewItem { Header = "First", IsSelected = true };
                TreeViewItem second = new TreeViewItem { Header = "Second" };
                view.Items.Add(first);
                view.Items.Add(second);

                overriddenView.OnItemsChangedActions.Test += (NotifyCollectionChangedEventArgs e) =>
                {
                    Assert.AreEqual(e.Action, NotifyCollectionChangedAction.Reset, "Expected Reset action!");
                    Assert.IsNull(e.NewItems, "Expected no new items!");
                    Assert.IsNull(e.OldItems, "Expected no old items!");
                };

                TestTaskAsync(
                    view,
                    () => view.Items.Clear());
            }

            EnqueueTestComplete();
        }

        /// <summary>
        /// Replace an unselected item and ensure OnItemsChanged is called.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Replace an unselected item and ensure OnItemsChanged is called.")]
        public virtual void OnItemsChangedReplace()
        {
            TreeView view = DefaultTreeViewToTest;
            ObservableCollection<TreeViewItem> items = new ObservableCollection<TreeViewItem>();
            view.ItemsSource = items;

            TreeViewItem first = new TreeViewItem { Header = "First", IsSelected = true };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            items.Add(first);
            items.Add(second);

            TestAsync(
                view,
                () => items[1] = new TreeViewItem { Header = "New Second" },
                () => Assert.IsTrue(first.IsSelected, "First should still be selected!"));
        }

        /// <summary>
        /// Replace a selected item and ensure OnItemsChanged is called.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Replace a selected item and ensure OnItemsChanged is called.")]
        public virtual void OnItemsChangedReplaceSelected()
        {
            TreeView view = DefaultTreeViewToTest;
            ObservableCollection<TreeViewItem> items = new ObservableCollection<TreeViewItem>();
            view.ItemsSource = items;

            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second", IsSelected = true };
            items.Add(first);
            items.Add(second);
            TreeViewItem newSecond = new TreeViewItem { Header = "New Second" };

            TestAsync(
                view,
                () => items[1] = newSecond,
                () => Assert.IsFalse(newSecond.IsSelected, "Second should not be selected!"));
        }
        #endregion OnItemsChanged

        #region Selection
        /// <summary>
        /// Selection changes call the OnSelectedItemChanged method.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Selection changes call the OnSelectedItemChanged method.")]
        public virtual void SelectionCallsOnChanged()
        {
            foreach (IOverriddenTreeView overriddenTreeView in OverriddenTreeViewsToTest)
            {
                TreeView view = overriddenTreeView as TreeView;
                if (view == null)
                {
                    continue;
                }

                TreeViewItem first = new TreeViewItem { Header = "First" };
                TreeViewItem second = new TreeViewItem { Header = "Second" };
                view.Items.Add(first);
                view.Items.Add(second);
                MethodCallMonitor monitor = overriddenTreeView.SelectedItemChangedActions.CreateMonitor();

                TestTaskAsync(
                    view,
                    () =>
                    {
                        monitor.Reset();
                        first.IsSelected = true;
                        monitor.AssertCalled("OnSelectedItemChanged was not called when selecting the first item!");
                    },
                    () =>
                    {
                        monitor.Reset();
                        second.IsSelected = true;
                        monitor.AssertCalled("OnSelectedItemChanged was not called when selecting the second item!");
                    },
                    () => Assert.IsFalse(first.IsSelected, "The first item should no longer be selected!"),
                    () =>
                    {
                        monitor.Reset();
                        second.IsSelected = false;
                        monitor.AssertCalled("OnSelectedItemChanged was not called when unselecting the second item!");
                    });
            }

            EnqueueTestComplete();
        }

        /// <summary>
        /// Selection changes raise the SelectedItemChanged event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Selection changes raise the SelectedItemChanged event.")]
        [Priority(0)]
        public virtual void SelectionRaisesEvent()
        {
            foreach (TreeView view in TreeViewsToTest)
            {
                TreeViewItem first = new TreeViewItem { Header = "First" };
                TreeViewItem second = new TreeViewItem { Header = "Second" };
                view.Items.Add(first);
                view.Items.Add(second);
                int count = 0;
                view.SelectedItemChanged += delegate { count++; };
                TestTaskAsync(
                    view,
                    () => Assert.AreEqual(0, count, "SelectedItemChanged should not have been fired yet!"),
                    () => first.IsSelected = true,
                    () => Assert.AreEqual(1, count, "SelectedItemChanged was not fired when selecting the first item!"),
                    () => second.IsSelected = true,
                    () => Assert.AreEqual(2, count, "SelectedItemChanged was not fired when selecting the second item!"),
                    () => Assert.IsFalse(first.IsSelected, "The first item should no longer be selected!"),
                    () => second.IsSelected = false,
                    () => Assert.AreEqual(3, count, "SelectedItemChanged was not fired when unselecting the second item!"));
            }

            EnqueueTestComplete();
        }

        /// <summary>
        /// Ensure we can succesfully unhook from the Selection event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure we can succesfully unhook from the Selection event.")]
        public virtual void SelectionCanUnhook()
        {
            TreeView view = DefaultTreeViewToTest;
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            view.Items.Add(second);
            int count = 0;
            RoutedPropertyChangedEventHandler<object> handler = delegate { count++; };
            view.SelectedItemChanged += handler;
            TestAsync(
                view,
                () => Assert.AreEqual(0, count, "SelectedItemChanged should not have been fired yet!"),
                () => first.IsSelected = true,
                () => Assert.AreEqual(1, count, "SelectedItemChanged was not fired when selecting the first item!"),
                () => second.IsSelected = true,
                () => Assert.AreEqual(2, count, "SelectedItemChanged was not fired when selecting the second item!"),
                () => view.SelectedItemChanged -= handler,
                () => second.IsSelected = false,
                () => Assert.AreEqual(2, count, "SelectedItemChanged was fired after unhooking!"));
        }

        /// <summary>
        /// Verify the SelectedValuePath is used to define SelectedValue.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the SelectedValuePath is used to define SelectedValue.")]
        [Bug("33637: ItemContainerGenerator.ItemFromContainer does not correctly return items", Fixed = true)]
        public virtual void CheckSelectedValue()
        {
            TreeView view = DefaultTreeViewToTest;
            view.ItemsSource = new string[] { "Hi", "There" };
            view.SelectedValuePath = "Length";
            TestAsync(
                view,
                () => (view.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem).IsSelected = true,
                () => Assert.AreEqual("Hi", view.SelectedItem, "Unexpected SelectedItem!"),
                () => Assert.AreEqual(2, view.SelectedValue, "Unexpected SelectedValue!"));
        }
        #endregion Selection

        /// <summary>
        /// Verify the ItemContainerStyle is applied.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the ItemContainerStyle is applied.")]
        [Bug("528117 - TreeView - ItemContainerStyle is ignored", Fixed = true)]
        [Priority(0)]
        public virtual void ItemContainerStyleIsApplied()
        {
            Style style = new Style(typeof(TreeViewItem));
            style.Setters.Add(new Setter(TreeView.ForegroundProperty, new SolidColorBrush(Colors.Red)));

            TreeView view = new TreeView
            {
                ItemsSource = new[] { "foo", "bar", "baz" },
                ItemContainerStyle = style
            };
            TreeViewItem item = null;

            TestAsync(
                5,
                view,
                () => item = view.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem,
                () => Assert.IsNotNull(item, "Did not find item!"),
                () => TestExtensions.AssertBrushesAreEqual(new SolidColorBrush(Colors.Red), item.Foreground, "Expected a red foreground!"));
        }

        /// <summary>
        /// Verify focusing the TreeView does not select the first item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify focusing the TreeView does not select the first item.")]
        [Tag(Tags.RequiresFocus)]
        [Bug("580436 - TreeView - Setting focus on TreeView selects first item", Fixed = true)]
        public virtual void FocusingTreeViewShouldNotSelectFirstItem()
        {
            TreeView view = new TreeView();
            TreeViewItem item = new TreeViewItem { Header = "Item" };
            view.Items.Add(item);

            TestAsync(
                view,
                () => Assert.IsFalse(item.IsSelected, "item should not be selected initially!"),
                () => view.Focus(),
                () => Assert.IsFalse(item.IsSelected, "item should not be selected after focusing TreeView!"));
        }

        /// <summary>
        /// Verify focusing the TreeView does not scroll the ScrollViewer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify focusing the TreeView does not scroll the ScrollViewer.")]
        [Tag(Tags.RequiresFocus)]
        [Bug("580436 - TreeView - TreeView selects root TreeViewItem when clicking in empty space Title is required", Fixed = true)]
        public virtual void FocusingTreeViewShouldNotScroll()
        {
            TreeView view = new TreeView
            {
                ItemsSource = new[] { 1, 2, 3, 4, 5, 6 },
                Height = 100
            };
            ScrollViewer scroller = null;

            TestAsync(
                25,
                view,
                () => scroller = view.GetVisualDescendents().OfType<ScrollViewer>().FirstOrDefault(),
                () => Assert.IsNotNull(scroller, "scroller should not be null!"),
                () => Assert.AreEqual(0, scroller.VerticalOffset, "Should not be scrolled initially!"),
                () => scroller.ScrollToVerticalOffset(25),
                () => view.Focus(),
                () => Assert.AreEqual(25, scroller.VerticalOffset, "Should not have scrolled after focusing!"));
        }

        /// <summary>
        /// Verify selection works before a TreeViewItem is in the visual tree.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify selection works before a TreeViewItem is in the visual tree.")]
        [Bug("541808 - TreeView - Scenario involving collapsing expansion and selection does not behave as expected", Fixed = true)]
        public virtual void SelectionBeforeInVisualTree()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second", IsSelected = true };
            view.Items.Add(first);
            first.Items.Add(second);

            TestAsync(
                500,
                view,
                () => first.IsExpanded = true,
                () => Assert.IsTrue(second.IsSelected, "Second is not selected!"),
                () => Assert.AreEqual(second, view.SelectedItem, "TreeView does not recognize Second as selected!"),
                () => first.IsSelected = true,
                () => Assert.IsFalse(second.IsSelected, "Second should not be selected any longer!"));
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
                    "xmlns:controls='clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls'>" +
                    "<Grid.Resources>" +
                        "<SolidColorBrush x:Key='SCBO' Color='Orange'/>" +
                    "</Grid.Resources>" +
                    "<controls:TreeView x:Name='TV' Background='{StaticResource SCBO}'>" +
                        "<controls:TreeView.Resources>" +
                            "<SolidColorBrush x:Key='SCBP' Color='Purple'/>" +
                        "</controls:TreeView.Resources>" +
                        "<controls:TreeView.ItemsSource>" +
                            "<local:TestObjectCollection>" +
                                "<ContentControl x:Name='CC' Background='{StaticResource SCBO}' Foreground='{StaticResource SCBP}'/>" +
                            "</local:TestObjectCollection>" +
                        "</controls:TreeView.ItemsSource>" +
                    "</controls:TreeView>" +
                "</Grid>") as Panel;
            SolidColorBrush scbo = panel.Resources["SCBO"] as SolidColorBrush;
            TreeView tv = panel.FindName("TV") as TreeView;
            SolidColorBrush scbp = tv.Resources["SCBP"] as SolidColorBrush;
            ContentControl cc = panel.FindName("CC") as ContentControl;
            if (null == cc)
            {
                cc = tv.ItemsSource.OfType<ContentControl>().Single();
            }
            Assert.AreEqual(scbo, tv.Background);
            Assert.AreEqual(scbo, cc.Background);
            Assert.AreEqual(scbp, cc.Foreground);
        }

        /// <summary>
        /// Verifies that binding to SelectedItem works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verifies that binding to SelectedItem works properly.")]
        [Bug("33637: ItemContainerGenerator.ItemFromContainer does not correctly return items", Fixed = true)]
        public void SelectedItemBinding()
        {
            TreeView treeView = new TreeView();
            treeView.ItemsSource = "some test text".Split();
            ContentControl contentControl = new ContentControl();
            Binding binding = new Binding("SelectedItem");
            binding.Source = treeView;
            contentControl.SetBinding(ContentControl.ContentProperty, binding);

            StackPanel panel = new StackPanel();
            panel.Children.Add(treeView);
            panel.Children.Add(contentControl);

            TreeViewItem treeViewItem = null;

            TestAsync(
                panel,
                () => treeViewItem = ((TreeViewItem)(treeView.ItemContainerGenerator.ContainerFromIndex(1))),
                () => treeViewItem.IsSelected = true,
                () => Assert.AreEqual("test", treeView.SelectedItem),
                () => Assert.AreEqual("test", contentControl.Content));
        }

        /// <summary>
        /// Verify that the TreeView does not scroll on expansion unless it was
        /// user initiated.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that the TreeView does not scroll on expansion unless it was user initiated.")]
        [Bug("602966: TreeView Scrolls when adding new TreeViewItems", Fixed = true)]
        public virtual void OnlyUserInitiatedExpansionScrollsTreeView()
        {
            TreeView view = new TreeView { Height = 100 };
            view.Items.Add(1);
            view.Items.Add(2);
            view.Items.Add(3);
            view.Items.Add(4);
            view.Items.Add(5);
            view.Items.Add(6);
            view.Items.Add(7);
            view.Items.Add(new TreeViewItem { Header = 8, IsExpanded = true, ItemsSource = new[] { 9, 10 } });
            ScrollViewer scroller = null;

            TestAsync(
                25,
                view,
                () => scroller = view.GetVisualDescendents().OfType<ScrollViewer>().FirstOrDefault(),
                () => Assert.IsNotNull(scroller, "scroller should not be null!"),
                () => Assert.AreEqual(0, scroller.VerticalOffset, "Should not be scrolled initially!"));
        }

        /// <summary>
        /// Verify that scrolling a TreeViewItem into view includes the entire top of the item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that scrolling a TreeViewItem into view includes the entire top of the item.")]
        [Bug("680117 - TreeView's Page Up does not ensure that the newly selected item is completely in view")]
        public virtual void ScrollingIntoViewIncludesTopOfTreeViewItem()
        {
            TreeView view = new TreeView { Height = 100 };
            TreeViewItem first = new TreeViewItem { Header = 1 };
            TreeViewItem last = new TreeViewItem { Header = 10 };
            view.Items.Add(first);
            view.Items.Add(2);
            view.Items.Add(3);
            view.Items.Add(4);
            view.Items.Add(5);
            view.Items.Add(6);
            view.Items.Add(7);
            view.Items.Add(8);
            view.Items.Add(9);
            view.Items.Add(last);
            ScrollViewer scroller = null;
            Panel host = null;

            Action<TreeViewItem, string> assertInView = (item, name) =>
                {
                    GeneralTransform transform = item.TransformToVisual(host);
                    Rect itemRect = new Rect(
                        transform.Transform(new Point()),
                        transform.Transform(new Point(item.ActualWidth, item.ActualHeight)));
                    double itemTop = itemRect.Top;
                    double itemBottom = itemRect.Bottom;
                    
                    double pageTop = scroller.VerticalOffset;
                    double pageBottom = pageTop + scroller.ViewportHeight;

                    Assert.IsTrue(itemTop >= pageTop && itemTop <= pageBottom, "Top of item '{0}' at {1} is not scrolled into view (0, {2})!", name, itemTop, pageBottom);
                    Assert.IsTrue(itemBottom >= pageTop && itemBottom <= pageBottom, "Bottom of item '{0}' at {1} is not scrolled into view (0, {2})!", name, itemTop, pageBottom);
                };

            double bottomOffset = 0.0;
            TestAsync(
                25,
                view,
                () => scroller = view.GetVisualDescendents().OfType<ScrollViewer>().FirstOrDefault(),
                () => Assert.IsNotNull(scroller, "scroller should not be null!"),
                () => host = view.GetVisualDescendents().OfType<StackPanel>().FirstOrDefault(),
                () => Assert.IsNotNull(host, "host should not be null!"),
                () => last.IsSelected = true,
                () => bottomOffset = scroller.VerticalOffset,
                () => Assert.AreNotEqual(0, bottomOffset, "ScrollViewer did not scroll to include last element!"),
                () => assertInView(last, "Last"),
                () => first.IsSelected = true,
                () => Assert.AreNotEqual(bottomOffset, scroller.VerticalOffset, "ScrollViewer did not return to first element!"),
                () => assertInView(first, "First"));
        }

        /// <summary>
        /// Verify that preventing selection from changing does not create two selected item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that preventing selection from changing does not create two selected item.")]
        [Bug("680061 - TreeView can easily end up with multiple selected TreeViewItem")]
        public virtual void PreventingSelectionDoesNotLeaveBothSelected()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = 1, IsSelected = true };
            TreeViewItem second = new TreeViewItem { Header = 2 };
            view.Items.Add(first);
            view.Items.Add(second);

            second.Unselected += (s, e) => second.IsSelected = true;

            TestAsync(
                view,
                () => second.IsSelected = true,
                () => first.IsSelected = true,
                () => Assert.IsTrue(first.IsSelected, "First should be selected!"),
                () => Assert.IsFalse(second.IsSelected, "Second should not be selected!"));
        }
    }
}