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
    /// AutomationPeer for GridSplitter.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    sealed public class GridSplitterAutomationPeer : FrameworkElementAutomationPeer, ITransformProvider
    {
        /// <summary>
        /// AutomationPeer for GridSplitter.
        /// </summary>
        /// <param name="owner">The GridSplitter.</param>
        public GridSplitterAutomationPeer(GridSplitter owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the control type for the element that is associated with the UI
        /// Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Thumb;
        }

        /// <summary>
        /// Called by GetClassName that gets a human readable name that, in
        /// addition to AutomationControlType,  differentiates the control
        /// represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
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
            if (patternInterface == PatternInterface.Transform)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets a value indicating whether Inherited code: Requires comment.
        /// </summary>
        bool ITransformProvider.CanMove { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether Inherited code: Requires comment.
        /// </summary>
        bool ITransformProvider.CanResize { get { return false; } }

        /// <summary>
        /// Gets a value indicating whether Inherited code: Requires comment.
        /// </summary>
        bool ITransformProvider.CanRotate { get { return false; } }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="x">Inherited code: Requires comment 1.</param>
        /// <param name="y">Inherited code: Requires comment 2.</param>
        void ITransformProvider.Move(double x, double y)
        {
            GridSplitter owner = (GridSplitter)Owner;
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            if (double.IsInfinity(x) || double.IsNaN(x))
            {
                // REMOVE_RTM: update when Jolt 23302 is fixed
                // throw new ArgumentOutOfRangeException("x");
                return;
            }

            if (double.IsInfinity(y) || double.IsNaN(y))
            {
                // REMOVE_RTM: update when Jolt 23302 is fixed
                // throw new ArgumentOutOfRangeException("y");
                return;
            }

            owner.InitializeAndMoveSplitter(x, y);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="width">Inherited code: Requires comment 1.</param>
        /// <param name="height">Inherited code: Requires comment 2.</param>
        void ITransformProvider.Resize(double width, double height)
        {
            // REMOVE_RTM: update when Jolt 23302 is fixed
            // throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.UIA_OperationCannotBePerformed);
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="degrees">Inherited code: Requires comment 1.</param>
        void ITransformProvider.Rotate(double degrees)
        {
            // REMOVE_RTM: update when Jolt 23302 is fixed
            // throw new InvalidOperationException(System.Windows.Controls.Properties.Resources.UIA_OperationCannotBePerformed);
        }
    }
}