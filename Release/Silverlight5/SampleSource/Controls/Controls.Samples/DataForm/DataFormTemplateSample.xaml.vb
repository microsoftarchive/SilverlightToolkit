' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Collections.ObjectModel
Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating the DataForm with templates.
''' </summary>
<Sample("Template-driven DataForm", DifficultyLevel.Intermediate, "DataForm")> _
Partial Public Class DataFormTemplateSample
    Inherits UserControl

    ''' <summary>
    ''' Initializes a DataFormTemplateSample.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()
        DataContext = Contact.People
    End Sub
End Class