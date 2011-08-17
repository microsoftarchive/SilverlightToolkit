// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace PhoneToolkitSample.Samples
{
    public partial class LockablePivotSample : PhoneApplicationPage
    {
        public LockablePivotSample()
        {
            InitializeComponent();

            InjectSampleData();
        }

        private void InjectSampleData()
        {
            textBlock1.Text =
                "Maecenas praesent accumsan bibendum " +
                "Dictumst eleifend facilisi faucibus " +
                "Habitant inceptos interdum lobortis " +
                "Nascetur pharetra placerat pulvinar " +
                "Maecenas praesent accumsan bibendum " +
                "Dictumst eleifend facilisi faucibus " +
                "Habitant inceptos interdum lobortis " +
                "Nascetur pharetra placerat pulvinar " +
                "Maecenas praesent accumsan bibendum " +
                "Dictumst eleifend facilisi faucibus " +
                "Habitant inceptos interdum lobortis " +
                "Nascetur pharetra placerat pulvinar " +
                "Maecenas praesent accumsan bibendum " +
                "Dictumst eleifend facilisi faucibus " +
                "Habitant inceptos interdum lobortis " +
                "Nascetur pharetra placerat pulvinar";
        }

        private void lockButton_Click(object sender, RoutedEventArgs e)
        {
            pivot.IsLocked = !pivot.IsLocked;
            lockButton.Content = (pivot.IsLocked ? "Unlock" : "Lock");

            Slider[] sliders = {
                slider1,
                slider2,
                slider3,
                slider4,
                slider5,
                slider6
                };

            foreach (Slider s in sliders)
            {
                s.IsEnabled = pivot.IsLocked;
            }
        }
    }
}