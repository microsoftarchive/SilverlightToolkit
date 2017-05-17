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
    /// RatingAutomationPeer unit tests.
    /// </summary>
    [TestClass]
    [Tag("RatingTest")]
    [Tag("Automation")]
    public class RatingAutomationPeerTest : TestBase
    {
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="RatingAutomationPeerTest"/> class.
        /// </summary>
        public RatingAutomationPeerTest()
            : base()
        {
        }

        /// <summary>
        /// Create a new RatingAutomationPeer.
        /// </summary>
        [TestMethod]
        [Description("Create a new RatingAutomationPeer.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.RatingAutomationPeer", Justification = "Don't need to use it")]
        public virtual void CreateRatingPeer()
        {
            Rating acc = new Rating();
            new RatingAutomationPeer(acc);
        }
        
        /// <summary>
        /// Create a new RatingAutomationPeer with a null Rating.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Create a new RatingAutomationPeer with a null Rating.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.RatingAutomationPeer", Justification = "Don't need to use it")]
        public virtual void CreateRatingPeerWithNull()
        {
            new RatingAutomationPeer(null);
        }

        /// <summary>
        /// Verify that Rating creates a RatingAutomationPeer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that Rating creates a RatingAutomationPeer.")]
        public virtual void RatingCreatesAutomationPeer()
        {
            Rating acc = new Rating();
            RatingAutomationPeer peer = null;
            TestAsync(
                acc,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as RatingAutomationPeer,
                () => Assert.IsNotNull(peer, "Rating peer should not be null!"),
                () => Assert.AreEqual(acc, peer.Owner, "Rating should be owner of the peer!"));
        }

        /// <summary>
        /// Verify the RatingAutomationPeer's type and class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingAutomationPeer's type and class.")]
        public virtual void RatingAutomationPeerTypeAndClass()
        {
            Rating acc = new Rating();
            RatingAutomationPeer peer = null;
            TestAsync(
                acc,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as RatingAutomationPeer,
                () => 
                    {
                        AutomationControlType controlType = peer.GetAutomationControlType();

                        Assert.AreEqual(controlType, AutomationControlType.Slider, "Unexpected AutomationControlType!");
                    });
        }

        /// <summary>
        /// Verify the RatingAutomationPeer returns null for its children when
        /// there are no items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingAutomationPeer returns null for its children when there are no items.")]
        public virtual void RatingPeerGetNoItems()
        {
            Rating acc = new Rating();
            acc.ItemsSource = null;
            RatingAutomationPeer peer = null;
            TestAsync(
                acc,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as RatingAutomationPeer,
                () => Assert.IsNull(peer.GetChildren(), "There should be no children when the Rating does not have items!"));
        }

        /// <summary>
        /// Verify the RatingAutomationPeer returns wrappers for its items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingAutomationPeer returns wrappers for its children.")]
        public virtual void RatingPeerGetWithItems()
        {
            Rating view = new Rating();
            view.ItemCount = 3;
            RatingAutomationPeer peer = null;
            List<AutomationPeer> items = null;
            TestAsync(
                view,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(view) as RatingAutomationPeer,
                () => items = peer.GetChildren(),
                () => Assert.AreEqual(3, items.Count, "Unexpected number of child peers!"),
                () => Assert.IsInstanceOfType(items[0], typeof(RatingItemAutomationPeer), "Child peer is not an RatingItemAutomationPeer!"));
        }

        /// <summary>
        /// Verify that RatingAutomationPeer only supports selection and value.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that RatingAutomationPeer only supports selection and value.")]
        public virtual void RatingPeerOnlySupportsSelectionAndValue()
        {
            Rating view = new Rating();
            RatingAutomationPeer peer = null;
            TestAsync(
                view,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(view) as RatingAutomationPeer,
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Dock), "RatingAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.ExpandCollapse), "RatingAutomationPeer should not support the ExpandCollapse pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Grid), "RatingAutomationPeer should not support the Grid pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.GridItem), "RatingAutomationPeer should not support the GridItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Invoke), "RatingAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.MultipleView), "RatingAutomationPeer should not support the MultipleView pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.RangeValue), "RatingAutomationPeer should not support the RangeValue pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.ScrollItem), "RatingAutomationPeer should not support the ScrollItem pattern!"),
                () => Assert.IsNotNull(peer.GetPattern(PatternInterface.Selection), "RatingAutomationPeer should support the Selection pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.SelectionItem), "RatingAutomationPeer should not support the SelectionItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Table), "RatingAutomationPeer should not support the Table pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.TableItem), "RatingAutomationPeer should not support the TableItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Toggle), "RatingAutomationPeer should not support the Toggle pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Transform), "RatingAutomationPeer should not support the Transform pattern!"),
                () => Assert.IsNotNull(peer.GetPattern(PatternInterface.Value), "RatingAutomationPeer should support the Value pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Window), "RatingAutomationPeer should not support the Window pattern!"));
        }

        /// <summary>
        /// Verify that RatingAutomationPeer implements the ISelectionProvider
        /// interface.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that RatingAutomationPeer implements the ISelectionProvider interface.")]
        public virtual void RatingPeerIsISelectionProvider()
        {
            Rating acc = new Rating();
            ISelectionProvider provider = null;
            TestAsync(
                acc,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as ISelectionProvider,
                () => Assert.IsNotNull(provider, "RatingAutomationPeer should implement ISelectionProvider!"));
        }

        /// <summary>
        /// Verify the RatingAutomationPeer does not require selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingAutomationPeer does not require selection.")]
        public virtual void RatingPeerDoesNotRequireSelection()
        {
            Rating acc = new Rating();
            ISelectionProvider provider = null;
            TestAsync(
                acc,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as ISelectionProvider,
                () => Assert.IsFalse(provider.IsSelectionRequired, "RatingAutomationPeer does not require selection!"));
        }

        /// <summary>
        /// Verify that RatingAutomationPeer supports the Selection pattern.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that RatingAutomationPeer supports the Selection pattern.")]
        public virtual void RatingPeerSupportsSelection()
        {
            Rating acc = new Rating();
            RatingAutomationPeer peer = null;
            ISelectionProvider provider = null;
            TestAsync(
                acc,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as RatingAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.Selection) as ISelectionProvider,
                () => Assert.IsNotNull(provider, "ISelectionProvider peer should not be null!"));
        }

        /// <summary>
        /// Verify the RatingAutomationPeer does support multiple
        /// selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingAutomationPeer does not support multiple selection.")]
        public virtual void RatingPeerDoesNotSupportMultipleSelection()
        {
            Rating view = new Rating();
            ISelectionProvider provider = null;
            TestAsync(
                view,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(view) as ISelectionProvider,
                () => Assert.IsFalse(provider.CanSelectMultiple, "RatingAutomationPeer shouldn't support multi-selection"));
        }

        /// <summary>
        /// Verify the RatingAutomationPeer returns an empty list when nothing
        /// is selected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingAutomationPeer returns an empty list when nothing is selected.")]
        public virtual void RatingPeerGetSelectionEmpty()
        {
            Rating acc = new Rating();
            acc.ItemsSource = null;
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
        /// Verify the RatingAutomationPeer does not return the selected item
        /// if it has no peer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingAutomationPeer does not return the selected item if it has no peer.")]
        public virtual void RatingPeerGetSelectionNoPeer()
        {
            Rating acc = new Rating();
            acc.ItemsSource = null;
            acc.SelectionMode = RatingSelectionMode.Individual;
            RatingItem first = new RatingItem();
            RatingItem second = new RatingItem();
            acc.Items.Add(first);
            acc.Items.Add(second);

            ISelectionProvider provider = null;
            IRawElementProviderSimple[] selection = null;
            TestAsync(
                acc,
                () => acc.Value = 0.0,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as ISelectionProvider,
                () => selection = provider.GetSelection(),
                () => Assert.AreEqual(0, selection.Length, "No items should be selected!"));
        }

        /// <summary>
        /// Verify the RatingAutomationPeer returns the correct selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingAutomationPeer returns the correct selection.")]
        public virtual void RatingPeerGetSelection()
        {
            Rating acc = new Rating();
            acc.ItemsSource = null;
            RatingItem first = new RatingItem();
            RatingItem second = new RatingItem();
            acc.Items.Add(first);
            acc.Items.Add(second);

            ISelectionProvider provider = null;
            IRawElementProviderSimple[] selection = null;
            TestAsync(
                acc,
                () => acc.Value = 1.0,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as ISelectionProvider,
                () => FrameworkElementAutomationPeer.CreatePeerForElement(second),
                () => selection = provider.GetSelection(),
                () => Assert.AreEqual(1, selection.Length, "Expected a selection!"));
        }

        /// <summary>
        /// Verify the RatingAutomationPeer responds to selection updates.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingAutomationPeer responds to selection updates.")]
        public virtual void RatingPeerGetSelectionChanged()
        {
            Rating acc = new Rating();
            acc.ItemsSource = null;
            RatingItem first = new RatingItem();
            RatingItem second = new RatingItem();
            acc.Items.Add(first);
            acc.Items.Add(second);

            ISelectionProvider provider = null;
            IRawElementProviderSimple[] selection = null;
            TestAsync(
                acc,
                () => acc.Value = 1.0,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as ISelectionProvider,
                () => FrameworkElementAutomationPeer.CreatePeerForElement(second),
                () => selection = provider.GetSelection(),
                () => Assert.AreEqual(1, selection.Length, "Expected a selection!"),
                () => acc.Value = 0.0,
                () => selection = provider.GetSelection(),
                () => Assert.IsNotNull(selection, "An empty selection was expected!"),
                () => Assert.AreEqual(0, selection.Length, "No items should be selected!"));
        }

        /////// <summary>
        /////// Verify the RatingAutomationPeer throws InvalidOperationException.
        /////// </summary>
        ////[TestMethod]
        ////[Asynchronous]
        ////[Description("Verify the RatingAutomationPeer responds to selection updates.")]
        ////[ExpectedException(typeof(InvalidOperationException))]
        ////public virtual void RatingPeerAutomationPeerSelections()
        ////{
        ////    Rating acc = new Rating();
        ////    acc.ItemsSource = null;
        ////    RatingItem first = new RatingItem();
        ////    RatingItem second = new RatingItem();
        ////    acc.Items.Add(first);
        ////    acc.Items.Add(second);

        ////    ISelectionProvider provider = null;
        ////    IRawElementProviderSimple[] selection = null;
        ////    TestAsync(
        ////        acc,
        ////        () => acc.Value = 1.0,
        ////        () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(acc) as ISelectionProvider,
        ////        () => provider.
        ////        () => Assert.AreEqual(0, selection.Length, "No items should be selected!"));
        ////}
        ////#endregion ISelectionProvider
    }
}
