' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Windows.Data
Imports System.Runtime.CompilerServices

Namespace Data

    Public Module ColorExtensions
        Public _accentColors(0 To 9) As String

        Sub New()
            _accentColors(0) = "magenta"
            _accentColors(1) = "purple"
            _accentColors(2) = "teal"
            _accentColors(3) = "lime"
            _accentColors(4) = "brown"
            _accentColors(5) = "pink"
            _accentColors(6) = "mango"
            _accentColors(7) = "blue"
            _accentColors(8) = "red"
            _accentColors(9) = "green"
        End Sub

        <Extension()>
        Public Function ToColor(argb As UInteger) As Color
            Return Color.FromArgb(CType((argb & &HFF000000) >> &H18, Byte), CType((argb & &HFF0000) >> &H10, Byte), CType((argb & &HFF00) >> 8, Byte), CType(argb & &HFF, Byte))
        End Function

        <Extension()>
        Public Function ToSolidColorBrush(argb As UInteger) As SolidColorBrush
            Return New SolidColorBrush(argb.ToColor())
        End Function
    End Module

End Namespace