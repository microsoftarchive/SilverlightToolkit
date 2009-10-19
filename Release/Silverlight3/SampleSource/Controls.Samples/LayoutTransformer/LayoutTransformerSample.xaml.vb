' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Globalization
Imports System.IO
Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Windows.Media.Imaging

''' <summary>
''' Sample page demonstrating LayoutTransformer.
''' </summary>
<Sample("Interactive Sample", DifficultyLevel.Basic), Category("LayoutTransformer")> _
Partial Public Class LayoutTransformerSample
	Inherits UserControl
	''' <summary>
	''' Specifies the size of large fonts.
	''' </summary>
	Private Const SampleFontSizeLarge As Integer = 64

	''' <summary>
	''' Specifies the size of small fonts.
	''' </summary>
	Private Const SampleFontSizeSmall As Integer = 28

	''' <summary>
	''' Indicates whether component initialization has finished.
	''' </summary>
	Private _initializeComponentFinished As Boolean

	''' <summary>
	''' Initializes a new instance of the LayoutTransformerSample class.
	''' </summary>
	Public Sub New()
		InitializeComponent()
		_initializeComponentFinished = True
		Slider_ValueChanged(Nothing, Nothing)
		Text_Click(Nothing, Nothing)
        AddHandler Loaded, AddressOf LayoutTransformerSample_Loaded
	End Sub

    Private Sub LayoutTransformerSample_Loaded(ByVal sender As Object, ByVal args As RoutedEventArgs)
        SampleHelpers.ChangeSampleAlignmentToStretch(Me)
    End Sub

	''' <summary>
	''' Handles the ValueChanged event for any Slider.
	''' </summary>
	''' <param name="sender">Source of event.</param>
	''' <param name="e">Event arguments.</param>
	Private Sub Slider_ValueChanged(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of Double))
		If _initializeComponentFinished Then
			scaleTransform.ScaleX = scaleXSlider.Value
			scaleXDisplay.Text = Math.Round(scaleXSlider.Value, 2).ToString(CultureInfo.CurrentCulture)
			scaleTransform.ScaleY = scaleYSlider.Value
			scaleYDisplay.Text = Math.Round(scaleYSlider.Value, 2).ToString(CultureInfo.CurrentCulture)
			skewTransform.AngleX = skewXSlider.Value
			skewXDisplay.Text = Math.Round(skewXSlider.Value, 2).ToString(CultureInfo.CurrentCulture)
			skewTransform.AngleY = skewYSlider.Value
			skewYDisplay.Text = Math.Round(skewYSlider.Value, 2).ToString(CultureInfo.CurrentCulture)
			rotateTransform.Angle = rotateSlider.Value
			rotateDisplay.Text = Math.Round(rotateSlider.Value, 2).ToString(CultureInfo.CurrentCulture)
			layoutTransform.ApplyLayoutTransform()
		End If
	End Sub

	''' <summary>
	''' Handles the Click event for the Empty Button.
	''' </summary>
	''' <param name="sender">Source of event.</param>
	''' <param name="e">Event arguments.</param>
	<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification := "Referenced in XAML.")> _
	Private Sub Empty_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
		layoutTransform.Content = Nothing
	End Sub

	''' <summary>
	''' Handles the Click event for the Text Button.
	''' </summary>
	''' <param name="sender">Source of event.</param>
	''' <param name="e">Event arguments.</param>
	<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification := "Referenced in XAML.")> _
	Private Sub Text_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
		layoutTransform.Content = New TextBlock With {.Text = "TextBlock", .FontSize = SampleFontSizeLarge}
	End Sub

	''' <summary>
	''' Handles the Click event for the Button Button.
	''' </summary>
	''' <param name="sender">Source of event.</param>
	''' <param name="e">Event arguments.</param>
	<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification := "Referenced in XAML.")> _
	Private Sub Button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
		layoutTransform.Content = New Button With {.Content = "Button", .FontSize = SampleFontSizeLarge}
	End Sub

	''' <summary>
	''' Handles the Click event for the Image Button.
	''' </summary>
	''' <param name="sender">Source of event.</param>
	''' <param name="e">Event arguments.</param>
	<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification := "Referenced in XAML.")> _
	Private Sub Image_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
		Dim bitmapImage As New BitmapImage()
		Using resourceStream As Stream = GetType(SharedResources).Assembly.GetManifestResourceStream("System.Windows.Controls.Samples.Images.Dock.jpg")
			If resourceStream IsNot Nothing Then
				bitmapImage.SetSource(resourceStream)
			End If
		End Using
        layoutTransform.Content = New Image With {.Source = bitmapImage}
	End Sub

	''' <summary>
	''' Handles the Click event for the Empty Button.
	''' </summary>
	''' <param name="sender">Source of event.</param>
	''' <param name="e">Event arguments.</param>
	<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification := "Referenced in XAML.")> _
	Private Sub ListBox_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
		Dim listBox = New ListBox()
		For Each item In "This-is a-fully-functional-ListBox!".Split("-"c)
			listBox.Items.Add(New ListBoxItem With {.Content = item, .FontSize = SampleFontSizeSmall})
		Next item
		layoutTransform.Content = listBox
	End Sub
End Class