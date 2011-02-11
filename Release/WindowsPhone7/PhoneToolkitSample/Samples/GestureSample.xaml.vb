' (c) Copyright Microsoft Corporation.
' This source is subject to the Microsoft Public License (Ms-PL).
' Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
' All other rights reserved.

Namespace Samples

    Partial Public Class GestureSample
        Inherits PhoneApplicationPage
        Private greenBrush As New SolidColorBrush(Colors.Green)
        Private redBrush As New SolidColorBrush(Colors.Red)
        Private normalBrush As SolidColorBrush

        Private initialAngle As Double
        Private initialScale As Double

        Public Sub New()
            InitializeComponent()

            normalBrush = CType(Resources("PhoneAccentBrush"), SolidColorBrush)
        End Sub

        Private Sub OnTap(ByVal sender As Object, ByVal e As GestureEventArgs)
            transform.TranslateY = 0
            transform.TranslateX = transform.TranslateY
        End Sub

        Private Sub OnDoubleTap(ByVal sender As Object, ByVal e As GestureEventArgs)
            transform.ScaleY = 1
            transform.ScaleX = transform.ScaleY
        End Sub

        Private Sub OnHold(ByVal sender As Object, ByVal e As GestureEventArgs)
            transform.TranslateY = 0
            transform.TranslateX = transform.TranslateY
            transform.ScaleY = 1
            transform.ScaleX = transform.ScaleY
            transform.Rotation = 0
        End Sub

        Private Sub OnDragStarted(ByVal sender As Object, ByVal e As DragStartedGestureEventArgs)
            border.Background = greenBrush
        End Sub

        Private Sub OnDragDelta(ByVal sender As Object, ByVal e As DragDeltaGestureEventArgs)
            transform.TranslateX += e.HorizontalChange
            transform.TranslateY += e.VerticalChange
        End Sub

        Private Sub OnDragCompleted(ByVal sender As Object, ByVal e As DragCompletedGestureEventArgs)
            border.Background = normalBrush
        End Sub

        Private Sub OnPinchStarted(ByVal sender As Object, ByVal e As PinchStartedGestureEventArgs)
            border.Background = redBrush

            initialAngle = transform.Rotation
            initialScale = transform.ScaleX
        End Sub

        Private Sub OnPinchDelta(ByVal sender As Object, ByVal e As PinchGestureEventArgs)
            transform.Rotation = initialAngle + e.TotalAngleDelta
            transform.ScaleY = initialScale * e.DistanceRatio
            transform.ScaleX = transform.ScaleY
        End Sub

        Private Sub OnPinchCompleted(ByVal sender As Object, ByVal e As PinchGestureEventArgs)
            border.Background = normalBrush
        End Sub

        Private Sub OnFlick(ByVal sender As Object, ByVal e As FlickGestureEventArgs)
            flickData.Text = String.Format("{0} Flick: Angle {1} Velocity {2},{3}",
                                           e.Direction, Math.Round(e.Angle), e.HorizontalVelocity, e.VerticalVelocity)
        End Sub
    End Class

End Namespace
