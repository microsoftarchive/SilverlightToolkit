' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Controls.DataVisualization
Imports System.Windows.Controls.DataVisualization.Charting

''' <summary>
''' FunctionSeries is used to single variable functions on a chart.
''' </summary>
<TemplatePart(Name:=FunctionSeries.PlotAreaName, Type:=GetType(Canvas))> _
Partial Public NotInheritable Class FunctionSeries
    Inherits Series
    Implements IRangeProvider, IAxisListener
    ''' <summary>
    ''' The default control template would normally reside in generic.xaml,
    ''' but the sample project is an application and doesn't have that file.
    ''' We're just putting it here, but a real control project wouldn't.
    ''' </summary>
    Private Const DefaultTemplate As String = "<ControlTemplate" & ControlChars.CrLf & _
    "  xmlns='http://schemas.microsoft.com/client/2007'" & ControlChars.CrLf & _
    "  xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'" & ControlChars.CrLf & _
    "  xmlns:samples='clr-namespace:System.Windows.Controls.Samples;assembly=System.Windows.Controls.Samples'" & ControlChars.CrLf & _
    "  TargetType='samples:FunctionSeries'>" & ControlChars.CrLf & _
    "    <Canvas x:Name='PlotArea'>" & ControlChars.CrLf & _
    "        <Path" & ControlChars.CrLf & _
    "          Stroke='{TemplateBinding LineBrush}'" & ControlChars.CrLf & _
    "          StrokeThickness='{TemplateBinding LineThickness}'" & ControlChars.CrLf & _
    "          Data='{TemplateBinding Geometry}' />" & ControlChars.CrLf & _
    "    </Canvas>" & ControlChars.CrLf & _
    "</ControlTemplate>"

#Region "Template Parts"
    ''' <summary>
    ''' Name of the plot area canvas.
    ''' </summary>
    Public Const PlotAreaName As String = "PlotArea"

    ''' <summary>
    ''' Gets or sets the plot area canvas.
    ''' </summary>
    Private privatePlotArea As Canvas
    Private Property PlotArea() As Canvas
        Get
            Return privatePlotArea
        End Get
        Set(ByVal value As Canvas)
            privatePlotArea = value
        End Set
    End Property
#End Region ' Template Parts

#Region "public Func<double, double> Function"
    ''' <summary>
    ''' Gets or sets the function to plot.
    ''' </summary>
    <TypeConverter(GetType(SimpleFunctionTypeConverter))> _
    Public Property [Function]() As Func(Of Double, Double)
        Get
            Return TryCast(GetValue(FunctionProperty), Func(Of Double, Double))
        End Get
        Set(ByVal value As Func(Of Double, Double))
            SetValue(FunctionProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Identifies the Function dependency property.
    ''' </summary>
    Public Shared ReadOnly FunctionProperty As DependencyProperty = _
        DependencyProperty.Register( _
            "Function", _
            GetType(Func(Of Double, Double)), _
            GetType(FunctionSeries), _
            New PropertyMetadata(Nothing, AddressOf OnFunctionPropertyChanged))

    ''' <summary>
    ''' FunctionProperty property changed handler.
    ''' </summary>
    ''' <param name="d">FunctionSeries that changed its Function.</param>
    ''' <param name="e">Event arguments.</param>
    Private Shared Sub OnFunctionPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        Dim source As FunctionSeries = TryCast(d, FunctionSeries)
        source.Refresh()
    End Sub
#End Region ' public Func<double, double> Function

#Region "public Geometry Geometry"
    ''' <summary>
    ''' Gets or sets the geometry of the line object rendering the function.
    ''' </summary>
    Public Property Geometry() As Geometry
        Get
            Return TryCast(GetValue(GeometryProperty), Geometry)
        End Get
        Set(ByVal value As Geometry)
            SetValue(GeometryProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Identifies the Geometry dependency property.
    ''' </summary>
    Public Shared ReadOnly GeometryProperty As DependencyProperty = _
        DependencyProperty.Register( _
            "Geometry", _
            GetType(Geometry), _
            GetType(FunctionSeries), _
            New PropertyMetadata(Nothing))
#End Region ' public Geometry Geometry

#Region "public Brush LineBrush"
    ''' <summary>
    ''' Gets or sets the brush used to plot the function.
    ''' </summary>
    Public Property LineBrush() As Brush
        Get
            Return TryCast(GetValue(LineBrushProperty), Brush)
        End Get
        Set(ByVal value As Brush)
            SetValue(LineBrushProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Identifies the LineBrush dependency property.
    ''' </summary>
    Public Shared ReadOnly LineBrushProperty As DependencyProperty = _
        DependencyProperty.Register( _
            "LineBrush", _
            GetType(Brush), _
            GetType(FunctionSeries), _
            New PropertyMetadata(Nothing, AddressOf OnLineBrushPropertyChanged))

    ''' <summary>
    ''' LineBrushProperty property changed handler.
    ''' </summary>
    ''' <param name="d">FunctionSeries that changed its LineBrush.</param>
    ''' <param name="e">Event arguments.</param>
    Private Shared Sub OnLineBrushPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        Dim source As FunctionSeries = TryCast(d, FunctionSeries)
        Dim value As Brush = TryCast(e.NewValue, Brush)
        source.LegendItem.DataContext = New ContentControl With {.Background = value}
    End Sub
#End Region ' public Brush LineBrush

#Region "public double LineThickness"
    ''' <summary>
    ''' Gets or sets the thickness of the line used to plot the function.
    ''' </summary>
    Public Property LineThickness() As Double
        Get
            Return CDbl(GetValue(LineThicknessProperty))
        End Get
        Set(ByVal value As Double)
            SetValue(LineThicknessProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Identifies the LineThickness dependency property.
    ''' </summary>
    Public Shared ReadOnly LineThicknessProperty As DependencyProperty = _
        DependencyProperty.Register( _
            "LineThickness", _
            GetType(Double), _
            GetType(FunctionSeries), _
            New PropertyMetadata(1.0))
#End Region ' public double LineThickness

#Region "private IRangeAxis IndependentAxis"
    ''' <summary>
    ''' Gets or sets the value of the independent axis.
    ''' </summary>
    Private Property IndependentAxis() As IRangeAxis
        Get
            Return TryCast(GetValue(IndependentAxisProperty), IRangeAxis)
        End Get
        Set(ByVal value As IRangeAxis)
            SetValue(IndependentAxisProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Identifies the IndependentAxis dependency property.
    ''' </summary>
    Private Shared ReadOnly IndependentAxisProperty As DependencyProperty = _
        DependencyProperty.Register( _
            "IndependentAxis", _
            GetType(IRangeAxis), _
            GetType(FunctionSeries), _
            Nothing)
#End Region ' protected IRangeAxis IndependentAxis

#Region "private IRangeAxis DependentAxis"
    ''' <summary>
    ''' Gets or sets the value of the dependent axis.
    ''' </summary>
    Private Property DependentAxis() As IRangeAxis
        Get
            Return TryCast(GetValue(DependentAxisProperty), IRangeAxis)
        End Get
        Set(ByVal value As IRangeAxis)
            SetValue(DependentAxisProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Identifies the DependentAxis dependency property.
    ''' </summary>
    Private Shared ReadOnly DependentAxisProperty As DependencyProperty = _
        DependencyProperty.Register( _
            "DependentAxis", _
            GetType(IRangeAxis), _
            GetType(FunctionSeries), _
            Nothing)
#End Region ' protected IRangeAxis DependentAxis

    ''' <summary>
    ''' Gets or sets the single chart legend item associated with the
    ''' series.
    ''' </summary>
    Private privateLegendItem As LegendItem
    Private Property LegendItem() As LegendItem
        Get
            Return privateLegendItem
        End Get
        Set(ByVal value As LegendItem)
            privateLegendItem = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the Geometry used to clip the line to the PlotArea
    ''' bounds.
    ''' </summary>
    Private privateClipGeometry As RectangleGeometry
    Private Property ClipGeometry() As RectangleGeometry
        Get
            Return privateClipGeometry
        End Get
        Set(ByVal value As RectangleGeometry)
            privateClipGeometry = value
        End Set
    End Property

    ''' <summary>
    ''' Initializes a new instance of the FunctionSeries class.
    ''' </summary>
    Public Sub New()
        LegendItem = New LegendItem()
        LegendItems.Add(LegendItem)
        ClipGeometry = New RectangleGeometry()
        Clip = ClipGeometry

        ' Explicitly load the default template since the samples project
        ' is an application and does not have a generic.xaml file.
        Template = TryCast(XamlReader.Load(DefaultTemplate), ControlTemplate)
        LineBrush = New SolidColorBrush(Colors.Purple)
    End Sub

    ''' <summary>
    ''' Refreshes data from data source and renders the series.
    ''' </summary>
    Private Sub Refresh()
        If SeriesHost Is Nothing OrElse ActualWidth = 0 Then
            Return
        End If

        ' Ensure we have a function to plot
        Dim [function] As Func(Of Double, Double) = Me.Function
        If [function] Is Nothing Then
            Return
        End If

        ' Ensure we have axes
        Dim independent As IRangeAxis = GetAxis(AxisOrientation.X, IndependentAxis)
        IndependentAxis = independent
        Dim dependent As IRangeAxis = GetAxis(AxisOrientation.Y, DependentAxis)
        DependentAxis = dependent
        If (Not independent.Range.HasData) Then
            Return
        End If

        ' Create a geometry that matches the function to plot
        Dim path As New PathGeometry()
        Dim figure As New PathFigure()

        ' Get the range over which we will 
        Dim start As Double = CDbl(independent.Range.Minimum)
        Dim [end] As Double = CDbl(independent.Range.Maximum)

        ' Adjust the line at each pixel
        Dim delta As Double = ([end] - start) / ActualWidth

        ' We'll only add a new line segment when the slope is changing
        ' between points
        Dim last As Point = GetPoint(start, [function], independent, dependent)
        figure.StartPoint = last
        Dim slope As Double = Double.NaN
        For x As Double = start + delta To [end] Step delta
            Dim [next] As Point = GetPoint(x, [function], independent, dependent)
            Dim newSlope As Double = ([next].Y - last.Y) / ([next].X - last.X)

            If slope <> newSlope Then
                figure.Segments.Add(New LineSegment With {.Point = last})
            End If

            slope = newSlope
            last = [next]
        Next x
        figure.Segments.Add(New LineSegment With {.Point = last})

        path.Figures.Add(figure)
        Geometry = path
    End Sub

    ''' <summary>
    ''' Get a point in screen coordinates.
    ''' </summary>
    ''' <param name="x">Independent value.</param>
    ''' <param name="function">The function.</param>
    ''' <param name="independent">The independent axis.</param>
    ''' <param name="dependent">The dependent axis.</param>
    ''' <returns>The point in screen coordinates.</returns>
    Private Function GetPoint(ByVal x As Double, ByVal [function] As Func(Of Double, Double), ByVal independent As IRangeAxis, ByVal dependent As IRangeAxis) As Point
        ' Get the dependent value
        Dim y As Double = Double.NaN
        Try
            y = [function](x)
        Catch e1 As DivideByZeroException
        End Try

        ' Map the actual values into coordinate values
        Return New Point( _
            independent.GetPlotAreaCoordinate(x).Value, _
            Math.Min( _
                Math.Max( _
                    ActualHeight - dependent.GetPlotAreaCoordinate(y).Value, _
                    -1), _
                ActualHeight + 1))
    End Function

    ''' <summary>
    ''' Get the plot area after loading it from XAML.
    ''' </summary>
    Public Overrides Sub OnApplyTemplate()
        MyBase.OnApplyTemplate()
        PlotArea = TryCast(GetTemplateChild("PlotArea"), Canvas)
    End Sub

    ''' <summary>
    ''' Updates the visual appearance of all the data points when the size
    ''' changes. 
    ''' </summary>
    ''' <param name="sender">The series.</param>
    ''' <param name="e">Event arguments.</param>
    Private Sub OnSizeChanged(ByVal sender As Object, ByVal e As SizeChangedEventArgs) Handles Me.SizeChanged
        ' Update the clip geometry
        ClipGeometry.Rect = New Rect(0.0, 0.0, e.NewSize.Width, e.NewSize.Height)

        ' Update the PlotArea size and refresh.
        If PlotArea IsNot Nothing Then
            PlotArea.Width = e.NewSize.Width
            PlotArea.Height = e.NewSize.Height
            Refresh()
        End If
    End Sub

    ''' <summary>
    ''' Sets all the text the legend items to the title.
    ''' </summary>
    ''' <param name="oldValue">The old title.</param>
    ''' <param name="newValue">The new title.</param>
    Protected Overrides Sub OnTitleChanged(ByVal oldValue As Object, ByVal newValue As Object)
        MyBase.OnTitleChanged(oldValue, newValue)
        LegendItem.Content = Title
    End Sub

    ''' <summary>
    ''' Get or create a linear numeric axis in the correct dimension.
    ''' </summary>
    ''' <param name="orientation">Dimension of the axis to create.</param>
    ''' <param name="oldAxis">
    ''' Old value of the axis in this dimension.
    ''' </param>
    ''' <returns>New value of the axis in this dimension.</returns>
    Private Function GetAxis(ByVal orientation As AxisOrientation, ByVal oldAxis As IRangeAxis) As IRangeAxis
        ' Check the existing axes for a potential axis
        Dim axis As IRangeAxis = ( _
          From a As IRangeAxis In SeriesHost.Axes.OfType(Of IRangeAxis)() _
          Where a.Orientation = orientation _
          Select a).FirstOrDefault()

        If axis Is Nothing Then
            ' Create a new axis if not found
            axis = New LinearAxis With {.Orientation = orientation}
        End If

        If oldAxis IsNot axis Then
            ' Unregister any existing axis
            If oldAxis IsNot Nothing Then
                oldAxis.RegisteredListeners.Remove(Me)
            End If

            ' Register the new axis
            If (Not axis.RegisteredListeners.Contains(Me)) Then
                axis.RegisteredListeners.Add(Me)
            End If
        End If

        Return axis
    End Function

    ''' <summary>
    ''' Updates the series when the axis is invalidated.
    ''' </summary>
    ''' <param name="axis">The axis that was invalidated.</param>
    Public Sub AxisInvalidated(ByVal axis As IAxis) Implements IAxisListener.AxisInvalidated
        If DependentAxis IsNot Nothing AndAlso IndependentAxis IsNot Nothing Then
            Refresh()
        End If
    End Sub

    ''' <summary>
    ''' Ensures that chart and series are kept in a consistent state when a
    ''' series is added or removed from a chart. 
    ''' </summary>
    ''' <param name="oldValue">Old chart.</param>
    ''' <param name="newValue">New chart.</param>
    Protected Overrides Sub OnSeriesHostPropertyChanged(ByVal oldValue As ISeriesHost, ByVal newValue As ISeriesHost)
        Dim axis As IRangeAxis = Nothing

        ' Unregister the axes from the old chart
        If oldValue IsNot Nothing Then
            axis = IndependentAxis
            If axis IsNot Nothing Then
                axis.RegisteredListeners.Remove(Me)
                IndependentAxis = Nothing
            End If

            axis = DependentAxis
            If axis IsNot Nothing Then
                axis.RegisteredListeners.Remove(Me)
                DependentAxis = Nothing
            End If
        End If

        ' Register the axes with new chart
        If newValue IsNot Nothing Then
            axis = IndependentAxis
            If axis IsNot Nothing Then
                axis.RegisteredListeners.Add(Me)
            End If

            axis = DependentAxis
            If axis IsNot Nothing Then
                axis.RegisteredListeners.Add(Me)
            End If
        End If

        MyBase.OnSeriesHostPropertyChanged(oldValue, newValue)
    End Sub

    ''' <summary>
    ''' If data is found returns the minimum and maximum dependent numeric
    ''' values. 
    ''' </summary>
    ''' <param name="rangeConsumer">IRangeConsumer that needs the data.</param>
    ''' <returns>
    ''' The range of values or empty if no data is present.
    ''' </returns>
    Public Function GetRange(ByVal rangeConsumer As IRangeConsumer) As Range(Of IComparable) Implements IRangeProvider.GetRange
        ' Use an empty range so we only plot over the area used by other
        ' axes.
        Return New Range(Of IComparable)()
    End Function
End Class
