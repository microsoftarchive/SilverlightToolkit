' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.Windows.Media

''' <summary>
''' Helper class for general sample-related utility methods.
''' </summary>
Friend NotInheritable Class SampleHelpers
	''' <summary>
	''' Changes the alignment of a sample to Stretch so that its contents
	''' will automatically fill the available area.
	''' </summary>
	''' <param name="sample">Sample control to modify.</param>
	Private Sub New()
	End Sub
	Public Shared Sub ChangeSampleAlignmentToStretch(ByVal sample As UserControl)
		' Set sample to stretch
		sample.HorizontalAlignment = HorizontalAlignment.Stretch
		sample.VerticalAlignment = VerticalAlignment.Stretch

		' Find ContentPresenter parent (i.e., TabItem in Silverlight world)...
		Dim parent As FrameworkElement = sample
		Do While parent IsNot Nothing
			Dim tabItem As ContentPresenter = TryCast(parent, ContentPresenter)
			If tabItem IsNot Nothing Then
				' Set to stretch as well
				tabItem.HorizontalAlignment = HorizontalAlignment.Stretch
				tabItem.VerticalAlignment = VerticalAlignment.Stretch
				Exit Do
			End If

			' Move up a level
			parent = TryCast(VisualTreeHelper.GetParent(parent), FrameworkElement)
		Loop
	End Sub
End Class