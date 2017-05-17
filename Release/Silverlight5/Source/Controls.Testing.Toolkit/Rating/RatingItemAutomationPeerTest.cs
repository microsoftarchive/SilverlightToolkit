// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Threading;
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
    public class RatingItemAutomationPeerTest : TestBase
    {
        /// <summary>
        /// Initializes a new instance of the RatingItemAutomationPeerTest
        /// class.
        /// </summary>
        public RatingItemAutomationPeerTest()
            : base()
        {
        }

        /// <summary>
        /// Create a new RatingItemAutomationPeer.
        /// </summary>
        [TestMethod]
        [Description("Create a new RatingItemAutomationPeer.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.RatingItemAutomationPeer", Justification = "Don't need to use it")]
        public virtual void CreateRatingItemPeer()
        {
            RatingItem item = new RatingItem();
            new RatingItemAutomationPeer(item);
        }

        /// <summary>
        /// Create a new RatingItemAutomationPeer with a null RatingItem.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Create a new RatingItemAutomationPeer with a null RatingItem.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.RatingItemAutomationPeer", Justification = "Don't need to use it")]
        public virtual void CreateRatingItemPeerWithNull()
        {
            new RatingItemAutomationPeer(null);
        }

        /// <summary>
        /// Verify that RatingItem creates a RatingItemAutomationPeer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that RatingItem creates a RatingItemAutomationPeer.")]
        public virtual void RatingItemCreatesAutomationPeer()
        {
            RatingItem item = new RatingItem();
            RatingItemAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as RatingItemAutomationPeer,
                () => Assert.IsNotNull(peer, "RatingItem peer should not be null!"),
                () => Assert.AreEqual(item, peer.Owner, "RatingItem should be owner of the peer!"));
        }

        /// <summary>
        /// Verify the RatingItemAutomationPeer's type and class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingItemAutomationPeer's type and class.")]
        public virtual void RatingItemAutomationPeerTypeAndClass()
        {
            RatingItem item = new RatingItem();
            RatingItemAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as RatingItemAutomationPeer,
                () => Assert.AreEqual(AutomationControlType.ListItem, peer.GetAutomationControlType(), "Unexpected AutomationControlType!"),
                () => Assert.AreEqual("RatingItem", peer.GetClassName(), "Unexpected ClassType!"));
        }

        /// <summary>
        /// Verify that RatingItemAutomationPeer only supports expand collapse and
        /// selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that RatingItemAutomationPeer only supports expandcollapse and selection.")]
        public virtual void RatingItemPeerOnlySupportsSelectionAndExpandCollapse()
        {
            RatingItem item = new RatingItem();
            RatingItemAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as RatingItemAutomationPeer,
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Dock), "RatingItemAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Grid), "RatingItemAutomationPeer should not support the Grid pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.GridItem), "RatingItemAutomationPeer should not support the GridItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Invoke), "RatingItemAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.MultipleView), "RatingItemAutomationPeer should not support the MultipleView pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.RangeValue), "RatingItemAutomationPeer should support the RangeValue pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Scroll), "RatingItemAutomationPeer should not support the Scroll pattern!"),
                () => Assert.IsNotNull(peer.GetPattern(PatternInterface.SelectionItem), "RatingItemAutomationPeer should support the SelectionItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.ExpandCollapse), "RatingItemAutomationPeer should not support the ExpandCollapse pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Table), "RatingItemAutomationPeer should not support the Table pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.TableItem), "RatingItemAutomationPeer should not support the TableItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Toggle), "RatingItemAutomationPeer should not support the Toggle pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Transform), "RatingItemAutomationPeer should not support the Transform pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Value), "RatingItemAutomationPeer should not support the Value pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Window), "RatingItemAutomationPeer should not support the Window pattern!"));
        }

        #region ISelectionItemProvider
        /// <summary>
        /// Verify that RatingItemAutomationPeer implements the
        /// ISelectionItemProvider interface.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that RatingItemAutomationPeer implements the ISelectionItemProvider interface.")]
        public virtual void RatingItemPeerIsISelectionItemProvider()
        {
            RatingItem item = new RatingItem();
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => Assert.IsNotNull(provider, "RatingItemAutomationPeer should implement ISelectionItemProvider!"));
        }

        /// <summary>
        /// Verify that RatingItemAutomationPeer supports the Selection
        /// pattern.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that RatingItemAutomationPeer supports the Selection pattern.")]
        public virtual void RatingItemPeerSupportsSelection()
        {
            RatingItem item = new RatingItem();
            RatingItemAutomationPeer peer = null;
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as RatingItemAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider,
                () => Assert.IsNotNull(provider, "ISelectionItemProvider peer should not be null!"));
        }

        /// <summary>
        /// Verify the RatingItemAutomationPeer provides IsSelected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingItemAutomationPeer provides IsSelected.")]
        public virtual void RatingItemPeerIsSelected()
        {
            Rating rating = new Rating();
            rating.ItemsSource = null;
            RatingItem item = new RatingItem();
            rating.Items.Add(item);
            ISelectionItemProvider provider = null;
            TestAsync(
                rating,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => Assert.IsFalse(provider.IsSelected, "RatingItemAutomationPeer should not be selected!"),
                () => rating.Value = 1.0,
                () => Assert.IsTrue(provider.IsSelected, "RatingItemAutomationPeer should be selected!"));
        }

        /// <summary>
        /// Verify the RatingItemAutomationPeer provides access to the
        /// selection container when it has no parent.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingItemAutomationPeer provides access to the selection container when it has no parent.")]
        public virtual void RatingItemPeerSelectionContainerNoParent()
        {
            RatingItem item = new RatingItem();
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => Assert.IsNull(provider.SelectionContainer, "RatingItemAutomationPeer should not have a SelectionContainer!"));
        }

        /// <summary>
        /// Verify the RatingItemAutomationPeer provides access to the
        /// selection container.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingItemAutomationPeer provides access to the selection container.")]
        public virtual void RatingItemPeerSelectionContainer()
        {
            RatingItem item = new RatingItem();
            Rating acc = new Rating();
            acc.ItemsSource = null;
            acc.Items.Add(item);

            ISelectionItemProvider provider = null;
            TestAsync(
                acc,
                () => FrameworkElementAutomationPeer.CreatePeerForElement(acc),
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => Assert.IsNotNull(provider.SelectionContainer, "RatingItemAutomationPeer should have a SelectionContainer!"));
        }

        /// <summary>
        /// Verify the RatingItemAutomationPeer throws an exception if there
        /// if the item is not in a Rating in AddToSelection.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Asynchronous]
        [Description("Verify the RatingItemAutomationPeer throws an exception if there if the item is not in a Rating.")]
        public virtual void RatingItemPeerAddSelectionNoRating()
        {
            RatingItem item = new RatingItem();
            ISelectionItemProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ISelectionItemProvider,
                () => provider.AddToSelection());
        }

        /// <summary>
        /// Verify the RatingItemAutomationPeer selects this item on
        /// AddToSelection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingItemAutomationPeer selects this item on AddToSelection.")]
        public virtual void RatingItemPeerAddSelection()
        {
            Rating acc = new Rating();
            acc.ItemsSource = null;
            acc.SelectionMode = RatingSelectionMode.Continuous;
            RatingItem first = new RatingItem();
            RatingItem second = new RatingItem();
            acc.Items.Add(first);
            acc.Items.Add(second);

            ISelectionItemProvider provider = null;
            TestAsync(
                acc,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(second) as ISelectionItemProvider,
                () => provider.AddToSelection(),
                () => Thread.Sleep(40),
                () => Assert.IsTrue(second.DisplayValue == 1.0, "Second item should have been selected!"));
        }

        /// <summary>
        /// Verify the RatingItemAutomationPeer selects an item when another
        /// was already selected.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingItemAutomationPeer selects an item when another was already selected.")]
        public virtual void RatingItemPeerSelectAlreadySelected()
        {
            Rating acc = new Rating();
            acc.ItemsSource = null;
            RatingItem first = new RatingItem();
            RatingItem second = new RatingItem();
            acc.Items.Add(first);
            acc.Items.Add(second);

            ISelectionItemProvider provider = null;
            TestAsync(
                acc,
                () => Assert.IsFalse(first.DisplayValue == 1.0, "First item should not be selected!"),
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(first) as ISelectionItemProvider,
                () => provider.Select(),
                () => Thread.Sleep(40),
                () => Assert.IsTrue(first.DisplayValue == 1.0, "First item should be selected!"),
                () => Assert.IsFalse(second.DisplayValue == 1.0, "Second item should not be selected!"));
        }

        /// <summary>
        /// Verify the RatingItemAutomationPeer selects this item on
        /// AddToSelection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingItemAutomationPeer selects this item on AddToSelection.")]
        public virtual void RatingItemPeerSelect()
        {
            Rating acc = new Rating();
            acc.ItemsSource = null;
            RatingItem first = new RatingItem();
            RatingItem second = new RatingItem();
            acc.Items.Add(first);
            acc.Items.Add(second);

            ISelectionItemProvider provider = null;
            TestAsync(
                acc,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(first) as ISelectionItemProvider,
                () => provider.Select(),
                () => Assert.IsTrue(first.DisplayValue == 1.0, "First item should have been selected!"));
        }

        /// <summary>
        /// Verify the RatingItemAutomationPeer unselects the item in
        /// RemoveFromSelection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the RatingItemAutomationPeer unselects the item in RemoveFromSelection.")]
        public virtual void RatingItemPeerRemoveSelectionNoRating()
        {
            Rating acc = new Rating();
            acc.ItemsSource = null;
            RatingItem first = new RatingItem();
            RatingItem second = new RatingItem();
            acc.Items.Add(first);
            acc.Items.Add(second);

            ISelectionItemProvider provider = null;
            TestAsync(
                acc,
                () => acc.Value = 1.0,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(first) as ISelectionItemProvider,
                () => provider.RemoveFromSelection(),
                () => Thread.Sleep(40),
                () => Assert.IsFalse(first.DisplayValue == 1.0, "Item should be not selected!"));
        }
        #endregion ISelectionItemProvider
    }
}
