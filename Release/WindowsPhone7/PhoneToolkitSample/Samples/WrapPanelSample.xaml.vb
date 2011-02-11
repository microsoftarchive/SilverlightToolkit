' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Namespace Samples

    Partial Public Class WrapPanelSample
        Inherits PhoneApplicationPage
        Private rnd As New Random

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub OnClear(ByVal sender As Object, ByVal e As RoutedEventArgs)
            WrapPanel.Children.Clear()
        End Sub

        Private Sub OnAdd(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim count = Int32.Parse(CStr((CType(sender, FrameworkElement)).Tag))

            Dim tempVar = count > 0
            count -= 1
            Do While tempVar
                AddItem()
                tempVar = count > 0
                count -= 1
            Loop
        End Sub

        Private Sub AddItem()
            Dim b As New Border With {.Width = 100,
                                      .Height = 100,
                                      .Background = New SolidColorBrush(Color.FromArgb(255,
                                                                                       CByte(rnd.Next(256)),
                                                                                       CByte(rnd.Next(256)),
                                                                                       CByte(rnd.Next(256)))),
                                      .BorderThickness = New Thickness(2),
                                      .Margin = New Thickness(8)}

            b.BorderBrush = CType(Resources("PhoneForegroundBrush"), SolidColorBrush)

            Dim listener As GestureListener = GestureService.GetGestureListener(b)
            AddHandler listener.Tap, AddressOf WrapPanelSample_Tap

            WrapPanel.Children.Add(b)
        End Sub

        Private Sub WrapPanelSample_Tap(ByVal sender As Object, ByVal e As GestureEventArgs)
            Dim b = CType(sender, Border)
            WrapPanel.Children.Remove(b)
        End Sub

    End Class

End Namespace
