// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.Windows.Controls.Automation.Peers.TreeViewItemAutomationPeer.#System.Windows.Automation.Provider.IExpandCollapseProvider.Collapse()", Justification = "Required for subset compat with WPF")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.Windows.Controls.Automation.Peers.TreeViewItemAutomationPeer.#System.Windows.Automation.Provider.IExpandCollapseProvider.Expand()", Justification = "Required for subset compat with WPF")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.Windows.Controls.Automation.Peers.TreeViewItemAutomationPeer.#System.Windows.Automation.Provider.IExpandCollapseProvider.ExpandCollapseState", Justification = "Required for subset compat with WPF")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.Windows.Controls.Automation.Peers.TreeViewItemAutomationPeer.#System.Windows.Automation.Provider.IScrollItemProvider.ScrollIntoView()", Justification = "Required for subset compat with WPF")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.Windows.Controls.Automation.Peers.TreeViewItemAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.AddToSelection()", Justification = "Required for subset compat with WPF")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.Windows.Controls.Automation.Peers.TreeViewItemAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.IsSelected", Justification = "Required for subset compat with WPF")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.Windows.Controls.Automation.Peers.TreeViewItemAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.RemoveFromSelection()", Justification = "Required for subset compat with WPF")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.Windows.Controls.Automation.Peers.TreeViewItemAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.Select()", Justification = "Required for subset compat with WPF")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.Windows.Controls.Automation.Peers.TreeViewItemAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.SelectionContainer", Justification = "Required for subset compat with WPF")]

namespace Microsoft.Windows.Controls.Automation.Peers
{
    /// <summary>
    /// Exposes TreeViewItem types to UI Automation.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public partial class TreeViewItemAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider, ISelectionItemProvider, IScrollItemProvider
    {
        /// <summary>
        /// Gets the TreeViewItem that owns this TreeViewItemAutomationPeer.
        /// </summary>
        private TreeViewItem OwnerTreeViewItem
        {
            get { return (TreeViewItem) Owner; }
        }

        /// <summary>
        /// Gets the state (expanded or collapsed) of the TreeViewItem.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
        {
            get
            {
                TreeViewItem owner = OwnerTreeViewItem;
                if (!owner.HasItems)
                {
                    return ExpandCollapseState.LeafNode;
                }
                else if (!owner.IsExpanded)
                {
                    return ExpandCollapseState.Collapsed;
                }
                else
                {
                    return ExpandCollapseState.Expanded;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the TreeViewItem is selected.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        bool ISelectionItemProvider.IsSelected
        {
            get { return OwnerTreeViewItem.IsSelected; }
        }

        /// <summary>
        /// Gets the UI Automation provider that implements ISelectionProvider
        /// and acts as the container for the calling object.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get
            {
                ItemsControl parent = OwnerTreeViewItem.ParentItemsControl;
                if (parent != null)
                {
                    AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(parent);
                    if (peer != null)
                    {
                        return ProviderFromPeer(peer);
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the TreeViewItemAutomationPeer class.
        /// </summary>
        /// <param name="owner">
        /// The TreeViewItem that is associated with this
        /// TreeViewItemAutomationPeer.
        /// </param>
        public TreeViewItemAutomationPeer(TreeViewItem owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control type for the TreeViewItem that is associated
        /// with this TreeViewItemAutomationPeer.  This method is called by
        /// GetAutomationControlType.
        /// </summary>
        /// <returns>Tree AutomationControlType.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.TreeItem;
        }

        /// <summary>
        /// Gets the name of the TreeViewItem that is associated with this
        /// TreeViewItemAutomationPeer.  This method is called by GetClassName.
        /// </summary>
        /// <returns>The name TreeView.</returns>
        protected override string GetClassNameCore()
        {
            return "TreeViewItem";
        }

        /// <summary>
        /// Gets the control pattern for the TreeViewItem that is associated
        /// with this TreeViewItemAutomationPeer.
        /// </summary>
        /// <param name="patternInterface">The desired PatternInterface.</param>
        /// <returns>The desired AutomationPeer or null.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ExpandCollapse ||
                patternInterface == PatternInterface.SelectionItem ||
                patternInterface == PatternInterface.ScrollItem)
            {
                return this;
            }

            return null;
        }

        /// <summary>
        /// Raise the IsSelected property changed event.
        /// </summary>
        /// <param name="isSelected">
        /// A value indicating whether the TreeViewItem is selected.
        /// </param>
        internal void RaiseAutomationIsSelectedChanged(bool isSelected)
        {
            RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, !isSelected, isSelected);
        }

        /// <summary>
        /// Raise an automation event when a TreeViewItem is expanded or
        /// collapsed.
        /// </summary>
        /// <param name="oldValue">
        /// A value indicating whether the TreeViewItem was expanded.
        /// </param>
        /// <param name="newValue">
        /// A value indicating whether the TreeViewItem is expanded.
        /// </param>
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            RaisePropertyChangedEvent(
                ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
                newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }

        /// <summary>
        /// Displays the child items of the TreeViewItem.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        void IExpandCollapseProvider.Expand()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            TreeViewItem owner = OwnerTreeViewItem;
            if (!owner.HasItems)
            {
                throw new InvalidOperationException(Properties.Resources.Automation_OperationCannotBePerformed);
            }

            owner.IsExpanded = true;
        }

        /// <summary>
        /// Hides all descendent controls of the TreeViewItem.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        void IExpandCollapseProvider.Collapse()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            TreeViewItem owner = OwnerTreeViewItem;
            if (!owner.HasItems)
            {
                throw new InvalidOperationException(Properties.Resources.Automation_OperationCannotBePerformed);
            }

            owner.IsExpanded = false;
        }

        /// <summary>
        /// Adds the TreeViewItem to the collection of selected items.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        void ISelectionItemProvider.AddToSelection()
        {
            TreeViewItem owner = OwnerTreeViewItem;
            TreeView parent = owner.ParentTreeView;
            if (parent == null || (parent.SelectedItem != null && parent.SelectedContainer != Owner))
            {
                throw new InvalidOperationException(Properties.Resources.Automation_OperationCannotBePerformed);
            }
            owner.IsSelected = true;
        }

        /// <summary>
        /// Clears selection from currently selected items and then proceeds to
        /// select the current TreeViewItem.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        void ISelectionItemProvider.Select()
        {
            OwnerTreeViewItem.IsSelected = true;
        }

        /// <summary>
        /// Removes the current TreeViewItem from the collection of selected
        /// items.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        void ISelectionItemProvider.RemoveFromSelection()
        {
            OwnerTreeViewItem.IsSelected = false;
        }

        /// <summary>
        /// Scrolls the content area of a TreeView in order to display the
        /// TreeViewItem within the visible region (viewport) of the TreeView.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        void IScrollItemProvider.ScrollIntoView()
        {
            // Note: WPF just calls BringIntoView on the current TreeViewItem.
            // This actually raises an event that can be handled by the
            // its containers.  Silverlight doesn't support this, so we will
            // approximate by moving scrolling the TreeView's ScrollHost to the
            // item.

            // Get the parent TreeView
            TreeViewItem owner = OwnerTreeViewItem;
            TreeView parent = owner.ParentTreeView;
            if (parent == null)
            {
                return;
            }

            // Scroll the item into view
            parent.ItemContainerGenerator.ScrollIntoView(owner);
        }
    }
}