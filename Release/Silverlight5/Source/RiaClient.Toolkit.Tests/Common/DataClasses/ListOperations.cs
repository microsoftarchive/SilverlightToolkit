//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace System.Windows.Controls.UnitTests
{
    /// <summary>
    /// Enumeration of the types of operations permissable on a list.
    /// </summary>
    [Flags]
    public enum ListOperations
    {
        /// <summary>
        /// Only Selection is permitted, no update operations are allowed.
        /// </summary>
        None = 0,

        /// <summary>
        /// New entities may be added
        /// </summary>
        Add = 1,

        /// <summary>
        /// Entities may be updated
        /// </summary>
        Edit = 2,

        /// <summary>
        /// Entities may be removed
        /// </summary>
        Remove = 4,

        /// <summary>
        /// Entities may be added, updated and removed
        /// </summary>
        All = Add | Edit | Remove
    }
}
