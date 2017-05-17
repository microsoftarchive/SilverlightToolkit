' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Controls.DataVisualization.Charting
Imports System.Collections
Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating Animations.
''' </summary>
<Sample("(2)Animation", DifficultyLevel.Advanced, "Pie Series")> _
Partial Public Class PieAnimationSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the AnimationSample class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Called when the Chart is clicked.
    ''' </summary>
    ''' <param name="sender">The object that raised the event.</param>
    ''' <param name="e">Information about the event.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Attached to event handler in XAML.")> _
    Private Overloads Sub OnMouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
        Dim chart As Chart = TryCast(sender, Chart)

        If Nothing IsNot chart Then
            ' Toggle each Series ItemsSource so the reveal/hide animations can be seen
            For Each series As DataPointSeries In chart.Series
                If Nothing Is series.Tag Then
                    series.Tag = series.ItemsSource
                    series.ItemsSource = Nothing
                Else
                    series.ItemsSource = TryCast(series.Tag, IEnumerable)
                    series.Tag = Nothing
                End If
            Next series
        End If
    End Sub
End Class

