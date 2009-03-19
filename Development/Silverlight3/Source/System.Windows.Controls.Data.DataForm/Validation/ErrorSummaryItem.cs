//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace System.Windows.Controls
{
    /// <summary>
    /// ErrorSummaryItem encapsulates validation errors.  It keeps track of the error itself, what control it's associated with, and the associated field.
    /// This class is immutable.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class ErrorSummaryItem
    {
        /// <summary>
        /// Initializes a new instance of the ErrorSummaryItem class.
        /// </summary>
        /// <param name="errorMessage">error message text</param>
        /// <param name="errorType">Whether the error originated from an entity or property</param>
        public ErrorSummaryItem(string errorMessage, ErrorType errorType) : this(errorMessage, errorType, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ErrorInfo class.  This overload can designate the ErrorSource.
        /// </summary>
        /// <param name="errorMessage">error message text</param>
        /// <param name="errorType">Whether the error originated from an entity or property</param>
        /// <param name="context">Context from which the error occurred.  This general property can be used as a container to keep track of the entity, for instance.</param>
        /// <param name="control">input control resulting in this error</param>
        /// <param name="propertyName">The name of the property associated with the error.</param>
        public ErrorSummaryItem(string errorMessage, ErrorType errorType, object context, FrameworkElement control, string propertyName)
        {
            this.ErrorMessage = errorMessage;
            this.ErrorType = errorType;
            this.Context = context;
            this.Control = control;
            this.PropertyName = propertyName;
        }

        /// <summary>
        /// Gets the error message text
        /// </summary>
        public string ErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ErrorSource, which is the method in which this error was discovered.  Only internal classes can set this field as
        /// anything but Custom.
        /// </summary>
        public ErrorType ErrorType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the field member which resulted in this error
        /// </summary>
        public object Context
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the reference to the input control that resulted in this error
        /// </summary>
        public string PropertyName
        {
            get;
            private set;
        }

        /// <summary>
        /// Control reference for the error.
        /// </summary>
        public FrameworkElement Control
        {
            get;
            private set;
        }
    }
}
