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
    /// <summary>
    /// Enumeration denoting the state of the edit controls' visibility.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames", Justification = "ShowEditControls is not plural.")]
    public enum ShowEditControls
    {
        /// <summary>
        /// Represents the case where the edit controls should always be shown.
        /// </summary>
        True,

        /// <summary>
        /// Represents the case where the edit controls should never be shown.
        /// </summary>
        False,

        /// <summary>
        /// Represents the case where the edit controls should be shown where appropriate.
        /// </summary>
        Automatic
    }
}
