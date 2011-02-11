' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Namespace Data

    ''' <summary>
    ''' A class to return a list of web page titles and addresses from lorem ipsum data.
    ''' </summary>
    Public Class LoremIpsumWebsites
        Implements System.Collections.Generic.IEnumerable(Of Tuple(Of String, String))

        ''' <summary>
        ''' Enumerates the Words property.
        ''' </summary>
        ''' <returns>The enumerator.</returns>
        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of Tuple(Of String, String)) Implements System.Collections.Generic.IEnumerable(Of Tuple(Of String, String)).GetEnumerator
            Dim results As New List(Of Tuple(Of String, String))

            For Each s In LoremIpsum.Words
                Dim t = String.Empty
                If s.Length > 0 Then
                    t = Char.ToUpper(s.Chars(0)) & s.Substring(1)
                End If
                results.Add(New Tuple(Of String, String) With {.Item1 = t, .Item2 = "http://www." & s & "+net.com/"})
            Next s

            Return results.GetEnumerator()
        End Function

        ''' <summary>
        ''' Enumerates the Words property.
        ''' </summary>
        ''' <returns>The enumerator.</returns>
        Private Function IEnumerable_GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

    End Class

End Namespace
