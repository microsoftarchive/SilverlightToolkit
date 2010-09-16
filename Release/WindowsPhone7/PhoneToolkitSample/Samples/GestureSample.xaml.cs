// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Media;
using Microsoft.Phone.Controls;

namespace PhoneToolkitSample.Samples
{
    public partial class GestureSample : PhoneApplicationPage
    {
        SolidColorBrush greenBrush = new SolidColorBrush(Colors.Green);
        SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
        SolidColorBrush normalBrush;

        double initialAngle;
        double initialScale;

        public GestureSample()
        {
            InitializeComponent();

            normalBrush = (SolidColorBrush) Resources["PhoneAccentBrush"];
        }

        private void OnTap(object sender, GestureEventArgs e)
        {
            transform.TranslateX = transform.TranslateY = 0;
        }

        private void OnDoubleTap(object sender, GestureEventArgs e)
        {
            transform.ScaleX = transform.ScaleY = 1;
        }

        private void OnHold(object sender, GestureEventArgs e)
        {
            transform.TranslateX = transform.TranslateY = 0;
            transform.ScaleX = transform.ScaleY = 1;
            transform.Rotation = 0;
        }

        private void OnDragStarted(object sender, DragStartedGestureEventArgs e)
        {
            border.Background = greenBrush;
        }

        private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            transform.TranslateX += e.HorizontalChange;
            transform.TranslateY += e.VerticalChange;
        }

        private void OnDragCompleted(object sender, DragCompletedGestureEventArgs e)
        {
            border.Background = normalBrush;
        }

        private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            border.Background = redBrush;

            initialAngle = transform.Rotation;
            initialScale = transform.ScaleX;
        }

        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            transform.Rotation = initialAngle + e.TotalAngleDelta;
            transform.ScaleX = transform.ScaleY = initialScale * e.DistanceRatio;
        }

        private void OnPinchCompleted(object sender, PinchGestureEventArgs e)
        {
            border.Background = normalBrush;
        }

        private void OnFlick(object sender, FlickGestureEventArgs e)
        {
            flickData.Text = string.Format("{0} Flick: Angle {1} Velocity {2},{3}",
                e.Direction, Math.Round(e.Angle), e.HorizontalVelocity, e.VerticalVelocity);
        }
    }
}