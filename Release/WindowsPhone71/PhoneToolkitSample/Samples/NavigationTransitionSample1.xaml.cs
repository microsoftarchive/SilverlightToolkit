// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace PhoneToolkitSample.Samples
{
    public partial class NavigationTransitionSample1 : PhoneApplicationPage
    {
        public NavigationTransitionSample1()
        {
            InitializeComponent();
        }

        private void Forward(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Samples/NavigationTransitionSample2.xaml", UriKind.Relative));
        }
    }
}