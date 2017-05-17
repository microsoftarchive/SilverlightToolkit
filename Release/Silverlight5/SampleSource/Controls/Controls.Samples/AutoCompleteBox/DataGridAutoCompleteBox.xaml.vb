' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Globalization
Imports System.Windows
Imports System.Windows.Controls
Imports System.ComponentModel

''' <summary>
''' A sample AutoCompleteBox with a DataGrid selection adapter.
''' </summary>
<Sample("DataGrid", DifficultyLevel.Advanced, "AutoCompleteBox")> _
Partial Public Class DataGridAutoCompleteBox
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the type.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Handle the loaded event.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub OnLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Loaded
        ' Bind to the sample airport data
        MyAutoCompleteBox.ItemsSource = Airport.SampleAirports

        ' A custom search, the same that is used in the basic lambda file
        MyAutoCompleteBox.FilterMode = AutoCompleteFilterMode.Custom
        MyAutoCompleteBox.ItemFilter = AddressOf MyItemFilter
    End Sub

    ''' <summary>
    ''' AutoComplete search predicate
    ''' </summary>
    ''' <param name="search">The search string.</param>
    ''' <param name="item">The target object.</param>
    Private Function MyItemFilter(ByVal search As String, ByVal item As Object) As Boolean
        Dim airport As Airport = TryCast(item, Airport)
        If airport IsNot Nothing Then
            Dim filter As String = search.ToUpper(CultureInfo.InvariantCulture)
            Return (airport.CodeFaa.ToUpper(CultureInfo.InvariantCulture).Contains(filter) OrElse _
                    airport.City.ToUpper(CultureInfo.InvariantCulture).Contains(filter) OrElse _
                    airport.Name.ToUpper(CultureInfo.InvariantCulture).Contains(filter))
        End If
        Return False
    End Function
End Class
