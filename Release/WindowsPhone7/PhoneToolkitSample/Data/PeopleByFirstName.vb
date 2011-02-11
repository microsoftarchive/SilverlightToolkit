' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Namespace Data

    Public Class PeopleByFirstName
        Inherits List(Of PeopleInGroup)
        Private Shared ReadOnly Groups As String = "#abcdefghijklmnopqrstuvwxyz"

        Private _personLookup As New Dictionary(Of Integer, Person)

        Public Sub New()
            Dim people As New List(Of Person)(AllPeople.Current)
            people.Sort(AddressOf Person.CompareByFirstName)

            Dim groups As New Dictionary(Of String, PeopleInGroup)

            For Each c In PeopleByFirstName.Groups
                Dim group As New PeopleInGroup(c.ToString())
                Me.Add(group)
                groups(c.ToString()) = group
            Next c

            For Each _person In people
                groups(Person.GetFirstNameKey(_person)).Add(_person)
            Next _person
        End Sub
    End Class

End Namespace
