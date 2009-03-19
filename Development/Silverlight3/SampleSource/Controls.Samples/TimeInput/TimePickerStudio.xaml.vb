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
<Sample("(3)TimePicker Studio", DifficultyLevel.Intermediate), Category("TimePicker")> _
Partial Public Class TimePickerStudio
    Inherits UserControl
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
        tbCultures.Text = tp.ActualCulture.Name
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
        Try
            Dim newCulture As New CultureInfo(tbCultures.Text)
            tp.Culture = newCulture
        Catch
            tbCultures.Text = tp.ActualCulture.Name
        End Try
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