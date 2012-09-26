// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace PhoneToolkitSample.Samples
{
    public partial class FeatheredTransitionsSample1 : PhoneApplicationPage
    {
        public FeatheredTransitionsSample1()
        {
            InitializeComponent();
        }

        private void BasicTurnstileFeathering_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Samples/TurnstileFeatherEffectSample1.xaml", UriKind.Relative));
        }

        private void TurnstileFeatheringWithPivot_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Samples/TurnstileFeatherEffectSample2.xaml", UriKind.Relative));
        }        
    }
}