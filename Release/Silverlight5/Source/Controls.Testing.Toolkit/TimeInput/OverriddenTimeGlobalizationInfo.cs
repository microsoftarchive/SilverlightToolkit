// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Inherits from TimeGlobalizationInfo to open up some functionality that
    /// needs testing.
    /// </summary>
    public sealed class OverriddenTimeGlobalizationInfo : TimeGlobalizationInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance will globalize 
        /// characters.
        /// </summary>
        /// <value><c>True</c> if globalizing characters; otherwise, <c>False</c>.</value>
        public bool GlobalizeCharacters { get; set; }

        /// <summary>
        /// Transforms a format to a format that only allows the characters
        /// h, m, s, t, H and the defined TimeSeperators (: and .).
        /// </summary>
        /// <param name="format">The format that needs to be transformed.</param>
        /// <returns>
        /// A format containing only the expected characters.
        /// </returns>
        internal new string GetTransformedFormat(string format)
        {
            return base.GetTransformedFormat(format);
        }
    }
}
