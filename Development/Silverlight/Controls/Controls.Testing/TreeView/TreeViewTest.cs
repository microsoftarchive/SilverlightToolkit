// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Windows.Controls.Testing
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
            LinearGradientBrush brush = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 0.0),
                EndPoint = new Point(0.5, 1.0)
            };
            brush.GradientStops.Add(
                new GradientStop
                {
                    Color = System.Windows.Media.Color.FromArgb(0xFF, 0xA3, 0xAE, 0xB9),
                    Offset = 0.0
                });
            brush.GradientStops.Add(
                new GradientStop
                {
                    Color = System.Windows.Media.Color.FromArgb(0xFF, 0x83, 0x99, 0xA9),
                    Offset = 0.375
                });
            brush.GradientStops.Add(
                new GradientStop
                {
                    Color = System.Windows.Media.Color.FromArgb(0xFF, 0x71, 0x85, 0x97),
                    Offset = 0.375
                });
            brush.GradientStops.Add(
                new GradientStop
                {
                    Color = System.Windows.Media.Color.FromArgb(0xFF, 0x61, 0x75, 0x84),
                    Offset = 1.0
                });
            BorderBrushProperty.DefaultValue = brush;
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
    }
}