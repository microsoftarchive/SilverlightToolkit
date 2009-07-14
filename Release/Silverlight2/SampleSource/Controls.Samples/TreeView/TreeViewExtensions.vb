' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Controls

''' <summary>
''' TreeView and TreeViewItem extension methods.
''' </summary>
Public Module TreeViewExtensions
    ''' <summary>
    ''' Expand all the items in a TreeView.
    ''' </summary>
    ''' <param name="view">The TreeView.</param>
    <System.Runtime.CompilerServices.Extension()> _
    Public Sub ExpandAll(ByVal view As TreeView)
        If view Is Nothing OrElse view.Items Is Nothing Then
            Return
        End If

        For i As Integer = 0 To view.Items.Count - 1
            Dim item As TreeViewItem = TryCast(view.ItemContainerGenerator.ContainerFromIndex(i), TreeViewItem)
            ExpandAll(item)
        Next i
    End Sub

    ''' <summary>
    ''' Expand all the items in a TreeViewItem.
    ''' </summary>
    ''' <param name="item">The TreeViewItem.</param>
    <System.Runtime.CompilerServices.Extension()> _
    Public Sub ExpandAll(ByVal item As TreeViewItem)
        If item Is Nothing Then
            Return
        End If

        Dim justExpanded As Boolean = Not item.IsExpanded
        item.IsExpanded = True
        If justExpanded Then
            item.Dispatcher.BeginInvoke(New Action(Of TreeViewItem)(AddressOf ExpandAll_Async), item)
        Else
            For i As Integer = 0 To item.Items.Count - 1
                Dim child As TreeViewItem = TryCast(item.ItemContainerGenerator.ContainerFromIndex(i), TreeViewItem)
                ExpandAll(child)
            Next i
        End If
    End Sub

    ''' <summary>
    ''' Recursively expands the specified TreeViewItem
    ''' </summary>
    ''' <param name="chart">The TreeViewItem to expand.</param>
    Private Sub ExpandAll_Async(ByVal item As TreeViewItem)
        ExpandAll(item)
    End Sub
End Module
