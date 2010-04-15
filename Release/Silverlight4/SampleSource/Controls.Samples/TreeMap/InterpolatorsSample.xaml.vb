' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.


Imports Microsoft.VisualBasic
Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis

<Assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope := "member", Target := "System.Windows.Controls.Samples.InterpolatorsSample.#itemBorder", Justification := "Artifact of using a name inside the custom control template.")>

''' <summary>
''' Example showing how to create and use Interpolators with for TreeMap. An interpolator 
''' calculates the minimum and maximum values for the bound property across the whole tree, and
''' then projects the value in the specified [From, To] range.
''' </summary>
<Sample("(2)Interpolators", DifficultyLevel.Basic), Category("TreeMap")> _
Partial Public Class InterpolatorsSample
	Inherits UserControl
	''' <summary>
	''' Initializes a new instance of the InterpolatorsSample class.
	''' </summary>
	Public Sub New()
		InitializeComponent()
		AddHandler Loaded, AddressOf InterpolatorsSample_Loaded
	End Sub

	''' <summary>
	''' Loads the XML sample data and populates the TreeMap.
	''' </summary>
	''' <param name="sender">The object where the event handler is attached.</param>
	''' <param name="e">The event data.</param>
	Private Sub InterpolatorsSample_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
		' Sample browser-specific layout change
		SampleHelpers.ChangeSampleAlignmentToStretch(Me)

        treeMapControl.ItemsSource = NhlDataHelper.LoadDefaultFile()
	End Sub
End Class
