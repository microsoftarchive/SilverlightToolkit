// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// NumericUpDownAutomationPeer unit tests.
    /// </summary>
    [TestClass]
    [Tag("NumericUpDown")]
    [Tag("Automation")]
    public class NumericUpDownAutomationPeerTest : UpDownBaseAutomationPeerTest
    {
        /// <summary>
        /// Initializes a new instance of the NumericUpDownAutomationPeerTest
        /// class.
        /// </summary>
        public NumericUpDownAutomationPeerTest()
            : base()
        {
        }

        /// <summary>
        /// Create a new NumericUpDownAutomationPeer.
        /// </summary>
        [TestMethod]
        [Description("Create a new NumericUpDownAutomationPeer.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.NumericUpDownAutomationPeer", Justification = "Don't need to use it")]
        public virtual void CreateNumericUpDownPeer()
        {
            NumericUpDown item = new NumericUpDown();
            new NumericUpDownAutomationPeer(item);
        }

        /// <summary>
        /// Create a new NumericUpDownAutomationPeer with a null NumericUpDown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Create a new NumericUpDownAutomationPeer with a null NumericUpDown.")]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Windows.Automation.Peers.NumericUpDownAutomationPeer", Justification = "Don't need to use it")]
        public virtual void CreateNumericUpDownPeerWithNull()
        {
            new NumericUpDownAutomationPeer(null);
        }

        /// <summary>
        /// Verify that NumericUpDown creates a NumericUpDownAutomationPeer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that NumericUpDown creates a NumericUpDownAutomationPeer.")]
        public virtual void NumericUpDownCreatesAutomationPeer()
        {
            NumericUpDown item = new NumericUpDown();
            NumericUpDownAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as NumericUpDownAutomationPeer,
                () => Assert.IsNotNull(peer, "NumericUpDown peer should not be null!"),
                () => Assert.AreEqual(item, peer.Owner, "NumericUpDown should be owner of the peer!"));
        }

        /// <summary>
        /// Verify the NumericUpDownAutomationPeer's type and class.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the NumericUpDownAutomationPeer's type and class.")]
        public virtual void NumericUpDownAutomationPeerTypeAndClass()
        {
            NumericUpDown item = new NumericUpDown();
            NumericUpDownAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as NumericUpDownAutomationPeer,
                () => Assert.AreEqual(AutomationControlType.Spinner, peer.GetAutomationControlType(), "Unexpected AutomationControlType!"),
                () => Assert.AreEqual("NumericUpDown", peer.GetClassName(), "Unexpected ClassType!"));
        }

        /// <summary>
        /// Verify that NumericUpDownAutomationPeer only supports scrolling and
        /// selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that NumericUpDownAutomationPeer only supports RangeValue.")]
        public virtual void NumericUpDownPeerSupportsRangeValueAndValue()
        {
            NumericUpDown item = new NumericUpDown();
            NumericUpDownAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as NumericUpDownAutomationPeer,
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Dock), "NumericUpDownAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.ExpandCollapse), "NumericUpDownAutomationPeer should not support the ExpandCollapse pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Grid), "NumericUpDownAutomationPeer should not support the Grid pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.GridItem), "NumericUpDownAutomationPeer should not support the GridItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Invoke), "NumericUpDownAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.MultipleView), "NumericUpDownAutomationPeer should not support the MultipleView pattern!"),
                () => Assert.IsNotNull(peer.GetPattern(PatternInterface.RangeValue), "NumericUpDownAutomationPeer should support the RangeValue pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Scroll), "NumericUpDownAutomationPeer should not support the Scroll pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.ScrollItem), "NumericUpDownAutomationPeer should not support the ScrollItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Selection), "NumericUpDownAutomationPeer should not support the Selection pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Table), "NumericUpDownAutomationPeer should not support the Table pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.TableItem), "NumericUpDownAutomationPeer should not support the TableItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Toggle), "NumericUpDownAutomationPeer should not support the Toggle pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Transform), "NumericUpDownAutomationPeer should not support the Transform pattern!"),
                () => Assert.IsNotNull(peer.GetPattern(PatternInterface.Value), "NumericUpDownAutomationPeer should support the Value pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Window), "NumericUpDownAutomationPeer should not support the Window pattern!"));
        }

        #region IRangeValueProvider
        /// <summary>
        /// Verify that NumericUpDownAutomationPeer implements the
        /// IRangeValueProvider interface.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that NumericUpDownAutomationPeer implements the IRangeValueProvider interface.")]
        public virtual void NumericUpDownPeerIsIRangeValueProvider()
        {
            NumericUpDown item = new NumericUpDown();
            IRangeValueProvider provider = null;
            TestAsync(
                item,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(item) as IRangeValueProvider,
                () => Assert.IsNotNull(provider, "NumericUpDownAutomationPeer should implement IRangeValueProvider!"));
        }

        /// <summary>
        /// Verify that NumericUpDownAutomationPeer supports the RangeValue
        /// pattern.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that NumericUpDownAutomationPeer supports the RangeValue pattern.")]
        public virtual void NumericUpDownPeerSupportsRangeValue()
        {
            NumericUpDown item = new NumericUpDown();
            NumericUpDownAutomationPeer peer = null;
            IRangeValueProvider provider = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as NumericUpDownAutomationPeer,
                () => provider = peer.GetPattern(PatternInterface.RangeValue) as IRangeValueProvider,
                () => Assert.IsNotNull(provider, "IRangeValueProvider peer should not be null!"));
        }
        #endregion IRangeValueProvider
    }
}