// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Media;
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
    public class TreeViewAutomationPeerTest : TestBase
    {
        /// <summary>
        /// Initializes a new instance of the TreeViewAutomationPeerTest class.
        /// </summary>
        public TreeViewAutomationPeerTest()
            : base()
        {
        }

        /// <summary>
        /// Create a new TreeViewAutomationPeer.
        /// </summary>
        [TestMethod]
        [Description("Create a new TreeViewAutomationPeer.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.TreeViewAutomationPeer", Justification = "Don't need to use it")]
        public virtual void CreateTreeViewPeer()
        {
            TreeView tree = new TreeView();
            new TreeViewAutomationPeer(tree);
        }

        /// <summary>
        /// Create a new TreeViewAutomationPeer with a null TreeView.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Create a new TreeViewAutomationPeer with a null TreeView.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.TreeViewAutomationPeer", Justification = "Don't need to use it")]
        public virtual void CreateTreeViewPeerWithNull()
        {
            new TreeViewAutomationPeer(null);
        }

        /// <summary>
        /// Verify that TreeView creates a TreeViewAutomationPeer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeView creates a TreeViewAutomationPeer.")]
        public virtual void TreeViewCreatesAutomationPeer()
        {
            TreeView view = new TreeView();
            TreeViewAutomationPeer peer = null;
            TestAsync(
                view,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(view) as TreeViewAutomationPeer,
                () => Assert.IsNotNull(peer, "TreeView peer should not be null!"),
                () => Assert.AreEqual(view, peer.Owner, "TreeView should be owner of the peer!"));
        }

        /// <summary>
        /// Verify the TreeViewAutomationPeer's type and class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewAutomationPeer's type and class.")]
        public virtual void TreeViewAutomationPeerTypeAndClass()
        {
            TreeView view = new TreeView();
            TreeViewAutomationPeer peer = null;
            TestAsync(
                view,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(view) as TreeViewAutomationPeer,
                () => Assert.AreEqual(AutomationControlType.Tree, peer.GetAutomationControlType(), "Unexpected AutomationControlType!"),
                () => Assert.AreEqual("TreeView", peer.GetClassName(), "Unexpected ClassType!"));
        }

        /// <summary>
        /// Verify the TreeViewAutomationPeer returns null for its children when
        /// there are no items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewAutomationPeer returns null for its children when there are no items.")]
        public virtual void TreeViewPeerGetNoItems()
        {
            TreeView view = new TreeView();
            TreeViewAutomationPeer peer = null;
            TestAsync(
                view,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(view) as TreeViewAutomationPeer,
                () => Assert.IsNull(peer.GetChildren(), "There should be no children when the TreeView does not have items!"));
        }

        /// <summary>
        /// Verify the TreeViewAutomationPeer returns wrappers for its items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewAutomationPeer returns wrappers for its children.")]
        public virtual void TreeViewPeerGetWithItems()
        {
            TreeView view = new TreeView { ItemsSource = new int[] { 1, 2, 3 } };
            TreeViewAutomationPeer peer = null;
            List<AutomationPeer> items = null;
            TestAsync(
                view,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(view) as TreeViewAutomationPeer,
                () => items = peer.GetChildren(),
                () => Assert.AreEqual(3, items.Count, "Unexpected number of child peers!"),
                () => Assert.IsInstanceOfType(items[0], typeof(TreeViewItemAutomationPeer), "Child peer is not a TreeViewItemAutomationPeer!"));
        }

        /// <summary>
        /// Verify that TreeViewAutomationPeer only supports scrolling and
        /// selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewAutomationPeer only supports scrolling and selection.")]
        public virtual void TreeViewPeerOnlySupportsScrollingAndSelection()
        {
            TreeView view = new TreeView();
            TreeViewAutomationPeer peer = null;
            TestAsync(
                view,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(view) as TreeViewAutomationPeer,
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Dock), "TreeViewAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.ExpandCollapse), "TreeViewAutomationPeer should not support the ExpandCollapse pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Grid), "TreeViewAutomationPeer should not support the Grid pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.GridItem), "TreeViewAutomationPeer should not support the GridItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Invoke), "TreeViewAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.MultipleView), "TreeViewAutomationPeer should not support the MultipleView pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.RangeValue), "TreeViewAutomationPeer should not support the RangeValue pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.ScrollItem), "TreeViewAutomationPeer should not support the ScrollItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.SelectionItem), "TreeViewAutomationPeer should not support the SelectionItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Table), "TreeViewAutomationPeer should not support the Table pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.TableItem), "TreeViewAutomationPeer should not support the TableItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Toggle), "TreeViewAutomationPeer should not support the Toggle pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Transform), "TreeViewAutomationPeer should not support the Transform pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Value), "TreeViewAutomationPeer should not support the Value pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Window), "TreeViewAutomationPeer should not support the Window pattern!"));
        }

        /// <summary>
        /// Verify that TreeViewAutomationPeer supports scrolling.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewAutomationPeer supports scrolling.")]
        public virtual void TreeViewPeerSupportsScrolling()
        {
            // Note: This test fails if ItemsSource isn't set because it
            // requires a container to find the ItemsHost and ScrollHost.

            TreeView view = new TreeView { ItemsSource = new int[] { 1, 2, 3 } };
            TreeViewAutomationPeer peer = null;
            IScrollProvider provider = null;
            TestAsync(
                view,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(view) as TreeViewAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.Scroll) as IScrollProvider,
                () => Assert.IsNotNull(provider, "IScrollProvider peer should not be null!"),
                () =>
                {
                    FrameworkElementAutomationPeer scroll = provider as FrameworkElementAutomationPeer;
                    Assert.IsNotNull(scroll, "IScrollProvider should be an automation peer!");
                    Assert.IsInstanceOfType(scroll.Owner, typeof(ScrollViewer), "IScrollProvider should wrap a ScrollViewer!");

                    for (UIElement current = scroll.Owner; current != null; current = VisualTreeHelper.GetParent(current) as UIElement)
                    {
                        if (current == view)
                        {
                            return;
                        }
                    }
                    Assert.Fail("IScrollProvider should be an automation peer for a child of the TreeView!");
                });
        }

        /// <summary>
        /// Verify that TreeViewAutomationPeer does not provide scrolling when
        /// there is no ScrollHost.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewAutomationPeer does not provide scrolling when there is no ScrollHost.")]
        public virtual void TreeViewPeerSupportsScrollingNoHost()
        {
            TreeView view = new TreeView();
            TreeViewAutomationPeer peer = null;
            IScrollProvider provider = null;
            TestAsync(
                view,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(view) as TreeViewAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.Scroll) as IScrollProvider,
                () => Assert.IsNull(provider, "IScrollProvider peer should be null!"));
        }

        #region ISelectionProvider
        /// <summary>
        /// Verify that TreeViewAutomationPeer implements the ISelectionProvider
        /// interface.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewAutomationPeer implements the ISelectionProvider interface.")]
        public virtual void TreeViewPeerIsISelectionProvider()
        {
            TreeView view = new TreeView();
            ISelectionProvider provider = null;
            TestAsync(
                view,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(view) as ISelectionProvider,
                () => Assert.IsNotNull(provider, "TreeViewAutomationPeer should implement ISelectionProvider!"));
        }

        /// <summary>
        /// Verify that TreeViewAutomationPeer supports the Selection pattern.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TreeViewAutomationPeer supports the Selection pattern.")]
        public virtual void TreeViewPeerSupportsSelection()
        {
            TreeView view = new TreeView();
            TreeViewAutomationPeer peer = null;
            ISelectionProvider provider = null;
            TestAsync(
                view,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(view) as TreeViewAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider,
                () => Assert.IsNotNull(provider, "ISelectionProvider peer should not be null!"));
        }

        /// <summary>
        /// Verify the TreeViewAutomationPeer does not support multiple
        /// selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewAutomationPeer does not support multiple selection.")]
        public virtual void TreeViewPeerDoesNotSupportMultipleSelection()
        {
            TreeView view = new TreeView();
            ISelectionProvider provider = null;
            TestAsync(
                view,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(view) as ISelectionProvider,
                () => Assert.IsFalse(provider.CanSelectMultiple, "TreeViewAutomationPeer should not support multi-selection!"));
        }

        /// <summary>
        /// Verify the TreeViewAutomationPeer does not require selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewAutomationPeer does not require selection.")]
        public virtual void TreeViewPeerDoesNotRequireSelection()
        {
            TreeView view = new TreeView();
            ISelectionProvider provider = null;
            TestAsync(
                view,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(view) as ISelectionProvider,
                () => Assert.IsFalse(provider.IsSelectionRequired, "TreeViewAutomationPeer does not require selection!"));
        }

        /// <summary>
        /// Verify the TreeViewAutomationPeer returns an empty list when nothing
        /// is selected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewAutomationPeer returns an empty list when nothing is selected.")]
        public virtual void TreeViewPeerGetSelectionEmpty()
        {
            TreeView view = new TreeView();
            ISelectionProvider provider = null;
            IRawElementProviderSimple[] selection = null;
            TestAsync(
                view,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(view) as ISelectionProvider,
                () => selection = provider.GetSelection(),
                () => Assert.IsNotNull(selection, "An empty selection was expected!"),
                () => Assert.AreEqual(0, selection.Length, "No items should be selected!"));
        }

        /// <summary>
        /// Verify the TreeViewAutomationPeer does not return the selected item
        /// if it has no peer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewAutomationPeer does not return the selected item if it has no peer.")]
        public virtual void TreeViewPeerGetSelectionNoPeer()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            view.Items.Add(second);

            ISelectionProvider provider = null;
            IRawElementProviderSimple[] selection = null;
            bool peersAutomaticallyCreated = false;
            TestAsync(
                view,
                () => first.IsSelected = true,
                () => peersAutomaticallyCreated = (FrameworkElementAutomationPeer.FromElement(first) as TreeViewItemAutomationPeer) != null,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(view) as ISelectionProvider,
                () => selection = provider.GetSelection(),
                () => Assert.AreEqual(peersAutomaticallyCreated ? 1 : 0, selection.Length, "No items should be selected (unless peers are automatically being created!"));
        }

        /// <summary>
        /// Verify the TreeViewAutomationPeer returns the correct selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewAutomationPeer returns the correct selection.")]
        public virtual void TreeViewPeerGetSelection()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            view.Items.Add(second);

            ISelectionProvider provider = null;
            IRawElementProviderSimple[] selection = null;
            TestAsync(
                view,
                () => first.IsSelected = true,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(view) as ISelectionProvider,
                () => FrameworkElementAutomationPeer.CreatePeerForElement(first),
                () => selection = provider.GetSelection(),
                () => Assert.AreEqual(1, selection.Length, "Expected a selection!"));
        }

        /// <summary>
        /// Verify the TreeViewAutomationPeer responds to selection updates.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TreeViewAutomationPeer responds to selection updates.")]
        public virtual void TreeViewPeerGetSelectionChanged()
        {
            TreeView view = new TreeView();
            TreeViewItem first = new TreeViewItem { Header = "First" };
            TreeViewItem second = new TreeViewItem { Header = "Second" };
            view.Items.Add(first);
            view.Items.Add(second);

            ISelectionProvider provider = null;
            IRawElementProviderSimple[] selection = null;
            TestAsync(
                view,
                () => first.IsSelected = true,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(view) as ISelectionProvider,
                () => FrameworkElementAutomationPeer.CreatePeerForElement(first),
                () => selection = provider.GetSelection(),
                () => Assert.AreEqual(1, selection.Length, "Expected a selection!"),
                () => first.IsSelected = false,
                () => selection = provider.GetSelection(),
                () => Assert.IsNotNull(selection, "An empty selection was expected!"),
                () => Assert.AreEqual(0, selection.Length, "No items should be selected!"));
        }
        #endregion ISelectionProvider
    }
}