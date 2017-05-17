' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating the DataGrid.
''' </summary>
<Sample("(0)DataGrid", DifficultyLevel.Basic, "DataGrid")> _
Public Class DataGridSample
    Inherits UserControl

    ''' <summary>
    ''' Initializes a DataGridSample.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()
        DataContext = New CustomerCollection()
    End Sub
End Class
