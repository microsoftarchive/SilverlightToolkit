' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.Globalization
Imports System.Windows.Data

''' <summary>
''' Converts a number into currency format.
''' </summary>
Public Class CurrencyConverter
	Implements IValueConverter
	''' <summary>
	''' Converts from a number to a currency format.
	''' </summary>
	''' <param name="value">The value of amount.</param>
	''' <param name="targetType">The type to convert the value to.</param>
	''' <param name="parameter">Information about the conversion.</param>
	''' <param name="culture">The culture to use for the conversion.</param>
	''' <returns>The converted value.</returns>
	Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
		Return String.Format(CultureInfo.CurrentUICulture, "{0:c}", value)
	End Function

	''' <summary>
	''' Converts to a number from a currency format.
	''' </summary>
	''' <param name="value">The string value to convert.</param>
	''' <param name="targetType">The type to convert the value to.</param>
	''' <param name="parameter">Information about the conversion.</param>
	''' <param name="culture">The culture to use for the conversion.</param>
	''' <returns>The converted value.</returns>
	Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function
End Class