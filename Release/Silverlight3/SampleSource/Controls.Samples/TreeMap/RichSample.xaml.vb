' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.


Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Windows.Data

<Assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope := "member", Target := "System.Windows.Controls.Samples.RichSample.#itemContainer")>
''' <summary>
''' Sample showing the TreeMap with richer visuals and functionality.
''' </summary>
<Sample("(5)Rich sample", DifficultyLevel.Intermediate), Category("TreeMap")> _
Partial Public Class RichSample
	Inherits UserControl
	''' <summary>
	''' Internal cache of the data source, used when refreshing the TreeMap.
	''' </summary>
	Private _dataSource As IList(Of NhlNode)

	''' <summary>
	''' Initializes a new instance of the RichSample class.
	''' </summary>
	Public Sub New()
		InitializeComponent()
		AddHandler Loaded, AddressOf RichSample_Loaded
	End Sub

	''' <summary>
	''' Loads the XML sample data and populates the TreeMap.
	''' </summary>
	''' <param name="sender">The object where the event handler is attached.</param>
	''' <param name="e">The event data.</param>
	Private Sub RichSample_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
		' Sample browser-specific layout change
		SampleHelpers.ChangeSampleAlignmentToStretch(Me)

		sizeMetric.SelectedIndex = 1
		colorMetric.SelectedIndex = 0

        _dataSource = NhlDataHelper.LoadDefaultFile()

		RefreshDataSource()
	End Sub

	''' <summary>
	''' Helper to force the TreeMap to refresh its data.
	''' </summary>
	Private Sub RefreshDataSource()
		treeMapControl.ItemsSource = Nothing
		treeMapControl.ItemsSource = _dataSource
	End Sub

	''' <summary>
	''' Changes the property used to generate sizes for rectangles.
	''' </summary>
	''' <param name="sender">The object where the event handler is attached.</param>
	''' <param name="e">The event data.</param>
	<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification := "Attached as an event handler in XAML")> _
	Private Sub OnSizeMetricSelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
		If e.AddedItems.Count > 0 Then
			Dim item As ComboBoxItem = TryCast(e.AddedItems(0), ComboBoxItem)
			treeMapControl.ItemDefinition.ValueBinding = New Binding(TryCast(item.Tag, String))

			RefreshDataSource()
		End If
	End Sub

	''' <summary>
	''' Changes the property used to set the background color on rectangles.
	''' </summary>
	''' <param name="sender">The object where the event handler is attached.</param>
	''' <param name="e">The event data.</param>
	<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification := "Attached as an event handler in XAML")> _
	Private Sub OnColorMetricSelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
		If e.AddedItems.Count > 0 Then
			Dim item As ComboBoxItem = TryCast(e.AddedItems(0), ComboBoxItem)
			treeMapControl.Interpolators(0).DataRangeBinding = New Binding(TryCast(item.Tag, String))

			RefreshDataSource()
		End If
	End Sub

	''' <summary>
	''' This is a workaround for the ToolTip behavior - when tooltip is a 
	''' nested element, DataContext is not inherited.
	''' It is not a TreeMap specific workaround.
	''' </summary>
	''' <param name="sender">Sending UI element - Border in our case.</param>
	''' <param name="e">Events - irrelevant in our case.</param>
	<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification := "Attached as an event handler in XAML")> _
	Private Sub ItemContainer_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
		Dim border As Border = TryCast(sender, Border)

		' see summary
		If border IsNot Nothing Then
			CType(ToolTipService.GetToolTip(border), FrameworkElement).DataContext = border.DataContext
		End If
	End Sub
End Class
