// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace PhoneToolkitSample
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void NavigateTo(string page)
        {
            this.NavigationService.Navigate(new Uri(page, UriKind.Relative));
        }

        private void OnToggleSwitch(object sender, RoutedEventArgs e)
        {
            NavigateTo("/Samples/ToggleSwitchSample.xaml");
        }

        private void OnTransitions(object sender, RoutedEventArgs e)
        {
            NavigateTo("/Samples/TransitionsSample.xaml");
        }

        private void OnTiltEffect(object sender, RoutedEventArgs e)
        {
            NavigateTo("/Samples/TiltEffectSample.xaml");
        }

        private void OnPerformanceProgressBar(object sender, RoutedEventArgs e)
        {
            NavigateTo("/Samples/PerformanceProgressBarSample.xaml");
        }

        private void OnContextMenu(object sender, RoutedEventArgs e)
        {
            NavigateTo("/Samples/ContextMenuSample.xaml");
        }

        private void OnDateTimePicker(object sender, RoutedEventArgs e)
        {
            NavigateTo("/Samples/DateTimePickerSample.xaml");
        }

        private void OnWrapPanel(object sender, RoutedEventArgs e)
        {
            NavigateTo("/Samples/WrapPanelSample.xaml");
        }

        private void OnGestures(object sender, RoutedEventArgs e)
        {
            NavigateTo("/Samples/GestureSample.xaml");
        }

        private void OnAutoCompleteBox(object sender, RoutedEventArgs e)
        {
            NavigateTo("/Samples/AutoCompleteBoxSample.xaml");
        }

        private void OnListPicker(object sender, RoutedEventArgs e)
        {
            NavigateTo("/Samples/ListPickerSample.xaml");
        }

        private void OnLongListSelector(object sender, RoutedEventArgs e)
        {
            NavigateTo("/Samples/LongListSelectorSample.xaml");
        }
    }
}