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
    /// Event args for the FormEditEnding event.
    /// </summary>
    public class DataFormFormEditEndingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Constructs a new instance of DataFormFormEditEndingEventArgs.
        /// </summary>
        /// <param name="editAction">The edit action.</param>
        public DataFormFormEditEndingEventArgs(DataFormEditAction editAction)
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
