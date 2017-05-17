' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.


Imports Microsoft.VisualBasic
Imports System.Collections.ObjectModel
Imports System.Windows.Controls.DataVisualization
Imports System.Windows.Markup

''' <summary>
''' Dynamically chooses a TreeMapItemDefintion based on an item's level in the hierarchy.
''' </summary>
<ContentProperty("Children")> _
Public Class AlternatingItemDefinitionSelector
	Inherits TreeMapItemDefinitionSelector
	''' <summary>
	''' Gets the list of templates that this selector will choose from.
	''' </summary>
	Private privateChildren As Collection(Of TreeMapItemDefinition)
	Public Property Children() As Collection(Of TreeMapItemDefinition)
		Get
			Return privateChildren
		End Get
		Private Set(ByVal value As Collection(Of TreeMapItemDefinition))
			privateChildren = value
		End Set
	End Property

	''' <summary>
	''' Initializes a new instance of the <see cref="AlternatingItemDefinitionSelector"/> class. 
	''' </summary>
	Public Sub New()
		Children = New Collection(Of TreeMapItemDefinition)()
	End Sub

	''' <summary>
	''' Returns an instance of a TreeMapItemDefinition class used to specify properties for the 
	''' current item. 
	''' </summary>
	''' <param name="treeMap">Reference to the TreeMap class.</param>
	''' <param name="item">One of the nodes in the ItemsSource hierarchy.</param>
	''' <param name="level">The level of the node in the hierarchy.</param>
	''' <returns>The TreeMapItemDefinition to be used for this node. If this method returns null 
	''' the TreeMap will use the value of its ItemDefinition property.</returns>
	Public Overrides Function SelectItemDefinition(ByVal treeMap As TreeMap, ByVal item As Object, ByVal level As Integer) As TreeMapItemDefinition
		If Children.Count > 0 Then
			Dim child As Integer = level Mod Children.Count
			Return Children(child)
		Else
			Return Nothing
		End If
	End Function
End Class
