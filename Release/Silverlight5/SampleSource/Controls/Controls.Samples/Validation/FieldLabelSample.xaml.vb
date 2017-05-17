' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating the FieldLabel.
''' </summary>
<Sample("FieldLabel", DifficultyLevel.Intermediate)> _
<Category("Validation")> _
Public Class FieldLabelSample
    Inherits UserControl

    ''' <summary>
    ''' Initializes a FieldLabelSample.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Initializes the data after loading.
    ''' </summary>
    ''' <param name="sender">This control.</param>
    ''' <param name="e">Event arguments.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Used by event defined in Xaml.")> _
    Private Sub UserControl_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        contactGrid.DataContext = Contact.JohnDoe
    End Sub
End Class
