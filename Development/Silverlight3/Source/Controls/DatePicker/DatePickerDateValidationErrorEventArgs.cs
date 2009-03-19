// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides data for the DateValidationError event.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public class DatePickerDateValidationErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private bool _throwException;

        /// <summary>
        /// Initializes a new instance of the
        /// DatePickerDateValidationErrorEventArgs class. 
        /// </summary>
        /// <param name="exception">
        /// The exception that initially triggered the DateValidationError
        /// event.
        /// </param>
        /// <param name="text">The text being parsed.</param>
        public DatePickerDateValidationErrorEventArgs(Exception exception, string text)
        {
            this.Text = text;
            this.Exception = exception;
        }

        /// <summary>
        /// Gets the exception that initially triggered the DateValidationError
        /// event.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets the text being parsed when the DateValidationError event was
        /// raised.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether an Exception should be
        /// thrown.
        /// </summary>
        public bool ThrowException
        {
            get { return this._throwException; }
            set
            {
                if (value && this.Exception == null)
                {
                    throw new ArgumentException(System.Windows.Controls.Properties.Resources.DatePickerDateValidationErrorEventArgs_ThrowException_NoException);
                }
                this._throwException = value;
            }
        }
    }
}