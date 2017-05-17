' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.Windows.Controls
Imports System.ComponentModel

''' <summary>
''' Sample demonstrating the TreeView restyled as an organizational chart.
''' </summary>
<Sample("Organizational Chart", DifficultyLevel.Scenario, "TreeView")> _
Partial Public Class OrganizationalChartSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the OrganizationalChartSample class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()
        DepartmentTree.ItemsSource = Department.AllDepartments
    End Sub
End Class
