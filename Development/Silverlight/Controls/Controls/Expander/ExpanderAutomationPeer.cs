// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.Windows.Controls.Automation.Peers.ExpanderAutomationPeer.#System.Windows.Automation.Provider.IExpandCollapseProvider.Collapse()", Justification = "Required for subset compat with WPF")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.Windows.Controls.Automation.Peers.ExpanderAutomationPeer.#System.Windows.Automation.Provider.IExpandCollapseProvider.Expand()", Justification = "Required for subset compat with WPF")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.Windows.Controls.Automation.Peers.ExpanderAutomationPeer.#System.Windows.Automation.Provider.IExpandCollapseProvider.ExpandCollapseState", Justification = "Required for subset compat with WPF")]

namespace Microsoft.Windows.Controls.Automation.Peers
{
    /// <summary>
    /// Exposes Expander types to UI Automation.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public partial class ExpanderAutomationPeer
        : FrameworkElementAutomationPeer, IExpandCollapseProvider
    {
        /// <summary>
        /// Initializes a new instance of the ExpanderAutomationPeer class.
        /// </summary>
        /// <param name="owner">
        /// The Expander that is associated with this ExpanderAutomationPeer.
        /// </param>
        public ExpanderAutomationPeer(Expander owner)
            : base(owner)
        {
        }

        #region AutomationPeer overrides
        /// <summary>
        /// Gets the control type for the Expander that is associated
        /// with this ExpanderAutomationPeer.  This method is called by
        /// GetAutomationControlType.
        /// </summary>
        /// <returns>Group AutomationControlType.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }

        /// <summary>
        /// Gets the name of the Expander that is associated with this
        /// ExpanderAutomationPeer.  This method is called by GetClassName.
        /// </summary>
        /// <returns>The name Expander.</returns>
        protected override string GetClassNameCore()
        {
            return "Expander";
        }

        /// <summary>
        /// Gets the control pattern for the Expander that is associated
        /// with this ExpanderAutomationPeer.
        /// </summary>
        /// <param name="patternInterface">The desired PatternInterface.</param>
        /// <returns>The desired AutomationPeer or null.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ExpandCollapse)
            {
                return this;
            }

            return null;
        }
        #endregion

        #region Implement IExpandCollapseProvider
        /// <summary>
        /// Displays the content of the Expander.
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

            Expander owner = (Expander)Owner;
            owner.IsExpanded = true;
        }

        /// <summary>
        /// Hides all descendent controls of the Expander.
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

            Expander owner = (Expander)Owner;
            owner.IsExpanded = false;
        }

        /// <summary>
        /// Gets the state (expanded or collapsed) of the Expander.
        /// </summary>
        /// <remarks>
        /// This API supports the .NET Framework infrastructure and is not 
        /// intended to be used directly from your code.
        /// </remarks>
        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
        {
            get
            {
                Expander owner = (Expander)Owner;
                return owner.IsExpanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
            }
        }
        #endregion

        /// <summary>
        /// Raise an automation event when a Expander is expanded or collapsed.
        /// </summary>
        /// <param name="oldValue">
        /// A value indicating whether the Expander was expanded.
        /// </param>
        /// <param name="newValue">
        /// A value indicating whether the Expander is expanded.
        /// </param>
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            RaisePropertyChangedEvent(
                ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
                newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }
    }
}
