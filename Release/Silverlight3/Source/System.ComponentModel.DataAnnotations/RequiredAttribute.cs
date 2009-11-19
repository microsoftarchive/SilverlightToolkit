// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel.DataAnnotations.Resources;
using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.DataAnnotations {
    /// <summary>
    /// Validation attribute to indicate that a property field or parameter is required.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "We want users to be able to extend this class")]
    public class RequiredAttribute : ValidationAttribute {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>This constructor selects a reasonable default error message for <see cref="ValidationAttribute.FormatErrorMessage"/></remarks>
        public RequiredAttribute()
            : base(() => DataAnnotationsResources.RequiredAttribute_ValidationError) {
        }

        /// <summary>
        /// Override of <see cref="ValidationAttribute.IsValid(object)"/>
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <returns><c>false</c> if the <paramref name="value"/> is null or an empty string</returns>
#if !SILVERLIGHT
        public
#else
        internal
#endif
        override bool IsValid(object value) {
            if (value == null) {
                return false;
            }

            var stringValue = value as string;
            if (stringValue != null) {
                return stringValue.Trim().Length != 0;
            } else {
                return true;
            }
        }
    }
}
