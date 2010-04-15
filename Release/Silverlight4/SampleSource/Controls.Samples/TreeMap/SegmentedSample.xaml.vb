' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.


Imports Microsoft.VisualBasic
Imports System.ComponentModel
Imports System.Collections.Generic

''' <summary>
''' Sample showing how to use different item definitions per branch of a tree.
''' </summary>
<Sample("(6) Segmented TreeMap", DifficultyLevel.Advanced), Category("TreeMap")> _
Partial Public Class SegmentedSample
	Inherits UserControl
	''' <summary>
	''' Initializes a new instance of the SegmentedSample class.
	''' </summary>
	Public Sub New()
		InitializeComponent()
		AddHandler Loaded, AddressOf SegmentedSample_Loaded
	End Sub

	''' <summary>
	''' Generate a tree with a given segment ID.
	''' </summary>
	''' <param name="depth">The depth of the tree.</param>
	''' <param name="maxChildren">The maximum number of children nodes allowed.</param>
	''' <param name="maxValue"> The maximum value allowed for the node's metrics.</param>
	''' <param name="segmentID">The ID to segment to which the leaves belong.</param>
	''' <param name="name">The name of the node (for making node names).</param>
	''' <param name="random">A random number generator for controlling tree generation.</param>
	''' <returns>A SegmentNode representing the root of the tree.</returns>
	Private Function GenerateTree(ByVal depth As Integer, ByVal maxChildren As Integer, ByVal maxValue As Integer, ByVal segmentID As Integer, ByVal name As String, ByVal random As Random) As SegmentNode
		Dim node As New SegmentNode()
		node.Name = name

		If depth <= 0 Then
			node.Value = random.Next(1, maxValue)
			node.Value2 = random.Next(1, maxValue)
			node.Segment = segmentID
			node.Children = New SegmentNode(){}
		Else
			Dim numChildren As Integer = random.Next(2, maxChildren)

			node.Children = New List(Of SegmentNode)()
			For i As Integer = 0 To numChildren - 1
				node.Children.Add(GenerateTree(depth - 1, maxChildren, maxChildren, segmentID, name & "." & i, random))
			Next i
		End If

		Return node
	End Function

	''' <summary>
	''' Loads the XML sample data and populates the TreeMap.
	''' </summary>
	''' <param name="sender">The object where the event handler is attached.</param>
	''' <param name="e">The event data.</param>
	Private Sub SegmentedSample_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
		' Sample browser-specific layout change.
		SampleHelpers.ChangeSampleAlignmentToStretch(Me)

		' Construct the tree.
		Dim r As New Random()
		treeMapControl.ItemsSource = New List(Of SegmentNode) (New SegmentNode() {GenerateTree(2, 9, 10, 1, "A", r), GenerateTree(2, 7, 15, 2, "B", r), GenerateTree(2, 5, 20, 3, "C", r)})
	End Sub
End Class

