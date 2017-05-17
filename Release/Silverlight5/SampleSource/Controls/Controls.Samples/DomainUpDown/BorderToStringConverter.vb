' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System
Imports System.Globalization
Imports System.Linq
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Data
Imports System.Reflection

''' <summary>
''' Converts a border to a friendly name (the color of its background).
''' </summary>
''' <remarks>Used in DomainUpDownSample.</remarks>
Public Class BorderToStringConverter
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
        ' expecting a border
        Dim element As Border = TryCast(value, Border)
        If element IsNot Nothing Then
            Dim b As SolidColorBrush = TryCast(element.Background, SolidColorBrush)

            If b IsNot Nothing Then
                ' use the colors class to find a friendly name for this color.
                Dim colorname As String = ( _
                  From c In GetType(Colors).GetProperties(BindingFlags.Public Or BindingFlags.Static) _
                  Where c.GetValue(Nothing, New Object() {}).Equals(b.Color) _
                  Select c.Name).FirstOrDefault()

                ' no friendly name found, use the rgb code.
                If String.IsNullOrEmpty(colorname) Then
                    colorname = b.Color.ToString()
                End If
                Return colorname
            End If
        End If
        Return String.Empty
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
        Throw New System.NotImplementedException()
    End Function
End Class
