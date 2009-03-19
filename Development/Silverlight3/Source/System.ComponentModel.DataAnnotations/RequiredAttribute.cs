using System.ComponentModel.DataAnnotations.Resources;
using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.DataAnnotations {
    /// <summary>
    /// Validation attribute to indicate that a property field or parameter is required.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "We want it to be accessible via method on parent.")]
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
        public override bool IsValid(object value) {
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
