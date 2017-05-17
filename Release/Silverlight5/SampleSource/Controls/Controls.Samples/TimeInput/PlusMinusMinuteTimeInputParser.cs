// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using System.Text.RegularExpressions;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Custom parser that will parse a plus or minus
    /// symbol and a number to an offset of minutes.
    /// </summary>
    /// <example>input: +2m, output DateTime.Now plus 2 minutes.</example>
    public class PlusMinusMinuteTimeInputParser : TimeParser
    {
        /// <summary>
        /// Expression used to parse.
        /// </summary>
        private static readonly Regex exp = new Regex(@"(?<minutes>[+|-]\d{1,2})[m|M]", RegexOptions.CultureInvariant);

        /// <summary>
        /// Tries to parse a string to a DateTime.
        /// </summary>
        /// <param name="text">The text that should be parsed.</param>
        /// <param name="culture">The culture being used.</param>
        /// <param name="result">The parsed DateTime.</param>
        /// <returns>
        /// True if the parse was successful, false if it was not.
        /// </returns>
        public override bool TryParse(string text, CultureInfo culture, out DateTime? result)
        {
            Match match = exp.Match(text);

            if (match.Success)
            {
                int number = int.Parse(match.Groups["minutes"].Value, culture);
                result = DateTime.Now.AddMinutes(number);
                return true;
            }
            result = null;
            return false;
        }
    }
}
