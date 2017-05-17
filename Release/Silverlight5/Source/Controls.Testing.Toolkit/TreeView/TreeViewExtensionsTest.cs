// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItemAndContainer = System.Collections.Generic.KeyValuePair<object, System.Windows.Controls.TreeViewItem>;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// TreeViewItemCheckBox unit tests.
    /// </summary>
    [TestClass]
    [Tag("TreeView")]
    [Tag("TreeViewItem")]
    [Tag("TreeViewExtensions")]
    public partial class TreeViewExtensionsTest : TestBase
    {
        /// <summary>
        /// Number of milliseconds to wait when we expand the entire TreeView.
        /// </summary>
        private const int ExpandAllDelay = 250;

        /// <summary>
        /// Initializes a new instance of the TreeViewExtensionsTest class.
        /// </summary>
        public TreeViewExtensionsTest()
        {
        }

        #region AssertSequence
        /// <summary>
        /// Assert that the actual sequence matches the expected sequence.
        /// </summary>
        /// <typeparam name="T">Type of elements in the sequences.</typeparam>
        /// <param name="expected">The expected sequence.</param>
        /// <param name="actual">The actual sequence.</param>
        /// <param name="message">Assertion message.</param>
        /// <param name="args">Assertion message arguments.</param>
        private static void AssertSequence<T>(IEnumerable<T> expected, IEnumerable<T> actual, string message, params object[] args)
        {
            TestExtensions.AssertSequencesEqual(expected, actual, ElementToString, message, args);
        }

        /// <summary>
        /// Convert a sequence element to a string.
        /// </summary>
        /// <typeparam name="T">Type of the element.</typeparam>
        /// <param name="element">The element.</param>
        /// <returns>A string representation.</returns>
        private static string ElementToString<T>(T element)
        {
            TreeViewItem item = element as TreeViewItem;
            if (item != null)
            {
                return ElementToString<object>(item.Header);
            }
            else
            {
                Hierarchy hierarchy = element as Hierarchy;
                if (hierarchy != null)
                {
                    return hierarchy.Element;
                }
                else if (element is ItemAndContainer)
                {
                    ItemAndContainer pair = (ItemAndContainer)(object)element;
                    return ElementToString<object>(pair.Key);
                }
                else if (element != null)
                {
                    return element.ToString();
                }
                else
                {
                    return "(null)";
                }
            }            
        }

        /// <summary>
        /// Create a new sequence (using this because StyleCop doesn't like
        /// inline array creation involving curly braces on the same line).
        /// </summary>
        /// <typeparam name="T">Type of elements in the sequence.</typeparam>
        /// <param name="elements">The elements of the sequence.</param>
        /// <returns>The sequence.</returns>
        /// <remarks>
        /// (define list (lambda l l)).
        /// </remarks>
        private static IEnumerable<T> Sequence<T>(params T[] elements)
        {
            return elements ?? Enumerable.Empty<T>();
        }
        #endregion AssertSequence

        #region Get Parents
        /// <summary>
        /// Verify that GetParentTreeView throws an exception for a null item.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetParentTreeView throws an exception for a null item.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetParentTreeViewThrows()
        {
            TreeViewItem item = null;
            item.GetParentTreeView();
        }

        /// <summary>
        /// Verify that GetParentTreeViewItem throws an exception for a null
        /// item.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetParentTreeViewItem throws an exception for a null item.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetParentTreeViewItemThrows()
        {
            TreeViewItem item = null;
            item.GetParentTreeViewItem();
        }

        /// <summary>
        /// Verify GetParentTreeView and GetParentTreeViewItem behavior.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify GetParentTreeView and GetParentTreeViewItem behavior.")]
        public virtual void GetParents()
        {
            TreeViewItem first, second, third, fourth, fifth, sixth, seventh;
            TreeView view = new TreeViewBuilder()
                .Items("first").Named(out first).Expand()
                    .Items("second").Named(out second).Expand()
                        .Items("third").Named(out third).Expand()
                            .Item("fourth").Named(out fourth)
                        .EndItems()
                        .Item("fifth").Named(out fifth)
                    .EndItems()
                    .Item("sixth").Named(out sixth)
                .EndItems()
                .Item("seventh").Named(out seventh);

            TestAsync(
                view,
                () => Assert.AreEqual(view, first.GetParentTreeView(), "first failed to find its parent TreeView"),
                () => Assert.AreEqual(view, second.GetParentTreeView(), "second failed to find its parent TreeView"),
                () => Assert.AreEqual(view, third.GetParentTreeView(), "third failed to find its parent TreeView"),
                () => Assert.AreEqual(view, fourth.GetParentTreeView(), "fourth failed to find its parent TreeView"),
                () => Assert.AreEqual(view, fifth.GetParentTreeView(), "fifth failed to find its parent TreeView"),
                () => Assert.AreEqual(view, sixth.GetParentTreeView(), "sixth failed to find its parent TreeView"),
                () => Assert.AreEqual(view, seventh.GetParentTreeView(), "seventh failed to find its parent TreeView"),
                () => Assert.AreEqual(null, first.GetParentTreeViewItem(), "failed to find parent TreeViewItem of first"),
                () => Assert.AreEqual(first, second.GetParentTreeViewItem(), "failed to find parent TreeViewItem of second"),
                () => Assert.AreEqual(second, third.GetParentTreeViewItem(), "failed to find parent TreeViewItem of third"),
                () => Assert.AreEqual(third, fourth.GetParentTreeViewItem(), "failed to find parent TreeViewItem of fourth"),
                () => Assert.AreEqual(second, fifth.GetParentTreeViewItem(), "failed to find parent TreeViewItem of fifth"),
                () => Assert.AreEqual(first, sixth.GetParentTreeViewItem(), "failed to find parent TreeViewItem of sixth"),
                () => Assert.AreEqual(null, seventh.GetParentTreeViewItem(), "failed to find parent TreeViewItem of seventh"));
        }

        /// <summary>
        /// Try to get the parents when not in the visual tree.
        /// </summary>
        [TestMethod]
        [Description("Try to get the parents when not in the visual tree.")]
        public virtual void GetParentsNotInVisualTree()
        {
            TreeViewItem first, second;
            new TreeViewBuilder()
                .Items("first").Named(out first).Expand()
                    .Item("second").Named(out second)
                .EndItems();

            Assert.IsNull(first.GetParentTreeView(), "first found its parent TreeView");
            Assert.IsNull(second.GetParentTreeView(), "second found its parent TreeView");
            Assert.IsNull(first.GetParentTreeViewItem(), "Found the wrong parent TreeViewItem of first");
            Assert.IsNull(second.GetParentTreeViewItem(), "Found parent TreeViewItem of second");
        }
        #endregion

        #region Get Items And Containers
        /// <summary>
        /// Verify that GetContainers throws when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetContainers throws when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetContainersTreeViewThrows()
        {
            TreeView view = null;
            view.GetContainers();
        }

        /// <summary>
        /// Verify that GetContainers throws when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetContainers throws when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetContainersRecursiveTreeViewThrows()
        {
            TreeView view = null;
            view.GetDescendantContainers();
        }

        /// <summary>
        /// Verify that GetContainers throws when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetContainers throws when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetContainersTreeViewItemThrows()
        {
            TreeViewItem item = null;
            item.GetContainers();
        }

        /// <summary>
        /// Verify that GetContainers throws when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetContainers throws when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetContainersRecursiveTreeViewItemThrows()
        {
            TreeViewItem item = null;
            item.GetContainers();
        }

        /// <summary>
        /// Verify that GetItemsAndContainers throws when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetItemsAndContainers throws when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetItemsAndContainersTreeViewThrows()
        {
            TreeView view = null;
            view.GetItemsAndContainers();
        }

        /// <summary>
        /// Verify that GetItemsAndContainers throws when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetItemsAndContainers throws when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetItemsAndContainersRecursiveTreeViewThrows()
        {
            TreeView view = null;
            view.GetDescendantItemsAndContainers();
        }

        /// <summary>
        /// Verify that GetItemsAndContainers throws when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetItemsAndContainers throws when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetItemsAndContainersTreeViewItemThrows()
        {
            TreeViewItem item = null;
            item.GetItemsAndContainers();
        }

        /// <summary>
        /// Verify that GetItemsAndContainers throws when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetItemsAndContainers throws when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetItemsAndContainersRecursiveTreeViewItemThrows()
        {
            TreeViewItem item = null;
            item.GetItemsAndContainers();
        }

        /// <summary>
        /// Verify that GetSiblingItemsAndContainers throws when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetSiblingItemsAndContainers throws when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetSiblingItemsAndContainersThrows()
        {
            TreeViewItem item = null;
            item.GetSiblingItemsAndContainers();
        }

        /// <summary>
        /// Verify that GetContainers returns the correct containers.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that GetContainers retruns the correct containers.")]
        [Priority(0)]
        public virtual void GetContainers()
        {
            TreeViewItem t1, t2, t3, t4, t5;
            TreeView view = new TreeViewBuilder()
                .Items("first").Named(out t1).Expand()
                    .Items("second").Named(out t2).Expand()
                        .Item("third").Named(out t3)
                    .EndItems()
                    .Item("fourth").Named(out t4)
                .EndItems()
                .Item("fifth").Named(out t5);

            TestAsync(
                view,
                () => AssertSequence(
                        Sequence(t1, t5),
                        view.GetContainers(),
                        "view's direct containers"),
                () => AssertSequence(
                        Sequence(t1, t5, t2, t4, t3),
                        view.GetDescendantContainers(),
                        "view's descendent containers"),
                () => AssertSequence(
                        Sequence(t2, t4),
                        t1.GetContainers(),
                        "t1's direct containers"),
                () => AssertSequence(
                        Sequence(t2, t4, t3),
                        t1.GetDescendantContainers(),
                        "t1's descendent containers"));
        }

        /// <summary>
        /// Verify that GetItemsAndContainers returns the correct values.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that GetItemsAndContainers retruns the correct values.")]
        [Priority(0)]
        public virtual void GetItemsAndContainers()
        {
            TreeView view = new TreeView { ItemTemplate = Hierarchy.GetDataTemplate() };

            Hierarchy h1, h2, h3, h4, h5;
            view.ItemsSource = new[]
            {
                new Hierarchy("first").Named(out h1)
                    .Items("second").Named(out h2)
                        .Item("third").Named(out h3)
                    .EndItems()
                    .Item("fourth").Named(out h4),
                new Hierarchy("fifth").Named(out h5)
            };

            TestAsync(
                ExpandAllDelay,
                view,
                () => view.ExpandAll(),
                () => view.ExpandAll(),
                () => AssertSequence(
                        Sequence(h1, h5),
                        view.GetItemsAndContainers().Select(p => p.Key as Hierarchy),
                        "view's direct items and containers"),
                () => AssertSequence(
                        Sequence(h2, h4),
                        view.GetContainerFromItem(h1).GetItemsAndContainers().Select(p => p.Key as Hierarchy),
                        "h1's direct items and containers"),
                () => AssertSequence(
                        Sequence(h1, h5, h2, h4, h3),
                        view.GetDescendantItemsAndContainers().Select(p => p.Key as Hierarchy),
                        "view's descendent items and containers"),
                () => AssertSequence(
                        Sequence(h2, h4, h3),
                        view.GetContainerFromItem(h1).GetDescendantItemsAndContainers().Select(p => p.Key as Hierarchy),
                        "h1's descendent items and containers"));
        }

        /// <summary>
        /// Get the items and containers when the hierarchy is only partially in
        /// the visual tree.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get the items and containers when the hierarchy is only partially in the visual tree.")]
        public virtual void GetItemsAndContainersPartiallyInVisualTree()
        {
            TreeView view = new TreeView { ItemTemplate = Hierarchy.GetDataTemplate() };

            Hierarchy h1, h2, h3, h4, h5;
            view.ItemsSource = new[]
            {
                new Hierarchy("first").Named(out h1)
                    .Items("second").Named(out h2)
                        .Item("third").Named(out h3)
                    .EndItems()
                    .Item("fourth").Named(out h4),
                new Hierarchy("fifth").Named(out h5)
            };

            TestAsync(
                25,
                view,
                () => view.GetContainerFromItem(h1).IsExpanded = true,
                () => AssertSequence(
                        Sequence(h1, h5, h2, h4),
                        view.GetDescendantItemsAndContainers().Select(p => p.Key as Hierarchy),
                        "view's visible descendents"));
        }

        /// <summary>
        /// Get the items and containers that have been in the visual tree at
        /// some point.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get the items and containers that have been in the visual tree at some point.")]
        public virtual void GetItemsAndContainersInAndOutOfVisualTree()
        {
            TreeView view = new TreeView { ItemTemplate = Hierarchy.GetDataTemplate() };

            Hierarchy h1, h2, h3, h4, h5;
            view.ItemsSource = new[]
            {
                new Hierarchy("first").Named(out h1)
                    .Items("second").Named(out h2)
                        .Item("third").Named(out h3)
                    .EndItems()
                    .Item("fourth").Named(out h4),
                new Hierarchy("fifth").Named(out h5)
            };

            TestAsync(
                25,
                view,
                () => view.GetContainerFromItem(h1).IsExpanded = true,
                () => view.GetContainerFromItem(h1).IsExpanded = false,
                () => AssertSequence(
                        Sequence(h1, h5, h2, h4),
                        view.GetDescendantItemsAndContainers().Select(p => p.Key as Hierarchy),
                        "view's visible descendents"));
        }

        /// <summary>
        /// Get the containers before the TreeViewItem enters the visual tree.
        /// </summary>
        [TestMethod]
        [Description("Get the containers before the TreeViewItem enters the visual tree.")]
        public virtual void GetContainersNotInVisualTree()
        {
            TreeView view = new TreeViewBuilder()
                .Items("first")
                    .Item("Second")
                    .Item("Third")
                .EndItems()
                .Item("Fourth");

            AssertSequence(
                Enumerable.Empty<TreeViewItem>(),
                view.GetDescendantContainers(),
                "view's descendents");
        }

        /// <summary>
        /// Get the items and containers before the TreeView's entered the
        /// visual tree.
        /// </summary>
        [TestMethod]
        [Description("Get the items and containers before the TreeView's entered the visual tree.")]
        public virtual void GetItemsAndContainersNotInVisualTree()
        {
            TreeView view = new TreeView { ItemTemplate = Hierarchy.GetDataTemplate() };
            view.ItemsSource = new[]
            {
                new Hierarchy("first")
                    .Items("second")
                        .Item("third")
                    .EndItems()
                    .Item("fourth"),
                new Hierarchy("fifth")
            };

            AssertSequence(
                Enumerable.Empty<Hierarchy>(),
                view.GetItemsAndContainers().Select(p => p.Key as Hierarchy),
                "view's descendents");
        }

        /// <summary>
        /// Verify that GetSiblings correctly returns the siblings.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that GetSiblings correctly returns the siblings.")]
        public virtual void GetSiblings()
        {
            TreeViewItem t1, t2, t3, t4, t5;
            TreeView view = new TreeViewBuilder()
                .Items("first").Named(out t1).Expand()
                    .Items("second").Named(out t2).Expand()
                        .Item("third").Named(out t3)
                    .EndItems()
                    .Item("fourth").Named(out t4)
                .EndItems()
                .Item("fifth").Named(out t5);

            TestAsync(
                view,
                () => AssertSequence(
                        Sequence(t5),
                        t1.GetSiblingItemsAndContainers().Select(p => p.Value),
                        "t1's siblings"),
                () => AssertSequence(
                        Sequence(t4),
                        t2.GetSiblingItemsAndContainers().Select(p => p.Value),
                        "t2's siblings"),
                () => AssertSequence(
                        Sequence<TreeViewItem>(),
                        t3.GetSiblingItemsAndContainers().Select(p => p.Value),
                        "t3's direct containers"),
                () => AssertSequence(
                        Sequence(t2),
                        t4.GetSiblingItemsAndContainers().Select(p => p.Value),
                        "t4's direct containers"),
                () => AssertSequence(
                        Sequence(t1),
                        t5.GetSiblingItemsAndContainers().Select(p => p.Value),
                        "t4's direct containers"));
        }
        #endregion Get Items And Containers

        #region Get Container(s) from Item
        /// <summary>
        /// Verify that GetContainersFromItem throws when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetContainersFromItem throws when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetContainersFromItemThrows()
        {
            TreeView view = null;
            view.GetContainersFromItem(null);
        }

        /// <summary>
        /// Verify that GetContainersFromItem throws when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetContainersFromItem throws when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetContainerFromItemItemThrows()
        {
            TreeView view = null;
            view.GetContainerFromItem(null);
        }

        /// <summary>
        /// Get the containers associated with an item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get the containers associated with an item.")]
        public virtual void GetContainersFromItem()
        {
            TreeViewItem t1, t2, t3, t4, t5;
            TreeView view = new TreeViewBuilder()
                .Items("first").Named(out t1).Expand()
                    .Items("second").Named(out t2).Expand()
                        .Item("third").Named(out t3)
                        .Item("fourth").Named(out t4)
                    .EndItems()
                .EndItems()
                .Item("fifth").Named(out t5);

            TestAsync(
                view,
                () => AssertSequence(
                        Sequence(t1),
                        view.GetContainersFromItem(t1),
                        "first"),
                () => Assert.AreEqual(t1, view.GetContainerFromItem(t1), "first container"),
                () => AssertSequence(
                        Sequence(t2),
                        view.GetContainersFromItem(t2),
                        "second"),
                () => Assert.AreEqual(t2, view.GetContainerFromItem(t2), "second container"),
                () => AssertSequence(
                        Sequence(t3),
                        view.GetContainersFromItem(t3),
                        "third"),
                () => Assert.AreEqual(t3, view.GetContainerFromItem(t3), "third container"),
                () => AssertSequence(
                        Sequence(t4),
                        view.GetContainersFromItem(t4),
                        "fourth"),
                () => Assert.AreEqual(t4, view.GetContainerFromItem(t4), "fourth container"),
                () => AssertSequence(
                        Sequence(t5),
                        view.GetContainersFromItem(t5),
                        "fifth"),
                () => Assert.AreEqual(t5, view.GetContainerFromItem(t5), "fifth container"),
                () => AssertSequence(
                        Sequence<TreeViewItem>(),
                        view.GetContainersFromItem(null),
                        "null"),
                () => Assert.IsNull(view.GetContainerFromItem(null), "null container"));
        }

        /// <summary>
        /// Get multiple containers associated with the same item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get multiple containers associated with the same item.")]
        public virtual void GetMultipleContainersFromItem()
        {
            TreeView view = new TreeView
            {
                ItemsSource = new[] { 12 },
                ItemTemplate = FactorizationValueConverter.GetDataTemplate()
            };

            TestAsync(
                ExpandAllDelay,
                view,
                () => view.ExpandAll(),
                () => view.ExpandAll(),
                () => Assert.AreEqual(3, view.GetContainersFromItem(2).Count(), "Unexpected number of containers generated!"),
                () => Assert.AreEqual(2, view.GetContainerFromItem(2).GetItem(), "First container found should map to item!"));                    
        }

        /// <summary>
        /// Try to get a container for an item that doesn't exist.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Try to get a container for an item that doesn't exist.")]
        public virtual void GetContainersFromItemThatDoesNotExist()
        {
            TreeView view = new TreeView
            {
                ItemsSource = new[] { 12 },
                ItemTemplate = FactorizationValueConverter.GetDataTemplate()
            };

            TestAsync(
                ExpandAllDelay,
                view,
                () => view.ExpandAll(),
                () => view.ExpandAll(),
                () => Assert.AreEqual(0, view.GetContainersFromItem(7).Count(), "Should not find containers!"),
                () => Assert.IsNull(view.GetContainerFromItem(7), "Should not find a first container"));
        }
        #endregion Get Container(s) from Item

        #region Get Path
        /// <summary>
        /// Verify that GetPath throws an exception when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetPath throws an exception when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetPathThrows()
        {
            TreeViewItem item = null;
            item.GetPath();
        }

        /// <summary>
        /// Get the path from a TreeViewItem from the root of the TreeView.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get the path from a TreeViewItem to the root of the TreeView.")]
        public virtual void GetPathFromRoot()
        {
            TreeViewItem t1, t2, t3, t4, t5;
            TreeView view = new TreeViewBuilder()
                .Items("first").Named(out t1).Expand()
                    .Item("second").Named(out t2)
                    .Items("third").Named(out t3).Expand()
                        .Items("fourth").Named(out t4).Expand()
                            .Item("fifth").Named(out t5)
                        .EndItems()
                    .EndItems()
                .EndItems();

            TestAsync(
                view,
                () => AssertSequence(
                        Sequence(t1, t3, t4, t5),
                        t5.GetPath().Select(p => p.Value),
                        "From root to fifth"));
        }

        /// <summary>
        /// Ensure that the path includes the item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure that the path includes the item.")]
        public virtual void GetPathIncludesItem()
        {
            TreeViewItem item;
            TreeView view = new TreeViewBuilder().Item("item").Named(out item);

            TestAsync(
                view,
                () => AssertSequence(
                        Sequence(item),
                        item.GetPath().Select(p => p.Value),
                        "Just the item"));
        }

        /// <summary>
        /// Get the path to an item not in the tree.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get the path to an item not in the tree.")]
        public virtual void GetPathNotInTree()
        {
            TreeViewItem item = new TreeViewItem { Header = "item" };
            TestAsync(
                item,
                () => AssertSequence(
                        Sequence<TreeViewItem>(),
                        item.GetPath().Select(p => p.Value),
                        "Empty path when not in a tree"));
        }
        #endregion Get Path

        #region TreeViewItem Attributes
        /// <summary>
        /// Verify that GetItem throws an exception given a null item.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetItem throws an exception given a null item.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetItemThrows()
        {
            TreeViewItem item = null;
            item.GetItem();
        }

        /// <summary>
        /// Verify GetItem returns null when not in the visual tree.
        /// </summary>
        [TestMethod]
        [Description("Verify GetItem returns null when not in the visual tree.")]
        public virtual void GetItemNotInVisualTree()
        {
            TreeViewItem item;
            new TreeViewBuilder().Item("item").Named(out item);

            Assert.IsNull(item.GetItem(), "Found the item!");
        }

        /// <summary>
        /// Verify that GetItem correctly finds a container's item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that GetItem correctly finds a container's item.")]
        public virtual void GetItemFindsItemForContainer()
        {
            Hierarchy h1, h2;
            TreeView view = new TreeView { ItemTemplate = Hierarchy.GetDataTemplate() };
            view.ItemsSource = new[] { new Hierarchy("h1").Named(out h1).Item("h2").Named(out h2) };
            TreeViewItem t1 = null, t2 = null;

            TestAsync(
                50,
                view,
                () => t1 = view.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem,
                () => Assert.AreEqual(h1, t1.GetItem(), "Failed to find first item!"),
                () => t1.IsExpanded = true,
                () => t2 = t1.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem,
                () => Assert.AreEqual(h2, t2.GetItem(), "Failed to find second item!"));
        }

        /// <summary>
        /// Verify that GetItem returns null when a TreeViewItem has no parent.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that GetItem returns null when a TreeViewItem has no parent.")]
        public virtual void GetItemNoParent()
        {
            TreeViewItem item = new TreeViewItem { Header = "item" };
            TestAsync(
                item,
                () => Assert.IsNull(item.GetItem(), "Cannot find item with no parent!"));
        }

        /// <summary>
        /// Verify GetParentItem returns null when not in the visual tree.
        /// </summary>
        [TestMethod]
        [Description("Verify GetParentItem returns null when not in the visual tree.")]
        public virtual void GetParentItemNotInVisualTree()
        {
            TreeViewItem item;
            TreeView view = new TreeViewBuilder().Item(1).Named(out item);

            Assert.IsNull(view.GetParentItem(item), "Found a parent!");
        }

        /// <summary>
        /// Verify GetParentItem returns null when it can't find the item's container.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify GetParentItem returns null when it can't find the item's container.")]
        public virtual void GetParentItemCannotFindContainer()
        {
            TreeViewItem item;
            TreeView view = new TreeViewBuilder().Item(1).Named(out item);

            TestAsync(
                view,
                () => Assert.IsNull(view.GetParentItem("does not exist"), "Found a parent!"));
        }

        /// <summary>
        /// Verify GetParentItem returns null when the item's container has no parent.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify GetParentItem returns null when the item's container has no parent.")]
        public virtual void GetParentItemHasNoParent()
        {
            TreeViewItem item;
            TreeView view = new TreeViewBuilder().Item(1).Named(out item);

            TestAsync(
                view,
                () => Assert.IsNotNull(view.GetContainerFromItem(item), "Could not find the container!"),
                () => Assert.IsNull(view.GetParentItem(item), "Found a parent!"));
        }

        /// <summary>
        /// Verify GetParentItem returns the correct parent.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify GetParentItem returns the correct parent.")]
        public virtual void GetParentItem()
        {
            TreeViewItem first, second, third, fourth;
            TreeView view = new TreeViewBuilder()
                .Items(1).Expand().Named(out first)
                    .Items(2).Expand().Named(out second)
                        .Items(3).Expand().Named(out third)
                            .Item(4).Named(out fourth)
                        .EndItems()
                    .EndItems()
                .EndItems();

            TestAsync(
                view,
                () => Assert.AreEqual(third, view.GetParentItem(fourth), "Did not find third!"),
                () => Assert.AreEqual(second, view.GetParentItem(third), "Did not find second!"),
                () => Assert.AreEqual(first, view.GetParentItem(second), "Did not find first!"),
                () => Assert.IsNull(view.GetParentItem(first), "Found first!"));
        }
        
        /// <summary>
        /// Verify that GetIsRoot throws when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetIsRoot throws when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetIsRootThrows()
        {
            TreeViewItem item = null;
            item.GetIsRoot();
        }

        /// <summary>
        /// Verify that GetIsRoot correctly distinguishes roots from other
        /// TreeViewItems.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that GetIsRoot correctly distinguishes roots from other TreeViewItems.")]
        public virtual void GetIsRoot()
        {
            TreeViewItem t1, t2, t3, t4, t5;
            TreeView view = new TreeViewBuilder()
                .Items("first").Named(out t1).Expand()
                    .Items("second").Named(out t2)
                        .Item("third").Named(out t3)
                    .EndItems()
                    .Item("fourth").Named(out t4)
                .EndItems()
                .Item("fifth").Named(out t5);

            TestAsync(
                view,
                () => Assert.IsTrue(t1.GetIsRoot(), "t1 is a root!"),
                () => Assert.IsFalse(t2.GetIsRoot(), "t2 is not a root!"),
                () => Assert.IsFalse(t3.GetIsRoot(), "t3 is not a root!"),
                () => Assert.IsFalse(t4.GetIsRoot(), "t4 is not a root!"),
                () => Assert.IsTrue(t5.GetIsRoot(), "t5 is a root!"));
        }

        /// <summary>
        /// Verify that GetIsLeaf throws when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetIsLeaf throws when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetIsLeafThrows()
        {
            TreeViewItem item = null;
            item.GetIsLeaf();
        }

        /// <summary>
        /// Verify that GetIsLeaf correctly distinguishes leaves from other
        /// TreeViewItems.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that GetIsLeaf correctly distinguishes leaves from other TreeViewItems.")]
        public virtual void GetIsLeaf()
        {
            TreeViewItem t1, t2, t3, t4, t5;
            TreeView view = new TreeViewBuilder()
                .Items("first").Named(out t1).Expand()
                    .Items("second").Named(out t2)
                        .Item("third").Named(out t3)
                    .EndItems()
                    .Item("fourth").Named(out t4)
                .EndItems()
                .Item("fifth").Named(out t5);

            TestAsync(
                view,
                () => Assert.IsFalse(t1.GetIsLeaf(), "t1 is not a leaf!"),
                () => Assert.IsFalse(t2.GetIsLeaf(), "t2 is not a leaf!"),
                () => Assert.IsTrue(t3.GetIsLeaf(), "t3 is a leaf!"),
                () => Assert.IsTrue(t4.GetIsLeaf(), "t4 is a leaf!"),
                () => Assert.IsTrue(t5.GetIsLeaf(), "t5 is a leaf!"));
        }

        /// <summary>
        /// Verify that GetDepth throws when given null.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetDepth throws when given null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetDepthThrowsOnNull()
        {
            TreeViewItem item = null;
            item.GetDepth();
        }

        /// <summary>
        /// Verify that GetDepth throws when an item has no TreeView.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetDepth throws when an item has no TreeView.")]
        [ExpectedException(typeof(ArgumentException))]
        public virtual void GetDepthThrowsWithNoTreeView()
        {
            TreeViewItem item = new TreeViewItem { Header = "Item" };
            item.GetDepth();
        }

        /// <summary>
        /// Verify that GetDepth starts indexing at zero (i.e. root items have
        /// depth zero).
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that GetDepth starts indexing at zero (i.e. root items have depth zero).")]
        public virtual void GetDepthStartsAtZero()
        {
            TreeViewItem item;
            TreeView view = new TreeViewBuilder().Item("item").Named(out item);

            TestAsync(
                view,
                () => Assert.AreEqual(0, item.GetDepth(), "Root item should have depth zero."));
        }

        /// <summary>
        /// Verify that GetDepth returns the correct depth.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that GetDepth returns the correct depth.")]
        public virtual void GetDepth()
        {
            TreeViewItem t1, t2, t3, t4, t5, t6;
            TreeView view = new TreeViewBuilder()
                .Items(1).Named(out t1).Expand()
                    .Items(2).Named(out t2).Expand()
                        .Item(3).Named(out t3)
                        .Items(4).Named(out t4).Expand()
                            .Item(5).Named(out t5)
                        .EndItems()
                    .EndItems()
                .EndItems()
                .Item(6).Named(out t6);

            TestAsync(
                view,
                () => Assert.AreEqual(0, t1.GetDepth(), "t1"),
                () => Assert.AreEqual(1, t2.GetDepth(), "t2"),
                () => Assert.AreEqual(2, t3.GetDepth(), "t3"),
                () => Assert.AreEqual(2, t4.GetDepth(), "t4"),
                () => Assert.AreEqual(3, t5.GetDepth(), "t5"),
                () => Assert.AreEqual(0, t6.GetDepth(), "t6"));
        }
        #endregion TreeViewItem Attributes

        #region Selection
        /// <summary>
        /// Verify that GetSelectedContainer thrwos when given a null TreeView.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetSelectedContainer thrwos when given a null TreeView.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetSelectedContainerThrows()
        {
            TreeView view = null;
            view.GetSelectedContainer();
        }

        /// <summary>
        /// Get a TreeView's selected TreeViewItem.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get a TreeView's selected TreeViewItem.")]
        public virtual void GetSelectedContainer()
        {
            TreeViewItem t1, t2, t3, t4, t5;
            TreeView view = new TreeViewBuilder()
                .Items(1).Named(out t1).Expand()
                    .Items(2).Named(out t2).Expand()
                        .Item(3).Named(out t3)
                        .Item(4).Named(out t4).Select()
                    .EndItems()
                .EndItems()
                .Item(5).Named(out t5);

            TestAsync(
                view,
                () => Assert.AreEqual(t4, view.GetSelectedContainer(), "Did not find expected container!"));
        }

        /// <summary>
        /// Get a TreeView's selected TreeViewItem when nothing is selected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get a TreeView's selected TreeViewItem when nothing is selected.")]
        public virtual void GetSelectedContainerNoSelection()
        {
            TreeViewItem t1, t2, t3, t4, t5;
            TreeView view = new TreeViewBuilder()
                .Items(1).Named(out t1).Expand()
                    .Items(2).Named(out t2).Expand()
                        .Item(3).Named(out t3)
                        .Item(4).Named(out t4)
                    .EndItems()
                .EndItems()
                .Item(5).Named(out t5);

            TestAsync(
                view,
                () => Assert.IsNull(view.GetSelectedContainer(), "Nothing is selected!"));
        }

        /// <summary>
        /// Verify that SetSelectedContainer throws given a null TreeView.
        /// </summary>
        [TestMethod]
        [Description("Verify that SetSelectedContainer throws given a null TreeView.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void SetSelectedContainerThrows()
        {
            TreeView view = null;
            view.SetSelectedContainer(null);
        }

        /// <summary>
        /// Verify setting the selected container to null clears the existing
        /// selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify setting the selected container to null clears the existing selection.")]
        public virtual void SetSelectedContainerNull()
        {
            TreeViewItem t1, t2;
            TreeView view = new TreeViewBuilder()
                .Item(1).Named(out t1)
                .Item(2).Named(out t2);

            TestAsync(
                view,
                () => t1.IsSelected = true,
                () => Assert.IsNotNull(view.SelectedItem, "The first item should be selected!"),
                () => view.SetSelectedContainer(null),
                () => Assert.IsNull(view.SelectedItem, "The TreeView should have no selection!"));
        }

        /// <summary>
        /// Verify setting the selected item changes the selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify setting the selected item changes the selection.")]
        public virtual void SetSelectedItemChangesSelection()
        {
            TreeViewItem t1, t2;
            TreeView view = new TreeViewBuilder()
                .Item(1).Named(out t1).Select()
                .Item(2).Named(out t2);

            TestAsync(
                view,
                () => Assert.IsTrue(t1.IsSelected, "t1 should start selected"),
                () => view.SetSelectedContainer(t2),
                () => Assert.IsFalse(t1.IsSelected, "t1 should no longer be selected"),
                () => Assert.IsTrue(t2.IsSelected, "t2 should be selected!"));
        }

        /// <summary>
        /// Verify that ClearSelection throws for a null TreeView.
        /// </summary>
        [TestMethod]
        [Description("Verify that ClearSelection throws for a null TreeView.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void ClearSelectionThrows()
        {
            TreeView view = null;
            view.ClearSelection();
        }

        /// <summary>
        /// Verify that clearing the selection unselects the existing item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that clearing the selection unselects the existing item.")]
        public virtual void ClearExistingSelection()
        {
            TreeViewItem t1, t2;
            TreeView view = new TreeViewBuilder()
                .Item(1).Named(out t1)
                .Item(2).Named(out t2);

            TestAsync(
                view,
                () => t1.IsSelected = true,
                () => Assert.IsNotNull(view.SelectedItem, "The first item should be selected!"),
                () => view.ClearSelection(),
                () => Assert.IsNull(view.SelectedItem, "The TreeView should have no selection!"),
                () => Assert.IsFalse(t1.IsSelected, "t1 should no longer be selected!"));
        }

        /// <summary>
        /// Verify that clearing selection when nothing was selected works correctly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that clearing selection when nothing was selected works correctly.")]
        public virtual void ClearSelectionWhenUnselected()
        {
            TreeViewItem t1, t2;
            TreeView view = new TreeViewBuilder()
                .Item(1).Named(out t1)
                .Item(2).Named(out t2);

            TestAsync(
                view,
                () => view.ClearSelection(),
                () => Assert.IsNull(view.SelectedItem, "The TreeView should have no selection!"));
        }

        /// <summary>
        /// Verify that SetSelectedItem throws for a null TreeView.
        /// </summary>
        [TestMethod]
        [Description("Verify that SetSelectedItem throws for a null TreeView.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void SetSelectedItemThrows()
        {
            TreeView view = null;
            view.SelectItem(null);
        }

        /// <summary>
        /// Set the selected item of the TreeView.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Set the selected item of the TreeView.")]
        public virtual void SetSelectedItem()
        {
            Hierarchy h1, h2, h3, h4, h5; 
            TreeView view = new TreeView { ItemTemplate = Hierarchy.GetDataTemplate() };
            view.ItemsSource = new[]
            {
                new Hierarchy("first").Named(out h1)
                    .Items("second").Named(out h2)
                        .Item("third").Named(out h3)
                    .EndItems()
                    .Item("fourth").Named(out h4),
                new Hierarchy("fifth").Named(out h5)
            };

            TestAsync(
                ExpandAllDelay,
                view,
                () => view.ExpandAll(),
                () => view.ExpandAll(),
                () => (view.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem).IsSelected = true,
                () => Assert.AreEqual(h1, view.SelectedItem, "The first item should be selected!"),
                () => Assert.IsTrue(view.SelectItem(h5), "Selection should succeed!"),
                () => Assert.AreEqual(h5, view.SelectedItem, "The fifth item should be selected!"),
                () => Assert.IsTrue((view.ItemContainerGenerator.ContainerFromIndex(1) as TreeViewItem).IsSelected, "Fifth's container should be selected!"),
                () => Assert.IsFalse(view.SelectItem(new Hierarchy()), "Invalid selection should fail!"));
        }

        /// <summary>
        /// Verify that GetSelectedPath throws on a null TreeView.
        /// </summary>
        [TestMethod]
        [Description("Verify that GetSelectedPath throws on a null TreeView.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetSelectedPathThrows()
        {
            TreeView view = null;
            view.GetSelectedPath();
        }

        /// <summary>
        /// Get the selected path when nothing is selected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get the selected path when nothing is selected.")]
        public virtual void GetSelectedPathWhenUnselected()
        {
            TreeView view = new TreeViewBuilder().Item(1).Item(2).Item(3);
            TestAsync(
                view,
                () => AssertSequence(
                        Sequence<ItemAndContainer>(),
                        view.GetSelectedPath(),
                        "The selected path should be empty!"));
        }

        /// <summary>
        /// Get the selected path.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Get the selected path.")]
        public virtual void GetSelectedPath()
        {
            TreeViewItem t1, t2, t3, t4, t5;
            TreeView view = new TreeViewBuilder()
                .Items(1).Named(out t1).Expand()
                    .Items(2).Named(out t2).Expand()
                        .Item(3).Named(out t3)
                        .Item(4).Named(out t4).Select()
                    .EndItems()
                .EndItems()
                .Item(5).Named(out t5);

            TestAsync(
                view,
                () => AssertSequence(
                        Sequence(t1, t2, t4),
                        view.GetSelectedPath().Select(p => p.Value),
                        "Get the selected path"));
        }
        #endregion Selection

        #region Expand and Collapse
        /// <summary>
        /// Verify that ExpandAll throws on a null TreeView.
        /// </summary>
        [TestMethod]
        [Description("Verify that ExpandAll throws on a null TreeView.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void ExpandAllThrows()
        {
            TreeView view = null;
            view.ExpandAll();
        }

        /// <summary>
        /// Verify that CollapseAll throws on a null TreeView.
        /// </summary>
        [TestMethod]
        [Description("Verify that CollapseAll throws on a null TreeView.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void CollapseAllThrows()
        {
            TreeView view = null;
            view.CollapseAll();
        }

        /// <summary>
        /// Verify that ExpandToDepth throws on a null TreeView.
        /// </summary>
        [TestMethod]
        [Description("Verify that ExpandToDepth throws on a null TreeView.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void ExpandToDepthThrows()
        {
            TreeView view = null;
            view.ExpandToDepth(0);
        }

        /// <summary>
        /// Verify that ExpandPath throws on a null TreeViewItem.
        /// </summary>
        [TestMethod]
        [Description("Verify that ExpandPath throws on a null TreeViewItem.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void ExpandPathFromItemThrows()
        {
            TreeViewItem item = null;
            item.ExpandPath();
        }

        /// <summary>
        /// Verify that ExpandSelectedPath throws on a null TreeView.
        /// </summary>
        [TestMethod]
        [Description("Verify that ExpandSelectedPath throws on a null TreeView.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void ExpandSelectedPathThrows()
        {
            TreeView view = null;
            view.ExpandSelectedPath();
        }

        /// <summary>
        /// Verify that CollapseAllButSelectedPath throws on a null TreeView.
        /// </summary>
        [TestMethod]
        [Description("Verify that CollapseAllButSelectedPath throws on a null TreeView.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void CollapseAllButSelectedPathThrows()
        {
            TreeView view = null;
            view.CollapseAllButSelectedPath();
        }

        /// <summary>
        /// Verify that ExpandPath throws on a null TreeView.
        /// </summary>
        [TestMethod]
        [Description("Verify that ExpandPath throws on a null TreeView.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void ExpandPathUsingItemsThrowsNullTreeView()
        {
            TreeView view = null;
            view.ExpandPath(1, 2, 3);
        }

        /// <summary>
        /// Verify that ExpandPath throws on a null TreeView.
        /// </summary>
        [TestMethod]
        [Description("Verify that ExpandPath throws on a null TreeView.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void ExpandPathUsingItemsThrowsNullPath()
        {
            TreeView view = new TreeView();
            view.ExpandPath(null);
        }

        /// <summary>
        /// Ensure ExpandAll opens all the interior items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure ExpandAll opens all the interior items.")]
        public virtual void ExpandAll()
        {
            TreeView view = new TreeViewBuilder()
                .Items(1)
                    .Item(2)
                    .Item(3)
                    .Items(4)
                        .Items(5)
                            .Item(6)
                        .EndItems()
                    .EndItems()
                .EndItems()
                .Item(7);

            // Expand all twice to ignore timing issues since we use the
            // dispatcher queue
            TestAsync(
                ExpandAllDelay,
                view,
                () => view.ExpandAll(),
                () => view.ExpandAll(),
                () => Assert.IsTrue(view.GetContainers().All(t => t.IsExpanded || !t.HasItems), "All items should be expanded!"));
        }

        /// <summary>
        /// Ensure CollapseAll collapses every item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Ensure CollapseAll collapses every item.")]
        public virtual void CollapseAll()
        {
            TreeView view = new TreeViewBuilder()
                .Items(1)
                    .Item(2)
                    .Item(3)
                    .Items(4)
                        .Items(5)
                            .Item(6)
                        .EndItems()
                    .EndItems()
                .EndItems()
                .Item(7);

            // Expand all twice to ignore timing issues since we use the
            // dispatcher queue
            TestAsync(
                ExpandAllDelay,
                view,
                () => view.ExpandAll(),
                () => view.ExpandAll(),
                () => Assert.IsTrue(view.GetContainers().All(t => t.IsExpanded || !t.HasItems), "All items should be expanded!"),
                () => view.CollapseAll(),
                () => view.CollapseAll(),
                () => Assert.IsFalse(view.GetContainers().Any(t => t.IsExpanded), "All items should be collapsed!"));
        }

        /// <summary>
        /// Verify that expanding to a negative depth does nothing.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that expanding to a negative depth does nothing.")]
        public virtual void ExpandToNegativeDepth()
        {
            TreeView view = new TreeViewBuilder()
                .Items(1)
                    .Item(2)
                    .Item(3)
                    .Items(4)
                        .Items(5)
                            .Item(6)
                        .EndItems()
                    .EndItems()
                .EndItems()
                .Item(7);

            TestAsync(
                ExpandAllDelay,
                view,
                () => view.ExpandToDepth(-1));
        }

        /// <summary>
        /// Verify that ExpandToDepth expands all the items in the first n
        /// levels.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that ExpandToDepth expands all the items in the first n levels.")]
        public virtual void ExpandToDepth()
        {
            TreeView view = new TreeViewBuilder()
                .Items(1)
                    .Item(2)
                    .Item(3)
                    .Items(4)
                        .Items(5)
                            .Items(6)
                                .Items(7)
                                    .Item(8)
                                .EndItems()
                            .EndItems()
                        .EndItems()
                    .EndItems()
                .EndItems()
                .Item(9);

            TestAsync(
                ExpandAllDelay,
                view,
                () => view.ExpandToDepth(2),
                () => Assert.IsTrue(view.GetContainers().All(t => !t.IsExpanded || t.GetDepth() <= 2), "Only items with depth less than or equal to 2 are expanded!"));
        }

        /// <summary>
        /// Expand the path of an item to the root of the TreeView.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Expand the path of an item to the root of the TreeView.")]
        public virtual void ExpandPathFromItem()
        {
            TreeViewItem t1, t4, t5, t6;
            TreeView view = new TreeViewBuilder()
                .Items(1).Named(out t1).Expand()
                    .Item(2)
                    .Item(3)
                    .Items(4).Named(out t4).Expand()
                        .Items(5).Named(out t5).Expand()
                            .Item(6).Named(out t6)
                        .EndItems()
                    .EndItems()
                .EndItems()
                .Item(7);

            TestAsync(
                ExpandAllDelay,
                view,
                () => view.CollapseAll(),
                () => t6.ExpandPath(),
                () => Assert.IsTrue(t5.IsExpanded, "t5 should have been expanded!"),
                () => Assert.IsTrue(t4.IsExpanded, "t5 should have been expanded!"),
                () => Assert.IsTrue(t1.IsExpanded, "t5 should have been expanded!"));
        }

        /// <summary>
        /// Verify that expanding the selected path works properly.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that expanding the selected path works properly.")]
        public virtual void ExpandSelectedPath()
        {
            TreeViewItem t1, t4, t5, t6;
            TreeView view = new TreeViewBuilder()
                .Items(1).Named(out t1).Expand()
                    .Item(2)
                    .Item(3)
                    .Items(4).Named(out t4).Expand()
                        .Items(5).Named(out t5).Expand()
                            .Item(6).Named(out t6)
                        .EndItems()
                    .EndItems()
                .EndItems()
                .Item(7);

            TestAsync(
                ExpandAllDelay,
                view,
                () => view.CollapseAll(),
                () => t6.IsSelected = true,
                () => view.ExpandSelectedPath(),
                () => Assert.IsTrue(t5.IsExpanded, "t5 should have been expanded!"),
                () => Assert.IsTrue(t4.IsExpanded, "t5 should have been expanded!"),
                () => Assert.IsTrue(t1.IsExpanded, "t5 should have been expanded!"));
        }

        /// <summary>
        /// Verify that expanding the selected path works does nothing when no
        /// TreeViewItem is selected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that expanding the selected path works does nothing when no TreeViewItem is selected.")]
        public virtual void ExpandSelectedPathNoSelection()
        {
            TreeViewItem t1, t4, t5, t6;
            TreeView view = new TreeViewBuilder()
                .Items(1).Named(out t1).Expand()
                    .Item(2)
                    .Item(3)
                    .Items(4).Named(out t4).Expand()
                        .Items(5).Named(out t5).Expand()
                            .Item(6).Named(out t6)
                        .EndItems()
                    .EndItems()
                .EndItems()
                .Item(7);

            TestAsync(
                ExpandAllDelay,
                view,
                () => view.CollapseAll(),
                () => view.ExpandSelectedPath(),
                () => Assert.IsTrue(view.GetContainers().All(t => t.GetIsRoot() || !t.IsExpanded), "Everything is still collapsed!"));
        }

        /// <summary>
        /// Collapse everything but the selected path.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Collapse everything but the selected path.")]
        public virtual void CollapseAllButSelectedPath()
        {
            TreeViewItem t1, t4, t5, t6;
            TreeView view = new TreeViewBuilder()
                .Items(1).Named(out t1).Expand()
                    .Item(2)
                    .Item(3)
                    .Items(4).Named(out t4).Expand()
                        .Items(5).Named(out t5).Expand()
                            .Item(6).Named(out t6)
                        .EndItems()
                    .EndItems()
                    .Items(7).Expand()
                        .Items(8).Expand()
                            .Item(9)
                        .EndItems()
                    .EndItems()
                .EndItems()
                .Item(10);

            List<TreeViewItem> path = null;
            TestAsync(
                ExpandAllDelay,
                view,
                () => t6.IsSelected = true,
                () => view.CollapseAllButSelectedPath(),
                () => path = new List<TreeViewItem>(view.GetSelectedPath().Select(p => p.Value)),
                () => Assert.IsTrue(view.GetContainers().All(t => t.GetIsRoot() || !t.IsExpanded || path.Contains(t)), "Everything but the selected path is collapsed!"),
                () => view.ClearSelection(),
                () => view.CollapseAllButSelectedPath(),
                () => Assert.IsTrue(view.GetContainers().All(t => t.GetIsRoot() || !t.IsExpanded), "Everything is now collapsed!"));
        }

        /////// <summary>
        /////// Expand a path using the items.
        /////// </summary>
        ////[TestMethod]
        ////[Asynchronous]
        ////[Description("Expand a path using the items.")]
        ////public virtual void ExpandPathUsingItems()
        ////{
        ////    TreeView view = new TreeView { ItemsSource = new[] { 12 } };
        ////    view.ItemTemplate = FactorizationValueConverter.GetDataTemplate();

        ////    TreeViewItem t12 = null, t6 = null;
        ////    TestAsync(
        ////        1500,
        ////        view,
        ////        () => view.ExpandPath(12, 6, 2),
        ////        () =>
        ////        {
        ////            t12 = view.GetContainerFromItem(12);
        ////            Assert.IsTrue(t12.IsExpanded, "12 should be expanded");
        ////            t6 = t12.GetItemsAndContainers().Where(p => (int)p.Key == 6).Select(p => p.Value).FirstOrDefault();
        ////            Assert.IsTrue(t6.IsExpanded, "6 should be expanded");
        ////        });
        ////}

        /////// <summary>
        /////// Expand a path using the items and a comparison selector.
        /////// </summary>
        ////[TestMethod]
        ////[Asynchronous]
        ////[Description("Expand a path using the items and a comparison selector.")]
        ////public virtual void ExpandPathUsingItemsAndComparisonSelector()
        ////{
        ////    TreeView view = new TreeView { ItemTemplate = Hierarchy.GetDataTemplate() };
        ////    view.ItemsSource = new[]
        ////    {
        ////        new Hierarchy("first")
        ////            .Items("second")
        ////                .Item("third")
        ////            .EndItems()
        ////            .Item("fourth"),
        ////        new Hierarchy("fifth")
        ////    };

        ////    TreeViewItem t1 = null, t2 = null;
        ////    TestAsync(
        ////        1500,
        ////        view,
        ////        () => view.ExpandPath(o => ((Hierarchy)o).Element, "first", "second", "third"),
        ////        () =>
        ////        {
        ////            t1 = view.GetItemsAndContainers().Where(p => ((Hierarchy)p.Key).Element == "first").Select(p => p.Value).FirstOrDefault();
        ////            Assert.IsTrue(t1.IsExpanded, "first should be expanded");
        ////            t2 = view.GetItemsAndContainers().Where(p => ((Hierarchy)p.Key).Element == "second").Select(p => p.Value).FirstOrDefault();
        ////            Assert.IsTrue(t2.IsExpanded, "second should be expanded");
        ////        });
        ////}
        #endregion Expand and Collapse

        #region IsChecked
        /// <summary>
        /// Ensure GetIsChecked throws an exception on a null TreeViewItem.
        /// </summary>
        [TestMethod]
        [Description("Ensure GetIsChecked throws an exception on a null TreeViewItem.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetIsCheckedThrows()
        {
            TreeViewItem item = null;
            item.GetIsChecked();
        }

        /// <summary>
        /// Ensure SetIsChecked throws an exception on a null TreeViewItem.
        /// </summary>
        [TestMethod]
        [Description("Ensure SetIsChecked throws an exception on a null TreeViewItem.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void SetIsCheckedThrows()
        {
            TreeViewItem item = null;
            item.SetIsChecked(false);
        }

        /// <summary>
        /// Ensure GetCheckedItemsAndContainers throws an exception on a null TreeView.
        /// </summary>
        [TestMethod]
        [Description("Ensure GetCheckedItemsAndContainers throws an exception on a null TreeView.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GetCheckedItemsThrows()
        {
            TreeView view = null;
            view.GetCheckedItemsAndContainers();
        }

        /// <summary>
        /// Verify basic IsChecked functionality.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify basic IsChecked functionality.")]
        public virtual void IsCheckedBasic()
        {
            TreeViewItem t1, t2, t3, t4, t5;
            TreeView view = new TreeViewBuilder()
                .Items(1).Named(out t1).Expand()
                    .Items(2).Named(out t2).Expand()
                        .Item(3).Named(out t3)
                        .Item(4).Named(out t4)
                    .EndItems()
                .EndItems()
                .Item(5).Named(out t5);

            TestAsync(
                15,
                view,
                () => t4.SetIsChecked(true),
                () => Assert.AreEqual(true, t4.GetIsChecked(), "t4 should be checked!"),
                () => Assert.AreEqual(null, t2.GetIsChecked(), "t2 should be intermediate!"),
                () => Assert.AreEqual(null, t1.GetIsChecked(), "t1 should be intermediate!"),
                () => t2.SetIsChecked(true),
                () => Assert.AreEqual(true, t3.GetIsChecked(), "t3 should be is checked!"),
                () => Assert.AreEqual(true, t1.GetIsChecked(), "t1 should be is checked now!"),
                () => AssertSequence(
                        Sequence<object>(1, 2, 3, 4),
                        view.GetCheckedItemsAndContainers().Select(o => ((TreeViewItem)o.Key).Header), 
                        "One through four should be checked!"));
        }

        /// <summary>
        /// Verify basic IsChecked functionality works with CheckBoxes and drive
        /// using the attached property.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify basic IsChecked functionality works with CheckBoxes and drive using the attached property.")]
        public virtual void IsCheckedWithCheckBoxesViaAttachedProperty()
        {
            TreeViewItem t1, t2, t3, t4, t5;
            TreeView view = new TreeViewBuilder()
                .Items(new TreeViewItemCheckBox { Content = 1 }).Named(out t1).Expand()
                    .Items(new TreeViewItemCheckBox { Content = 2 }).Named(out t2).Expand()
                        .Item(new TreeViewItemCheckBox { Content = 3 }).Named(out t3)
                        .Item(new TreeViewItemCheckBox { Content = 4 }).Named(out t4)
                    .EndItems()
                .EndItems()
                .Item(new TreeViewItemCheckBox { Content = 5 }).Named(out t5);

            TestAsync(
                25,
                view,
                () => t4.SetIsChecked(true),
                () => Assert.AreEqual(true, t4.GetIsChecked(), "t4 should be checked!"),
                () => Assert.AreEqual(null, t1.GetIsChecked(), "t1 should be intermediate!"),
                () => t2.SetIsChecked(true),
                () => Assert.AreEqual(true, t3.GetIsChecked(), "t3 should be is checked!"),
                () => Assert.AreEqual(true, t1.GetIsChecked(), "t1 should be is checked now!"),
                () => AssertSequence(
                        Sequence<object>(1, 2, 3, 4),
                        view.GetCheckedItemsAndContainers().Select(o => ((CheckBox)((TreeViewItem)o.Key).Header).Content),
                        "One through four should be checked!"),
                () => t1.SetIsChecked(false),
                () => Assert.AreEqual(0, view.GetCheckedItemsAndContainers().Count(), "Nothing should be checked!"));
        }

        /// <summary>
        /// Verify basic IsChecked functionality works with CheckBoxes and drive
        /// using the CheckBox.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify basic IsChecked functionality works with CheckBoxes and drive using the CheckBox.")]
        public virtual void IsCheckedWithCheckBoxesViaCheckBox()
        {
            TreeViewItemCheckBox c1 = new TreeViewItemCheckBox { Content = 1 };
            TreeViewItemCheckBox c2 = new TreeViewItemCheckBox { Content = 2 };
            TreeViewItemCheckBox c3 = new TreeViewItemCheckBox { Content = 3 };
            TreeViewItemCheckBox c4 = new TreeViewItemCheckBox { Content = 4 };
            TreeViewItemCheckBox c5 = new TreeViewItemCheckBox { Content = 5 };

            TreeViewItem t1, t2, t3, t4, t5;
            TreeView view = new TreeViewBuilder()
                .Items(c1).Named(out t1).Expand()
                    .Items(c2).Named(out t2).Expand()
                        .Item(c3).Named(out t3)
                        .Item(c4).Named(out t4)
                    .EndItems()
                .EndItems()
                .Item(c5).Named(out t5);

            TestAsync(
                25,
                view,
                () => c4.IsChecked = true,
                () => Assert.AreEqual(true, t4.GetIsChecked(), "t4 should be checked!"),
                () => Assert.AreEqual(null, t2.GetIsChecked(), "t2 should be intermediate!"),
                () => Assert.AreEqual(null, t1.GetIsChecked(), "t1 should be intermediate!"),
                () => c2.IsChecked = true,
                () => Assert.AreEqual(true, t3.GetIsChecked(), "t3 should be is checked!"),
                () => Assert.AreEqual(true, t1.GetIsChecked(), "t1 should be is checked now!"),
                () => AssertSequence(
                        Sequence<object>(1, 2, 3, 4),
                        view.GetCheckedItemsAndContainers().Select(o => ((CheckBox)((TreeViewItem)o.Key).Header).Content),
                        "One through four should be checked!"),
                () => c1.IsChecked = false,
                () => Assert.AreEqual(0, view.GetCheckedItemsAndContainers().Count(), "Nothing should be checked!"));
        }
        #endregion IsChecked
    }
}