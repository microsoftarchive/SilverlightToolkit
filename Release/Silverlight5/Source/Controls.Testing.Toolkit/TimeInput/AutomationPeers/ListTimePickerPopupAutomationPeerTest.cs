// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Tests for ListTimePickerPopupAutomationPeer.
    /// </summary>
    [TestClass]
    [Tag("TimeInput")]
    [Tag("ListTimePickerPopupTest")]
    [Tag("Automation")]
    public class ListTimePickerPopupAutomationPeerTest : TimePickerPopupAutomationPeerTest
    {
        /// <summary>
        /// Gets the TimePickerPopup instance that we might test.
        /// </summary>
        /// <returns>A correctly typed TimePickerPopup.</returns>
        protected override TimePickerPopup TimePickerPopupInstance
        {
            get
            {
                return new ListTimePickerPopup();
            }
        }

        /// <summary>
        /// Gets an automation.
        /// </summary>
        /// <param name="picker">The picker.</param>
        /// <returns>A ListTimePickerAutomationPeer for the picker.</returns>
        protected override TimePickerPopupAutomationPeer GetTimePickerPopupAutomationPeer(TimePickerPopup picker)
        {
            return new ListTimePickerPopupAutomationPeer((ListTimePickerPopup) picker);
        }

        /// <summary>
        /// Gets the expected patterns.
        /// </summary>
        /// <returns>Patterns Value and Selection.</returns>
        protected override IList<PatternInterface> ExpectedPatterns
        {
            get
            {
                return new List<PatternInterface>
                           {
                                   PatternInterface.Value,
                                   PatternInterface.Selection
                           };
            }
        }

        #region Selection provider
        /// <summary>
        /// Tests that provider does not allow selecting multiple items.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that provider does not allow selecting multiple items.")]
        public virtual void ShouldNotAllowSelectingMultipleItems()
        {
            ListTimePickerPopup ltp = new ListTimePickerPopup();
            ISelectionProvider provider = null;

            TestAsync(
                ltp,
                () => provider = (ISelectionProvider)FrameworkElementAutomationPeer.CreatePeerForElement(ltp),
                () => Assert.IsFalse(provider.CanSelectMultiple));
        }

        /// <summary>
        /// Tests that provider gets correct selection.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that provider gets correct selection.")]
        public virtual void ShouldFillSelectionWithAPeer()
        {
            ListTimePickerPopup ltp = new ListTimePickerPopup();
            ISelectionProvider provider = null;
            ItemSelectionHelper<KeyValuePair<string, DateTime?>> helper = null;
            TestAsync(
                ltp,
                () => provider = (ISelectionProvider)FrameworkElementAutomationPeer.CreatePeerForElement(ltp),
                () => helper = ltp.TimeItemsSelection,
                () => Assert.IsTrue(provider.GetSelection().Length == 0),
                () => helper.SelectedItem = helper.Items[3],
                () => Assert.IsNotNull(provider.GetSelection(), "There have been intermittent Automation problems where the ListBoxAutomationPeer reports null children."),
                () => Assert.IsTrue(provider.GetSelection().Length == 1));
        }

        /// <summary>
        /// Tests that selection is not required.
        /// </summary>
        [TestMethod]
        [Asynchronous]
        [Description("Tests that selection is not required.")]
        public virtual void ShouldNotRequireSelection()
        {
            ListTimePickerPopup ltp = new ListTimePickerPopup();
            ISelectionProvider provider = null;

            TestAsync(
                ltp,
                () => provider = (ISelectionProvider)FrameworkElementAutomationPeer.CreatePeerForElement(ltp),
                () => Assert.IsFalse(provider.CanSelectMultiple));
        }
        #endregion
    }
}
