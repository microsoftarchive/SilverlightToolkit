using System;
using System.Windows.Input;

namespace Microsoft.Xna.Framework.Input
{
    /// <summary>
    /// Represents a state of keystrokes recorded by a keyboard input device. 
    /// </summary>
    public struct KeyboardState
    {
        #region Fields

        static readonly uint stateMask0;
        static readonly uint stateMask1;
        static readonly uint stateMask2;
        static readonly uint stateMask3;
        static readonly uint stateMask4;
        static readonly uint stateMask5;
        static readonly uint stateMask6;
        static readonly uint stateMask7;
        uint currentState0;
        uint currentState1;
        uint currentState2;
        uint currentState3;
        uint currentState4;
        uint currentState5;
        uint currentState6;
        uint currentState7;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the state of a particular key.
        /// </summary>
        public KeyState this[Key key]
        {
            get
            {
                uint num;
                switch ((Key) ((int) key >> 5))
                {
                    case Key.None:
                        {
                            num = currentState0;
                            break;
                        }
                    case (Key) 1:
                        {
                            num = currentState1;
                            break;
                        }
                    case (Key) 2:
                        {
                            num = currentState2;
                            break;
                        }
                    case (Key)3:
                        {
                            num = currentState3;
                            break;
                        }
                    case (Key)4:
                        {
                            num = currentState4;
                            break;
                        }
                    case (Key)5:
                        {
                            num = currentState5;
                            break;
                        }
                    case (Key)6:
                        {
                            num = currentState6;
                            break;
                        }
                    case (Key)7:
                        {
                            num = currentState7;
                            break;
                        }
                    default:
                        {
                            return KeyState.Up;
                        }
                }
                uint num2 = 1u << (int) key;
                if ((num & num2) == 0u)
                {
                    return KeyState.Up;
                }
                return KeyState.Down;
            }
        }

        #endregion

        #region Creation

        static KeyboardState()
        {
            stateMask0 = 4294967295u;
            stateMask1 = 4294967295u;
            stateMask2 = 4294967295u;
            stateMask3 = 4294967295u;
            stateMask4 = 4294967295u;
            stateMask5 = 4294967295u;
            stateMask6 = 4294967295u;
            stateMask7 = 4294967295u;
            KeyboardState keyboardState = default(KeyboardState);
            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                keyboardState.AddPressedKey(key);
            }
            stateMask0 = keyboardState.currentState0;
            stateMask1 = keyboardState.currentState1;
            stateMask2 = keyboardState.currentState2;
            stateMask3 = keyboardState.currentState3;
            stateMask4 = keyboardState.currentState4;
            stateMask5 = keyboardState.currentState5;
            stateMask6 = keyboardState.currentState6;
            stateMask7 = keyboardState.currentState7;
        }

        /// <summary>
        /// Initializes a new instance of the KeyboardState class. 
        /// </summary>
        internal KeyboardState(KeyboardState sourceKeyboardState)
        {
            currentState0 = sourceKeyboardState.currentState0;
            currentState1 = sourceKeyboardState.currentState1;
            currentState2 = sourceKeyboardState.currentState2;
            currentState3 = sourceKeyboardState.currentState3;
            currentState4 = sourceKeyboardState.currentState4;
            currentState5 = sourceKeyboardState.currentState5;
            currentState6 = sourceKeyboardState.currentState6;
            currentState7 = sourceKeyboardState.currentState7;
        }

        /// <summary>
        /// Initializes a new instance of the KeyboardState class. 
        /// </summary>
        public KeyboardState(params Key[] keys)
        {
            currentState0 = (currentState1 = (currentState2 = (currentState3 = (currentState4 = (currentState5 = (currentState6 = (currentState7 = 0u)))))));
            
            if (keys == null) 
                return;

            foreach (Key t in keys)
            {
                AddPressedKey(t);
            }
        }

        #endregion

        #region Methods

        internal void AddPressedKey(Key key)
        {
            int keyValue = (int) key;
            uint num = 1u << keyValue;
            switch (keyValue >> 5)
            {
                case 0:
                    {
                        currentState0 |= (num & stateMask0);
                        return;
                    }
                case 1:
                    {
                        currentState1 |= (num & stateMask1);
                        return;
                    }
                case 2:
                    {
                        currentState2 |= (num & stateMask2);
                        return;
                    }
                case 3:
                    {
                        currentState3 |= (num & stateMask3);
                        return;
                    }
                case 4:
                    {
                        currentState4 |= (num & stateMask4);
                        return;
                    }
                case 5:
                    {
                        currentState5 |= (num & stateMask5);
                        return;
                    }
                case 6:
                    {
                        currentState6 |= (num & stateMask6);
                        return;
                    }
                case 7:
                    {
                        currentState7 |= (num & stateMask7);
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        internal void RemovePressedKey(Key key)
        {
            int keyValue = (int) key;
            uint num = 1u << keyValue;
            switch (keyValue >> 5)
            {
                case 0:
                    {
                        currentState0 &= ~(num & stateMask0);
                        return;
                    }
                case 1:
                    {
                        currentState1 &= ~(num & stateMask1);
                        return;
                    }
                case 2:
                    {
                        currentState2 &= ~(num & stateMask2);
                        return;
                    }
                case 3:
                    {
                        currentState3 &= ~(num & stateMask3);
                        return;
                    }
                case 4:
                    {
                        currentState4 &= ~(num & stateMask4);
                        return;
                    }
                case 5:
                    {
                        currentState5 &= ~(num & stateMask5);
                        return;
                    }
                case 6:
                    {
                        currentState6 &= ~(num & stateMask6);
                        return;
                    }
                case 7:
                    {
                        currentState7 &= ~(num & stateMask7);
                        return;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        /// <summary>
        /// Returns whether a specified key is currently being pressed.
        /// </summary>
        public bool IsKeyDown(Key key)
        {
            return this[key] == KeyState.Down;
        }

        /// <summary>
        /// Returns whether a specified key is currently not pressed.
        /// </summary>
        public bool IsKeyUp(Key key)
        {
            return this[key] == KeyState.Up;
        }

        /// <summary>
        /// Gets an array of values that correspond to the keyboard keys that are currently being pressed.
        /// </summary>
        public Key[] GetPressedKeys()
        {
            int num = 0;
            CheckPressedKeys(currentState0, 0, null, ref num);
            CheckPressedKeys(currentState1, 0, null, ref num);
            CheckPressedKeys(currentState2, 0, null, ref num);
            CheckPressedKeys(currentState3, 0, null, ref num);
            CheckPressedKeys(currentState4, 0, null, ref num);
            CheckPressedKeys(currentState5, 0, null, ref num);
            CheckPressedKeys(currentState6, 0, null, ref num);
            CheckPressedKeys(currentState7, 0, null, ref num);
            Key[] array = new Key[num];

            if (num > 0)
            {
                int num2 = 0;
                CheckPressedKeys(currentState0, 0, array, ref num2);
                CheckPressedKeys(currentState1, 1, array, ref num2);
                CheckPressedKeys(currentState2, 2, array, ref num2);
                CheckPressedKeys(currentState3, 3, array, ref num2);
                CheckPressedKeys(currentState4, 4, array, ref num2);
                CheckPressedKeys(currentState5, 5, array, ref num2);
                CheckPressedKeys(currentState6, 6, array, ref num2);
                CheckPressedKeys(currentState7, 7, array, ref num2);
            }
            return array;
        }

        static void CheckPressedKeys(uint packedState, int packedOffset, Key[] pressedKeys, ref int index)
        {
            if (packedState == 0u)
            {
                return;
            }
            for (int i = 0; i < 32; i++)
            {
                if ((packedState & (ulong) (1L << (i & 31))) != 0uL)
                {
                    if (pressedKeys != null)
                    {
                        pressedKeys[index] = (Key)(packedOffset * 32 + i);
                    }
                    index++;
                }
            }
        }

        public override int GetHashCode()
        {
            return currentState0.GetHashCode() ^ currentState1.GetHashCode() ^ currentState2.GetHashCode() ^ currentState3.GetHashCode() ^ currentState4.GetHashCode() ^ currentState5.GetHashCode() ^ currentState6.GetHashCode() ^ currentState7.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is KeyboardState && this == (KeyboardState) obj;
        }

        /// <summary>
        /// Compares two objects to determine whether they are the same.
        /// </summary>
        public static bool operator ==(KeyboardState a, KeyboardState b)
        {
            return a.currentState0 == b.currentState0 && a.currentState1 == b.currentState1 && a.currentState2 == b.currentState2 && a.currentState3 == b.currentState3 && a.currentState4 == b.currentState4 && a.currentState5 == b.currentState5 && a.currentState6 == b.currentState6 && a.currentState7 == b.currentState7;
        }

        /// <summary>
        /// Compares two objects to determine whether they are different.
        /// </summary>
        public static bool operator !=(KeyboardState a, KeyboardState b)
        {
            return !(a == b);
        }

        #endregion
    }
}
