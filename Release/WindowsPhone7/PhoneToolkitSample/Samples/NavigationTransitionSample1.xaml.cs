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