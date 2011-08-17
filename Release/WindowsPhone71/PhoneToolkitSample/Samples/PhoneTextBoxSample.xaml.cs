// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace PhoneToolkitSample.Samples
{
    public partial class PhoneTextBoxSample : PhoneApplicationPage
    {
        public PhoneTextBoxSample()
        {
            InitializeComponent();
        }

        private void Search_ActionIconTapped(object sender, EventArgs e)
        {
            MessageBox.Show("The action icon was tapped on the search field.");
        }
    }
}