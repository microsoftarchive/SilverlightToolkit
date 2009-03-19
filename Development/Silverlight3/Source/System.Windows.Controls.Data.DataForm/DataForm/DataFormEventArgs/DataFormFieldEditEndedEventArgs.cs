//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;

namespace System.Windows.Controls
{
    /// <summary>
    /// Event args for the FieldEditEnded event.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataFormFieldEditEndedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new instance of DataFormFieldEditEndedEventArgs.
        /// </summary>
        /// <param name="field">The field whose edit is ending.</param>
        /// <param name="editAction">The edit action.</param>
        public DataFormFieldEditEndedEventArgs(DataFormField field, DataFormEditAction editAction)
        {
            this.Field = field;
            this.EditAction = editAction;
        }

        /// <summary>
        /// Gets the field to be used.
        /// </summary>
        public DataFormField Field
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the edit action.
        /// </summary>
        public DataFormEditAction EditAction
        {
            get;
            private set;
        }
    }
}
