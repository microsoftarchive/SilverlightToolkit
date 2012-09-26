using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace PhoneToolkitSample.Samples
{
    public partial class RatingControlSample : PhoneApplicationPage
    {
        public RatingControlSample()
        {
            InitializeComponent();
        }

        private void Button_Click_Increase(object sender, RoutedEventArgs e)
        {
            RatingControl.Value += 0.5;
        }

        private void Button_Click_Decrease(object sender, RoutedEventArgs e)
        {
            RatingControl.Value -= 0.10;
        }

        private void Button_Click_Reset(object sender, RoutedEventArgs e)
        {
            RatingControl.Value = 0;
            RatingControl.RatingItemCount = 5;
            RatingControl.Orientation = System.Windows.Controls.Orientation.Horizontal;
            RatingControl.Width = 250;
            RatingControl.Height = 50;
        }

        private void Button_Click_Flip(object sender, RoutedEventArgs e)
        {
            if (RatingControl.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                RatingControl.Orientation = System.Windows.Controls.Orientation.Vertical;
                RatingControl.Width = 50;
                RatingControl.Height = 250;
            }
            else
            {
                RatingControl.Orientation = System.Windows.Controls.Orientation.Horizontal;
                RatingControl.Width = 250;
                RatingControl.Height = 50;
            }
        }

        private void Button_Click_More(object sender, RoutedEventArgs e)
        {
            RatingControl.RatingItemCount += 1;
        }
    }
}