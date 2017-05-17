' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.Windows.Controls
Imports System.ComponentModel

''' <summary>
''' The NestedHierarchicalDataTemplate sample page shows how to nest
''' HierarchicalDataTemplate templates with each HierarchicalDataTemplate
''' having its own unique template.
''' </summary>
<Sample("Nested HierarchicalDataTemplate", DifficultyLevel.Scenario, "TreeView")> _
Partial Public Class NestedHierarchicalDataTemplateSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the
    ''' NestedHierarchicalDataTemplateSample class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()
        ArtistTree.ItemsSource = Artist.AllArtists
    End Sub
End Class
