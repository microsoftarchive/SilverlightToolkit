// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

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
        /// Identifies the Header DependencyProperty.
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
        /// Identifies the HeaderTemplate DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ToggleSwitch), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the template used to display the control's header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the SwitchForeground DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty SwitchForegroundProperty =
            DependencyProperty.Register("SwitchForeground", typeof(Brush), typeof(ToggleSwitch), null);

        /// <summary>
        /// Gets or sets the switch foreground.
        /// </summary>
        public Brush SwitchForeground
        {
            get { return (Brush)GetValue(SwitchForegroundProperty); }
            set { SetValue(SwitchForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the ToggleSwitch is checked.
        /// </summary>
        [TypeConverter(typeof(NullableBoolConverter))]
        public bool? IsChecked
        {
            get { return (bool?)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// Identifies the IsChecked DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool?), typeof(ToggleSwitch), new PropertyMetadata(false, OnIsCheckedChanged));

        /// <summary>
        /// Invoked when the IsChecked DependencyProperty is changed.
        /// </summary>
        /// <param name="d">The event sender.</param>
        /// <param name="e">The event information.</param>
        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)d;
            if (toggleSwitch._toggleButton != null)
            {
                toggleSwitch._toggleButton.IsChecked = (bool?)e.NewValue;
            }
        }

        /// <summary>
        /// Occurs when the
        /// <see cref="T:Microsoft.Phone.Controls.ToggleSwitch"/>
        /// is checked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Checked;

        /// <summary>
        /// Occurs when the
        /// <see cref="T:Microsoft.Phone.Controls.ToggleSwitch"/>
        /// is unchecked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Unchecked;

        /// <summary>
        /// Occurs when the
        /// <see cref="T:Microsoft.Phone.Controls.ToggleSwitch"/>
        /// is indeterminate.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Indeterminate;

        /// <summary>
        /// Occurs when the
        /// <see cref="System.Windows.Controls.Primitives.ToggleButton"/>
        /// is clicked.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Click;

        /// <summary>
        /// The
        /// <see cref="System.Windows.Controls.Primitives.ToggleButton"/>
        /// template part.
        /// </summary>
        private ToggleButton _toggleButton;

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

            if (!_wasContentSet && GetBindingExpression(ContentProperty) == null)
            {
                SetDefaultContent();
            }

            if (_toggleButton != null)
            {
                _toggleButton.Checked -= CheckedHandler;
                _toggleButton.Unchecked -= UncheckedHandler;
                _toggleButton.Indeterminate -= IndeterminateHandler;
                _toggleButton.Click -= ClickHandler;
            }
            _toggleButton = GetTemplateChild(SwitchPart) as ToggleButton;
            if (_toggleButton != null)
            {
                _toggleButton.Checked += CheckedHandler;
                _toggleButton.Unchecked += UncheckedHandler;
                _toggleButton.Indeterminate += IndeterminateHandler;
                _toggleButton.Click += ClickHandler;
                _toggleButton.IsChecked = IsChecked;
            }
            IsEnabledChanged += delegate
            {
                ChangeVisualState(true);
            };
            ChangeVisualState(false);
        }

        /// <summary>
        /// Mirrors the
        /// <see cref="E:System.Windows.Controls.Primitives.ToggleButton.Checked"/>
        /// event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void CheckedHandler(object sender, RoutedEventArgs e)
        {
            IsChecked = true;
            SafeRaise.Raise(Checked, this, e);
        }

        /// <summary>
        /// Mirrors the
        /// <see cref="E:System.Windows.Controls.Primitives.ToggleButton.Unchecked"/>
        /// event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void UncheckedHandler(object sender, RoutedEventArgs e)
        {
            IsChecked = false;
            SafeRaise.Raise(Unchecked, this, e);
        }

        /// <summary>
        /// Mirrors the
        /// <see cref="E:System.Windows.Controls.Primitives.ToggleButton.Indeterminate"/>
        /// event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void IndeterminateHandler(object sender, RoutedEventArgs e)
        {
            IsChecked = null;
            SafeRaise.Raise(Indeterminate, this, e);
        }

        /// <summary>
        /// Mirrors the 
        /// <see cref="E:System.Windows.Controls.Primitives.ToggleButton.Click"/>
        /// event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event information.</param>
        private void ClickHandler(object sender, RoutedEventArgs e)
        {
            SafeRaise.Raise(Click, this, e);
        }

        /// <summary>
        /// Returns a
        /// <see cref="T:System.String"/>
        /// that represents the current
        /// <see cref="T:System.Object"/>
        /// .
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{{ToggleSwitch IsChecked={0}, Content={1}}}",
                IsChecked,
                Content
            );
        }
    }
}