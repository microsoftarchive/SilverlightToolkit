//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents all of the errors associated with a given input control
    /// </summary>
    internal class ValidationErrorCollection : ObservableCollection<ErrorSummaryItem>
    {
        /// <summary>
        /// Find an error with the given error message
        /// </summary>
        /// <param name="errorType">The source of the error</param>
        /// <param name="message">The message string to search for</param>
        /// <param name="control">The reference to the control associated with the error.</param>
        /// <returns>The found ErrorInfo</returns>
        internal ErrorSummaryItem FindError(ErrorType errorType, string message, FrameworkElement control)
        {
            foreach (ErrorSummaryItem esi in this)
            {
                if (esi.ErrorType == errorType && esi.ErrorMessage == message && esi.Control == control)
                {
                    return esi;
                }
            }
            return null;
        }

        /// <summary>
        /// Clears errors of the given source type
        /// </summary>
        /// <param name="errorType">The type of the error (Entity or Property)</param>
        internal void ClearErrors(ErrorType errorType)
        {
            // Clear entity errors
            ValidationErrorCollection errorsToRemove = new ValidationErrorCollection();
            foreach (ErrorSummaryItem error in this)
            {
                if (error != null && error.ErrorType == errorType)
                {
                    errorsToRemove.Add(error);
                }
            }
            foreach (ErrorSummaryItem error in errorsToRemove)
            {
                this.Remove(error);
            }
        }

        /// <summary>
        /// Clears all the ErrorInfo from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            // Manually removes each item at a time so that the "OldItems" parameter is filled
            while (this.Count > 0)
            {
                this.RemoveAt(0);
            }
        }
    }
}
