' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis

''' <summary>
''' Sample page demonstrating the DatePicker.
''' </summary>
<Sample("TabControl", DifficultyLevel.Basic, "TabControl")> _
Partial Public Class TabControlSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the DatePickerSample class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Handle button clicks to add new tab items.
    ''' </summary>
    ''' <param name="sender">The Button.</param>
    ''' <param name="e">Event arguments.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Event handler attached in XAML.")> _
    Private Sub OnAddTabItem(ByVal sender As Object, ByVal e As RoutedEventArgs)
        sampleTabs.Items.Add(New TabItem With {.Header = "Dynamically Created TabItem", .Content = "Some Content"})
    End Sub
End Class