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
    public partial class TurnstileFeatherEffectSample2 : PhoneApplicationPage
    {
        private bool _isDataLoaded;

        private MultilineItemCollection viewModel = new MultilineItemCollection();

        public TurnstileFeatherEffectSample2()
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
                viewModel.Items.Add(new MultilineItem() { LineOne = "One: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Two: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Three: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Four: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Five: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Six: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Seven: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Eight: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Nine: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Ten: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Eleven: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Twelve: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Thirteen: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Fourteen: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Fifteen: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Sixteen: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Seventeen: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Eighteen: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Nineteen: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
                viewModel.Items.Add(new MultilineItem() { LineOne = "Twenty: Tap here to navigate to the next page", LineTwo = "This effect animates items individually" });
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