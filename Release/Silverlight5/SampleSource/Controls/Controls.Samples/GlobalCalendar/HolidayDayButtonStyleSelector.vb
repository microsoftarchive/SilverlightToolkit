' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.ObjectModel
Imports System.Linq
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives


''' <summary>
''' Provides a way to apply different styles for holidays and normal days.
''' </summary>
Partial Public Class HolidayDayButtonStyleSelector
    Inherits CalendarDayButtonStyleSelector

    ''' <summary>
    ''' Gets or sets the style for normal days.
    ''' </summary>
    Private _normalStyle As Style
    Public Property NormalStyle() As Style
        Get
            Return _normalStyle
        End Get
        Set(ByVal value As Style)
            _normalStyle = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the style for holidays.
    ''' </summary>
    Private _holidayStyle As Style
    Public Property HolidayStyle() As Style
        Get
            Return _holidayStyle
        End Get
        Set(ByVal value As Style)
            _holidayStyle = value
        End Set
    End Property

    ''' <summary>
    ''' Gets a collection of Holidays.
    ''' </summary>
    Private _holidays As Collection(Of Holiday)
    Public ReadOnly Property Holidays() As Collection(Of Holiday)
        Get
            Return _holidays
        End Get
    End Property

    ''' <summary>
    ''' Initializes a new instance of the HolidayDayButtonStyleSelector
    ''' class.
    ''' </summary>
    Sub New()
        _holidays = New Collection(Of Holiday)()
    End Sub

    ''' <summary>
    ''' Returns a GlobalCalendarDayButton Style based on whether the day is
    ''' a holiday.
    ''' </summary>
    ''' <param name="day">The day to select a Style for.</param>
    ''' <param name="container">The GlobalCalendarDayButton.</param>
    ''' <returns>A Style for the GlobalCalendarDayButton.</returns>
    Public Overrides Function SelectStyle(ByVal day As Date, ByVal container As GlobalCalendarDayButton) As Style
        ' Get the current holiday
        Dim holiday As Holiday = Nothing
        For Each h As Holiday In Holidays
            If h.FallsOn(day) Then
                holiday = h
                Exit For
            End If
        Next

        ' Use the Holiday.Title as the Tooltip
        Dim title As String = Nothing
        If holiday IsNot Nothing Then
            title = holiday.Title
        End If
        ToolTipService.SetToolTip(container, title)

        If holiday IsNot Nothing Then
            Return HolidayStyle
        Else
            Return NormalStyle
        End If
    End Function
End Class