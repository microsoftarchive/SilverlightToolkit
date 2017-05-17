' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.ComponentModel
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System
Imports System.Globalization

''' <summary>
''' Sample application for Accordion.
''' </summary>
<Sample("Accordion Playaround sample", DifficultyLevel.Basic, "Accordion")> _
Partial Public Class AccordionSample
    Inherits UserControl
    ''' <summary>
    ''' Initializes a new instance of the <see cref="AccordionSample"/> class.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification:="InitializeComponent Required by Visual Studio.")> _
    Public Sub New()
        InitializeComponent()

        icSelectedIndices.SetBinding(ItemsControl.ItemsSourceProperty, New Binding("SelectedIndices") With {.Source = acc, .Mode = BindingMode.OneWay})

        tbSelectedIndex.SetBinding(TextBox.TextProperty, New Binding("SelectedIndex") With {.Source = acc, .Mode = BindingMode.TwoWay})

        acc.SetBinding(Accordion.SelectedIndexProperty, New Binding("Index") With {.Source = Me, .Mode = BindingMode.TwoWay})

        cbSelectionMode.SelectedItem = cbSelectionMode.Items.OfType(Of ComboBoxItem)().FirstOrDefault(Function(item) item.Content.Equals(acc.SelectionMode.ToString()))

        cbExpandDirection.SelectedItem = cbExpandDirection.Items.OfType(Of ComboBoxItem)().FirstOrDefault(Function(item) item.Content.Equals(acc.ExpandDirection.ToString()))

        cbSelectionSequence.SelectedItem = cbSelectionSequence.Items.OfType(Of ComboBoxItem)().FirstOrDefault(Function(item) item.Content.Equals(acc.SelectionSequence.ToString()))
    End Sub

    ''' <summary>
    ''' Gets or sets the index.
    ''' </summary>
    ''' <value>The index.</value>
    Public Property Index() As Integer
        Get
            Return index_Renamed
        End Get
        Set(ByVal value As Integer)
            index_Renamed = value
            tbSelectedIndex.Text = value.ToString(CultureInfo.InvariantCulture)
        End Set
    End Property

    ''' <summary>
    ''' Backingfield for Index.
    ''' </summary>
    Private index_Renamed As Integer

    ''' <summary>
    ''' Expands the direction changed.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub ExpandDirectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        acc.ExpandDirection = CType(System.Enum.Parse(GetType(ExpandDirection), (CType(cbExpandDirection.SelectedItem, ComboBoxItem)).Content.ToString(), True), ExpandDirection)
    End Sub

    ''' <summary>
    ''' Sets the height.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub SetHeight(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        acc.Height = 500
    End Sub

    ''' <summary>
    ''' Removes the height.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub RemoveHeight(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        acc.ClearValue(Control.HeightProperty)
    End Sub

    ''' <summary>
    ''' Selections the mode changed.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub SelectionModeChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        acc.SelectionMode = CType(System.Enum.Parse(GetType(AccordionSelectionMode), (CType(cbSelectionMode.SelectedItem, ComboBoxItem)).Content.ToString(), True), AccordionSelectionMode)
    End Sub

    ''' <summary>
    ''' Selects all.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub SelectAll(ByVal sender As Object, ByVal e As RoutedEventArgs)
        acc.SelectAll()
    End Sub

    ''' <summary>
    ''' Unselects all.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub UnselectAll(ByVal sender As Object, ByVal e As RoutedEventArgs)
        acc.UnselectAll()
    End Sub

    ''' <summary>
    ''' React to selectionSequence event.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub SelectionSequenceChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        acc.SelectionSequence = CType(System.Enum.Parse(GetType(SelectionSequence), (CType(cbSelectionSequence.SelectedItem, ComboBoxItem)).Content.ToString(), True), SelectionSequence)
    End Sub

    ''' <summary>
    ''' Sets the width.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub SetWidth(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        acc.Width = 300
    End Sub

    ''' <summary>
    ''' Removes the width.
    ''' </summary>
    ''' <param name="sender">The sender.</param>
    ''' <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification:="Hooked up in Xaml.")> _
    Private Sub RemoveWidth(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        acc.ClearValue(Control.WidthProperty)
    End Sub
End Class
