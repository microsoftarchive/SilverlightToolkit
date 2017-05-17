' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.Windows.Controls
Imports System.Windows.Controls.DataVisualization.Charting
Imports System.ComponentModel

''' <summary>
''' Sample page demonstrating ColumnSeries.
''' </summary>
<Sample("(1)Column", DifficultyLevel.Basic, "Column Series")> _
Partial Public Class ColumnSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the ColumnSample class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()

        SampleGenerators.GenerateNumericSeriesSamples(GeneratedChartsPanel, Function() New ColumnSeries(), False)
        SampleGenerators.GenerateValueSeriesSamples(GeneratedChartsPanel, Function() New ColumnSeries())
        SampleGenerators.GenerateCategoryValueSeriesSamples(GeneratedChartsPanel, Function() New ColumnSeries())
        SampleGenerators.GenerateMultipleValueSeriesSamples(GeneratedChartsPanel, Function() New ColumnSeries(), False)
    End Sub
End Class
