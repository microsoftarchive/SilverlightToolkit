// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using Microsoft.Phone.Controls;
using PhoneToolkitSample.Data;

namespace PhoneToolkitSample.Samples
{
    public partial class ListPickerSample : PhoneApplicationPage
    {
        public ListPickerSample()
        {
            InitializeComponent();

            DataContext = ColorExtensions.AccentColors();

            PrintInColors.SummaryForSelectedItemsDelegate = Summarize;
        }

        private string Summarize(IList items)
        {
            string str = "";
            if (null != items)
            {
                if (items.Contains("Cyan"))
                {
                    str += "C";
                }
                if (items.Contains("Majenta"))
                {
                    str += "M";
                }
                if (items.Contains("Yellow"))
                {
                    str += "Y";
                }
                if (items.Contains("Black"))
                {
                    str += "K";
                }
            }

            return str;
        }
    }
}
