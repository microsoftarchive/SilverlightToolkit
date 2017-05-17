' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Controls
Imports System.Linq
Imports System.Diagnostics
Imports System.Windows.Media
Imports System.ComponentModel

''' <summary>
''' Charting sample that demonstrates zooming.
''' </summary>
<Sample("Zoom", DifficultyLevel.Scenario, "DataVisualization")> _
Partial Public Class SeriesZoomSample
    Inherits UserControl
    ''' <summary>
    ''' Caching of the ChartArea template part.
    ''' </summary>
    Private chartArea_Renamed As Panel

    ''' <summary>
    ''' Gets the ChartArea.
    ''' </summary>
    ''' <returns>TemplatePart ChartArea</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Called by ZoomChanged.")> _
    Private ReadOnly Property ChartArea() As Panel
        Get
            If chartArea_Renamed Is Nothing Then
                chartArea_Renamed = TryCast(GetLogicalChildrenBreadthFirst(ZoomChart).Where(Function(element) element.Name.Equals("ChartArea")).FirstOrDefault(), Panel)
            End If

            Return chartArea_Renamed
        End Get
    End Property

    ''' <summary>
    ''' Caching of the ScrollArea template part.
    ''' </summary>
    Private scrollArea_Renamed As ScrollViewer

    ''' <summary>
    ''' Gets the ScrollArea.
    ''' </summary>
    ''' <returns>TemplatePart ScrollArea</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Called by an event handler in XAML.")> _
    Private ReadOnly Property ScrollArea() As ScrollViewer
        Get
            If scrollArea_Renamed Is Nothing Then
                scrollArea_Renamed = TryCast(GetLogicalChildrenBreadthFirst(ZoomChart).Where(Function(element) element.Name.Equals("ScrollArea")).FirstOrDefault(), ScrollViewer)
            End If
            Return scrollArea_Renamed
        End Get
    End Property

    ''' <summary>
    ''' Initializes a new instance of the ZoomIntoChartSample class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Force an update of the chart.
    ''' </summary>
    ''' <param name="sender">The ZoomIntoChartSample instance.</param>
    ''' <param name="e">Event arguments.</param>
    Private Sub ZoomIntoChartSample_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Loaded
        ' force synchronous layout pass
        ZoomChart.UpdateLayout()

        ' and force initial zoom 
        UpdateChart(0)
    End Sub

    ''' <summary>
    ''' Handles the changing of the zoomlevel.
    ''' </summary>
    ''' <param name="sender">The zoom slider.</param>
    ''' <param name="e">Event arguments.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Called by an event handler in XAML.")> _
    Private Sub ZoomChanged(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of Double))
        Debug.Assert(ChartArea IsNot Nothing AndAlso ScrollArea IsNot Nothing, "Zoom should not be called before layout has occurred")

        Dim zoom As Double = e.NewValue

        UpdateChart(zoom)
    End Sub

    ''' <summary>
    ''' Updates the chart to zoom with the correct zoom factor.
    ''' </summary>
    ''' <param name="zoom">The percentage of zoom we wish to apply.</param>
    Private Sub UpdateChart(ByVal zoom As Double)
        ChartArea.Width = ScrollArea.ViewportWidth + (ScrollArea.ViewportWidth * zoom / 100.0)
    End Sub

    ''' <summary>
    ''' Helper function that returns a list of the visual children.
    ''' </summary>
    ''' <param name="parent">Element whose visual children will be returned.</param>
    ''' <returns>A collection of visualchildren.</returns>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Called by ChartArea and ScrollArea.")> _
    Private Shared Function GetLogicalChildrenBreadthFirst(ByVal parent As FrameworkElement) As IEnumerable(Of FrameworkElement)
        Debug.Assert(parent IsNot Nothing, "The parent cannot be null.")

        Dim queue As Queue(Of FrameworkElement) = New Queue(Of FrameworkElement)(GetVisualChildren(parent).OfType(Of FrameworkElement)())
        Dim list As New List(Of FrameworkElement)()
        Do While queue.Count > 0
            Dim element As FrameworkElement = queue.Dequeue()
            list.Add(element)

            For Each visualChild As FrameworkElement In GetVisualChildren(element).OfType(Of FrameworkElement)()
                queue.Enqueue(visualChild)
            Next visualChild
        Loop
        Return list
    End Function

    ''' <summary>
    ''' Helper function that returns the direct visual children of an element.
    ''' </summary>
    ''' <param name="parent">The element whose visual children will be returned.</param>
    ''' <returns>A collection of visualchildren.</returns>
    Private Shared Function GetVisualChildren(ByVal parent As DependencyObject) As IEnumerable(Of DependencyObject)
        Debug.Assert(parent IsNot Nothing, "The parent cannot be null.")

        Dim childCount As Integer = VisualTreeHelper.GetChildrenCount(parent)
        Dim list As New List(Of DependencyObject)()
        For counter As Integer = 0 To childCount - 1
            list.Add(VisualTreeHelper.GetChild(parent, counter))
        Next counter
        Return list
    End Function
End Class
