using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Automation.Peers;

namespace SL_31005_VS.TimeInput
{
    #region timepicker
    /// <summary>
    /// Concept stolen from AutoCompleteBox. 
    /// 6.1. A TimeInput control will have a time popup shown in a popup that scales to match. 
    /// </summary>
    [TemplatePart(Name = "TimePickerPopup", Type = typeof(ITimePickerPopup))]
    /// <summary>
    /// Stolen from Picker. 
    /// </summary>
    [TemplatePart(Name = "Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "DropDownToggle", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "TimeUpDown", Type = typeof(TimeUpDown))]
    [StyleTypedProperty(Property = "SpinnerStyle", StyleTargetType = typeof(ButtonSpinner))]
    [TemplateVisualState(Name = "PopupClosed", GroupName = "PopupStates")]
    [TemplateVisualState(Name = "PopupOpened", GroupName = "PopupStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Valid", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "InvalidFocused", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "InvalidUnfocused", GroupName = "ValidationStates")]
    [StyleTypedProperty(Property = "TimeUpDownStyle", StyleTargetType = typeof(TimeUpDown))]
    public class TimePicker : Picker
    {
        public TimePicker()
        {
        }

        /// <summary>
        /// Stolen from UpDownBase(of T)
        /// </summary>
        [TypeConverter(typeof(TimeTypeConverter))]
        public virtual DateTime? Value { get; set; }
        public event RoutedPropertyChangedEventHandler<DateTime?> ValueChanged;
        public event RoutedPropertyChangingEventHandler<DateTime?> ValueChanging;
        protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<DateTime?> e) { ; }
        protected virtual void OnValueChanging(RoutedPropertyChangingEventArgs<DateTime?> e) { ; }

        /// <summary>
        /// DPs. Stolen from NumericUpDown. Default: Null. 
        /// 4.9. If the new Value is outside the minimum and maximum time range, use last valid Value. 
        /// 5.4. If a spinner sets a value outside the time range and is not cyclic, use last valid Value.
        /// 5.2. If a spinner sets a value lower than the minimum and is cyclic 
        ///         the new value is maximum-(value-minimum)
        /// 5.3. If a spinner sets a value higher than the maximum and is cyclic 
        ///         the new value is minimum+(value-maximum)
        /// 6.7. A TimeInput would allow limiting the popup’s time range to a maximum and minimum time. 
        /// </summary>
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Maximum { get; set; }
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Minumum { get; set; }

        /// <summary>
        /// DP. Default: new TimeGlobalizationInfo().
        /// 1.2. All globalized decisions can be overridden from an external strategy. 
        /// </summary>
        public TimeGlobalizationInfo TimeGlobalizationInfo { get; set; }

        /// <summary>
        /// DP. Required custom design time UX. Default: null.
        /// 2.1. The end-developer/end-designer could determine which culture is used with the control.
        /// </summary>
        [TypeConverter(typeof(CultureInfoTypeConverter))]
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Readonly DP. 
        /// 2.	Culture Determination (prioritized order) 
        /// 2.2. If (2.1) missing or neutral,  Use current thread UI culture.
        /// 2.3. If (2.2) missing or neutral, Use current thread culture.
        /// 2.4. If (2.3) missing or neutral, use “en-US”.
        /// 2.5. The end-developer should be able to determine what culture is being used in the control.
        /// </summary>
        public CultureInfo ActualCulture { get { return null; } }

        /// <summary>
        /// DP. Default: TimePickerFormat.Short
        /// 3.2. The InputControl would be able to display short and long values based on the determined culture
        /// </summary>
        public TimePickerFormat Format { get; set; }

        /// <summary>
        /// DP. Default: null
        /// 3.3. The end-developer/designer can specify a specific format that would be used to format display.
        /// 4.3. A TimeInput would allow using a format string specified for it.
        /// </summary>
        public string FormatString { get; set; }

        /// <summary>
        /// DP. Default: TimeInputFormatCollection with (4.6.2.X)
        /// 4.7.2.	We will ship with the following custom formats [pri1]
        /// 4.7.2.1. Custom input formatting “X” -> “X:00”.
        /// 4.7.2.2. Custom input formatting “Xp” -> “X AM” and “xp” -> “X PM”
        /// 4.7.2.3. Custom Input formatting “+Xh” -> Current hour + X and “-Xh” -> Current hour - X
        /// </summary>
        public TimeParsersCollection TimeInputParsers { get; set; }

        /// <summary>
        /// DP. 
        /// 6.17. A TimeInput control would allow styling it’s TimeUpDown control and it’s spinner.
        /// </summary>
        public Style TimeUpDownStyle { get; set; }
        public Style SpinnerStyle { get; set; }

        /// <summary>
        /// Stolen from UpDownBase(of T) and it's new additions.
        /// </summary>
        public event EventHandler<UpDownParseErrorEventArgs> ParseError;
        public event EventHandler<ParsingEventArgs<DateTime?>> Parsing;
        /// <summary>
        /// DP. Default: Sepcified in each popup.
        /// 6.12. A TimeInput would allow setting the seconds interval for its popup.
        /// </summary>
        public int PopupSecondsInterval { get; set; }

        /// <summary>
        /// DP. Default: Sepcified in each popup.
        /// 6.11. A TimeInput would allow setting the minutes interval for its popup.
        /// </summary>
        public int PopupMinutesInterval { get; set; }

        /// <summary>
        /// DP. Default: ProgressiveTimePickerPopup.
        /// 6.15. When showing time formats, the popup will follow “TextBox display” work flow.
        /// </summary>
        public ITimePickerPopup Popup { get; set; }

        /// <summary>
        /// DPs.Default: TimePickerOptions.None
        /// 6.8. A TimeInput would be able decide whether a popup supports selecting seconds.
        /// 6.9. A TimeInput would be able decide whether a popup supports selecting AM/PM.
        /// </summary>
        public PopupTimeSelectionMode PopupTimeSelectionMode { get; set; }
    }
    #endregion

    #region TimeUpDown
    /// <summary>
    /// Stolen from UpDownBase(of T). 
    /// </summary>
    [TemplateVisualStateAttribute(Name = "Focused", GroupName = "FocusStates")]
    [TemplateVisualStateAttribute(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualStateAttribute(Name = "Unfocused", GroupName = "FocusStates")]
    [TemplateVisualStateAttribute(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualStateAttribute(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualStateAttribute(Name = "Pressed", GroupName = "CommonStates")]
    [TemplatePartAttribute(Name = "Text", Type = typeof(TextBox))]
    /// <summary>
    /// Stolen from UpDownBase(of T). 
    /// 5.1. A TimeInput has a visible spinner control.
    /// </summary>
    [TemplatePartAttribute(Name = "Spinner", Type = typeof(Spinner))]
    [StyleTypedPropertyAttribute(Property = "SpinnerStyle", StyleTargetType = typeof(ButtonSpinner))]
    /// <summary>
    /// Stolen from Jolt vNext "Data Validation UI" spec.  
    /// </summary> 
    [TemplateVisualState(Name = "Valid", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "InvalidFocused", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "InvalidUnfocused", GroupName = "ValidationStates")]
    public class TimeUpDown : UpDownBase<DateTime?>
    {
        /// <summary>
        /// Override property for Value so we could add a TypeConverter. 
        /// </summary>
        [TypeConverter(typeof(TimeTypeConverter))]
        public /*override*/new DateTime? Value { get; set; }
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Maximum { get; set; }
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Minumum { get; set; }

        /// <summary>
        /// UpDownBase(of T) methods.
        /// Value property defaults to: new DateTime()
        /// Heavily uses TimeGlobalizationInfo.
        /// </summery>
        protected override string FormatValue() { return string.Empty; }
        protected override void OnDecrement() { ; }
        protected override void OnIncrement() { ; }
        protected override DateTime? ParseValue(string text) { return new DateTime(); }

        public TimeParsersCollection TimeInputParsers { get; set; }
        public TimeParsersCollection ActualTimeInputParsers { get; }

        /// <summary>
        /// Stolen from TimePicker. TemplateBound in default Template. 
        /// </summary>
        public TimePickerFormat Format { get; set; }
        public string FormatString { get; set; }

        [TypeConverter(typeof(CultureInfoTypeConverter))]
        public CultureInfo Culture { get; set; }
        public CultureInfo ActualCulture { get { return null; } }

        public TimeGlobalizationInfo TimeGlobalizationInfo { get; set; }
    }
    #endregion

    #region Globalization strategy
    /// <summary>
    /// 1.2. All globalized decisions can be overridden from an external strategy. 
    /// 1.3. The external strategy would be comprehensive, yet have modular activation. 
    /// 
    /// </summary>
    public class TimeGlobalizationInfo
    {
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Get display string to put in TextBox when a new value is set. 
        /// 3.1. If An external globalization strategy can format the display string we’ll use it. 
        /// </summary>
        public virtual string FormatTime(DateTime value, TimePickerFormat Format, string FormatString) { return string.Empty; }

        /// <summary>
        /// 4.1. If an external globalization strategy can format the input string we’ll use it.
        /// </summary>
        public virtual DateTime ParseTime(string value, string FormatString,
            IList<TimeParser> TimeInputParsers, DateTime? Maximum, DateTime? Minumum) { return new DateTime(); }

        /// <summary>
        /// will return a new collection of timeparsers that will be during parsing.
        /// </summary>
        /// <param name="TimeParsers"></param>
        /// <returns></returns>
        public virtual IList<TimeParser> GetActualTimeParsers(IEnumerable<TimeParser> TimeParsers) { return null; }

        /// <summary>
        /// 6.13. A popup will display localized number strings if available from external globalization strategy.
        /// </summary>
        public virtual string FormatGlobalizedNumber(int number) { return string.Empty; }

        /// <summary>
        /// 5.5.1.	If an external globalization strategy can spin the string we’ll use it.
        /// </summary>
        public virtual DateTime GetTimeUnitAtTextPosition(string textBoxText, int caretPosition, string formatString, bool Increment) { return new TimeSpan(); }
        public virtual DateTime OnIncrement(DateTime value, TimeSpan timeSpan);
        public virtual DateTime OnDecrement(DateTime value, TimeSpan timeSpan);

        /// <summary>
        /// 6.18.	AM/PM time time differentiators displayed in a popup can be overridden by the globalization strategy. 
        /// </summary>
        public virtual string AMDesignator { get { return null; } }
        public virtual string PMDesignator { get { return null; } }
        public virtual char[] TimeSeparators { get { return null; } }
    }
    #endregion Globalization strategy

    #region Picker
    /// <summary>
    /// Stolen from AutoCompleteBox. 
    /// </summary>
    [TemplateVisualState(Name = "PopupClosed", GroupName = "PopupStates")]
    [TemplateVisualState(Name = "PopupOpened", GroupName = "PopupStates")]
    [TemplatePartAttribute(Name = "Popup", Type = typeof(Popup))]
    /// <summary>
    /// 6.3. A TimeInput will have an icon used to open the popup.
    /// </summary>
    [TemplatePartAttribute(Name = "DropDownToggle", Type = typeof(ToggleButton))]
    /// <summary>
    /// Stolen from UpDownBase(of T). 
    /// </summary>
    [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Valid", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "InvalidFocused", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "InvalidUnfocused", GroupName = "ValidationStates")]
    /// <summary>
    /// Base class for all controls with popup functionality.
    /// TimePicker, AutoCompleteBox, DatePicker and ComboBox. 
    /// </summary>
    public abstract class Picker : Control, IUpdateVisualState
    {
        /// <summary>
        /// DP. Stolen from DatePicker/AutoCompleteBox. Default: false. 
        /// 6.2. A TimeInput will enable opening its popup and checking if the popup is open or not.
        /// </summary>
        public bool IsDropDownOpen { get; set; }

        /// <summary>
        /// DP. Default: PopupButtonModes.Press
        /// 6.4. A TimeInput will open its popup based on a combination of possible user interactions.  
        /// </summary>
        public PopupButtonModes PopupButtonModes { get; set; }

        /// <summary>
        /// Occurs when the IsDropDownOpen property is changing from false to 
        /// true. The event can be cancelled.
        /// </summary>
        public event RoutedPropertyChangingEventHandler<bool> DropDownOpening;

        /// <summary>
        /// Occurs when the IsDropDownOpen property was changed from false to true.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> DropDownOpened;

        /// <summary>
        /// Occurs when the IsDropDownOpen property is changing from true to 
        /// false. The event can be cancelled.
        /// </summary>
        public event RoutedPropertyChangingEventHandler<bool> DropDownClosing;

        /// <summary>
        /// Occurs when the IsDropDownOpen property was changed from true to 
        /// false.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> DropDownClosed;

        public void UpdateVisualState(bool useTransitions);

        /// <summary>
        /// DP: stolen from AutoCompleteBox
        /// Gets or sets the maximum drop down height.
        /// </summary>
        public double MaxDropDownHeight;

    }
    #endregion

    #region parsing
    /// <summary>
    /// Changes for UpDownBase(of T)
    /// </summary>
    public class UpDownBaseAdditions<T>
    {
        /// <summary>
        /// 4.2.The TimeInput control would allow users to do control-specific formatting via an event.
        /// </summary>
        public event EventHandler<ParsingEventArgs<T>> Parsing;

        /// <summary>
        /// make this property virual so we could override it with typeconverter.
        /// </summary>
        public virtual T Value { get; set; }
    }

    public class ParsingEventArgs<T> : EventArgs
    {
        public string Input { get { return string.Empty; } }
        public T Value { get; set; }
        public bool Handled { get; set; }
    }
    #endregion parsing

    #region TimePopupSelectionMode
    /// <summary>
    /// 6.8. A TimeInput would be able decide whether a popup supports selecting seconds.
    /// 6.9. A TimeInput would be able decide whether a popup supports selecting AM/PM.
    /// </summary>
    public enum PopupTimeSelectionMode
    {
        /// <summary>
        /// AM/PM + Seconds
        /// </summary>
        SecondsAndAmPmSelection,
        /// <summary>
        /// AM/PM only
        /// </summary>
        AmPmSelection,
        /// <summary>
        /// Seconds Only
        /// </summary>
        SecondsSelection,
        /// <summary>
        /// No AM/PM and No seconds selection
        /// </summary>
        HoursAndMinutesOnly
    }
    #endregion TimePopupSelectionMode

    #region TimePickerFormat
    /// <summary>
    /// 3.2. The InputControl would be able to display short and long values based on the determined culture
    /// </summary>
    public enum TimePickerFormat
    {
        /// <summary>
        /// uses ActualCulture.ShortTimeString to format display values.
        /// </summary>
        Short,
        /// <summary>
        /// uses ActualCulture.LongTimeString to format display values.
        /// </summary>
        Long,
        /// <summary>
        /// uses DisplayFormatString to format display values. 
        /// </summary>
        Custom
    }
    #endregion TimePickerFormat

    #region time parsers
    /// <summary>
    /// Silverlight XAML parser can't handle generics. 
    /// Voila - this isn't a generic collection. 
    /// </summary>
    public class TimeParsersCollection : ObservableCollection<TimeParser>, IValueConverter
    {
        /// <summary>
        /// [ShawnOs] Allow to parse using a collection of parsers.
        /// Scenario: "Attaching" the parsers to a TextBox control.
        /// [JaHusain] Make it an IValueConverter. 
        /// </summary>
        public virtual bool TryParse(string text, CultureInfo culture, out DateTime? result);
    }

    /// <summary>
    /// 4.7. A TimeInput would use custom formatting defined on the control. 
    /// </summary>
    public abstract class TimeParser : IValueConverter
    {
        /// <summary>
        /// 4.7.1.	Each custom formatting will attempt formatting the input value or return null if it can’t.
        /// </summary>
        public virtual bool TryParse(string text, CultureInfo culture, out DateTime? result);
    }


    /// <summary>
    /// 4.7.2.	We will ship with the following custom formats [pri2 samples]
    /// 4.7.2.1. Custom input formatting “X” -> “X:00”.
    /// example: Input: "13", result: "13:00". 
    /// </summary>
    public class NumberToHourTimeInputFormat : TimeParser
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) { return null; }
    }

    /// <summary>
    /// 4.7.2.	We will ship with the following custom formats [pri2 samples]
    /// 4.7.2.2. Custom input formatting “Xa(m)” -> “X AM” 
    ///                              and “Xp(m)” -> “X PM”
    /// example: Input: "2p", result: "14:00"
    /// </summary>
    public class NumberAmPmToHourTimeInputFormat : TimeParser
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) { return null; }
    }

    /// <summary>
    /// 4.7.2.	We will ship with the following custom formats [pri2 samples]
    /// 4.7.2.3. Custom Input formatting “+Xh” -> Current time + X hours 
    ///                              and “-Xh” -> Current time – X hours
    /// example: DateTime.Now = "16:30", input: "+2h", result: "18:30"
    /// </summary>
    public class PlusMinusHourTimeInputFormat : TimeParser
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) { return null; }
    }

    /// <summary>
    /// 4.7.2.	We will ship with the following custom formats [pri2 samples]
    /// 4.7.2.3. Custom Input formatting “+Xh” -> Current time + X minutes 
    ///                              and “-Xh” -> Current time – X minutes
    /// example: DateTime.Now = "16:30", input: "+30m", result: "17:00"
    /// </summary>
    public class PlusMinusMinuteTimeInputFormat : TimeParser
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) { return null; }
    }
    #endregion time parsers

    #region popups
    public interface ITimePickerPopup
    {
        /// <summary>
        /// TemplateBinding from TimePicker.IsDropDownOpen. 
        /// 6.5.4.	A popup should be able to close itself based on its own logic.  
        /// </summary>
        event RoutedEventHandler Cancel;
        event RoutedEventHandler Commit;

        /// <summary>
        /// TemplateBinding to TimePicker properties. 
        /// 6.7. A TimeInput would allow limiting the popup’s time range to a maximum and minimum time. 
        /// </summary>
        DateTime? Maximum { get; set; }
        DateTime? Minimum { get; set; }

        /// <summary>
        /// TemplateBinding to TimePicker.TimePickerOption. 
        /// 6.8. A TimeInput would be able decide whether a popup supports selecting seconds.
        /// 6.9. A TimeInput would be able decide whether a popup supports selecting AM/PM.
        /// </summary>
        TimePopupSelectionMode TimePopupSelectionMode { get; set; }

        /// <summary>
        /// Stolen from UpDownBase(of T).
        /// TemplateBinding to TimePicker.Value.  
        /// 6.10. A  TimeInput would keep its value synced up with the popup’s value. 
        /// </summary>
        DateTime? Value { get; set; }
        event RoutedPropertyChangedEventHandler<DateTime?> ValueChanged;
        event RoutedPropertyChangingEventHandler<DateTime?> ValueChanging;

        /// <summary>
        /// 6.12. A TimeInput would allow setting the seconds interval for its popup.
        /// </summary>
        int SecondsInternal { get; set; }

        /// <summary>
        /// 6.11. A TimeInput would allow setting the minutes interval for its popup.
        /// </summary>
        int MinutesInterval { get; set; }

        /// <summary>
        /// TemplateBinding from TimePicker.TimeStrategy.
        /// Used to display localized values in picker. 
        /// </summary>
        TimeGlobalizationInfo TimeGlobalizationInfo { get; set; }

        /// <summary>
        /// Stolen from AutoCompleteBox. 
        /// 6.14. A popup will support UI Automation.
        /// </summary>
        AutomationPeer CreateAutomationPeer();

        /// <summary>
        /// Stolen from TimePicker.
        /// We want the Popups to be independent controls and to do that they need
        /// to be globalized seperately. 
        /// </summary>
        CultureInfo Culture { get; set; }
        CultureInfo ActualCulture { get; }

        /// <summary>
        /// Handles key presses in the TimeUpDown control and allows the picker to respond.
        /// RJ decides during Dev phase if we need this or not. 
        /// </summary>
        //void HandleKey(Key key); 
    }

    [TemplatePart(Name = "Grid", Type = typeof(Grid))]
    [StyleTypedProperty(Property = "ButtonStyle", StyleTargetType = typeof(TimeButton))]
    /// <summary>
    /// 7.	Progressive disclosure popup
    /// </summary>
    public class ProgressiveTimePickerPopup : Control, ITimePickerPopup
    {
        /// <summary>
        /// DPs. 
        /// Used to TemplateBind hour Buttons ItemsControls to localized strings.
        /// 7.2. Each row of time buttons shows a globalized strings array. 
        /// 7.3. The hour popup will contain 1 or 2 rows based on whether the popup supports AM/PM or not. 
        /// </summary>
        public IEnumerable<string> First12Hours { get; private set; }
        public IEnumerable<string> Last12Hours { get; private set; }

        /// <summary>
        /// DPs. 
        /// Used to TemplateBind hour Minutes ItemsControl to localized strings.
        /// 7.2. Each row of time buttons shows a globalized strings array. 
        /// 7.4. The minutes popup will only expose minutes based on the minutes interval. 
        /// </summary>
        public IEnumerable<string> Minutes { get; private set; }

        /// <summary>
        /// DPs. 
        /// Used to TemplateBind hour Seconds ItemsControl to localized strings.
        /// 7.2. Each row of time buttons shows a globalized strings array. 
        /// 7.5. The seconds popup will only expose minutes based on the seconds interval. 
        /// </summary>
        public IEnumerable<string> Seconds { get; private set; }

        /// <summary>
        /// DP. default: null.
        /// TemplateBound to the ItemsControl button style. 
        /// 7.6. The popup will enable end-developers/end-designers to style all the popup buttons. 
        /// </summary>
        public Style ButtonStyle { get; set; }

        /// <summary>
        /// Default SecondsInternal = 0,  Default MinutesInterval=15
        /// </summary>
        #region ITimePickerPopup Members
        public event RoutedEventHandler Cancel;
        public event RoutedEventHandler Commit;
        public event RoutedPropertyChangedEventHandler<DateTime?> ValueChanged;
        public event RoutedPropertyChangingEventHandler<DateTime?> ValueChanging;
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Maximum { get; set; }
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Minimum { get; set; }
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Value { get; set; }
        public int SecondsInternal { get; set; }
        public int MinutesInterval { get; set; }
        public TimeGlobalizationInfo TimeGlobalizationInfo { get; set; }
        public AutomationPeer CreateAutomationPeer() { return null; }
        public TimePopupSelectionMode TimePopupSelectionMode { get; set; }
        public CultureInfo Culture { get; set; }
        public CultureInfo ActualCulture { get { return null; } }
        #endregion
    }

    [TemplateVisualStateAttribute(Name = "Unfocused", GroupName = "FocusStates")]
    [TemplateVisualStateAttribute(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualStateAttribute(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualStateAttribute(Name = "Pressed", GroupName = "CommonStates")]
    [TemplateVisualStateAttribute(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualStateAttribute(Name = "Focused", GroupName = "FocusStates")]
    /// <summary>
    /// 7.6. The popup will enable end-developers/end-designers to style all the popup buttons. 
    /// Used by The Progressive popup to have default style & template in blend. 
    /// </summary>
    public class TimeButton : Button
    {

    }

    /// <summary>
    /// 8.	Listbox popup [pri1] 
    /// </summary>
    public class ListTimePickerPopup : Control, ITimePickerPopup
    {
        /// <summary>
        /// DP. 
        /// TemplateBound to the ListBox ItemsSource. 
        /// 8.1.	The popup popup will contain a list of localized & globalized times 
        /// 8.1.1.	From the minimum & maximum time ranges  
        /// 8.1.2.	With the appropriate minutes intervals 
        /// </summary>
        public IEnumerable<string> TimeItems { get; private set; }

        /// <summary>
        /// Default SecondsInternal = 0,  Default MinutesInterval=30;
        /// </summary>
        #region ITimePickerPopup Members
        public event RoutedEventHandler Cancel;
        public event RoutedEventHandler Commit;
        public event RoutedPropertyChangedEventHandler<DateTime?> ValueChanged;
        public event RoutedPropertyChangingEventHandler<DateTime?> ValueChanging;
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Maximum { get; set; }
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Minimum { get; set; }
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Value { get; set; }
        public int SecondsInternal { get; set; }
        public int MinutesInterval { get; set; }
        public TimeGlobalizationInfo TimeGlobalizationInfo { get; set; }
        public AutomationPeer CreateAutomationPeer() { return null; }
        public TimePopupSelectionMode TimePopupSelectionMode { get; set; }
        public CultureInfo Culture { get; set; }
        public CultureInfo ActualCulture { get { return null; } }
        #endregion
    }

    /// <summary>
    /// 9.5. On the top left the clock would have 2 AM/PM buttons which determine the AM/PM respectively. 
    /// </summary>
    [TemplatePart(Type = typeof(ToggleButton), Name = "AmPmButton")]
    /// <summary>
    /// 9.	Analog Clock popup [pri1]
    /// </summary>
    public class ClockTimePickerPopup : Control, ITimePickerPopup
    {
        /// <summary>
        /// DP. default: 0.
        /// TemplateBound to RotateTransform.Angle for the hours hand. 
        /// 9.2. The clock would have a short hours hand rotated based on the current value/time.
        /// </summary>
        public double HoursHandAngle { get; set; }

        /// <summary>
        /// DP. default: 0.
        /// TemplateBound to RotateTransform.Angle for the minutes hand. 
        /// 9.3. The clock would have a long minutes hand rotated based on the current value/time.
        /// </summary>
        public double MinutesHandAngle { get; set; }

        /// <summary>
        /// 9.1.3.	The face of the clock will have globalized hours placed 30o apart. 
        /// </summary>
        public string Hour1 { get; private set; }
        public string Hour2 { get; private set; }
        public string Hour3 { get; private set; }
        public string Hour4 { get; private set; }
        public string Hour5 { get; private set; }
        public string Hour6 { get; private set; }
        public string Hour7 { get; private set; }
        public string Hour8 { get; private set; }
        public string Hour9 { get; private set; }
        public string Hour10 { get; private set; }
        public string Hour11 { get; private set; }
        public string Hour12 { get; private set; }

        /// <summary>
        /// 9.7. Any mouse click on the face of the clock would cause the closest hand to rotate accordingly.
        /// 9.7.1. “The closest hand” is defined base on what angle (0-360) was clicked and the current rotation angles for clock hands.
        /// </summary>
        protected virtual void OnClockClick(double Angle) { }
        public event RoutedPropertyChangedEventHandler<double> ClockClicked;

        /// <summary>
        /// Default SecondsInternal = 0,  Default MinutesInterval=10
        /// </summary>
        #region ITimePickerPopup Members
        public event RoutedEventHandler Cancel;
        public event RoutedEventHandler Commit;
        public event RoutedPropertyChangedEventHandler<DateTime?> ValueChanged;
        public event RoutedPropertyChangingEventHandler<DateTime?> ValueChanging;
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Maximum { get; set; }
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Minimum { get; set; }
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Value { get; set; }
        public int SecondsInternal { get; set; }
        public int MinutesInterval { get; set; }
        public TimeGlobalizationInfo TimeGlobalizationInfo { get; set; }
        public AutomationPeer CreateAutomationPeer() { return null; }
        public TimePopupSelectionMode TimePopupSelectionMode { get; set; }
        public CultureInfo Culture { get; set; }
        public CultureInfo ActualCulture { get { return null; } }
        #endregion
    }

    /// <summary>
    /// 10.	Ruler popup [Pri3 – will be shipped in the sample app]
    /// </summary>
    public class RulerTimePickerPopup : ItemsControl, ITimePickerPopup
    {
        /// <summary>
        /// 10.5.	For every valid selection the popup will create a new styled button that looks like so: ┌─XX─┐
        /// 10.5.1.	Hour buttons would look like so: ┌─13:00─┐.
        /// </summary>
        public IEnumerable<string> TimeItems { get; private set; }

        /// <summary>
        /// Default SecondsInternal = 0,  Default MinutesInterval=10
        /// </summary>
        #region ITimePickerPopup Members
        public event RoutedEventHandler Cancel;
        public event RoutedEventHandler Commit;
        public event RoutedPropertyChangedEventHandler<DateTime?> ValueChanged;
        public event RoutedPropertyChangingEventHandler<DateTime?> ValueChanging;
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Maximum { get; set; }
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Minimum { get; set; }
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Value { get; set; }
        public int SecondsInternal { get; set; }
        public int MinutesInterval { get; set; }
        public TimeGlobalizationInfo TimeGlobalizationInfo { get; set; }
        public AutomationPeer CreateAutomationPeer() { return null; }
        public TimePopupSelectionMode TimePopupSelectionMode { get; set; }
        public CultureInfo Culture { get; set; }
        public CultureInfo ActualCulture { get { return null; } }
        #endregion
    }

    /// <summary>
    /// 6.16.	The TimeInput control would allow switching between TimePopups by setting a single property.
    /// This control sits in the default TimePicker template and gets all the TemplateBindings.
    /// Than it gets a TimePickerPopup and adds it to the visual tree with TemplateBindings to it.
    /// </summary>
    public class TimePickerPopupAdapter : Control, ITimePickerPopup
    {
        /// <summary>
        /// Adds the TimePickerPopup to the visual tree and sets appropriate bindings. 
        /// </summary>
        public ITimePickerPopup Popup { get; set; }

        #region ITimePickerPopup Members
        public event RoutedEventHandler Cancel;
        public event RoutedEventHandler Commit;
        public event RoutedPropertyChangedEventHandler<DateTime?> ValueChanged;
        public event RoutedPropertyChangingEventHandler<DateTime?> ValueChanging;
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Maximum { get; set; }
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Minimum { get; set; }
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Value { get; set; }
        public int SecondsInternal { get; set; }
        public int MinutesInterval { get; set; }
        public TimeGlobalizationInfo TimeGlobalizationInfo { get; set; }
        public AutomationPeer CreateAutomationPeer() { return null; }
        public TimePopupSelectionMode TimePopupSelectionMode { get; set; }
        public CultureInfo Culture { get; set; }
        public CultureInfo ActualCulture { get { return null; } }
        #endregion
    }

    /// <summary>
    /// 11.7.	The popup will allow styling it’s Sliders without editing it’s template. 
    /// </summary>
    [StyleTypedProperty(Property = "SliderStyle", StyleTargetType = typeof(Slider))]
    /// <summary>
    /// 11.1.	This popup will expose the current time in the form of 3 vertical sliders. 
    ///         (hours, minutes, seconds)
    /// </summary>
    [TemplatePart(Name = "HoursSlider", Type = typeof(RangeBase))]
    [TemplatePart(Name = "MinutesSlider", Type = typeof(RangeBase))]
    [TemplatePart(Name = "SecondsSlider", Type = typeof(RangeBase))]
    /// <summary>
    /// 11.	 Slider Popup [Pri1] 
    /// </summary>
    public class SliderTimePickerPopup : ITimePickerPopup
    {
        /// <summary>
        /// DP. TemplateBinding to Sliders.Value. 
        /// 11.1.	This popup will expose the current time in the form of 3 vertical sliders. 
        /// (hours, minutes, seconds)
        /// </summary>
        public int Hour { get; private set; }
        public int Minute { get; private set; }
        public int Second { get; private set; }

        /// <summary>
        /// 11.7.	The popup will allow styling it’s Sliders without editing it’s template. 
        /// </summary>
        public Style SliderStyle { get; set; }

        #region ITimePickerPopup Members
        public event RoutedEventHandler Cancel;
        public event RoutedEventHandler Commit;
        public event RoutedPropertyChangedEventHandler<DateTime?> ValueChanged;
        public event RoutedPropertyChangingEventHandler<DateTime?> ValueChanging;
        /// <summary>
        /// 11.4. This popup will use the minimum value’s hour and maximum value’s hour to set hours range. 
        /// 11.5. This popup will use the minimum value’s minutes and maximum value’s minutes to set hours range.
        /// 11.6. This popup will use the seconds value’s minutes and maximum value’s seconds to set hours range.
        /// </summary>
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Maximum { get; set; }
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Minimum { get; set; }
        [TypeConverter(typeof(TimeTypeConverter))]
        public DateTime? Value { get; set; }
        /// <summary>
        /// 11.9.	This popup will use the seconds interval to set the seconds slider small change.
        /// </summary>
        public int SecondsInternal { get; set; }
        /// <summary>
        /// 11.8. This popup will use the minutes interval to set the minutes slider small change.
        /// </summary>
        public int MinutesInterval { get; set; }
        public TimeGlobalizationInfo TimeGlobalizationInfo { get; set; }
        public AutomationPeer CreateAutomationPeer() { return null; }
        /// <summary>
        /// 11.2. The popup supports hiding the Seconds slider. 
        /// 11.3. This popup does not support AM/PM mode and will through an exception if set as such.
        public TimePopupSelectionMode TimePopupSelectionMode { get; set; }
        public CultureInfo Culture { get; set; }
        public CultureInfo ActualCulture { get { return null; } }
        #endregion
    }
    #endregion

    #region converters
    /// <summary>
    /// TypeConverter from string to CultureInfo. 
    /// "en-US" or "English (United States)" to en-US CultureInfo. 
    /// </summary>
    public class CultureInfoTypeConverter : TypeConverter
    {
    }

    /// <summary>
    /// TypeConverter from string to DateTime.
    /// tries to parse the string as any of the follwing time patterns:
    /// HH:mm, HH:mm:ss, hh:mm tt, hh:mm:ss tt, tt hh:mm, tt hh:mm:ss , H:mm, H:mm:ss, h:mm tt
    /// h:mm:ss tt, H.mm, H.mm.ss, tt h:mm, tt h:mm:ss, h:mm.tt, h:mm:ss.tt, HH.mm, HH.mm.ss.
    /// Uses the culture sent to the type converter. 
    /// Does not use any of the Globalization Time extensions from TimePicker. 
    /// </summary>
    public class TimeTypeConverter : TypeConverter
    {

    }
    #endregion converters

    #region automation peers
    /// <summary>
    /// ControlType.ComboBox
    /// Raises property indentifier changed for: ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty
    /// </summary>
    public class PickerAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider
    {
    }

    /// <summary>
    /// Raises property change for: RangeValuePatternIdentifiers.Min/Max/IsReadOnly/Value, ValuePatternIdentifiers.Value
    /// </summary>
    public class TimePickerAutomationPeer : PickerAutomationPeer, IValueProvider, IRangeValueProvider
    {
    }

    /// <summary>
    /// Raises property change for: RangeValuePatternIdentifiers.Min/Max/IsReadOnly/Value
    /// </summary>
    public class TimeUpDownAutomationPeer : UpDownBaseAutomationPeer<DateTime?>, IRangeValueProvider
    {
    }

    /// <summary>
    /// ControlType.Calender
    /// Raises property change for: RangeValuePatternIdentifiers.Min/Max/IsReadOnly/Value, ValuePatternIdentifiers.Value
    /// </summary>
    public class ClockTimePickerPopupAutomationPicker : FrameworkElementAutomationPeer, IValueConverter, IRangeValueProvider
    {
    }

    /// <summary>
    /// ControlType.Calender
    /// Raises property change for: RangeValuePatternIdentifiers.Min/Max/IsReadOnly/Value, ValuePatternIdentifiers.Value
    /// </summary>
    public class ProgressiveClockTimePickerPopupAutomationPeer : FrameworkElementAutomationPeer, IValueConverter, IRangeValueProvider, ISelectionAdapter
    {
    }

    /// <summary>
    /// Raises propety changed for: SelectionItemPatternIdentifiers.IsSelectedProperty
    /// </summary>
    public class TimeButtonAutomationPeer : ButtonAutomationPeer, ISelectionItemProvider
    {
    }

    /// <summary>
    /// ControlType.Calender
    /// Raises property change for: RangeValuePatternIdentifiers.Min/Max/IsReadOnly/Value, ValuePatternIdentifiers.Value
    /// </summary>
    public class RulerTimePickerPopupAutomationPeeer : ItemsControlAutomationPeer, IValueProvider, IRangeValueProvider
    {
    }

    /// <summary>
    /// ControlType.Calender
    /// Raises property change for: RangeValuePatternIdentifiers.Min/Max/IsReadOnly/Value, ValuePatternIdentifiers.Value
    /// AutomationEvents: SelectionItemPatternOnElementSelected
    /// </summary>
    public class ListTimePickerPopupAutomationPeeer : FrameworkElement, ISelectionProvider, IValueProvider, IRangeValueProvider
    {
    }
    #endregion
}