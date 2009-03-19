using System.Collections.Generic;

namespace System.ComponentModel.DataAnnotations {
    /// <summary>
    /// Container class for the results of a validation request.  
    /// <remarks>
    /// See <see cref="ValidationAttribute.TryValidate"/>
    /// </remarks>
    /// </summary>
    public class ValidationResult {
        private IEnumerable<string> _memberNames;
        private string _errorMessage;

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="errorMessage">The user-visible error message.  If null, <see cref="ValidationAttribute.TryValidate"/> 
        /// will use <see cref="ValidationAttribute.FormatErrorMessage"/> for its error message.</param>
        /// <param name="memberNames">The list of member names affected by by this result.  
        /// If empty, <see cref="ValidationAttribute.TryValidate"/>
        /// will construct this list from its <see cref="ValidationContext.MemberName"/>.  
        /// This list of member names is meant to be used by presentation layers to indicate which fields are in error.</param>
        public ValidationResult(string errorMessage, IEnumerable<string> memberNames) {
            this._errorMessage = errorMessage;
            this._memberNames = memberNames ?? new string[0];
        }

        /// <summary>
        /// Constructor that accepts an error message
        /// </summary>
        /// <param name="errorMessage">The user-visible error message.  If null, <see cref="ValidationAttribute.TryValidate"/> will use
        /// <see cref="ValidationAttribute.FormatErrorMessage"/> for its error message.</param>
        public ValidationResult(string errorMessage)
            : this(errorMessage, null) {
        }

        /// <summary>
        /// Gets the collection of member names affected by this result.  The collection may be empty but will never be null.
        /// </summary>
        public IEnumerable<string> MemberNames {
            get {
                return this._memberNames;
            }
        }

        /// <summary>
        /// Gets the error message for this result.  It may be null.
        /// </summary>
        public string ErrorMessage {
            get {
                return this._errorMessage;
            }
            set {
                this._errorMessage = value;
            }
        }
    }
}
