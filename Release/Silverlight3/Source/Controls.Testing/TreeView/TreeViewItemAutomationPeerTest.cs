// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// TreeViewAutomationPeer unit tests.
    /// </summary>
    [TestClass]
    [Tag("TreeView")]
    [Tag("Automation")]
    public class TreeViewItemAutomationPeerTest : TestBase
    {
        /// <summary>
        /// Initializes a new instance of the TreeViewItemAutomationPeerTest
        /// class.
        /// </summary>
        public TreeViewItemAutomationPeerTest()
            : base()
        {
        }

        /// <summary>
        /// Create a new TreeViewItemAutomationPeer.
        /// </summary>
        [TestMethod]
        [Description("Create a new TreeViewItemAutomationPeer.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.TreeViewItemAutomationPeer", Justification = "Don't need to use it")]
        public virtual void CreateTreeViewItemPeer()
        {
            TreeViewItem item = new TreeViewItem();
            new TreeViewItemAutomationPeer(item);
        }

        /// <summary>
        /// Create a new TreeViewItemAutomationPeer with a null TreeViewItem.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Create a new TreeViewItemAutomationPeer with a null TreeViewItem.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.TreeViewItemAutomationPeer", Justification = "Don't need to use it")]
        public virtual void CreateTreeViewItemPeerWithNull()
        {
            new TreeViewItemAutomationPeer(null);
        }

        /// <summary>
        /// Verify that TreeViewItem creates a TreeViewItemAutomationPeer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewItem creates a TreeViewItemAutomationPeer.")]
        public virtual void TreeViewItemCreatesAutomationPeer()
        {
            TreeViewItem item = new TreeViewItem();
            TreeViewItemAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => Assert.IsNotNull(peer, "TreeViewItem peer should not be null!"),
                () => Assert.AreEqual(item, peer.Owner, "TreeViewItem should be owner of the peer!"));
        }

        /// <summary>
        /// Verify the TreeViewItemAutomationPeer's type and class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewItemAutomationPeer's type and class.")]
        public virtual void TreeViewItemAutomationPeerTypeAndClass()
        {
            TreeViewItem item = new TreeViewItem();
            TreeViewItemAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => Assert.AreEqual(AutomationControlType.TreeItem, peer.GetAutomationControlType(), "Unexpected AutomationControlType!"),
                () => Assert.AreEqual("TreeViewItem", peer.GetClassName(), "Unexpected ClassType!"));
        }

        /// <summary>
        /// Verify the TreeViewItemAutomationPeer returns null for its children
        /// when there are no items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewItemAutomationPeer returns null for its children when there are no items.")]
        [Bug("TODO: Make sure this is the correct behavior")]
        public virtual void TreeViewItemPeerGetNoItems()
        {
            TreeViewItem item = new TreeViewItem();
            TreeViewItemAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => Assert.IsNull(peer.GetChildren(), "There should be no children when the TreeViewItem does not have items!"));
        }

        /// <summary>
        /// Verify the TreeViewItemAutomationPeer returns wrappers for its
        /// items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewItemAutomationPeer returns wrappers for its children.")]
        [Bug("TODO: Make sure this is the correct behavior")]
        public virtual void TreeViewItemPeerGetWithItems()
        {
            TreeViewItem item = new TreeViewItem { ItemsSource = new int[] { 1, 2, 3 } };
            TreeViewItemAutomationPeer peer = null;
            List<AutomationPeer> items = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => items = peer.GetChildren(),
                () => Assert.AreEqual(3, items.Count, "Unexpected number of child peers!"),
                () => Assert.IsInstanceOfType(items[0], typeof(TreeViewItemAutomationPeer), "Child peer is not a TreeViewItemAutomationPeer!"));
        }

        /// <summary>
        /// Verify that TreeViewItemAutomationPeer only supports scrolling and
        /// selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewItemAutomationPeer only supports scrolling and selection.")]
        public virtual void TreeViewItemPeerOnlySupportsScrollingAndSelection()
        {
            TreeViewItem item = new TreeViewItem();
            TreeViewItemAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Dock), "TreeViewItemAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Grid), "TreeViewItemAutomationPeer should not support the Grid pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.GridItem), "TreeViewItemAutomationPeer should not support the GridItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Invoke), "TreeViewItemAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.MultipleView), "TreeViewItemAutomationPeer should not support the MultipleView pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.RangeValue), "TreeViewItemAutomationPeer should not support the RangeValue pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Scroll), "TreeViewItemAutomationPeer should not support the Scroll pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Selection), "TreeViewItemAutomationPeer should not support the Selection pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Table), "TreeViewItemAutomationPeer should not support the Table pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.TableItem), "TreeViewItemAutomationPeer should not support the TableItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Toggle), "TreeViewItemAutomationPeer should not support the Toggle pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Transform), "TreeViewItemAutomationPeer should not support the Transform pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Value), "TreeViewItemAutomationPeer should not support the Value pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Window), "TreeViewItemAutomationPeer should not support the Window pattern!"));
        }

        #region ISelectionItemProvider
        /// <summary>
        /// Verify that TreeViewItemAutomationPeer implements the
        /// ISelectionItemProvider interface.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewItemAutomationPeer implements the ISelectionItemProvider interface.")]
        public virtual void TreeViewItemPeerIsISelectionItemProvider()
        {
            TreeViewItem item = new TreeViewItem();
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => Assert.IsNotNull(provider, "TreeViewItemAutomationPeer should implement ISelectionItemProvider!"));
        }

        /// <summary>
        /// Verify that TreeViewItemAutomationPeer supports the Selection
        /// pattern.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewItemAutomationPeer supports the Selection pattern.")]
        public virtual void TreeViewItemPeerSupportsSelection()
        {
            TreeViewItem item = new TreeViewItem();
            TreeViewItemAutomationPeer peer = null;
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider,
                () => Assert.IsNotNull(provider, "ISelectionItemProvider peer should not be null!"));
        }

        /// <summary>
        /// Verify the TreeViewItemAutomationPeer provides IsSelected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewItemAutomationPeer provides IsSelected.")]
        public virtual void TreeViewItemPeerIsSelected()
        {
            TreeViewItem item = new TreeViewItem();
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => Assert.IsFalse(provider.IsSelected, "TreeViewItemAutomationPeer should not be selected!"),
                () => item.IsSelected = true,
                () => Assert.IsTrue(provider.IsSelected, "TreeViewItemAutomationPeer should be selected!"));
        }

        /// <summary>
        /// Verify the TreeViewItemAutomationPeer provides access to the
        /// selection container when it has no parent.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewItemAutomationPeer provides access to the selection container when it has no parent.")]
        public virtual void TreeViewItemPeerSelectionContainerNoParent()
        {
            TreeViewItem item = new TreeViewItem();
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => Assert.IsNull(provider.SelectionContainer, "TreeViewItemAutomationPeer should not have a SelectionContainer!"));
        }

        /// <summary>
        /// Verify the TreeViewItemAutomationPeer provides access to the
        /// selection container when it has no parent peer created.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewItemAutomationPeer provides access to the selection container when it has no parent peer created.")]
        public virtual void TreeViewItemPeerSelectionContainerNoParentPeer()
        {
            TreeViewItem item = new TreeViewItem { Header = "Item", IsExpanded = true, ItemsSource = new int[] { 1, 2, 3 } };
            TreeViewItem child = null;
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => child = item.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(child) as ISelectionItemProvider,
                () => Assert.IsNull(provider.SelectionContainer, "TreeViewItemAutomationPeer does not have a SelectionContainer when no parent peer was created!"));
        }

        /// <summary>
        /// Verify the TreeViewItemAutomationPeer provides access to the
        /// selection container.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewItemAutomationPeer provides access to the selection container.")]
        public virtual void TreeViewItemPeerSelectionContainer()
        {
            TreeViewItem item = new TreeViewItem { Header = "Item", IsExpanded = true, ItemsSource = new int[] { 1, 2, 3 } };
            TreeViewItem child = null;
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => FrameworkElementAutomationPeer.CreatePeerForElement(item),
                () => child = item.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(child) as ISelectionItemProvider,
                () => Assert.IsNotNull(provider.SelectionContainer, "TreeViewItemAutomationPeer should have a SelectionContainer!"));
        }

        /// <summary>
        /// Verify the TreeViewItemAutomationPeer throws an exception if there
        /// if the item is not in a TreeView in AddToSelection.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Asynchronous]
        [Description("Verify the TreeViewItemAutomationPeer throws an exception if there if the item is not in a TreeView.")]
        public virtual void TreeViewItemPeerAddSelectionNoTreeView()
        {
            TreeViewItem item = new TreeViewItem();
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => provider.AddToSelection());
        }

        /// <summary>
        /// Verify the TreeViewItemAutomationPeer throws an exception if there
        /// is already a selected item in AddToSelection.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Asynchronous]
        [Description("Verify the TreeViewItemAutomationPeer throws an exception if there is already a selected item in AddToSelection.")]
        public virtual void TreeViewItemPeerAddSelectionAlreadySelected()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second", IsSelected = true };
            view.Items.Add(first);
            view.Items.Add(second);

            ISelectionItemProvider provider = null;
            TestAsync(
                view,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(first) as ISelectionItemProvider,
                () => provider.AddToSelection());
        }

        /// <summary>
        /// Verify the TreeViewItemAutomationPeer selects this item on
        /// AddToSelection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewItemAutomationPeer selects this item on AddToSelection.")]
        public virtual void TreeViewItemPeerAddSelection()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            view.Items.Add(second);

            ISelectionItemProvider provider = null;
            TestAsync(
                view,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(first) as ISelectionItemProvider,
                () => provider.AddToSelection(),
                () => Assert.IsTrue(first.IsSelected, "First item should have been selected!"));
        }

        /// <summary>
        /// Verify the TreeViewItemAutomationPeer selects the item in Select.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewItemAutomationPeer selects the item in Select.")]
        public virtual void TreeViewItemPeerSelectNoTreeView()
        {
            TreeViewItem item = new TreeViewItem();
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => provider.Select(),
                () => Assert.IsTrue(item.IsSelected, "Item should be selected!"));
        }

        /// <summary>
        /// Verify the TreeViewItemAutomationPeer selects an item when another
        /// was already selected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewItemAutomationPeer selects an item when another was already selected.")]
        public virtual void TreeViewItemPeerSelectAlreadySelected()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second", IsSelected = true };
            view.Items.Add(first);
            view.Items.Add(second);

            ISelectionItemProvider provider = null;
            TestAsync(
                view,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(first) as ISelectionItemProvider,
                () => provider.Select(),
                () => Assert.IsTrue(first.IsSelected, "First item should be selected!"),
                () => Assert.IsFalse(second.IsSelected, "Second item should not be selected!"));
        }

        /// <summary>
        /// Verify the TreeViewItemAutomationPeer selects this item on
        /// AddToSelection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewItemAutomationPeer selects this item on AddToSelection.")]
        public virtual void TreeViewItemPeerSelect()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            view.Items.Add(second);

            ISelectionItemProvider provider = null;
            TestAsync(
                view,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(first) as ISelectionItemProvider,
                () => provider.Select(),
                () => Assert.IsTrue(first.IsSelected, "First item should have been selected!"));
        }

        /// <summary>
        /// Verify the TreeViewItemAutomationPeer unselects the item in
        /// RemoveFromSelection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewItemAutomationPeer unselects the item in RemoveFromSelection.")]
        public virtual void TreeViewItemPeerRemoveSelectionNoTreeView()
        {
            TreeViewItem item = new TreeViewItem { IsSelected = true };
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => provider.RemoveFromSelection(),
                () => Assert.IsFalse(item.IsSelected, "Item should be not selected!"));
        }

        /// <summary>
        /// Verify the TreeViewItemAutomationPeer unselects an item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewItemAutomationPeer unselects an item.")]
        public virtual void TreeViewItemPeerRemoveSelection()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second", IsSelected = true };
            view.Items.Add(first);
            view.Items.Add(second);

            ISelectionItemProvider provider = null;
            TestAsync(
                view,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(first) as ISelectionItemProvider,
                () => provider.RemoveFromSelection(),
                () => Assert.IsFalse(first.IsSelected, "First item should not be selected!"),
                () => Assert.IsTrue(second.IsSelected, "Second item should be selected!"));
        }
        #endregion ISelectionItemProvider

        #region IScrollItemProvider
        /// <summary>
        /// Verify that TreeViewItemAutomationPeer implements the
        /// IScrollItemProvider interface.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewItemAutomationPeer implements the IScrollItemProvider interface.")]
        public virtual void TreeViewItemPeerIsIScrollItemProvider()
        {
            TreeViewItem item = new TreeViewItem();
            IScrollItemProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as IScrollItemProvider,
                () => Assert.IsNotNull(provider, "TreeViewItemAutomationPeer should implement IScrollItemProvider!"));
        }

        /// <summary>
        /// Verify that TreeViewItemAutomationPeer supports the ScrollItem
        /// pattern.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewItemAutomationPeer supports the ScrollItem pattern.")]
        public virtual void TreeViewItemPeerSupportsScroll()
        {
            TreeViewItem item = new TreeViewItem();
            TreeViewItemAutomationPeer peer = null;
            IScrollItemProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ScrollItem) as IScrollItemProvider,
                () => Assert.IsNotNull(provider, "IScrollItemProvider peer should not be null!"));
        }

        /// <summary>
        /// Verify TreeViewItemAutomationPeer does not scroll an item if there
        /// is no TreeView.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify TreeViewItemAutomationPeer does not scroll an item if there is no TreeView.")]
        public virtual void TreeViewItemPeerScrollNoParent()
        {
            TreeViewItem item = new TreeViewItem();
            TreeViewItemAutomationPeer peer = null;
            IScrollItemProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ScrollItem) as IScrollItemProvider,
                () => provider.ScrollIntoView());
        }

        /// <summary>
        /// Verify TreeViewItemAutomationPeer scrolls the item into view.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify TreeViewItemAutomationPeer scrolls the item into view.")]
        public virtual void TreeViewItemPeerScroll()
        {
            TreeView view = new TreeView { Height = 20, ItemsSource = new int[] { 1, 2, 3, 4, 5 } };
            TreeViewItem item = null;
            TreeViewItemAutomationPeer peer = null;
            IScrollItemProvider provider = null;
            TestAsync(
                5,
                view,
                () => item = view.ItemContainerGenerator.ContainerFromIndex(4) as TreeViewItem,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ScrollItem) as IScrollItemProvider,
                () => provider.ScrollIntoView(),
                () =>
                {
                    foreach (DependencyObject obj in view.GetVisualDescendents())
                    {
                        ScrollViewer viewer = obj as ScrollViewer;
                        if (viewer != null)
                        {
                            Assert.AreNotEqual(0, viewer.VerticalOffset, "ScrollHost was not scrolled!");
                            return;
                        }
                    }
                    Assert.Fail("Did not find the ScrollHost!");
                });
        }
        #endregion IScrollItemProvider

        #region IExpandCollapseProvider
        /// <summary>
        /// Verify that TreeViewItemAutomationPeer implements the
        /// IExpandCollapseProvider interface.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewItemAutomationPeer implements the IExpandCollapseProvider interface.")]
        public virtual void TreeViewItemPeerIsIExpandCollapseProvider()
        {
            TreeViewItem item = new TreeViewItem();
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as IExpandCollapseProvider,
                () => Assert.IsNotNull(provider, "TreeViewItemAutomationPeer should implement IExpandCollapseProvider!"));
        }

        /// <summary>
        /// Verify that TreeViewItemAutomationPeer supports the ExpandCollapse
        /// pattern.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewItemAutomationPeer supports the ExpandCollapse pattern.")]
        public virtual void TreeViewItemPeerSupportsExpandCollapse()
        {
            TreeViewItem item = new TreeViewItem();
            TreeViewItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => Assert.IsNotNull(provider, "IExpandCollapseProvider peer should not be null!"));
        }

        /// <summary>
        /// Verify that TreeViewItemAutomationPeer has the right
        /// ExpandCollapseState for an item with no items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewItemAutomationPeer has the right ExpandCollapseState for an item with no items.")]
        public virtual void TreeViewItemPeerExpandStateNoItems()
        {
            TreeViewItem item = new TreeViewItem();
            TreeViewItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => Assert.AreEqual(ExpandCollapseState.LeafNode, provider.ExpandCollapseState, "Unexpected ExpandCollapseState!"));
        }

        /// <summary>
        /// Verify that TreeViewItemAutomationPeer has the right
        /// ExpandCollapseState for an item that is collapsed.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewItemAutomationPeer has the right ExpandCollapseState for an item that is collapsed.")]
        public virtual void TreeViewItemPeerExpandStateCollapsed()
        {
            TreeViewItem item = new TreeViewItem { ItemsSource = new int[] { 1, 2, 3 } };
            TreeViewItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => Assert.AreEqual(ExpandCollapseState.Collapsed, provider.ExpandCollapseState, "Unexpected ExpandCollapseState!"));
        }

        /// <summary>
        /// Verify that TreeViewItemAutomationPeer has the right
        /// ExpandCollapseState for an item that is expanded.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewItemAutomationPeer has the right ExpandCollapseState for an item that is expanded.")]
        public virtual void TreeViewItemPeerExpandStateExpanded()
        {
            TreeViewItem item = new TreeViewItem { IsExpanded = true, ItemsSource = new int[] { 1, 2, 3 } };
            TreeViewItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => Assert.AreEqual(ExpandCollapseState.Expanded, provider.ExpandCollapseState, "Unexpected ExpandCollapseState!"));
        }

        /// <summary>
        /// Verify that TreeViewItemAutomationPeer throws an exception when
        /// expanding a disabled item.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementNotEnabledException))]
        [Asynchronous]
        [Description("Verify that TreeViweItemAutomationPeer throws an exception when expanding a disabled item.")]
        public virtual void TreeViewItemPeerExpandDisabled()
        {
            TreeViewItem item = new TreeViewItem { IsEnabled = false };
            TreeViewItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Expand());
        }

        /// <summary>
        /// Verify that TreeViewItemAutomationPeer throws an exception when
        /// expanding a leaf node.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Asynchronous]
        [Description("Verify that TreeViweItemAutomationPeer throws an exception when expanding a leaf node.")]
        public virtual void TreeViewItemPeerExpandNoItems()
        {
            TreeViewItem item = new TreeViewItem();
            TreeViewItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Expand());
        }

        /// <summary>
        /// Verify that TreeViewItemAutomationPeer expands an item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewItemAutomationPeer expands an item.")]
        public virtual void TreeViewItemPeerExpands()
        {
            TreeViewItem item = new TreeViewItem { ItemsSource = new int[] { 1, 2, 3 } };
            TreeViewItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Expand(),
                () => Assert.IsTrue(item.IsExpanded, "Item should be expanded!"));
        }

        /// <summary>
        /// Verify that TreeViewItemAutomationPeer expands an expanded item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewItemAutomationPeer expands an expanded item.")]
        public virtual void TreeViewItemPeerExpandsExpanded()
        {
            TreeViewItem item = new TreeViewItem { IsExpanded = true, ItemsSource = new int[] { 1, 2, 3 } };
            TreeViewItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Expand(),
                () => Assert.IsTrue(item.IsExpanded, "Item should be expanded!"));
        }

        /// <summary>
        /// Verify that TreeViewItemAutomationPeer throws an exception when
        /// collapsing a disabled item.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementNotEnabledException))]
        [Asynchronous]
        [Description("Verify that TreeViweItemAutomationPeer throws an exception when collapsing a disabled item.")]
        public virtual void TreeViewItemPeerCollapseDisabled()
        {
            TreeViewItem item = new TreeViewItem { IsEnabled = false };
            TreeViewItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Collapse());
        }

        /// <summary>
        /// Verify that TreeViewItemAutomationPeer throws an exception when
        /// collapsing a leaf node.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Asynchronous]
        [Description("Verify that TreeViweItemAutomationPeer throws an exception when collapsing a leaf node.")]
        public virtual void TreeViewItemPeerCollapsingNoItems()
        {
            TreeViewItem item = new TreeViewItem();
            TreeViewItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Collapse());
        }

        /// <summary>
        /// Verify that TreeViewItemAutomationPeer collapses an item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewItemAutomationPeer collapses an item.")]
        public virtual void TreeViewItemPeerCollapse()
        {
            TreeViewItem item = new TreeViewItem { IsExpanded = true, ItemsSource = new int[] { 1, 2, 3 } };
            TreeViewItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Collapse(),
                () => Assert.IsFalse(item.IsExpanded, "Item should be collapsed!"));
        }

        /// <summary>
        /// Verify that TreeViewItemAutomationPeer collapses an collapsed item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewItemAutomationPeer collapses an collapsed item.")]
        public virtual void TreeViewItemPeerCollapseCollapsed()
        {
            TreeViewItem item = new TreeViewItem { ItemsSource = new int[] { 1, 2, 3 } };
            TreeViewItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TreeViewItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Collapse(),
                () => Assert.IsFalse(item.IsExpanded, "Item should be collapsed!"));
        }
        #endregion IExpandCollapseProvider
    }
}