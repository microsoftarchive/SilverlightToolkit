' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.Globalization

''' <summary>
''' Overridden TimeGlobalizationInfo for shows mapping of characters to 
''' Arabic.
''' </summary>
Public Class ArabicTimeGlobalizationInfo
    Inherits TimeGlobalizationInfo
    ''' <summary>
    ''' Dictionary helpful for translation.
    ''' </summary>
    Private ReadOnly arabicNumbers As New Dictionary(Of Integer, Char)

    ''' <summary>
    ''' Initializes a new instance of the ArabicTimeGlobalizationInfo class.
    ''' </summary>
    Public Sub New()
        arabicNumbers.Add(0, "٠"c)
        arabicNumbers.Add(1, "١"c)
        arabicNumbers.Add(2, "٢"c)
        arabicNumbers.Add(3, "٣"c)
        arabicNumbers.Add(4, "٤"c)
        arabicNumbers.Add(5, "٥"c)
        arabicNumbers.Add(6, "٦"c)
        arabicNumbers.Add(7, "٧"c)
        arabicNumbers.Add(8, "٨"c)
        arabicNumbers.Add(9, "٩"c)
    End Sub

    ''' <summary>
    ''' Returns the global representation of a character.
    ''' </summary>
    ''' <param name="input">Character that will be mapped to a different
    ''' character.</param>
    ''' <returns>
    ''' The global version of a character that represents the input.
    ''' </returns>
    Protected Overrides Function MapDigitToCharacter(ByVal input As Integer) As Char
        If arabicNumbers.ContainsKey(input) Then
            Return arabicNumbers(input)
        Else
            Return MyBase.MapDigitToCharacter(input)
        End If
    End Function

    ''' <summary>
    ''' Returns the char that is represented by a global character.
    ''' </summary>
    ''' <param name="input">The global version of the character that needs
    ''' to be mapped to a regular character.</param>
    ''' <returns>
    ''' The character that represents the global version of a character.
    ''' </returns>
    Protected Overrides Function MapCharacterToDigit(ByVal input As Char) As Char
        If arabicNumbers.ContainsValue(input) Then
            For Each pair In arabicNumbers
                If pair.Value = input Then
                    Return pair.Key.ToString(CultureInfo.InvariantCulture)(0)
                End If
            Next pair
        End If

        Return MyBase.MapCharacterToDigit(input)
    End Function
End Class