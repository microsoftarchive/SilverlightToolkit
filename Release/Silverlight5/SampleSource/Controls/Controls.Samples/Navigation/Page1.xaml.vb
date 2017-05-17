' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Linq

''' <summary>
''' A Page that displays an image to which a frame can navigate.
''' </summary>
Public Class Page1
    Inherits Page

    ''' <summary>
    ''' Initializes a Page1.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()
        DataContext = Photograph.GetPhotographs().First().Image
    End Sub
End Class
