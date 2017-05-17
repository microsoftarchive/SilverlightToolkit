// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for the ItemsControlExtensions.
    /// </summary>
    [TestClass]
    [Tag("ItemsControlExtensions")]
    [Tag("TreeView")]
    [Tag("TreeViewExtensions")]
    public partial class ItemsControlExtensionsTest : TestBase
    {
        /// <summary>
        /// Initializes a new instance of the ItemsControlExtensionsTest class.
        /// </summary>
        public ItemsControlExtensionsTest()
        {
        }

        #region GetItemsHost
        /// <summary>
        /// Ensure GetItemsHost throws an exception when passed null.
        /// </summary>
        [TestMethod]
        [Description("Ensure GetItemsHost throws an exception when passed null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetItemsHostThrowsOnNull()
        {
            ListBox control = null;
            control.GetItemsHost();
        }

        /// <summary>
        /// Get the ItemsHost of an ItemsControl.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get the ItemsHost of an ItemsControl.")]
        public virtual void GetItemsHostForItemsControl()
        {
            ItemsControl control = new ItemsControl { ItemsSource = new[] { 1, 2, 3, 4 } };
            Panel host = null;
            TestAsync(
                control,
                () => host = control.GetItemsHost(),
                () => Assert.IsNotNull(host, "ItemsHost not found!"));
        }

        /// <summary>
        /// Get the ItemsHost of an ListBox.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get the ItemsHost of an ListBox.")]
        public virtual void GetItemsHostForListBox()
        {
            ListBox control = new ListBox { ItemsSource = new[] { 1, 2, 3, 4 } };
            Panel host = null;
            TestAsync(
                control,
                () => host = control.GetItemsHost(),
                () => Assert.IsNotNull(host, "ItemsHost not found!"));
        }

        /// <summary>
        /// Get the ItemsHost of an TreeView.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get the ItemsHost of an TreeView.")]
        public virtual void GetItemsHostForTreeView()
        {
            TreeView control = new TreeView { ItemsSource = new[] { 1, 2, 3, 4 } };
            Panel host = null;
            TestAsync(
                control,
                () => host = control.GetItemsHost(),
                () => Assert.IsNotNull(host, "ItemsHost not found!"));
        }

        /// <summary>
        /// Get the ItemsHost of an TreeView.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get the ItemsHost of an TreeView.")]
        public virtual void GetItemsHostForTreeViewItem()
        {
            TreeViewItem item;
            TreeView control =
                new TreeViewBuilder()
                    .Items("first").Named(out item).Expand()
                        .Item("a")
                        .Item("b")
                        .Item("c")
                    .EndItems()
                    .Item("d");
            Panel host = null;

            TestAsync(
                control,
                () => host = item.GetItemsHost(),
                () => Assert.IsNotNull(host, "ItemsHost not found!"));
        }

        /// <summary>
        /// Try to get the ItemsHost when there isn't one in the template.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Try to get the ItemsHost when there isn't one in the template.")]
        public virtual void GetItemsHostWithNoHost()
        {
            ItemsControl control = new ItemsControl { ItemsSource = new[] { 1, 2, 3, 4, 5 } };
            XamlBuilder<ControlTemplate> builder = new XamlBuilder<ControlTemplate>
            {
                AttributeProperties = new Dictionary<string, string> { { "TargetType", "ItemsControl" } },
                Children = new List<XamlBuilder> { new XamlBuilder<TextBlock>() }
            };
            control.Template = builder.Load();
            Panel host = null;

            TestAsync(
                control,
                () => host = control.GetItemsHost(),
                () => Assert.IsNull(host, "Should not have found ItemsHost!"));
        }

        /// <summary>
        /// Try to get the ItemsHost when there are no items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Try to get the ItemsHost when there are no items.")]
        [Bug("79921: Toolkit test GetItemsHost is failing due to Silverlight behavioral difference vs. WPF")]
        public virtual void GetItemsHostWithNoItems()
        {
            ListBox control = new ListBox();
            Panel host = null;

            TestAsync(
                control,
                () => host = control.GetItemsHost(),
                () => Assert.IsNull(host, "Should not have found ItemsHost!"));
        }

        /// <summary>
        /// Try to get the ItemsHost when not in the visual tree.
        /// </summary>
        [TestMethod]
        [Description("Try to get the ItemsHost when not in the visual tree.")]
        public virtual void GetItemsHostNotInVisualTree()
        {
            ListBox control = new ListBox { ItemsSource = new[] { 1, 2, 3, 4, 5 } };
            Panel host = control.GetItemsHost();
            Assert.IsNull(host, "Should not have found ItemsHost!");
        }

        /// <summary>
        /// Ensure that GetItemsHost returns the correct Panel.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that GetItemsHost returns the correct Panel.")]
        public virtual void GetItemsHostIsCorrectPanel()
        {
            string name = "DesiredPanel";
            ItemsControl control = new ItemsControl { ItemsSource = new[] { 1, 2, 3, 4, 5 } };
            Panel host = null;
            XamlBuilder<ItemsPanelTemplate> builder = new XamlBuilder<ItemsPanelTemplate>
            {
                Children = new List<XamlBuilder>
                {
                    new XamlBuilder<StackPanel> { Name = name }
                }
            };
            control.ItemsPanel = builder.Load();

            TestAsync(
                control,
                () => host = control.GetItemsHost(),
                () => Assert.IsNotNull(host, "Failed to find ItemsHost!"),
                () => Assert.AreEqual(name, host.Name, "ItemsHost name did not match!"));
        }
        #endregion GetItemsHost

        #region GetScrollHost
        /// <summary>
        /// Ensure GetScrollHost throws an exception when given a null
        /// ItemsControl.
        /// </summary>
        [TestMethod]
        [Description("Ensure GetScrollHost throws an exception when given a null ItemsControl.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetScrollHostThrowsOnNull()
        {
            ItemsControl control = null;
            control.GetScrollHost();
        }

        /// <summary>
        /// Get the ScrollHost from a ListBox.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get the ScrollHost from a ListBox.")]
        public virtual void GetScrollHostFromListBox()
        {
            ListBox control = new ListBox { ItemsSource = new[] { 1, 2, 3, 4, 5 } };
            ScrollViewer host = null;

            TestAsync(
                control,
                () => host = control.GetScrollHost(),
                () => Assert.IsNotNull(host, "ScrollHost not found!"));
        }

        /// <summary>
        /// Get the ScrollHost from a TreeViewItem.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get the ScrollHost from a TreeViewItem.")]
        public virtual void GetScrollHostFromTreeView()
        {
            TreeView view = new TreeView { ItemsSource = new[] { 1, 2, 3, 4, 5 } };
            ScrollViewer host = null;

            TestAsync(
                view,
                () => host = view.GetScrollHost(),
                () => Assert.IsNotNull("ScrollHost not found!"));
        }

        /// <summary>
        /// Ensure we get no ScrollHost when the control doesn't have one in its template.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure we get no ScrollHost when the control doesn't have one in its template.")]
        public virtual void NoScrollHostOnTreeViewItem()
        {
            TreeViewItem item = new TreeViewItem { ItemsSource = new[] { 1, 2, 3, 4, 5 } };
            ScrollViewer host = null;

            TestAsync(
                item,
                () => host = item.GetScrollHost(),
                () => Assert.IsNull(host, "TreeViewItem should not have a ScrollHost!"));
        }

        /// <summary>
        /// Try to get the ScrollHost when there isn't an ItemsHost in the
        /// template.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Try to get the ScrollHost when there isn't an ItemsHost in the template.")]
        public virtual void GetScrollHostWithNoItemsHost()
        {
            ListBox control = new ListBox { ItemsSource = new[] { 1, 2, 3, 4, 5 } };
            XamlBuilder<ControlTemplate> builder = new XamlBuilder<ControlTemplate>
            {
                AttributeProperties = new Dictionary<string, string> { { "TargetType", "ListBox" } },
                Children = new List<XamlBuilder> { new XamlBuilder<TextBlock>() }
            };
            control.Template = builder.Load();
            ScrollViewer host = null;

            TestAsync(
                control,
                () => host = control.GetScrollHost(),
                () => Assert.IsNull(host, "Should not have found ScrollHost!"));
        }

        /// <summary>
        /// Try to get the ScrollHost when there are no items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Try to get the ScrollHost when there are no items.")]
        [Bug("79921: Toolkit test GetItemsHost is failing due to Silverlight behavioral difference vs. WPF")]
        public virtual void GetScrollHostWithNoItems()
        {
            ListBox control = new ListBox();
            ScrollViewer host = null;

            TestAsync(
                control,
                () => host = control.GetScrollHost(),
                () => Assert.IsNull(host, "Should not have found ScrollHost!"));
        }

        /// <summary>
        /// Try to get the ScrollHost when not in the visual tree.
        /// </summary>
        [TestMethod]
        [Description("Try to get the ScrollHost when not in the visual tree.")]
        public virtual void GetScrollHostNotInVisualTree()
        {
            ListBox control = new ListBox { ItemsSource = new[] { 1, 2, 3, 4, 5 } };
            ScrollViewer host = control.GetScrollHost();
            Assert.IsNull(host, "Should not have found ScrollHost!");
        }
        #endregion GetScrollHost

        #region GetContainers
        /// <summary>
        /// Ensure GetContainers throws an exception when passed null.
        /// </summary>
        [TestMethod]
        [Description("Ensure GetContainers throws an exception when passed null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetContainersThrowsOnNull()
        {
            ItemsControl control = null;
            control.GetContainers();
        }

        /// <summary>
        /// Ensure GetContainers throws an exception when passed null.
        /// </summary>
        [TestMethod]
        [Description("Ensure GetContainers throws an exception when passed null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetContainersGenericThrowsOnNull()
        {
            ListBox control = null;
            control.GetContainers<ListBox>();
        }

        /// <summary>
        /// Try to get the containers when not in the visual tree.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Try to get the containers when not in the visual tree.")]
        public virtual void GetContainersNoItems()
        {
            ListBox control = new ListBox();
            IEnumerable<DependencyObject> containers = null;
            IEnumerable<ListBoxItem> items = null;

            TestAsync(
                control,
                () => containers = control.GetContainers(),
                () => items = control.GetContainers<ListBoxItem>(),
                () => Assert.IsNotNull(containers, "containers should not be null!"),
                () => Assert.AreEqual(0, containers.Count(), "containers should be empty!"),
                () => Assert.IsNotNull(items, "items should not be null!"),
                () => Assert.AreEqual(0, items.Count(), "items should be empty!"));
        }

        /// <summary>
        /// Verify the basic GetContainers scenarios work.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the basic GetContainers scenarios work.")]
        [Priority(0)]
        public virtual void GetContainers()
        {
            ListBox control = new ListBox { ItemsSource = new[] { 1, 2, 3, 4, 5 } };
            IEnumerable<DependencyObject> containers = null;
            IEnumerable<ListBoxItem> items = null;
            List<DependencyObject> allContainers = null;
            List<ListBoxItem> allItems = null;

            TestAsync(
                control,
                () => containers = control.GetContainers(),
                () => items = control.GetContainers<ListBoxItem>(),
                () => Assert.IsNotNull(containers, "containers should not be null!"),
                () => allContainers = new List<DependencyObject>(containers),
                () => Assert.AreEqual(5, allContainers.Count, "containers should have 5 items!"),
                () => Assert.IsNotNull(items, "items should not be null!"),
                () => allItems = new List<ListBoxItem>(items),
                () => Assert.AreEqual(5, allItems.Count, "items should have 5 items!"),
                () =>
                {
                    for (int i = 0; i < allItems.Count; i++)
                    {
                        Assert.AreEqual(allItems[i], allContainers[i], "Container {0} does not match item {0}!", i);
                    }
                },
                () =>
                {
                    for (int i = 0; i < allItems.Count; i++)
                    {
                        Assert.AreEqual(i + 1, control.ItemContainerGenerator.ItemFromContainer(allItems[i]), "Container {0} does not map to the correct item!", i);
                    }
                });
        }

        /// <summary>
        /// Get the containers using the wrong type.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get the containers using the wrong type.")]
        public virtual void GetContainersWrongType()
        {
            ListBox control = new ListBox { ItemsSource = new[] { 1, 2, 3, 4, 5 } };

            TestAsync(
                control,
                () => Assert.IsFalse(control.GetContainers<TreeViewItem>().Any(t => t != null), "All items should have been null!"));
        }

        /// <summary>
        /// Ensure that the GetContainers does not recurse on a hierarchical
        /// ItemsControl.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the GetContainers does not recurse on a hierarchical ItemsControl.")]
        public virtual void GetContainersDoesNotRecurse()
        {
            TreeView view = new TreeViewBuilder()
                .Items("first").Expand()
                    .Item("a")
                    .Item("b")
                    .Item("c")
                .EndItems()
                .Items("second").Expand()
                    .Item("1")
                    .Item("2")
                    .Item("3")
                .EndItems();

            ItemsControl control = view as ItemsControl;
            TestAsync(
                control,
                () => Assert.AreEqual(2, control.GetContainers().Count(), "Only expecting two containers!"));
        }
        #endregion GetContainers

        #region GetContainersAndItems
        /// <summary>
        /// Ensure GetItemsAndContainers throws an exception when passed null.
        /// </summary>
        [TestMethod]
        [Description("Ensure GetItemsAndContainers throws an exception when passed null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetItemsAndContainersThrowsOnNull()
        {
            ItemsControl control = null;
            control.GetItemsAndContainers();
        }

        /// <summary>
        /// Ensure GetItemsAndContainers throws an exception when passed null.
        /// </summary>
        [TestMethod]
        [Description("Ensure GetItemsAndContainers throws an exception when passed null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetItemsAndContainersGenericThrowsOnNull()
        {
            ListBox control = null;
            control.GetItemsAndContainers<ListBox>();
        }

        /// <summary>
        /// Try to get the containers when not in the visual tree.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Try to get the containers when not in the visual tree.")]
        public virtual void GetItemsAndContainersNoItems()
        {
            ListBox control = new ListBox();
            IEnumerable<KeyValuePair<object, DependencyObject>> containers = null;
            IEnumerable<KeyValuePair<object, ListBoxItem>> typedContainers = null;

            TestAsync(
                control,
                () => containers = control.GetItemsAndContainers(),
                () => typedContainers = control.GetItemsAndContainers<ListBoxItem>(),
                () => Assert.IsNotNull(containers, "containers should not be null!"),
                () => Assert.AreEqual(0, containers.Count(), "containers should be empty!"),
                () => Assert.IsNotNull(typedContainers, "items should not be null!"),
                () => Assert.AreEqual(0, typedContainers.Count(), "items should be empty!"));
        }

        /// <summary>
        /// Verify the basic GetItemsAndContainers scenarios work.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the basic GetItemsAndContainers scenarios work.")]
        [Priority(0)]
        public virtual void GetItemsAndContainers()
        {
            ListBox control = new ListBox { ItemsSource = new[] { 1, 2, 3, 4, 5 } };
            IEnumerable<KeyValuePair<object, DependencyObject>> containers = null;
            IEnumerable<KeyValuePair<object, ListBoxItem>> typedContainers = null;
            List<KeyValuePair<object, DependencyObject>> allContainers = null;
            List<KeyValuePair<object, ListBoxItem>> allTypedContainers = null;

            TestAsync(
                control,
                () => containers = control.GetItemsAndContainers(),
                () => typedContainers = control.GetItemsAndContainers<ListBoxItem>(),
                () => Assert.IsNotNull(containers, "containers should not be null!"),
                () => allContainers = new List<KeyValuePair<object, DependencyObject>>(containers),
                () => Assert.AreEqual(5, allContainers.Count, "containers should have 5 items!"),
                () => Assert.IsNotNull(typedContainers, "items should not be null!"),
                () => allTypedContainers = new List<KeyValuePair<object, ListBoxItem>>(typedContainers),
                () => Assert.AreEqual(5, allTypedContainers.Count, "items should have 5 items!"),
                () =>
                {
                    for (int i = 0; i < allTypedContainers.Count; i++)
                    {
                        Assert.AreEqual(allTypedContainers[i].Value, allContainers[i].Value, "Container {0} does not match typed container {0}!", i);
                    }
                },
                () =>
                {
                    for (int i = 0; i < allTypedContainers.Count; i++)
                    {
                        Assert.AreEqual(allTypedContainers[i].Key, control.ItemContainerGenerator.ItemFromContainer(allTypedContainers[i].Value), "Container {0} does not map to the correct item!", i);
                    }
                });
        }

        /// <summary>
        /// Get the containers using the wrong type.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get the containers using the wrong type.")]
        public virtual void GetItemsAndContainersWrongType()
        {
            ListBox control = new ListBox { ItemsSource = new[] { 1, 2, 3, 4, 5 } };

            TestAsync(
                control,
                () => Assert.IsFalse(control.GetItemsAndContainers<TreeViewItem>().Any(p => p.Value != null), "All containers should have been null!"));
        }

        /// <summary>
        /// Ensure that the GetContainers does not recurse on a hierarchical
        /// ItemsControl.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the GetItemsAndContainers does not recurse on a hierarchical ItemsControl.")]
        public virtual void GetItemsAndContainersDoesNotRecurse()
        {
            TreeView view = new TreeViewBuilder()
                .Items("first").Expand()
                    .Item("a")
                    .Item("b")
                    .Item("c")
                .EndItems()
                .Items("second").Expand()
                    .Item("1")
                    .Item("2")
                    .Item("3")
                .EndItems();

            ItemsControl control = view as ItemsControl;
            TestAsync(
                control,
                () => Assert.AreEqual(2, control.GetItemsAndContainers<TreeViewItem>().Count(), "Only expecting two containers!"));
        }
        #endregion GetItemsAndContainers
    }
}