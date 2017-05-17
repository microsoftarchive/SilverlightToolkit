// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// ExpanderAutomationPeer unit tests.
    /// </summary>
    [TestClass]
    [Tag("Expander")]
    [Tag("Automation")]
    public class ExpanderAutomationPeerTest : TestBase
    {
        /// <summary>
        /// Initializes a new instance of the ExpanderAutomationPeerTest
        /// class.
        /// </summary>
        public ExpanderAutomationPeerTest()
            : base()
        {
        }

        /// <summary>
        /// Create a new ExpanderAutomationPeer.
        /// </summary>
        [TestMethod]
        [Description("Create a new ExpanderAutomationPeer.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.ExpanderAutomationPeer", Justification = "Don't need to use it")]
        public virtual void CreateExpanderPeer()
        {
            Expander item = new Expander();
            new ExpanderAutomationPeer(item);
        }

        /// <summary>
        /// Create a new ExpanderAutomationPeer with a null Expander.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Create a new ExpanderAutomationPeer with a null Expander.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.ExpanderAutomationPeer", Justification = "Don't need to use it")]
        public virtual void CreateExpanderPeerWithNull()
        {
            new ExpanderAutomationPeer(null);
        }

        /// <summary>
        /// Verify that Expander creates a ExpanderAutomationPeer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that Expander creates a ExpanderAutomationPeer.")]
        public virtual void ExpanderCreatesAutomationPeer()
        {
            Expander item = new Expander();
            ExpanderAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ExpanderAutomationPeer,
                () => Assert.IsNotNull(peer, "Expander peer should not be null!"),
                () => Assert.AreEqual(item, peer.Owner, "Expander should be owner of the peer!"));
        }

        /// <summary>
        /// Verify the ExpanderAutomationPeer's type and class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the ExpanderAutomationPeer's type and class.")]
        public virtual void ExpanderAutomationPeerTypeAndClass()
        {
            Expander item = new Expander();
            ExpanderAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ExpanderAutomationPeer,
                () => Assert.AreEqual(AutomationControlType.Group, peer.GetAutomationControlType(), "Unexpected AutomationControlType!"),
                () => Assert.AreEqual("Expander", peer.GetClassName(), "Unexpected ClassType!"));
        }

        /// <summary>
        /// Verify that ExpanderAutomationPeer only supports scrolling and
        /// selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that ExpanderAutomationPeer only supports ExpandCollapse.")]
        public virtual void ExpanderPeerOnlySupportsExpandCollapse()
        {
            Expander item = new Expander();
            ExpanderAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ExpanderAutomationPeer,
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Dock), "ExpanderAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNotNull(peer.GetPattern(PatternInterface.ExpandCollapse), "ExpanderAutomationPeer should support the ExpandCollapse pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Grid), "ExpanderAutomationPeer should not support the Grid pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.GridItem), "ExpanderAutomationPeer should not support the GridItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Invoke), "ExpanderAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.MultipleView), "ExpanderAutomationPeer should not support the MultipleView pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.RangeValue), "ExpanderAutomationPeer should not support the RangeValue pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Scroll), "ExpanderAutomationPeer should not support the Scroll pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.ScrollItem), "ExpanderAutomationPeer should not support the ScrollItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Selection), "ExpanderAutomationPeer should not support the Selection pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Table), "ExpanderAutomationPeer should not support the Table pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.TableItem), "ExpanderAutomationPeer should not support the TableItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Toggle), "ExpanderAutomationPeer should not support the Toggle pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Transform), "ExpanderAutomationPeer should not support the Transform pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Value), "ExpanderAutomationPeer should not support the Value pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Window), "ExpanderAutomationPeer should not support the Window pattern!"));
        }

        #region IExpandCollapseProvider
        /// <summary>
        /// Verify that ExpanderAutomationPeer implements the
        /// IExpandCollapseProvider interface.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that ExpanderAutomationPeer implements the IExpandCollapseProvider interface.")]
        public virtual void ExpanderPeerIsIExpandCollapseProvider()
        {
            Expander item = new Expander();
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as IExpandCollapseProvider,
                () => Assert.IsNotNull(provider, "ExpanderAutomationPeer should implement IExpandCollapseProvider!"));
        }

        /// <summary>
        /// Verify that ExpanderAutomationPeer supports the ExpandCollapse
        /// pattern.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that ExpanderAutomationPeer supports the ExpandCollapse pattern.")]
        public virtual void ExpanderPeerSupportsExpandCollapse()
        {
            Expander item = new Expander();
            ExpanderAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ExpanderAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => Assert.IsNotNull(provider, "IExpandCollapseProvider peer should not be null!"));
        }

        /// <summary>
        /// Verify that ExpanderAutomationPeer has the right
        /// ExpandCollapseState for an item that is collapsed.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that ExpanderAutomationPeer has the right ExpandCollapseState for an item that is collapsed.")]
        public virtual void ExpanderPeerExpandStateCollapsed()
        {
            Expander item = new Expander();
            ExpanderAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ExpanderAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => Assert.AreEqual(ExpandCollapseState.Collapsed, provider.ExpandCollapseState, "Unexpected ExpandCollapseState!"));
        }

        /// <summary>
        /// Verify that ExpanderAutomationPeer has the right
        /// ExpandCollapseState for an item that is expanded.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that ExpanderAutomationPeer has the right ExpandCollapseState for an item that is expanded.")]
        public virtual void ExpanderPeerExpandStateExpanded()
        {
            Expander item = new Expander { IsExpanded = true };
            ExpanderAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ExpanderAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => Assert.AreEqual(ExpandCollapseState.Expanded, provider.ExpandCollapseState, "Unexpected ExpandCollapseState!"));
        }

        /// <summary>
        /// Verify that ExpanderAutomationPeer throws an exception when
        /// expanding a disabled item.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementNotEnabledException))]
        [Asynchronous]
        [Description("Verify that ExpanderAutomationPeer throws an exception when expanding a disabled item.")]
        public virtual void ExpanderPeerExpandDisabled()
        {
            Expander item = new Expander { IsEnabled = false };
            ExpanderAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ExpanderAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Expand());
        }

        /// <summary>
        /// Verify that ExpanderAutomationPeer expands an item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that ExpanderAutomationPeer expands an item.")]
        public virtual void ExpanderPeerExpands()
        {
            Expander item = new Expander();
            ExpanderAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ExpanderAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Expand(),
                () => Assert.IsTrue(item.IsExpanded, "Item should be expanded!"));
        }

        /// <summary>
        /// Verify that ExpanderAutomationPeer expands an expanded item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that ExpanderAutomationPeer expands an expanded item.")]
        public virtual void ExpanderPeerExpandsExpanded()
        {
            Expander item = new Expander { IsExpanded = true };
            ExpanderAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ExpanderAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Expand(),
                () => Assert.IsTrue(item.IsExpanded, "Item should be expanded!"));
        }

        /// <summary>
        /// Verify that ExpanderAutomationPeer throws an exception when
        /// collapsing a disabled item.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementNotEnabledException))]
        [Asynchronous]
        [Description("Verify that ExpanderAutomationPeer throws an exception when collapsing a disabled item.")]
        public virtual void ExpanderPeerCollapseDisabled()
        {
            Expander item = new Expander { IsEnabled = false };
            ExpanderAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ExpanderAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Collapse());
        }

        /// <summary>
        /// Verify that ExpanderAutomationPeer collapses an item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that ExpanderAutomationPeer collapses an item.")]
        public virtual void ExpanderPeerCollapse()
        {
            Expander item = new Expander { IsExpanded = true };
            ExpanderAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ExpanderAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Collapse(),
                () => Assert.IsFalse(item.IsExpanded, "Item should be collapsed!"));
        }

        /// <summary>
        /// Verify that ExpanderAutomationPeer collapses an collapsed item.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that ExpanderAutomationPeer collapses an collapsed item.")]
        public virtual void ExpanderPeerCollapseCollapsed()
        {
            Expander item = new Expander();
            ExpanderAutomationPeer peer = null;
            IExpandCollapseProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as ExpanderAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider,
                () => provider.Collapse(),
                () => Assert.IsFalse(item.IsExpanded, "Item should be collapsed!"));
        }
        #endregion IExpandCollapseProvider
    }
}