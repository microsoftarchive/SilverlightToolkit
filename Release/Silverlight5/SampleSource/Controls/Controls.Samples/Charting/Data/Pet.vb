' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.ComponentModel

''' <summary>
''' Class representing a species of pet for use by Chart samples.
''' </summary>
Public Class Pet
    Implements INotifyPropertyChanged
    ''' <summary>
    ''' Gets or sets the species of the Pet.
    ''' </summary>
    Private privateSpecies As String
    Public Property Species() As String
        Get
            Return privateSpecies
        End Get
        Set(ByVal value As String)
            privateSpecies = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the number of Pets.
    ''' </summary>
    Public Property Count() As Integer
        Get
            Return _count
        End Get
        Set(ByVal value As Integer)
            _count = value
            OnPropertyChanged("Count")
        End Set
    End Property

    ''' <summary>
    ''' Stores the pet count.
    ''' </summary>
    Private _count As Integer

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
