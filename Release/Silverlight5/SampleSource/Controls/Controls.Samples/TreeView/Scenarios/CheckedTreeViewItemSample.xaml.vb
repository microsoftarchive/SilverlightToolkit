' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.Diagnostics.CodeAnalysis
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.ComponentModel

''' <summary>
''' Sample demonstrating CheckBoxes in a TreeView.
''' </summary>    
<Sample("(2)Using CheckBoxes", DifficultyLevel.Scenario, "TreeView")> _
Partial Public Class CheckedTreeViewItemSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the CheckedTreeViewSample class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Handle the ItemCheckbox.Click event.
    ''' </summary>
    ''' <param name="sender">The CheckBox.</param>
    ''' <param name="e">Event arguments.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Called from an event declared in XAML")> _
    Private Sub ItemCheckbox_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim item As TreeViewItem = GetParentTreeViewItem(CType(sender, DependencyObject))
        If item IsNot Nothing Then
            Dim feature As Feature = TryCast(item.DataContext, Feature)
            If feature IsNot Nothing Then
                UpdateChildrenCheckedState(feature)
                UpdateParentCheckedState(item)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Gets the parent TreeViewItem of the passed in dependancy object.
    ''' </summary>
    ''' <param name="item">Item whose parent to wish to find.</param>
    ''' <returns>
    ''' If item is a TreeViewItem then returns its parent TreeViewItem,
    ''' else returns the TreeViewItem containing the item.
    ''' </returns>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Called from an event declared in XAML")> _
    Private Shared Function GetParentTreeViewItem(ByVal item As DependencyObject) As TreeViewItem
        If item IsNot Nothing Then
            Dim parent As DependencyObject = VisualTreeHelper.GetParent(item)
            Dim parentTreeViewItem As TreeViewItem = TryCast(parent, TreeViewItem)
            Return If((parentTreeViewItem IsNot Nothing), parentTreeViewItem, GetParentTreeViewItem(parent))
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Sets the Feature bound to the item's parent to the combined
    ''' check state of all the children.
    ''' </summary>
    ''' <param name="item">Item whose parent should be adjust.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Called from an event declared in XAML")> _
    Private Shared Sub UpdateParentCheckedState(ByVal item As TreeViewItem)
        Dim parent As TreeViewItem = GetParentTreeViewItem(item)
        If parent IsNot Nothing Then
            Dim feature As Feature = TryCast(parent.DataContext, Feature)
            If feature IsNot Nothing Then
                ' Get the combined checked state of all the children,
                ' determing if they're all checked, all unchecked or a
                ' combination.
                Dim childrenCheckedState? As Boolean = feature.Subcomponents.First().ShouldInstall
                For i As Integer = 1 To feature.Subcomponents.Count() - 1
                    If Not childrenCheckedState.Equals(feature.Subcomponents(i).ShouldInstall) Then
                        childrenCheckedState = Nothing
                        Exit For
                    End If
                Next i

                ' Set the parent to the combined state of the children.
                feature.ShouldInstall = childrenCheckedState

                ' Continue up the tree updating each parent with the
                ' correct combined state.
                UpdateParentCheckedState(parent)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Sets the feature's children checked states, including subcomponents,
    ''' to match the state of feature.
    ''' </summary>
    ''' <param name="feature">Feature whose children should be set.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Called from an event declared in XAML")> _
    Private Shared Sub UpdateChildrenCheckedState(ByVal feature As Feature)
        If feature.ShouldInstall.HasValue Then
            For Each childFeature As Feature In feature.Subcomponents
                childFeature.ShouldInstall = feature.ShouldInstall
                If childFeature.Subcomponents.Count() > 0 Then
                    UpdateChildrenCheckedState(childFeature)
                End If
            Next childFeature
        End If
    End Sub
End Class
