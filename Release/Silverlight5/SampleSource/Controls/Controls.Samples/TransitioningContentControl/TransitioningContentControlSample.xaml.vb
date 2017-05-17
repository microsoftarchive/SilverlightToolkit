' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis

''' <summary>
''' Sample for the experimental control: TransitioningContentControl.
''' </summary>
''' <QualityBand>Experimental</QualityBand>
<Sample("TransitioningContentControl", DifficultyLevel.Basic, "TransitioningContentControl")> _
Partial Public Class TransitioningContentControlSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the <see cref="TransitioningContentControlSample"/> class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Handles the Loaded event.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    Private Sub OnLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Loaded
        dud.ItemsSource = Airport.SampleAirports
    End Sub

    ''' <summary>
    ''' Changes the content with the default transition.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Used by Xaml.")> _
    Private Sub ChangeContentSample1(ByVal sender As Object, ByVal e As RoutedEventArgs)
        defaultTCC.Transition = TransitioningContentControl.DefaultTransitionState
        defaultTCC.Content = DateTime.Now.Ticks
    End Sub

    ''' <summary>
    ''' Changes the content with the down transition.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Used by Xaml.")> _
    Private Sub ChangeContentSample1Down(ByVal sender As Object, ByVal e As RoutedEventArgs)
        defaultTCC.Transition = "DownTransition"
        defaultTCC.Content = DateTime.Now.Ticks
    End Sub

    ''' <summary>
    ''' Changes the content up transition.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Used by Xaml.")> _
    Private Sub ChangeContentSample1Up(ByVal sender As Object, ByVal e As RoutedEventArgs)
        defaultTCC.Transition = "UpTransition"
        defaultTCC.Content = DateTime.Now.Ticks
    End Sub
End Class