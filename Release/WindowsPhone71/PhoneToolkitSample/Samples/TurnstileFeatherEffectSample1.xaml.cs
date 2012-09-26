// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using Microsoft.Phone.Controls;
using PhoneToolkitSample.Data;

namespace PhoneToolkitSample.Samples
{
    public partial class TurnstileFeatherEffectSample1 : PhoneApplicationPage
    {
        private bool _isDataLoaded;

        private MultilineItemCollection viewModel = new MultilineItemCollection();

        public TurnstileFeatherEffectSample1()
        {
            InitializeComponent();

            DataContext = viewModel;
            this.Loaded += MainPage_Loaded;
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isDataLoaded)
            {
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here one", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here two", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here three", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here four", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here five", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here six", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here seven", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here eight", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here nine", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here ten", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here eleven", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here twelve", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here thirteen", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here fourteen", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here fifteen", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here sixteen", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here seventeen", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here eighteen", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here nineteen", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here twenty", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here twenty-one", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here twenty-two", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here twenty-three", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here twenty-four", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here twenty-five", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here twenty-six", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here twenty-seven", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "tap here twenty-eight", LineTwo = "Navigate to the next page using transitions", LineThree = "This effect animates items individually" });
                _isDataLoaded = true;
            }
        }

        // Initiate a forward navigation.
        private void Item_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Samples/FeatheredTransitionsSample2.xaml", UriKind.Relative));
        }
    }
}