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
    /// The source of the error, for error management
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801", Justification = "ErrorType of None is not permitted.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008", Justification = "ErrorType of None is not permitted.")]
    public enum ErrorType
    {
        /// <summary>
        /// The error came from entity level validation
        /// </summary>
        EntityError = 1,

        /// <summary>
        /// The error came from the binding engine, which exposes only a single error at a time
        /// </summary>
        PropertyError = 2
    }
}
