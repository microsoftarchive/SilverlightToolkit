' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Windows
Imports System.Windows.Controls

''' <summary>
''' Sample page demonstrating the DatePicker.
''' </summary>
<Sample("DatePicker", DifficultyLevel.Basic, "DatePicker")> _
Partial Public Class DatePickerSample
    Inherits UserControl
    ''' <summary>
    ''' Flag indicating whether to ignore the next DatePicker update.
    ''' </summary>
    Private _ignoreNextUpdate As Boolean = True

    ''' <summary>
    ''' Initializes a new instance of the DatePickerSample class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Inherited code: Requires comment.
    ''' </summary>
    ''' <param name="sender">Inherited code: Requires comment 1.</param>
    ''' <param name="e">Inherited code: Requires comment 2.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Event handler attached in XAML."), SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification:="Simplifies sample.")> _
    Private Sub OnPastDatesChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If sampleDatePicker IsNot Nothing AndAlso sampleLongDatePicker IsNot Nothing Then
            If chkPastDateSelection.IsChecked = True Then
                sampleDatePicker.BlackoutDates.Clear()
                sampleLongDatePicker.BlackoutDates.Clear()
            Else
                Try
                    sampleDatePicker.BlackoutDates.AddDatesInPast()
                    sampleLongDatePicker.BlackoutDates.AddDatesInPast()
                Catch
                    chkPastDateSelection.IsChecked = True
                End Try
            End If
        End If
    End Sub

    ''' <summary>
    ''' Handle changes to the selected date.
    ''' </summary>
    ''' <param name="sender">The DatePicker.</param>
    ''' <param name="e">Event arguments.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Event handler attached in XAML.")> _
    Private Sub OnDateSelected(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        UpdateSelectedDate(sampleDatePicker, sampleLongDatePicker)
    End Sub

    ''' <summary>
    ''' Handle changes to the selected date.
    ''' </summary>
    ''' <param name="sender">The long DatePicker.</param>
    ''' <param name="e">Event arguments.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Event handler attached in XAML.")> _
    Private Sub OnLongDateSelected(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        UpdateSelectedDate(sampleLongDatePicker, sampleDatePicker)
    End Sub

    ''' <summary>
    ''' Update the selected date.
    ''' </summary>
    ''' <param name="current">The current DatePicker.</param>
    ''' <param name="other">The other DatePicker.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Called via event hanlders attached in XAML.")> _
    Private Sub UpdateSelectedDate(ByVal current As DatePicker, ByVal other As DatePicker)
        If _ignoreNextUpdate Then
            _ignoreNextUpdate = False
            other.SelectedDate = current.SelectedDate

            If current.SelectedDate IsNot Nothing Then
                txtSelectedDate.Text = current.SelectedDate.ToString()
            Else
                txtSelectedDate.Text = ""
            End If
        Else
            _ignoreNextUpdate = True
        End If
    End Sub
End Class