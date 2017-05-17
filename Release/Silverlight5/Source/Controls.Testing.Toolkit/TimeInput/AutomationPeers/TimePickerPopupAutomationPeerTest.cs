// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Collections.Generic;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Test base for TimePickerPopupAutomationPeer types.
    /// </summary>
    public abstract class TimePickerPopupAutomationPeerTest : TestBase
    {
        /// <summary>
        /// Gets the TimePickerPopup instance that we might test.
        /// </summary>
        /// <returns>A correctly typed TimePickerPopup.</returns>
        protected abstract TimePickerPopup TimePickerPopupInstance { get; }

        /// <summary>
        /// Gets an automation peer for this picker.
        /// </summary>
        /// <param name="picker">The picker.</param>
        /// <returns>An AutomationPeer for the picker.</returns>
        protected abstract TimePickerPopupAutomationPeer GetTimePickerPopupAutomationPeer(TimePickerPopup picker);

        /// <summary>
        /// Gets the expected automation patterns.
        /// </summary>
        /// <returns>The patterns that will be tested.</returns>
        protected abstract IList<PatternInterface> ExpectedPatterns { get; }

        /// <summary>
        /// Create a new TimePickerPopupAutomationPeer.
        /// </summary>
        [Description("Create a new TimePickerPopupAutomationPeer.")]
        [TestMethod]
        public virtual void CreateTimePickerPopupPeer()
        {
            TimePickerPopup tpp = TimePickerPopupInstance;
            TimePickerPopupAutomationPeer peer = GetTimePickerPopupAutomationPeer(tpp);
            Assert.IsNotNull(peer);
        }

        /// <summary>
        /// Verify that TimePickerPopup creates a TimePickerPopupAutomationPeer.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify that TimePickerPopup creates a TimePickerPopupAutomationPeer.")]
        public virtual void TimePickerPopupCreatesCorrectAutomationPeer()
        {
            TimePickerPopup tpp = TimePickerPopupInstance;
            TimePickerPopupAutomationPeer peer = null;
            TestAsync(
                tpp,
                () => peer = FrameworkElementAutomationPeer.CreatePeerForElement(tpp) as TimePickerPopupAutomationPeer,
                () => Assert.IsNotNull(peer, "TimePickerPopup peer should not be null!"),
                () => Assert.AreEqual(tpp, peer.Owner, "TimePickerPopup should be owner of the peer!"));
        }

        /// <summary>
        /// Verify the correct patterns are supported.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Verify the PickerAutomationPeer supports expected patterns.")]
        public virtual void PickerPeerOnlySupportsCorrectPatterns()
        {
            TimePickerPopup instance = TimePickerPopupInstance;
            TimePickerPopupAutomationPeer peer = null;

            TestAsync(
                    instance,
                    () => peer = (TimePickerPopupAutomationPeer) FrameworkElementAutomationPeer.CreatePeerForElement(instance),
                    () =>
                        {
                            int index = 0;
                            while (Enum.IsDefined(typeof(PatternInterface), index))
                            {
                                PatternInterface currentInterface = (PatternInterface) Enum.ToObject(typeof(PatternInterface), index);
                                object implementation = peer.GetPattern(currentInterface);
                                if (ExpectedPatterns.Contains(currentInterface))
                                {
                                    Assert.IsNotNull(implementation);
                                }
                                else
                                {
                                    Assert.IsNull(implementation);
                                }
                                index++;
                            }
                        });
        }

        /// <summary>
        /// Tests that a value can be set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that a value can be set.")]
        public virtual void ShouldBeAbleToSetValueThroughAutomationPeer()
        {
            TimePickerPopup item = TimePickerPopupInstance;
            item.Culture = new CultureInfo("nl-NL");
            item.Format = new CustomTimeFormat("HH:mm:ss");
            TimePickerPopupAutomationPeer peer = null;
            IValueProvider provider = null;

            TestAsync(
                    item,
                    () => peer = (TimePickerPopupAutomationPeer)FrameworkElementAutomationPeer.CreatePeerForElement(item),
                    () => provider = (IValueProvider)peer.GetPattern(PatternInterface.Value),
                    () => provider.SetValue("03:30:00"), // take care to set a time that is not snapped
                    () => Assert.AreEqual(item.Value.Value.TimeOfDay, new DateTime(1900, 1, 1, 3, 30, 00).TimeOfDay));
        }

        /// <summary>
        /// Tests that a value can be read.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that a value can be read.")]
        public virtual void ShouldBeAbleToGetValueThroughAutomationPeer()
        {
            TimePickerPopup item = TimePickerPopupInstance;
            item.Culture = new CultureInfo("nl-NL");
            item.Format = new CustomTimeFormat("HH:mm:ss");
            item.Value = new DateTime(1900, 1, 1, 3, 45, 12);
            TimePickerPopupAutomationPeer peer = null;
            IValueProvider provider = null;

            TestAsync(
                    item,
                    () => peer = (TimePickerPopupAutomationPeer)FrameworkElementAutomationPeer.CreatePeerForElement(item),
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
            TimePickerPopup item = TimePickerPopupInstance;
            item.Value = new DateTime(1900, 1, 1, 3, 45, 12);
            TimePickerPopupAutomationPeer peer = null;
            IValueProvider provider = null;

            TestAsync(
                    item,
                    () => peer = (TimePickerPopupAutomationPeer)FrameworkElementAutomationPeer.CreatePeerForElement(item),
                    () => provider = (IValueProvider)peer.GetPattern(PatternInterface.Value),
                    () => Assert.IsFalse(provider.IsReadOnly),
                    () => item.IsEnabled = false,
                    () => Assert.IsTrue(provider.IsReadOnly));
        }
    }
}
