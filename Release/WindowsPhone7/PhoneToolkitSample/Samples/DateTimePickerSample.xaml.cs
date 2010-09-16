// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Phone.Controls;

namespace PhoneToolkitSample.Samples
{
    // To see the correct ApplicationBar icons in the DatePicker and TimePicker, you will need 
    // to create a folder in the root of your project called "Toolkit.Content" and put the icons 
    // in there. You can copy them from this project. They must be named "ApplicationBar.Cancel.png" 
    // and "ApplicationBar.Check.png", and the build action must be "Content".

    public partial class DateTimePickerSample : PhoneApplicationPage
    {
        public DateTimePickerSample()
        {
            InitializeComponent();
        }

        private void DatePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {

        }

        private void TimePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {

        }
    }
}