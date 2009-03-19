' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.Windows.Controls
Imports System.Windows.Controls.Theming
Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating StyleHelper control.
''' </summary>
<Sample("ImplicitStyleManager", DifficultyLevel.Basic), Category("ImplicitStyleManager")> _
Partial Public Class ImplicitStyleManagerSample
    Inherits UserControl
    ''' <summary>
    ''' Class constructor.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Applies styles to a container manually.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">Information about the event.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Xaml uses it.")> _
    Private Sub ApplyStylesManuallyButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        ImplicitStyleManager.Apply(manualContainer)
    End Sub

    ''' <summary>
    ''' Adds a button to a container to style it.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">Information about the event.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="XAML uses it.")> _
    Private Sub AddButtonToContainerButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        dynamicContainer.Children.Add(New Button With {.Content = "New styled button"})
    End Sub
End Class
