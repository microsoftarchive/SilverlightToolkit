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
    using System.ComponentModel;

    /// <summary>
    /// Event args for the FormEditEnded event.
    /// </summary>
    public class DataFormFormEditEndedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new instance of DataFormFieldEditEndedEventArgs.
        /// </summary>
        /// <param name="editAction">The edit action.</param>
        public DataFormFormEditEndedEventArgs(DataFormEditAction editAction)
        {
            this.EditAction = editAction;
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
