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
<Sample("Line Series", DifficultyLevel.Basic, "Line Series")> _
Partial Public Class LineSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the ScatterSample class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()

        SampleGenerators.GenerateNumericSeriesSamples(GeneratedChartsPanel, Function() New LineSeries(), True)
        SampleGenerators.GenerateDateTimeValueSeriesSamples(GeneratedChartsPanel, Function() New LineSeries())
        SampleGenerators.GenerateMultipleValueSeriesSamples(GeneratedChartsPanel, Function() New LineSeries(), True)
    End Sub
End Class
