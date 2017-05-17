' (c) Copyright Microsoft Corporation.
' This source is subject to <###LICENSE_NAME###>.
' Please see <###LICENSE_LINK###> for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Linq
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports System.Globalization

''' <summary>
''' Sample page for Accordion, showing usages.
''' </summary>
<Sample("Accordion Usage samples", DifficultyLevel.Basic, "Accordion")> _
Partial Public Class AccordionUsage
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the <see cref="AccordionUsage"/> class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()

        Dim data() As KeyValuePair(Of String, String) = {New KeyValuePair(Of String, String)("Hello", "World"), New KeyValuePair(Of String, String)("foo", "bar"), New KeyValuePair(Of String, String)("Silverlight", "Toolkit")}

        ' initialize accordions
        accordionGeneratedContent.ItemsSource = data
        accordionDefaultHeaderTemplate.ItemsSource = data
        accordionCLRTypes.ItemsSource = data
        accordionAccordionItem.ItemsSource = data
    End Sub

    ''' <summary>
    ''' Handles the SelectionChanged event of the accordion control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml")> _
    Private Sub Accordion_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        For Each SelectedAccordionItem As AccordionItem In e.AddedItems
            Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "AccordionItem {0} has been selected.", SelectedAccordionItem.Header))
        Next SelectedAccordionItem
    End Sub

    ''' <summary>
    ''' Handles the Click event of the btnExpandAll control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml")> _
    Private Sub ExpandAll_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        accordionExpandCollapse.SelectAll()
    End Sub

    ''' <summary>
    ''' Handles the Click event of the btnCollapseAll control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml")> _
    Private Sub CollapseAll_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        accordionExpandCollapse.UnselectAll()
    End Sub

    ''' <summary>
    ''' Handles the SelectionChanged event for the CLRTypes sample.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml")> _
    Private Sub CLRTypesSelectedItemsChanged(ByVal sender As Object, ByVal e As NotifyCollectionChangedEventArgs)
        For Each keyValuePair As KeyValuePair(Of String, String) In accordionCLRTypes.SelectedItems
            Debug.WriteLine("Interesting, people like seeing details on " & keyValuePair.Key)
        Next keyValuePair
    End Sub

    ''' <summary>
    ''' Hooks up events for the MouseOver sample.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml")> _
    Private Sub SetMouseEvents(ByVal sender As Object, ByVal e As RoutedEventArgs)
        For Each keyValuePair As KeyValuePair(Of String, String) In accordionAccordionItem.Items
            Dim container As AccordionItem = CType(accordionAccordionItem.ItemContainerGenerator.ContainerFromItem(keyValuePair), AccordionItem)
            AddHandler container.MouseEnter, AddressOf Mouse_Enter
            AddHandler container.MouseLeave, AddressOf Mouse_Leave
        Next keyValuePair
    End Sub

    ''' <summary>
    ''' Handle the MouseEnter event.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
    Private Sub Mouse_Enter(ByVal sender As Object, ByVal e As MouseEventArgs)
        Dim container As AccordionItem = CType(sender, AccordionItem)
        If (Not container.IsLocked) Then
            container.IsSelected = True
        End If
    End Sub

    ''' <summary>
    ''' Handle the MouseLeave event.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
    Private Sub Mouse_Leave(ByVal sender As Object, ByVal e As MouseEventArgs)
        Dim container As AccordionItem = CType(sender, AccordionItem)
        If (Not container.IsLocked) Then
            container.IsSelected = False
        End If
    End Sub
End Class