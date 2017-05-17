' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.ComponentModel
Imports System.Globalization

''' <summary>
''' Sample page demonstrating navigation using Pages and a Frame.
''' </summary>
<Sample("(2)Mapping URIs", DifficultyLevel.Intermediate, "Navigation")> _
Partial Public Class MappingNavigationSample
    Inherits UserControl

    ''' <summary>
    ''' Initializes a NavigationSample.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Causes navigation when a button is clicked.
    ''' </summary>
    ''' <param name="sender">The clicked button.</param>
    ''' <param name="e">Event arguments.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Used by event defined in Xaml.")> _
    Private Sub Button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        navFrame.Navigate(New Uri(String.Format(CultureInfo.CurrentCulture, "/Query/{0}/{1}/{2}", Uri.EscapeDataString(x.Text), Uri.EscapeDataString(y.Text), Uri.EscapeDataString(z.Text)), UriKind.Relative))
    End Sub

    ''' <summary>
    ''' Causes navigation when a button is clicked.
    ''' </summary>
    ''' <param name="sender">The clicked button.</param>
    ''' <param name="e">Event arguments.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Used by event defined in Xaml.")> _
    Private Sub Button_Click_Page(ByVal sender As Object, ByVal e As RoutedEventArgs)
        navFrame.Navigate(New Uri(DirectCast(sender, Button).Tag.ToString(), UriKind.Relative))
    End Sub
End Class