' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Windows.Data
Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating the DataPager.
''' </summary>
<Sample("DataPager", DifficultyLevel.Basic, "DataPager")> _
Public Class DataPagerSample
    Inherits UserControl

    ''' <summary>
    ''' Initializes a DataPagerSample.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
        Dim pcv As PagedCollectionView = New PagedCollectionView(Airport.SampleAirports)
        pcv.PageSize = 6
        DataContext = pcv
    End Sub

End Class
