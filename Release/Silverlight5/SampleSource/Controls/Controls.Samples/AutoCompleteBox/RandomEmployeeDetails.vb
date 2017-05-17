' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input

''' <summary>
''' RandomEmployeeDetails is a sample type that contains mostly random data 
''' for use in DataGrid sample scenarios.
''' </summary>
Public Class RandomEmployeeDetails
    ''' <summary>
    ''' A random number generator.
    ''' </summary>
    Private Shared RandomGenerator As New Random()

    ''' <summary>
    ''' Initializes a new instance of the RandomEmployeeDetails type. A 
    ''' random number and bool value will be generated in the constructor.
    ''' </summary>
    Public Sub New()
        RandomNumber = RandomGenerator.Next()
        RandomTrueFalse = RandomGenerator.Next(0, 2) = 1
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the RandomEmployeeDetails type.
    ''' </summary>
    ''' <param name="employee">An Employee object to read the Name from.</param>
    Public Sub New(ByVal employee As Employee)
        Me.New()
        Name = employee.DisplayName
    End Sub

    ''' <summary>
    ''' Gets or sets a name.
    ''' </summary>
    Private privateName As String
    Public Property Name() As String
        Get
            Return privateName
        End Get
        Set(ByVal value As String)
            privateName = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a random number.
    ''' </summary>
    Private privateRandomNumber As Integer
    Public Property RandomNumber() As Integer
        Get
            Return privateRandomNumber
        End Get
        Set(ByVal value As Integer)
            privateRandomNumber = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether the value is true or false.
    ''' </summary>
    Private privateRandomTrueFalse As Boolean
    Public Property RandomTrueFalse() As Boolean
        Get
            Return privateRandomTrueFalse
        End Get
        Set(ByVal value As Boolean)
            privateRandomTrueFalse = value
        End Set
    End Property
End Class
