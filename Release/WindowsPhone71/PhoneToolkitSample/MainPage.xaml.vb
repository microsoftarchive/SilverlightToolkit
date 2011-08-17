' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Partial Public Class MainPage
    Inherits PhoneApplicationPage
    ' Constructor
    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub NavigateTo(ByVal page As String)
        Me.NavigationService.Navigate(New Uri(page, UriKind.Relative))
    End Sub

    Private Sub OnToggleSwitch(ByVal sender As Object, ByVal e As RoutedEventArgs)
        NavigateTo("/Samples/ToggleSwitchSample.xaml")
    End Sub

    Private Sub OnTransitions(ByVal sender As Object, ByVal e As RoutedEventArgs)
        NavigateTo("/Samples/TransitionsSample.xaml")
    End Sub

    Private Sub OnTiltEffect(ByVal sender As Object, ByVal e As RoutedEventArgs)
        NavigateTo("/Samples/TiltEffectSample.xaml")
    End Sub

    Private Sub OnPerformanceProgressBar(ByVal sender As Object, ByVal e As RoutedEventArgs)
        NavigateTo("/Samples/PerformanceProgressBarSample.xaml")
    End Sub

    Private Sub OnContextMenu(ByVal sender As Object, ByVal e As RoutedEventArgs)
        NavigateTo("/Samples/ContextMenuSample.xaml")
    End Sub

    Private Sub OnDateTimePicker(ByVal sender As Object, ByVal e As RoutedEventArgs)
        NavigateTo("/Samples/DateTimePickerSample.xaml")
    End Sub

    Private Sub OnWrapPanel(ByVal sender As Object, ByVal e As RoutedEventArgs)
        NavigateTo("/Samples/WrapPanelSample.xaml")
    End Sub

    Private Sub OnGestures(ByVal sender As Object, ByVal e As RoutedEventArgs)
        NavigateTo("/Samples/GestureSample.xaml")
    End Sub

    Private Sub OnAutoCompleteBox(ByVal sender As Object, ByVal e As RoutedEventArgs)
        NavigateTo("/Samples/AutoCompleteBoxSample.xaml")
    End Sub

    Private Sub OnListPicker(ByVal sender As Object, ByVal e As RoutedEventArgs)
        NavigateTo("/Samples/ListPickerSample.xaml")
    End Sub

    Private Sub OnLongListSelector(ByVal sender As Object, ByVal e As RoutedEventArgs)
        NavigateTo("/Samples/LongListSelectorSample.xaml")
    End Sub
End Class

