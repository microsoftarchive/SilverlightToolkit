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

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Represents a spinner control that includes two Buttons.
    /// </summary>
    /// <remarks>
    /// ButtonSpinner inherits from Spinner. 
    /// It adds two button template parts and a content property.
    /// </remarks>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]

    [TemplateVisualState(Name = "Focused", GroupName = "FocusedStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusedStates")]
    
    [TemplatePart(Name = "IncreaseButton", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "DecreaseButton", Type = typeof(ButtonBase))]
    
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
            Interaction = new InteractionHelper(this);
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

        #region visual state management
        /// <summary>
        /// Gets or sets the helper that provides all of the standard
        /// interaction functionality. Making it internal for subclass access.
        /// </summary>
        internal InteractionHelper Interaction { get; set; }

        /// <summary>
        /// Update current visual state.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal virtual void UpdateVisualState(bool useTransitions)
        {
            // Handle the Common and Focused states
            Interaction.UpdateVisualStateBase(useTransitions);
        }
        #endregion
    }
}
