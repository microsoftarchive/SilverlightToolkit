' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Namespace Data

    Public Class Movie
        Private Shared _rnd As New Random(42)

        Public Shared ReadOnly Categories() As String = {"Action", "Romance", "Thrillers", "Comedy", "Documentaries", "Drama"}
        Private Shared _categoryIndex As Integer

        Private Shared ReadOnly Ratings() As String = {"G", "PG", "PG-13", "R"}

        Private _title As String
        Public Property Title As String
            Get
                Return _title
            End Get
            Private Set(ByVal value As String)
                _title = value
            End Set
        End Property

        Private _description As String
        Public Property Description As String
            Get
                Return _description
            End Get
            Private Set(ByVal value As String)
                _description = value
            End Set
        End Property

        Private _year As Integer
        Public Property Year As Integer
            Get
                Return _year
            End Get
            Private Set(ByVal value As Integer)
                _year = value
            End Set
        End Property

        Private _rating As String
        Public Property Rating As String
            Get
                Return _rating
            End Get
            Private Set(ByVal value As String)
                _rating = value
            End Set
        End Property

        Private _star1 As String
        Public Property Star1 As String
            Get
                Return _star1
            End Get
            Private Set(ByVal value As String)
                _star1 = value
            End Set
        End Property

        Private _star2 As String
        Public Property Star2 As String
            Get
                Return _star2
            End Get
            Private Set(ByVal value As String)
                _star2 = value
            End Set
        End Property

        Private _runTime As TimeSpan
        Public Property RunTime As TimeSpan
            Get
                Return _runTime
            End Get
            Private Set(ByVal value As TimeSpan)
                _runTime = value
            End Set
        End Property

        Public Property ImageUrl As String

        Public ReadOnly Property Stars As String
            Get
                Return Star1 & ", " & Star2
            End Get
        End Property

        Public ReadOnly Property Information As String
            Get
                Return String.Format("{0} {1} {2}:{3:D2}", Year, Rating, RunTime.Hours, RunTime.Minutes)
            End Get
        End Property

        Private _category As String
        Public Property Category As String
            Get
                Return _category
            End Get
            Private Set(ByVal value As String)
                _category = value
            End Set
        End Property

        Public Shared Function CreateRandom() As Movie
            Dim category = Categories(_categoryIndex)
            _categoryIndex = (_categoryIndex + 1) Mod Categories.Length
            Return CreateRandom(category)
        End Function

        Public Shared Function CreateRandom(ByVal category As String) As Movie
            Dim myMovie As New Movie
            myMovie.Title = LoremIpsum.GetWords(_rnd.Next(1, 5), LoremIpsum.Capitalization.AllWords)
            myMovie.Year = _rnd.Next(1965, 2010)
            myMovie.Rating = _rnd.Next(Ratings.Count)
            myMovie.RunTime = TimeSpan.FromMinutes(_rnd.Next(60, 180))
            myMovie.Description = LoremIpsum.GetParagraph(_rnd.Next(3, 7))
            myMovie.Star1 = RandomPeople.GetRandomFullName()
            myMovie.Star2 = RandomPeople.GetRandomFullName()
            myMovie.Category = category
            myMovie.ImageUrl = "/Images/Movie.jpg"

            Return myMovie
        End Function
    End Class

End Namespace
