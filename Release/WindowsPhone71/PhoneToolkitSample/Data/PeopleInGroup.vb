' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Namespace Data

    Public Class PeopleInGroup
        Inherits List(Of Person)
        Public Sub New(ByVal category As String)
            Key = category
        End Sub

        Public Property Key As String

        Public ReadOnly Property HasItems As Boolean
            Get
                Return Count > 0
            End Get
        End Property
    End Class

End Namespace
