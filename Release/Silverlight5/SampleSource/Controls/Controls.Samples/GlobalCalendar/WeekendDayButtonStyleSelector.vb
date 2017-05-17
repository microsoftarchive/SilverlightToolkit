' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives

''' <summary>
''' Provides a way to apply different styles for weekdays and weekends.
''' </summary>
Partial Public Class WeekendDayButtonStyleSelector
    Inherits CalendarDayButtonStyleSelector

    ''' <summary>
    ''' Gets or sets the style for weekdays.
    ''' </summary>
    Private _weekdayStyle As Style
    Public Property WeekdayStyle() As Style
        Get
            Return _weekdayStyle
        End Get
        Set(ByVal value As Style)
            _weekdayStyle = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the style for weekend days.
    ''' </summary>
    Private _weekendStyle As Style
    Public Property WeekendStyle() As Style
        Get
            Return _weekendStyle
        End Get
        Set(ByVal value As Style)
            _weekendStyle = value
        End Set
    End Property

    ''' <summary>
    ''' Initializes a new instance of the WeekendDayButtonStyleSelector
    ''' class.
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' Returns a GlobalCalendarDayButton Style based on whether the day is
    ''' a weekday or a weekend.
    ''' </summary>
    ''' <param name="day">The day to select a Style for.</param>
    ''' <param name="container">The GlobalCalendarDayButton.</param>
    ''' <returns>A Style for the GlobalCalendarDayButton.</returns>
    Public Overrides Function SelectStyle(ByVal day As Date, ByVal container As GlobalCalendarDayButton) As Style
        Dim d As DayOfWeek = day.DayOfWeek
        If d = DayOfWeek.Saturday Or d = DayOfWeek.Sunday Then
            Return WeekendStyle
        Else
            Return WeekdayStyle
        End If
    End Function
End Class