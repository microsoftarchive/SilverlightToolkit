// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public class AccordionAutomationPeerTest : TestBase
    {
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="AccordionAutomationPeerTest"/> class.
        /// </summary>
        public AccordionAutomationPeerTest()
            : base()
        {
        }

        /// <summary>
        /// Create a new AccordionAutomationPeer.
        /// </summary>
        [TestMethod]
        [Description("Create a new AccordionAutomationPeer.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.AccordionAutomationPeer", Justification = "Don't need to use it")]
        public virtual void CreateAccordionPeer()
        {
            Accordion acc = new Accordion();
            new AccordionAutomationPeer(acc);
        }
        
        /// <summary>
        /// Create a new AccordionAutomationPeer with a null Accordion.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Create a new AccordionAutomationPeer with a null Accordion.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.AccordionAutomationPeer", Justification = "Don't need to use it")]
        public virtual void CreateAccordionPeerWithNull()
        {
            new AccordionAutomationPeer(null);
        }

        /// <summary>
        /// Verify that Accordion creates a AccordionAutomationPeer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that Accordion creates a AccordionAutomationPeer.")]
        public virtual void AccordionCreatesAutomationPeer()
        {
            Accordion acc = new Accordion();
            AccordionAutomationPeer peer = null;
            TestAsync(
                acc,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as AccordionAutomationPeer,
                () => Assert.IsNotNull(peer, "Accordion peer should not be null!"),
                () => Assert.AreEqual(acc, peer.Owner, "Accordion should be owner of the peer!"));
        }

        /// <summary>
        /// Verify the AccordionAutomationPeer's type and class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionAutomationPeer's type and class.")]
        public virtual void AccordionAutomationPeerTypeAndClass()
        {
            Accordion acc = new Accordion();
            AccordionAutomationPeer peer = null;
            TestAsync(
                acc,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as AccordionAutomationPeer,
                () => Assert.AreEqual(AutomationControlType.List, peer.GetAutomationControlType(), "Unexpected AutomationControlType!"),
                () => Assert.AreEqual("Accordion", peer.GetClassName(), "Unexpected ClassType!"));
        }

        /// <summary>
        /// Verify the AccordionAutomationPeer returns null for its children when
        /// there are no items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionAutomationPeer returns null for its children when there are no items.")]
        public virtual void AccordionPeerGetNoItems()
        {
            Accordion acc = new Accordion();
            AccordionAutomationPeer peer = null;
            TestAsync(
                acc,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as AccordionAutomationPeer,
                () => Assert.IsNull(peer.GetChildren(), "There should be no children when the Accordion does not have items!"));
        }

        /// <summary>
        /// Verify the AccordionAutomationPeer returns wrappers for its items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionAutomationPeer returns wrappers for its children.")]
        public virtual void AccordionPeerGetWithItems()
        {
            Accordion view = new Accordion { ItemsSource = new[] { 1, 2, 3 } };
            AccordionAutomationPeer peer = null;
            List<AutomationPeer> items = null;
            TestAsync(
                view,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(view) as AccordionAutomationPeer,
                () => items = peer.GetChildren(),
                () => Assert.AreEqual(3, items.Count, "Unexpected number of child peers!"),
                () => Assert.IsInstanceOfType(items[0], typeof(AccordionItemAutomationPeer), "Child peer is not an AccordionItemAutomationPeer!"));
        }

        /// <summary>
        /// Verify that AccordionAutomationPeer only supports selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that AccordionAutomationPeer only supports selection.")]
        public virtual void AccordionPeerOnlySupportsSelection()
        {
            Accordion view = new Accordion();
            AccordionAutomationPeer peer = null;
            TestAsync(
                view,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(view) as AccordionAutomationPeer,
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Dock), "AccordionAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.ExpandCollapse), "AccordionAutomationPeer should not support the ExpandCollapse pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Grid), "AccordionAutomationPeer should not support the Grid pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.GridItem), "AccordionAutomationPeer should not support the GridItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Invoke), "AccordionAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.MultipleView), "AccordionAutomationPeer should not support the MultipleView pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.RangeValue), "AccordionAutomationPeer should not support the RangeValue pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.ScrollItem), "AccordionAutomationPeer should not support the ScrollItem pattern!"),
                () => Assert.IsNotNull(peer.GetPattern(PatternInterface.Selection), "AccordionAutomationPeer should support the Selection pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.SelectionItem), "AccordionAutomationPeer should not support the SelectionItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Table), "AccordionAutomationPeer should not support the Table pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.TableItem), "AccordionAutomationPeer should not support the TableItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Toggle), "AccordionAutomationPeer should not support the Toggle pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Transform), "AccordionAutomationPeer should not support the Transform pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Value), "AccordionAutomationPeer should not support the Value pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Window), "AccordionAutomationPeer should not support the Window pattern!"));
        }

        #region ISelectionProvider
        /// <summary>
        /// Verify that AccordionAutomationPeer implements the ISelectionProvider
        /// interface.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that AccordionAutomationPeer implements the ISelectionProvider interface.")]
        public virtual void AccordionPeerIsISelectionProvider()
        {
            Accordion acc = new Accordion();
            ISelectionProvider provider = null;
            TestAsync(
                acc,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as ISelectionProvider,
                () => Assert.IsNotNull(provider, "AccordionAutomationPeer should implement ISelectionProvider!"));
        }

        /// <summary>
        /// Verify that AccordionAutomationPeer supports the Selection pattern.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that AccordionAutomationPeer supports the Selection pattern.")]
        public virtual void AccordionPeerSupportsSelection()
        {
            Accordion acc = new Accordion();
            AccordionAutomationPeer peer = null;
            ISelectionProvider provider = null;
            TestAsync(
                acc,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as AccordionAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider,
                () => Assert.IsNotNull(provider, "ISelectionProvider peer should not be null!"));
        }

        /// <summary>
        /// Verify the AccordionAutomationPeer does support multiple
        /// selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionAutomationPeer does not support multiple selection.")]
        public virtual void AccordionPeerDoesSupportMultipleSelection()
        {
            Accordion view = new Accordion();
            view.SelectionMode = AccordionSelectionMode.OneOrMore;
            ISelectionProvider provider = null;
            TestAsync(
                view,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(view) as ISelectionProvider,
                () => Assert.IsTrue(provider.CanSelectMultiple, "AccordionAutomationPeer should support multi-selection when in OneOrMoreMode!"),
                () => view.SelectionMode = AccordionSelectionMode.One,
                () => Assert.IsFalse(provider.CanSelectMultiple, "AccordionAutomationPeer should not support multi-selection when in OneMode!"),
                () => view.SelectionMode = AccordionSelectionMode.ZeroOrMore,
                () => Assert.IsTrue(provider.CanSelectMultiple, "AccordionAutomationPeer should support multi-selection when in ZeroOrMoreMode!"),
                () => view.SelectionMode = AccordionSelectionMode.ZeroOrOne,
                () => Assert.IsFalse(provider.CanSelectMultiple, "AccordionAutomationPeer should not support multi-selection when in ZeroOrOneMode!"));
        }

        /// <summary>
        /// Verify the AccordionAutomationPeer does not require selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionAutomationPeer does not require selection.")]
        public virtual void AccordionPeerDoesNotRequireSelection()
        {
            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.ZeroOrOne;
            ISelectionProvider provider = null;
            TestAsync(
                acc,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as ISelectionProvider,
                () => Assert.IsFalse(provider.IsSelectionRequired, "AccordionAutomationPeer does not require selection!"),
                () => acc.SelectionMode = AccordionSelectionMode.One,
                () => Assert.IsTrue(provider.IsSelectionRequired, "AccordionAutomationPeer does require selection!"));
        }

        /// <summary>
        /// Verify the AccordionAutomationPeer returns an empty list when nothing
        /// is selected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionAutomationPeer returns an empty list when nothing is selected.")]
        public virtual void AccordionPeerGetSelectionEmpty()
        {
            Accordion acc = new Accordion();
            ISelectionProvider provider = null;
            IRawElementProviderSimple[] selection = null;
            TestAsync(
                acc,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as ISelectionProvider,
                () => selection = provider.GetSelection(),
                () => Assert.IsNotNull(selection, "An empty selection was expected!"),
                () => Assert.AreEqual(0, selection.Length, "No items should be selected!"));
        }

        /// <summary>
        /// Verify the AccordionAutomationPeer does not return the selected item
        /// if it has no peer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionAutomationPeer does not return the selected item if it has no peer.")]
        public virtual void AccordionPeerGetSelectionNoPeer()
        {
            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.One;
            AccordionItem first = new AccordionItem { Header = "First", Content = "a" };
            AccordionItem second = new AccordionItem { Header = "Second", Content = "b" };
            acc.Items.Add(first);
            acc.Items.Add(second);

            ISelectionProvider provider = null;
            IRawElementProviderSimple[] selection = null;
            TestAsync(
                acc,
                () => second.IsSelected = true,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as ISelectionProvider,
                () => selection = provider.GetSelection(),
                () => Assert.AreEqual(0, selection.Length, "No items should be selected!"));
        }

        /// <summary>
        /// Verify the AccordionAutomationPeer returns the correct selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionAutomationPeer returns the correct selection.")]
        public virtual void AccordionPeerGetSelection()
        {
            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.One;
            AccordionItem first = new AccordionItem { Header = "First", Content = "a" };
            AccordionItem second = new AccordionItem { Header = "Second", Content = "b" };
            acc.Items.Add(first);
            acc.Items.Add(second);

            ISelectionProvider provider = null;
            IRawElementProviderSimple[] selection = null;
            TestAsync(
                acc,
                () => second.IsSelected = true,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as ISelectionProvider,
                () => FrameworkElementAutomationPeer.CreatePeerForElement(second),
                () => selection = provider.GetSelection(),
                () => Assert.AreEqual(1, selection.Length, "Expected a selection!"));
        }

        /// <summary>
        /// Verify the AccordionAutomationPeer responds to selection updates.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AccordionAutomationPeer responds to selection updates.")]
        public virtual void AccordionPeerGetSelectionChanged()
        {
            Accordion acc = new Accordion();
            acc.SelectionMode = AccordionSelectionMode.ZeroOrMore;
            AccordionItem first = new AccordionItem { Header = "First", Content = "a" };
            AccordionItem second = new AccordionItem { Header = "Second", Content = "b" };
            acc.Items.Add(first);
            acc.Items.Add(second);

            ISelectionProvider provider = null;
            IRawElementProviderSimple[] selection = null;
            TestAsync(
                acc,
                () => second.IsSelected = true,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as ISelectionProvider,
                () => FrameworkElementAutomationPeer.CreatePeerForElement(second),
                () => selection = provider.GetSelection(),
                () => Assert.AreEqual(1, selection.Length, "Expected a selection!"),
                () => second.IsSelected = false,
                () => selection = provider.GetSelection(),
                () => Assert.IsNotNull(selection, "An empty selection was expected!"),
                () => Assert.AreEqual(0, selection.Length, "No items should be selected!"));
        }
        #endregion ISelectionProvider
    }
}
