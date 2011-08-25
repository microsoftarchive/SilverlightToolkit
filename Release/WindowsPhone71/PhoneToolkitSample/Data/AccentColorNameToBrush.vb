' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Windows.Data

Namespace Data

    Public Class AccentColorNameToBrush
        Implements IValueConverter

        Private Shared ColorNameToBrush As Dictionary(Of String, SolidColorBrush)

        Shared Sub New()
            ColorNameToBrush.Add("magenta", &HFFFF0097UI.ToSolidColorBrush())
            ColorNameToBrush.Add("purple", &HFFA200FFUI.ToSolidColorBrush())
            ColorNameToBrush.Add("teal", &HFF00ABA9UI.ToSolidColorBrush())
            ColorNameToBrush.Add("lime", &HFF8CBF26UI.ToSolidColorBrush())
            ColorNameToBrush.Add("brown", &HFFA05000UI.ToSolidColorBrush())
            ColorNameToBrush.Add("pink", &HFFE671B8UI.ToSolidColorBrush())
            ColorNameToBrush.Add("orange", &HFFF09609UI.ToSolidColorBrush())
            ColorNameToBrush.Add("blue", &HFF1BA1E2UI.ToSolidColorBrush())
            ColorNameToBrush.Add("red", &HFFE51400UI.ToSolidColorBrush())
            ColorNameToBrush.Add("green", &HFF339933UI.ToSolidColorBrush())
            ColorNameToBrush.Add("mango", &HFFF09609UI.ToSolidColorBrush())
        End Sub

        Public Function Convert(ByVal value As Object, ByVal targetType As Type,
                                ByVal parameter As Object,
                                ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
            Dim v As String = value

            If v Is Nothing Then
                Throw New Exception("value")
            End If

            Dim brush As SolidColorBrush = Nothing
            If ColorNameToBrush.TryGetValue(v.ToLowerInvariant(), brush) Then
                Return brush
            End If

            Return Nothing
        End Function

        Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type,
                                    ByVal parameter As Object,
                                    ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException
        End Function
    End Class

End Namespace