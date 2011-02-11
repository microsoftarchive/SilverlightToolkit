' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Text
Imports System.Windows.Resources
Imports System.IO

Namespace Data

    ''' <summary>
    ''' A class to return paragraphs of random sentences or a word list from lorem ipsum data.
    ''' </summary>
    Public Class LoremIpsum
        Implements System.Collections.Generic.IEnumerable(Of String)
        Public Enum Capitalization
            None
            FirstWord
            AllWords
        End Enum
        Private Shared _rnd As New Random(42)

        Private Shared _builder As New StringBuilder
        Private Shared _sentences As List(Of String)
        Private Shared _words As List(Of String)

        ''' <summary>
        ''' Returns random lorem ipsum sentences combined into a single string.
        ''' </summary>
        ''' <param name="sentenceCount">The nunmber of sentences.</param>
        ''' <returns>The paragraph, composed of random sentences.</returns>
        Public Shared Function GetParagraph(ByVal sentenceCount As Integer) As String
            EnsureSentences()

            _builder.Length = 0

            Dim tempVar = sentenceCount > 0
            sentenceCount -= 1
            Do While tempVar
                _builder.Append(_rnd.Next(_sentences))
                If sentenceCount > 0 Then
                    _builder.Append(" "c)
                End If
                tempVar = sentenceCount > 0
                sentenceCount -= 1
            Loop

            Return _builder.ToString()
        End Function

        ''' <summary>
        ''' Return an alphabetized, lower-case list of lorem ipsum words.
        ''' </summary>
        Public Shared ReadOnly Property Words As ICollection(Of String)
            Get
                EnsureWords()
                Return CType(_words, ICollection(Of String))
            End Get
        End Property

        ''' <summary>
        ''' Get a string composed of random lorem ipsum words. Will not end with punctuation.
        ''' </summary>
        ''' <param name="wordCount">Number of words.</param>
        ''' <param name="capitalization">How to capitalize the words.</param>
        ''' <returns></returns>
        Public Shared Function GetWords(ByVal wordCount As Integer, ByVal capitalization As Capitalization) As String
            EnsureWords()

            _builder.Length = 0

            Dim tempVar = wordCount > 0
            wordCount -= 1
            Do While tempVar
                Dim position = _builder.Length
                _builder.Append(_rnd.Next(_words))
                If capitalization = capitalization.AllWords OrElse (position = 0 AndAlso capitalization = capitalization.FirstWord) Then
                    _builder(position) = Char.ToUpper(_builder(position))
                End If

                If wordCount > 0 Then
                    _builder.Append(" "c)
                End If
                tempVar = wordCount > 0
                wordCount -= 1
            Loop

            Return _builder.ToString()
        End Function

        ''' <summary>
        ''' Enumerates the Words property.
        ''' </summary>
        ''' <returns>The enumerator.</returns>
        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of String) Implements System.Collections.Generic.IEnumerable(Of String).GetEnumerator
            Return LoremIpsum.Words.GetEnumerator()
        End Function

        ''' <summary>
        ''' Enumerates the Words property.
        ''' </summary>
        ''' <returns>The enumerator.</returns>
        Private Function IEnumerable_GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return LoremIpsum.Words.GetEnumerator()
        End Function

        ''' <summary>
        ''' Reads the lorem ipsum sentences. Supplies some data in case reading fails, which
        ''' it will do at design time.
        ''' </summary>
        Private Shared Sub EnsureSentences()
            If _sentences Is Nothing Then
                _sentences = New List(Of String)

                Dim info = Application.GetResourceStream(New Uri("Data/LoremIpsum.txt", UriKind.Relative))
                If info IsNot Nothing Then
                    Dim stream = info.Stream
                    If stream IsNot Nothing Then
                        Using reader As New StreamReader(stream)
                            Do While Not reader.EndOfStream
                                _sentences.Add(reader.ReadLine())
                            Loop
                        End Using
                    End If
                End If

                If _sentences.Count = 0 Then
                    _sentences.Add("Lorem ipsum dolor sit amet, consectetuer adipiscing elit.")
                    _sentences.Add("Maecenas porttitor congue massa.")
                    _sentences.Add("Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.")
                    _sentences.Add("Nunc viverra imperdiet enim.")
                    _sentences.Add("Fusce est.")
                End If
            End If
        End Sub

        ''' <summary>
        ''' Creates an alphabetized list of the words from the lorem ipsum text.
        ''' </summary>
        Private Shared Sub EnsureWords()
            Dim separators() = {" "c, ","c, "."c}

            EnsureSentences()

            If _words Is Nothing Then
                Dim temp As New Dictionary(Of String, Object)

                For Each sentence In _sentences
                    Dim words() = sentence.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    For Each word In words
                        temp(word.ToLower()) = Nothing
                    Next word
                Next sentence

                _words = New List(Of String)(temp.Keys)
                _words.Sort()
            End If
        End Sub


    End Class

End Namespace
