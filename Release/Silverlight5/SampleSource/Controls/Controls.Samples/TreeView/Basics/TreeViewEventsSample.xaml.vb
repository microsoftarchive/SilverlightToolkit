' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports System.Windows
Imports System.Windows.Controls
Imports System.ComponentModel

''' <summary>
''' Sample demonstrating TreeView and TreeViewItem events.
''' </summary>
<Sample("(3)Events", DifficultyLevel.Basic, "TreeView")> _
Partial Public Class TreeViewEventsSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the TreeViewEventsSample class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Handle the TreeView.SelectedItemChanged event.
    ''' </summary>
    ''' <param name="sender">The TreeView.</param>
    ''' <param name="e">Event arguments.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="The event handler is declared in XAML.")> _
    Private Sub OnSelectedItemChanged(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of Object))
        ' Note: Our sample added actual TreeViewItems to the TreeView.Items
        ' collection (instead of proding other CLR or business objects
        ' directly to the Items or ItemsSource properties).  That means
        ' e.OldValue and e.NewValue will be TreeViewItems instead of
        ' strings, business objects, etc.
        Dim oldItem As TreeViewItem = TryCast(e.OldValue, TreeViewItem)
        Dim newItem As TreeViewItem = TryCast(e.NewValue, TreeViewItem)
        Log(String.Format(CultureInfo.CurrentUICulture, "TreeView: SelectedItemChanged from '{0}' to '{1}'", If(oldItem IsNot Nothing, TryCast(oldItem.Header, String), "(null)"), If(newItem IsNot Nothing, TryCast(newItem.Header, String), "(null)")))
    End Sub

    ''' <summary>
    ''' Handle the TreeViewItem.Selected event.
    ''' </summary>
    ''' <param name="sender">The TreeViewItem.</param>
    ''' <param name="e">Event arguments.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="The event handler is declared in XAML.")> _
    Private Sub OnSelected(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim item As TreeViewItem = TryCast(sender, TreeViewItem)
        Log(item, "Selected")
    End Sub

    ''' <summary>
    ''' Handle the TreeViewItem.Unselected event.
    ''' </summary>
    ''' <param name="sender">The TreeViewItem.</param>
    ''' <param name="e">Event arguments.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="The event handler is declared in XAML.")> _
    Private Sub OnUnselected(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim item As TreeViewItem = TryCast(sender, TreeViewItem)
        Log(item, "Unselected")
    End Sub

    ''' <summary>
    ''' Handle the TreeViewItem.Expanded event.
    ''' </summary>
    ''' <param name="sender">The TreeViewItem.</param>
    ''' <param name="e">Event arguments.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="The event handler is declared in XAML.")> _
    Private Sub OnExpanded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim item As TreeViewItem = TryCast(sender, TreeViewItem)
        Log(item, "Expanded")
    End Sub

    ''' <summary>
    ''' Handle the TreeViewItem.Collapsed event.
    ''' </summary>
    ''' <param name="sender">The TreeViewItem.</param>
    ''' <param name="e">Event arguments.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="The event handler is declared in XAML.")> _
    Private Sub OnCollapsed(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim item As TreeViewItem = TryCast(sender, TreeViewItem)
        Log(item, "Collapsed")
    End Sub

    ''' <summary>
    ''' Add an event to the list of raised events on the demo.
    ''' </summary>
    ''' <param name="message">The message to log.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Called by an event handler declared in XAML.")> _
    Private Sub Log(ByVal message As String)
        ' Add a new message to the event log
        message = String.Format(CultureInfo.CurrentUICulture, "[{0:hh:mm:ss}]  {1}", DateTime.Now, message)
        EventLog.Children.Add(New TextBlock With {.Text = message})

        ' Scroll to the bottom of the event log
        EventViewer.ScrollToVerticalOffset(EventViewer.ExtentHeight)
    End Sub

    ''' <summary>
    ''' Add an event to the list of raised events on the demo.
    ''' </summary>
    ''' <param name="item">The item that raised the event.</param>
    ''' <param name="eventName">The name of the event to log.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Called by an event handler declared in XAML.")> _
    Private Sub Log(ByVal item As TreeViewItem, ByVal eventName As String)
        Log(String.Format(CultureInfo.CurrentUICulture, "TreeViewItem '{0}': {1}", If(item IsNot Nothing, TryCast(item.Header, String), "(null)"), eventName))
    End Sub

    ''' <summary>
    ''' Clear the event log.
    ''' </summary>
    ''' <param name="sender">The Button.</param>
    ''' <param name="e">Event arguments.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="The event handler is declared in XAML.")> _
    Private Sub OnClearLog(ByVal sender As Object, ByVal e As RoutedEventArgs)
        EventLog.Children.Clear()
    End Sub
End Class
