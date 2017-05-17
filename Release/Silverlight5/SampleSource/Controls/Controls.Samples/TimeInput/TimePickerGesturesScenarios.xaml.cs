// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page that demonstrates different
    /// interactions that are possible.
    /// </summary>
    [Sample("TimePicker Gestures", DifficultyLevel.Scenario, "TimePicker")]
    public partial class TimePickerGesturesScenarios : UserControl
    {
        /// <summary>
        /// Closing the dropdown will move focus to the ToggleButton, which 
        /// will raise the 'GotFocus' event on the TimePicker, resulting in a
        /// loop. We will ignore the next Focus event after closing the dropdown
        /// to prevent this.
        /// </summary>
        private bool _ignoreNextGain;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimePickerGesturesScenarios"/> class.
        /// </summary>
        public TimePickerGesturesScenarios()
        {
            InitializeComponent();

            tp.GotFocus += GainedFocus;
        }

        #region Focus
        /// <summary>
        /// Called when the picker gains focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Set from Xaml.")]
        private void GainedFocus(object sender, RoutedEventArgs e)
        {
            if (!_ignoreNextGain)
            {
                tp.DropDownClosed += DropDownClosed;
                tp.IsDropDownOpen = true;
            }
            else
            {
                _ignoreNextGain = false;
            }
        }

        /// <summary>
        /// Handles the DropDownClosed event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            tp.DropDownClosed -= DropDownClosed;

            // closing dropdown will move focus to button.
            _ignoreNextGain = true;
        }
        #endregion Focus

        #region Hover
        /// <summary>
        /// Called when the mouse enters the Picker.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called by Xaml.")]
        private void PickerMouseEnter(object sender, MouseEventArgs e)
        {
            tp2.IsDropDownOpen = true;
        }
        #endregion Hover
    }
}
