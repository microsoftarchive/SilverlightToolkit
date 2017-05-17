' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.ComponentModel

''' <summary>
''' Sample demonstrating TreeView data-binding.
''' </summary>
<Sample("(4)Data Binding", DifficultyLevel.Basic, "TreeView")> _
Partial Public Class TreeViewDataBindingSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the TreeViewDataBindingSample class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()

        ' Fill the tree with data
        TreeOfLife.ItemsSource = Taxonomy.Life
    End Sub
End Class
