' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Collections.ObjectModel

Namespace Data

    Public Class MoviesByCategory
        Inherits ObservableCollection(Of MoviesInCategory)
        Public Sub New()
            Dim sortedCategories As New List(Of String)(Movie.Categories)
            sortedCategories.Sort()

            For Each category In sortedCategories
                Dim group As New MoviesInCategory(category)
                Me.Add(group)

                For i = 0 To 4
                    group.Add(Movie.CreateRandom(category))
                Next i
            Next category
        End Sub
    End Class


    Public Class MoreCommand
        Implements ICommand

#Region "ICommand Members"

        Public Function CanExecute(ByVal parameter As Object) As Boolean Implements System.Windows.Input.ICommand.CanExecute
            Return True
        End Function

        Public Event CanExecuteChanged As EventHandler Implements System.Windows.Input.ICommand.CanExecuteChanged

        Public Sub Execute(ByVal parameter As Object) Implements System.Windows.Input.ICommand.Execute
            Dim group = TryCast(parameter, MoviesInCategory)
            If group IsNot Nothing Then
                group.Add(Movie.CreateRandom(group.Key))
            End If
        End Sub

#End Region

    End Class

End Namespace
