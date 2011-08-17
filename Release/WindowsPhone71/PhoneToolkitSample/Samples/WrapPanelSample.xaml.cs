// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;

namespace PhoneToolkitSample.Samples
{
    public partial class WrapPanelSample : PhoneApplicationPage
    {
        Random rnd = new Random();

        public WrapPanelSample()
        {
            InitializeComponent();
        }

        private void OnClear(object sender, RoutedEventArgs e)
        {
            wrapPanel.Children.Clear();
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            int count = Int32.Parse((string)((FrameworkElement)sender).Tag);

            while (count-- > 0)
            {
                AddItem();
            }
        }

        private void AddItem()
        {
            Border b = new Border() { 
                Width = 100, Height = 100, 
                Background = new SolidColorBrush(Color.FromArgb(255, (byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256))), 
                BorderThickness = new Thickness(2), Margin=new Thickness(8) };

            b.BorderBrush = (SolidColorBrush)Resources["PhoneForegroundBrush"];

            GestureListener listener = GestureService.GetGestureListener(b);
            listener.Tap += new EventHandler<GestureEventArgs>(WrapPanelSample_Tap);

            wrapPanel.Children.Add(b);
        }

        void WrapPanelSample_Tap(object sender, GestureEventArgs e)
        {
            Border b = (Border)sender;
            wrapPanel.Children.Remove(b);
        }

    }
}