' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating the DataGrid with grouping enabled.
''' </summary>
<Sample("DataGrid with Grouping", DifficultyLevel.Basic)> _
<Category("DataGrid")> _
Public Class DataGridGroupingSample
    Inherits UserControl

    ''' <summary>
    ''' Initializes a DataGridGroupingSample.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()
        DataContext = Contact.People
    End Sub
End Class
