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
<Assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope := "member", Target := "System.Windows.Controls.Samples.AutoCompleteBoxSample.#Value1")>
<Assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope := "member", Target := "System.Windows.Controls.Samples.AutoCompleteBoxSample.#Value2")>
<Assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope := "member", Target := "System.Windows.Controls.Samples.AutoCompleteBoxSample.#Value3")>
<Assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope := "member", Target := "System.Windows.Controls.Samples.AutoCompleteBoxSample.#Value4")>
<Assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope := "member", Target := "System.Windows.Controls.Samples.AutoCompleteBoxSample.#Value5")>
<Assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope := "member", Target := "System.Windows.Controls.Samples.AutoCompleteBoxSample.#Value6")>
<Assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope := "member", Target := "System.Windows.Controls.Samples.AutoCompleteBoxSample.#Value7")>

''' <summary>
''' The AutoCompleteGettingStarted sample page shows several common uses 
''' of the AutoCompleteBox control.
''' </summary>
<Sample("(0)AutoCompleteBox", DifficultyLevel.Basic, "AutoCompleteBox")> _
Partial Public Class AutoCompleteBoxSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the AutoCompleteGettingStarted class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Hook up to the Loaded event.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub OnLoaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        ' Words
        WordComplete.ItemsSource = Words.GetAliceInWonderland()

        ' Sliders
        AddHandler SetDelay.ValueChanged, Function(s, args) DynamicDelayAutoCompleteBox.MinimumPopulateDelay = CInt(Fix(Math.Floor(SetDelay.Value)))
        AddHandler SetPrefixLength.ValueChanged, Function(s, args) WordComplete.MinimumPrefixLength = CInt(Fix(Math.Floor(SetPrefixLength.Value)))
    End Sub

    ''' <summary>
    ''' Called when an AutoCompleteBox's selected value changes. Uses the 
    ''' Tag property to identify the content presenter to be updated.
    ''' </summary>
    ''' <param name="sender">The source AutoCompleteBox control.</param>
    ''' <param name="e">The event data.</param>
    <SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Event is wired up in XAML.")> _
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
