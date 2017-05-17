// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls
{
    /// <summary>
    /// Determines the granularity of time selection
    /// by a popup. Hours and minutes are always used.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum PopupTimeSelectionMode
    {
        // todo: the enum values that are commented out will be included after mix.

        // /// <summary>
        // /// AM/PM, Hours, Minutes and Seconds.
        // /// </summary>
        // AllowSecondsAndDesignatorsSelection,

        // /// <summary>
        // /// AM/PM, Hours and Minutes.
        // /// </summary>
        // AllowTimeDesignatorsSelection,
        
        /// <summary>
        /// Hours, Minutes and Seconds.
        /// </summary>
        AllowSecondsSelection,
        
        /// <summary>
        /// Hours and Minutes.
        /// </summary>
        HoursAndMinutesOnly
    }
}
