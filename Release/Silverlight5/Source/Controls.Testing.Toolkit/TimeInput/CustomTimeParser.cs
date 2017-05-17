// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Test implementation of a TimeParser.
    /// </summary>
    public class CustomTimeParser : TimeParser
    {
        /// <summary>
        /// Tries to parse a string to a DateTime.
        /// </summary>
        /// <param name="text">The text that should be parsed.</param>
        /// <param name="culture">The culture being used.</param>
        /// <param name="result">The parsed DateTime.</param>
        /// <returns>
        /// True if the parse was succesfull, false if it was not.
        /// </returns>
        public override bool TryParse(string text, System.Globalization.CultureInfo culture, out DateTime? result)
        {
            result = null;
            if (text == "3 uur 40")
            {
                result = new DateTime(1, 1, 1, 15, 40, 0);
                return true;
            }
            return false;
        }
    }
}
