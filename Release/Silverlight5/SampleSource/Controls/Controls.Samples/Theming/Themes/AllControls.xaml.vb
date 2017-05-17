' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics.CodeAnalysis
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Controls.Theming
Imports System.Collections.Generic
Imports System.Windows.Controls.DataVisualization.Charting
Imports System.Collections
Imports System.Linq
Imports System.Windows.Data
Imports System.Windows.Controls.Samples

''' <summary>
''' A user control with examples of every control to demonstrate themes.
''' </summary>
Partial Public Class AllControls
    Inherits UserControl

    ''' <summary>
    ''' Initializes a new instance of the AllControls class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()

        SampleDataGrid.ItemsSource = Employee.Executives
        SampleAutoComplete.ItemsSource = Catalog.VacationMediaItems

        Dim pcv As New PagedCollectionView(Employee.Executives)
        pcv.PageSize = 1
        DataContext = pcv

    End Sub

End Class