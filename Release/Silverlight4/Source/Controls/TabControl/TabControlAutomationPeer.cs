// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

[assembly: SuppressMessage("Microsoft.Usage", "CA2222:DoNotDecreaseInheritedMemberVisibility", Scope = "member", Target = "System.Windows.Automation.Peers.TabControlAutomationPeer.#CreateItemAutomationPeer(System.Object)", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.TabControlAutomationPeer.#System.Windows.Automation.Provider.ISelectionProvider.CanSelectMultiple", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.TabControlAutomationPeer.#System.Windows.Automation.Provider.ISelectionProvider.GetSelection()", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.TabControlAutomationPeer.#System.Windows.Automation.Provider.ISelectionProvider.IsSelectionRequired", Justification = "WPF Compatibility")]

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes <see cref="T:System.Windows.Controls.TabControl" /> types to UI
    /// automation.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public partial class TabControlAutomationPeer : ItemsControlAutomationPeer, ISelectionProvider
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Automation.Peers.TabControlAutomationPeer" />
        /// class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="T:System.Windows.Controls.TabControl" /> that is
        /// associated with this
        /// <see cref="T:System.Windows.Automation.Peers.TabControlAutomationPeer" />.
        /// </param>
        public TabControlAutomationPeer(TabControl owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Creates a new System.Windows.Automation.Peers.TabItemAutomationPeer.
        /// </summary>
        /// <param name="item">
        /// The System.Windows.Controls.TabItem that is associated with the new
        /// System.Windows.Automation.Peers.TabItemAutomationPeer.
        /// </param>
        /// <returns>The TabItemAutomationPeer that is created.</returns>
        private static new ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new TabItemAutomationPeer(item);
        }

        /// <summary>
        /// Gets the control type for the element that is associated with the UI
        /// Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Tab;
        }

        /// <summary>
        /// Called by GetClassName that gets a human readable name that, in
        /// addition to AutomationControlType, differentiates the control
        /// represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <summary>
        /// This method is called by
        /// System.Windows.Automation.Peers.AutomationPeer.GetClickablePoint().
        /// </summary>
        /// <returns>
        /// A System.Windows.Point containing System.Double.NaN,
        /// System.Double.NaN; the only clickable points in a
        /// System.Windows.Controls.TabControl are the child
        /// System.Windows.Controls.TabItem elements.
        /// </returns>
        protected override Point GetClickablePointCore()
        {
            return new Point(double.NaN, double.NaN);
        }

        /// <summary>
        /// Gets the control pattern for the
        /// <see cref="T:System.Windows.Controls.TabControl" /> that is
        /// associated with this
        /// <see cref="T:System.Windows.Automation.Peers.TabControlAutomationPeer" />.
        /// </summary>
        /// <param name="patternInterface">
        /// One of the enumeration values.
        /// </param>
        /// <returns>
        /// The object that implements the pattern interface, or null if the
        /// specified pattern interface is not implemented by this peer.
        /// </returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Selection)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets a value indicating whether the UI automation provider
        /// allows more than one child element to be selected concurrently.
        /// </summary>
        /// <value>
        /// True if multiple selection is allowed; otherwise, false.
        /// </value>
        bool ISelectionProvider.CanSelectMultiple
        {
            get { return false; }
        }

        /// <summary>
        /// Retrieves a UI automation provider for each child element that is
        /// selected.
        /// </summary>
        /// <returns>An array of UI automation providers.</returns>
        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            TabControl tabControl = Owner as TabControl;
            if (tabControl.SelectedItem == null)
            {
                return null;
            }
            List<IRawElementProviderSimple> list = new List<IRawElementProviderSimple>(1);

            ItemAutomationPeer peer = CreateItemAutomationPeer(tabControl.SelectedItem);

            if (peer != null)
            {
                list.Add(ProviderFromPeer(peer));
            }
            return list.ToArray();
        }

        /// <summary>
        /// Gets a value indicating whether the UI automation provider
        /// requires at least one child element to be selected.
        /// </summary>
        /// <value>
        /// True if selection is required; otherwise, false.
        /// </value>
        bool ISelectionProvider.IsSelectionRequired
        {
            get { return true; }
        }
    }
}