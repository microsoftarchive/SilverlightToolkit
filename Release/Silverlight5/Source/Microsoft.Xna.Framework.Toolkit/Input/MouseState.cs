using System.Globalization;

namespace Microsoft.Xna.Framework.Input
{
    /// <summary>
    /// Represents the state of a mouse input device, including mouse cursor position and buttons pressed. 
    /// </summary>
    public struct MouseState
    {
        #region Fields

        internal int x;
        internal int y;
        internal ButtonState leftButton;
        internal ButtonState rightButton;
        internal ButtonState middleButton;
        internal ButtonState xb1;
        internal ButtonState xb2;
        internal int wheel;

        #endregion

        #region Properties

        /// <summary>
        /// Specifies the horizontal position of the mouse cursor.
        /// </summary>
        public int X
        {
            get
            {
                return x;
            }
        }

        /// <summary>
        /// Specifies the vertical position of the mouse cursor.
        /// </summary>
        public int Y
        {
            get
            {
                return y;
            }
        }

        /// <summary>
        /// Returns the state of the left mouse button.
        /// </summary>
        public ButtonState LeftButton
        {
            get
            {
                return leftButton;
            }
        }

        /// <summary>
        /// Returns the state of the right mouse button.
        /// </summary>
        public ButtonState RightButton
        {
            get
            {
                return rightButton;
            }
        }

        /// <summary>
        /// Returns the state of the middle mouse button.
        /// </summary>
        public ButtonState MiddleButton
        {
            get
            {
                return middleButton;
            }
        }

        /// <summary>
        /// Returns the state of XBUTTON1.
        /// </summary>
        public ButtonState XButton1
        {
            get
            {
                return xb1;
            }
        }

        /// <summary>
        /// Returns the state of XBUTTON2.
        /// </summary>
        public ButtonState XButton2
        {
            get
            {
                return xb2;
            }
        }

        /// <summary>
        /// Gets the cumulative mouse scroll wheel value since the game was started.
        /// </summary>
        public int ScrollWheelValue
        {
            get
            {
                return wheel;
            }
        }

        #endregion

        #region Creation

        /// <summary>
        /// Initializes a new instance of the MouseState class.
        /// </summary>
        public MouseState(int x, int y, int scrollWheel, ButtonState leftButton, ButtonState middleButton, ButtonState rightButton, ButtonState xButton1, ButtonState xButton2)
        {
            this.x = x;
            this.y = y;
            wheel = scrollWheel;
            this.leftButton = leftButton;
            this.rightButton = rightButton;
            this.middleButton = middleButton;
            xb1 = xButton1;
            xb2 = xButton2;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ leftButton.GetHashCode() ^ rightButton.GetHashCode() ^ middleButton.GetHashCode() ^ xb1.GetHashCode() ^ xb2.GetHashCode() ^ wheel.GetHashCode();
        }

        public override string ToString()
        {
            string text = string.Empty;
            if (leftButton == ButtonState.Pressed)
            {
                text = text + (string.IsNullOrEmpty(text) ? "" : " ") + "Left";
            }
            if (rightButton == ButtonState.Pressed)
            {
                text = text + (string.IsNullOrEmpty(text) ? "" : " ") + "Right";
            }
            if (middleButton == ButtonState.Pressed)
            {
                text = text + (string.IsNullOrEmpty(text) ? "" : " ") + "Middle";
            }
            if (xb1 == ButtonState.Pressed)
            {
                text = text + (string.IsNullOrEmpty(text) ? "" : " ") + "XButton1";
            }
            if (xb2 == ButtonState.Pressed)
            {
                text = text + (string.IsNullOrEmpty(text) ? "" : " ") + "XButton2";
            }
            if (string.IsNullOrEmpty(text))
            {
                text = "None";
            }

            return string.Format(CultureInfo.CurrentCulture, "{{X:{0} Y:{1} Buttons:{2} Wheel:{3}}}", new object[]
            {
                x, 
                y, 
                text, 
                wheel
            });
        }

        public override bool Equals(object obj)
        {
            return obj is MouseState && this == (MouseState)obj;
        }

        /// <summary>
        /// Determines whether two MouseState instances are equal.
        /// </summary>
        public static bool operator ==(MouseState left, MouseState right)
        {
            return left.x == right.x && left.y == right.y && left.leftButton == right.leftButton && left.rightButton == right.rightButton && left.middleButton == right.middleButton && left.xb1 == right.xb1 && left.xb2 == right.xb2 && left.wheel == right.wheel;
        }

        /// <summary>
        /// Determines whether two MouseState instances are not equal.
        /// </summary>
        public static bool operator !=(MouseState left, MouseState right)
        {
            return !(left == right);
        }

        #endregion

    }
}
