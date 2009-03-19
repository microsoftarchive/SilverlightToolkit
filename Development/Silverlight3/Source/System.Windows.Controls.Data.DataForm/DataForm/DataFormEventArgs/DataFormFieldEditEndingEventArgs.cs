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
    /// Event args for the FieldEditEnding event.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataFormFieldEditEndingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Constructs a new instance of DataFormFieldEditEndingEventArgs.
        /// </summary>
        /// <param name="field">The field whose edit is ending.</param>
        /// <param name="editingElement">The element within the field.</param>
        /// <param name="editAction">The edit action.</param>
        public DataFormFieldEditEndingEventArgs(DataFormField field, FrameworkElement editingElement, DataFormEditAction editAction)
        {
            this.Field = field;
            this.EditingElement = editingElement;
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

        /// <summary>
        /// Gets the element within the field.
        /// </summary>
        public FrameworkElement EditingElement
        {
            get;
            private set;
        }
    }
}
