' (c) Copyright Microsoft Corporation.
' This source is subject to <###LICENSE_NAME###>.
' Please see <###LICENSE_LINK###> for details.
' All other rights reserved.

Imports System.Reflection
Imports System.Windows
Imports System.Windows.Controls
Imports System.ComponentModel

''' <summary>
''' The Welcome page is placed at the top of the samples list and is shown 
''' when the page initially loads.
''' </summary>
''' <remarks>The SampleAttribute value is prefixed with a period to enable 
''' it to show up at the top of the samples list. The period is removed in 
''' the sample browser control.</remarks>
<Sample("Welcome", DifficultyLevel.None), Category("Controls")> _
Partial Public Class Welcome
    Inherits UserControl
    ''' <summary>
    ''' A token used for replacement of the assembly name.
    ''' </summary>
    Private Const AssemblyToken As String = "{ASSEMBLY}"

    ''' <summary>
    ''' A token used for replacement of the sample assembly name.
    ''' </summary>
    Private Const SampleAssemblyToken As String = "{SAMPLE_ASSEMBLY}"

    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' The loaded event.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub Welcome_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Loaded
        Dim welcome As String = WelcomeText.Text

        If welcome.Contains(SampleAssemblyToken) Then
            ' Extract the sample assembly name
            Dim [me] As Assembly = welcome.GetType().Assembly()
            Dim friendlyAssembly As String = [me].FullName
            Dim comma As Integer = friendlyAssembly.IndexOf(",", StringComparison.Ordinal)
            If comma > 0 Then
                friendlyAssembly = friendlyAssembly.Substring(0, comma)
            End If

            ' Update the text value
            welcome = welcome.Replace(SampleAssemblyToken, friendlyAssembly)

            ' Extract and set the sample assembly name
            If welcome.Contains(AssemblyToken) Then
                Dim assembly As String = friendlyAssembly.Replace(".Samples", "")
                welcome = welcome.Replace(AssemblyToken, assembly)
            End If

            WelcomeText.Text = welcome
        End If
    End Sub

End Class