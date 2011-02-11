' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Imports PhoneToolkitSample.Data

Namespace Samples

    Partial Public Class PersonDetail
        Inherits PhoneApplicationPage
        Public Sub New()
            InitializeComponent()

            quote.Text =
                LoremIpsum.GetParagraph(4) & Environment.NewLine & Environment.NewLine &
                LoremIpsum.GetParagraph(8) & Environment.NewLine & Environment.NewLine &
                LoremIpsum.GetParagraph(6)
        End Sub

        Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
            MyBase.OnNavigatedTo(e)
            Dim idParam As String = Nothing
            If NavigationContext.QueryString.TryGetValue("ID", idParam) Then
                Dim id = Int32.Parse(idParam)
                DataContext = AllPeople.Current(id)
            End If
        End Sub
    End Class

End Namespace
