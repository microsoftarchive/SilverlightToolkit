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
    /// An enum to specify the error filtering level.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [Flags]
    public enum ErrorSummaryFilters
    {
        /// <summary>
        /// None of the errors are displayed in the ErrorSummary
        /// </summary>
        None = 0,
        
        /// <summary>
        /// ErrorSummary only displays the Entity level errors
        /// </summary>
        EntityErrors = 1,

        /// <summary>
        /// ErrorSummary only displays the property level errors
        /// </summary>
        PropertyErrors = 2,

        /// <summary>
        /// ErrorSummary displays all errors
        /// </summary>
        All = EntityErrors | PropertyErrors
    }
}
