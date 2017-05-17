' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating navigation with mapped URIs.
''' </summary>
<Sample("(0)Navigation", DifficultyLevel.Basic, "Navigation")> _
Partial Public Class NavigationSample
    Inherits UserControl

    ''' <summary>
    ''' Initializes a MappingNavigationSample.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Causes navigation when a page button is clicked.
    ''' </summary>
    ''' <param name="sender">The clicked button.</param>
    ''' <param name="e">Event arguments.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Used by event defined in Xaml.")> _
    Private Sub Button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        navFrame.Navigate(New Uri(DirectCast(sender, Button).Tag.ToString(), UriKind.Relative))
    End Sub
End Class