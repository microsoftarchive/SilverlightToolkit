' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics.CodeAnalysis
Imports System.Windows
Imports System.Windows.Controls
Imports System.ComponentModel

''' <summary>
''' Sample demonstrating TreeView user interaction.
''' </summary>
<Sample("(1)Interaction", DifficultyLevel.Basic, "TreeView")> _
Partial Public Class TreeViewInteractionSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the TreeViewInteractionSample class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()

        ' Fill the tree with data
        TreeOfLife.ItemsSource = Taxonomy.Life
    End Sub

    ''' <summary>
    ''' Expand all of the TreeOfLife TreeViewItems.
    ''' </summary>
    ''' <param name="sender">Expand All Button.</param>
    ''' <param name="e">Event arguments.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Attached as an event handler in XAML")> _
    Private Sub OnExpandAll(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Use the TreeViewExtensions.ExpandAll helper
        TreeOfLife.ExpandAll()
    End Sub
End Class
