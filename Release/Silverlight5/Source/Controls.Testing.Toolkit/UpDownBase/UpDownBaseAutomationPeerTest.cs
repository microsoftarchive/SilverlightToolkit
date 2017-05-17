// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Automation.Peers;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// UpDownBaseAutomationPeer unit tests.
    /// </summary>
    [TestClass]
    [Tag("UpDownBaseTest")]
    [Tag("Automation")]
    public class UpDownBaseAutomationPeerTest : TestBase
    {
        /// <summary>
        /// Create a new UpDownBaseAutomationPeer.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.UpDownBaseAutomationPeer`1<System.Double>", Justification = "Test to see there are no exceptions when creating a new peer.")]
        [TestMethod]
        [Description("Create a new UpDownBaseAutomationPeer.")]
        public virtual void CreateUpDownBasePeer()
        {
            NumericUpDown nud = new NumericUpDown();
            new UpDownBaseAutomationPeer<double>(nud);
        }

        /// <summary>
        /// Verify the UpDownAutomationPeer supports Value pattern.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the UpDownAutomationPeer supports Value pattern.")]
        public virtual void UpDownBasePeerOnlySupportsValuePattern()
        {
            NumericUpDown item = new NumericUpDown();
            UpDownBaseAutomationPeer<double> peer = null;
            TestAsync(
                item,
                () => peer = new UpDownBaseAutomationPeer<double>(item),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Dock), "UpDownBaseAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.ExpandCollapse), "UpDownBaseAutomationPeer should not support the ExpandCollapse pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Grid), "UpDownBaseAutomationPeer should not support the Grid pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.GridItem), "UpDownBaseAutomationPeer should not support the GridItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Invoke), "UpDownBaseAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.MultipleView), "UpDownBaseAutomationPeer should not support the MultipleView pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.RangeValue), "UpDownBaseAutomationPeer should not support the RangeValue pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Scroll), "UpDownBaseAutomationPeer should not support the Scroll pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.ScrollItem), "UpDownBaseAutomationPeer should not support the ScrollItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Selection), "UpDownBaseAutomationPeer should not support the Selection pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Table), "UpDownBaseAutomationPeer should not support the Table pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.TableItem), "UpDownBaseAutomationPeer should not support the TableItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Toggle), "UpDownBaseAutomationPeer should not support the Toggle pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Transform), "UpDownBaseAutomationPeer should not support the Transform pattern!"),
                () => Assert.IsNotNull(peer.GetPattern(PatternInterface.Value), "UpDownBaseAutomationPeer should support the Value pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Window), "UpDownBaseAutomationPeer should not support the Window pattern!"));
        }
    }
}
