' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.ComponentModel
Imports System.Windows.Data

''' <summary>
''' Sample page demonstrating the DataGrid with grouping enabled.
''' </summary>
<Sample("DataGrid with Grouping", DifficultyLevel.Basic, "DataGrid")> _
Public Class DataGridGroupingSample
    Inherits UserControl

    ''' <summary>
    ''' Initializes a DataGridGroupingSample.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()
        Dim pcv As PagedCollectionView = New PagedCollectionView(Contact.People)
        pcv.GroupDescriptions.Add(New PropertyGroupDescription("State"))
        pcv.GroupDescriptions.Add(New PropertyGroupDescription("City"))
        DataContext = pcv
    End Sub
End Class
