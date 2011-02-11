' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Namespace Data

    Public Class Person
        Private _id As Integer
        Public Property ID As Integer
            Get
                Return _id
            End Get
            Private Set(ByVal value As Integer)
                _id = value
            End Set
        End Property

        Private _firstName As String
        Public Property FirstName As String
            Get
                Return _firstName
            End Get
            Private Set(ByVal value As String)
                _firstName = value
            End Set
        End Property

        Private _lastName As String
        Public Property LastName As String
            Get
                Return _lastName
            End Get
            Private Set(ByVal value As String)
                _lastName = value
            End Set
        End Property

        Public ReadOnly Property FullName As String
            Get
                Return FirstName & " " & LastName
            End Get
        End Property

        Private _email As String
        Public Property Email As String
            Get
                Return _email
            End Get
            Private Set(ByVal value As String)
                _email = value
            End Set
        End Property

        Private _mobile As String
        Public Property Mobile As String
            Get
                Return _mobile
            End Get
            Private Set(ByVal value As String)
                _mobile = value
            End Set
        End Property

        Private _home As String
        Public Property Home As String
            Get
                Return _home
            End Get
            Private Set(ByVal value As String)
                _home = value
            End Set
        End Property

        Private _imageUrl As String
        Public Property ImageUrl As String
            Get
                Return _imageUrl
            End Get
            Private Set(ByVal value As String)
                _imageUrl = value
            End Set
        End Property

        Public Shared Function GetRandomPerson(ByVal id As Integer) As Person
            Dim myPerson As New Person

            myPerson.ID = id
            myPerson.FirstName = RandomPeople.GetRandomFirstName()
            myPerson.LastName = RandomPeople.GetRandomLastName()
            myPerson.Email = myPerson.FirstName & "@email+more.com"
            myPerson.Mobile = RandomPeople.GetRandomPhoneNumber()
            myPerson.Home = RandomPeople.GetRandomPhoneNumber()
            myPerson.ImageUrl = "/Images/Person.jpg"
            Return myPerson
        End Function

        Public Shared Function GetFirstNameKey(ByVal myPerson As Person) As String
            Dim key = Char.ToLower(myPerson.FirstName.Chars(0))

            If key < "a"c OrElse key > "z"c Then
                key = "#"c
            End If

            Return key.ToString()
        End Function

        Public Shared Function CompareByFirstName(ByVal obj1 As Object, ByVal obj2 As Object) As Integer
            Dim p1 = CType(obj1, Person)
            Dim p2 = CType(obj2, Person)

            Dim result = p1.FirstName.CompareTo(p2.FirstName)
            If result = 0 Then
                result = p1.LastName.CompareTo(p2.LastName)
            End If

            Return result
        End Function
    End Class

End Namespace
