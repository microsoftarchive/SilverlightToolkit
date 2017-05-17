' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

''' <summary>
''' Sample ChildWindow for demonstration purposes.
''' </summary>
Partial Public Class DemoChildWindow
    Inherits ChildWindow

    ''' <summary>
    ''' Initializes a DemoChildWindow.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
        optionsStack.DataContext = Me
    End Sub

    ''' <summary>
    ''' Handles the Click event of the OK button.
    ''' </summary>
    ''' <param name="sender">OK Button.</param>
    ''' <param name="e">Event arguments.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Used by event defined in Xaml.")> _
    Private Sub OKButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        DialogResult = True
    End Sub

    ''' <summary>
    ''' Handles the Click event of the Cancel button.
    ''' </summary>
    ''' <param name="sender">Cancel button.</param>
    ''' <param name="e">Event arguments.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Used by event defined in Xaml.")> _
    Private Sub CancelButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        DialogResult = False
    End Sub
End Class