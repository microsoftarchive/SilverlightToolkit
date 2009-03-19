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
    public class EditableTestClass : IEditableObject
    {
        /// <summary>
        /// Structure used to store the curren fields and the backed-up
        /// ones in case the object is edited.
        /// </summary>
        private struct EditableTestClassData
        {
            internal int IntProperty;
            internal string StringProperty;
        }

        private EditableTestClassData _backupData, _data;
        private bool _editing;

        /// <summary>
        /// Default constructor
        /// </summary>
        public EditableTestClass()
        {
            this._data = new EditableTestClassData();
        }

        /// <summary>
        /// Gets or sets an integer property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int", Justification = "This is a property used only within these specific unit tests and will never be seen outside of C#.")]
        public int IntProperty 
        {
            get
            {
                return this._data.IntProperty;
            }

            set
            {
                if (this._data.IntProperty != value)
                {
                    this._data.IntProperty = value;
                }
            }
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

        /// <summary>
        /// Gets or sets a string property.
        /// </summary>
        public string StringProperty
        {
            get
            {
                return this._data.StringProperty;
            }

            set
            {
                if (this._data.StringProperty != value)
                {
                    this._data.StringProperty = value;
                }
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
                this._backupData = this._data;
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
                this._data = this._backupData;
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
                this._backupData = new EditableTestClassData();
                this._editing = false;
                this.DebugString = "EndEdit";
            }
        }

        #endregion IEditableObject Members
    }
}
