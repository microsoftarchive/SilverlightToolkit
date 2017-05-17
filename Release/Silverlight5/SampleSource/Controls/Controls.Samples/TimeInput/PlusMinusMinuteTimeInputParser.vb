' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports Microsoft.VisualBasic
Imports System.Globalization
Imports System.Text.RegularExpressions

''' <summary>
''' Custom parser that will parse a plus or minus
''' symbol and a number to an offset of minutes.
''' </summary>
''' <example>input: +2m, output DateTime.Now plus 2 minutes.</example>
Public Class PlusMinusMinuteTimeInputParser
    Inherits TimeParser
    ''' <summary>
    ''' Expression used to parse.
    ''' </summary>
    Private Shared ReadOnly exp As New Regex("(?<minutes>[+|-]\d{1,2})[m|M]", RegexOptions.CultureInvariant)

    ''' <summary>
    ''' Tries to parse a string to a DateTime.
    ''' </summary>
    ''' <param name="text">The text that should be parsed.</param>
    ''' <param name="culture">The culture being used.</param>
    ''' <param name="result">The parsed DateTime.</param>
    ''' <returns>
    ''' True if the parse was successful, false if it was not.
    ''' </returns>
    Public Overrides Function TryParse(ByVal text As String, ByVal culture As CultureInfo, ByRef result? As DateTime) As Boolean
        Dim match As Match = exp.Match(text)

        If match.Success Then
            Dim number As Integer = Integer.Parse(match.Groups("minutes").Value, culture)
            result = DateTime.Now.AddMinutes(number)
            Return True
        End If
        result = Nothing
        Return False
    End Function
End Class