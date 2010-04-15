// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.ComponentModel.DataAnnotations {
    /// <summary>
    /// Enumeration of logical data types that may appear in <see cref="DataTypeAttribute"/>
    /// </summary>
    public enum DataType {
        /// <summary>
        /// Custom data type, not one of the static data types we know
        /// </summary>
        Custom,

        /// <summary>
        /// DateTime data type
        /// </summary>
        DateTime,

        /// <summary>
        /// Date data type
        /// </summary>
        Date,

        /// <summary>
        /// Time data type
        /// </summary>
        Time,

        /// <summary>
        /// Duration data type
        /// </summary>
        Duration,

        /// <summary>
        /// Phone number data type
        /// </summary>
        PhoneNumber,

        /// <summary>
        /// Currency data type
        /// </summary>
        Currency,

        /// <summary>
        /// Plain text data type
        /// </summary>
        Text,

        /// <summary>
        /// Html data type
        /// </summary>
        Html,

        /// <summary>
        /// Multiline text data type
        /// </summary>
        MultilineText,

        /// <summary>
        /// Email address data type
        /// </summary>
        EmailAddress,

        /// <summary>
        /// Password data type -- do not echo in UI
        /// </summary>
        Password,

        /// <summary>
        /// URL data type
        /// </summary>
        Url,

        /// <summary>
        /// URL to an Image -- to be displayed as an image instead of text
        /// </summary>
        ImageUrl
    }
}
