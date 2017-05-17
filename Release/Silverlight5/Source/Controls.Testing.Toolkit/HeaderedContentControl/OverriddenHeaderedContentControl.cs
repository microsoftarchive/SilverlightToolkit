// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation.Peers;
using System.Windows.Input;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Overridden HeaderedContentControl that provides access to virtual members for testing.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Headered", Justification = "Consistency with WPF")]
    public partial class OverriddenHeaderedContentControl : HeaderedContentControl, IOverriddenHeaderedContentControl
    {
        /// <summary>
        /// Initializes a new instance of the HeaderedContentControl class.
        /// </summary>
        public OverriddenHeaderedContentControl()
        {
            Action invariantTest = null;
            HeaderChangedActions = new OverriddenMethod<object, object>(invariantTest);
            HeaderTemplateChangedActions = new OverriddenMethod<DataTemplate, DataTemplate>(invariantTest);
            ContentChangedActions = new OverriddenMethod<object, object>(invariantTest);
            GotFocusActions = new OverriddenMethod<RoutedEventArgs>(invariantTest);
            LostFocusActions = new OverriddenMethod<RoutedEventArgs>(invariantTest);
            KeyDownActions = new OverriddenMethod<KeyEventArgs>(invariantTest);
            KeyUpActions = new OverriddenMethod<KeyEventArgs>(invariantTest);
            MouseEnterActions = new OverriddenMethod<MouseEventArgs>(invariantTest);
            MouseLeaveActions = new OverriddenMethod<MouseEventArgs>(invariantTest);
            MouseMoveActions = new OverriddenMethod<MouseEventArgs>(invariantTest);
            MouseLeftButtonDownActions = new OverriddenMethod<MouseButtonEventArgs>(invariantTest);
            MouseLeftButtonUpActions = new OverriddenMethod<MouseButtonEventArgs>(invariantTest);
            ApplyTemplateActions = new OverriddenMethod(invariantTest);
            CreateAutomationPeerActions = new OverriddenMethod<AutomationPeer>(invariantTest);
            MeasureActions = new OverriddenMethod<Size, Size?>(invariantTest);
            ArrangeActions = new OverriddenMethod<Size, Size?>(invariantTest);
        }

        /// <summary>
        /// Gets test actions for the OnHeaderChanged method.
        /// </summary>
        public OverriddenMethod<object, object> HeaderChangedActions { get; private set; }

        /// <summary>
        /// Called when the Header property changes.
        /// </summary>
        /// <param name="oldHeader">
        /// The old value of the Header property.
        /// </param>
        /// <param name="newHeader">
        /// The new value of the Header property.
        /// </param>
        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            HeaderChangedActions.DoPreTest(oldHeader, newHeader);
            base.OnHeaderChanged(oldHeader, newHeader);
            HeaderChangedActions.DoTest(oldHeader, newHeader);
        }

        /// <summary>
        /// Gets test actions for the OnHeaderTemplateChanged method.
        /// </summary>
        public OverriddenMethod<DataTemplate, DataTemplate> HeaderTemplateChangedActions { get; private set; }

        /// <summary>
        /// Called when the HeaderTemplate property changes.
        /// </summary>
        /// <param name="oldHeaderTemplate">
        /// The old value of the HeaderTemplate property.
        /// </param>
        /// <param name="newHeaderTemplate">
        /// The new value of the HeaderTemplate property.
        /// </param>
        protected override void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
        {
            HeaderTemplateChangedActions.DoPreTest(oldHeaderTemplate, newHeaderTemplate);
            base.OnHeaderTemplateChanged(oldHeaderTemplate, newHeaderTemplate);
            HeaderTemplateChangedActions.DoTest(oldHeaderTemplate, newHeaderTemplate);
        }

        /// <summary>
        /// Gets test actions for the OnContentChanged method.
        /// </summary>
        public OverriddenMethod<object, object> ContentChangedActions { get; private set; }

        /// <summary>
        /// Called when the Content property changes.
        /// </summary>
        /// <param name="oldContent">
        /// The old value of the Content property.
        /// </param>
        /// <param name="newContent">
        /// The new value of the Content property.
        /// </param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            ContentChangedActions.DoPreTest(oldContent, newContent);
            base.OnContentChanged(oldContent, newContent);
            ContentChangedActions.DoTest(oldContent, newContent);
        }

        /// <summary>
        /// Gets the OnGotFocus test actions.
        /// </summary>
        public OverriddenMethod<RoutedEventArgs> GotFocusActions { get; private set; }

        /// <summary>
        /// Handle the GotFocus event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            GotFocusActions.DoPreTest(e);
            base.OnGotFocus(e);
            GotFocusActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnLostFocus test actions.
        /// </summary>
        public OverriddenMethod<RoutedEventArgs> LostFocusActions { get; private set; }

        /// <summary>
        /// Handle the LostFocus event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            LostFocusActions.DoPreTest(e);
            base.OnLostFocus(e);
            LostFocusActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnKeyDown test actions.
        /// </summary>
        public OverriddenMethod<KeyEventArgs> KeyDownActions { get; private set; }

        /// <summary>
        /// Handle the KeyDown event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            KeyDownActions.DoPreTest(e);
            base.OnKeyDown(e);
            KeyDownActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnKeyUp test actions.
        /// </summary>
        public OverriddenMethod<KeyEventArgs> KeyUpActions { get; private set; }

        /// <summary>
        /// Handle the KeyUp event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            KeyUpActions.DoPreTest(e);
            base.OnKeyUp(e);
            KeyUpActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnMouseEnter test actions.
        /// </summary>
        public OverriddenMethod<MouseEventArgs> MouseEnterActions { get; private set; }

        /// <summary>
        /// Handle the MouseEnter event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            MouseEnterActions.DoPreTest(e);
            base.OnMouseEnter(e);
            MouseEnterActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnMouseLeave test actions.
        /// </summary>
        public OverriddenMethod<MouseEventArgs> MouseLeaveActions { get; private set; }

        /// <summary>
        /// Handle the MouseLeave event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            MouseLeaveActions.DoPreTest(e);
            base.OnMouseLeave(e);
            MouseLeaveActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnMouseMove test actions.
        /// </summary>
        public OverriddenMethod<MouseEventArgs> MouseMoveActions { get; private set; }

        /// <summary>
        /// Handle the MouseMove event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            MouseMoveActions.DoPreTest(e);
            base.OnMouseMove(e);
            MouseMoveActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnMouseLeftButtonDown test actions.
        /// </summary>
        public OverriddenMethod<MouseButtonEventArgs> MouseLeftButtonDownActions { get; private set; }

        /// <summary>
        /// Handle the MouseLeftButtonDown event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            MouseLeftButtonDownActions.DoPreTest(e);
            base.OnMouseLeftButtonDown(e);
            MouseLeftButtonDownActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnMouseLeftButtonUp test actions.
        /// </summary>
        public OverriddenMethod<MouseButtonEventArgs> MouseLeftButtonUpActions { get; private set; }

        /// <summary>
        /// Handle the MouseLeftButtonUp event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            MouseLeftButtonUpActions.DoPreTest(e);
            base.OnMouseLeftButtonUp(e);
            MouseLeftButtonUpActions.DoTest(e);
        }

        /// <summary>
        /// Gets test actions for the OnItemsChanged method.
        /// </summary>
        public OverriddenMethod ApplyTemplateActions { get; private set; }

        /// <summary>
        /// Apply a control template and updates its visual
        /// state.
        /// </summary>
        public override void OnApplyTemplate()
        {
            ApplyTemplateActions.DoPreTest();
            base.OnApplyTemplate();
            ApplyTemplateActions.DoTest();
        }

        /// <summary>
        /// Gets test actions for the OnCreateAutomationPeer method.
        /// </summary>
        public OverriddenMethod<AutomationPeer> CreateAutomationPeerActions { get; private set; }

        /// <summary>
        /// Defines an AutomationPeer for the control.
        /// </summary>
        /// <returns>
        /// An AutomationPeer for the control.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            CreateAutomationPeerActions.DoPreTest(null);
            AutomationPeer peer = base.OnCreateAutomationPeer();
            CreateAutomationPeerActions.DoTest(peer);
            return peer;
        }

        /// <summary>
        /// Gets test actions for the MeasureOverride method.
        /// </summary>
        public OverriddenMethod<Size, Size?> MeasureActions { get; private set; }

        /// <summary>
        /// Measures the control.
        /// </summary>
        /// <param name="availableSize">
        /// Size available for the control.
        /// </param>
        /// <returns>Desired size of the control.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            MeasureActions.DoPreTest(availableSize, null);
            Size desired = base.MeasureOverride(availableSize);
            MeasureActions.DoTest(availableSize, desired);
            return desired;
        }

        /// <summary>
        /// Gets test actions for the ArrangeOverride method.
        /// </summary>
        public OverriddenMethod<Size, Size?> ArrangeActions { get; private set; }

        /// <summary>
        /// Arrange the control.
        /// </summary>
        /// <param name="finalSize">Final size for the control.</param>
        /// <returns>Final size used by the control.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeActions.DoPreTest(finalSize, null);
            Size used = base.ArrangeOverride(finalSize);
            ArrangeActions.DoTest(finalSize, used);
            return used;
        }
    }
}