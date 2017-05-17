' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Windows.Controls
Imports System.Windows.Controls.Theming
Imports System.Collections.Generic
Imports System.Collections
Imports System.Windows.Controls.DataVisualization.Charting
Imports System.ComponentModel

''' <summary>
''' Theme Sample Browser.
''' </summary>
<Sample("Theme Browser", DifficultyLevel.Basic, "Theme Browser")> _
Partial Public Class ThemeBrowserSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the ThemeBrowserSample class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()

        ThemesList.ItemsSource = ThemeCatalogItem.ThemesCatalog

        ' Show the first theme
        ShowTheme(ThemeCatalogItem.TwilightBlue)
    End Sub

    ''' <summary>
    ''' Fired when the selected item in the list changes.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">Event arguments.</param>
    Private Sub OnThemeChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs) Handles ThemesList.SelectionChanged
        ' Remove the current theme from the end of the DemoControls area
        DemoControls.Children.RemoveAt(1)

        ' Show the new theme
        ShowTheme(TryCast(ThemesList.SelectedItem, ThemeCatalogItem))
    End Sub

    ''' <summary>
    ''' Show the selected theme.
    ''' </summary>
    ''' <param name="item">The selected theme.</param>
    Private Sub ShowTheme(ByVal item As ThemeCatalogItem)
        Dim demo As New AllControls()
        AddHandler demo.Loaded, AddressOf demo_Loaded
        DemoControls.Children.Add(demo)
    End Sub

    ''' <summary>
    ''' Hook up to the Loaded event.
    ''' </summary>
    ''' <param name="sender">The source object.</param>
    ''' <param name="e">The event data.</param>
    Private Sub demo_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        System.Diagnostics.Debug.WriteLine("Loaded")
    End Sub
End Class
