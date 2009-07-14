' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics.CodeAnalysis
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Controls.Theming
Imports System.Collections.Generic
Imports System.Windows.Controls.DataVisualization.Charting
Imports System.Collections
Imports System.Linq

''' <summary>
''' A user control with examples of every control to demonstrate themes.
''' </summary>
Partial Public Class AllControls
    Inherits UserControl
    ''' <summary>
    ''' Gets or sets the Background Brush for this user Control.
    ''' </summary>
    Public Property PreferredBackground() As Brush
        Get
            Return Root.Background
        End Get
        Set(ByVal value As Brush)
            Root.Background = value
        End Set
    End Property

    ''' <summary>
    ''' Initializes a new instance of the AllControls class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()

        ' Use AddHandler here because RemoveHandler is called by handler
        AddHandler LayoutUpdated, AddressOf OnLayoutUpdated
        SampleDataGrid.ItemsSource = Employee.Executives
        SampleAutoComplete.ItemsSource = Catalog.VacationMediaItems
    End Sub

    ''' <summary>
    ''' Initialize ImplicitStyleManager once the visual tree is ready.
    ''' </summary>
    ''' <param name="sender">The UserControl.</param>
    ''' <param name="e">Event arguments.</param>
    Private Sub OnLayoutUpdated(ByVal sender As Object, ByVal e As EventArgs)
        ' Only apply once
        RemoveHandler LayoutUpdated, AddressOf OnLayoutUpdated

        ' ImplicitStyleManager is design to only style controls in the
        ' namescope it was defined in.  Since user controls create new
        ' namescopes, the ImplicitStyleManager acting on the Theme controls
        ' will not style the controls in the AllControls user control.  By
        ' applying ImplicitStyleManager to the root of the user control
        ' (without giving it a theme to use), it will walk up the visual
        ' tree to the Theme control and use its styles.
        ImplicitStyleManager.SetApplyMode(Root, ImplicitStylesApplyMode.OneTime)
        ImplicitStyleManager.Apply(Root)
    End Sub

    ''' <summary>
    ''' Applies the style.
    ''' </summary>
    ''' <param name="uri">The URI of the theme used.</param>
    Public Sub ApplyStyle(ByVal uri As Uri)
        ImplicitStyleManager.SetResourceDictionaryUri(Me, uri)
        ImplicitStyleManager.SetApplyMode(Me, ImplicitStylesApplyMode.Auto)
        ImplicitStyleManager.Apply(Me)

        ' explicitly set content of expander.
        ImplicitStyleManager.SetApplyMode(CType(expander.Content, FrameworkElement), ImplicitStylesApplyMode.OneTime)
        ImplicitStyleManager.Apply(CType(expander.Content, FrameworkElement))
    End Sub
End Class
