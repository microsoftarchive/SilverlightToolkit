' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel

''' <summary>
''' Class representing a vacation budget for use by Chart samples.
''' </summary>
Public Class Budget
    Implements INotifyPropertyChanged
    ''' <summary>
    ''' Gets or sets the date on which the expense was spent.
    ''' </summary>
    Private privateDate As DateTime
    Public Property [Date]() As DateTime
        Get
            Return privateDate
        End Get
        Set(ByVal value As DateTime)
            privateDate = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the species of the type of expense.
    ''' </summary>
    Private privateExpenseType As String
    Public Property ExpenseType() As String
        Get
            Return privateExpenseType
        End Get
        Set(ByVal value As String)
            privateExpenseType = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the Volume (used for bubble chart).
    ''' </summary>
    Private privateVolume As Double
    Public Property Volume() As Double
        Get
            Return privateVolume
        End Get
        Set(ByVal value As Double)
            privateVolume = value
        End Set
    End Property


    ''' <summary>
    ''' Gets or sets the expense value.
    ''' </summary>
    Public Property ExpenseValue() As Double
        Get
            Return _expenseValue
        End Get
        Set(ByVal value As Double)
            _expenseValue = value
            OnPropertyChanged("Count")
        End Set
    End Property

    ''' <summary>
    ''' Stores the expense value.
    ''' </summary>
    Private _expenseValue As Double

    ''' <summary>
    ''' Fires the PropertyChanged event.
    ''' </summary>
    ''' <param name="propertyName">Property that changed.</param>
    Private Sub OnPropertyChanged(ByVal propertyName As String)
        Dim handler As PropertyChangedEventHandler = PropertyChangedEvent
        If Nothing IsNot handler Then
            handler.Invoke(Me, New PropertyChangedEventArgs(propertyName))
        End If
    End Sub

    ''' <summary>
    ''' Implements the INotifyPropertyChanged interface.
    ''' </summary>
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
End Class