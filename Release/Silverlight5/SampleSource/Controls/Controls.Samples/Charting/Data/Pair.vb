' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.ComponentModel

''' <summary>
''' Class representing an untyped pair of values.
''' </summary>
Public Class Pair
    Implements INotifyPropertyChanged
    ''' <summary>
    ''' Gets or sets the first value.
    ''' </summary>
    Public Property First() As Object
        Get
            Return _first
        End Get
        Set(ByVal value As Object)
            _first = value
            OnPropertyChanged("First")
        End Set
    End Property

    ''' <summary>
    ''' Stores the value of the First property.
    ''' </summary>
    Private _first As Object

    ''' <summary>
    ''' Gets or sets the second value.
    ''' </summary>
    Public Property Second() As Object
        Get
            Return _second
        End Get
        Set(ByVal value As Object)
            _second = value
            OnPropertyChanged("Second")
        End Set
    End Property

    ''' <summary>
    ''' Stores the value of the Second property.
    ''' </summary>
    Private _second As Object

    ''' <summary>
    ''' Implements the INotifyPropertyChanged interface.
    ''' </summary>
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    ''' <summary>
    ''' Fires the PropertyChanged event.
    ''' </summary>
    ''' <param name="propertyName">Name of the property that changed.</param>
    Private Sub OnPropertyChanged(ByVal propertyName As String)
        Dim handler As PropertyChangedEventHandler = PropertyChangedEvent
        If Nothing IsNot handler Then
            handler.Invoke(Me, New PropertyChangedEventArgs(propertyName))
        End If
    End Sub
End Class
