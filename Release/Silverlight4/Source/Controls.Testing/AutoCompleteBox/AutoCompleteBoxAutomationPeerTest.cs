// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation.Peers;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// AutoCompleteBoxAutomationPeer unit tests.
    /// </summary>
    [TestClass]
    [Tag("AutoCompleteBoxTest")]
    [Tag("Automation")]
    public class AutoCompleteBoxAutomationPeerTest : TestBase
    {
        /// <summary>
        /// Initializes a new instance of the AutoCompleteBoxAutomationPeerTest
        /// class.
        /// </summary>
        public AutoCompleteBoxAutomationPeerTest()
        {
        }

        /// <summary>
        /// Create a new AutoCompleteBoxAutomationPeer.
        /// </summary>
        [TestMethod]
        [Description("Create a new AutoCompleteBoxAutomationPeer.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.AutoCompleteBoxAutomationPeer", Justification = "Not needed.")]
        public virtual void CreateAutoCompleteBoxPeer()
        {
            AutoCompleteBox acb = new AutoCompleteBox();
            new AutoCompleteBoxAutomationPeer(acb);
        }

        /// <summary>
        /// Create a new AutoCompleteBoxAutomationPeer with a null
        /// AutoCompleteBox.
        /// </summary>
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Create a new AutoCompleteBoxAutomationPeer with a null AutoCompleteBox.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.AutoCompleteBoxAutomationPeer", Justification = "Not needed.")]
        [TestMethod]
        public virtual void CreateAutoCompleteBoxPeerWithNull()
        {
            new AutoCompleteBoxAutomationPeer(null);
        }

        /// <summary>
        /// Verify that AutoCompleteBox creates a AutoCompleteBoxAutomationPeer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that control creates an AutoCompleteBoxAutomationPeer.")]
        public virtual void ControlCreatesPeer()
        {
            AutoCompleteBox acb = new AutoCompleteBox();
            AutoCompleteBoxAutomationPeer peer = null;
            TestAsync(
                acb,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(acb) as AutoCompleteBoxAutomationPeer,
                () => Assert.IsNotNull(peer, "AutoCompleteBox peer should not be null!"),
                () => Assert.AreEqual(acb, peer.Owner, "AutoCompleteBox should be owner of the peer!"));
        }

        /// <summary>
        /// Verify the AutoCompleteBoxAutomationPeer's type and class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the AutoCompleteBoxAutomationPeer's type and class.")]
        public virtual void AutoCompleteBoxAutomationPeerTypeAndClass()
        {
            AutoCompleteBox acb = new AutoCompleteBox();
            AutoCompleteBoxAutomationPeer peer = null;
            TestAsync(
                acb,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(acb) as AutoCompleteBoxAutomationPeer,
                () => Assert.AreEqual(AutomationControlType.ComboBox, peer.GetAutomationControlType(), "Unexpected AutomationControlType!"),
                () => Assert.AreEqual("AutoCompleteBox", peer.GetClassName(), "Unexpected ClassType!"));
        }

        // Look into adopting TreeView automation peer tests over time:
        // CONSIDER: TreeViewPeerGetNoItems
        // CONSIDER: TreeViewPeerGetWithItems
        // CONSIDER: TreeViewPeerOnlySupportsScrollingAndSelection
        // CONSIDER: TreeViewPeerSupportsScrolling
        // CONSIDER: TreeViewPeerSupportsScrollingNoHost
        // CONSIDER: The I(n)Provider implementation validations
    }
}