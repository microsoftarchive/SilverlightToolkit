' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Globalization
Imports System.Windows
Imports System.Windows.Browser
Imports System.Windows.Controls
Imports System.ComponentModel

''' <summary>
''' The AutoCompleteLambda sample page demonstrates using a custom data 
''' template, binding to elements in a rich CLR type, and using a lambda 
''' expression to provide custom search filtering capabilities to the 
''' AutoCompleteBox control.
''' </summary>
<Sample("(1)ItemFilter lambda", DifficultyLevel.Basic, "AutoCompleteBox")> _
Partial Public Class AutoCompleteLambda
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the AutoCompleteLambda class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Handle the Loaded event of the page.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event arguments.</param>
    Private Sub Airports_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Loaded
        ' Provide airport data and a custom filter
        Dim airports As ObjectCollection = Airport.SampleAirports
        DepartureAirport.ItemsSource = airports
        ArrivalAirport.ItemsSource = airports
        DepartureAirport.ItemFilter = AddressOf MyItemFilter
        ArrivalAirport.ItemFilter = DepartureAirport.ItemFilter

        ' Set the date defaults in code
        DepartureDate.SelectedDate = DateTime.UtcNow + TimeSpan.FromDays(7)
        ArrivalDate.SelectedDate = DateTime.UtcNow + TimeSpan.FromDays(14)
    End Sub

    ''' <summary>
    ''' AutoComplete search predicate
    ''' </summary>
    ''' <param name="prefix">The search string.</param>
    ''' <param name="item">The target object.</param>
    Private Function MyItemFilter(ByVal search As String, ByVal item As Object) As Boolean
        Dim airport As Airport = TryCast(item, Airport)
        If airport IsNot Nothing Then
            Dim filter As String = search.ToUpper(CultureInfo.InvariantCulture)
            Return (airport.CodeFaa.ToUpper(CultureInfo.InvariantCulture).Contains(filter) _
                    OrElse airport.City.ToUpper(CultureInfo.InvariantCulture).Contains(filter) _
                    OrElse airport.Name.ToUpper(CultureInfo.InvariantCulture).Contains(filter))
        End If
        Return False
    End Function

    ''' <summary>
    ''' The button to book the flight has been clicked.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub BookFlight_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles BookFlight.Click
        Dim d As Airport = CType(DepartureAirport.SelectedItem, Airport)
        Dim a As Airport = CType(ArrivalAirport.SelectedItem, Airport)
        Dim dd As DateTime = CDate(DepartureDate.SelectedDate)
        Dim ad As DateTime = CDate(ArrivalDate.SelectedDate)
        Dim p As Integer = CInt(Fix(Passengers.Value))

        Dim travel As Uri = WebServiceHelper.CreateAirfareSearchUri(d, a, dd, ad, p)
        HtmlPage.Window.Navigate(travel, "_new")
    End Sub

    ''' <summary>
    ''' Update the form when valid values are present in the controls.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub SelectedItemsChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs) _
        Handles DepartureAirport.SelectionChanged, ArrivalAirport.SelectionChanged, _
        DepartureDate.SelectedDateChanged, ArrivalDate.SelectedDateChanged

        BookFlight.IsEnabled = DepartureAirport.SelectedItem IsNot Nothing AndAlso _
        ArrivalAirport.SelectedItem IsNot Nothing AndAlso _
        DepartureDate.SelectedDate IsNot Nothing AndAlso _
        ArrivalDate.SelectedDate IsNot Nothing AndAlso _
        Passengers.Value > 0
    End Sub
End Class

