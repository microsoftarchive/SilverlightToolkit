' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.Windows.Controls
Imports System.Windows.Controls.DataVisualization.Charting
Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating ScatterSeries.
''' </summary>
<Sample("Bubble", DifficultyLevel.Basic, "Bubble Series")> _
Partial Public Class BubbleSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the ScatterSample class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()

        SampleGenerators.GenerateNumericSeriesSamples(GeneratedChartsPanel, Function() New BubbleSeries(), True)
        SampleGenerators.GenerateDateTimeValueSeriesSamples(GeneratedChartsPanel, Function() New BubbleSeries())
        SampleGenerators.GenerateMultipleValueSeriesSamples(GeneratedChartsPanel, Function() New BubbleSeries(), True)
    End Sub
End Class
