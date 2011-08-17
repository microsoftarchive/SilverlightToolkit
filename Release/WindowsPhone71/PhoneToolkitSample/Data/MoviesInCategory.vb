' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports System.Collections.ObjectModel

Namespace Data

    Public Class MoviesInCategory
        Inherits ObservableCollection(Of Movie)
        Public Sub New(ByVal category As String)
            Key = category
        End Sub

        Public Property Key As String

        Public ReadOnly Property GetMore As String
            Get
                Return "More " & Key
            End Get
        End Property
    End Class

End Namespace
