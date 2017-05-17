' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System

''' <summary>
''' Class representing a month, used in the DomainUpDown sample.
''' </summary>
Public Class Month
    ''' <summary>
    ''' Gets or sets the friendly name representation of this month.
    ''' </summary>
    ''' <value>The name of the friendly.</value>
    Private privateFriendlyName As String
    Public Property FriendlyName() As String
        Get
            Return privateFriendlyName
        End Get
        Set(ByVal value As String)
            privateFriendlyName = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the month number.
    ''' </summary>
    ''' <value>The number.</value>
    Private privateNumber As Integer
    Public Property Number() As Integer
        Get
            Return privateNumber
        End Get
        Set(ByVal value As Integer)
            privateNumber = value
        End Set
    End Property

    ''' <summary>
    ''' Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    ''' </summary>
    ''' <returns>
    ''' A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    ''' </returns>
    Public Overrides Function ToString() As String
        Return FriendlyName
    End Function
End Class
