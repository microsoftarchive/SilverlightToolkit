// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// DomainUpDownAutomationPeer Unit tests.
    /// </summary>
    [TestClass]
    [Tag("DomainUpDownTest")]
    [Tag("Automation")]
    public class DomainUpDownAutomationPeerTest : UpDownBaseAutomationPeerTest
    {
        /// <summary>
        /// Create a new DomainUpDownAutomationPeer.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.DomainUpDownAutomationPeer", Justification = "Test to see there are no exceptions when creating a new peer."), TestMethod]
        [Description("Create a new DomainUpDownAutomationPeer.")]
        public virtual void CreateDomainUpDownPeer()
        {
            DomainUpDown dud = new DomainUpDown();
            new DomainUpDownAutomationPeer(dud);
        }

        /// <summary>
        /// Verify that DomainUpDown creates a DomainUpDownAutomationPeer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that DomainUpDown creates a DomainUpDownAutomationPeer.")]
        public virtual void DomainUpDownCreatesCorrectAutomationPeer()
        {
            DomainUpDown dud = new DomainUpDown();
            DomainUpDownAutomationPeer peer = null;
            TestAsync(
                dud,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(dud) as DomainUpDownAutomationPeer,
                () => Assert.IsNotNull(peer, "DomainUpDown peer should not be null!"),
                () => Assert.AreEqual(dud, peer.Owner, "DomainUpDown should be owner of the peer!"));
        }

        /// <summary>
        /// Verify the DomainUpDownAutomationPeer supports Value pattern.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the DomainUpDownAutomationPeer supports Value pattern.")]
        public virtual void DomainUpDownPeerOnlySupportsValuePattern()
        {
            DomainUpDown item = new DomainUpDown();
            DomainUpDownAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as DomainUpDownAutomationPeer,
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Dock), "DomainUpDownAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.ExpandCollapse), "DomainUpDownAutomationPeer should not support the ExpandCollapse pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Grid), "DomainUpDownAutomationPeer should not support the Grid pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.GridItem), "DomainUpDownAutomationPeer should not support the GridItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Invoke), "DomainUpDownAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.MultipleView), "DomainUpDownAutomationPeer should not support the MultipleView pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.RangeValue), "DomainUpDownAutomationPeer should not support the RangeValue pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Scroll), "DomainUpDownAutomationPeer should not support the Scroll pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.ScrollItem), "DomainUpDownAutomationPeer should not support the ScrollItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Selection), "DomainUpDownAutomationPeer should not support the Selection pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Table), "DomainUpDownAutomationPeer should not support the Table pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.TableItem), "DomainUpDownAutomationPeer should not support the TableItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Toggle), "DomainUpDownAutomationPeer should not support the Toggle pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Transform), "DomainUpDownAutomationPeer should not support the Transform pattern!"),
                () => Assert.IsNotNull(peer.GetPattern(PatternInterface.Value), "DomainUpDownAutomationPeer should support the Value pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Window), "DomainUpDownAutomationPeer should not support the Window pattern!"));
        }

        #region IValueProvider
        /// <summary>
        /// Verify that DomainUpDownAutomationPeer implements the
        /// IValueProvider interface.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that DomainUpDownPeer implements the IValueProvider interface.")]
        public virtual void DomainUpDownPeerIsIValueProvider()
        {
            DomainUpDown dud = new DomainUpDown();
            IValueProvider provider = null;
            TestAsync(
                dud,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(dud) as IValueProvider,
                () => Assert.IsNotNull(provider, "DomainUpDownAutomationPeer should implement IValueProvider!"));
        }
        #endregion
    }
}
