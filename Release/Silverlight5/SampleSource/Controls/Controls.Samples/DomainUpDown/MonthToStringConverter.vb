' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Windows.Data
Imports System.Globalization

''' <summary>
''' Converts the custom Month class to a string.
''' </summary>
Public Class MonthToStringConverter
    Implements IValueConverter
    ''' <summary>
    ''' Modifies the source data before passing it to the target for display in the UI.
    ''' </summary>
    ''' <param name="value">The source data being passed to the target.</param>
    ''' <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
    ''' <param name="parameter">An optional parameter to be used in the converter logic.</param>
    ''' <param name="culture">The culture of the conversion.</param>
    ''' <returns>
    ''' The value to be passed to the target dependency property.
    ''' </returns>
    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim month As Month = TryCast(value, Month)
        If month IsNot Nothing Then
            If culture Is Nothing Then
                culture = CultureInfo.CurrentCulture
            End If
            Return New DateTime(1900, month.Number, 1).ToString("MMMM", culture.DateTimeFormat)
        End If

        Throw New ArgumentException("Expected a Month object.", "value")
    End Function

    ''' <summary>
    ''' Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
    ''' </summary>
    ''' <param name="value">The target data being passed to the source.</param>
    ''' <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
    ''' <param name="parameter">An optional parameter to be used in the converter logic.</param>
    ''' <param name="culture">The culture of the conversion.</param>
    ''' <returns>
    ''' The value to be passed to the source object.
    ''' </returns>
    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class
