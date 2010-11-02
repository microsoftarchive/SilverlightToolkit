// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Phone.Controls;

namespace PhoneToolkitSample.Samples
{
    public partial class ListPickerSample : PhoneApplicationPage
    {
        static readonly string[] AccentColors = {"magenta", "purple", "teal", "lime", "brown", "pink", "orange", "blue", "red", "green"};

        public ListPickerSample()
        {
            InitializeComponent();

            DataContext = AccentColors;
        }
    }
}
