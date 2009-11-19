// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if !SILVERLIGHT
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.DataAnnotations {
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Validatable", Justification = "While not in the dictionary, this is spelled to match other APIs and frameworks.")]
    public interface IValidatableObject {
        IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
    }
}
#endif
