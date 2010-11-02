using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace PhoneToolkitSample.Samples
{
    public partial class NavigationTransitionSample2 : PhoneApplicationPage
    {
        public NavigationTransitionSample2()
        {
            InitializeComponent();
        }

        private void Forward(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Samples/NavigationTransitionSample1.xaml", UriKind.Relative));
        }
    }
}