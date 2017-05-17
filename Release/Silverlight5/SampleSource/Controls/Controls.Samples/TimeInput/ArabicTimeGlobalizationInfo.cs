// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Globalization;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Overridden TimeGlobalizationInfo for shows mapping of characters to 
    /// Arabic.
    /// </summary>
    public class ArabicTimeGlobalizationInfo : TimeGlobalizationInfo
    {
        /// <summary>
        /// Dictionary helpful for translation.
        /// </summary>
        private readonly Dictionary<int, char> arabicNumbers = new Dictionary<int, char>
                                                                    {
                                                                            { 0, '٠' },
                                                                            { 1, '١' },
                                                                            { 2, '٢' },
                                                                            { 3, '٣' },
                                                                            { 4, '٤' },
                                                                            { 5, '٥' },
                                                                            { 6, '٦' },
                                                                            { 7, '٧' },
                                                                            { 8, '٨' },
                                                                            { 9, '٩' }
                                                                    };

        /// <summary>
        /// Returns the global representation of a character.
        /// </summary>
        /// <param name="input">Character that will be mapped to a different
        /// character.</param>
        /// <returns>
        /// The global version of a character that represents the input.
        /// </returns>
        protected override char MapDigitToCharacter(int input)
        {
            if (arabicNumbers.ContainsKey(input))
            {
                return arabicNumbers[input];
            }
            else
            {
                return base.MapDigitToCharacter(input);
            }
        }

        /// <summary>
        /// Returns the char that is represented by a global character.
        /// </summary>
        /// <param name="input">The global version of the character that needs
        /// to be mapped to a regular character.</param>
        /// <returns>
        /// The character that represents the global version of a character.
        /// </returns>
        protected override char MapCharacterToDigit(char input)
        {
            if (arabicNumbers.ContainsValue(input))
            {
                foreach (var pair in arabicNumbers)
                {
                    if (pair.Value == input)
                    {
                        return pair.Key.ToString(CultureInfo.InvariantCulture)[0];
                    }
                }
            }

            return base.MapCharacterToDigit(input);
        }
    }
}