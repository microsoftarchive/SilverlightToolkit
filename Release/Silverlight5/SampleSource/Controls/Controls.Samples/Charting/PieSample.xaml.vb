' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.Windows.Controls
Imports System.Windows.Controls.DataVisualization.Charting
Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating the Pie Chart.
''' </summary>
<Sample("(1)Pie", DifficultyLevel.Basic, "Pie Series")> _
Partial Public Class PieSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the PieSample class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()

        SampleGenerators.GenerateNumericSeriesSamples(GeneratedChartsPanel, Function() New PieSeries(), False)
        SampleGenerators.GenerateValueSeriesSamples(GeneratedChartsPanel, Function() New PieSeries())
        SampleGenerators.GenerateCategoryValueSeriesSamples(GeneratedChartsPanel, Function() New PieSeries())
        SampleGenerators.GenerateMultipleValueSeriesSamples(GeneratedChartsPanel, Function() New PieSeries(), False)
    End Sub

    ''' <summary>
    ''' Handles the SelectionChanged event of the "Drill-Down" sample Chart.
    ''' </summary>
    ''' <param name="sender">Event source.</param>
    ''' <param name="e">Event arguments.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Attached to event handler in XAML.")> _
    Private Sub DrillDown_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        InformationPanel.DataContext = If((1 <= e.AddedItems.Count), e.AddedItems(0), Nothing)
    End Sub

    ''' <summary>
    ''' Handles the Click event of the "Unselect" button for the "Drill-Down" sample Chart.
    ''' </summary>
    ''' <param name="sender">Event source.</param>
    ''' <param name="e">Event arguments.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Attached to event handler in XAML.")> _
    Private Sub Unselect_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        TryCast(DrillDownChart.Series(0), PieSeries).SelectedItem = Nothing
    End Sub
End Class
