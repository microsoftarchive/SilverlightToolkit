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
    /// <QualityBand>Preview</QualityBand>
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
                    string lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

                    if (lang == "ar" || lang == "fa")
                    {
                        // For arabic and persian, we want the am/pm designator to be displayed at the left.
                        pattern = "\u200F" + pattern;
                    }
                    else
                    {
                        // For LTR languages and Hebrew, we want the am/pm designator to be displayed at the right.
                        pattern = "\u200E" + pattern;
                    }

                    _fallbackValueStringFormat = "{0:" + pattern + "}";
                }
                return _fallbackValueStringFormat;
            }
        }
    }
}
