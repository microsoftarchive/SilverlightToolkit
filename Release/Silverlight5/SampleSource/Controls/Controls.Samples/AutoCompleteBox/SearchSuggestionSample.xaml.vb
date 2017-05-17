' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics.CodeAnalysis
Imports System.Json
Imports System.Linq
Imports System.Net
Imports System.Windows
Imports System.Windows.Browser
Imports System.Windows.Controls
Imports System.ComponentModel

''' <summary>
''' A simple auto complete search suggestions sample that connects to a 
''' real web service.
''' </summary>
<Sample("Search Suggestions", DifficultyLevel.Scenario, "AutoCompleteBox")> _
Partial Public Class SearchSuggestionSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the SearchSuggestionSample class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Handles the Loaded event by initializing the control for live web 
    ''' service use if the stack is available.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub OnLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Loaded
        If WebServiceHelper.CanMakeHttpRequests Then
            HostingWarning.Visibility = Visibility.Collapsed
            Go.IsEnabled = True
            Search.IsEnabled = True
        End If
    End Sub

    ''' <summary>
    ''' Handle and cancel the Populating event, and kick off the web service
    ''' request.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub Search_Populating(ByVal sender As Object, ByVal e As PopulatingEventArgs) Handles Search.Populating
        Dim autoComplete As AutoCompleteBox = CType(sender, AutoCompleteBox)

        ' Allow us to wait for the response
        e.Cancel = True

        ' Create a request for suggestion
        Dim wc As New WebClient()
        AddHandler wc.DownloadStringCompleted, AddressOf OnDownloadStringCompleted
        wc.DownloadStringAsync(WebServiceHelper.CreateWebSearchSuggestionsUri(autoComplete.SearchText), autoComplete)
    End Sub

    ''' <summary>
    ''' Handle the AutoCompleteBox KeyUp event, and kick off the web service
    ''' request when the Enter key is pressed.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub Search_KeyUp(ByVal sender As Object, ByVal e As Input.KeyEventArgs) Handles Search.KeyUp
        If e.Key = System.Windows.Input.Key.Enter Then
            CallWebServiceWithSearchString(Search.Text)
        End If
    End Sub

    ''' <summary>
    ''' Handle the Go button Click event, and kick off the web service
    ''' request.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub Go_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Go.Click
        CallWebServiceWithSearchString(Search.Text)
    End Sub

    ''' <summary>
    ''' Kick off the web service request.
    ''' </summary>
    ''' <param name="searchText">The search text.</param>
    Private Shared Sub CallWebServiceWithSearchString(ByRef searchText As String)
        HtmlPage.Window.Navigate(WebServiceHelper.CreateWebSearchUri(searchText), "_blank")
    End Sub

    ''' <summary>
    ''' Handle the string download completed event of WebClient.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    <SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification:="Any failure in the Json or request parsing should not be surfaced.")> _
    Private Sub OnDownloadStringCompleted(ByVal sender As Object, ByVal e As DownloadStringCompletedEventArgs)
        Dim autoComplete As AutoCompleteBox = TryCast(e.UserState, AutoCompleteBox)
        If autoComplete IsNot Nothing AndAlso e.Error Is Nothing AndAlso (Not e.Cancelled) AndAlso (Not String.IsNullOrEmpty(e.Result)) Then
            Dim data As List(Of String) = New List(Of String)()
            Try
                Dim result As JsonArray = TryCast((CType(JsonArray.Parse(e.Result), JsonArray)), JsonArray)
                If result.Count > 1 Then
                    Dim originalSearchString As String = result(0)
                    If originalSearchString = autoComplete.SearchText Then
                        For Each suggestion As JsonPrimitive In CType(result(1), JsonArray)
                            data.Add(suggestion)
                        Next suggestion

                        ' Diplay the AutoCompleteBox drop down with any suggestions
                        autoComplete.ItemsSource = data
                        autoComplete.PopulateComplete()
                    End If
                End If
            Catch
            End Try
        End If
    End Sub
End Class
