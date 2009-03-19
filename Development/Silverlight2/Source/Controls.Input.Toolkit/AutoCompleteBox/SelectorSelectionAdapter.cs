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

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents the selection adapter contained in the drop-down portion 
    /// of an AutoCompleteBox control.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class wraps a Selector control and implements the ISelectionAdapter 
    /// interface. A selection adapter is a part required by the AutoCompleteBox 
    /// control template. The SelectorSelectionAdapter provides custom behavior 
    /// for the AutoCompleteBox control, such as an item collection, selection 
    /// members, and key handling. 
    /// </para>
    /// <para>
    /// You can create a new template for the AutoCompleteBox and provide your 
    /// own ISelectionAdapter to provide custom selection functionality without 
    /// deriving from the AutoCompleteBox class.
    /// </para>
    /// </remarks>
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
        /// <remarks>
        /// The SelectorSelectionAdapter is used to set the Selector used by 
        /// the AutoCompleteBox control. You specify the SelectorControl 
        /// property in the template for the AutoCompleteBox.
        /// </remarks>
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
        /// Occurs when an item is selected and is committed to the underlying 
        /// Selector control.
        /// </summary>
        public event RoutedEventHandler Commit;

        /// <summary>
        /// Occurs when a selection is canceled before it is committed.
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
        /// Gets or sets the selected item of the selection adapter.
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
        /// content of the selection adapter.
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
        /// Increments the SelectedIndex property of the underlying Selector 
        /// control.
        /// </summary>
        /// <remarks>
        /// If the currently selected item is the last item in the collection, 
        /// SelectedIndex is set to -1.
        /// </remarks>
        protected void SelectedIndexIncrement()
        {
            if (SelectorControl != null)
            {
                SelectorControl.SelectedIndex = SelectorControl.SelectedIndex + 1 >= SelectorControl.Items.Count ? -1 : SelectorControl.SelectedIndex + 1;
            }
        }

        /// <summary>
        /// Decrements the SelectedIndex property of the underlying Selector 
        /// control.
        /// </summary>
        /// <remarks>
        /// If the currently selected item is the first item in the collection, 
        /// SelectedIndex is set to the index of the last item in the collection.
        /// </remarks>
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
        /// pressed while the drop-down portion of the AutoCompleteBox has focus.
        /// </summary>
        /// <remarks>
        /// This method is called by the KeyDown event of the AutoCompleteBox, 
        /// when the drop-down portion of the AutoCompleteBox has focus.
        /// This method marks the KeyDown event of the AutoCompleteBox as 
        /// handled when one of the following keys is pressed: Enter 
        /// The HandleKeyDown method raises the Commit event when the Enter 
        /// key is pressed. The method raises the Cancel event when the Escape 
        /// key is pressed.
        /// </remarks>
        /// <param name="e">
        /// A KeyEventArgs that contains data about the KeyDown event.
        /// </param>
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
        /// Raises the Commit event.
        /// </summary>
        /// <remarks>
        /// Raising an event invokes the event handler through a delegate. 
        /// The OnCommit method also allows derived classes to handle the 
        /// event without attaching a delegate. This is the preferred 
        /// technique for handling the event in a derived class.
        /// Notes to Inheritors: When overriding OnCommit in a derived class, 
        /// be sure to call the base class’s OnCommit method so that registered 
        /// delegates receive the event. 
        /// </remarks>
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
        /// Raises the Cancel event.
        /// </summary>
        /// <remarks>
        /// Raising an event invokes the event handler through a delegate. 
        /// The OnCancel method also allows derived classes to handle the 
        /// event without attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.
        /// Notes to Inheritors: When overriding OnCancel in a derived class, be 
        /// sure to call the base class’s OnCancel method so that registered 
        /// delegates receive the event. 
        /// </remarks>
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
        /// Returns an automation peer for the underlying Selector control, for 
        /// use by the Silverlight automation infrastructure.
        /// </summary>
        /// <returns>
        /// An automation peer for use by the Silverlight automation infrastructure.
        /// </returns>
        /// <remarks>
        /// This method returns an automation peer for the underlying Selector 
        /// control. It creates a new automation peer instance if one has not 
        /// been created; otherwise, it returns the automation peer previously 
        /// created. The automation peer is included as a child by the 
        /// AutoCompleteBox control.
        /// Classes that participate in the Silverlight automation 
        /// infrastructure must implement this method to return a class-specific 
        /// derived class of AutomationPeer that reports information for 
        /// automation behavior.
        /// </remarks>
        public AutomationPeer CreateAutomationPeer()
        {
            return _selector != null ? FrameworkElementAutomationPeer.CreatePeerForElement(_selector) : null;
        }
    }
}