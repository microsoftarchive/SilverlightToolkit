' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics.CodeAnalysis
Imports System.Windows
Imports System.Windows.Controls
Imports System.ComponentModel

' Global suppressions for this sample
<Assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope := "member", Target := "System.Windows.Controls.Samples.CustomEvents.#Value1")>
<Assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope := "member", Target := "System.Windows.Controls.Samples.CustomEvents.#Value2")>
<Assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope := "member", Target := "System.Windows.Controls.Samples.CustomEvents.#Value3")>
<Assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope := "member", Target := "System.Windows.Controls.Samples.CustomEvents.#Value4")>

''' <summary>
''' The PopulationEvents class shows how a developer might hook into the 
''' population events to provide custom data.
''' </summary>
<Sample("(2)Custom Events", DifficultyLevel.Basic, "AutoCompleteBox")> _
Partial Public Class CustomEvents
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the type.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' The Populating event handler.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub OnPopulatingSynchronous(ByVal sender As Object, ByVal e As PopulatingEventArgs) _
        Handles NowAutoComplete.Populating, NowAutoComplete2.Populating

        Dim source As AutoCompleteBox = CType(sender, AutoCompleteBox)

        source.ItemsSource = New String() _
        { _
            e.Parameter & "1", _
            e.Parameter & "2", _
            e.Parameter & "3" _
        }
    End Sub

    ''' <summary>
    ''' The populating handler.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub OnPopulatingAsynchronous(ByVal sender As Object, ByVal e As PopulatingEventArgs) _
        Handles LaterAutoComplete.Populating, LaterAutoComplete2.Populating

        ' Cancel the populating value: this will allow us to call 
        ' PopulateComplete as necessary.
        e.Cancel = True

        ' Use the dispatcher to simulate an asynchronous callback when 
        ' data becomes available
        Dispatcher.BeginInvoke(New EventHandler(AddressOf PopulateHandler), sender, e)
    End Sub

    ''' <summary>
    ''' The asynchronous populating method.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub PopulateHandler(ByVal sender As Object, ByVal e As EventArgs)
        Dim source As AutoCompleteBox = CType(sender, AutoCompleteBox)
        Dim args As PopulatingEventArgs = CType(e, PopulatingEventArgs)

        source.ItemsSource = New String() _
        { _
            args.Parameter & "1", _
            args.Parameter & "2", _
            args.Parameter & "3" _
        }

        ' Population is complete
        source.PopulateComplete()
    End Sub

    ''' <summary>
    ''' Called when an AutoCompleteBox's selected value changes. Uses the 
    ''' Tag property to identify the content presenter to be updated.
    ''' </summary>
    ''' <param name="sender">The source AutoCompleteBox control.</param>
    ''' <param name="e">The event data.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Event handler is wired up in XAML.")> _
    Private Sub OnSelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        Dim acb As AutoCompleteBox = CType(sender, AutoCompleteBox)

        ' In these sample scenarios, the Tag is the name of the content 
        ' presenter to use to display the value.
        Dim contentPresenterName As String = CStr(acb.Tag)
        Dim cp As ContentPresenter = TryCast(FindName(contentPresenterName), ContentPresenter)
        If cp IsNot Nothing Then
            cp.Content = acb.SelectedItem
        End If
    End Sub
End Class
