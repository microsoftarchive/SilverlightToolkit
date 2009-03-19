// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a spinner control that includes two Buttons.
    /// </summary>
    /// <remarks>
    /// ButtonSpinner inherits from Spinner. 
    /// It adds two button template parts and a content property.
    /// </remarks>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]

    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplateVisualState(Name = VisualStates.StateIncreaseEnabled, GroupName = VisualStates.GroupIncrease)]
    [TemplateVisualState(Name = VisualStates.StateIncreaseDisabled, GroupName = VisualStates.GroupIncrease)]

    [TemplateVisualState(Name = VisualStates.StateDecreaseEnabled, GroupName = VisualStates.GroupDecrease)]
    [TemplateVisualState(Name = VisualStates.StateDecreaseDisabled, GroupName = VisualStates.GroupDecrease)]

    [TemplatePart(Name = ButtonSpinner.ElementIncreaseButtonName, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ButtonSpinner.ElementDecreaseButtonName, Type = typeof(ButtonBase))]
    
    [ContentProperty("Content")]
    public partial class ButtonSpinner : Spinner
    {
        #region template parts
        /// <summary>
        /// Name constant of the IncreaseButton template part.
        /// </summary>
        private const string ElementIncreaseButtonName = "IncreaseButton";

        /// <summary>
        /// Name constant of the DecreaseButton template part.
        /// </summary>
        private const string ElementDecreaseButtonName = "DecreaseButton";

        /// <summary>
        /// Private field for IncreaseButton template part.
        /// </summary>
        private ButtonBase _increaseButton;

        /// <summary>
        /// Gets or sets the IncreaseButton template part.
        /// </summary>
        private ButtonBase IncreaseButton
        {
            get { return _increaseButton; }
            set
            {
                if (_increaseButton != null)
                {
                    _increaseButton.Click -= OnButtonClick;
                }

                _increaseButton = value;

                if (_increaseButton != null)
                {
                    _increaseButton.Click += OnButtonClick;
                }
            }
        }

        /// <summary>
        /// Private field for DecreaseButton template part.
        /// </summary>
        private ButtonBase _decreaseButton;

        /// <summary>
        /// Gets or sets the DecreaseButton template part.
        /// </summary>
        private ButtonBase DecreaseButton
        {
            get { return _decreaseButton; }
            set
            {
                if (_decreaseButton != null)
                {
                    _decreaseButton.Click -= OnButtonClick;
                }

                _decreaseButton = value;

                if (_decreaseButton != null)
                {
                    _decreaseButton.Click += OnButtonClick;
                }
            }
        }
        #endregion 

        #region public object Content
        /// <summary>
        /// Gets or sets the content that is contained within the button spinner.
        /// </summary>
        public object Content
        {
            get { return GetValue(ContentProperty) as object; }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Identifies the Content dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                "Content",
                typeof(object),
                typeof(ButtonSpinner),
                new PropertyMetadata(null, OnContentPropertyChanged));

        /// <summary>
        /// ContentProperty property changed handler.
        /// </summary>
        /// <param name="d">ButtonSpinner that changed its Content.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ButtonSpinner source = d as ButtonSpinner;
            source.OnContentChanged(e.OldValue, e.NewValue);
        }
        #endregion public object Content

        /// <summary>
        /// Initializes a new instance of the ButtonSpinner class.
        /// </summary>
        public ButtonSpinner() : base()
        {
            DefaultStyleKey = typeof(ButtonSpinner);
        }

        /// <summary>
        /// Builds the visual tree for the ButtonSpinner control when a new 
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            IncreaseButton = GetTemplateChild(ElementIncreaseButtonName) as ButtonBase;
            DecreaseButton = GetTemplateChild(ElementDecreaseButtonName) as ButtonBase;
            Interaction.OnApplyTemplateBase();

            UpdateVisualState(false);

            SetButtonUsage();
        }

        /// <summary>
        /// Occurs when the Content property value changed.
        /// </summary>
        /// <param name="oldValue">The old value of the Content property.</param>
        /// <param name="newValue">The new value of the Content property.</param>
        protected virtual void OnContentChanged(object oldValue, object newValue)
        {
        }

        /// <summary>
        /// Handle click event of IncreaseButton and DecreaseButton template parts,
        /// translating Click to appropriate Spin event.
        /// </summary>
        /// <param name="sender">Event sender, should be either IncreaseButton or DecreaseButton template part.</param>
        /// <param name="e">Event args.</param>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Debug.Assert(
                sender == IncreaseButton || sender == DecreaseButton,
                "This can't happen: OnButtonClick is called on neither IncreaseButton nor DecreaseButton!");

            SpinDirection direction = sender == IncreaseButton ? SpinDirection.Increase : SpinDirection.Decrease;

            OnSpin(new SpinEventArgs(direction));
        }

        /// <summary>
        /// Called when valid spin direction changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnValidSpinDirectionChanged(ValidSpinDirections oldValue, ValidSpinDirections newValue)
        {
            base.OnValidSpinDirectionChanged(oldValue, newValue);

            SetButtonUsage();
        }

        /// <summary>
        /// Disables or enables the buttons based on the valid spin direction.
        /// </summary>
        private void SetButtonUsage()
        {
            // buttonspinner adds buttons that spin, so disable accordingly.
            if (IncreaseButton != null)
            {
                IncreaseButton.IsEnabled = ((ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase);
            }

            if (DecreaseButton != null)
            {
                DecreaseButton.IsEnabled = ((ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease);
            }
        }
    }
}