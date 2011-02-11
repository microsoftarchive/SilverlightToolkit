' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports PhoneToolkitSample.Data

Namespace Samples

    Partial Public Class LongListSelectorSample
        Inherits PhoneApplicationPage
        Public Sub New()
            InitializeComponent()
            LoadLinqMovies()
            AddHandler linqMovies.SelectionChanged, AddressOf MovieSelectionChanged
            AddHandler codeMovies.SelectionChanged, AddressOf MovieSelectionChanged
        End Sub


        Private Sub LoadLinqMovies()
            Dim movies As New List(Of Movie)

            For i = 0 To 49
                movies.Add(Movie.CreateRandom())
            Next i

            Dim g = movies.GroupBy(Function(p) p.Category).OrderBy(Function(p) p.Key)
            Dim moviesByCategory = From c In g
                  Select New PublicGrouping(Of String, Movie)(c)

            linqMovies.ItemsSource = moviesByCategory
        End Sub

        Private Sub MovieSelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)

        End Sub

        Private Sub PersonSelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs) Handles buddies.SelectionChanged
            Dim _person = TryCast(buddies.SelectedItem, Person)
            If _person IsNot Nothing Then
                NavigationService.Navigate(New Uri("/Samples/PersonDetail.xaml?ID=" & _person.ID, UriKind.Relative))
            End If
        End Sub
    End Class

End Namespace
