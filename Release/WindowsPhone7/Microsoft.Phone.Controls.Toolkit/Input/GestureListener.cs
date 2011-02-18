// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Phone.Controls
{
    //public enum GestureType
    //{
    //    None = 0,
    //    Tap = 1,
    //    DoubleTap = 2,
    //    Hold = 4,
    //    HorizontalDrag = 8,
    //    VerticalDrag = 16,
    //    FreeDrag = 32,
    //    PinchDelta = 64,
    //    Flick = 128,
    //    DragCompleted = 256,
    //    PinchCompleted = 512,
    //}

    public partial class GestureListener
    {
        /// <summary>
        /// The GestureBegin event.
        /// </summary>
        public event EventHandler<GestureEventArgs> GestureBegin;

        /// <summary>
        /// The GestureCompleted event.
        /// </summary>
        public event EventHandler<GestureEventArgs> GestureCompleted;

        /// <summary>
        /// The Tap event (touch, release, no movement).
        /// </summary>
        public event EventHandler<GestureEventArgs> Tap;

        /// <summary>
        /// The DoubleTap event is raised instead of Tap if the time between two taps is short eonugh.
        /// </summary>
        public event EventHandler<GestureEventArgs> DoubleTap;

        /// <summary>
        /// The Hold event (touch and hold for one second)
        /// </summary>
        public event EventHandler<GestureEventArgs> Hold;

        /// <summary>
        /// The DragStarted event.
        /// </summary>
        public event EventHandler<DragStartedGestureEventArgs> DragStarted;

        /// <summary>
        /// The DragDelta event.
        /// </summary>
        public event EventHandler<DragDeltaGestureEventArgs> DragDelta;

        /// <summary>
        /// The DragCompleted event. Will be raised on touch release after a drag, or
        /// when a second touch point is added.
        /// </summary>
        public event EventHandler<DragCompletedGestureEventArgs> DragCompleted;

        /// <summary>
        /// The Flick event. Raised when a drag that was fast enough ends with a release.
        /// </summary>
        public event EventHandler<FlickGestureEventArgs> Flick;

        /// <summary>
        /// The PinchStarted event.
        /// </summary>
        public event EventHandler<PinchStartedGestureEventArgs> PinchStarted;

        /// <summary>
        /// Any two-touch point (two finger) operation.
        /// </summary>
        public event EventHandler<PinchGestureEventArgs> PinchDelta;

        /// <summary>
        /// The end of a pinch operation.
        /// </summary>
        public event EventHandler<PinchGestureEventArgs> PinchCompleted;
    }
}
