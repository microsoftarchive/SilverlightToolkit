// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Automation.Peers;
using System.Collections.Generic;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Tests for RangeTimePickerPopup types.
    /// </summary>
    [TestClass]
    [Tag("TimeInput")]
    [Tag("RangeTimePickerPopupTest")]
    [Tag("Automation")]
    public class RangeTimePickerPopupAutomationPeerTest : TimePickerPopupAutomationPeerTest
    {
        /// <summary>
        /// Gets the TimePickerPopup instance that we might test.
        /// </summary>
        /// <returns>A correctly typed TimePickerPopup.</returns>
        protected override TimePickerPopup TimePickerPopupInstance
        {
            get
            {
                return new RangeTimePickerPopup();
            }
        }

        /// <summary>
        /// Gets an automation.
        /// </summary>
        /// <param name="picker">The picker.</param>
        /// <returns>A RangeTimePickerPopupAutomationPeer for this picker.</returns>
        protected override TimePickerPopupAutomationPeer GetTimePickerPopupAutomationPeer(TimePickerPopup picker)
        {
            return new RangeTimePickerPopupAutomationPeer((RangeTimePickerPopup)picker);
        }

        /// <summary>
        /// Gets the expected patterns.
        /// </summary>
        /// <returns>The Value pattern.</returns>
        protected override IList<PatternInterface> ExpectedPatterns
        {
            get
            {
                return new List<PatternInterface>()
                           {
                                   PatternInterface.Value
                           };
            }
        } 
    }
}
