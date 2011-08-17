' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Linq

Public Module Extensions
    ''' <summary>
    ''' Return a random item from a list.
    ''' </summary>
    ''' <typeparam name="T">The item type.</typeparam>
    ''' <param name="rnd">The Random instance.</param>
    ''' <param name="list">The list to choose from.</param>
    ''' <returns>A randomly selected item from the list.</returns>
    <Runtime.CompilerServices.Extension()>
    Public Function [Next](Of T)(ByVal rnd As Random, ByVal list As IList(Of T)) As T
        Return list(rnd.Next(list.Count))
    End Function
End Module


''' <summary>
''' A class used to expose the Key property on a dynamically-created Linq grouping.
''' The grouping will be generated as an internal class, so the Key property will not
''' otherwise be available to databind.
''' </summary>
''' <typeparam name="TKey">The type of the key.</typeparam>
''' <typeparam name="TElement">The type of the items.</typeparam>
Public Class PublicGrouping(Of TKey, TElement)
    Implements Linq.IGrouping(Of TKey, TElement)
    Private ReadOnly _internalGrouping As Linq.IGrouping(Of TKey, TElement)

    Public Sub New(ByVal internalGrouping As Linq.IGrouping(Of TKey, TElement))
        _internalGrouping = internalGrouping
    End Sub

    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim that = TryCast(obj, PublicGrouping(Of TKey, TElement))

        Return (that IsNot Nothing) AndAlso (Me.Key.Equals(that.Key))
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Key.GetHashCode()
    End Function

#Region "IGrouping<TKey,TElement> Members"

    Public ReadOnly Property Key As TKey Implements Linq.IGrouping(Of TKey, TElement).Key
        Get
            Return _internalGrouping.Key
        End Get
    End Property

#End Region

#Region "IEnumerable<TElement> Members"

    Public Function GetEnumerator() As IEnumerator(Of TElement) Implements Collections.Generic.IEnumerable(Of TElement).GetEnumerator
        Return _internalGrouping.GetEnumerator()
    End Function

#End Region

#Region "IEnumerable Members"

    Private Function IEnumerable_GetEnumerator() As Collections.IEnumerator Implements Collections.IEnumerable.GetEnumerator
        Return _internalGrouping.GetEnumerator()
    End Function

#End Region


End Class


Public Class CommandButton
    Inherits Button
    Public Property Command As ICommand
        Get
            Return CType(GetValue(CommandProperty), ICommand)
        End Get
        Set(ByVal value As ICommand)
            SetValue(CommandProperty, value)
        End Set
    End Property

    Public Shared ReadOnly CommandProperty As DependencyProperty =
        DependencyProperty.Register("Command", GetType(ICommand), GetType(CommandButton),
                                    New PropertyMetadata(AddressOf OnCommandChanged))

    Private Shared Sub OnCommandChanged(ByVal obj As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        CType(obj, CommandButton).OnCommandChanged(e)
    End Sub

    Public Property CommandParameter As Object
        Get
            Return CObj(GetValue(CommandParameterProperty))
        End Get
        Set(ByVal value As Object)
            SetValue(CommandParameterProperty, value)
        End Set
    End Property

    Public Shared ReadOnly CommandParameterProperty As DependencyProperty =
        DependencyProperty.Register("CommandParameter", GetType(Object), GetType(CommandButton),
                                    New PropertyMetadata(AddressOf OnCommandParameterChanged))

    Private Shared Sub OnCommandParameterChanged(ByVal obj As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        CType(obj, CommandButton).UpdateIsEnabled()
    End Sub

    Private Sub OnCommandChanged(ByVal e As DependencyPropertyChangedEventArgs)
        If e.OldValue IsNot Nothing Then
            Dim command = TryCast(e.OldValue, ICommand)
            If command IsNot Nothing Then
                RemoveHandler command.CanExecuteChanged, AddressOf CommandCanExecuteChanged
            End If
        End If

        If e.NewValue IsNot Nothing Then
            Dim command = TryCast(e.NewValue, ICommand)
            If command IsNot Nothing Then
                AddHandler command.CanExecuteChanged, AddressOf CommandCanExecuteChanged
            End If
        End If

        UpdateIsEnabled()
    End Sub

    Private Sub CommandCanExecuteChanged(ByVal sender As Object, ByVal e As EventArgs)
        UpdateIsEnabled()
    End Sub

    Private Sub UpdateIsEnabled()
        IsEnabled = If(Command IsNot Nothing, Command.CanExecute(CommandParameter), False)
    End Sub

    Protected Overrides Sub OnClick()
        MyBase.OnClick()

        If Command IsNot Nothing Then
            Command.Execute(CommandParameter)
        End If
    End Sub
End Class

