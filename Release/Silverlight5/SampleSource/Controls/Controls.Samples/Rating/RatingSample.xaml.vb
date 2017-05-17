' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports System.Windows.Media

''' <summary>
''' A class that demonstrates the Rating control.
''' </summary>
<Sample("(0)Rating", DifficultyLevel.Basic, "Rating")> _
Partial Public Class RatingSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the RatingSample class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Changes the foreground of the rating control to yellow.
    ''' </summary>
    ''' <param name="sender">Sender Rating.</param>
    ''' <param name="e">Event args.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Method is called in XAML.")> _
    Private Sub OnMovieValueChanged(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of Global.System.Nullable(Of Double)))
        movie.Foreground = New SolidColorBrush(Color.FromArgb(255, 255, 203, 0))
    End Sub

    ''' <summary>
    ''' Set the value of the rating control to 0.
    ''' </summary>
    ''' <param name="sender">Sender Button.</param>
    ''' <param name="e">Event args.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Method is called in XAML.")> _
    Private Sub Button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        movie.Value = 0
    End Sub
End Class