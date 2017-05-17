' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating the WrapPanel.
''' </summary>
<Sample("WrapPanel", DifficultyLevel.Basic, "WrapPanel")> _
Partial Public Class WrapPanelSample
    Inherits UserControl
    ''' <summary>
    ''' Gets or sets a value indicating whether the demonstration has
    ''' already been loaded.
    ''' </summary>
    Private privateIsLoaded As Boolean
    Private Overloads Property IsLoaded() As Boolean
        Get
            Return privateIsLoaded
        End Get
        Set(ByVal value As Boolean)
            privateIsLoaded = value
        End Set
    End Property

    ''' <summary>
    ''' Initializes a new instance of the WrapPanelSample class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Load the demonstration.
    ''' </summary>
    ''' <param name="sender">Sample page.</param>
    ''' <param name="e">Event arguments.</param>
    Private Sub OnLoad(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Loaded
        ' Don't repopulate if the page has already been loaded.
        If IsLoaded Then
            Return
        End If
        IsLoaded = True

        ' Generate the text to wrap
        Dim lorem As String = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Suspendisse sed tellus non sapien laoreet accumsan. Phasellus rhoncus imperdiet pede. Morbi semper ipsum at leo. Nullam elit mi, dignissim et, vestibulum ut, laoreet quis, velit. Nulla aliquet risus sed arcu. Nunc vitae tortor in lectus tristique iaculis. Morbi elit. Quisque euismod mollis orci. Nullam cursus interdum eros. Curabitur tristique mi non nulla. Curabitur non nisi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Nam aliquet, velit eu pretium placerat, massa lorem sollicitudin dolor, non ultricies nisi lorem bibendum enim. Pellentesque mollis egestas ipsum. Donec odio quam, tempus ut, iaculis molestie, viverra vitae, sapien."
        For i As Integer = 0 To 3
            For Each word As String In lorem.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
                ManualTextWrapping.Children.Add(New TextBlock With {.Text = word, .Margin = New Thickness(3)})
            Next word
        Next i

        ' Generate the color swatch
        Dim granularity As Integer = 50
        For r As Integer = 0 To 254 Step granularity
            For g As Integer = 0 To 254 Step granularity
                For b As Integer = 0 To 254 Step granularity
                    Swatch.Items.Add(New Rectangle With _
                    { _
                        .Width = 20, _
                        .Height = 20, _
                        .Margin = New Thickness(5), _
                        .Stroke = New SolidColorBrush(Colors.Black), _
                        .StrokeThickness = 1, _
                        .Fill = New SolidColorBrush(Color.FromArgb(255, CByte(r), CByte(g), CByte(b))) _
                    })
                Next b
            Next g
        Next r

        ' Set the thumbnails
        Thumbnails.ItemsSource = Photograph.GetPhotographs().OrderBy(Function(p) p.Name)
    End Sub

    ''' <summary>
    ''' Update the manual text layout sample to match the CheckBox.
    ''' </summary>
    ''' <param name="sender">The CheckBox that was changed.</param>
    ''' <param name="e">Event arguments.</param>
    Private Sub OnCheckChanged(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles chkHorizontal.Checked, chkHorizontal.Unchecked
        If chkHorizontal.IsChecked = True Then
            ManualTextWrapping.Height = Double.NaN
            ManualTextWrapping.Orientation = Orientation.Horizontal
        Else
            ManualTextWrapping.Height = 600
            ManualTextWrapping.Orientation = Orientation.Vertical
        End If
    End Sub
End Class