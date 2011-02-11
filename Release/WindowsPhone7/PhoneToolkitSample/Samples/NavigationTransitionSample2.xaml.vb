' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Namespace Samples

    Partial Public Class NavigationTransitionSample2
        Inherits PhoneApplicationPage
        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub Forward(ByVal sender As Object, ByVal e As RoutedEventArgs)
            NavigationService.Navigate(New Uri("/Samples/NavigationTransitionSample1.xaml", UriKind.Relative))
        End Sub
    End Class
End Namespace
