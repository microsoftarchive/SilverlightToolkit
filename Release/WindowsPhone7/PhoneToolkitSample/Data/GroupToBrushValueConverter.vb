' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Windows.Data

Namespace Data

    Public Class GroupToBrushValueConverter
        Implements IValueConverter
        Public Function Convert(ByVal value As Object, ByVal targetType As Type,
                                ByVal parameter As Object,
                                ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
            Dim group = TryCast(value, PeopleInGroup)
            Dim result = Nothing

            If group IsNot Nothing Then
                If group.Count = 0 Then
                    result = CType(Application.Current.Resources("PhoneChromeBrush"), SolidColorBrush)
                Else
                    result = CType(Application.Current.Resources("PhoneAccentBrush"), SolidColorBrush)
                End If
            End If

            Return result
        End Function

        Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type,
                                    ByVal parameter As Object,
                                    ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
            Throw New NotImplementedException
        End Function
    End Class

End Namespace
