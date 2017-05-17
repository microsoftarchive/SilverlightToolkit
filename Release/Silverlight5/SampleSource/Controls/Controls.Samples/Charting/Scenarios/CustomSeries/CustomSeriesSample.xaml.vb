' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Controls.DataVisualization.Charting
Imports System.ComponentModel

''' <summary>
''' Charting sample demonstrating how to create a custom series.
''' </summary>
<Sample("(2)Custom Series", DifficultyLevel.Advanced, "DataVisualization")> _
Partial Public Class CustomSeriesSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the CustomSeriesSample class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()

        ' Use a custom function for a series
        Dim series As FunctionSeries = TryCast(CustomFunctionChart.Series(1), FunctionSeries)
        series.Function = Function(x As Double) 110 + 3 * Math.Sin(x)
    End Sub

    ''' <summary>
    ''' Perform a regression against the particulate levels data.
    ''' </summary>
    ''' <param name="sender">The regression ComboBox.</param>
    ''' <param name="e">Event arguments.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Called by an event handler in XAML."), _
    SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId:="Body", Justification:="Simplifies the sample."), _
    SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification:="Simplifies the sample.")> _
    Private Sub OnRegressionTypeChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        ' Get the options and the series
        Dim combo As ComboBox = TryCast(sender, ComboBox)
        If combo Is Nothing OrElse ParticulateAnalysis Is Nothing Then
            Return
        End If
        Dim dataSeries As ScatterSeries = TryCast(ParticulateAnalysis.Series(0), ScatterSeries)
        Dim regressionSeries As FunctionSeries = TryCast(ParticulateAnalysis.Series(1), FunctionSeries)
        If dataSeries Is Nothing OrElse regressionSeries Is Nothing Then
            Return
        End If

        ' Get the active DataPoints (this assumes the default template for
        ' ScatterSeries)
        Dim plotArea As Canvas = TryCast(VisualTreeHelper.GetChild(dataSeries, 0), Canvas)
        If plotArea Is Nothing Then
            Return
        End If
        Dim activePoints As List(Of DataPoint) = plotArea.Children.OfType(Of DataPoint)().ToList()

        ' The dimensions were added linearly to the ComboBox
        Dim dimension As Integer = combo.SelectedIndex + 1

        ' Initialize a simple least squares analysis
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim k As Integer = 0
        Dim y(activePoints.Count - 1) As Double
        Dim x(activePoints.Count - 1, dimension) As Double
        For i = 0 To activePoints.Count - 1
            Dim point As DataPoint = activePoints(i)
            Dim independentValue As Double = Convert.ToDouble(point.IndependentValue, CultureInfo.InvariantCulture)

            For j = 0 To dimension
                x(i, j) = Math.Pow(independentValue, j)
            Next j

            y(i) = Convert.ToDouble(point.DependentValue, CultureInfo.InvariantCulture)
        Next i

        ' Create the equations
        Dim matrix(dimension)() As Double
        For i = 0 To dimension
            ' Create the row
            matrix(i) = New Double(dimension + 2 - 1) {}

            ' indeterminate coefficients
            For j = 0 To dimension
                matrix(i)(j) = 0.0
                For k = 0 To activePoints.Count - 1
                    matrix(i)(j) += x(k, i) * x(k, j)
                Next k
            Next j

            ' determinate values
            For k = 0 To activePoints.Count - 1
                matrix(i)(dimension + 1) += x(k, i) * y(k)
            Next k
        Next i

        ' Convert to row-echelon form
        i = 0
        j = 0
        Do While i <= dimension AndAlso j <= dimension
            ' Get the pivot in column j starting at row i
            Dim pivotRow As Integer = i
            For k = i To dimension
                If Math.Abs(matrix(k)(j)) > Math.Abs(matrix(pivotRow)(j)) Then
                    pivotRow = k
                End If
            Next k
            Dim pivot As Double = matrix(pivotRow)(j)

            ' If we have a pivot element
            If pivot <> 0 Then
                ' Swap the current row with the pivot row
                Dim temp() As Double = matrix(i)
                matrix(i) = matrix(pivotRow)
                matrix(pivotRow) = temp
                pivotRow = i

                ' Normalize the pivot row to the pivot
                Dim c As Double = matrix(i)(j)
                For k = 0 To dimension + 1
                    matrix(i)(k) /= c
                Next k

                ' Clear out the pivot position from the remaining rows
                For k = i + 1 To dimension
                    c = matrix(k)(j)
                    For m As Integer = i To dimension + 1
                        matrix(k)(m) -= c * matrix(i)(m)
                    Next m
                Next k

                i += 1
            End If

            j += 1
        Loop

        ' Solve using substitution
        For i = dimension - 1 To 0 Step -1
            For j = dimension To i + 1 Step -1
                matrix(i)(dimension + 1) -= matrix(i)(j) * matrix(j)(dimension + 1)
                matrix(i)(j) = 0
            Next j
        Next i

        ' Capture the coefficients
        Dim a0 As Double = matrix(0)(dimension + 1)
        Dim a1 As Double = matrix(1)(dimension + 1)
        Dim a2 As Double = If((dimension >= 2), matrix(2)(dimension + 1), Double.NaN)
        Dim a3 As Double = If((dimension >= 3), matrix(3)(dimension + 1), Double.NaN)
        Dim a4 As Double = If((dimension = 4), matrix(4)(dimension + 1), Double.NaN)

        ' Create the function
        Dim [function] As Func(Of Double, Double) = Nothing
        Select Case dimension
            Case 1
                [function] = Function(z) a1 * z + a0
            Case 2
                [function] = Function(z) a2 * z * z + a1 * z + a0
            Case 3
                [function] = Function(z) a3 * z * z * z + a2 * z * z + a1 * z + a0
            Case 4
                [function] = Function(z) a4 * z * z * z * z + a3 * z * z * z + a2 * z * z + a1 * z + a0
        End Select

        ' Create the title
        Dim title As StackPanel = New StackPanel With {.Orientation = Orientation.Horizontal}
        title.Children.Add(New TextBlock With _
                            { _
                                .Text = "f(x) = ", _
                                .Margin = New Thickness(0, 4, 0, 0) _
                            })

        title.Children.Add(New TextBlock With _
                            { _
                                .Text = a0.ToString("N3", CultureInfo.InvariantCulture), _
                                .Margin = New Thickness(0, 4, 0, 0) _
                            })
        AddTitleTerm(title, a1, 1)
        If dimension >= 2 Then
            AddTitleTerm(title, a2, 2)
        End If
        If dimension >= 3 Then
            AddTitleTerm(title, a3, 3)
        End If
        If dimension = 4 Then
            AddTitleTerm(title, a4, 4)
        End If

        ' Set the function and the title
        regressionSeries.Function = [function]
        regressionSeries.Title = title
    End Sub

    ''' <summary>
    ''' Add a term to the title.
    ''' </summary>
    ''' <param name="title">The title container.</param>
    ''' <param name="value">The value of the term.</param>
    ''' <param name="exponent">The exponent of the term.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Called by a method called by an event handler in XAML.")> _
    Private Shared Sub AddTitleTerm(ByVal title As StackPanel, ByVal value As Double, ByVal exponent As Integer)
        If value = 0 Then
            Return
        End If

        title.Children.Add(New TextBlock With _
                            { _
                                .Text = If(value >= 0, " + ", " - "), _
                                .Margin = New Thickness(0, 4, 0, 0) _
                            })
        title.Children.Add(New TextBlock With _
                            { _
                                .Text = String.Format(CultureInfo.InvariantCulture, "{0:N3}x", Math.Abs(value)), _
                                .Margin = New Thickness(0, 4, 0, 0) _
                            })

        If exponent > 1 Then
            title.Children.Add(New TextBlock With _
                                { _
                                    .Text = exponent.ToString(CultureInfo.InvariantCulture), _
                                    .FontSize = 8 _
                                })
        End If
    End Sub
End Class
