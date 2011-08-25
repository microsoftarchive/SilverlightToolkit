' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Windows.Data

Namespace Data

    Public Class GroupToBackgroundBrushValueConverter
        Implements IValueConverter
        Public Function Convert(ByVal value As Object, ByVal targetType As Type,
                                ByVal parameter As Object,
                                ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
            Dim group = TryCast(value, PeopleInGroup)
            Dim result = Nothing

            If group IsNot Nothing Then
                If group.Count = 0 Then
                    result = CType(Application.Current.Resources("PhoneDisabledBrush"), SolidColorBrush)
                Else
                    result = New SolidColorBrush(Colors.White)
                End If
            End If

            Return result
        End Function

        Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type,
                                    ByVal parameter As Object,
                                    ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException
        End Function
    End Class

End Namespace