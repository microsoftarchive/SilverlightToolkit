// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Represents a switch that can be toggled between two states.
    /// </summary>
    [TemplateVisualState(Name = NormalState, GroupName = CommonStates)]
    [TemplateVisualState(Name = DisabledState, GroupName = CommonStates)]
    [TemplatePart(Name = SwitchPart, Type = typeof(ToggleButton))]
    public class ToggleSwitch : ContentControl
    {
        /// <summary>
        /// Common visual states.
        /// </summary>
        private const string CommonStates = "CommonStates";

        /// <summary>
        /// Normal visual state.
        /// </summary>
        private const string NormalState = "Normal";

        /// <summary>
        /// Disabled visual state.
        /// </summary>
        private const string DisabledState = "Disabled";

        /// <summary>
        /// The ToggleButton that drives this.
        /// </summary>
        private const string SwitchPart = "Switch";

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(ToggleSwitch), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the HeaderTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ToggleSwitch), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the header template.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Gets and sets the IsChecked property.
        /// </summary>
        [TypeConverter(typeof(NullableBoolConverter))]
        public bool? IsChecked
        {
            get { return (bool?)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// The IsChecked DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool?), typeof(ToggleSwitch), new PropertyMetadata(false, OnIsCheckedChanged));

        private static void OnIsCheckedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ToggleSwitch sender = (ToggleSwitch)obj;
            if (sender.Switch != null)
            {
                sender.Switch.IsChecked = (bool?)e.NewValue;
            }
        }

        /// <summary>
        /// The Checked event handler. Will be raised when the switch is checked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Checked;

        /// <summary>
        /// The Unchecked event handler. Will be raised when the switch is unchecked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Unchecked;

        /// <summary>
        /// The Switch part.
        /// </summary>
        private ToggleButton _switch;

        /// <summary>
        /// Whether the content was set.
        /// </summary>
        private bool _wasContentSet;

        /// <summary>
        /// Initializes a new instance of the ToggleSwitch class.
        /// </summary>
        public ToggleSwitch()
        {
            DefaultStyleKey = typeof(ToggleSwitch);
            Loaded += LoadedHandler;
        }

        /// <summary>
        /// Makes the content an "Off" or "On" string to match the state.
        /// </summary>
        private void SetDefaultContent()
        {
            Binding binding = new Binding("IsChecked") { Source = this, Converter = new OffOnConverter() };
            SetBinding(ContentProperty, binding);
        }

        /// <summary>
        /// Change the visual state.
        /// </summary>
        /// <param name="useTransitions">Indicates whether to use animation transitions.</param>
        private void ChangeVisualState(bool useTransitions)
        {
            if (IsEnabled)
            {
                VisualStateManager.GoToState(this, NormalState, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, DisabledState, useTransitions);
            }
        }

        /// <summary>
        /// Makes the content an "Off" or "On" string to match the state if the content is set to null in the design tool.
        /// </summary>
        /// <param name="oldContent">The old content.</param>
        /// <param name="newContent">The new content.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            _wasContentSet = true;
            if (DesignerProperties.IsInDesignTool && newContent == null && GetBindingExpression(ContentProperty) == null)
            {
                SetDefaultContent();
            }
        }

        /// <summary>
        /// Gets all the template parts and initializes the corresponding state.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Switch = (GetTemplateChild(SwitchPart) as ToggleButton) ?? new ToggleButton();
            Switch.IsChecked = IsChecked;

            ChangeVisualState(false);
        }

        /// <summary>
        /// Handles the loading of this control.
        /// Sets the content if it is null when the control is loaded.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void LoadedHandler(object sender, RoutedEventArgs e)
        {
            if (!_wasContentSet && GetBindingExpression(ContentProperty) == null)
            {
                SetDefaultContent();
            }
        }

        /// <summary>
        /// Gets or sets the toggle switch.
        /// </summary>
        private ToggleButton Switch
        {
            get { return _switch; }
            set
            {
                if (_switch != null)
                {
                    _switch.Checked -= Switch_Checked;
                    _switch.Unchecked -= Switch_Unchecked;
                }
                _switch = value;
                if (_switch != null)
                {
                    _switch.Checked += Switch_Checked;
                    _switch.Unchecked += Switch_Unchecked;
                }
            }
        }

        /// <summary>
        /// Checks the state when the toggle switch is checked and simulates the event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void Switch_Checked(object sender, RoutedEventArgs e)
        {
            IsChecked = true;
            SafeRaise.Raise(Checked, this, e);
        }

        /// <summary>
        /// Unchecks the state when the toggle switch is unchecked and simulates the event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void Switch_Unchecked(object sender, RoutedEventArgs e)
        {
            IsChecked = false;
            SafeRaise.Raise(Unchecked, this, e);
        }
    }
}