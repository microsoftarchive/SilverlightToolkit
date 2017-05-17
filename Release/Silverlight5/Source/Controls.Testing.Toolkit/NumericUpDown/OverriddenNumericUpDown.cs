// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Overridden NumericUpDown that provides access to virtual members for
    /// testing.
    /// </summary>
    public partial class OverriddenNumericUpDown : NumericUpDown, IOverriddenNumericUpDown
    {
        /// <summary>
        /// Initializes a new instance of the OverriddenNumericUpDown class.
        /// </summary>
        public OverriddenNumericUpDown()
        {
            MinimumChangedActions = new OverriddenMethod<double, double>();
            MaximumChangedActions = new OverriddenMethod<double, double>();
            IncrementChangedActions = new OverriddenMethod<double, double>();
            DecimalPlacesChangedActions = new OverriddenMethod<int, int>();
            ValueChangedActions = new OverriddenMethod<RoutedPropertyChangedEventArgs<double>>();
            ValueChangingActions = new OverriddenMethod<RoutedPropertyChangingEventArgs<double>>();
            IsEditableChangedActions = new OverriddenMethod<bool, bool>();
            ParseErrorActions = new OverriddenMethod<UpDownParseErrorEventArgs>();
            SpinActions = new OverriddenMethod<SpinEventArgs>();
            ApplyValueActions = new OverriddenMethod<string>();
            FormatValueActions = new OverriddenMethod<string>();
            IncrementActions = new OverriddenMethod();
            DecrementActions = new OverriddenMethod();
            SpinnerStyleChangedActions = new OverriddenMethod<Style, Style>();
            GotFocusActions = new OverriddenMethod<RoutedEventArgs>();
            LostFocusActions = new OverriddenMethod<RoutedEventArgs>();
            KeyDownActions = new OverriddenMethod<KeyEventArgs>();
            KeyUpActions = new OverriddenMethod<KeyEventArgs>();
            MouseEnterActions = new OverriddenMethod<MouseEventArgs>();
            MouseLeaveActions = new OverriddenMethod<MouseEventArgs>();
            MouseMoveActions = new OverriddenMethod<MouseEventArgs>();
            MouseLeftButtonDownActions = new OverriddenMethod<MouseButtonEventArgs>();
            MouseLeftButtonUpActions = new OverriddenMethod<MouseButtonEventArgs>();
            ApplyTemplateActions = new OverriddenMethod();
            CreateAutomationPeerActions = new OverriddenMethod<AutomationPeer>();
            MeasureActions = new OverriddenMethod<Size, Size?>();
            ArrangeActions = new OverriddenMethod<Size, Size?>();
        }

        /// <summary>
        /// Gets the OnMinimumChanged test actions.
        /// </summary>
        public OverriddenMethod<double, double> MinimumChangedActions { get; private set; }

        /// <summary>
        /// Called when the Minimum property value has changed.
        /// </summary>
        /// <param name="oldValue">Old value of the Minimum property.</param>
        /// <param name="newValue">New value of the Minimum property.</param>
        protected override void OnMinimumChanged(double oldValue, double newValue)
        {
            MinimumChangedActions.DoPreTest(oldValue, newValue);
            base.OnMinimumChanged(oldValue, newValue);
            MinimumChangedActions.DoTest(oldValue, newValue);
        }

        /// <summary>
        /// Gets the OnMaximumChanged test actions.
        /// </summary>
        public OverriddenMethod<double, double> MaximumChangedActions { get; private set; }

        /// <summary>
        /// Called when the Maximum property value has changed.
        /// </summary>
        /// <param name="oldValue">Old value of the Maximum property.</param>
        /// <param name="newValue">New value of the Maximum property.</param>
        protected override void OnMaximumChanged(double oldValue, double newValue)
        {
            MaximumChangedActions.DoPreTest(oldValue, newValue);
            base.OnMaximumChanged(oldValue, newValue);
            MaximumChangedActions.DoTest(oldValue, newValue);
        }

        /// <summary>
        /// Gets the OnIncrementChanged test actions.
        /// </summary>
        public OverriddenMethod<double, double> IncrementChangedActions { get; private set; }

        /// <summary>
        /// Called when the Increment property value has changed.
        /// </summary>
        /// <param name="oldValue">Old value of the Increment property.</param>
        /// <param name="newValue">New value of the Increment property.</param>
        protected override void OnIncrementChanged(double oldValue, double newValue)
        {
            IncrementChangedActions.DoPreTest(oldValue, newValue);
            base.OnIncrementChanged(oldValue, newValue);
            IncrementChangedActions.DoTest(oldValue, newValue);
        }

        /// <summary>
        /// Gets the OnDecimalPlacesChanged test actions.
        /// </summary>
        public OverriddenMethod<int, int> DecimalPlacesChangedActions { get; private set; }

        /// <summary>
        /// Called when the DecimalPlaces property value has changed.
        /// </summary>
        /// <param name="oldValue">
        /// Old value of the DecimalPlaces property.
        /// </param>
        /// <param name="newValue">
        /// New value of the DecimalPlaces property.
        /// </param>
        protected override void OnDecimalPlacesChanged(int oldValue, int newValue)
        {
            DecimalPlacesChangedActions.DoPreTest(oldValue, newValue);
            base.OnDecimalPlacesChanged(oldValue, newValue);
            DecimalPlacesChangedActions.DoTest(oldValue, newValue);
        }
        
        /// <summary>
        /// Gets the OnValueChanged test actions.
        /// </summary>
        public OverriddenMethod<RoutedPropertyChangedEventArgs<double>> ValueChangedActions { get; private set; }

        /// <summary>
        /// Override UpDownBase&lt;T&gt;.OnValueChanged to raise value changed
        /// automation event and determine if a maximum or minimum has been
        /// reached.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnValueChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            ValueChangedActions.DoPreTest(e);
            base.OnValueChanged(e);
            ValueChangedActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnValueChanging test actions.
        /// </summary>
        public OverriddenMethod<RoutedPropertyChangingEventArgs<double>> ValueChangingActions { get; private set; }

        /// <summary>
        /// Override UpDownBase&lt;T&gt;.OnValueChanging to do validation and
        /// coercion.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnValueChanging(RoutedPropertyChangingEventArgs<double> e)
        {
            ValueChangingActions.DoPreTest(e);
            base.OnValueChanging(e);
            ValueChangingActions.DoTest(e);
        }

        /// <summary>
        /// Gets the OnIsEditableChanged test actions.
        /// </summary>
        public OverriddenMethod<bool, bool> IsEditableChangedActions { get; private set; }

        /// <summary>
        /// Called when IsEditable property value changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnIsEditableChanged(bool oldValue, bool newValue)
        {
            IsEditableChangedActions.DoPreTest(oldValue, newValue);
            base.OnIsEditableChanged(oldValue, newValue);
            IsEditableChangedActions.DoTest(oldValue, newValue);
        }
        
        /// <summary>
        /// Gets the OnParseError test actions.
        /// </summary>
        public OverriddenMethod<UpDownParseErrorEventArgs> ParseErrorActions { get; private set; }

        /// <summary>
        /// Raises the ParserError event when there is an error in parsing user
        /// input.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnParseError(UpDownParseErrorEventArgs e)
        {
            ParseErrorActions.DoPreTest(e);
            base.OnParseError(e);
            ParseErrorActions.DoTest(e);
        }
        
        /// <summary>
        /// Gets the OnSpin test actions.
        /// </summary>
        public OverriddenMethod<SpinEventArgs> SpinActions { get; private set; }

        /// <summary>
        /// Occurs when the spinner spins.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnSpin(SpinEventArgs e)
        {
            SpinActions.DoPreTest(e);
            base.OnSpin(e);
            SpinActions.DoTest(e);
        }
        
        /// <summary>
        /// Gets the ApplyValue test actions.
        /// </summary>
        public OverriddenMethod<string> ApplyValueActions { get; private set; }

        /// <summary>
        /// Processes user input when the TextBox.TextChanged event occurs.
        /// </summary>
        /// <param name="text">User input.</param>
        protected override void ApplyValue(string text)
        {
            ApplyValueActions.DoPreTest(text);
            base.ApplyValue(text);
            ApplyValueActions.DoTest(text);
        }
        
        /// <summary>
        /// Gets the FormatValue test actions.
        /// </summary>
        public OverriddenMethod<string> FormatValueActions { get; private set; }

        /// <summary>
        /// Provides decimal specific value formatting for the value property.
        /// </summary>
        /// <returns>Formatted Value.</returns>
        protected override string FormatValue()
        {
            FormatValueActions.DoPreTest(null);
            string value = base.FormatValue();
            FormatValueActions.DoTest(value);
            return value;
        }
        
        /// <summary>
        /// Gets the OnIncrement test actions.
        /// </summary>
        public OverriddenMethod IncrementActions { get; private set; }

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Increase.
        /// </summary>
        protected override void OnIncrement()
        {
            IncrementActions.DoPreTest();
            base.OnIncrement();
            IncrementActions.DoTest();
        }

        /// <summary>
        /// Perform an increment.
        /// </summary>
        public void DoIncrement()
        {
            OnIncrement();
        }
        
        /// <summary>
        /// Gets the OnDecrement test actions.
        /// </summary>
        public OverriddenMethod DecrementActions { get; private set; }

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Decrease.
        /// </summary>
        protected override void OnDecrement()
        {
            DecrementActions.DoPreTest();
            base.OnDecrement();
            DecrementActions.DoTest();
        }

        /// <summary>
        /// Perform an increment.
        /// </summary>
        public void DoDecrement()
        {
            OnDecrement();
        }
        
        /// <summary>
        /// Gets the OnSpinnerStyleChanged test actions.
        /// </summary>
        public OverriddenMethod<Style, Style> SpinnerStyleChangedActions { get; private set; }

        /// <summary>
        /// Called when SpinnerStyle property value has changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnSpinnerStyleChanged(Style oldValue, Style newValue)
        {
            SpinnerStyleChangedActions.DoPreTest(oldValue, newValue);
            base.OnSpinnerStyleChanged(oldValue, newValue);
            SpinnerStyleChangedActions.DoTest(oldValue, newValue);
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
        /// Gets test actions for the OnApplyTemplate method.
        /// </summary>
        public OverriddenMethod ApplyTemplateActions { get; private set; }

        /// <summary>
        /// Apply a control template to the TreeView.
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
        /// Defines an AutomationPeer for the TreeView control.
        /// </summary>
        /// <returns>
        /// A TreeViewAutomationPeer for the TreeView control.
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
        /// Measure the TreeView.
        /// </summary>
        /// <param name="availableSize">Size available for the TreeView.</param>
        /// <returns>Desired size of the TreeView.</returns>
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
        /// Arrange the TreeView.
        /// </summary>
        /// <param name="finalSize">Final size for the TreeView.</param>
        /// <returns>Final size used by the TreeView.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeActions.DoPreTest(finalSize, null);
            Size used = base.ArrangeOverride(finalSize);
            ArrangeActions.DoTest(finalSize, used);
            return used;
        }
    }
}
