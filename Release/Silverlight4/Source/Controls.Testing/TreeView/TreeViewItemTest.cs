// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// TreeViewItem unit tests.
    /// </summary>
    [TestClass]
    [Tag("TreeView")]
    public partial class TreeViewItemTest : HeaderedItemsControlTest
    {
        #region HeaderedItemsControls to test
        /// <summary>
        /// Gets a default instance of HeaderedItemsControl (or a derived type) to test.
        /// </summary>
        public override HeaderedItemsControl DefaultHeaderedItemsControlToTest
        {
            get { return DefaultTreeViewItemToTest; }
        }

        /// <summary>
        /// Gets instances of HeaderedItemsControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<HeaderedItemsControl> HeaderedItemsControlsToTest
        {
            get { return TreeViewItemsToTest.OfType<HeaderedItemsControl>(); }
        }

        /// <summary>
        /// Gets instances of IOverriddenHeaderedItemsControl (or derived types) to test.
        /// </summary>
        public override IEnumerable<IOverriddenHeaderedItemsControl> OverriddenHeaderedItemsControlsToTest
        {
            get { return OverriddenTreeViewItemsToTest.OfType<IOverriddenHeaderedItemsControl>(); }
        }
        #endregion HeaderedItemsControls to test

        #region TreeViewItems to test
        /// <summary>
        /// Gets a default instance of TreeViewItem (or a derived type) to test.
        /// </summary>
        public virtual TreeViewItem DefaultTreeViewItemToTest
        {
            get { return new TreeViewItem(); }
        }

        /// <summary>
        /// Gets instances of TreeViewItem (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<TreeViewItem> TreeViewItemsToTest
        {
            get
            {
                yield return DefaultTreeViewItemToTest;

                Style itemContainerStyle = new Style(typeof(Control));
                itemContainerStyle.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.Red)));
                itemContainerStyle.Setters.Add(new Setter(Control.FontWeightProperty, FontWeights.Bold));
                yield return new TreeViewItem { ItemContainerStyle = itemContainerStyle };

                yield return new TreeViewItem
                {
                    Header = "Test Header",
                    ItemsSource = new string[] { "First", "Second", "Third" }
                };

                yield return new TreeViewItem
                {
                    Header = "Test Header",
                    ItemsSource = new string[] { "First", "Second", "Third" },
                    IsExpanded = true
                };
            }
        }

        /// <summary>
        /// Gets instances of IOverriddenTreeViewItem (or derived types) to test.
        /// </summary>
        public virtual IEnumerable<IOverriddenTreeViewItem> OverriddenTreeViewItemsToTest
        {
            get { yield return new OverriddenTreeViewItem(); }
        }
        #endregion TreeViewItems to test

        /// <summary>
        /// Gets the HasItems dependency property test.
        /// </summary>
        protected DependencyPropertyTest<TreeViewItem, bool> HasItemsProperty { get; private set; }

        /// <summary>
        /// Gets the IsExpanded dependency property test.
        /// </summary>
        protected DependencyPropertyTest<TreeViewItem, bool> IsExpandedProperty { get; private set; }

        /// <summary>
        /// Gets the IsSelected dependency property test.
        /// </summary>
        protected DependencyPropertyTest<TreeViewItem, bool> IsSelectedProperty { get; private set; }

        /// <summary>
        /// Gets the IsSelectionActive dependency property test.
        /// </summary>
        protected DependencyPropertyTest<TreeViewItem, bool> IsSelectionActiveProperty { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TreeViewItemTest class.
        /// </summary>
        public TreeViewItemTest()
            : base()
        {
            BackgroundProperty.DefaultValue = new SolidColorBrush(Colors.Transparent);
            BorderThicknessProperty.DefaultValue = new Thickness(1);
            PaddingProperty.DefaultValue = new Thickness(3);

            Func<TreeViewItem> initializer = () => DefaultTreeViewItemToTest;
            HasItemsProperty = new DependencyPropertyTest<TreeViewItem, bool>(this, "HasItems")
                {
                    Property = TreeViewItem.HasItemsProperty,
                    Initializer = initializer,
                    DefaultValue = false,
                    OtherValues = new bool[] { true }
                };
            IsExpandedProperty = new DependencyPropertyTest<TreeViewItem, bool>(this, "IsExpanded")
                {
                    Property = TreeViewItem.IsExpandedProperty,
                    Initializer = initializer,
                    DefaultValue = false,
                    OtherValues = new bool[] { true }
                };
            IsSelectedProperty = new DependencyPropertyTest<TreeViewItem, bool>(this, "IsSelected")
                {
                    Property = TreeViewItem.IsSelectedProperty,
                    Initializer = initializer,
                    DefaultValue = false,
                    OtherValues = new bool[] { true }
                };
            IsSelectionActiveProperty = new DependencyPropertyTest<TreeViewItem, bool>(this, "IsSelectionActive")
                {
                    Property = TreeViewItem.IsSelectionActiveProperty,
                    Initializer = initializer,
                    DefaultValue = false,
                    OtherValues = new bool[] { true }
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

            // HasItemsProperty tests
            tests.Add(HasItemsProperty.CheckDefaultValueTest);
            tests.Add(HasItemsProperty.IsReadOnlyTest);

            // IsExpandedProperty tests
            tests.Add(IsExpandedProperty.BindingTest);
            tests.Add(IsExpandedProperty.CheckDefaultValueTest);
            tests.Add(IsExpandedProperty.ChangeClrSetterTest);
            tests.Add(IsExpandedProperty.ChangeSetValueTest);
            tests.Add(IsExpandedProperty.ClearValueResetsDefaultTest);
            tests.Add(IsExpandedProperty.CanBeStyledTest);
            tests.Add(IsExpandedProperty.TemplateBindTest);
            tests.Add(IsExpandedProperty.ChangesVisualStateTest(false, true, "Expanded"));
            tests.Add(IsExpandedProperty.ChangesVisualStateTest(true, false, "Collapsed"));
            tests.Add(IsExpandedProperty.SetXamlAttributeTest);
            tests.Add(IsExpandedProperty.SetXamlElementTest);

            // IsSelectedProperty tests
            tests.Add(IsSelectedProperty.BindingTest);
            tests.Add(IsSelectedProperty.CheckDefaultValueTest);
            tests.Add(IsSelectedProperty.ClearValueResetsDefaultTest);
            tests.Add(IsSelectedProperty.CanBeStyledTest);
            tests.Add(IsSelectedProperty.TemplateBindTest);

            // IsSelectionActiveProperty tests
            tests.Add(IsSelectionActiveProperty.CheckDefaultValueTest);
            tests.Add(IsSelectionActiveProperty.IsReadOnlyTest);

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
            IDictionary<string, Type> parts = DefaultTreeViewItemToTest.GetType().GetTemplateParts();
            Assert.AreEqual(2, parts.Count, "Incorrect number of template parts");
            Assert.AreEqual(typeof(ToggleButton), parts["ExpanderButton"], "Failed to find expected part ExpanderButton!");
            Assert.AreEqual(typeof(FrameworkElement), parts["Header"], "Failed to find expected part Header!");
        }

        /// <summary>
        /// Verify the control's template visual states.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's template visual states.")]
        public override void TemplateVisualStatesAreDefined()
        {
            IDictionary<string, string> states = DefaultTreeViewItemToTest.GetType().GetVisualStates();
            Assert.AreEqual(13, states.Count, "Incorrect number of template states");
            Assert.AreEqual("ExpansionStates", states["Expanded"], "Failed to find expected state Expanded!");
            Assert.AreEqual("ExpansionStates", states["Collapsed"], "Failed to find expected state Collapsed!");
            Assert.AreEqual("HasItemsStates", states["HasItems"], "Failed to find expected state HasItems!");
            Assert.AreEqual("HasItemsStates", states["NoItems"], "Failed to find expected state NoItems!");
            Assert.AreEqual("CommonStates", states["Normal"], "Failed to find expected state Normal!");
            Assert.AreEqual("CommonStates", states["MouseOver"], "Failed to find expected state MouseOver!");
            Assert.AreEqual("CommonStates", states["Pressed"], "Failed to find expected state Pressed!");
            Assert.AreEqual("CommonStates", states["Disabled"], "Failed to find expected state Disabled!");
            Assert.AreEqual("FocusStates", states["Focused"], "Failed to find expected state Focused!");
            Assert.AreEqual("FocusStates", states["Unfocused"], "Failed to find expected state Unfocused!");
            Assert.AreEqual("SelectionStates", states["Selected"], "Failed to find expected state Selected!");
            Assert.AreEqual("SelectionStates", states["Unselected"], "Failed to find expected state Unselected!");
            Assert.AreEqual("SelectionStates", states["SelectedInactive"], "Failed to find expected state SelectedInactive!");
        }

        /// <summary>
        /// Verify the control's style typed properties.
        /// </summary>
        [TestMethod]
        [Description("Verify the control's style typed properties.")]
        public override void StyleTypedPropertiesAreDefined()
        {
            IDictionary<string, Type> properties = DefaultTreeViewItemToTest.GetType().GetStyleTypedProperties();
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
        public virtual void TreeViewItemsAreNotWrapped()
        {
            TreeView view = new TreeView();
            TreeViewItem parent = new TreeViewItem { Header = "Parent", IsExpanded = true };
            TreeViewItem child = new TreeViewItem { Header = "Child" };
            view.Items.Add(parent);
            parent.Items.Add(child);
            TestAsync(
                5,
                view,
                () => Assert.AreEqual(child, parent.Items[0], "Unexpected value for item!"),
                () => Assert.AreEqual(child, parent.ItemContainerGenerator.ContainerFromIndex(0), "Item should not have been wrapped!"));
        }

        /// <summary>
        /// Ensure objects are wrapped with TreeViewItems.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure objects are wrapped with TreeViewItems.")]
        [Bug("33637: ItemContainerGenerator.ItemFromContainer does not correctly return items", Fixed = true)]
        public virtual void ObjectsAreWrappedWithTreeViewItems()
        {
            TreeViewItem item = new TreeViewItem { Header = "Items", IsExpanded = true, ItemsSource = new int[] { 1, 2, 3 } };
            TestAsync(
                5,
                item,
                () => Assert.AreEqual(3, item.Items.Count, "Expected 3 Items in the TreeViewItem!"),
                () => Assert.AreEqual(1, item.Items[0], "Unexpected value for item 0!"),
                () => Assert.IsInstanceOfType(item.ItemContainerGenerator.ContainerFromIndex(0), typeof(TreeViewItem), "Item 0 was not a TreeViewItem!"),
                () => Assert.AreEqual(1, item.ItemContainerGenerator.ItemFromContainer(item.ItemContainerGenerator.ContainerFromIndex(0)), "Item 0 did not have the correct value!"));
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
            TreeViewItem parent = new TreeViewItem { Header = "Parent", IsExpanded = true };
            TreeViewItem child = new TreeViewItem { Header = "Child" };
            view.Items.Add(parent);
            parent.Items.Add(child);
            TestAsync(
                5,
                view,
                () => Assert.AreEqual(1, parent.Items.Count, "Expected 1 item in the TreeViewItem!"),
                () => Assert.AreEqual(child, parent.Items[0], "Unexpected value for item!"),
                () => parent.Items.Clear(),
                () => Assert.AreEqual(0, parent.Items.Count, "Expected 0 items in the TreeViewItem!"),
                () => parent.ItemsSource = new int[] { 1, 2, 3 },
                () => Assert.IsNull(child.Parent, "Parent should be null!"));
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
            foreach (IOverriddenTreeViewItem overriddenItem in OverriddenTreeViewItemsToTest)
            {
                TreeViewItem item = overriddenItem as TreeViewItem;
                TreeViewItem child = new TreeViewItem { Header = "Added" };

                overriddenItem.OnItemsChangedActions.Test += (NotifyCollectionChangedEventArgs e) =>
                {
                    Assert.AreEqual(e.Action, NotifyCollectionChangedAction.Add, "Expected Add action!");
                    Assert.AreEqual(1, e.NewItems.Count, "Expected 1 new item!");
                    Assert.AreEqual(child, e.NewItems[0], "Expected added item!");
                };

                TestTaskAsync(
                    item,
                    () => item.Items.Add(child));
            }

            EnqueueTestComplete();
        }

        /// <summary>
        /// Add an item and ensure OnItemsChanged updates the selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Add an item and ensure OnItemsChanged updates the selection.")]
        public virtual void OnItemsChangedAddSelected()
        {
            foreach (IOverriddenTreeViewItem overriddenItem in OverriddenTreeViewItemsToTest)
            {
                TreeView view = new TreeView();
                TreeViewItem item = overriddenItem as TreeViewItem;
                view.Items.Add(item);
                TreeViewItem first = new TreeViewItem { Header = "First", IsSelected = true };
                item.Items.Add(first);
                TreeViewItem second = new TreeViewItem { Header = "Added", IsSelected = true };

                overriddenItem.OnItemsChangedActions.Test += (NotifyCollectionChangedEventArgs e) =>
                {
                    Assert.IsFalse(first.IsSelected, "Original item should not be selected!");
                    Assert.IsTrue(second.IsSelected, "Added item should be selected!");
                };

                TestTaskAsync(
                    view,
                    () => item.Items.Add(second));
            }

            EnqueueTestComplete();
        }

        /// <summary>
        /// Add an item and ensure OnItemsChanged updates the selection when in
        /// a TreeView.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Add an item and ensure OnItemsChanged updates the selection.")]
        [Priority(0)]
        public virtual void OnItemsChangedAddSelectedInTreeView()
        {
            foreach (IOverriddenTreeViewItem overriddenItem in OverriddenTreeViewItemsToTest)
            {
                TreeViewItem item = overriddenItem as TreeViewItem;
                TreeView view = new TreeView();
                view.Items.Add(item);
                TreeViewItem first = new TreeViewItem { Header = "First", IsSelected = true };
                item.Items.Add(first);
                TreeViewItem second = new TreeViewItem { Header = "Added", IsSelected = true };

                overriddenItem.OnItemsChangedActions.Test += (NotifyCollectionChangedEventArgs e) =>
                {
                    Assert.IsFalse(first.IsSelected, "Original item should not be selected!");
                    Assert.IsTrue(second.IsSelected, "Added item should be selected!");
                };

                TestTaskAsync(
                    view,
                    () => item.Items.Add(second));
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
            foreach (IOverriddenTreeViewItem overriddenItem in OverriddenTreeViewItemsToTest)
            {
                TreeViewItem item = overriddenItem as TreeViewItem;
                TreeViewItem child = new TreeViewItem { Header = "Removing" };
                item.Items.Add(child);

                overriddenItem.OnItemsChangedActions.Test += (NotifyCollectionChangedEventArgs e) =>
                {
                    Assert.AreEqual(e.Action, NotifyCollectionChangedAction.Remove, "Expected Remove action!");
                    Assert.IsNull(e.NewItems, "Expected no new items!");
                    Assert.AreEqual(1, e.OldItems.Count, "Expected 1 old item!");
                    Assert.AreEqual(child, e.OldItems[0], "Expected removed item!");
                };

                TestTaskAsync(
                    item,
                    () => item.Items.Remove(child));
            }

            EnqueueTestComplete();
        }

        /// <summary>
        /// Remove the selected item and ensure OnItemsChanged does not selects
        /// the root item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Remove the selected item and ensure OnItemsChanged does not selects the root item.")]
        public virtual void OnItemsChangedRemoveSelected()
        {
            TreeViewItem item = DefaultTreeViewItemToTest;
            TreeViewItem first = new TreeViewItem { Header = "First", IsSelected = true };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            item.Items.Add(first);
            item.Items.Add(second);

            TestAsync(
                item,
                () => item.Items.Remove(first),
                () => Assert.IsFalse(item.IsSelected, "Item should not be selected!"));
        }

        /// <summary>
        /// Remove the selected item and ensure OnItemsChanged selects the root
        /// item when in a TreeView.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Remove the selected item and ensure OnItemsChanged selects the root item when in a TreeView.")]
        public virtual void OnItemsChangedRemoveSelectedInTreeView()
        {
            TreeView view = new TreeView();
            TreeViewItem item = DefaultTreeViewItemToTest;
            item.IsExpanded = true;
            TreeViewItem first = new TreeViewItem { Header = "First", IsSelected = true };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            item.Items.Add(first);
            item.Items.Add(second);
            view.Items.Add(item);

            TestAsync(
                view,
                () => item.Items.Remove(first),
                () => Assert.IsTrue(item.IsSelected, "Item should be selected!"));
        }

        /// <summary>
        /// Remove an unselected item and ensure OnItemsChanged is called.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Remove an unselected item and ensure OnItemsChanged is called.")]
        public virtual void OnItemsChangedRemoveUnselected()
        {
            TreeViewItem item = DefaultTreeViewItemToTest;
            item.IsExpanded = true;
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            item.Items.Add(first);
            item.Items.Add(second);

            TestAsync(
                item,
                () => item.Items.Remove(second));
        }

        /// <summary>
        /// Clear the list and ensure OnItemsChanged is raisd with a rest.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Clear the list and ensure OnItemsChanged is raisd with a rest.")]
        public virtual void OnItemsChangedClear()
        {
            foreach (IOverriddenTreeViewItem overriddenItem in OverriddenTreeViewItemsToTest)
            {
                TreeViewItem item = overriddenItem as TreeViewItem;
                item.IsExpanded = true;
                TreeViewItem first = new TreeViewItem { Header = "First", IsSelected = true };
                TreeViewItem second = new TreeViewItem { Header = "Second" };
                item.Items.Add(first);
                item.Items.Add(second);

                overriddenItem.OnItemsChangedActions.Test += (NotifyCollectionChangedEventArgs e) =>
                {
                    Assert.AreEqual(e.Action, NotifyCollectionChangedAction.Reset, "Expected Reset action!");
                    Assert.IsNull(e.NewItems, "Expected no new items!");
                    Assert.IsNull(e.OldItems, "Expected no old items!");
                };

                TestTaskAsync(
                    item,
                    () => item.Items.Clear());
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
            TreeViewItem item = DefaultTreeViewItemToTest;
            item.IsExpanded = true;
            ObservableCollection<TreeViewItem> items = new ObservableCollection<TreeViewItem>();
            item.ItemsSource = items;

            TreeViewItem first = new TreeViewItem { Header = "First", IsSelected = true };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            items.Add(first);
            items.Add(second);

            TestAsync(
                item,
                () => items[1] = new TreeViewItem { Header = "New Second" },
                () => Assert.IsFalse(item.IsSelected, "Item should not be selected!"));
        }

        /// <summary>
        /// Replace an unselected item and ensure OnItemsChanged is called in a
        /// TreeView.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Replace an unselected item and ensure OnItemsChanged is called in a TreeView.")]
        public virtual void OnItemsChangedReplaceInTreeView()
        {
            TreeView view = new TreeView();
            TreeViewItem item = DefaultTreeViewItemToTest;
            item.IsExpanded = true;
            ObservableCollection<TreeViewItem> items = new ObservableCollection<TreeViewItem>();
            item.ItemsSource = items;
            view.Items.Add(items);

            TreeViewItem first = new TreeViewItem { Header = "First", IsSelected = true };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            items.Add(first);
            items.Add(second);

            TestAsync(
                item,
                () => items[1] = new TreeViewItem { Header = "New Second" },
                () => Assert.IsTrue(first.IsSelected, "First item should be selected!"));
        }

        /// <summary>
        /// Replace a selected item and ensure OnItemsChanged is called.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Replace a selected item and ensure OnItemsChanged is called.")]
        public virtual void OnItemsChangedReplaceSelected()
        {
            TreeViewItem item = DefaultTreeViewItemToTest;
            item.IsExpanded = true;
            ObservableCollection<TreeViewItem> items = new ObservableCollection<TreeViewItem>();
            item.ItemsSource = items;

            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second", IsSelected = true };
            items.Add(first);
            items.Add(second);
            TreeViewItem newSecond = new TreeViewItem { Header = "New Second" };

            TestAsync(
                item,
                () => items[1] = newSecond,
                () => Assert.IsFalse(item.IsSelected, "Item should not be selected!"));
        }

        /// <summary>
        /// Replace a selected item and ensure OnItemsChanged is called in a
        /// TreeView.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Replace a selected item and ensure OnItemsChanged is called in a TreeView.")]
        public virtual void OnItemsChangedReplaceSelectedInTreeView()
        {
            TreeView view = new TreeView();
            TreeViewItem item = DefaultTreeViewItemToTest;
            item.IsExpanded = true;
            ObservableCollection<TreeViewItem> items = new ObservableCollection<TreeViewItem>();
            item.ItemsSource = items;
            view.Items.Add(item);

            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second", IsSelected = true };
            items.Add(first);
            items.Add(second);
            TreeViewItem newSecond = new TreeViewItem { Header = "New Second" };

            TestAsync(
                view,
                () => items[1] = newSecond,
                () => Assert.IsFalse(newSecond.IsSelected, "New second item should be not be selected!"));
        }
        #endregion OnItemsChanged

        #region Expand
        /// <summary>
        /// Expand and collapse a TreeViewItem.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Expand and collapse a TreeViewItem.")]
        [Priority(0)]
        public virtual void ExpandCollapse()
        {
            TreeViewItem parent = new TreeViewItem { Header = "Parent" };
            parent.Items.Add(new TreeViewItem { Header = "First" });
            parent.Items.Add(new TreeViewItem { Header = "Second" });

            TestAsync(
                5,
                parent,
                () => Assert.IsFalse(parent.IsExpanded, "Parent should start collapsed!"),
                () => Assert.IsNull(parent.ItemContainerGenerator.ContainerFromIndex(0), "Child items should not be generated before expanding!"),
                () => parent.IsExpanded = true,
                () => Assert.IsNotNull(parent.ItemContainerGenerator.ContainerFromIndex(0), "Child items should be generated after expanding!"),
                () => parent.IsExpanded = false,
                () => Assert.IsFalse(parent.IsExpanded, "Parent should finish collapsed!"));
        }

        /// <summary>
        /// Collapse a TreeViewItem when one of its descendents had selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Tag(Tags.RequiresFocus)]
        [Description("Collapse a TreeViewItem when one of its descendents had selection.")]
        [Priority(0)]
        public virtual void CollapseWithSelectedDescendent()
        {
            TreeView view = new TreeView();
            TreeViewItem item = new TreeViewItem { Header = "Item", IsExpanded = true };
            TreeViewItem first = new TreeViewItem { Header = "First", IsExpanded = true };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            first.Items.Add(second);
            item.Items.Add(first);
            view.Items.Add(item);

            TestAsync(
                5,
                view,
                () => second.IsSelected = true,
                () => item.IsExpanded = false,
                () => Assert.IsTrue(item.IsSelected, "Item should be selected after collapsing a selected descendent!"));
        }

        /// <summary>
        /// Expand a TreeViewItem with no child items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Expand a TreeViewItem with no child items.")]
        public virtual void ExpandNoItems()
        {
            TreeView view = new TreeView();
            TreeViewItem item = new TreeViewItem { Header = "Item" };
            view.Items.Add(item);

            TestAsync(
                view,
                () => item.IsExpanded = true,
                () => item.IsExpanded = false);
        }

        /// <summary>
        /// Expansion changes call the OnExpanded method.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Expansion changes call the OnExpanded method.")]
        public virtual void ExpansionCallsOnExpanded()
        {
            foreach (IOverriddenTreeViewItem overriddenItem in OverriddenTreeViewItemsToTest)
            {
                TreeView view = new TreeView();
                TreeViewItem item = overriddenItem as TreeViewItem;
                TreeViewItem first = new TreeViewItem { Header = "First" };
                TreeViewItem second = new TreeViewItem { Header = "Second" };
                view.Items.Add(item);
                item.Items.Add(first);
                item.Items.Add(second);
                MethodCallMonitor monitor = overriddenItem.ExpandedActions.CreateMonitor();

                TestTaskAsync(
                    view,
                    () =>
                    {
                        monitor.Reset();
                        item.IsExpanded = true;
                        monitor.AssertCalled("OnExpanded was not called when expanding the first item!");
                    });
            }

            EnqueueTestComplete();
        }

        /// <summary>
        /// Expansion changes call the OnCollapsed method.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Expansion changes call the OnCollapsed method.")]
        public virtual void ExpansionCallsOnCollapsed()
        {
            foreach (IOverriddenTreeViewItem overriddenItem in OverriddenTreeViewItemsToTest)
            {
                TreeView view = new TreeView();
                TreeViewItem item = overriddenItem as TreeViewItem;
                TreeViewItem first = new TreeViewItem { Header = "First" };
                TreeViewItem second = new TreeViewItem { Header = "Second" };
                view.Items.Add(item);
                item.Items.Add(first);
                item.Items.Add(second);
                MethodCallMonitor monitor = overriddenItem.CollapsedActions.CreateMonitor();

                TestTaskAsync(
                    view,
                    () => item.IsExpanded = true,
                    () =>
                    {
                        monitor.Reset();
                        item.IsExpanded = false;
                        monitor.AssertCalled("OnCollapsed was not called when collapsing the first item!");
                    });
            }

            EnqueueTestComplete();
        }

        /// <summary>
        /// Expansion changes raise the Expanded event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Expansion changes raise the Expanded event.")]
        [Priority(0)]
        public virtual void ExpansionRaisesExpandedEvent()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            first.Items.Add(second);
            int count = 0;
            first.Expanded += delegate { count++; };
            TestAsync(
                view,
                () => Assert.AreEqual(0, count, "Expanded should not have been fired yet!"),
                () => first.IsExpanded = true,
                () => Assert.AreEqual(1, count, "Expanded was not fired when expanding the first item!"),
                () => first.IsExpanded = false,
                () => Assert.AreEqual(1, count, "Expanded should not be fired when collapsing the first item!"),
                () => first.IsExpanded = true,
                () => Assert.AreEqual(2, count, "Expanded was not fired when expanding the first item again!"));
        }

        /// <summary>
        /// Expansion changes raise the Collapsed event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Expansion changes raise the Collapsed event.")]
        public virtual void ExpansionRaisesCollapsedEvent()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            first.Items.Add(second);
            int count = 0;
            first.Collapsed += delegate { count++; };
            TestAsync(
                view,
                () => first.IsExpanded = true,
                () => Assert.AreEqual(0, count, "Collapsed should not have been fired yet!"),
                () => first.IsExpanded = false,
                () => Assert.AreEqual(1, count, "Collapsed was not fired when collapsing the first item!"),
                () => first.IsExpanded = true,
                () => Assert.AreEqual(1, count, "Collapsed was not fired when collapsing the first item again!"));
        }

        /// <summary>
        /// Ensure we can succesfully unhook from the Expanded event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure we can succesfully unhook from the Expanded event.")]
        public virtual void ExpansionCanUnhookExpanded()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            first.Items.Add(second);
            int count = 0;
            RoutedEventHandler handler = delegate { count++; };
            first.Expanded += handler;
            TestAsync(
                view,
                () => Assert.AreEqual(0, count, "Expanded should not have been fired yet!"),
                () => first.IsExpanded = true,
                () => Assert.AreEqual(1, count, "Expanded was not fired when expanding the first item!"),
                () => first.IsExpanded = false,
                () => first.Expanded -= handler,
                () => second.IsExpanded = true,
                () => Assert.AreEqual(1, count, "Expanded was fired after unhooking!"));
        }

        /// <summary>
        /// Ensure we can succesfully unhook from the Collapsed event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure we can succesfully unhook from the Collapsed event.")]
        public virtual void ExpansionCanUnhookCollapsed()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            first.Items.Add(second);
            int count = 0;
            RoutedEventHandler handler = delegate { count++; };
            first.Collapsed += handler;
            TestAsync(
                view,
                () => Assert.AreEqual(0, count, "Collapsed should not have been fired yet!"),
                () => first.IsExpanded = true,
                () => first.IsExpanded = false,
                () => Assert.AreEqual(1, count, "Collapsed was not fired when collapsing the first item!"),
                () => first.IsExpanded = true,
                () => first.Collapsed -= handler,
                () => first.IsExpanded = false,
                () => Assert.AreEqual(1, count, "Collapsed was fired after unhooking!"));
        }

        /// <summary>
        /// Ensure clicking the ExpanderButton changes IsExpanded.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure clicking the ExpanderButton changes IsExpanded.")]
        [Bug("TODO: Why doesn't IToggleProvider raise the Click event?")]
        public virtual void ExpanderButtonClickTogglesIsExpanded()
        {
            TreeView view = new TreeView();
            TreeViewItem item = new TreeViewItem { Header = "Item", IsExpanded = true };
            TreeViewItem child = new TreeViewItem { Header = "Child" };
            view.Items.Add(item);
            item.Items.Add(child);
            ToggleButton ExpanderButton = null;
            AutomationPeer peer = null;
            IToggleProvider provider = null;

            TestAsync(
                5,
                view,
                () => ExpanderButton = item.GetVisualChild("ExpanderButton") as ToggleButton,
                () => Assert.IsNotNull(ExpanderButton, "Failed to find template part ExpanderButton!"),
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(ExpanderButton),
                () => provider = peer.GetPattern(PatternInterface.Toggle) as IToggleProvider,
                () => Assert.IsNotNull(provider, "IToggleProvider should not be null!"),
                () => provider.Toggle(),
                () => Assert.IsFalse(item.IsExpanded, "Item should be collapsed when clicking ExpanderButton!"),
                () => provider.Toggle(),
                () => Assert.IsTrue(item.IsExpanded, "Item should be expanded when clicking ExpanderButton again!"));
        }
        #endregion Expand

        #region Selection
        /// <summary>
        /// Focus selects a TreeViewItem.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Focus selects a TreeViewItem.")]
        [Tag(Tags.RequiresFocus)]
        public virtual void FocusSelects()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            view.Items.Add(second);

            TestAsync(
                view,
                () => first.Focus(),
                () => Assert.IsTrue(first.IsSelected, "First should be selected!"),
                () => second.Focus(),
                () => Assert.IsTrue(second.IsSelected, "Second should be selected!"),
                () => Assert.IsFalse(first.IsSelected, "First should not be selected now!"));
        }

        /// <summary>
        /// Losing focus should make the TreeViewItem selection inactive.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Losing focus should make the TreeViewItem selection inactive.")]
        [Tag(Tags.RequiresFocus)]
        public virtual void SelectionInactiveOnLostFocus()
        {
            StackPanel root = new StackPanel();
            TreeView view = new TreeView();
            TreeViewItem item = new TreeViewItem { Header = "Item" };
            view.Items.Add(item);
            Button other = new Button { Content = "Button" };
            root.Children.Add(view);
            root.Children.Add(other);

            TestAsync(
                root,
                () => Assert.IsFalse(item.IsSelected, "Item should not be selected initially!"),
                () => Assert.IsFalse(item.IsSelectionActive, "Item should not have inactive selection initially!"),
                () => item.Focus(),
                () => Assert.IsTrue(item.IsSelected, "Item should be selected after focus!"),
                () => Assert.IsTrue(item.IsSelectionActive, "Item should not have inactive selection after focus!"),
                () => other.Focus(),
                () => Assert.IsTrue(item.IsSelected, "Item should be selected after focusing another element!"),
                () => Assert.IsFalse(item.IsSelectionActive, "Item should have an inactive selection after focusing another element!"));
        }

        /// <summary>
        /// Selection changes call the OnSelected method.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Selection changes call the OnSelected method.")]
        public virtual void SelectionCallsOnSelected()
        {
            foreach (IOverriddenTreeViewItem overriddenItem in OverriddenTreeViewItemsToTest)
            {
                TreeView view = new TreeView();
                TreeViewItem item = overriddenItem as TreeViewItem;
                TreeViewItem first = new TreeViewItem { Header = "First" };
                TreeViewItem second = new TreeViewItem { Header = "Second" };
                view.Items.Add(item);
                item.Items.Add(first);
                item.Items.Add(second);
                MethodCallMonitor monitor = overriddenItem.SelectedActions.CreateMonitor();

                TestTaskAsync(
                    view,
                    () =>
                    {
                        monitor.Reset();
                        item.IsSelected = true;
                        monitor.AssertCalled("OnSelected was not called when selecting the first item!");
                    });
            }

            EnqueueTestComplete();
        }

        /// <summary>
        /// Selection changes call the OnUnselected method.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Selection changes call the OnUnselected method.")]
        public virtual void SelectionCallsOnUnselected()
        {
            foreach (IOverriddenTreeViewItem overriddenItem in OverriddenTreeViewItemsToTest)
            {
                TreeView view = new TreeView();
                TreeViewItem item = overriddenItem as TreeViewItem;
                TreeViewItem first = new TreeViewItem { Header = "First" };
                TreeViewItem second = new TreeViewItem { Header = "Second" };
                view.Items.Add(item);
                item.Items.Add(first);
                item.Items.Add(second);
                MethodCallMonitor monitor = overriddenItem.UnselectedActions.CreateMonitor();

                TestTaskAsync(
                    view,
                    () => item.IsSelected = true,
                    () =>
                    {
                        monitor.Reset();
                        first.IsSelected = true;
                        monitor.AssertCalled("OnSelected was not called when selecting the first item!");
                    });
            }

            EnqueueTestComplete();
        }

        /// <summary>
        /// Selection changes raise the Selected event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Selection changes raise the Selected event.")]
        [Priority(0)]
        public virtual void SelectionRaisesSelectedEvent()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            view.Items.Add(second);
            int count = 0;
            first.Selected += delegate { count++; };
            TestAsync(
                view,
                () => Assert.AreEqual(0, count, "Selected should not have been fired yet!"),
                () => first.IsSelected = true,
                () => Assert.AreEqual(1, count, "Selected was not fired when selecting the first item!"),
                () => second.IsSelected = true,
                () => Assert.AreEqual(1, count, "Selected should not be fired when selecting the second item!"),
                () => Assert.IsFalse(first.IsSelected, "The first item should no longer be selected!"),
                () => first.IsSelected = true,
                () => Assert.AreEqual(2, count, "Selected was not fired when selecting the first item again!"));
        }

        /// <summary>
        /// Selection changes raise the Unselected event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Selection changes raise the Unselected event.")]
        public virtual void SelectionRaisesUnselectedEvent()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            view.Items.Add(second);
            int count = 0;
            first.Unselected += delegate { count++; };
            TestAsync(
                view,
                () => Assert.AreEqual(0, count, "Unselected should not have been fired yet!"),
                () => first.IsSelected = true,
                () => Assert.AreEqual(0, count, "Unselected was not fired when selecting the first item!"),
                () => second.IsSelected = true,
                () => Assert.AreEqual(1, count, "Unselected should be fired when selecting the second item!"),
                () => Assert.IsFalse(first.IsSelected, "The first item should no longer be selected!"),
                () => first.IsSelected = true,
                () => Assert.AreEqual(1, count, "Unselected was not fired when selecting the first item again!"));
        }

        /// <summary>
        /// Ensure we can succesfully unhook from the Selected event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure we can succesfully unhook from the Selected event.")]
        public virtual void SelectionCanUnhookSelected()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            view.Items.Add(second);
            int count = 0;
            RoutedEventHandler handler = delegate { count++; };
            first.Selected += handler;
            TestAsync(
                view,
                () => Assert.AreEqual(0, count, "Selected should not have been fired yet!"),
                () => first.IsSelected = true,
                () => Assert.AreEqual(1, count, "Selected was not fired when selecting the first item!"),
                () => first.IsSelected = false,
                () => first.Selected -= handler,
                () => second.IsSelected = true,
                () => Assert.AreEqual(1, count, "Selected was fired after unhooking!"));
        }

        /// <summary>
        /// Ensure we can succesfully unhook from the Unselected event.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure we can succesfully unhook from the Unselected event.")]
        public virtual void SelectionCanUnhookUnselected()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            view.Items.Add(first);
            int count = 0;
            RoutedEventHandler handler = delegate { count++; };
            first.Unselected += handler;
            TestAsync(
                view,
                () => Assert.AreEqual(0, count, "Unselected should not have been fired yet!"),
                () => first.IsSelected = true,
                () => first.IsSelected = false,
                () => Assert.AreEqual(1, count, "Unselected was not fired when unselecting the first item!"),
                () => first.IsSelected = true,
                () => first.Unselected -= handler,
                () => first.IsSelected = false,
                () => Assert.AreEqual(1, count, "Unselected was fired after unhooking!"));
        }

        /// <summary>
        /// Ensure the ExpanderButton part activates selection when focused.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure the ExpanderButton part activates selection when focused.")]
        [Bug("TODO: Why isn't this activating selection?")]
        public virtual void ExpanderButtonFocusActivatesSelection()
        {
            StackPanel root = new StackPanel();
            TreeView view = new TreeView();
            TreeViewItem item = new TreeViewItem { Header = "Item" };
            Button button = new Button { Content = "Button" };
            view.Items.Add(item);
            root.Children.Add(view);
            root.Children.Add(button);
            ToggleButton ExpanderButton = null;

            TestAsync(
                5,
                root,
                () => item.Focus(),
                () => Assert.IsTrue(item.IsSelected, "Item should be selected!"),
                () => Assert.IsTrue(item.IsSelectionActive, "Selection should be active after focusing!"),
                () => button.Focus(),
                () => Assert.IsFalse(item.IsSelectionActive, "Selection should be inactive after focusing on something else!"),
                () => ExpanderButton = item.GetVisualChild("ExpanderButton") as ToggleButton,
                () => Assert.IsNotNull(ExpanderButton, "Failed to find template part ExpanderButton!"),
                () => ExpanderButton.Focus(),
                () => Assert.IsTrue(item.IsSelectionActive, "Selection should be active after focusing ExpanderButton!"));
        }
        #endregion Selection

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
                    "<controls:TreeViewItem x:Name='TVI' Background='{StaticResource SCBO}'>" +
                        "<controls:TreeViewItem.Resources>" +
                            "<SolidColorBrush x:Key='SCBP' Color='Purple'/>" +
                        "</controls:TreeViewItem.Resources>" +
                        "<controls:TreeViewItem.ItemsSource>" +
                            "<local:TestObjectCollection>" +
                                "<ContentControl x:Name='CC' Background='{StaticResource SCBO}' Foreground='{StaticResource SCBP}'/>" +
                            "</local:TestObjectCollection>" +
                        "</controls:TreeViewItem.ItemsSource>" +
                    "</controls:TreeViewItem>" +
                "</Grid>") as Panel;
            SolidColorBrush scbo = panel.Resources["SCBO"] as SolidColorBrush;
            TreeViewItem tvi = panel.FindName("TVI") as TreeViewItem;
            SolidColorBrush scbp = tvi.Resources["SCBP"] as SolidColorBrush;
            ContentControl cc = panel.FindName("CC") as ContentControl;
            if (null == cc)
            {
                cc = tvi.ItemsSource.OfType<ContentControl>().Single();
            }
            Assert.AreEqual(scbo, tvi.Background);
            Assert.AreEqual(scbo, cc.Background);
            Assert.AreEqual(scbp, cc.Foreground);
        }

        /// <summary>
        /// Verify that TreeViewItems can contain UIElement content.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewItems can contain UIElement content.")]
        [Bug("528257: TreeView: The App crashes when we add a button as TreeViewItem content", Fixed = true)]
        public virtual void CanContainUIElements()
        {
            TreeView view = new TreeView();
            view.Items.Add(new Button { Content = "Test" });
            view.Items.Add(new Ellipse { Fill = new SolidColorBrush(Colors.Red), Height = 20, Width = 20 });
            view.Items.Add(new TextBlock { Text = "Test" });

            TestAsync(
                view,
                () => Assert.IsInstanceOfType(view.Items[0], typeof(Button), "First item should be a Button!"));
        }
    }
}