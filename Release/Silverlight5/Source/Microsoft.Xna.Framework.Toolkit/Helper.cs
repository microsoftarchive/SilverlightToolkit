using System;
using System.Globalization;

namespace Microsoft.Xna.Framework.Toolkit
{
    internal static class Helper
    {
        public static T Convert<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static int ConvertToInt32(this string value)
        {
            value = value.ToLower();

            if (value.StartsWith("0x"))
                return int.Parse(value.Substring(2), NumberStyles.HexNumber);

            return int.Parse(value);
        }

        public static float ConvertToFloat(this string value)
        {
            value = value.ToLower();

            if (value.EndsWith("f"))
                value = value.Substring(0, value.Length - 1);

            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        public static Color ConvertToColor(this int value)
        {
            int alpha = (int)((value & 0xFF000000) >> 24);
            int red = (value & 0x00FF0000) >> 16;
            int green = (value & 0x0000FF00) >> 8;
            int blue = (value & 0x000000FF);

            return new Color(red, green, blue, alpha);
        }
    }
}
