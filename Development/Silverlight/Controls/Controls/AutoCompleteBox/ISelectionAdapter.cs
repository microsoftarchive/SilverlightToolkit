// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Represents an interface to a control or object that has items and 
    /// selection members.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public interface ISelectionAdapter
    {
        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        object SelectedItem { get; set; }

        /// <summary>
        /// Occurs when the SelectedItem property value changes.
        /// </summary>
        event SelectionChangedEventHandler SelectionChanged;
        
        /// <summary>
        /// Gets or sets a collection that is used to generate the content of 
        /// the control.
        /// </summary>
        IEnumerable ItemsSource { get; set; }

        /// <summary>
        /// Occurs when a selection has occurred and has been committed.
        /// </summary>
        event RoutedEventHandler Commit;

        /// <summary>
        /// Occurs when a selection has occurred and has been canceled.
        /// </summary>
        event RoutedEventHandler Cancel;

        /// <summary>
        /// Provides handling for the KeyDown event that occurs when a key is 
        /// pressed while the control has focus.
        /// </summary>
        /// <param name="e">The event data.</param>
        void HandleKeyDown(KeyEventArgs e);

        /// <summary>
        /// Creates an automation peer for the selection adapter.
        /// </summary>
        /// <returns>Returns a new automation peer for the selection adapter, if
        /// available.</returns>
        AutomationPeer CreateAutomationPeer();
    }
}