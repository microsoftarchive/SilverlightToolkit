// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Overrides RangeTimePicker for easy testing.
    /// </summary>
    public class OverriddenRangeTimePickerPopup : RangeTimePickerPopup
    {
        /// <summary>
        /// Gets the hours slider.
        /// </summary>
        /// <returns></returns>
        public RangeBase OverriddenHoursSlider
        {
            get { return ((Panel) VisualTreeHelper.GetChild(this, 0)).FindName("HoursSlider") as RangeBase; }
        }

        /// <summary>
        /// Gets the minutes slider.
        /// </summary>
        /// <returns></returns>
        public RangeBase OverriddenMinutesSlider
        {
            get { return ((Panel) VisualTreeHelper.GetChild(this, 0)).FindName("MinutesSlider") as RangeBase; }
        }

        /// <summary>
        /// Gets the seconds slider.
        /// </summary>
        /// <returns></returns>
        public RangeBase OverriddenSecondsSlider
        {
            get { return ((Panel) VisualTreeHelper.GetChild(this, 0)).FindName("SecondsSlider") as RangeBase; }
        }

        /// <summary>
        /// Gets the hours labels.
        /// </summary>
        /// <returns></returns>
        public Panel OverriddenHourContainer
        {
            get { return ((Panel)VisualTreeHelper.GetChild(this, 0)).FindName("HoursPanel") as Panel; }
        }

        /// <summary>
        /// Gets the minutes labels.
        /// </summary>
        /// <returns></returns>
        public Panel OverriddenMinuteContainer
        {
            get { return ((Panel)VisualTreeHelper.GetChild(this, 0)).FindName("MinutesPanel") as Panel; }
        }

        /// <summary>
        /// Gets the seconds labels.
        /// </summary>
        /// <returns></returns>
        public Panel OverriddenSecondContainer
        {
            get { return ((Panel)VisualTreeHelper.GetChild(this, 0)).FindName("SecondsPanel") as Panel; }
        }
    }
}