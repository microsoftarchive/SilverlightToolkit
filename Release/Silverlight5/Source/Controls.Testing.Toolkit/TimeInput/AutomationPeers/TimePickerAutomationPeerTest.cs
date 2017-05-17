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
    /// Tests for TimePickerAutomationPeer types.
    /// </summary>
    [TestClass]
    [Tag("TimeInput")]
    [Tag("TimePickerTest")]
    [Tag("Automation")]
    public class TimePickerAutomationPeerTest : PickerAutomationPeerTest
    {
        /// <summary>
        /// Gets the picker instance.
        /// </summary>
        /// <returns>A TimePicker.</returns>
        protected override Picker PickerInstance
        {
            get
            {
                return new TimePicker();
            }
        }

        /// <summary>
        /// Gets the picker automation peer.
        /// </summary>
        /// <param name="picker">The picker.</param>
        /// <returns>A TimePickerAutomationPeer for this Picker.</returns>
        protected override PickerAutomationPeer CreatePickerAutomationPeer(Picker picker)
        {
            return new TimePickerAutomationPeer((TimePicker)picker);
        }

        /// <summary>
        /// Gets the expected patterns.
        /// </summary>
        /// <returns>
        /// An IList of patterns that this Picker is expected to
        /// implement: Value and ExpandCollapse.
        /// </returns>
        protected override IList<PatternInterface> ExpectedPatterns
        {
            get
            {
                return new List<PatternInterface>()
                           {
                                   PatternInterface.Value,
                                   PatternInterface.ExpandCollapse
                           };
            }
        }

        /// <summary>
        /// Tests that a value can be set.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that a value can be set.")]
        public virtual void ShouldBeAbleToSetValueThroughAutomationPeer()
        {
            TimePicker item = new TimePicker();
            item.Culture = new CultureInfo("nl-NL");
            item.Format = new CustomTimeFormat("HH:mm:ss");
            TimePickerAutomationPeer peer = null;
            IValueProvider provider = null;

            TestAsync(
                    item,
                    () => peer = (TimePickerAutomationPeer) FrameworkElementAutomationPeer.CreatePeerForElement(item),
                    () => provider = (IValueProvider) peer.GetPattern(PatternInterface.Value),
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
            TimePicker item = new TimePicker();
            item.Culture = new CultureInfo("nl-NL");
            item.Format = new CustomTimeFormat("HH:mm:ss");
            item.Value = new DateTime(1900, 1, 1, 3, 45, 12);
            TimePickerAutomationPeer peer = null;
            IValueProvider provider = null;

            TestAsync(
                    item,
                    () => peer = (TimePickerAutomationPeer)FrameworkElementAutomationPeer.CreatePeerForElement(item),
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
            TimePicker item = new TimePicker();
            item.Value = new DateTime(1900, 1, 1, 3, 45, 12);
            TimePickerAutomationPeer peer = null;
            IValueProvider provider = null;

            TestAsync(
                    item,
                    () => peer = (TimePickerAutomationPeer)FrameworkElementAutomationPeer.CreatePeerForElement(item),
                    () => provider = (IValueProvider)peer.GetPattern(PatternInterface.Value),
                    () => Assert.IsFalse(provider.IsReadOnly),
                    () => item.IsEnabled = false,
                    () => Assert.IsTrue(provider.IsReadOnly));
        }
    }
}
