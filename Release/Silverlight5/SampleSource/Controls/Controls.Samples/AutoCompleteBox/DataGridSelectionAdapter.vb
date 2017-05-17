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
Imports System.Windows.Automation.Peers
Imports System.Windows.Controls
Imports System.Windows.Input

''' <summary>
''' An implementation of ISelectionAdapter for the DataGrid control. This 
''' adapter, unlike the standard SelectorSelectionAdapter, actually derives 
''' directly from DataGrid.
''' </summary>
Public Class DataGridSelectionAdapter
    Inherits DataGrid
    Implements ISelectionAdapter
    ''' <summary>
    ''' Gets or sets a value indicating whether the selection should be 
    ''' ignored. Since the DataGrid automatically selects the first row 
    ''' whenever the data changes, this simple implementation only works 
    ''' with key navigation and mouse clicks. This prevents the text box 
    ''' of the AutoCompleteBox control from being updated continuously.
    ''' </summary>
    Private privateIgnoreAnySelection As Boolean
    Private Property IgnoreAnySelection() As Boolean
        Get
            Return privateIgnoreAnySelection
        End Get
        Set(ByVal value As Boolean)
            privateIgnoreAnySelection = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether the selection change event 
    ''' should not be fired.
    ''' </summary>
    Private privateIgnoringSelectionChanged As Boolean
    Private Property IgnoringSelectionChanged() As Boolean
        Get
            Return privateIgnoringSelectionChanged
        End Get
        Set(ByVal value As Boolean)
            privateIgnoringSelectionChanged = value
        End Set
    End Property

    ''' <summary>
    ''' Occurs when the currently selected item changes.
    ''' </summary>
    Public Shadows Event SelectionChanged As SelectionChangedEventHandler Implements ISelectionAdapter.SelectionChanged

    ''' <summary>
    ''' An event that indicates that a selection is complete and has been 
    ''' made, effectively a commit action.
    ''' </summary>
    Public Event Commit As RoutedEventHandler Implements ISelectionAdapter.Commit

    ''' <summary>
    ''' An event that indicates that the selection operation has been 
    ''' canceled.
    ''' </summary>
    Public Event Cancel As RoutedEventHandler Implements ISelectionAdapter.Cancel

    ''' <summary>
    ''' Gets or sets the selected item through the adapter.
    ''' </summary>
    Public Shadows Property SelectedItem() As Object Implements ISelectionAdapter.SelectedItem
        Get
            Return MyBase.SelectedItem
        End Get

        Set(ByVal value As Object)
            IgnoringSelectionChanged = True
            MyBase.SelectedItem = value
            IgnoringSelectionChanged = False
        End Set
    End Property

    ''' <summary>
    ''' Handles the mouse left button up event on the selector control.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub OnSelectorMouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs) Handles Me.MouseLeftButtonUp
        IgnoreAnySelection = False

        OnSelectionChanged(Me, Nothing)
        OnCommit(Me, New RoutedEventArgs())
    End Sub

    ''' <summary>
    ''' Handles the SelectionChanged event on the Selector control.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The selection changed event data.</param>
    Private Shadows Sub OnSelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs) Handles MyBase.SelectionChanged
        If IgnoringSelectionChanged Then
            Return
        End If

        If IgnoreAnySelection Then
            Return
        End If

        Dim handler As SelectionChangedEventHandler = Me.SelectionChangedEvent
        If handler IsNot Nothing Then
            handler(sender, e)
        End If
    End Sub

    ''' <summary>
    ''' Gets or sets the items source.
    ''' </summary>
    Public Shadows Property ItemsSource() As IEnumerable Implements ISelectionAdapter.ItemsSource
        Get
            Return MyBase.ItemsSource
        End Get

        Set(ByVal value As IEnumerable)
            If MyBase.ItemsSource IsNot Nothing Then
                Dim notify As INotifyCollectionChanged = TryCast(MyBase.ItemsSource, INotifyCollectionChanged)
                If notify IsNot Nothing Then
                    RemoveHandler notify.CollectionChanged, AddressOf OnCollectionChanged
                End If
            End If

            MyBase.ItemsSource = value

            If MyBase.ItemsSource IsNot Nothing Then
                Dim notify As INotifyCollectionChanged = TryCast(MyBase.ItemsSource, INotifyCollectionChanged)
                If notify IsNot Nothing Then
                    AddHandler notify.CollectionChanged, AddressOf OnCollectionChanged
                End If
            End If
        End Set
    End Property

    ''' <summary>
    ''' Handles the CollectionChanged event, resetting the selection 
    ''' ignore flag.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub OnCollectionChanged(ByVal sender As Object, ByVal e As NotifyCollectionChangedEventArgs)
        IgnoreAnySelection = True
    End Sub

    ''' <summary>
    ''' Gets the observable collection set by AutoCompleteBox.
    ''' </summary>
    Private ReadOnly Property Items() As ObservableCollection(Of Object)
        Get
            Return TryCast(ItemsSource, ObservableCollection(Of Object))
        End Get
    End Property

    ''' <summary>
    ''' Increment the selected index, or wrap.
    ''' </summary>
    Private Sub SelectedIndexIncrement()
        SelectedIndex = If(SelectedIndex + 1 >= Items.Count, -1, SelectedIndex + 1)
        ScrollIntoView(SelectedItem, Me.Columns(0))
    End Sub

    ''' <summary>
    ''' Decrement the SelectedIndex, or wrap around, inside the nested 
    ''' SelectionAdapter's control.
    ''' </summary>
    Private Sub SelectedIndexDecrement()
        Dim index As Integer = SelectedIndex
        If index >= 0 Then
            SelectedIndex -= 1
        ElseIf index = -1 Then
            SelectedIndex = Items.Count - 1
        End If

        ScrollIntoView(SelectedItem, Me.Columns(0))
    End Sub

    ''' <summary>
    ''' Process a key down event.
    ''' </summary>
    ''' <param name="e">The key event arguments object.</param>
    Public Sub HandleKeyDown(ByVal e As KeyEventArgs) Implements ISelectionAdapter.HandleKeyDown
        Select Case e.Key
            Case Key.Enter
                OnCommit(Me, e)
                e.Handled = True

            Case Key.Up
                IgnoreAnySelection = False
                SelectedIndexDecrement()
                e.Handled = True

            Case Key.Down
                If (ModifierKeys.Alt And Keyboard.Modifiers) = ModifierKeys.None Then
                    IgnoreAnySelection = False
                    SelectedIndexIncrement()
                    e.Handled = True
                End If

            Case Key.Escape
                OnCancel(Me, e)
                e.Handled = True

            Case Else
        End Select
    End Sub

    ''' <summary>
    ''' Fires the Commit event.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub OnCommit(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim handler As RoutedEventHandler = CommitEvent
        If handler IsNot Nothing Then
            handler(sender, e)
        End If

        AfterAdapterAction()
    End Sub

    ''' <summary>
    ''' Fires the Cancel event.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub OnCancel(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim handler As RoutedEventHandler = CancelEvent
        If handler IsNot Nothing Then
            handler(sender, e)
        End If

        AfterAdapterAction()
    End Sub

    ''' <summary>
    ''' Change the selection after the actions are complete.
    ''' </summary>
    Private Sub AfterAdapterAction()
        IgnoringSelectionChanged = True
        SelectedItem = Nothing
        SelectedIndex = -1
        IgnoringSelectionChanged = False

        ' Reset, to ignore any future changes
        IgnoreAnySelection = True
    End Sub

    ''' <summary>
    ''' Initializes a new instance of a DataGridAutomationPeer.
    ''' </summary>
    ''' <returns>Returns a new DataGridAutomationPeer.</returns>
    Public Function CreateAutomationPeer() As AutomationPeer Implements ISelectionAdapter.CreateAutomationPeer
        Return New DataGridAutomationPeer(Me)
    End Function
End Class
