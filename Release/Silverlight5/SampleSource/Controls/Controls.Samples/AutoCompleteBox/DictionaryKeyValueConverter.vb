' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Reflection
Imports System.IO
Imports System.Linq
Imports System.Windows
Imports System.Windows.Data
Imports System.Windows.Media.Imaging
Imports System.Windows.Controls

''' <summary>
''' A simple value converter.
''' </summary>
''' <typeparam name="TKey">The key type.</typeparam>
''' <typeparam name="TValue">The value type.</typeparam>
Public Class DictionaryKeyValueConverter(Of TKey, TValue)
    Implements IValueConverter
    ''' <summary>
    ''' Converts the value back.
    ''' </summary>
    ''' <param name="value">The object reference.</param>
    ''' <param name="targetType">The type object.</param>
    ''' <param name="parameter">The optional parameter.</param>
    ''' <param name="culture">The optional culture.</param>
    ''' <returns>Returns an object or null.</returns>
    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Return Nothing
    End Function

    ''' <summary>
    ''' Convert the object to a string.
    ''' </summary>
    ''' <param name="value">The object reference.</param>
    ''' <param name="targetType">The type object.</param>
    ''' <param name="parameter">The optional parameter.</param>
    ''' <param name="culture">The optional culture.</param>
    ''' <returns>Returns an object or null.</returns>
    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
        If TypeOf value Is KeyValuePair(Of TKey, TValue) Then
            Dim pair As KeyValuePair(Of TKey, TValue) = CType(value, KeyValuePair(Of TKey, TValue))
            Return pair.Key.ToString()
        End If

        Return Nothing
    End Function
End Class
