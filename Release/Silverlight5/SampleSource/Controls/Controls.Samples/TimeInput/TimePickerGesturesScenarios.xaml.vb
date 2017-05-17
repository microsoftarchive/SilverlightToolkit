' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.ComponentModel
Imports System.Threading
Imports System.Windows.Input
Imports System.Diagnostics.CodeAnalysis

''' <summary>
''' Sample page that demonstrates different
''' interactions that are possible.
''' </summary>
<Sample("TimePicker Gestures", DifficultyLevel.Scenario, "TimePicker")> _
Partial Public Class TimePickerGesturesScenarios
    Inherits UserControl
    ''' <summary>
    ''' Closing the dropdown will move focus to the ToggleButton, which 
    ''' will raise the 'GotFocus' event on the TimePicker, resulting in a
    ''' loop. We will ignore the next Focus event after closing the dropdown
    ''' to prevent this.
    ''' </summary>
    Private _ignoreNextGain As Boolean

    ''' <summary>
    ''' Initializes a new instance of the <see cref="TimePickerGesturesScenarios"/> class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

#Region "Focus"
    ''' <summary>
    ''' Called when the picker gains focus.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Set from Xaml.")> _
    Private Sub GainedFocus(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles tp.GotFocus
        If (Not _ignoreNextGain) Then
            AddHandler tp.DropDownClosed, AddressOf DropDownClosed
            tp.IsDropDownOpen = True
        Else
            _ignoreNextGain = False
        End If
    End Sub

    ''' <summary>
    ''' Handles the DropDownClosed event of the control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The instance containing the event data.</param>
    Private Sub DropDownClosed(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of Boolean))
        RemoveHandler tp.DropDownClosed, AddressOf DropDownClosed

        ' closing dropdown will move focus to button.
        _ignoreNextGain = True
    End Sub
#End Region ' Focus

#Region "Hover"
    ''' <summary>
    ''' Called when the mouse enters the Picker.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Called by Xaml.")> _
    Private Sub PickerMouseEnter(ByVal sender As Object, ByVal e As MouseEventArgs)
        tp2.IsDropDownOpen = True
    End Sub
#End Region ' Hover
End Class