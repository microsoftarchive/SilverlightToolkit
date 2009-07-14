//-----------------------------------------------------------------------
// <copyright file="EditableTestClass.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.ComponentModel.UnitTests
{
    /// <summary>
    /// Test class that implements IEditableObject
    /// </summary>
    public class EditableTestClass : TestClass, IEditableObject
    {
        private int _backupInt;
        private string _backupString;
        private bool _editing;

        /// <summary>
        /// Default constructor
        /// </summary>
        public EditableTestClass()
        {
            _backupInt = 0;
            _backupString = null;
        }

        /// <summary>
        /// Gets a value indicating whether this object is in editing mode or not.
        /// </summary>
        public bool IsEditing
        {
            get
            {
                return this._editing;
            }
        }

        public string DebugString { get; set; }

        #region IEditableObject Members

        /// <summary>
        /// Begins editing mode
        /// </summary>
        public void BeginEdit()
        {
            if (!this._editing)
            {
                this._backupInt = this.IntProperty;
                this._backupString = this.StringProperty;
                this._editing = true;
                this.DebugString = "BeginEdit";
            }
        }

        /// <summary>
        /// Cancels previous edits
        /// </summary>
        public void CancelEdit()
        {
            if (this._editing)
            {
                this.IntProperty = this._backupInt;
                this.StringProperty = this._backupString;
                this._editing = false;
                this.DebugString = "CancelEdit";
            }
        }

        /// <summary>
        /// Commits the edits
        /// </summary>
        public void EndEdit()
        {
            if (this._editing)
            {
                _backupInt = 0;
                _backupString = null;
                this._editing = false;
                this.DebugString = "EndEdit";
            }
        }

        #endregion IEditableObject Members
    }
}
