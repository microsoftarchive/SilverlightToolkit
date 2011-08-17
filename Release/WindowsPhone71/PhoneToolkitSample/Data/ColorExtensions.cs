// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;

namespace PhoneToolkitSample.Data
{
    /// <summary>
    /// Adds extension methods relating to color.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// An array of all the names of the accent colors.
        /// </summary>
        private static string[] _accentColors = { "magenta", 
                                                  "purple",
                                                  "teal", 
                                                  "lime", 
                                                  "brown", 
                                                  "pink", 
                                                  "mango",
                                                  "blue",
                                                  "red",
                                                  "green" };


        /// <summary>
        /// Returns an array of all the names of the accent colors.
        /// </summary>
        public static ReadOnlyCollection<string> AccentColors()
        {
            return new ReadOnlyCollection<string>(_accentColors);
        }

        /// <summary>
        /// Returns a Color for a hex value.
        /// </summary>
        /// <param name="argb">The hex value</param>
        /// <remarks>Calls to this method should look like 0xFF11FF11.ToColor().</remarks>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "argb", Justification ="By design")]
        public static Color ToColor(this uint argb)
        {
            return Color.FromArgb((byte)((argb & 0xff000000) >> 0x18),
                                  (byte)((argb & 0xff0000) >> 0x10),
                                  (byte)((argb & 0xff00) >> 8),
                                  (byte)(argb & 0xff));
        }

        /// <summary>
        /// Returns a SolidColorBrush for a hex value.
        /// </summary>
        /// <param name="argb">The hex value</param>
        /// <remarks>Calls to this method should look like 0xFF11FF11.ToSolidColorBrush().</remarks>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "argb", Justification ="By design")]
        public static SolidColorBrush ToSolidColorBrush(this uint argb)
        {
            return new SolidColorBrush(argb.ToColor());
        }
    }
}
