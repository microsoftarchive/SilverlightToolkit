' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.ComponentModel
Imports System.Globalization

''' <summary>
''' Sample page demonstrating navigation with mapped URIs.
''' </summary>
<Sample("(1)Using a Query String", DifficultyLevel.Intermediate, "Navigation")> _
Public Class QueryNavigationSample
    Inherits UserControl

    ''' <summary>
    ''' Initializes a QueryNavigationSample.
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
        navFrame.Navigate(New Uri(String.Format(CultureInfo.CurrentCulture, "/Navigation/QueryPage.xaml?x={0}&y={1}&z={2}", Uri.EscapeDataString(x.Text), Uri.EscapeDataString(y.Text), Uri.EscapeDataString(z.Text)), UriKind.Relative))
    End Sub
End Class
