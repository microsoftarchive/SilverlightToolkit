' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports System.Windows
Imports System.Windows.Controls
Imports System.ComponentModel

''' <summary>
''' Sample demonstrating TreeView selection.
''' </summary>
<Sample("(2)Selection", DifficultyLevel.Basic, "TreeView")> _
Partial Public Class TreeViewSelectionSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the TreeViewSelectionSample class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()

        ' Fill the tree with data
        TreeOfLife.ItemsSource = Taxonomy.Life
    End Sub

    ''' <summary>
    ''' Handle the TreeView.SelectedItemChanged event.
    ''' </summary>
    ''' <param name="sender">The TreeView.</param>
    ''' <param name="e">Event arguments.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="The event handler is declared in XAML.")> _
    Private Sub OnSelectedItemChanged(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of Object))
        Dim view As TreeView = TryCast(sender, TreeView)
        SelectedItem.Content = view.SelectedItem
        SelectedValue.Content = view.SelectedValue
    End Sub
End Class
