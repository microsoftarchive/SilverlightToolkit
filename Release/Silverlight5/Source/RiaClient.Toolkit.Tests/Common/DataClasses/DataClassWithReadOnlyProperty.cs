//-----------------------------------------------------------------------
// <copyright company="Microsoft" file="DataClass.cs">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// A data class for testing the <see cref="DataForm"/>.
    /// </summary>
    public class DataClassWithReadOnlyProperty
    {
        /// <summary>
        /// Private accessor to ReadOnlyStringProperty.
        /// </summary>
        private string readOnlyStringProperty;

        public DataClassWithReadOnlyProperty()
        {
            this.readOnlyStringProperty = "test string";
        }

        /// <summary>
        /// Gets or sets a string.
        /// </summary>
        [Display(AutoGenerateField = true, Description = "Read-Only String Property Description", Name = "Read-Only String Property")]
        public string ReadOnlyStringProperty
        {
            get
            {
                return this.readOnlyStringProperty;
            }
        }
    }
}
