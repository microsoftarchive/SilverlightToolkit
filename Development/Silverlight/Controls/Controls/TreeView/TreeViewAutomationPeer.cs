// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.Windows.Controls.Automation.Peers.TreeViewAutomationPeer.#System.Windows.Automation.Provider.ISelectionProvider.CanSelectMultiple", Justification = "Required for subset compat with WPF")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.Windows.Controls.Automation.Peers.TreeViewAutomationPeer.#System.Windows.Automation.Provider.ISelectionProvider.GetSelection()", Justification = "Required for subset compat with WPF")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.Windows.Controls.Automation.Peers.TreeViewAutomationPeer.#System.Windows.Automation.Provider.ISelectionProvider.IsSelectionRequired", Justification = "Required for subset compat with WPF")]

namespace Microsoft.Windows.Controls.Automation.Peers
{
    /// <summary>
    /// Exposes TreeView types to UI Automation.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public partial class TreeViewAutomationPeer : FrameworkElementAutomationPeer, ISelectionProvider
    {
        /// <summary>
        /// Gets the TreeView that owns this TreeViewAutomationPeer.
        /// </summary>
        private TreeView OwnerTreeView
        {
            get { return (TreeView) Owner; }
        }

        /// <summary>
        /// Gets a value indicating whether the UI Automation provider allows
        /// more than one child element to be selected concurrently.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        bool ISelectionProvider.CanSelectMultiple
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the UI Automation provider requires
        /// at least one child element to be selected.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        bool ISelectionProvider.IsSelectionRequired
        {
            get { return false; }
        }

        /// <summary>
        /// Initializes a new instance of the TreeViewAutomationPeer class.
        /// </summary>
        /// <param name="owner">
        /// The TreeView that is associated with this TreeViewAutomationPeer.
        /// </param>
        public TreeViewAutomationPeer(TreeView owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control type for the TreeView that is associated
        /// with this TreeViewAutomationPeer.  This method is called by
        /// GetAutomationControlType.
        /// </summary>
        /// <returns>Tree AutomationControlType.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Tree;
        }

        /// <summary>
        /// Gets the name of the TreeView that is associated with this
        /// TreeViewAutomationPeer.  This method is called by GetClassName.
        /// </summary>
        /// <returns>The name TreeView.</returns>
        protected override string GetClassNameCore()
        {
            return "TreeView";
        }

        /// <summary>
        /// Gets the control pattern for the TreeView that is associated with
        /// this TreeViewAutomationPeer.
        /// </summary>
        /// <param name="patternInterface">The desired PatternInterface.</param>
        /// <returns>The desired AutomationPeer or null.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Selection)
            {
                return this;
            }
            else if (patternInterface == PatternInterface.Scroll)
            {
                ScrollViewer scroll = OwnerTreeView.ItemContainerGenerator.ScrollHost;
                if (scroll != null)
                {
                    AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(scroll);
                    IScrollProvider provider = peer as IScrollProvider;
                    if (provider != null)
                    {
                        peer.EventsSource = this;
                        return provider;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the collection of child elements of the TreeView that is
        /// associated with this TreeViewAutomationPeer.  This method is called
        /// by GetChildren.
        /// </summary>
        /// <returns>
        /// A collection of TreeViewItemAutomationPeer elements, or null if the
        /// TreeView that is associated with this TreeViewAutomationPeer is
        /// empty.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Required by automation")]
        protected override List<AutomationPeer> GetChildrenCore()
        {
            TreeView owner = OwnerTreeView;
            
            ItemCollection items = owner.Items;
            if (items.Count <= 0)
            {
                return null;
            }

            List<AutomationPeer> peers = new List<AutomationPeer>(items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                TreeViewItem element = owner.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
                if (element != null)
                {
                    peers.Add(
                        FrameworkElementAutomationPeer.FromElement(element) ??
                        FrameworkElementAutomationPeer.CreatePeerForElement(element));
                }
            }

            return peers;
        }

        /// <summary>
        /// Retrieves a UI Automation provider for each child element that is
        /// selected.
        /// </summary>
        /// <returns>A collection of UI Automation providers.</returns>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            IRawElementProviderSimple[] selection = null;

            TreeViewItem selectedItem = OwnerTreeView.SelectedContainer;
            if (selectedItem != null)
            {
                AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(selectedItem);
                if (peer != null)
                {
                    selection = new IRawElementProviderSimple[] { ProviderFromPeer(peer) };
                }
            }

            return selection ?? new IRawElementProviderSimple[] { };
        }
    }
}