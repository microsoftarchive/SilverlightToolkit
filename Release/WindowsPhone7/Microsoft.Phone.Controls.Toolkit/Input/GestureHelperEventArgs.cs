// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// The base class for all gesture events. Also used by Tap, DoubleTap and Hold.
    /// </summary>
    public class GestureEventArgs : EventArgs
    {
        /// <summary>
        /// The point, in unrotated screen coordinates, where the gesture occurred.
        /// </summary>
        protected Point Origin0 { get; private set; }

        internal GestureEventArgs(Point hostOrigin)
        {
            Origin0 = hostOrigin;
        }

        /// <summary>
        /// The first hit-testable item under the touch point. Determined by a combination of order in the tree and
        /// Z-order.
        /// </summary>
        public object OriginalSource { get; internal set; }

        /// <summary>
        /// If an event handler sets this to true, it stops event bubbling.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Returns the position of the gesture's starting point relative to a given UIElement.
        /// </summary>
        /// <param name="relativeTo">The return value will be relative to this element.</param>
        /// <returns>The gesture's starting point relative to the given UIElement.</returns>
        public Point GetPosition(UIElement relativeTo)
        {
            return GetPosition(relativeTo, Origin0);
        }

        /// <summary>
        /// Returns the position of a given point relative to a given UIElement.
        /// </summary>
        /// <param name="relativeTo">The return value will be relative to this element.</param>
        /// <param name="point">The point to translate.</param>
        /// <returns>The given point relative to the given UIElement.</returns>
        protected static Point GetPosition(UIElement relativeTo, Point point)
        {
            if (relativeTo == null)
            {
                // Transform relative to RootVisual
                relativeTo = Application.Current.RootVisual;
            }
            if (relativeTo != null)
            {
                // Determine position
                GeneralTransform transform = relativeTo.TransformToVisual(null).Inverse;
                return transform.Transform(point);
            }
            else
            {
                // Unable to transform; return point as-is
                return point;
            }
        }
    }

    /// <summary>
    /// The event args used in the DragStarted event.
    /// </summary>
    public class DragStartedGestureEventArgs : GestureEventArgs
    {
        internal DragStartedGestureEventArgs(Point hostOrigin, Orientation direction) 
            : base(hostOrigin)
        {
            Direction = direction;
        }

        /// <summary>
        /// The direction of the drag gesture, as determined by the initial drag change.
        /// </summary>
        public Orientation Direction { get; private set; }
    }

    /// <summary>
    /// The event args used by the DragDelta event.
    /// </summary>
    public class DragDeltaGestureEventArgs : GestureEventArgs
    {
        internal DragDeltaGestureEventArgs(Point hostOrigin, Point change, Orientation direction) 
            : base(hostOrigin)
        {
            HorizontalChange = change.X;
            VerticalChange = change.Y;
            Direction = direction;
        }

        /// <summary>
        /// The horizontal (X) change for this drag event.
        /// </summary>
        public double HorizontalChange { get; private set; }

        /// <summary>
        /// The vertical (Y) change for this drag event.
        /// </summary>
        public double VerticalChange { get; private set; }

        /// <summary>
        /// The direction of the drag gesture, as determined by the initial drag change.
        /// </summary>
        public Orientation Direction { get; private set; }
    }

    /// <summary>
    /// The event args used by the DragCompleted event.
    /// </summary>
    public class DragCompletedGestureEventArgs : GestureEventArgs
    {
        internal DragCompletedGestureEventArgs(Point hostOrigin, Point change, Orientation direction, Point finalVelocity) 
            : base(hostOrigin)
        {
            HorizontalChange = change.X;
            VerticalChange = change.Y;
            Direction = direction;
            HorizontalVelocity = finalVelocity.X;
            VerticalVelocity = finalVelocity.Y;
        }

        /// <summary>
        /// The total horizontal (X) change of the drag event.
        /// </summary>
        public double HorizontalChange { get; private set; }

        /// <summary>
        /// The total vertical (Y) change of the drag event.
        /// </summary>
        public double VerticalChange { get; private set; }

        /// <summary>
        /// The direction of the drag gesture, as determined by the initial drag change.
        /// </summary>
        public Orientation Direction { get; private set; }

        /// <summary>
        /// The final horizontal (X) velocity of the drag, if the drag was inertial.
        /// </summary>
        public double HorizontalVelocity { get; private set; }

        /// <summary>
        /// The final vertical (Y) velocity of the drag, if the drag was inertial.
        /// </summary>
        public double VerticalVelocity { get; private set; }
    }

    /// <summary>
    /// The event args used by the Flick event.
    /// </summary>
    public class FlickGestureEventArgs : GestureEventArgs
    {
        private Point _velocity;

        internal FlickGestureEventArgs(Point hostOrigin, Point velocity) 
            : base(hostOrigin)
        {
            _velocity = velocity;
        }

        /// <summary>
        /// The horizontal (X) velocity of the flick.
        /// </summary>
        public double HorizontalVelocity { get { return _velocity.X; } }

        /// <summary>
        /// The vertical (Y) velocity of the flick.
        /// </summary>
        public double VerticalVelocity { get { return _velocity.Y; } }

        /// <summary>
        /// The angle of the flick.
        /// </summary>
        public double Angle
        {
            get { return MathHelpers.GetAngle(_velocity.X, _velocity.Y); }
        }

        /// <summary>
        /// The direction of the flick gesture, as determined by the flick velocities.
        /// </summary>
        public Orientation Direction
        {
            get { return Math.Abs(_velocity.X) >= Math.Abs(_velocity.Y) ? Orientation.Horizontal : Orientation.Vertical; }
        }
    }

    /// <summary>
    /// The base class for multi-touch gesture event args. Currently used only for
    /// two-finger (pinch) operations.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
    public class MultiTouchGestureEventArgs : GestureEventArgs
    {
        /// <summary>
        /// The second touch point. The first is stored in GestureEventArgs.
        /// </summary>
        protected Point Origin1 { get; private set; }

        internal MultiTouchGestureEventArgs(Point origin0, Point origin1)
            : base(origin0)
        {
            Origin1 = origin1;
        }

        /// <summary>
        /// Returns the position of either of the two touch points (0 or 1) relative to
        /// the UIElement provided.
        /// </summary>
        /// <param name="relativeTo">The return value will be relative to this element.</param>
        /// <param name="index">The touchpoint to use (0 or 1).</param>
        /// <returns>The gesture's starting point relative to the given UIElement.</returns>
        public Point GetPosition(UIElement relativeTo, int index)
        {
            if (index == 0)
            {
                return GetPosition(relativeTo);
            }
            else if (index == 1)
            {
                return GetPosition(relativeTo, Origin1);
            }
            else
                throw new ArgumentOutOfRangeException("index");
        }
    }

    /// <summary>
    /// The event args used by the PinchStarted event.
    /// </summary>
    public class PinchStartedGestureEventArgs : MultiTouchGestureEventArgs
    {
        internal PinchStartedGestureEventArgs(Point pinch0, Point pinch1)
            : base(pinch0, pinch1)
        {
        }

        /// <summary>
        /// The distance between the two touch points.
        /// </summary>
        public double Distance
        {
            get { return MathHelpers.GetDistance(Origin0, Origin1); }
        }

        /// <summary>
        /// The angle defined by the two touch points.
        /// </summary>
        public double Angle
        {
            get { return MathHelpers.GetAngle(Origin1.X - Origin0.X, Origin1.Y - Origin0.Y); }
        }
    }

    /// <summary>
    /// The event args used by the PinchDelta and PinchCompleted events.
    /// </summary>
    public class PinchGestureEventArgs : MultiTouchGestureEventArgs
    {
        // The current position of the first touch point
        private Point _position0;

        // The current position of the second touch point
        private Point _position1;

        internal PinchGestureEventArgs(Point origin0, Point origin1, Point position0, Point position1)
            : base(origin0, origin1)
        {
            _position0 = position0;
            _position1 = position1;
        }

        /// <summary>
        /// Returns the ratio of the current distance between touchpoints / the original distance
        /// between the touchpoints.
        /// </summary>
        public double DistanceRatio
        {
            get
            {
                double originalDistance = Math.Max(MathHelpers.GetDistance(Origin0, Origin1), 1.0);
                double newDistance = Math.Max(MathHelpers.GetDistance(_position0, _position1), 1.0);

                return newDistance / originalDistance;
            }
        }

        /// <summary>
        /// Returns the difference in angle between the current touch positions and the original
        /// touch positions.
        /// </summary>
        public double TotalAngleDelta
        {
            get
            {
                double oldAngle = MathHelpers.GetAngle(Origin1.X - Origin0.X, Origin1.Y - Origin0.Y);
                double newAngle = MathHelpers.GetAngle(_position1.X - _position0.X, _position1.Y - _position0.Y);

                return newAngle - oldAngle;
            }
        }
    }
}
