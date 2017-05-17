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

''' <summary>
''' Sample page demonstrating the HeaderedItemsControl.
''' </summary>
<SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId:="Headered", Justification:="Name of the control"), Sample("(4)HeaderedItemsControl", DifficultyLevel.Basic, "TreeView")> _
Partial Public Class HeaderedItemsControlSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the HeaderedItemsControlSample class.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Load the demonstration.
    ''' </summary>
    ''' <param name="sender">Sample page.</param>
    ''' <param name="e">Event arguments.</param>
    Private Sub OnLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Loaded
        MicrosoftExecutives.ItemsSource = Employee.Executives
    End Sub
End Class
