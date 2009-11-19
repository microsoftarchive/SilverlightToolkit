' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.


Imports Microsoft.VisualBasic
Imports System.ComponentModel

''' <summary>
''' Sample showing how an ItemDefinitionSelector can be used to return a different
''' template for each level in the tree.
''' </summary>
<Sample("(1)ItemDefinitionSelector", DifficultyLevel.Intermediate), Category("TreeMap")> _
Partial Public Class FlexibleTemplateSample
	Inherits UserControl
	''' <summary>
	''' Initializes a new instance of the FlexibleTemplateSample class.
	''' </summary>
	Public Sub New()
		InitializeComponent()
		AddHandler Loaded, AddressOf FlexibleTemplateSample_Loaded
	End Sub

	''' <summary>
	''' Loads the XML sample data and populates the TreeMap.
	''' </summary>
	''' <param name="sender">The object where the event handler is attached.</param>
	''' <param name="e">The event data.</param>
	Private Sub FlexibleTemplateSample_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
		' Sample browser-specific layout change
		SampleHelpers.ChangeSampleAlignmentToStretch(Me)

        treeMapControl.ItemsSource = NhlDataHelper.LoadDefaultFile()
	End Sub
End Class
