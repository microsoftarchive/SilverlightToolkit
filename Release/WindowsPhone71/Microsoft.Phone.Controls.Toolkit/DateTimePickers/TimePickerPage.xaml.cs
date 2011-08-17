// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using Microsoft.Phone.Controls.Primitives;

namespace Microsoft.Phone.Controls
{
    /// <summary>
    /// Represents a page used by the TimePicker control that allows the user to choose a time (hour/minute/am/pm).
    /// </summary>
    public partial class TimePickerPage : DateTimePickerPageBase
    {
        /// <summary>
        /// Initializes a new instance of the TimePickerPage control.
        /// </summary>
        public TimePickerPage()
        {
            InitializeComponent();

            // Hook up the data sources
            PrimarySelector.DataSource = DateTimeWrapper.CurrentCultureUsesTwentyFourHourClock() ?
                 (DataSource)(new TwentyFourHourDataSource()) :
                 (DataSource)(new TwelveHourDataSource());
            SecondarySelector.DataSource = new MinuteDataSource();
            TertiarySelector.DataSource = new AmPmDataSource();

            InitializeDateTimePickerPage(PrimarySelector, SecondarySelector, TertiarySelector);
        }

        /// <summary>
        /// Gets a sequence of LoopingSelector parts ordered according to culture string for date/time formatting.
        /// </summary>
        /// <returns>LoopingSelectors ordered by culture-specific priority.</returns>
        protected override IEnumerable<LoopingSelector> GetSelectorsOrderedByCulturePattern()
        {
            return GetSelectorsOrderedByCulturePattern(
                CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern.ToUpperInvariant(),
                new char[] { 'H', 'M', 'T' },
                new LoopingSelector[] { PrimarySelector, SecondarySelector, TertiarySelector });
        }

        /// <summary>
        /// Handles changes to the page's Orientation property.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (null == e)
            {
                throw new ArgumentNullException("e");
            }

            base.OnOrientationChanged(e);
            SystemTrayPlaceholder.Visibility = (0 != (PageOrientation.Portrait & e.Orientation)) ?
                Visibility.Visible :
                Visibility.Collapsed;
        }
    }
}
