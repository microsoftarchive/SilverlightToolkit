// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Provides access to the localized resources used by the DatePicker and TimePicker.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "Making this class static breaks its use as a resource in generic.xaml.")]
    public class DateTimePickerResources
    {
        /// <summary>
        /// Gets the localized DatePicker title string.
        /// </summary>
        public static string DatePickerTitle { get { return Properties.Resources.DatePickerTitle; } }

        /// <summary>
        /// Gets the localized TimePicker title string.
        /// </summary>
        public static string TimePickerTitle { get { return Properties.Resources.TimePickerTitle; } }
    }
}
