' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel

''' <summary>
''' Represents a holiday to be marked on the GlobalCalendar.
''' </summary>
Partial Public MustInherit Class Holiday
    ''' <summary>
    ''' Gets or sets the title of the holiday.
    ''' </summary>
    Private _title As String
    Public Property Title() As String
        Get
            Return _title
        End Get
        Set(ByVal value As String)
            _title = value
        End Set
    End Property

    ''' <summary>
    ''' Initializes a new instance of the Holiday class.
    ''' </summary>
    Protected Sub New()
    End Sub

    ''' <summary>
    ''' Determine if this holiday falls on a specific date.
    ''' </summary>
    ''' <param name="day">The date to check.</param>
    ''' <returns>
    ''' A value indicating whether this holiday falls on a specific date.
    ''' </returns>
    Public MustOverride Function FallsOn(ByVal day As Date) As Boolean
End Class