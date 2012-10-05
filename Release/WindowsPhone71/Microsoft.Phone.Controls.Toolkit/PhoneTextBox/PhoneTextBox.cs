// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// An extended TextBox for Windows Phone which implements hint text, an action icon, and a 
    /// length indicator.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    [TemplateVisualState(Name = LengthIndicatorVisibleState, GroupName = LengthIndicatorStates)]
    [TemplateVisualState(Name = LengthIndicatorHiddenState, GroupName = LengthIndicatorStates)]
    [TemplateVisualState(Name = NormalState, GroupName = CommonStates)]
    [TemplateVisualState(Name = DisabledState, GroupName = CommonStates)]
    [TemplateVisualState(Name = ReadOnlyState, GroupName = CommonStates)]
    [TemplateVisualState(Name = FocusedState, GroupName = FocusStates)]
    [TemplateVisualState(Name = UnfocusedState, GroupName = FocusStates)]
    [TemplatePart(Name = TextBoxName, Type = typeof(TextBox))]
    [TemplatePart(Name = HintContentName, Type = typeof(ContentControl))]
    [TemplatePart(Name = LengthIndicatorName, Type = typeof(TextBlock))]
    public class PhoneTextBox : TextBox
    {

        #region PhoneTextBox Properties & Variables
        private Grid RootGrid;
        private TextBox TextBox;
        private TextBlock MeasurementTextBlock; // Used to measure the height of the TextBox to determine if the action icon is being overlapped.

        private Brush ForegroundBrushInactive = (Brush)Application.Current.Resources["PhoneTextBoxReadOnlyBrush"];

        private Brush ForegroundBrushEdit;

        // Hint Private Variables.
        private ContentControl HintContent;
        private Border HintBorder;

        // Length Indicator Private Variables.
        private TextBlock LengthIndicator;

        // Action Icon Private Variables.
        private Border ActionIconBorder;

        // Ignore flags for the dependency properties.
        private bool _ignorePropertyChange = false;

        //Temporarily ignore focus?
        private bool _ignoreFocus = false;

        #endregion

        #region Constants
        /// <summary>
        /// Root grid.
        /// </summary>
        private const string RootGridName = "RootGrid";

        /// <summary>
        /// Main text box.
        /// </summary>
        private const string TextBoxName = "Text";

        /// <summary>
        /// Hint Content.
        /// </summary>
        private const string HintContentName = "HintContent";

        /// <summary>
        /// Hint border.
        /// </summary>
        private const string HintBorderName = "HintBorder";

        /// <summary>
        /// Length indicator name.
        /// </summary>
        private const string LengthIndicatorName = "LengthIndicator";

        /// <summary>
        /// Action icon canvas.
        /// </summary>
        private const string ActionIconCanvasName = "ActionIconCanvas";

        /// <summary>
        /// Measurement Text Block name.
        /// </summary>
        private const string MeasurementTextBlockName = "MeasurementTextBlock";

        /// <summary>
        /// Action icon image name.
        /// </summary>
        private const string ActionIconBorderName = "ActionIconBorder";
        #endregion

        #region Visual States
        /// <summary>
        /// Length indicator states.
        /// </summary>
        private const string LengthIndicatorStates = "LengthIndicatorStates";

        /// <summary>
        /// Length indicator visible visual state.
        /// </summary>
        private const string LengthIndicatorVisibleState = "LengthIndicatorVisible";

        /// <summary>
        /// Length indicator hidden visual state.
        /// </summary>
        private const string LengthIndicatorHiddenState = "LengthIndicatorHidden";

        /// <summary>
        /// Common States.
        /// </summary>
        private const string CommonStates = "CommonStates";

        /// <summary>
        /// Normal state.
        /// </summary>
        private const string NormalState = "Normal";

        /// <summary>
        /// Disabled state.
        /// </summary>
        private const string DisabledState = "Disabled";

        /// <summary>
        /// ReadOnly state.
        /// </summary>
        private const string ReadOnlyState = "ReadOnly";

        /// <summary>
        /// Focus states.
        /// </summary>
        private const string FocusStates = "FocusStates";

        /// <summary>
        /// Focused state.
        /// </summary>
        private const string FocusedState = "Focused";

        /// <summary>
        /// Unfocused state.
        /// </summary>
        private const string UnfocusedState = "Unfocused";
        #endregion

        #region Hint
        /// <summary>
        /// Identifies the Hint DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HintProperty =
            DependencyProperty.Register("Hint", typeof(string), typeof(PhoneTextBox), new PropertyMetadata(
                    new PropertyChangedCallback(OnHintPropertyChanged)
                )
            );

        /// <summary>
        /// Gets or sets the Hint
        /// </summary>
        public string Hint
        {
            get { return base.GetValue(HintProperty) as string; }
            set { base.SetValue(HintProperty, value); }
        }

        /// <summary>
        /// Identifies the HintStyle DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HintStyleProperty =
            DependencyProperty.Register("HintStyle", typeof(Style), typeof(PhoneTextBox), null);

        /// <summary>
        /// Gets or sets the Hint style
        /// </summary>
        public Style HintStyle
        {
            get { return base.GetValue(HintStyleProperty) as Style; }
            set { base.SetValue(HintStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the HintVisibility DependencyProperty
        /// </summary>
        public static readonly DependencyProperty ActualHintVisibilityProperty =
            DependencyProperty.Register("ActualHintVisibility", typeof(Visibility), typeof(PhoneTextBox), null);

        /// <summary>
        /// Gets or sets whether the hint is actually visible.
        /// </summary>
        public Visibility ActualHintVisibility
        {
            get { return (Visibility)base.GetValue(ActualHintVisibilityProperty); }
            set { base.SetValue(ActualHintVisibilityProperty, value); }
        }


        /// <summary>
        /// When the Hint is changed, check if it needs to be hidden or shown.
        /// </summary>
        /// <param name="sender">Sending PhoneTextBox.</param>
        /// <param name="args">DependencyPropertyChangedEvent Arguments.</param>
        private static void OnHintPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            PhoneTextBox phoneTextBox = sender as PhoneTextBox;

            if (phoneTextBox != null && phoneTextBox.HintContent != null)
            {
                phoneTextBox.UpdateHintVisibility();
            }
        }


        /// <summary>
        /// Determines if the Hint should be shown or not based on if there is content in the TextBox.
        /// </summary>
        private void UpdateHintVisibility()
        {
            if (HintContent != null)
            {
                if (string.IsNullOrEmpty(Text))
                {
                    ActualHintVisibility = Visibility.Visible;
                    Foreground = ForegroundBrushInactive;
                }
                else
                {
                    ActualHintVisibility = Visibility.Collapsed;
                    Foreground = ForegroundBrushEdit;
                }
            }
        }

        /// <summary>
        /// Override the Blur/LostFocus event to show the Hint if needed.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            UpdateHintVisibility();
            base.OnLostFocus(e);
        }

        /// <summary>
        /// Override the Focus event to hide the Hint.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (_ignoreFocus)
            {
                _ignoreFocus = false;

                var rootFrame = Application.Current.RootVisual as Frame;
                rootFrame.Focus();

                return;
            }

            Foreground = ForegroundBrushEdit;

            if (HintContent != null)
            {
                ActualHintVisibility = Visibility.Collapsed;
            }

            base.OnGotFocus(e);
        }

        #endregion

        #region Length Indicator
        /// <summary>
        /// Length Indicator Visibile Dependency Property.
        /// </summary>
        public static readonly DependencyProperty LengthIndicatorVisibleProperty =
            DependencyProperty.Register("LengthIndicatorVisible", typeof(Boolean), typeof(PhoneTextBox), null);

        /// <summary>
        /// Boolean that determines if the length indicator should be visible.
        /// </summary>
        public Boolean LengthIndicatorVisible
        {
            get { return (bool)base.GetValue(LengthIndicatorVisibleProperty); }
            set { base.SetValue(LengthIndicatorVisibleProperty, value); }
        }

        /// <summary>
        /// Length Indicator Visibility Threshold Dependency Property.
        /// </summary>
        public static readonly DependencyProperty LengthIndicatorThresholdProperty =
            DependencyProperty.Register("LengthIndicatorThreshold", typeof(int), typeof(PhoneTextBox), new PropertyMetadata(
                new PropertyChangedCallback(OnLengthIndicatorThresholdChanged)
               )
            );

        /// <summary>
        /// Threshold after which the length indicator will appear.
        /// </summary>
        public int LengthIndicatorThreshold
        {
            get { return (int)base.GetValue(LengthIndicatorThresholdProperty); }
            set { base.SetValue(LengthIndicatorThresholdProperty, value); }
        }

        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The property name is the appropriate value to throw.")]
        private static void OnLengthIndicatorThresholdChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            PhoneTextBox phoneTextBox = sender as PhoneTextBox;

            if (phoneTextBox._ignorePropertyChange)
            {
                phoneTextBox._ignorePropertyChange = false;
                return;
            }

            if (phoneTextBox.LengthIndicatorThreshold < 0)
            {
                phoneTextBox._ignorePropertyChange = true;
                phoneTextBox.SetValue(LengthIndicatorThresholdProperty, args.OldValue);

                throw new ArgumentOutOfRangeException("LengthIndicatorThreshold", "The length indicator visibility threshold must be greater than zero.");
            }

        }

        /// <summary>
        /// The displayed maximum length of text that can be entered. This value takes
        /// priority over the MaxLength property in the Length Indicator display.
        /// </summary>
        public static readonly DependencyProperty DisplayedMaxLengthProperty =
            DependencyProperty.Register("DisplayedMaxLength", typeof(int), typeof(PhoneTextBox), new PropertyMetadata(
                new PropertyChangedCallback(DisplayedMaxLengthChanged)
              )  
             );


        /// <summary>
        /// The displayed value for the maximum length of the input.
        /// </summary>
        public int DisplayedMaxLength
        {
            get { return (int)base.GetValue(DisplayedMaxLengthProperty); }
            set { base.SetValue(DisplayedMaxLengthProperty, value); }
        }

        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "The property name is the appropriate value to throw.")]
        private static void DisplayedMaxLengthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            PhoneTextBox phoneTextBox = sender as PhoneTextBox;

            if (phoneTextBox.DisplayedMaxLength > phoneTextBox.MaxLength && phoneTextBox.MaxLength > 0)
            {
                throw new ArgumentOutOfRangeException("DisplayedMaxLength", "The displayed maximum length cannot be greater than the MaxLength.");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Globalization", 
            "CA1303:Do not pass literals as localized parameters", 
            MessageId = "System.Windows.Controls.TextBlock.set_Text(System.String)",
            Justification = "At this time the length indicator is not culture-specific or retrieved from the resources.")]
        private void UpdateLengthIndicatorVisibility()
        {
            if (RootGrid == null || LengthIndicator == null)
            {
                return;
            }

            bool isHidden = true;
            if (LengthIndicatorVisible)
            {
                // The current implementation is culture invariant.
                LengthIndicator.Text = String.Format(
                    CultureInfo.InvariantCulture,
                    "{0}/{1}", Text.Length, 
                    ((DisplayedMaxLength > 0) ? DisplayedMaxLength : MaxLength));

                if (Text.Length >= LengthIndicatorThreshold)
                {
                    isHidden = false;
                }
            }

            VisualStateManager.GoToState(this, isHidden ? LengthIndicatorHiddenState : LengthIndicatorVisibleState, false);
        }

        #endregion

        #region Action Icon
        /// <summary>
        /// Identifies the ActionIcon DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty ActionIconProperty =
            DependencyProperty.Register("ActionIcon", typeof(ImageSource), typeof(PhoneTextBox), new PropertyMetadata(
                new PropertyChangedCallback(OnActionIconChanged)
               )
            );

        /// <summary>
        /// Gets or sets the ActionIcon.
        /// </summary>
        public ImageSource ActionIcon
        {
            get { return base.GetValue(ActionIconProperty) as ImageSource; }
            set { base.SetValue(ActionIconProperty, value); }
        }


        /// <summary>
        /// Gets or sets whether the ActionItem is hidden when there is not text entered in the PhoneTextBox.
        /// </summary>
        public bool HidesActionItemWhenEmpty
        {
            get { return (bool)GetValue(HidesActionItemWhenEmptyProperty); }
            set { SetValue(HidesActionItemWhenEmptyProperty, value); }
        }

        /// <summary>
        /// Identifies the HidesActionItemWhenEmpty DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty HidesActionItemWhenEmptyProperty =
            DependencyProperty.Register("HidesActionItemWhenEmpty", typeof(bool), typeof(PhoneTextBox), new PropertyMetadata(false, OnActionIconChanged));

        

        /// <summary>
        /// Action Icon Tapped event.
        /// </summary>
        public event EventHandler ActionIconTapped;

        private static void OnActionIconChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            PhoneTextBox phoneTextBox = sender as PhoneTextBox;

            if (phoneTextBox != null)
            {
                phoneTextBox.UpdateActionIconVisibility();
            }
        }

        private void UpdateActionIconVisibility()
        {
            if (ActionIconBorder != null)
            {
                if (ActionIcon == null || (HidesActionItemWhenEmpty && string.IsNullOrEmpty(Text)))
                {
                    ActionIconBorder.Visibility = Visibility.Collapsed;
                    HintBorder.Padding = new Thickness(0);
                }
                else
                {
                    ActionIconBorder.Visibility = Visibility.Visible;

                    if (TextWrapping != System.Windows.TextWrapping.Wrap)
                    {
                        HintBorder.Padding = new Thickness(0, 0, 48, 0);
                    }
                }
            }
        }

        /// <summary>
        /// Determines if the developer set an event for ActionIconTapped.
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The RoutedEventArgs for the event</param>
        private void OnActionIconTapped(object sender, RoutedEventArgs e)
        {
            _ignoreFocus = true;

            var handler = ActionIconTapped;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void ResizeTextBox()
        {
            if (ActionIcon == null || TextWrapping != System.Windows.TextWrapping.Wrap) { return; }

            MeasurementTextBlock.Width = ActualWidth;

            if (MeasurementTextBlock.ActualHeight > ActualHeight - 72)
            {
                Height = ActualHeight + 72;
            }
            else if (ActualHeight > MeasurementTextBlock.ActualHeight + 144)
            {
                Height = ActualHeight - 72;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the PhoneTextBox class.
        /// </summary>
        public PhoneTextBox()
        {
            DefaultStyleKey = typeof(PhoneTextBox);
        }

        /// <summary>
        /// Applies the template and checks to see if the Hint should be shown
        /// when the page is first loaded.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (TextBox != null)
            {
                TextBox.TextChanged -= OnTextChanged;
            }

            if (ActionIconBorder != null)
            {
                ActionIconBorder.MouseLeftButtonDown -= OnActionIconTapped;
            }

            RootGrid = GetTemplateChild(RootGridName) as Grid;
            TextBox = GetTemplateChild(TextBoxName) as TextBox;
            
            // Getting the foreground color to save for later.
            ForegroundBrushEdit = Foreground;

            // Getting template children for the hint text.
            HintContent = GetTemplateChild(HintContentName) as ContentControl;
            HintBorder = GetTemplateChild(HintBorderName) as Border;

            if (HintContent != null)
            {
                UpdateHintVisibility();
            }
            
            // Getting template children for the length indicator.
            LengthIndicator = GetTemplateChild(LengthIndicatorName) as TextBlock;
            
            // Getting template child for the action icon
            ActionIconBorder = GetTemplateChild(ActionIconBorderName) as Border;

            if (RootGrid != null && LengthIndicator != null)
            {
                UpdateLengthIndicatorVisibility();
            }

            if (TextBox != null)
            {
                TextBox.TextChanged += OnTextChanged;
            }

            if (ActionIconBorder != null)
            {
                ActionIconBorder.MouseLeftButtonDown += OnActionIconTapped;
                UpdateActionIconVisibility(); // Add back the padding if needed.
            }

            
            // Get template child for the action icon measurement text block.
            MeasurementTextBlock = GetTemplateChild(MeasurementTextBlockName) as TextBlock;
        }

        /// <summary>
        /// Called when the selection changed event occurs. This determines whether the length indicator should be shown or
        /// not and if the TextBox needs to grow.
        /// </summary>
        /// <param name="sender">Sender TextBox</param>
        /// <param name="e">Event arguments</param>
        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            UpdateLengthIndicatorVisibility();
            UpdateActionIconVisibility();
            UpdateHintVisibility();
            ResizeTextBox();
        }
    }
}