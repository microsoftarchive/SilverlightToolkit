// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls.Primitives;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// A testable derivation of the SelectorSelectionAdapter.
    /// </summary>
    public class OverriddenSelectionAdapter : SelectorSelectionAdapter
    {
        /// <summary>
        /// Gets or sets the most recent instance of the 
        /// OverriddenSelectionAdapter.
        /// </summary>
        public static OverriddenSelectionAdapter Current { get; set; }

        /// <summary>
        /// Initializes a new instance of the OverriddenSelectionAdapter class.
        /// </summary>
        /// <param name="selector">The selector instance to wrap.</param>
        public OverriddenSelectionAdapter(Selector selector)
            : base(selector)
        {
            Current = this;
        }

        /// <summary>
        /// Select the first item in the selector.
        /// </summary>
        public void SelectFirst()
        {
            SelectorControl.SelectedIndex = 0;
        }

        /// <summary>
        /// Select the next item. Equivalent to pressing down typically.
        /// </summary>
        public void SelectNext()
        {
            SelectedIndexIncrement();
        }

        /// <summary>
        /// Select the previous item. Equivalent to pressing up typically.
        /// </summary>
        public void SelectPrevious()
        {
            SelectedIndexDecrement();
        }

        /// <summary>
        /// Performs the selection adapter Commit action.
        /// </summary>
        public void TestCommit()
        {
            OnCommit();
        }

        /// <summary>
        /// Performs the selection adapter Cancel action.
        /// </summary>
        public void TestCancel()
        {
            OnCancel();
        }
    }
}