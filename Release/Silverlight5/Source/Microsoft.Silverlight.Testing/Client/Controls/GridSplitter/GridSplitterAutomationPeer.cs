// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation.Provider;
using Microsoft.Silverlight.Testing.Controls;
using System.Windows.Automation.Peers;
using System.Windows.Automation;

[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.GridSplitterAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.CanMove", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.GridSplitterAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.CanResize", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.GridSplitterAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.CanRotate", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.GridSplitterAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.Move(System.Double,System.Double)", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.GridSplitterAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.Resize(System.Double,System.Double)", Justification = "WPF Compatibility")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.GridSplitterAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.Rotate(System.Double)", Justification = "WPF Compatibility")]

namespace Microsoft.Silverlight.Testing.Controls
{
    /// <summary>
    /// Exposes <see cref="T:Microsoft.Silverlight.Testing.Controls.GridSplitter" /> types to
    /// UI automation.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public partial class GridSplitterAutomationPeer : FrameworkElementAutomationPeer, ITransformProvider
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Automation.Peers.GridSplitterAutomationPeer" />
        /// class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="T:Microsoft.Silverlight.Testing.Controls.GridSplitter" /> to
        /// associate with the
        /// <see cref="T:System.Windows.Automation.Peers.GridSplitterAutomationPeer" />.
        /// </param>
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
        /// Gets the control pattern for the
        /// <see cref="T:Microsoft.Silverlight.Testing.Controls.GridSplitter" /> that is
        /// associated with this
        /// <see cref="T:System.Windows.Automation.Peers.GridSplitterAutomationPeer" />.
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
            if (patternInterface == PatternInterface.Transform)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets a value indicating whether the control can be moved.
        /// </summary>
        /// <value>
        /// True if the element can be moved; otherwise, false.
        /// </value>
        bool ITransformProvider.CanMove { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether the UI automation element can be
        /// resized.
        /// </summary>
        /// <value>
        /// True if the element can be resized; otherwise, false.
        /// </value>
        bool ITransformProvider.CanResize { get { return false; } }

        /// <summary>
        /// Gets a value indicating whether the control can be rotated.
        /// </summary>
        /// <value>
        /// True if the element can be rotated; otherwise, false.
        /// </value>
        bool ITransformProvider.CanRotate { get { return false; } }

        /// <summary>
        /// Moves the control.
        /// </summary>
        /// <param name="x">
        /// Absolute screen coordinates of the left side of the control.
        /// </param>
        /// <param name="y">
        /// Absolute screen coordinates of the top of the control.
        /// </param>
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
        /// Resizes the control.
        /// </summary>
        /// <param name="width">The new width of the window, in pixels.</param>
        /// <param name="height">
        /// The new height of the window, in pixels.
        /// </param>
        void ITransformProvider.Resize(double width, double height)
        {
            // REMOVE_RTM: update when Jolt 23302 is fixed
            // throw new InvalidOperationException(Microsoft.Silverlight.Testing.Controls.Client.Controls.Properties.Resources.UIA_OperationCannotBePerformed);
        }

        /// <summary>
        /// Rotates the control.
        /// </summary>
        /// <param name="degrees">
        /// The number of degrees to rotate the control. A positive number
        /// rotates clockwise; a negative number rotates counterclockwise.
        /// </param>
        void ITransformProvider.Rotate(double degrees)
        {
            // REMOVE_RTM: update when Jolt 23302 is fixed
            // throw new InvalidOperationException(Microsoft.Silverlight.Testing.Controls.Client.Controls.Properties.Resources.UIA_OperationCannotBePerformed);
        }
    }
}