' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports Microsoft.Windows
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Windows
Imports System.Windows.Input
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Controls.DataVisualization.Charting
Imports System.Linq

''' <summary>
''' A sample that demonstrates the use of the drag and drop functionality
''' in the Silverlight Toolkit.
''' </summary>
<Sample("Drag and Drop", DifficultyLevel.Advanced), Category("Drag and Drop")> _
Partial Public Class DragAndDropSample
	Inherits UserControl
    ''' <summary>
    ''' Flattens a tech employee.
    ''' </summary>
    ''' <param name="techEmployee">The tech employee.</param>
    ''' <returns>A list including the tech employee and all reports.</returns>
    Private Function FlattenTechEmployee(ByVal techEmployee As TechEmployee) As IEnumerable(Of TechEmployee)
        Dim myTechEmployees As New ObservableCollection(Of TechEmployee)
        myTechEmployees.Add(techEmployee)

        For Each employee As TechEmployee In techEmployee.Reports.SelectMany(Function(emp) FlattenTechEmployee(emp))
            myTechEmployees.Add(employee)
        Next employee

        Return myTechEmployees
    End Function

	''' <summary>
	''' Initializes a new instance of the DragAndDropSample class.
	''' </summary>
	Public Sub New()
		InitializeComponent()

        treeView.ItemsSource = TechEmployee.AllTechEmployees

		Dim allEmployees As New ObservableCollection(Of TechEmployee)()

        For Each employee As TechEmployee In TechEmployee.AllTechEmployees.SelectMany(Function(emp) FlattenTechEmployee(emp))
            allEmployees.Add(employee)
        Next employee

		fromListBox.ItemsSource = allEmployees

		Dim bugsCollection As New ObservableCollection(Of TechEmployee)()
		TryCast(bugsChart.Series(0), DataPointSeries).ItemsSource = bugsCollection
		TryCast(bugsChart.Series(1), DataPointSeries).ItemsSource = bugsCollection
		TryCast(salaryChart.Series(0), DataPointSeries).ItemsSource = New ObservableCollection(Of TechEmployee)()
	End Sub
End Class