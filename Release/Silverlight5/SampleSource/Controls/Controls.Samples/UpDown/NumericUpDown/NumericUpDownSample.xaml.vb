' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Globalization
Imports System.Windows
Imports System.Windows.Controls
Imports System.ComponentModel

''' <summary>
''' NumericUpDown sample page.
''' </summary>
<Sample("NumericUpDown", DifficultyLevel.Basic, "NumericUpDown")> _
Partial Public Class NumericUpDownSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the NumericUpDownSample class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Handle the NumericUpDown ValueChanged event
    ''' </summary>
    ''' <param name="sender">NumericUpDown.</param>
    ''' <param name="e">Event arguments.</param>
    Private Sub Nud_ValueChanged(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of Double)) Handles nud.ValueChanged
        OutputNUD()
    End Sub

    ''' <summary>
    ''' Load the demo page.
    ''' </summary>
    ''' <param name="sender">Sample page.</param>
    ''' <param name="e">Event arguments.</param>
    Private Sub OnLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Loaded
        AddHandler btnChange.Click, AddressOf ChangeSettings
    End Sub

    ''' <summary>
    ''' Change ContentControl settings (Header, HeaderTemplate, Content, ContentTemplate).
    ''' </summary>
    ''' <param name="sender">Sender Button.</param>
    ''' <param name="e">Event args.</param>
    Private Sub ChangeSettings(ByVal sender As Object, ByVal e As RoutedEventArgs)
        nud.Minimum = ParseUtility.ReadValue(tbMininum, nud.Minimum)
        nud.Maximum = ParseUtility.ReadValue(tbMaximum, nud.Maximum)
        nud.Value = ParseUtility.ReadValue(tbValue, nud.Value)
        nud.Increment = ParseUtility.ReadValue(tbIncrement, nud.Increment)
        nud.DecimalPlaces = ParseUtility.ReadValue(tbDecimalPlaces, nud.DecimalPlaces)
        nud.IsEditable = If(cbIsEditable.IsChecked, False)
        nud.IsEnabled = If(cbIsEnabled.IsChecked, True)

        OutputNUD()
    End Sub

    ''' <summary>
    ''' Display the interactive NumericUpDown control's properties.
    ''' </summary>
    Private Sub OutputNUD()
        Dim formatString As String = Constants.vbLf + Constants.vbLf & _
        " Minimum:" & _
        Constants.vbTab & "{0}" & Constants.vbLf & " Maximum:" & _
        Constants.vbTab & "{1}" & Constants.vbLf & " Value:" & _
        Constants.vbTab & "{2}" & Constants.vbLf & " Increment:" & _
        Constants.vbTab & "{3}" & Constants.vbLf & " DecimalPlaces:" & _
        Constants.vbTab & "{4}" & Constants.vbLf & " IsEditable:" & _
        Constants.vbTab & "{5}" & Constants.vbLf & " IsEnabled:" & _
        Constants.vbTab & "{6}" & Constants.vbLf
        output.Text = String.Format(CultureInfo.InvariantCulture, _
                                    formatString, _
                                    nud.Minimum, _
                                    nud.Maximum, _
                                    nud.Value, _
                                    nud.Increment, _
                                    nud.DecimalPlaces, _
                                    nud.IsEditable, _
                                    nud.IsEnabled)
    End Sub
End Class