' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System
Imports System.Windows
Imports System.Collections.Generic
Imports System.Linq

''' <summary>
''' System.Windows.Controls samples application.
''' </summary>
Partial Public Class App
    Inherits Application

    ''' <summary>
    ''' Initializes a new instance of the App class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Gets a collection of SampleTreeItems to populate the SampleBrowser TreeView.
    ''' </summary>
    Public Shared ReadOnly Property SampleTreeItems() As IEnumerable(Of SampleTreeItem)
        Get
            Dim data As IEnumerable(Of Object) = DirectCast(Application.Current.Resources("SampleTreeView"), IEnumerable(Of Object))
            Return CType(If(data IsNot Nothing, data.OfType(Of SampleTreeItem)(), Enumerable.Empty(Of SampleTreeItem)()), IEnumerable(Of SampleTreeItem))
        End Get
    End Property

    ''' <summary>
    ''' Handles the Application.Startup event to set the root visual to the sample browser
    ''' </summary>
    Private Sub MyStartupEventHandler(ByVal sender As Object, ByVal e As StartupEventArgs) Handles Me.Startup
        RootVisual = CType(New SampleBrowser(Me.GetType.Assembly, SampleTreeItems), UIElement)
    End Sub
End Class