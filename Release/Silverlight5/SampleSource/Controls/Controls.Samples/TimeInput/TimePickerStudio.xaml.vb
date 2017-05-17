' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports System.Linq
Imports System.Collections.Generic

''' <summary>
''' Sample that allows setting properties on a TimePicker.
''' </summary>
<Sample("(3)TimePicker Studio", DifficultyLevel.Intermediate, "TimePicker")> _
Partial Public Class TimePickerStudio
    Inherits UserControl

    ''' <summary>
    ''' Cultures that are recognized on most configurations.
    ''' </summary>
    Private ReadOnly _cultureNames As String() = New String() {"af", "af-ZA", "ar", "ar-AE", "ar-BH", "ar-DZ", _
    "ar-EG", "ar-IQ", "ar-JO", "ar-KW", "ar-LB", "ar-LY", _
    "ar-MA", "ar-OM", "ar-QA", "ar-SA", "ar-SY", "ar-TN", _
    "ar-YE", "az", "az-Cyrl-AZ", "az-Latn-AZ", "be", "be-BY", _
    "bg", "bg-BG", "ca", "ca-ES", "cs", "cs-CZ", _
    "da", "da-DK", "de", "de-AT", "de-CH", "de-DE", _
    "de-LI", "de-LU", "dv", "dv-MV", "el", "el-GR", _
    "en", "en-029", "en-AU", "en-BZ", "en-CA", "en-GB", _
    "en-IE", "en-JM", "en-NZ", "en-PH", "en-TT", "en-US", _
    "en-ZA", "en-ZW", "es", "es-AR", "es-BO", "es-CL", _
    "es-CO", "es-CR", "es-DO", "es-EC", "es-ES", "es-GT", _
    "es-HN", "es-MX", "es-NI", "es-PA", "es-PE", "es-PR", _
    "es-PY", "es-SV", "es-UY", "es-VE", "et", "et-EE", _
    "eu", "eu-ES", "fa", "fa-IR", "fi", "fi-FI", _
    "fo", "fo-FO", "fr", "fr-BE", "fr-CA", "fr-CH", _
    "fr-FR", "fr-LU", "fr-MC", "gl", "gl-ES", "gu", _
    "gu-IN", "he", "he-IL", "hi", "hi-IN", "hr", _
    "hr-HR", "hu", "hu-HU", "hy", "hy-AM", "id", _
    "id-ID", "is", "is-IS", "it", "it-CH", "it-IT", _
    "ja", "ja-JP", "ka", "ka-GE", "kk", "kk-KZ", _
    "kn", "kn-IN", "ko", "kok", "kok-IN", "ko-KR", _
    "ky", "ky-KG", "lt", "lt-LT", "lv", "lv-LV", _
    "mk", "mk-MK", "mn", "mn-MN", "mr", "mr-IN", _
    "ms", "ms-BN", "ms-MY", "nb-NO", "nl", "nl-BE", _
    "nl-NL", "nn-NO", "no", "pa", "pa-IN", "pl", _
    "pl-PL", "pt", "pt-BR", "pt-PT", "ro", "ro-RO", _
    "ru", "ru-RU", "sa", "sa-IN", "sk", "sk-SK", _
    "sl", "sl-SI", "sq", "sq-AL", "sr", "sr-Cyrl-CS", _
    "sr-Latn-CS", "sv", "sv-FI", "sv-SE", "sw", "sw-KE", _
    "syr", "syr-SY", "ta", "ta-IN", "te", "te-IN", _
    "th", "th-TH", "tr", "tr-TR", "tt", "tt-RU", _
    "uk", "uk-UA", "ur", "ur-PK", "uz", "uz-Cyrl-UZ", _
    "uz-Latn-UZ", "vi", "vi-VN", "zh-CHS", "zh-CHT", "zh-CN", _
    "zh-HK", "zh-MO", "zh-SG", "zh-TW"}

    ''' <summary>
    ''' Initializes a new instance of the <see cref="TimePickerStudio"/> class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Handles the Loaded event of the TimePickerStudio control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    Private Sub TimePickerStudio_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Loaded
        ' init
        cmbPopupSelectionMode.ItemsSource = GetType(PopupTimeSelectionMode) _
        .GetMembers() _
        .ToList() _
        .Where(Function(m) _
                   m.DeclaringType.Equals(GetType(PopupTimeSelectionMode)) AndAlso _
                   (Not m.Name.StartsWith("_", StringComparison.Ordinal)) AndAlso _
                   (Not m.Name.EndsWith("_", StringComparison.Ordinal))) _
        .Select(Function(m) m.Name) _
        .ToList()

        Dim myItemsSource As New Dictionary(Of String, Type)()
        myItemsSource.Add("ListTimePicker", GetType(ListTimePickerPopup))
        myItemsSource.Add("RangeTimePicker", GetType(RangeTimePickerPopup))
        cmbPopup.ItemsSource = myItemsSource

        Dim myFormatSource As New Dictionary(Of String, ITimeFormat)()
        myFormatSource.Add("ShortTimeFormat", New ShortTimeFormat())
        myFormatSource.Add("LongTimeFormat", New LongTimeFormat())
        myFormatSource.Add("Custom: hh:mm:ss", New CustomTimeFormat("hh:mm:ss"))
        myFormatSource.Add("Custom: hh.mm", New CustomTimeFormat("hh.mm"))
        cmbFormat.ItemsSource = myFormatSource

        Dim myTimeParserSource As New Dictionary(Of String, TimeParser)()
        myTimeParserSource.Add("+/- hours, try +3h", New PlusMinusHourTimeParser())
        myTimeParserSource.Add("+/- minutes, try +3m", New PlusMinusMinuteTimeInputParser())
        cmbTimeParser.ItemsSource = myTimeParserSource

        ' defaults
        cmbFormat.SelectedIndex = 0
        cmbPopupSecondsInterval.SelectedIndex = 1
        cmbPopupMinutesInterval.SelectedIndex = 3
        cmbPopupSelectionMode.SelectedIndex = cmbPopupSelectionMode.Items.ToList().IndexOf(tp.PopupTimeSelectionMode.ToString())
        cmbPopup.SelectedIndex = 0

        Dim cultures As New List(Of CultureInfo)()

        ' work through long list of cultures and check if it is actually 
        ' allowed in this configuration.
        For Each cultureName As String In _cultureNames
            Try
                Dim c As New CultureInfo(cultureName)
                cultures.Add(c)
            Catch generatedExceptionName As ArgumentException
            End Try
        Next

        cmbCultures.ItemsSource = cultures
        ' preselect current culture.
        cmbCultures.SelectedItem = cultures.FirstOrDefault(Function(info) info.Name = tp.ActualCulture.Name)
    End Sub

    ''' <summary>
    ''' Called when Minimum ComboBox is changed.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The instance containing the event data.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub MinimumChanged(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of DateTime?))
        tp.Minimum = e.NewValue
    End Sub

    ''' <summary>
    ''' Called when Maximum ComboBox is changed.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The instance containing the event data.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub MaximumChanged(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of DateTime?))
        tp.Maximum = e.NewValue
    End Sub

    ''' <summary>
    ''' Called when Popup ComboBox is changed.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub PopupChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        tp.Popup = TryCast(Activator.CreateInstance((CType(cmbPopup.SelectedItem, KeyValuePair(Of String, Type))).Value), TimePickerPopup)
    End Sub

    ''' <summary>
    ''' Called when PopupSecondsInterval ComboBox is changed.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub PopupSecondsIntervalChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        tp.PopupSecondsInterval = CInt(Fix(e.AddedItems(0)))
    End Sub

    ''' <summary>
    ''' Called when PopupMinutesInterval ComboBox is changed.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub PopupMinutesIntervalChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        tp.PopupMinutesInterval = CInt(Fix(e.AddedItems(0)))
    End Sub

    ''' <summary>
    ''' Called when the PopupSelectionMode ComboBox is changed.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub PopupSelectionModeChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        Try
            tp.PopupTimeSelectionMode = CType([Enum].Parse(GetType(PopupTimeSelectionMode), e.AddedItems(0).ToString(), False), PopupTimeSelectionMode)
        Catch ex As ArgumentOutOfRangeException
            Dispatcher.BeginInvoke(New SelectionChangedEventHandler(AddressOf AsyncPopupSelectionChanged), sender, e)
        End Try
    End Sub

    ''' <summary>
    ''' Called asynchronously when the PopupSelectionMode ComboBox is changed.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
    Private Sub AsyncPopupSelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        cmbPopupSelectionMode.SelectedIndex = cmbPopupSelectionMode.Items.ToList().IndexOf(e.RemovedItems(0))
    End Sub

    ''' <summary>
    ''' Called when Culture ComboBox has changed.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> 
    ''' instance containing the event data.</param>
    <SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification:="Wish to catch all."), SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub CultureChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If cmbCultures.SelectedItem IsNot Nothing Then
            tp.Culture = DirectCast(cmbCultures.SelectedItem, CultureInfo)
        End If
    End Sub

    ''' <summary>
    ''' Called when Format ComboBox has changed.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> 
    ''' instance containing the event data.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub FormatChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        tp.Format = (CType(cmbFormat.SelectedItem, KeyValuePair(Of String, ITimeFormat))).Value
    End Sub

    ''' <summary>
    ''' Called when Timeparsers ComboBox is changed.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> 
    ''' instance containing the event data.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub TimeparserChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        Dim myTimeParserCollection As New TimeParserCollection()
        myTimeParserCollection.Add((CType(cmbTimeParser.SelectedItem, KeyValuePair(Of String, TimeParser))).Value)
        tp.TimeParsers = myTimeParserCollection
    End Sub
End Class
