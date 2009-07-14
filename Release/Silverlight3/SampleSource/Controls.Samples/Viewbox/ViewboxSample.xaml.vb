' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics.CodeAnalysis
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating the Viewbox.
''' </summary>
<SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId:="Viewbox", Justification:="Name of the control"), Sample("Viewbox", DifficultyLevel.Basic), Category("Viewbox")> _
Partial Public Class ViewboxSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the ViewboxSample class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()

        ' Add this line to validate namescope works for Viewbox.
        childButton.Content = "Content"
    End Sub

    ''' <summary>
    ''' Update the interactive Viewbox demo when any property changes.
    ''' </summary>
    Private Sub UpdateInteractiveDemo(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles InteractiveHorizontalSlider.ValueChanged, _
        InteractiveVerticalSlider.ValueChanged, StretchNone.Checked, StretchFill.Checked, StretchUniform.Checked, StretchUniformToFill.Checked, _
        StretchDirectionUpOnly.Checked, StretchDirectionDownOnly.Checked, StretchDirectionBoth.Checked

        InteractiveViewbox.Width = InteractiveContainer.ActualWidth * InteractiveHorizontalSlider.Value / 100.0
        InteractiveWidthIndicator.Width = InteractiveViewbox.Width
        InteractiveViewbox.Height = InteractiveContainer.ActualHeight * InteractiveVerticalSlider.Value / 100.0
        InteractiveHeightIndicator.Height = InteractiveViewbox.Height
        InteractiveViewbox.Stretch = If((StretchFill.IsChecked = True), Stretch.Fill, _
                                        If((StretchUniform.IsChecked = True), Stretch.Uniform, _
                                           If((StretchUniformToFill.IsChecked = True), Stretch.UniformToFill, _
                                              Stretch.None)))
        InteractiveViewbox.StretchDirection = If((StretchDirectionUpOnly.IsChecked = True), StretchDirection.UpOnly, _
                                                 If((StretchDirectionDownOnly.IsChecked = True), StretchDirection.DownOnly, _
                                                    StretchDirection.Both))
    End Sub
End Class