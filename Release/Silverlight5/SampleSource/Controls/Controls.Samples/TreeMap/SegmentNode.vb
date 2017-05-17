' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.


Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.Diagnostics.CodeAnalysis
Imports System.Text

''' <summary>
''' Represents a node within a tree with value, name and belonging to a particular data segment.
''' </summary>
Public Class SegmentNode
	''' <summary>
	''' Gets or sets a value representing the segment to which a node belongs.
	''' </summary>
	Private privateSegment As Integer
	Public Property Segment() As Integer
		Get
			Return privateSegment
		End Get
		Set(ByVal value As Integer)
			privateSegment = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the primary value associated with the node.
	''' </summary>
	Private privateValue As Double
	Public Property Value() As Double
		Get
			Return privateValue
		End Get
		Set(ByVal value As Double)
			privateValue = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the second value associated with the node.
	''' </summary>
	Private privateValue2 As Double
	Public Property Value2() As Double
		Get
			Return privateValue2
		End Get
		Set(ByVal value As Double)
			privateValue2 = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the name associated with the node.
	''' </summary>
	Private privateName As String
	Public Property Name() As String
		Get
			Return privateName
		End Get
		Set(ByVal value As String)
			privateName = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets a value representing the children of this division, conference, or league. Empty for teams.
	''' </summary>
	Private privateChildren As IList(Of SegmentNode)
	<SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification := "Simplifies samples.")> _
	Public Property Children() As IList(Of SegmentNode)
		Get
			Return privateChildren
		End Get
		Set(ByVal value As IList(Of SegmentNode))
			privateChildren = value
		End Set
	End Property

	''' <summary>
	''' Gets the desired tooltip content.
	''' </summary>
	Public ReadOnly Property ToolTip() As String
		Get
			Dim outStr As New StringBuilder()
			outStr.Append("Name: ").Append(Name)
			outStr.Append(Constants.vbLf & "Value: ").Append(Value)
			outStr.Append(Constants.vbLf & "Value2: ").Append(Value2)
			outStr.Append(Constants.vbLf & "Segment: ").Append(Segment)
			Return outStr.ToString()
		End Get
	End Property
End Class
