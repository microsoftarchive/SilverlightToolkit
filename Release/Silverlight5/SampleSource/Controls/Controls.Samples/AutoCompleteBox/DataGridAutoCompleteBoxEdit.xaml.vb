' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.ComponentModel

''' <summary>
''' The DataGridAutoCompleteBoxEdit class selects a small set of data to 
''' display in the DataGrid. The XAML file contains the custom editing 
''' template for AutoCompleteBox.
''' </summary>
<Sample("DataGrid Editing", DifficultyLevel.Advanced, "AutoCompleteBox")> _
Partial Public Class DataGridAutoCompleteBoxEdit
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the DataGridAutoCompleteBoxEdit type.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Handle the Loaded event of the page. This creates a small, random 
    ''' set of data to display in the grid.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event arguments.</param>
    Private Sub OnLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Loaded
        Dim data As List(Of RandomEmployeeDetails) = New List(Of RandomEmployeeDetails)()
        Dim random As New Random()

        ' Select up to 8 random employees
        For Each employee As Employee In New SampleEmployeeCollection().Where(Function(item) random.Next(2) = 0).Distinct().Take(8)
            data.Add(New RandomEmployeeDetails(employee))
        Next employee
        MyDataGrid.ItemsSource = data
    End Sub
End Class
