' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Linq
Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating the ChildWindow.
''' </summary>
<Sample("ChildWindow", DifficultyLevel.Basic, "ChildWindow")> _
Partial Public Class ChildWindowSample
    Inherits UserControl

    ''' <summary>
    ''' Keeps an instance of a ChildWindow that will be shown when a button is clicked.
    ''' </summary>
    Private WithEvents dcw As DemoChildWindow

    ''' <summary>
    ''' Initializes a new instance of the ChildWindowSample class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()
        dcw() = New DemoChildWindow()
        thumbs.ItemsSource = From p In Photograph.GetPhotographs() _
                             Order By p.Name _
                             Select p
        thumbs.SelectedIndex = 0
    End Sub

    ''' <summary>
    ''' Handles the "Closed" event of the ChildWindow.
    ''' </summary>
    ''' <param name="sender">Child Window.</param>
    ''' <param name="e">Event Arguments.</param>
    Private Sub Dcw_Closed(ByVal sender As Object, ByVal e As EventArgs) Handles dcw.Closed
        dialogResult.Text = dcw.DialogResult.ToString()
    End Sub

    ''' <summary>
    ''' Handles clicking the "Show" button.
    ''' </summary>
    ''' <param name="sender">Clicked Button.</param>
    ''' <param name="e">Event Arguments.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Used by event defined in Xaml.")> _
    Private Sub Button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        dcw.Title = titleText.Text
        dcw.DataContext = (From p In Photograph.GetPhotographs() _
                           Where p.Name.Equals(DirectCast(thumbs.SelectedItem, Photograph).Name) _
                           Select p).First().Image
        dcw.Show()
    End Sub
End Class