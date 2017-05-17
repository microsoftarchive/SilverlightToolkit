' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel

''' <summary>
''' Represents a holiday to be marked on the GlobalCalendar that occurs on
''' the exact date every year.
''' </summary>
Partial Public Class ExactHoliday
    Inherits Holiday

    ''' <summary>
    ''' Gets or sets the date of the holiday.
    ''' </summary>
    Private _date As Date
    <TypeConverter(GetType(DateTimeTypeConverter))> _
    Public Property [Date]() As Date
        Get
            Return _date
        End Get
        Set(ByVal value As Date)
            _date = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether the holiday occurs annually
    ''' on the same date.
    ''' </summary>
    Private _isAnnual As Boolean
    Public Property IsAnnual() As Boolean
        Get
            Return _isAnnual
        End Get
        Set(ByVal value As Boolean)
            _isAnnual = value
        End Set
    End Property

    ''' <summary>
    ''' Initializes a new instance of the ExactHoliday class.
    ''' </summary>
    Sub New()
        IsAnnual = True
    End Sub

    ''' <summary>
    ''' Determine if this holiday falls on a specific date.
    ''' </summary>
    ''' <param name="day">The date to check.</param>
    ''' <returns>
    ''' A value indicating whether this holiday falls on a specific date.
    ''' </returns>
    Public Overrides Function FallsOn(ByVal day As Date) As Boolean
        If IsAnnual Then
            Return day.Day = [Date].Day AndAlso day.Month = [Date].Month
        Else
            Return day = [Date]
        End If
    End Function
End Class