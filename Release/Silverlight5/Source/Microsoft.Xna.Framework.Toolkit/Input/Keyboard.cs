using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Xna.Framework.Input
{
    /// <summary>
    /// Allows retrieval of keystrokes from a keyboard input device. 
    /// </summary>
    public static class Keyboard
    {
        #region Fields

        static KeyboardState keyboardState = default(KeyboardState);
        static Control _rootControl;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the control used for keyboard processing.
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
                    _rootControl.KeyDown -= _rootControl_KeyDown;
                    _rootControl.KeyUp -= _rootControl_KeyUp;
                }

                _rootControl = value;

                _rootControl.KeyDown += _rootControl_KeyDown;
                _rootControl.KeyUp += _rootControl_KeyUp;
            }
        }

        #endregion

        #region Events handling

        static void _rootControl_KeyUp(object sender, KeyEventArgs e)
        {
            keyboardState.RemovePressedKey(e.Key);
            CheckModifiers();
        }

        static void _rootControl_KeyDown(object sender, KeyEventArgs e)
        {
            keyboardState.AddPressedKey(e.Key);
            CheckModifiers();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the current keyboard state.
        /// </summary>
        public static KeyboardState GetState()
        {
            if (_rootControl == null)
                throw new NullReferenceException("You must set the RootControl before calling GetState");

            return new KeyboardState(keyboardState);
        }

        /// <summary>
        /// We must handle modifier separately because sometimes they won't raise keyup event.
        /// </summary>
        static void CheckModifiers()
        {
            // Modifiers
            if ((System.Windows.Input.Keyboard.Modifiers & ModifierKeys.Control) > 0)
            {
                keyboardState.AddPressedKey(Key.Ctrl);
            }
            else
            {
                keyboardState.RemovePressedKey(Key.Ctrl);
            }

            if ((System.Windows.Input.Keyboard.Modifiers & ModifierKeys.Alt) > 0)
            {
                keyboardState.AddPressedKey(Key.Alt);
            }
            else
            {
                keyboardState.RemovePressedKey(Key.Alt);
            }

            if ((System.Windows.Input.Keyboard.Modifiers & ModifierKeys.Shift) > 0)
            {
                keyboardState.AddPressedKey(Key.Shift);
            }
            else
            {
                keyboardState.RemovePressedKey(Key.Shift);
            }
        }

        #endregion
    }
}
