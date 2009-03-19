// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a control that uses a spinner and textbox to allow a user to 
    /// input time.
    /// </summary>
    /// <remarks>TimeInput supports only the following formatting characters:
    /// 'h', 'm', 's', 'H', 't'. All other characters are filtered out:
    /// 'd', 'f', 'F', 'g', 'K', 'M', 'y', 'z'.</remarks>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplateVisualState(Name = BalloonOpenedStateName, GroupName = GroupBalloonHint)]
    [TemplateVisualState(Name = BalloonClosedStateName, GroupName = GroupBalloonHint)]
    
    [TemplateVisualState(Name = ValidTimeStateName, GroupName = GroupTimeParsingStates)]
    [TemplateVisualState(Name = InvalidTimeStateName, GroupName = GroupTimeParsingStates)]
    [TemplateVisualState(Name = EmptyTimeStateName, GroupName = GroupTimeParsingStates)]

    [TemplatePart(Name = System.Windows.Controls.UpDownBase.ElementTextName, Type = typeof(TextBox))]
    [TemplatePart(Name = System.Windows.Controls.UpDownBase.ElementSpinnerName, Type = typeof(Spinner))]
    [StyleTypedProperty(Property = System.Windows.Controls.UpDownBase.SpinnerStyleName, StyleTargetType = typeof(ButtonSpinner))]
    public class TimeUpDown : UpDownBase<DateTime?>, IUpdateVisualState, ITimeInput
    {
        /// <summary>
        /// StringFormat used in the balloon hint.
        /// </summary>
        private const string WatermarkHintFormat = "<{0}>";

        #region Visual state names
        /// <summary>
        /// The group name "BalloonHintStates".
        /// </summary>
        internal const string GroupBalloonHint = "BalloonHintStates";

        /// <summary>
        /// The group name "ParsingStates".
        /// </summary>
        internal const string GroupTimeParsingStates = "ParsingStates";

        /// <summary>
        /// The state name "Opened" that indicates that the balloon is being
        /// shown.
        /// </summary>
        internal const string BalloonOpenedStateName = "BalloonHintOpened";

        /// <summary>
        /// The state name "Closed" that indicates that no balloon is being 
        /// shown.
        /// </summary>
        internal const string BalloonClosedStateName = "BalloonHintClosed";

        /// <summary>
        /// The state name "ValidTime" that indicates that currently the textbox
        /// text parses to a valid Time.
        /// </summary>
        internal const string ValidTimeStateName = "ValidTime";
        
        /// <summary>
        /// The state name "InvalidTime" that indicates that currently the textbox 
        /// text does not allow parsing.
        /// </summary>
        internal const string InvalidTimeStateName = "InvalidTime";
        
        /// <summary>
        /// The state name "EmptyTime" that indicates that currently the textbox
        /// text would parse to a Null.
        /// </summary>
        internal const string EmptyTimeStateName = "EmptyTime";
        #endregion
        
        /// <summary>
        /// Helper class that centralizes the coercion logic across all 
        /// TimeInput controls.
        /// </summary>
        private readonly TimeCoercionHelper _timeCoercionHelper;

        /// <summary>
        /// The text that was last parsed. Used in comparisons.
        /// </summary>
        private string _lastParsedText;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is showing a
        /// balloon visual.
        /// </summary>
        /// <value><c>True</c> if this instance is showing the balloon; otherwise, <c>false</c>.</value>
        private bool IsShowBalloon
        {
            get { return _isShowBalloon; }
            set
            {
                _isShowBalloon = value;
                UpdateVisualState(true);
            }
        }

        /// <summary>
        /// BackingField for IsShowBalloon.
        /// </summary>
        private bool _isShowBalloon;

        /// <summary>
        /// Gets the actual minimum. If a Minimum is set, use that, otherwise
        /// use the start of the day.
        /// </summary>        
        private DateTime ActualMinimum
        {
            get
            {
                return Minimum.HasValue ? 
                    (Value.HasValue ? Value.Value.Date.Add(Minimum.Value.TimeOfDay) : Minimum.Value) 
                    : (Value.HasValue ? Value.Value.Date : DateTime.Now.Date);
            }
        }

        /// <summary>
        /// Gets the actual maximum. If a Maximum is set, use that, otherwise
        /// use the end of the day.
        /// </summary>
        /// <value>The actual maximum.</value>
        private DateTime ActualMaximum
        {
            get
            {
                return Maximum.HasValue
                               ? (Value.HasValue ? Value.Value.Date.Add(Maximum.Value.TimeOfDay) : Maximum.Value)
                               : (Value.HasValue
                                          ? Value.Value.Date.Add(TimeSpan.FromDays(1).Subtract(TimeSpan.FromSeconds(1)))
                                          : DateTime.Now.Date.AddDays(1).Subtract(TimeSpan.FromSeconds(1)));
            }
        }

        /// <summary>
        /// Gets or sets the currently selected time.
        /// </summary>
        [TypeConverter(typeof(TimeTypeConverter))]
        public override DateTime? Value
        {
            get { return base.Value; }
            set { base.Value = value; }
        }

        /// <summary>
        /// A value indicating whether a dependency property change handler
        /// should ignore the next change notification.  This is used to reset
        /// the value of properties without performing any of the actions in
        /// their change handlers.
        /// </summary>
        private bool _ignoreValueChange;

        #region public DateTime? Minimum
        /// <summary>
        /// Gets or sets the minimum time considered valid by the control.
        /// </summary>
        /// <remarks>Setting the minimum property is applicable for the following 
        /// features: Parsing a new value from the textbox, spinning a new value 
        /// and programmatically specifying a value.</remarks>
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Minimum
        {
            get { return (DateTime?)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Identifies the Minimum dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum",
                typeof(DateTime?),
                typeof(TimeUpDown),
                new PropertyMetadata(null, OnMinimumPropertyChanged));

        /// <summary>
        /// MinimumProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Minimum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown source = (TimeUpDown)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;

            source._ignoreValueChange = true;
            source._timeCoercionHelper.ProcessMinimumChange(newValue);
            source._ignoreValueChange = false;
            source.OnMinimumChanged(oldValue, newValue);
            source.SetValidSpinDirection();
        }

        /// <summary>
        /// Called when the Minimum property value has changed.
        /// </summary>
        /// <param name="oldValue">Old value of the Minimum property.</param>
        /// <param name="newValue">New value of the Minimum property.</param>
        protected virtual void OnMinimumChanged(DateTime? oldValue, DateTime? newValue)
        {
        }
        #endregion public DateTime? Minimum

        #region public DateTime? Maximum
        /// <summary>
        /// Gets or sets the maximum time considered valid by the control.
        /// </summary>
        /// <remarks>Setting the Maximum property is applicable for the following 
        /// features: Parsing a new value from the textbox, spinning a new value 
        /// and programmatically specifying a value. </remarks>
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Maximum
        {
            get { return (DateTime?)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Identifies the Maximum dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum",
                typeof(DateTime?),
                typeof(TimeUpDown),
                new PropertyMetadata(null, OnMaximumPropertyChanged));

        /// <summary>
        /// MaximumProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Maximum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown source = (TimeUpDown)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;

            source._ignoreValueChange = true;
            source._timeCoercionHelper.ProcessMaximumChange(newValue);
            source._ignoreValueChange = false;
            source.OnMaximumChanged(oldValue, newValue);
            source.SetValidSpinDirection();
        }

        /// <summary>
        /// Called when the Maximum property value has changed.
        /// </summary>
        /// <param name="oldValue">Old value of the Maximum property.</param>
        /// <param name="newValue">New value of the Maximum property.</param>
        protected virtual void OnMaximumChanged(DateTime? oldValue, DateTime? newValue)
        {
        }
        #endregion public DateTime? Maximum

        #region public TimeParserCollection TimeParsers
        /// <summary>
        /// Gets or sets a collection of TimeParsers that are used when parsing 
        /// text to time.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Can be set from xaml.")]
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Can be set from xaml.")]
        public TimeParserCollection TimeParsers
        {
            get { return GetValue(TimeParsersProperty) as TimeParserCollection; }
            set { SetValue(TimeParsersProperty, value); }
        }

        /// <summary>
        /// Identifies the TimeParsers dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeParsersProperty =
            DependencyProperty.Register(
                "TimeParsers",
                typeof(TimeParserCollection),
                typeof(TimeUpDown),
                new PropertyMetadata(OnTimeParsersPropertyChanged));

        /// <summary>
        /// TimeParsersProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its TimeParsers.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeParsersPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets the actual TimeParsers that will be used for parsing by the control.
        /// </summary>
        /// <remarks>Includes the TimeParsers introduced in the TimeGlobalizationInfo.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Can be set from xaml.")]
        public TimeParserCollection ActualTimeParsers
        {
            get
            {
                return new TimeParserCollection(ActualTimeGlobalizationInfo.GetActualTimeParsers(TimeParsers == null ? null : TimeParsers.ToList()));
            }
        }        
        #endregion public TimeParserCollection TimeParsers

        #region public ITimeFormat Format
        /// <summary>
        /// Gets or sets the Format used by the control. 
        /// From XAML Use either "Short", "Long" or a custom format. 
        /// Custom formats can only contain "H", "h", "m", "s" or "t". 
        /// For example: use 'hh:mm:ss' is used to format time as "13:45:30".
        /// </summary>
        [TypeConverter(typeof(TimeFormatConverter))]
        public ITimeFormat Format
        {
            get { return GetValue(FormatProperty) as ITimeFormat; }
            set { SetValue(FormatProperty, value); }
        }

        /// <summary>
        /// Identifies the Format dependency property.
        /// </summary>
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register(
                "Format",
                typeof(ITimeFormat),
                typeof(TimeUpDown),
                new PropertyMetadata(OnFormatPropertyChanged));

        /// <summary>
        /// FormatProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its Format.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown source = (TimeUpDown)d;
            if (e.NewValue != null)
            {
                // no need for cache any more.
                source._actualFormat = null;
            }
            source.SetTextBoxText();
        }

        /// <summary>
        /// Gets the actual format that will be used to display Time  in the 
        /// TimeUpDown. If no format is specified, ShortTimeFormat is used.
        /// </summary>
        /// <value>The actual display format.</value>
        public ITimeFormat ActualFormat
        {
            get
            {
                if (Format == null)
                {
                    if (_actualFormat == null)
                    {
                        _actualFormat = new ShortTimeFormat();
                    }
                    return _actualFormat;
                }
                else
                {
                    return Format;
                }
            }
        }

        /// <summary>
        /// BackingField for ActualFormat.
        /// </summary>
        private ITimeFormat _actualFormat;
        #endregion public ITimeFormat Format

        #region public CultureInfo Culture
        /// <summary>
        /// Gets or sets the culture that will be used by the control for 
        /// parsing and formatting.
        /// </summary>
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }

        /// <summary>
        /// Identifies the Culture dependency property.
        /// </summary>
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register(
                "Culture",
                typeof(CultureInfo),
                typeof(TimeUpDown),
                new PropertyMetadata(null, OnCulturePropertyChanged));

        /// <summary>
        /// CultureProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Culture.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnCulturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown source = (TimeUpDown)d;
            source.ActualTimeGlobalizationInfo.Culture = e.NewValue as CultureInfo;
            source.SetTextBoxText();
        }
        #endregion public CultureInfo Culture

        /// <summary>
        /// Gets the actual culture used by the control for formatting and parsing.
        /// </summary>
        /// <value>The actual culture.</value>
        public CultureInfo ActualCulture
        {
            get
            {
                return ActualTimeGlobalizationInfo.ActualCulture;
            }
        }

        #region public TimeGlobalizationInfo TimeGlobalizationInfo
        /// <summary>
        /// Gets or sets the strategy object that determines how the control 
        /// interacts with DateTime and CultureInfo.
        /// </summary>
        public TimeGlobalizationInfo TimeGlobalizationInfo
        {
            get { return (TimeGlobalizationInfo)GetValue(TimeGlobalizationInfoProperty); }
            set { SetValue(TimeGlobalizationInfoProperty, value); }
        }

        /// <summary>
        /// Identifies the TimeGlobalizationInfo dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeGlobalizationInfoProperty =
            DependencyProperty.Register(
                "TimeGlobalizationInfo",
                typeof(TimeGlobalizationInfo),
                typeof(TimeUpDown),
                new PropertyMetadata(OnTimeGlobalizationInfoPropertyChanged));

        /// <summary>
        /// TimeGlobalizationInfoProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its TimeGlobalizationInfo.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeGlobalizationInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown source = (TimeUpDown)d;
            TimeGlobalizationInfo newValue = e.NewValue as TimeGlobalizationInfo;
            if (newValue != null)
            {
                newValue.Culture = source.Culture;
                source._actualTimeGlobalizationInfo = null; // no need for default any more.
            }
        }

        /// <summary>
        /// Gets the actual TimeGlobalization info used by the control.
        /// </summary>
        /// <remarks>If TimeGlobalizationInfo is not set, will return 
        /// default TimeGlobalizationInfo instance.</remarks>
        public TimeGlobalizationInfo ActualTimeGlobalizationInfo
        {
            get
            {
                TimeGlobalizationInfo info = TimeGlobalizationInfo;

                if (info == null)
                {
                    if (_actualTimeGlobalizationInfo == null)
                    {
                        // set the default strategy object
                        _actualTimeGlobalizationInfo = new TimeGlobalizationInfo();
                    }
                    return _actualTimeGlobalizationInfo;
                }
                else
                {
                    return info;
                }
            }
        }

        /// <summary>
        /// BackingField for ActualTimeGlobalizationInfo.
        /// </summary>
        private TimeGlobalizationInfo _actualTimeGlobalizationInfo;
        #endregion public TimeGlobalizationInfo TimeGlobalizationInfo

        #region public bool IsCyclic
        /// <summary>
        /// Gets or sets a value indicating whether the TimeUpDown control will 
        /// cycle through values when trying to spin the first and last item.
        /// </summary>
        public bool IsCyclic
        {
            get { return (bool)GetValue(IsCyclicProperty); }
            set { SetValue(IsCyclicProperty, value); }
        }

        /// <summary>
        /// Identifies the IsCyclic dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCyclicProperty =
            DependencyProperty.Register(
                "IsCyclic",
                typeof(bool),
                typeof(TimeUpDown),
                new PropertyMetadata(true, OnIsCyclicPropertyChanged));

        /// <summary>
        /// IsCyclicProperty property changed handler.
        /// </summary>
        /// <param name="d">DomainUpDown instance that changed its IsCyclic value.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsCyclicPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeUpDown source = (TimeUpDown)d;
            source.SetValidSpinDirection();
        }
        #endregion public bool IsCyclic

        #region public object BalloonContent
        /// <summary>
        /// Gets the text used to guide the user when entering time.
        /// </summary>
        public object BalloonContent
        {
            get { return GetValue(BalloonContentProperty) as object; }
        }

        /// <summary>
        /// Identifies the BalloonContent dependency property.
        /// </summary>
        public static readonly DependencyProperty BalloonContentProperty =
            DependencyProperty.Register(
                "BalloonContent",
                typeof(object),
                typeof(TimeUpDown),
                new PropertyMetadata(String.Empty, OnBalloonContentPropertyChanged));

        /// <summary>
        /// BalloonContentProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its BalloonContent.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnBalloonContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion public object BalloonContent

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeUpDown"/> class.
        /// </summary>
        public TimeUpDown()
        {
            DefaultStyleKey = typeof(TimeUpDown);

            _timeCoercionHelper = new TimeCoercionHelper(this);
        }

        /// <summary>
        /// Builds the visual tree for the TimeUpDown control when a new
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (Text != null)
            {
                Text.SelectionChanged -= SelectionChanged;
                Text.TextChanged -= InputChanged;
            }
            base.OnApplyTemplate();
            if (Text != null)
            {
                Text.SelectionChanged += SelectionChanged;
                Text.TextChanged += InputChanged;
            }

            SetValidSpinDirection();
        }

        /// <summary>
        /// Provides handling for the ValueChanging event.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnValueChanging(RoutedPropertyChangingEventArgs<DateTime?> e)
        {
            if (_ignoreValueChange)
            {
                e.InCoercion = true;
                OnValueChanged(new RoutedPropertyChangedEventArgs<DateTime?>(e.OldValue, e.NewValue));
                return;
            }

            // change is from value itself.
            bool success = _timeCoercionHelper.CoerceValue(e.OldValue, e.NewValue);

            if (success)
            {
                e.InCoercion = false;
                e.NewValue = Value;
                base.OnValueChanging(e);
                SetValidSpinDirection();
            }
            else
            {
                e.InCoercion = true;
            }
        }

        /// <summary>
        /// Called by ApplyValue to parse user input.
        /// </summary>
        /// <param name="text">User input.</param>
        /// <returns>Value parsed from user input.</returns>
        protected override DateTime? ParseValue(string text)
        {
            DateTime? parsed = ActualTimeGlobalizationInfo.ParseTime(text, ActualFormat, TimeParsers);
            if (parsed.HasValue && Value.HasValue)
            {
                // use the datepart of Value, and the timepart of parsed.
                return Value.Value.Date.Add(parsed.Value.TimeOfDay);
            }

            return parsed;
        }

        /// <summary>
        /// Renders the value property into the textbox text.
        /// </summary>
        /// <returns>Formatted Value.</returns>
        protected internal override string FormatValue()
        {
            _lastParsedText = ActualTimeGlobalizationInfo.FormatTime(Value, ActualFormat);
            return _lastParsedText;
        }

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Increase.
        /// </summary>
        protected override void OnIncrement() 
        {
            if (Text != null)
            {
                int caretPosition = Text.SelectionStart;

                TimeSpan ts = ActualTimeGlobalizationInfo.GetTimeUnitAtTextPosition(Text.Text, caretPosition, ActualFormat);
                
                // new value could be on a new day.
                DateTime newValue = ActualTimeGlobalizationInfo.OnIncrement(Value.Value, ts);

                // if spinning 10 hours and going over boundary is confusing
                // boundary is the maximum set, or the day boundary
                // revert back to 1 hour.
                if (ts == TimeSpan.FromHours(10) && newValue > ActualMaximum)
                {
                    ts = TimeSpan.FromHours(1);
                    newValue = ActualTimeGlobalizationInfo.OnIncrement(Value.Value, ts);
                }

                if (newValue > ActualMaximum)
                {
                    if (IsCyclic)
                    {
                        // if there is no maximum set, we can skip over it
                        // otherwise set the time to the minimum
                        if (Maximum.HasValue)
                        {
                            newValue = ActualMinimum;
                        }
                    }
                    else
                    {
                        newValue = ActualMaximum;
                    }
                }

                Value = newValue;

                // check if old caretPosition is still valid for this TimeSpan
                if (ts == ActualTimeGlobalizationInfo.GetTimeUnitAtTextPosition(Text.Text, caretPosition, ActualFormat))
                {
                    Text.SelectionStart = caretPosition;
                }
                else
                {
                    int newCaretPosition = ActualTimeGlobalizationInfo.GetTextPositionForTimeUnit(Text.Text, ts, ActualFormat);
                    Text.SelectionStart = newCaretPosition > -1 ? newCaretPosition : caretPosition;
                }
            }
        }

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Increase.
        /// </summary>
        protected override void OnDecrement()
        {
            if (Text != null)
            {
                int caretPosition = Text.SelectionStart;

                TimeSpan ts = ActualTimeGlobalizationInfo.GetTimeUnitAtTextPosition(Text.Text, caretPosition, ActualFormat);

                // new value could be on a new day.
                DateTime newValue = ActualTimeGlobalizationInfo.OnDecrement(Value.Value, ts);

                // if spinning 10 hours and going over boundary is confusing
                // boundary is the maximum set, or the day boundary
                // revert back to 1 hour.
                if (ts == TimeSpan.FromHours(10) && newValue < ActualMinimum)
                {
                    ts = TimeSpan.FromHours(1);
                    newValue = ActualTimeGlobalizationInfo.OnDecrement(Value.Value, ts);
                }

                if (newValue < ActualMinimum)
                {
                    if (IsCyclic)
                    {
                        // if there is no maximum set, we can skip over it
                        // otherwise set the time to the minimum
                        if (Minimum.HasValue)
                        {
                            newValue = ActualMaximum;
                        }
                    }
                    else
                    {
                        newValue = ActualMinimum;
                    }
                }

                Value = newValue;

                // check if old caretPosition is still valid for this TimeSpan
                if (ts == ActualTimeGlobalizationInfo.GetTimeUnitAtTextPosition(Text.Text, caretPosition, ActualFormat))
                {
                    Text.SelectionStart = caretPosition;
                }
                else
                {
                    int newCaretPosition = ActualTimeGlobalizationInfo.GetTextPositionForTimeUnit(Text.Text, ts, ActualFormat);
                    Text.SelectionStart = newCaretPosition > -1 ? newCaretPosition : caretPosition;
                }
            }
        }

        /// <summary>
        /// Sets the valid spin direction based on the position of the caret,
        /// the value and the minimum and maximum.
        /// </summary>
        private void SetValidSpinDirection()
        {
            ValidSpinDirections spinDirections = ValidSpinDirections.None;

            if (Text != null && FocusManager.GetFocusedElement() == Text && !string.IsNullOrEmpty(Text.Text) && Value.HasValue)
            {
                if (IsCyclic)
                {
                    spinDirections = ValidSpinDirections.Decrease | ValidSpinDirections.Increase;
                }
                else
                {
                    TimeSpan span = ActualTimeGlobalizationInfo.GetTimeUnitAtTextPosition(
                        Text.Text,
                        Text.SelectionStart,
                        ActualFormat);

                    if (ActualTimeGlobalizationInfo.OnIncrement(Value.Value, span) <= ActualMaximum)
                    {
                        spinDirections = spinDirections | ValidSpinDirections.Increase;
                    }

                    if (ActualTimeGlobalizationInfo.OnDecrement(Value.Value, span) >= ActualMinimum)
                    {
                        spinDirections = spinDirections | ValidSpinDirections.Decrease;
                    }
                }
            }
            if (Spinner != null)
            {
                Spinner.ValidSpinDirection = spinDirections;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event from TextBox.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> 
        /// instance containing the event data.</param>
        private void SelectionChanged(object sender, RoutedEventArgs e)
        {
            SetValidSpinDirection();
        }

        /// <summary>
        /// Handles the TextChanged event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> 
        /// instance containing the event data.</param>
        private void InputChanged(object sender, TextChangedEventArgs e)
        {
            DetermineHint();
        }

        /// <summary>
        /// Determines the value of the hint property.
        /// </summary>
        private void DetermineHint()
        {
            // when in a designer, ignore this logic.
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            // might show current date as a guide. However, check if that is within range.
            DateTime fallbackTime = DateTime.Now;
            if (fallbackTime.TimeOfDay > ActualMaximum.TimeOfDay)
            {
                fallbackTime = ActualMaximum;
            }
            if (fallbackTime.TimeOfDay < ActualMinimum.TimeOfDay)
            {
                fallbackTime = ActualMinimum;
            }

            DateTime? parsed;
            if (!ActualTimeGlobalizationInfo.TryParseTime(Text.Text, ActualFormat, TimeParsers, out parsed))
            {
                IsShowBalloon = true;

                // unable to parse
                VisualStateManager.GoToState(this, InvalidTimeStateName, true);

                SetValue(BalloonContentProperty, String.Format(CultureInfo.InvariantCulture, WatermarkHintFormat, ActualTimeGlobalizationInfo.FormatTime(fallbackTime, ActualFormat)));
            }
            else
            {
                if (parsed.HasValue)
                {
                    // we were able to parse, does the value fall within range?
                    if ((parsed.Value.TimeOfDay > ActualMaximum.TimeOfDay) ||
                        (parsed.Value.TimeOfDay < ActualMinimum.TimeOfDay))
                    {
                        // value does not fall within range.

                        IsShowBalloon = true;
                        VisualStateManager.GoToState(this, InvalidTimeStateName, true);

                        SetValue(BalloonContentProperty, String.Format(CultureInfo.InvariantCulture, WatermarkHintFormat, ActualTimeGlobalizationInfo.FormatTime(fallbackTime, ActualFormat)));
                    }
                    else
                    {
                        // value falls within range

                        string balloonHint = ActualTimeGlobalizationInfo.FormatTime(parsed.Value, ActualFormat);
                        IsShowBalloon = !Text.Text.Equals(balloonHint, StringComparison.OrdinalIgnoreCase);
                        VisualStateManager.GoToState(this, ValidTimeStateName, true);
                        SetValue(BalloonContentProperty, balloonHint);
                    }
                }
                else
                {
                    // parsed to a null
                    IsShowBalloon = true;
                    VisualStateManager.GoToState(this, EmptyTimeStateName, true);
                    SetValue(BalloonContentProperty, String.Format(CultureInfo.InvariantCulture, WatermarkHintFormat, ActualTimeGlobalizationInfo.FormatTime(fallbackTime, ActualFormat)));
                }
            }
        }

        /// <summary>
        /// Provides handling for the KeyDown event.
        /// </summary>
        /// <param name="e">Key event args.</param>
        /// <remarks>
        /// Only support up and down arrow keys.
        /// </remarks>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (IsEditable == false)
            {
                e.Handled = true;
                return;
            }

            if (Text.Text != _lastParsedText)
            {
                if (e.Key == Key.Up || e.Key == Key.Down)
                {
                    int caretPosition = Text.SelectionStart;
                    ApplyValue(Text.Text);
                    e.Handled = true;
                    if (caretPosition >= 0 && caretPosition < Text.Text.Length)
                    {
                        // there are situations where the caretposition
                        // is not correct. However, this is the 99% case.
                        Text.SelectionStart = caretPosition;
                        if (e.Key == Key.Up)
                        {
                            OnIncrement();
                        }
                        else
                        {
                            OnDecrement();
                        }
                    }
                }
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Provides handling for the GotFocus event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (Interaction.AllowGotFocus(e))
            {
                DetermineHint();
                SetValidSpinDirection();
                Interaction.OnGotFocusBase();
                base.OnGotFocus(e);
            }
        }

        /// <summary>
        /// Provides handling for the LostFocus event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (Interaction.AllowLostFocus(e))
            {
                IsShowBalloon = false;
                SetValidSpinDirection();
                Interaction.OnLostFocusBase();
                base.OnLostFocus(e);
            }
        }

        /// <summary>
        /// Update current visual state.
        /// </summary>
        /// <param name="useTransitions">True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.</param>
        internal override void UpdateVisualState(bool useTransitions)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                // edit or display state
                VisualStateManager.GoToState(this, IsShowBalloon ? BalloonOpenedStateName : BalloonClosedStateName, useTransitions);
            }

            // handle common states
            base.UpdateVisualState(useTransitions);
        }

        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            UpdateVisualState(useTransitions);
        }

        #region ITimeInput Members
        /// <summary>
        /// Gets or sets the Value property.
        /// </summary>
        /// <value></value>
        DateTime? ITimeInput.Value
        {
            get { return Value; }
            set { Value = value; }
        }

        /// <summary>
        /// Gets or sets the minimum time.
        /// </summary>
        /// <value>The minimum time.</value>
        DateTime? ITimeInput.Minimum
        {
            get { return Minimum; }
            set { Minimum = value; }
        }

        /// <summary>
        /// Gets or sets the maximum time.
        /// </summary>
        /// <value>The maximum time.</value>
        DateTime? ITimeInput.Maximum
        {
            get { return Maximum; }
            set { Maximum = value; }
        }
        #endregion
    }
}