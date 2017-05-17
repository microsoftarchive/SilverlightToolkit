using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Xna.Framework.Input
{
    /// <summary>
    /// Allows retrieval of position and button clicks from a mouse input device. 
    /// </summary>
    public static class Mouse
    {
        #region Fields

        static Control _rootControl;
        static System.Windows.Point _currentMousePosition;
        static bool leftButtonDown;
        static bool rightButtonDown;
        static int wheelValue;
        
        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the control used for mouse processing. Mouse coordinates returned by GetState are relative to the upper-left corner of this control.
        /// </summary>
        public static Control RootControl
        {
            get
            {
                return _rootControl;
            }
            set
            {
                if (_rootControl != null)
                {
                    _rootControl.MouseMove -= _rootControl_MouseMove;
                    _rootControl.MouseLeftButtonDown -= _rootControl_MouseLeftButtonDown;
                    _rootControl.MouseLeftButtonUp -= _rootControl_MouseLeftButtonUp;
                    _rootControl.MouseRightButtonDown -= _rootControl_MouseRightButtonDown;
                    _rootControl.MouseRightButtonUp -= _rootControl_MouseRightButtonUp;
                    _rootControl.MouseWheel -= _rootControl_MouseWheel;
                }

                _rootControl = value;

                _rootControl.MouseMove += _rootControl_MouseMove;
                _rootControl.MouseLeftButtonDown += _rootControl_MouseLeftButtonDown;
                _rootControl.MouseLeftButtonUp += _rootControl_MouseLeftButtonUp;
                _rootControl.MouseRightButtonDown += _rootControl_MouseRightButtonDown;
                _rootControl.MouseRightButtonUp += _rootControl_MouseRightButtonUp;
                _rootControl.MouseWheel += _rootControl_MouseWheel;
            }
        }

        #endregion

        #region Event handlers
        
        static void _rootControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            wheelValue += e.Delta;
            GetCurrentPosition(e);
        }

        static void _rootControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            rightButtonDown = false;
            GetCurrentPosition(e);
        }

        static void _rootControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            rightButtonDown = true;
            GetCurrentPosition(e);
            e.Handled = true;
        }

        static void _rootControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            leftButtonDown = false;
            GetCurrentPosition(e);
        }

        static void _rootControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            leftButtonDown = true;
            GetCurrentPosition(e);
        }

        static void _rootControl_MouseMove(object sender, MouseEventArgs e)
        {
            GetCurrentPosition(e);
        }

        static void GetCurrentPosition(MouseEventArgs e)
        {
            _currentMousePosition = e.GetPosition(_rootControl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the current state of the mouse, including mouse position and buttons pressed.
        /// </summary>
        public static MouseState GetState()
        {
            if (_rootControl == null)
                throw new NullReferenceException("You must set the RootControl before calling GetState");

            MouseState result = default(MouseState);

            result.x = (int)_currentMousePosition.X;
            result.y = (int)_currentMousePosition.Y;
            result.leftButton = leftButtonDown ? ButtonState.Pressed : ButtonState.Released;
            result.rightButton = rightButtonDown ? ButtonState.Pressed : ButtonState.Released;
            result.wheel = wheelValue;

            return result;
        }

        /// <summary>
        /// Sets the position of the mouse cursor relative to the upper-left corner of the control.
        /// </summary>
        public static void SetPosition(int x, int y)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
