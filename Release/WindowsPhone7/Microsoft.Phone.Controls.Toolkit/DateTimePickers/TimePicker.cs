// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Represents a control that allows the user to choose a time (hour/minute/am/pm).
    /// </summary>
    public class TimePicker : DateTimePickerBase
    {
        private string _fallbackValueStringFormat;

        /// <summary>
        /// Initializes a new instance of the TimePicker control.
        /// </summary>
        public TimePicker()
        {
            DefaultStyleKey = typeof(TimePicker);
            Value = DateTime.Now;
        }

        /// <summary>
        /// Gets the fallback value for the ValueStringFormat property.
        /// </summary>
        protected override string ValueStringFormatFallback
        {
            get
            {
                if (null == _fallbackValueStringFormat)
                {
                    // Need to convert LongTimePattern into ShortTimePattern to work around a platform bug
                    // such that only LongTimePattern respects the "24-hour clock" override setting.
                    // This technique is not perfect, but works for all the initially-supported languages.
                    string pattern = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern.Replace(":ss", "");
                    _fallbackValueStringFormat = "{0:" + pattern + "}";
                }
                return _fallbackValueStringFormat;
            }
        }
    }
}
