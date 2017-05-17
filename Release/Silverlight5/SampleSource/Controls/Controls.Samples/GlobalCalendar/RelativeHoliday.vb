' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel

''' <summary>
''' Represents a holiday to be marked on the GlobalCalendar that occurs on
''' the same day of the same week every year (i.e., Thanksgiving in the
''' United States falls on the fourth Thursday in November).
''' </summary>
Partial Public Class RelativeHoliday
    Inherits Holiday

    ''' <summary>
    ''' Gets or sets the month of the holiday.
    ''' </summary>
    Private _month As Integer
    Public Property Month() As Integer
        Get
            Return _month
        End Get
        Set(ByVal value As Integer)
            _month = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the day of the holiday.
    ''' </summary>
    Private _dayOfWeek As DayOfWeek
    Public Property DayOfWeek() As DayOfWeek
        Get
            Return _dayOfWeek
        End Get
        Set(ByVal value As DayOfWeek)
            _dayOfWeek = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value that specifies the nth day of week in the
    ''' month.  Negative values start from the end of the month (which is
    ''' used to specify relative holidays like the last Monday in May).
    ''' </summary>
    Private _dayOfWeekNumber As Integer
    Public Property DayOfWeekNumber() As Integer
        Get
            Return _dayOfWeekNumber
        End Get
        Set(ByVal value As Integer)
            _dayOfWeekNumber = value
        End Set
    End Property

    ''' <summary>
    ''' Initializes a new instance of the RelativeHoliday class.
    ''' </summary>
    Sub New()
    End Sub

    ''' <summary>
    ''' Determine if this holiday falls on a specific date.
    ''' </summary>
    ''' <param name="day">The date to check.</param>
    ''' <returns>
    ''' A value indicating whether this holiday falls on a specific date.
    ''' </returns>
    Public Overrides Function FallsOn(ByVal day As Date) As Boolean
        ' Short circuit on the month or day
        If day.Month <> Month Or day.DayOfWeek <> DayOfWeek Then
            Return False
        End If

        ' Trim off anything but the date
        day = New Date(day.Year, day.Month, day.Day)

        ' Loop through all of the dates in the month to count how many days
        ' are occurences of DayOfWeek.
        Dim occurences As Integer = 0
        Dim dateOccurenceNumber As Integer = 0
        Dim d As Date = New Date(day.Year, day.Month, 1)
        While d.Month = day.Month
            If d.DayOfWeek = DayOfWeek Then
                occurences = occurences + 1

                If d <= day Then
                    dateOccurenceNumber = dateOccurenceNumber + 1
                End If
            End If

            d = d.AddDays(1)
        End While

        If DayOfWeekNumber >= 0 Then
            Return DayOfWeekNumber = dateOccurenceNumber
        Else
            Return occurences + DayOfWeekNumber + 1 = dateOccurenceNumber
        End If
    End Function
End Class
