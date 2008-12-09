// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Used as a wrapper object by AutoCompleteBox to represent a Selector 
    /// control.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public partial class SelectorSelectionAdapter : ISelectionAdapter
    {
        /// <summary>
        /// The Selector instance.
        /// </summary>
        private Selector _selector;

        /// <summary>
        /// Gets or sets a value indicating whether the selection change event 
        /// should not be fired.
        /// </summary>
        private bool IgnoringSelectionChanged { get; set; }

        /// <summary>
        /// Gets or sets the underlying Selector control.
        /// </summary>
        public Selector SelectorControl
        {
            get { return _selector; }
            
            set
            {
                if (_selector != null)
                {
                    _selector.SelectionChanged -= OnSelectionChanged;
                    _selector.MouseLeftButtonUp -= OnSelectorMouseLeftButtonUp;
                }

                _selector = value;

                if (_selector != null)
                {
                    _selector.SelectionChanged += OnSelectionChanged;
                    _selector.MouseLeftButtonUp += OnSelectorMouseLeftButtonUp;
                }
            }
        }

        /// <summary>
        /// Occurs when the SelectedItem property value changes.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Occurs when a selection has occurred and has been committed.
        /// </summary>
        public event RoutedEventHandler Commit;

        /// <summary>
        /// Occurs when a selection has occurred and has been canceled.
        /// </summary>
        public event RoutedEventHandler Cancel;

        /// <summary>
        /// Initializes a new instance of the SelectorSelectionAdapter class.
        /// </summary>
        public SelectorSelectionAdapter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SelectorSelectionAdapter class.
        /// </summary>
        /// <param name="selector">A selector control instance.</param>
        public SelectorSelectionAdapter(Selector selector)
        {
            SelectorControl = selector;
        }

        /// <summary>
        /// Gets or sets the selected item through the adapter.
        /// </summary>
        public object SelectedItem 
        {
            get 
            { 
                return SelectorControl == null ? null : SelectorControl.SelectedItem; 
            }
            
            set
            {
                IgnoringSelectionChanged = true;
                if (SelectorControl != null)
                {
                    SelectorControl.SelectedItem = value;
                }
                
                // Attempt to reset the scroll viewer's position
                if (value == null)
                {
                    ResetScrollViewer();
                }

                IgnoringSelectionChanged = false;
            }
        }

        /// <summary>
        /// Gets or sets a collection that is used to generate the 
        /// content of the control.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return SelectorControl == null ? null : SelectorControl.ItemsSource; 
            }
            set 
            {
                if (SelectorControl != null)
                {
                    SelectorControl.ItemsSource = value;
                }
            }
        }

        /// <summary>
        /// If the control contains a ScrollViewer, this will reset the viewer 
        /// to be scrolled to the top.
        /// </summary>
        private void ResetScrollViewer()
        {
            if (SelectorControl != null)
            {
                ScrollViewer sv = SelectorControl.GetLogicalChildrenBreadthFirst().OfType<ScrollViewer>().FirstOrDefault();
                if (sv != null)
                {
                    sv.ScrollToTop();
                }
            }
        }

        /// <summary>
        /// Handles the mouse left button up event on the selector control.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnSelectorMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnCommit();
        }

        /// <summary>
        /// Handles the SelectionChanged event on the Selector control.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The selection changed event data.</param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IgnoringSelectionChanged)
            {
                return;
            }

            SelectionChangedEventHandler handler = SelectionChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// Increment the selected index, or wrap.
        /// </summary>
        protected void SelectedIndexIncrement()
        {
            if (SelectorControl != null)
            {
                SelectorControl.SelectedIndex = SelectorControl.SelectedIndex + 1 >= SelectorControl.Items.Count ? -1 : SelectorControl.SelectedIndex + 1;
            }
        }

        /// <summary>
        /// Decrement the SelectedIndex, or wrap around, inside the nested 
        /// SelectionAdapter's control.
        /// </summary>
        protected void SelectedIndexDecrement()
        {
            if (SelectorControl != null)
            {
                int index = SelectorControl.SelectedIndex;
                if (index >= 0)
                {
                    SelectorControl.SelectedIndex--;
                }
                else if (index == -1)
                {
                    SelectorControl.SelectedIndex = SelectorControl.Items.Count - 1;
                }
            }
        }

        /// <summary>
        /// Provides handling for the KeyDown event that occurs when a key is 
        /// pressed while the control has focus.
        /// </summary>
        /// <param name="e">The key event arguments object.</param>
        public void HandleKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    OnCommit();
                    e.Handled = true;
                    break;

                case Key.Up:
                    SelectedIndexDecrement();
                    e.Handled = true;
                    break;

                case Key.Down:
                    if ((ModifierKeys.Alt & Keyboard.Modifiers) == ModifierKeys.None)
                    {
                        SelectedIndexIncrement();
                        e.Handled = true;
                    }
                    break;

                case Key.Escape:
                    OnCancel();
                    e.Handled = true;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Fires the Commit event.
        /// </summary>
        protected virtual void OnCommit()
        {
            OnCommit(this, new RoutedEventArgs());
        }

        /// <summary>
        /// Fires the Commit event.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnCommit(object sender, RoutedEventArgs e)
        {
            RoutedEventHandler handler = Commit;
            if (handler != null)
            {
                handler(sender, e);
            }

            AfterAdapterAction();
        }

        /// <summary>
        /// Fires the Cancel event.
        /// </summary>
        protected virtual void OnCancel()
        {
            OnCancel(this, new RoutedEventArgs());
        }

        /// <summary>
        /// Fires the Cancel event.
        /// </summary>
        /// <param name="sender">The source object.</param>
        /// <param name="e">The event data.</param>
        private void OnCancel(object sender, RoutedEventArgs e)
        {
            RoutedEventHandler handler = Cancel;
            if (handler != null)
            {
                handler(sender, e);
            }

            AfterAdapterAction();
        }

        /// <summary>
        /// Change the selection after the actions are complete.
        /// </summary>
        private void AfterAdapterAction()
        {
            IgnoringSelectionChanged = true;
            if (SelectorControl != null)
            {
                SelectorControl.SelectedItem = null;
                SelectorControl.SelectedIndex = -1;
            }
            IgnoringSelectionChanged = false;
        }

        /// <summary>
        /// Creates an automation peer for the selection adapter, included as a 
        /// child by the AutoCompleteBox control.
        /// </summary>
        /// <returns>Returns null or a new instance of the AutomationPeer.</returns>
        public AutomationPeer CreateAutomationPeer()
        {
            return _selector != null ? FrameworkElementAutomationPeer.CreatePeerForElement(_selector) : null;
        }
    }
}