' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating the DockPanel.
''' </summary>
<Sample("DockPanel", DifficultyLevel.Basic, "DockPanel")> _
Partial Public Class DockPanelSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the DockPanelSample class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Update the toolbar demo when a font property changes.
    ''' </summary>
    Private Sub UpdateToolbarDemo(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnBold.Checked, btnBold.Unchecked, btnItalic.Checked, btnItalic.Unchecked, _
        btnUnderline.Checked, btnUnderline.Unchecked

        UsageText.FontWeight = If((btnBold.IsChecked = True), _
                                  FontWeights.Bold, FontWeights.Normal)
        UsageText.FontStyle = If((btnItalic.IsChecked = True), _
                                 FontStyles.Italic, FontStyles.Normal)
        UsageText.TextDecorations = If((btnUnderline.IsChecked = True), _
                                       TextDecorations.Underline, Nothing)
    End Sub
End Class