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
<Sample("Welcome", DifficultyLevel.None, "Controls")> _
Partial Public Class Welcome
    Inherits UserControl

    Public Sub New()
        InitializeComponent()
    End Sub

End Class