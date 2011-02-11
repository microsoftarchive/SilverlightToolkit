' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Namespace Data

    Public Class AllPeople
        Implements IEnumerable(Of Person)
        Private Shared _personLookup As Dictionary(Of Integer, Person)
        Private Shared _instance As AllPeople

        Public Shared ReadOnly Property Current As AllPeople
            Get
                If _instance IsNot Nothing Then
                    Return _instance
                Else
                    _instance = New AllPeople
                    Return _instance
                End If
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal index As Integer) As Person
            Get
                Dim person As Person = Nothing
                _personLookup.TryGetValue(index, person)
                Return person
            End Get
        End Property

#Region "IEnumerable<Person> Members"

        Public Function GetEnumerator() As IEnumerator(Of Person) Implements IEnumerable(Of Person).GetEnumerator
            EnsureData()
            Return _personLookup.Values.GetEnumerator()
        End Function

#End Region

#Region "IEnumerable Members"

        Private Function IEnumerable_GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            EnsureData()
            Return _personLookup.Values.GetEnumerator()
        End Function

#End Region

        Private Sub EnsureData()
            If _personLookup Is Nothing Then
                _personLookup = New Dictionary(Of Integer, Person)
                For n = 0 To 99
                    Dim person As Person = person.GetRandomPerson(n)
                    _personLookup(n) = person
                Next n
            End If
        End Sub

    End Class

End Namespace
