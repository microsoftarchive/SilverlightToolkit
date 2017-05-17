' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Globalization
Imports System.Windows.Controls
Imports System.ComponentModel

''' <summary>
''' ButtonSpinner sample page.
''' </summary>
<Sample("ButtonSpinner", DifficultyLevel.Basic, "ButtonSpinner")> _
Partial Public Class ButtonSpinnerSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the ButtonSpinnerSample class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ' Declares a delegate for a method that takes in a String.
    Private Delegate Sub PropertyDisplayHandler(ByVal properties As String)

    ''' <summary>
    ''' Display the interactive ButtonSpinner control's properties.
    ''' </summary>
    ''' <param name="sender">The target ButtonSpinner</param>
    ''' <param name="e">The SpinEventArgs.</param>
    Private Sub spinner_Spin(ByVal sender As Object, ByVal e As SpinEventArgs) Handles spinner1.Spin, spinner2.Spin
        Dim s As String = String.Format(CultureInfo.InvariantCulture, _
                                        "SpinEventArgs.Direction: " & _
                                        Constants.vbTab & "{0}", e.Direction)
        display.Items.Add(s)
        display.SelectedIndex = display.Items.Count - 1
        Dispatcher.BeginInvoke(New PropertyDisplayHandler(AddressOf ScrollPropertiesAsync), s)
    End Sub

    ''' <summary>
    ''' Handles the asynchronous display of the ButtonSpinner control's properties.
    ''' </summary>
    ''' <param name="properties">The properties to display.</param>
    Private Sub ScrollPropertiesAsync(ByVal properties As String)
        display.ScrollIntoView(properties)
    End Sub
End Class