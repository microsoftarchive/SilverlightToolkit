// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Tests for TimeUpDownAutomationPeer types.
    /// </summary>
    [TestClass]
    [Tag("TimeInput")]
    [Tag("TimeUpDownTest")]
    [Tag("Automation")]
    public class TimeUpDownAutomationPeerTest : UpDownBaseAutomationPeerTest
    {
        /// <summary>
        /// Create a new TimeUpDownAutomationPeer.
        /// </summary>
        [Description("Create a new TimeUpDownAutomationPeer.")]
        [TestMethod]
        public virtual void CreateTimeUpDownPeer()
        {
            TimeUpDown tud = new TimeUpDown();
            TimeUpDownAutomationPeer peer = new TimeUpDownAutomationPeer(tud);
            Assert.IsNotNull(peer);
        }

        /// <summary>
        /// Verify that TimeUpDown creates a TimeUpDownAutomationPeer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TimeUpDown creates a TimeUpDownAutomationPeer.")]
        public virtual void TimeUpDownCreatesCorrectAutomationPeer()
        {
            TimeUpDown tud = new TimeUpDown();
            TimeUpDownAutomationPeer peer = null;
            TestAsync(
                tud,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(tud) as TimeUpDownAutomationPeer,
                () => Assert.IsNotNull(peer, "TimeUpDown peer should not be null!"),
                () => Assert.AreEqual(tud, peer.Owner, "TimeUpDown should be owner of the peer!"));
        }

        /// <summary>
        /// Verify the TimeUpDownAutomationPeer supports Value pattern.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the TimeUpDownAutomationPeer supports Value pattern.")]
        public virtual void TimeUpDownPeerOnlySupportsValuePattern()
        {
            TimeUpDown item = new TimeUpDown();
            TimeUpDownAutomationPeer peer = null;
            TestAsync(
                item,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(item) as TimeUpDownAutomationPeer,
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Dock), "TimeUpDownAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.ExpandCollapse), "TimeUpDownAutomationPeer should not support the ExpandCollapse pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Grid), "TimeUpDownAutomationPeer should not support the Grid pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.GridItem), "TimeUpDownAutomationPeer should not support the GridItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Invoke), "TimeUpDownAutomationPeer should not support the Dock pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.MultipleView), "TimeUpDownAutomationPeer should not support the MultipleView pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.RangeValue), "TimeUpDownAutomationPeer should not support the RangeValue pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Scroll), "TimeUpDownAutomationPeer should not support the Scroll pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.ScrollItem), "TimeUpDownAutomationPeer should not support the ScrollItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Selection), "TimeUpDownAutomationPeer should not support the Selection pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Table), "TimeUpDownAutomationPeer should not support the Table pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.TableItem), "TimeUpDownAutomationPeer should not support the TableItem pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Toggle), "TimeUpDownAutomationPeer should not support the Toggle pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Transform), "TimeUpDownAutomationPeer should not support the Transform pattern!"),
                () => Assert.IsNotNull(peer.GetPattern(PatternInterface.Value), "TimeUpDownAutomationPeer should support the Value pattern!"),
                () => Assert.IsNull(peer.GetPattern(PatternInterface.Window), "TimeUpDownAutomationPeer should not support the Window pattern!"));
        }

        #region IValueProvider
        /// <summary>
        /// Verify that TimeUpDownAutomationPeer implements the
        /// IValueProvider interface.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TimeUpDownPeer implements the IValueProvider interface.")]
        public virtual void TimeUpDownPeerIsIValueProvider()
        {
            TimeUpDown tud = new TimeUpDown();
            IValueProvider provider = null;
            TestAsync(
                tud,
                () => provider = FrameworkElementAutomationPeer.CreatePeerForElement(tud) as IValueProvider,
                () => Assert.IsNotNull(provider, "TimeUpDownAutomationPeer should implement IValueProvider!"));
        }

        /// <summary>
        /// Tests that a value can be set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that a value can be set.")]
        public virtual void ShouldBeAbleToSetValueThroughAutomationPeer()
        {
            TimeUpDown item = new TimeUpDown();
            item.Culture = new CultureInfo("nl-NL");
            item.Format = new CustomTimeFormat("HH:mm:ss");
            TimeUpDownAutomationPeer peer = null;
            IValueProvider provider = null;

            TestAsync(
                    item,
                    () => peer = (TimeUpDownAutomationPeer)FrameworkElementAutomationPeer.CreatePeerForElement(item),
                    () => provider = (IValueProvider)peer.GetPattern(PatternInterface.Value),
                    () => provider.SetValue("03:45:12"),
                    () => Assert.AreEqual(item.Value.Value.TimeOfDay, new DateTime(1900, 1, 1, 3, 45, 12).TimeOfDay));
        }

        /// <summary>
        /// Tests that a value can be read.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that a value can be read.")]
        public virtual void ShouldBeAbleToGetValueThroughAutomationPeer()
        {
            TimeUpDown item = new TimeUpDown();
            item.Culture = new CultureInfo("nl-NL");
            item.Format = new CustomTimeFormat("HH:mm:ss");
            item.Value = new DateTime(1900, 1, 1, 3, 45, 12);
            TimeUpDownAutomationPeer peer = null;
            IValueProvider provider = null;

            TestAsync(
                    item,
                    () => peer = (TimeUpDownAutomationPeer)FrameworkElementAutomationPeer.CreatePeerForElement(item),
                    () => provider = (IValueProvider)peer.GetPattern(PatternInterface.Value),
                    () => Assert.AreEqual(provider.Value, "03:45:12"));
        }

        /// <summary>
        /// Tests that readonly is correctly set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that readonly is correctly set.")]
        public virtual void ShouldCorrelateReadOnlyToIsEnabledInAutomationPeer()
        {
            TimeUpDown item = new TimeUpDown();
            item.Value = new DateTime(1900, 1, 1, 3, 45, 12);
            TimeUpDownAutomationPeer peer = null;
            IValueProvider provider = null;

            TestAsync(
                    item,
                    () => peer = (TimeUpDownAutomationPeer)FrameworkElementAutomationPeer.CreatePeerForElement(item),
                    () => provider = (IValueProvider)peer.GetPattern(PatternInterface.Value),
                    () => Assert.IsFalse(provider.IsReadOnly),
                    () => item.IsEnabled = false,
                    () => Assert.IsTrue(provider.IsReadOnly));
        }
        #endregion
    }
}
