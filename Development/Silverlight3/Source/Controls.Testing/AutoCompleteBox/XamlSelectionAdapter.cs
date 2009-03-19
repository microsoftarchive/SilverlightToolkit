// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Windows.Automation.Peers;
using System.Windows.Input;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// A testable SelectionAdapter that derives from ListBox. This is NOT a 
    /// complete implementation of a SelectionAdapter, but instead if used for 
    /// testing a specific scenario currently.
    /// </summary>
    public class XamlSelectionAdapter : ListBox, ISelectionAdapter
    {
        /// <summary>
        /// Gets or sets the most recent instance of the 
        /// XamlSelectionAdapter.
        /// </summary>
        public static XamlSelectionAdapter Current { get; set; }

        /// <summary>
        /// Initializes a new instance of the XamlSelectionAdapter class.
        /// </summary>
        public XamlSelectionAdapter()
        {
            Current = this;
        }

        #region Adapter contract

        /// <summary>
        /// Occurs when the SelectedItem property value changes.
        /// </summary>
        public new event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Calls the SelectionChanged handler.
        /// </summary>
        public void DoSelect()
        {
            SelectionChangedEventHandler handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        /// <summary>
        /// Calls the Commit handler.
        /// </summary>
        public void DoCommit()
        {
            RoutedEventHandler handler = Commit;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        /// <summary>
        /// Calls the Cancel handler.
        /// </summary>
        public void DoCancel()
        {
            RoutedEventHandler handler = Cancel;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        /// <summary>
        /// Occurs when a selection has occurred and has been committed.
        /// </summary>
        public event RoutedEventHandler Commit;

        /// <summary>
        /// Occurs when a selection has occurred and has been canceled.
        /// </summary>
        public event RoutedEventHandler Cancel;

        /// <summary>
        /// Gets or sets the selected item through the adapter.
        /// </summary>
        public new object SelectedItem
        {
            get
            {
                return base.SelectedItem;
            }

            set
            {
                base.SelectedItem = value;
            }
        }

        /// <summary>
        /// Gets or sets a collection that is used to generate the 
        /// content of the control.
        /// </summary>
        public new IEnumerable ItemsSource
        {
            get
            {
                return base.ItemsSource;
            }
            set
            {
                base.ItemsSource = value;
            }
        }

        /// <summary>
        /// Creates the automation peer.
        /// </summary>
        /// <returns>Returns a new ListBoxAutomationPeer.</returns>
        public AutomationPeer CreateAutomationPeer()
        {
            return new ListBoxAutomationPeer(this);
        }

        /// <summary>
        /// Provides handling for the KeyDown event that occurs when a key is 
        /// pressed while the control has focus.
        /// </summary>
        /// <param name="e">The key event arguments object.</param>
        public void HandleKeyDown(KeyEventArgs e)
        {
        }
        #endregion
    }
}