' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Threading
Imports System.Windows.Controls.DataVisualization.Charting

''' <summary>
''' Contains methods that generate sample Charts for any Series type.
''' </summary>
<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification:="Need to do additional initialization")> _
Public NotInheritable Class SampleGenerators
    ''' <summary>
    ''' Stores a shared ObservableCollection for use by the dynamic collection scenario.
    ''' </summary>
    Private Shared _dynamicCollectionItemsSource As ObservableCollection(Of Integer) = New ObservableCollection(Of Integer)()

    ''' <summary>
    ''' Stores a shared List for use by the dynamic data items scenario.
    ''' </summary>
    Private Shared _dynamicDataItemsSource As List(Of Pet) = New List(Of Pet)

    ''' <summary>
    ''' Stores a shared List for use by the dynamic date items scenario.
    ''' </summary>
    Private Shared _dynamicDateItemsSource As List(Of Pair) = New List(Of Pair)

    ''' <summary>
    ''' Stores a shared random number generator.
    ''' </summary>
    Private Shared _random As New Random()

    ''' <summary>
    ''' Initializes static members of the SampleGenerators class.
    ''' </summary>
    <SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification:="Need to do additional initialization.")> _
    Shared Sub New()
        _dynamicDataItemsSource.AddRange(New Pet() _
        { _
            New Pet With {.Species = "Dogs"}, _
            New Pet With {.Species = "Cats"}, _
            New Pet With {.Species = "Birds"}, _
            New Pet With {.Species = "Fish"} _
        })

        _dynamicDateItemsSource.AddRange(New Pair() _
        { _
            New Pair With {.First = New DateTime(2008, 10, 11), .Second = 0.0}, _
            New Pair With {.First = New DateTime(2008, 10, 12), .Second = 0.0}, _
            New Pair With {.First = New DateTime(2008, 10, 13), .Second = 0.0}, _
            New Pair With {.First = New DateTime(2008, 10, 14), .Second = 0.0}, _
            New Pair With {.First = New DateTime(2008, 10, 15), .Second = 0.0}, _
            New Pair With {.First = New DateTime(2008, 10, 16), .Second = 0.0} _
        })

        ' Create a timer to update the dynamic data regularly
        Dim dispatcherTimer As New DispatcherTimer()
        dispatcherTimer.Interval = TimeSpan.FromSeconds(2)
        AddHandler dispatcherTimer.Tick, AddressOf dispatcherTimer_Tick
        dispatcherTimer.Start()
    End Sub

    ''' <summary>
    ''' Hook up to the dispatcherTimer Tick event.
    ''' </summary>
    ''' <param name="sender">The sender object.</param>
    ''' <param name="e">The event data.</param>
    Private Shared Sub dispatcherTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs)
        _dynamicCollectionItemsSource.Add(_random.Next(1, 11))
        If 10 < _dynamicCollectionItemsSource.Count Then
            _dynamicCollectionItemsSource.RemoveAt(0)
        End If
        For Each pet As Pet In _dynamicDataItemsSource
            pet.Count = _random.Next(1, 20)
        Next pet
        For Each pair As Pair In _dynamicDateItemsSource
            pair.Second = _random.NextDouble() * 20
        Next pair
    End Sub

    ''' <summary>
    ''' Generates numeric Series samples.
    ''' </summary>
    ''' <param name="panel">Panel to add the generated Charts to.</param>
    ''' <param name="seriesConstructor">Function that returns a Series instance for each sample.</param>
    ''' <param name="includeIndependentValueBinding">True if an IndependentValueBinding should be created.</param>
    Public Shared Sub GenerateNumericSeriesSamples(ByVal panel As Panel, ByVal seriesConstructor As Func(Of Series), ByVal includeIndependentValueBinding As Boolean)
        Dim independentValueBinding As Binding = If(includeIndependentValueBinding, New Binding(), Nothing)
        Dim scenarios() As Scenario = _
        { _
            New Scenario With {.Title = "One Int", .ItemsSource = New Integer() {3}, .IndependentValueBinding = independentValueBinding}, _
            New Scenario With {.Title = "Three Ints", .ItemsSource = New Integer() {4, 2, 7}, .IndependentValueBinding = independentValueBinding}, _
            New Scenario With {.Title = "Seven Doubles", .ItemsSource = New Double() {4.4, 6.2, 9.7, 7.0, 2.1, 8.3, 5.5}, .IndependentValueBinding = independentValueBinding}, _
            New Scenario With {.Title = "No Points", .ItemsSource = New Integer() {}, .IndependentValueBinding = independentValueBinding}, _
            New Scenario With {.Title = "Some Points with Value 0", .ItemsSource = New Integer() {0, 1, 0, 2, 0}, .IndependentValueBinding = independentValueBinding}, _
            New Scenario With {.Title = "Some Negative-Value Points", .ItemsSource = New Double() {2.1, 3.7, -2.5, -4.6, 1.0}, .IndependentValueBinding = independentValueBinding}, _
            New Scenario With {.Title = "High-Value Points", .ItemsSource = New Integer() {2000000000, 1200000000, 2100000000, 1000000000, 2000000000}, .IndependentValueBinding = independentValueBinding}, _
            New Scenario With {.Title = "Dynamic Collection", .ItemsSource = _dynamicCollectionItemsSource, .IndependentValueBinding = independentValueBinding} _
        }
        ' New Scenario With {.Title = "100 Points", .ItemsSource = Enumerable.Range(1, 100), .IndependentValueBinding = independentValueBinding}, _
        GenerateSeriesSamples(panel, seriesConstructor, scenarios, Nothing)
    End Sub

    ''' <summary>
    ''' Generates value Series samples.
    ''' </summary>
    ''' <param name="panel">Panel to add the generated Charts to.</param>
    ''' <param name="seriesConstructor">Function that returns a Series instance for each sample.</param>
    Public Shared Sub GenerateValueSeriesSamples(ByVal panel As Panel, ByVal seriesConstructor As Func(Of Series))
        Dim scenarios() As Scenario = _
        { _
            New Scenario With {.Title = "Pet Counts (No Names)", .ItemsSource = pets, .DependentValueBinding = New Binding("Count")}, _
            New Scenario With {.Title = "Dynamic Data Items (No Names)", .ItemsSource = _dynamicDataItemsSource, .DependentValueBinding = New Binding("Count")} _
        }
        GenerateSeriesSamples(panel, seriesConstructor, scenarios, Nothing)
    End Sub

    ''' <summary>
    ''' Generates category/value Series samples.
    ''' </summary>
    ''' <param name="panel">Panel to add the generated Charts to.</param>
    ''' <param name="seriesConstructor">Function that returns a Series instance for each sample.</param>
    Public Shared Sub GenerateCategoryValueSeriesSamples(ByVal panel As Panel, ByVal seriesConstructor As Func(Of Series))
        Dim scenarios() As Scenario = _
        { _
            New Scenario With {.Title = "Pet Counts (Names)", .ItemsSource = pets, .DependentValueBinding = New Binding("Count"), .IndependentValueBinding = New Binding("Species")}, _
            New Scenario With {.Title = "Dynamic Data Items (Names)", .ItemsSource = _dynamicDataItemsSource, .DependentValueBinding = New Binding("Count"), .IndependentValueBinding = New Binding("Species")} _
        }
        GenerateSeriesSamples(panel, seriesConstructor, scenarios, Nothing)
    End Sub

    ''' <summary>
    ''' Generates value/value Series samples.
    ''' </summary>
    ''' <param name="panel">Panel to add the generated Charts to.</param>
    ''' <param name="seriesConstructor">Function that returns a Series instance for each sample.</param>
    Public Shared Sub GenerateValueValueSeriesSamples(ByVal panel As Panel, ByVal seriesConstructor As Func(Of Series))
        Dim circle As List(Of Point) = New List(Of Point)()
        Dim i As Double = 0.0
        While i < (2 * Math.PI)
            circle.Add(New Point(Math.Sin(i), Math.Cos(i)))
            i += 0.1
        End While

        Dim scenarios() As Scenario = _
        { _
            New Scenario With {.Title = "Circle", .ItemsSource = circle, .DependentValueBinding = New Binding("X"), .IndependentValueBinding = New Binding("Y")} _
        }
        GenerateSeriesSamples(panel, seriesConstructor, scenarios, Nothing)
    End Sub

    ''' <summary>
    ''' Generates value/value Series samples.
    ''' </summary>
    ''' <param name="panel">Panel to add the generated Charts to.</param>
    ''' <param name="seriesConstructor">Function that returns a Series instance for each sample.</param>
    Public Shared Sub GenerateDateTimeValueSeriesSamples(ByVal panel As Panel, ByVal seriesConstructor As Func(Of Series))
        Dim scenarios() As Scenario = _
        { _
            New Scenario With {.Title = "Value by Date", .ItemsSource = _dynamicDateItemsSource, .DependentValueBinding = New Binding("Second"), .IndependentValueBinding = New Binding("First")} _
        }
        Dim chartModifier As Action(Of Chart) = AddressOf ChartAxisModifier
        GenerateSeriesSamples(panel, seriesConstructor, scenarios, chartModifier)
    End Sub

    ''' <summary>
    ''' Adds axes to the specified Chart
    ''' </summary>
    ''' <param name="chart">The Chart to add generated axes to.</param>
    Private Shared Sub ChartAxisModifier(ByVal chart As Chart)
        Dim dateAxis As IAxis = New DateTimeAxis With {.Orientation = AxisOrientation.X}
        chart.Axes.Add(dateAxis)
        Dim valueAxis As IAxis = New LinearAxis With {.Orientation = AxisOrientation.Y, .Minimum = 0, .Maximum = 20, .ShowGridLines = True}
        chart.Axes.Add(valueAxis)
    End Sub

    ''' <summary>
    ''' Generates multiple value Series samples.
    ''' </summary>
    ''' <param name="panel">Panel to add the generated Charts to.</param>
    ''' <param name="seriesConstructor">Function that returns a Series instance for each sample.</param>
    ''' <param name="includeIndependentValueBinding">True if an IndependentValueBinding should be created.</param>
    Public Shared Sub GenerateMultipleValueSeriesSamples(ByVal panel As Panel, ByVal seriesConstructor As Func(Of Series), ByVal includeIndependentValueBinding As Boolean)
        Dim independentValueBinding As Binding = If(includeIndependentValueBinding, New Binding(), Nothing)
        Dim items() As Double = {3.1, 1.6, 4.9, 0.8, 2.2}
        Dim itemsRepeated As List(Of IEnumerable) = New List(Of IEnumerable)()
        For i As Integer = 0 To 29
            itemsRepeated.Add(items)
        Next i
        Dim scenarios() As Scenario = _
        { _
            New Scenario With {.Title = "Three Series", .ItemsSources = New IEnumerable() {New Integer() {1, 2, 3}, New Integer() {4, 5, 6}, New Integer() {7, 8, 9}}, .IndependentValueBinding = independentValueBinding} _
        }
        ' New Scenario With { .Title = "Thirty Series", .ItemsSources = itemsRepeated, .IndependentValueBinding = independentValueBinding },
        GenerateSeriesSamples(panel, seriesConstructor, scenarios, Nothing)
    End Sub

    ''' <summary>
    ''' Generates various Series samples.
    ''' </summary>
    ''' <param name="panel">Panel to add the generated Charts to.</param>
    ''' <param name="seriesConstructor">Function that returns a Series instance for each sample.</param>
    ''' <param name="scenarios">Collection of scenarios to generate.</param>
    ''' <param name="chartModifier">Function that applies any necessary modifications to the Chart.</param>
    Private Shared Sub GenerateSeriesSamples(ByVal panel As Panel, ByVal seriesConstructor As Func(Of Series), ByVal scenarios As IEnumerable(Of Scenario), ByVal chartModifier As Action(Of Chart))
        Dim wrapperStyle As Style = TryCast(Application.Current.Resources("WrapperStyle"), Style)

        ' For each scenario...
        For Each scenario As Scenario In scenarios
            ' Create the sample Chart
            Dim chart As Chart = New Chart With {.Title = scenario.Title, .MaxWidth = 500, .MaxHeight = 270}
            For Each itemsSource As IEnumerable In scenario.ItemsSources
                Dim series As DataPointSeries = TryCast(seriesConstructor(), DataPointSeries)
                series.ItemsSource = itemsSource
                series.DependentValueBinding = scenario.DependentValueBinding
                series.IndependentValueBinding = scenario.IndependentValueBinding
                chart.Series.Add(series)
            Next itemsSource
            If Nothing IsNot chartModifier Then
                chartModifier(chart)
            End If

            ' Wrap the Chart in a suitably formatted Grid
            Dim grid As Grid = New Grid With {.Style = wrapperStyle}
            grid.Children.Add(chart)
            panel.Children.Add(grid)
        Next scenario
    End Sub

    ''' <summary>
    ''' Collection of Pet objects for use by Chart samples.
    ''' </summary>
    Private Shared pets() As Pet = _
    { _
        New Pet With {.Species = "Dogs", .Count = 5}, _
        New Pet With {.Species = "Cats", .Count = 4}, _
        New Pet With {.Species = "Birds", .Count = 6}, _
        New Pet With {.Species = "Fish", .Count = 9} _
    }

    ''' <summary>
    ''' Class representing an automatically generated Chart sample.
    ''' </summary>
    Private Class Scenario
        ''' <summary>
        ''' Gets or sets the title of the scenario.
        ''' </summary>
        Private privateTitle As String
        Public Property Title() As String
            Get
                Return privateTitle
            End Get
            Set(ByVal value As String)
                privateTitle = value
            End Set
        End Property

        ''' <summary>
        ''' Sets the ItemsSource for the Chart's Series.
        ''' </summary>
        Public WriteOnly Property ItemsSource() As IEnumerable
            Set(ByVal value As IEnumerable)
                ItemsSources = New IEnumerable() {value}
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the ItemsSources for the Chart's Series.
        ''' </summary>
        Private privateItemsSources As IEnumerable(Of IEnumerable)
        Public Property ItemsSources() As IEnumerable(Of IEnumerable)
            Get
                Return privateItemsSources
            End Get
            Set(ByVal value As IEnumerable(Of IEnumerable))
                privateItemsSources = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the DependentValueBinding for the Chart's Series.
        ''' </summary>
        Private privateDependentValueBinding As Binding
        Public Property DependentValueBinding() As Binding
            Get
                Return privateDependentValueBinding
            End Get
            Set(ByVal value As Binding)
                privateDependentValueBinding = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the IndependentValueBinding for the Chart's Series.
        ''' </summary>
        Private privateIndependentValueBinding As Binding
        Public Property IndependentValueBinding() As Binding
            Get
                Return privateIndependentValueBinding
            End Get
            Set(ByVal value As Binding)
                privateIndependentValueBinding = value
            End Set
        End Property
    End Class
End Class
