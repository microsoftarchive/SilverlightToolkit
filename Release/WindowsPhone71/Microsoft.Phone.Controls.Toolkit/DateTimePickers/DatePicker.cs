// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using System.Windows;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Represents a control that allows the user to choose a date (day/month/year).
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DatePicker : DateTimePickerBase
    {
        private string _fallbackValueStringFormat;

        /// <summary>
        /// Initializes a new instance of the DatePicker control.
        /// </summary>
        public DatePicker()
        {
            DefaultStyleKey = typeof(DatePicker);
            Value = DateTime.Now.Date;
        }

        /// <summary>
        /// Gets the fallback value for the ValueStringFormat property.
        /// </summary>
        protected override string ValueStringFormatFallback
        {
            get
            {
                if (_fallbackValueStringFormat == null)
                {
                    string pattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

                    if (DateShouldFlowRTL())
                    {
                        char[] reversedPattern = pattern.ToCharArray();
                        Array.Reverse(reversedPattern);
                        pattern = new string(reversedPattern);
                    }

                    _fallbackValueStringFormat = "{0:" + pattern + "}";
                }

                return _fallbackValueStringFormat;
            }
        }
        
    }
}
