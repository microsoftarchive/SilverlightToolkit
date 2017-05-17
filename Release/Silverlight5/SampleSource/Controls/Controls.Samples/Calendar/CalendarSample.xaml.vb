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
''' Sample page demonstrating the Calendar.
''' </summary>
<Sample("Calendar", DifficultyLevel.Basic, "Calendar")> _
Partial Public Class CalendarSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the CalendarSample class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()

        ' Setting the DatePickers
        txtDisplayDate.Text = sampleCalendar.DisplayDate.ToShortDateString()
        txtSelectedDate.Text = sampleCalendar.SelectedDate.ToString()
    End Sub

    ''' <summary>
    ''' Inherited code: Requires comment.
    ''' </summary>
    ''' <param name="sender">Inherited code: Requires comment 1.</param>
    ''' <param name="e">Inherited code: Requires comment 2.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Event handler attached in XAML."), SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification:="Simplifies sample.")> _
    Private Sub OnPastDatesChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If sampleCalendar Is Nothing Then
            Return
        End If

        If CBool(chkPastDateSelection.IsChecked) Then
            sampleCalendar.BlackoutDates.Clear()
        Else
            Try
                sampleCalendar.BlackoutDates.AddDatesInPast()
            Catch
                chkPastDateSelection.IsChecked = True
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Inherited code: Requires comment.
    ''' </summary>
    ''' <param name="sender">Inherited code: Requires comment 1.</param>
    ''' <param name="e">Inherited code: Requires comment 2.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Event handler attached in XAML."), SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification:="Simplifies sample.")> _
    Private Sub OnDisplayDateStartSelected(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        If sampleCalendar Is Nothing Then
            Return
        End If

        Try
            sampleCalendar.DisplayDateStart = CType(e.AddedItems(0), Nullable(Of DateTime))
        Catch
            sampleCalendar.DisplayDateStart = Nothing
            dateStart.Text = ""
        End Try
    End Sub

    ''' <summary>
    ''' Inherited code: Requires comment.
    ''' </summary>
    ''' <param name="sender">Inherited code: Requires comment 1.</param>
    ''' <param name="e">Inherited code: Requires comment 2.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Event handler attached in XAML."), SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification:="Simplifies sample.")> _
    Private Sub OnDisplayDateEndSelected(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        If sampleCalendar Is Nothing Then
            Return
        End If

        Try
            sampleCalendar.DisplayDateEnd = CType(e.AddedItems(0), Nullable(Of DateTime))
        Catch
            sampleCalendar.DisplayDateEnd = Nothing
            dateEnd.Text = ""
        End Try
    End Sub

    ''' <summary>
    ''' Inherited code: Requires comment.
    ''' </summary>
    ''' <param name="sender">Inherited code: Requires comment 1.</param>
    ''' <param name="e">Inherited code: Requires comment 2.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Event handler attached in XAML.")> _
    Private Sub OnDisplayDateChanged(ByVal sender As Object, ByVal e As CalendarDateChangedEventArgs)
        If sampleCalendar IsNot Nothing Then
            txtDisplayDate.Text = sampleCalendar.DisplayDate.ToShortDateString()
        End If
    End Sub

    ''' <summary>
    ''' Inherited code: Requires comment.
    ''' </summary>
    ''' <param name="sender">Inherited code: Requires comment 1.</param>
    ''' <param name="e">Inherited code: Requires comment 2.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Event handler attached in XAML.")> _
    Private Sub OnSelectedDateChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        If sampleCalendar IsNot Nothing AndAlso sampleCalendar.SelectedDate IsNot Nothing Then
            txtSelectedDate.Text = sampleCalendar.SelectedDate.ToString()
        End If
    End Sub

    ''' <summary>
    ''' Inherited code: Requires comment.
    ''' </summary>
    ''' <param name="sender">Inherited code: Requires comment 1.</param>
    ''' <param name="e">Inherited code: Requires comment 2.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Event handler attached in XAML.")> _
    Private Sub OnSelectionModeChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If sampleCalendar IsNot Nothing Then
            sampleCalendar.SelectionMode = If((sender Is radioNone), CalendarSelectionMode.None, _
                                              If((sender Is radioSingleRange), CalendarSelectionMode.SingleRange, _
                                                 If((sender Is radioMultiRange), CalendarSelectionMode.MultipleRange, _
                                                    CalendarSelectionMode.SingleDate)))
        End If
    End Sub
End Class