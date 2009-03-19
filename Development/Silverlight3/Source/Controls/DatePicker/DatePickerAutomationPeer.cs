// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// AutomationPeer for DatePicker Control.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public sealed class DatePickerAutomationPeer :
        FrameworkElementAutomationPeer,
        IExpandCollapseProvider,
        IValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the AutomationPeer for DatePicker
        /// control.
        /// </summary>
        /// <param name="owner">The DatePicker.</param>
        public DatePickerAutomationPeer(DatePicker owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets Inherited code: Requires comment.
        /// </summary>
        private DatePicker OwningDatePicker
        {
            get { return this.Owner as DatePicker; }
        }
        
        /// <summary>
        /// Gets the control pattern that is associated with the specified
        /// System.Windows.Automation.Peers.PatternInterface.
        /// </summary>
        /// <param name="patternInterface">
        /// A value from the System.Windows.Automation.Peers.PatternInterface
        /// enumeration.
        /// </param>
        /// <returns>
        /// The object that supports the specified pattern, or null if
        /// unsupported.
        /// </returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ExpandCollapse || patternInterface == PatternInterface.Value)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets the control type for the element that is associated with the UI
        /// Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ComboBox;
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
        /// Overrides the GetLocalizedControlTypeCore method for DatePicker.
        /// </summary>
        /// <returns>Inherited code: Requires comment.</returns>
        protected override string GetLocalizedControlTypeCore()
        {
            return System.Windows.Controls.Properties.Resources.DatePickerAutomationPeer_LocalizedControlType;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <returns>Inherited code: Requires comment 1.</returns>
        protected override string GetNameCore()
        {
            string nameCore = base.GetNameCore();

            if (string.IsNullOrEmpty(nameCore))
            {
                AutomationPeer labeledByCore = this.GetLabeledByCore();
                if (labeledByCore != null)
                {
                    nameCore = labeledByCore.GetName();
                }
                if (string.IsNullOrEmpty(nameCore))
                {
                    nameCore = this.OwningDatePicker.ToString();
                }
            }
            return nameCore;
        }

        /// <summary>
        /// Gets Inherited code: Requires comment.
        /// </summary>
        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
        {
            get
            {
                if (this.OwningDatePicker.IsDropDownOpen)
                {
                    return ExpandCollapseState.Expanded;
                }
                else
                {
                    return ExpandCollapseState.Collapsed;
                }
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        void IExpandCollapseProvider.Collapse()
        {
            this.OwningDatePicker.IsDropDownOpen = false;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        void IExpandCollapseProvider.Expand()
        {
            this.OwningDatePicker.IsDropDownOpen = true;
        }

        /// <summary>
        /// Gets a value indicating whether Inherited code: Requires comment.
        /// </summary>
        bool IValueProvider.IsReadOnly { get { return false; } }

        /// <summary>
        /// Gets Inherited code: Requires comment.
        /// </summary>
        string IValueProvider.Value { get { return this.OwningDatePicker.ToString(); } }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="value">Inherited code: Requires comment 1.</param>
        void IValueProvider.SetValue(string value)
        {
            this.OwningDatePicker.Text = value;
        }
    }
}