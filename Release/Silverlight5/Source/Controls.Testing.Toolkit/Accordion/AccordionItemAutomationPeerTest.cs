// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// AccordionAutomationPeer unit tests.
    /// </summary>
    [TestClass]
    [Tag("Accordion")]
    [Tag("Automation")]
    public class AccordionItemAutomationPeerTest : TestBase
    {
        /// <summary>
        /// Initializes a new instance of the AccordionItemAutomationPeerTest
        /// class.
        /// </summary>
        public AccordionItemAutomationPeerTest()
            : base()
        {
        }

        /// <summary>
        /// Create a new AccordionItemAutomationPeer.
        /// </summary>
        [TestMethod]
        [Description("Create a new AccordionItemAutomationPeer.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.AccordionItemAutomationPeer", Justification = "Don't need to use it")]
        public virtual void CreateAccordionItemPeer()
        {
            AccordionItem item = new AccordionItem();
            new AccordionItemAutomationPeer(item);
        }

        /// <summary>
        /// Create a new AccordionItemAutomationPeer with a null AccordionItem.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Create a new AccordionItemAutomationPeer with a null AccordionItem.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.AccordionItemAutomationPeer", Justification = "Don't need to use it")]
        public virtual void CreateAccordionItemPeerWithNull()
        {
            new AccordionItemAutomationPeer(null);
        }

        /// <summary>
        /// Verify that AccordionItem creates a AccordionItemAutomationPeer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that AccordionItem creates a AccordionItemAutomationPeer.")]
        public virtual void AccordionItemCreatesAutomationPeer()
        {
            AccordionItem item = new AccordionItem();
            AccordionItemAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as AccordionItemAutomationPeer,
                () => Assert.IsNotNull(peer, "AccordionItem peer should not be null!"),
                () => Assert.AreEqual(item, peer.Owner, "AccordionItem should be owner of the peer!"));
        }

        /// <summary>
        /// Verify the AccordionItemAutomationPeer's type and class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionItemAutomationPeer's type and class.")]
        public virtual void AccordionItemAutomationPeerTypeAndClass()
        {
            AccordionItem item = new AccordionItem();
            AccordionItemAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as AccordionItemAutomationPeer,
                () => Assert.AreEqual(AutomationControlType.ListItem, peer.GetAutomationControlType(), "Unexpected AutomationControlType!"),
                () => Assert.AreEqual("AccordionItem", peer.GetClassName(), "Unexpected ClassType!"));
        }

        /// <summary>
        /// Verify that AccordionItemAutomationPeer only supports expand collapse and
        /// selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that AccordionItemAutomationPeer only supports expandcollapse and selection.")]
        public virtual void AccordionItemPeerOnlySupportsSelectionAndExpandCollapse()
        {
            AccordionItem item = new AccordionItem();
            AccordionItemAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as AccordionItemAutomationPeer,
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Dock), "AccordionItemAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Grid), "AccordionItemAutomationPeer should not support the Grid pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.GridItem), "AccordionItemAutomationPeer should not support the GridItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Invoke), "AccordionItemAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.MultipleView), "AccordionItemAutomationPeer should not support the MultipleView pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.RangeValue), "AccordionItemAutomationPeer should not support the RangeValue pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Scroll), "AccordionItemAutomationPeer should not support the Scroll pattern!"),
                () => Assert.IsNotNull(peer.GetPattern(PatternInterface.SelectionItem), "AccordionItemAutomationPeer should support the SelectionItem pattern!"),
                () => Assert.IsNotNull(peer.GetPattern(PatternInterface.ExpandCollapse), "AccordionItemAutomationPeer should support the ExpandCollapse pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Table), "AccordionItemAutomationPeer should not support the Table pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.TableItem), "AccordionItemAutomationPeer should not support the TableItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Toggle), "AccordionItemAutomationPeer should not support the Toggle pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Transform), "AccordionItemAutomationPeer should not support the Transform pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Value), "AccordionItemAutomationPeer should not support the Value pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Window), "AccordionItemAutomationPeer should not support the Window pattern!"));
        }

        #region ISelectionItemProvider
        /// <summary>
        /// Verify that AccordionItemAutomationPeer implements the
        /// ISelectionItemProvider interface.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that AccordionItemAutomationPeer implements the ISelectionItemProvider interface.")]
        public virtual void AccordionItemPeerIsISelectionItemProvider()
        {
            AccordionItem item = new AccordionItem();
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => Assert.IsNotNull(provider, "AccordionItemAutomationPeer should implement ISelectionItemProvider!"));
        }

        /// <summary>
        /// Verify that AccordionItemAutomationPeer supports the Selection
        /// pattern.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that AccordionItemAutomationPeer supports the Selection pattern.")]
        public virtual void AccordionItemPeerSupportsSelection()
        {
            AccordionItem item = new AccordionItem();
            AccordionItemAutomationPeer peer = null;
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as AccordionItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider,
                () => Assert.IsNotNull(provider, "ISelectionItemProvider peer should not be null!"));
        }

        /// <summary>
        /// Verify the AccordionItemAutomationPeer provides IsSelected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionItemAutomationPeer provides IsSelected.")]
        public virtual void AccordionItemPeerIsSelected()
        {
            AccordionItem item = new AccordionItem();
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => Assert.IsFalse(provider.IsSelected, "AccordionItemAutomationPeer should not be selected!"),
                () => item.IsSelected = true,
                () => Assert.IsTrue(provider.IsSelected, "AccordionItemAutomationPeer should be selected!"));
        }

        /// <summary>
        /// Verify the AccordionItemAutomationPeer provides access to the
        /// selection container when it has no parent.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionItemAutomationPeer provides access to the selection container when it has no parent.")]
        public virtual void AccordionItemPeerSelectionContainerNoParent()
        {
            AccordionItem item = new AccordionItem();
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => Assert.IsNull(provider.SelectionContainer, "AccordionItemAutomationPeer should not have a SelectionContainer!"));
        }

        /// <summary>
        /// Verify the AccordionItemAutomationPeer provides access to the
        /// selection container.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionItemAutomationPeer provides access to the selection container.")]
        public virtual void AccordionItemPeerSelectionContainer()
        {
            AccordionItem item = new AccordionItem { Header = "Item", Content = "a" };
            Accordion acc = new Accordion();
            acc.Items.Add(item);

            ISelectionItemProvider provider = null;
            TestAsync(
                acc,
                () => FrameworkElementAutomationPeer.CreatePeerForElement(acc),
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => Assert.IsNotNull(provider.SelectionContainer, "AccordionItemAutomationPeer should have a SelectionContainer!"));
        }

        /// <summary>
        /// Verify the AccordionItemAutomationPeer throws an exception if there
        /// if the item is not in a Accordion in AddToSelection.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Asynchronous]
        [Description("Verify the AccordionItemAutomationPeer throws an exception if there if the item is not in a Accordion.")]
        public virtual void AccordionItemPeerAddSelectionNoAccordion()
        {
            AccordionItem item = new AccordionItem();
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => provider.AddToSelection());
        }

        /// <summary>
        /// Verify the AccordionItemAutomationPeer selects this item on
        /// AddToSelection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionItemAutomationPeer selects this item on AddToSelection.")]
        public virtual void AccordionItemPeerAddSelection()
        {
            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.OneOrMore;
            AccordionItem first = new AccordionItem { Header = "First", Content = "a" };
            AccordionItem second = new AccordionItem { Header = "Second", Content = "b" };
            acc.Items.Add(first);
            acc.Items.Add(second);

            ISelectionItemProvider provider = null;
            TestAsync(
                acc,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(second) as ISelectionItemProvider,
                () => provider.AddToSelection(),
                () => Assert.IsTrue(second.IsSelected, "Second item should have been selected!"));
        }
 
        /// <summary>
        /// Verify the AccordionItemAutomationPeer selects the item in Select.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionItemAutomationPeer selects the item in Select.")]
        public virtual void AccordionItemPeerSelectNoAccordion()
        {
            AccordionItem item = new AccordionItem();
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => provider.Select(),
                () => Assert.IsTrue(item.IsSelected, "Item should be selected!"));
        }

        /// <summary>
        /// Verify the AccordionItemAutomationPeer selects an item when another
        /// was already selected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionItemAutomationPeer selects an item when another was already selected.")]
        public virtual void AccordionItemPeerSelectAlreadySelected()
        {
            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.One;
            AccordionItem first = new AccordionItem { Header = "First" };
            AccordionItem second = new AccordionItem { Header = "Second" };
            acc.Items.Add(first);
            acc.Items.Add(second);

            ISelectionItemProvider provider = null;
            TestAsync(
                acc,
                () => second.IsSelected = true,
                () => Assert.IsFalse(first.IsSelected, "First item should not be selected!"),
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(first) as ISelectionItemProvider,
                () => provider.Select(),
                () => Thread.Sleep(40),
                () => Assert.IsTrue(first.IsSelected, "First item should be selected!"),
                () => Assert.IsFalse(second.IsSelected, "Second item should not be selected!"));
        }

        /// <summary>
        /// Verify the AccordionItemAutomationPeer selects this item on
        /// AddToSelection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionItemAutomationPeer selects this item on AddToSelection.")]
        public virtual void AccordionItemPeerSelect()
        {
            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.ZeroOrMore;
            AccordionItem first = new AccordionItem { Header = "First" };
            AccordionItem second = new AccordionItem { Header = "Second" };
            acc.Items.Add(first);
            acc.Items.Add(second);

            ISelectionItemProvider provider = null;
            TestAsync(
                acc,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(first) as ISelectionItemProvider,
                () => provider.Select(),
                () => Assert.IsTrue(first.IsSelected, "First item should have been selected!"));
        }

        /// <summary>
        /// Verify the AccordionItemAutomationPeer unselects the item in
        /// RemoveFromSelection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionItemAutomationPeer unselects the item in RemoveFromSelection.")]
        public virtual void AccordionItemPeerRemoveSelectionNoAccordion()
        {
            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.ZeroOrMore;
            AccordionItem first = new AccordionItem { Header = "First" };
            AccordionItem second = new AccordionItem { Header = "Second" };
            acc.Items.Add(first);
            acc.Items.Add(second);

            ISelectionItemProvider provider = null;
            TestAsync(
                acc,
                () => first.IsSelected = true,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(first) as ISelectionItemProvider,
                () => provider.RemoveFromSelection(),
                () => Thread.Sleep(40),
                () => Assert.IsFalse(first.IsSelected, "Item should be not selected!"));
        }
        #endregion ISelectionItemProvider

        #region IExpandCollapseProvider
        /// <summary>
        /// Verify that AccordionItemAutomationPeer implements the
        /// IExpandCollapseProvider interface.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that AccordionItemAutomationPeer implements the IExpandCollapseProvider interface.")]
        public virtual void AccordionItemPeerIsIExpandCollapseProvider()
        {
            AccordionItem item = new AccordionItem();
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as IExpandCollapseProvider,
                () => Assert.IsNotNull(provider, "AccordionItemAutomationPeer should implement IExpandCollapseProvider!"));
        }

        /// <summary>
        /// Verify that AccordionItemAutomationPeer supports the ExpandCollapse
        /// pattern.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that AccordionItemAutomationPeer supports the ExpandCollapse pattern.")]
        public virtual void AccordionItemPeerSupportsExpandCollapse()
        {
            AccordionItem item = new AccordionItem();
            AccordionItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as AccordionItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => Assert.IsNotNull(provider, "IExpandCollapseProvider peer should not be null!"));
        }

        /// <summary>
        /// Verify that AccordionItemAutomationPeer has the right
        /// ExpandCollapseState for an item with no items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that AccordionItemAutomationPeer has the right ExpandCollapseState for an item with no items.")]
        public virtual void AccordionItemPeerExpandStateNoItems()
        {
            AccordionItem item = new AccordionItem();
            AccordionItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as AccordionItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => Assert.AreEqual(ExpandCollapseState.Collapsed, provider.ExpandCollapseState, "Unexpected ExpandCollapseState!"));
        }

        /// <summary>
        /// Verify that AccordionItemAutomationPeer has the right
        /// ExpandCollapseState for an item that is collapsed.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that AccordionItemAutomationPeer has the right ExpandCollapseState for an item that is collapsed.")]
        public virtual void AccordionItemPeerExpandStateCollapsed()
        {
            AccordionItem item = new AccordionItem();
            AccordionItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as AccordionItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => Assert.AreEqual(ExpandCollapseState.Collapsed, provider.ExpandCollapseState, "Unexpected ExpandCollapseState!"));
        }

        /// <summary>
        /// Verify that AccordionItemAutomationPeer has the right
        /// ExpandCollapseState for an item that is expanded.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that AccordionItemAutomationPeer has the right ExpandCollapseState for an item that is expanded.")]
        public virtual void AccordionItemPeerExpandStateExpanded()
        {
            AccordionItem item = new AccordionItem { IsSelected = true };
            AccordionItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as AccordionItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => Assert.AreEqual(ExpandCollapseState.Expanded, provider.ExpandCollapseState, "Unexpected ExpandCollapseState!"));
        }

        /// <summary>
        /// Verify that AccordionItemAutomationPeer throws an exception when
        /// expanding a disabled item.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementNotEnabledException))]
        [Asynchronous]
        [Description("Verify that AccordionItemAutomationPeer throws an exception when expanding a disabled item.")]
        public virtual void AccordionItemPeerExpandDisabled()
        {
            AccordionItem item = new AccordionItem { IsEnabled = false };
            AccordionItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as AccordionItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Expand());
        }

        /// <summary>
        /// Verify that AccordionItemAutomationPeer expands an item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that AccordionItemAutomationPeer expands an item.")]
        public virtual void AccordionItemPeerExpands()
        {
            AccordionItem item = new AccordionItem();
            AccordionItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as AccordionItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Expand(),
                () => Assert.IsTrue(item.IsSelected, "Item should be expanded!"));
        }

        /// <summary>
        /// Verify that AccordionItemAutomationPeer expands an expanded item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that AccordionItemAutomationPeer expands an expanded item.")]
        public virtual void AccordionItemPeerExpandsExpanded()
        {
            AccordionItem item = new AccordionItem { IsSelected = true };
            AccordionItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as AccordionItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Expand(),
                () => Assert.IsTrue(item.IsSelected, "Item should be expanded!"));
        }

        /// <summary>
        /// Verify that AccordionItemAutomationPeer throws an exception when
        /// collapsing a disabled item.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementNotEnabledException))]
        [Asynchronous]
        [Description("Verify that AccordionItemAutomationPeer throws an exception when collapsing a disabled item.")]
        public virtual void AccordionItemPeerCollapseDisabled()
        {
            AccordionItem item = new AccordionItem { IsEnabled = false };
            AccordionItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as AccordionItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Collapse());
        }

        /// <summary>
        /// Verify that AccordionItemAutomationPeer collapses an item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that AccordionItemAutomationPeer collapses an item.")]
        public virtual void AccordionItemPeerCollapse()
        {
            AccordionItem item = new AccordionItem { IsSelected = true };
            AccordionItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as AccordionItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Collapse(),
                () => Thread.Sleep(40),
                () => Assert.IsFalse(item.IsSelected, "Item should be collapsed!"));
        }

        /// <summary>
        /// Verify that AccordionItemAutomationPeer collapses an collapsed item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that AccordionItemAutomationPeer collapses an collapsed item.")]
        public virtual void AccordionItemPeerCollapseCollapsed()
        {
            AccordionItem item = new AccordionItem();
            AccordionItemAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as AccordionItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Collapse(),
                () => Thread.Sleep(40),
                () => Assert.IsFalse(item.IsSelected, "Item should be collapsed!"));
        }
        #endregion IExpandCollapseProvider
    }
}
