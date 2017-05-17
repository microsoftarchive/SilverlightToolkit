' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Windows.Data
Imports System.ComponentModel

''' <summary>
''' Sample page that demonstrates using a DataGrid, DataForm, and DataPager
''' to create a paged Master-Details view.
<Sample("Master-Details with DataGrid and DataForm", DifficultyLevel.Scenario, "DataGrid")> _
Public Class DataGridMasterDetailSample
    Inherits UserControl

    ''' <summary>
    ''' Initializes a DataGridMasterDetailsSample.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
        Dim pcv As PagedCollectionView = New PagedCollectionView(Contact.People)
        DataContext = pcv
    End Sub
End Class
