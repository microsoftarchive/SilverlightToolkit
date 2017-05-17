' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

''' <summary>
''' A Page that displays values assigned by query string.
''' </summary>
Public Class QueryPage
    Inherits Page

    ''' <summary>
    ''' Initializes a QueryPage.
    ''' </summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Executes when the user navigates to this page.
    ''' </summary>
    ''' <param name="e">Event arguments.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As Windows.Navigation.NavigationEventArgs)
        x.Text = GetArgument("x")
        y.Text = GetArgument("y")
        z.Text = GetArgument("z")
    End Sub

    ''' <summary>
    ''' Gets a value passed in through the query if available.
    ''' </summary>
    ''' <param name="argName">The key to find.</param>
    ''' <returns>The value associated with the key if available, otherwise "{Not Specified}".</returns>
    Private Function GetArgument(ByVal argName As String) As String
        If NavigationContext.QueryString.ContainsKey(argName) Then
            Return NavigationContext.QueryString(argName)
        End If
        Return "{Not Specified}"
    End Function
End Class