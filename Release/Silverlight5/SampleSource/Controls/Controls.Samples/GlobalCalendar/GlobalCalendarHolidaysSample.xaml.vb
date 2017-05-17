' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel
Imports System.Windows
Imports System.Windows.Controls

''' <summary>
''' Sample page demonstrating the CalendarDayButtonStyleSelector with
''' holidays.
''' </summary>
<Sample("(3)Holidays", DifficultyLevel.Basic, "GlobalCalendar")> _
Partial Public Class GlobalCalendarHolidaysSample
    Inherits UserControl

    ''' <summary>
    ''' Initializes a new instance of the GlobalCalendarHolidaysSample
    ''' class.
    ''' </summary>
    Sub New()
        InitializeComponent()
    End Sub
End Class