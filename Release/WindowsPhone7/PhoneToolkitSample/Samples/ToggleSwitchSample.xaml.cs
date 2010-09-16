// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using Microsoft.Phone.Controls;

namespace PhoneToolkitSample.Samples
{
    public partial class ToggleSwitchSample : PhoneApplicationPage
    {
        public ToggleSwitchSample()
        {
            InitializeComponent();
        }

        private void OnAlarmTap(object sender, GestureEventArgs e)
        {
            Debug.WriteLine("Alarm tapped!");
        }
    }
}