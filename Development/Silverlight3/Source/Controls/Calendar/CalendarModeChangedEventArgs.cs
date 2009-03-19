// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides data for the DisplayModeChanged event.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public class CalendarModeChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets the previous mode of the Calendar.
        /// </summary>
        public CalendarMode OldMode { get; private set; }

        /// <summary>
        /// Gets the new mode of the Calendar.
        /// </summary>
        public CalendarMode NewMode { get; private set; }

        /// <summary>
        /// Initializes a new instance of the CalendarModeChangedEventArgs
        /// class.
        /// </summary>
        /// <param name="oldMode">The previous mode.</param>
        /// <param name="newMode">The new mode.</param>
        public CalendarModeChangedEventArgs(CalendarMode oldMode, CalendarMode newMode)
        {
            OldMode = oldMode;
            NewMode = newMode;
        }
    }
}