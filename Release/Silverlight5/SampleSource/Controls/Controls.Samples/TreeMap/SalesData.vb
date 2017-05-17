' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

''' <summary>
''' Represents product sales data.
''' </summary>

Imports Microsoft.VisualBasic
Imports System
Public Class SalesData
	''' <summary>
	''' Gets or sets the name of the product.
	''' </summary>
	Private privateProduct As String
	Public Property Product() As String
		Get
			Return privateProduct
		End Get
		Set(ByVal value As String)
			privateProduct = value
		End Set
	End Property

	''' <summary>
	''' Gets or sets the quantity of the product sold.
	''' </summary>
	Private privateQuantity As Integer
	Public Property Quantity() As Integer
		Get
			Return privateQuantity
		End Get
		Set(ByVal value As Integer)
			privateQuantity = value
		End Set
	End Property

	''' <summary>
	''' Gets the desired tooltip content.
	''' </summary>
	Public ReadOnly Property ToolTip() As String
		Get
			Return Product & ": " & Quantity
		End Get
	End Property
End Class
