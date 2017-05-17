//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataClassWithValidationValidator.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// Provides custom validation for the <see cref="DataClassWithValidation"/>.
    /// </summary>
    public static class DataClassWithValidationValidator
    {
        /// <summary>
        /// Returns whether or not the <see cref="DataClassWithValidation"/> is valid.
        /// </summary>
        /// <param name="value">The <see cref="DataClassWithValidation"/>.</param>
        /// <param name="context">The <see cref="ValidationContext"/>.</param>
        /// <param name="result">The <see cref="ValidationResult"/>.</param>
        /// <returns>Whether or not the <see cref="DataClassWithValidation"/> is valid.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "The method signature must be as it is right now to work.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "context", Justification = "The method signature must be as it is right now to work.")]
        public static ValidationResult IsDataClassWithValidationValid(object value, ValidationContext context)
        {
            DataClassWithValidation dataClassWithValidation = value as DataClassWithValidation;

            if (dataClassWithValidation == null)
            {
                return new ValidationResult(null);
            }

            if (dataClassWithValidation.IntProperty.ToString(CultureInfo.InvariantCulture) == dataClassWithValidation.StringProperty)
            {
                return new ValidationResult("IntProperty cannot be equal to StringProperty.", new string[] { "IntProperty", "StringProperty" });
            }

            return ValidationResult.Success;
        }
    }
}
