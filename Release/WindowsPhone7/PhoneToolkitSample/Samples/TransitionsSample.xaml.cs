using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Microsoft.Phone.Controls;

namespace PhoneToolkitSample.Samples
{
    public partial class TransitionsSample : PhoneApplicationPage
    {
        public TransitionsSample()
        {
            DataContext = this;
            InitializeComponent();
        }

        public IList<string> Families
        {
            get
            {
                return new List<string>
                {
                    "Roll",
                    "Rotate",
                    "Slide",
                    "Swivel",
                    "Turnstile"
                };
            }
        }

        private RotateTransition RotateTransitionElement(string mode)
        {
            RotateTransitionMode rotateTransitionMode = (RotateTransitionMode)Enum.Parse(typeof(RotateTransitionMode), mode, false);
            return new RotateTransition { Mode = rotateTransitionMode };
        }

        private SlideTransition SlideTransitionElement(string mode)
        {
            SlideTransitionMode slideTransitionMode = (SlideTransitionMode)Enum.Parse(typeof(SlideTransitionMode), mode, false);
            return new SlideTransition { Mode = slideTransitionMode };
        }

        private SwivelTransition SwivelTransitionElement(string mode)
        {
            SwivelTransitionMode swivelTransitionMode = (SwivelTransitionMode)Enum.Parse(typeof(SwivelTransitionMode), mode, false);
            return new SwivelTransition { Mode = swivelTransitionMode };
        }

        private TurnstileTransition TurnstileTransitionElement(string mode)
        {
            TurnstileTransitionMode turnstileTransitionMode = (TurnstileTransitionMode)Enum.Parse(typeof(TurnstileTransitionMode), mode, false);
            return new TurnstileTransition { Mode = turnstileTransitionMode };
        }

        private TransitionElement TransitionElement(string family, string mode)
        {
            switch (family)
            {
                case "Rotate":
                    return RotateTransitionElement(mode);
                case "Slide":
                    return SlideTransitionElement(mode);
                case "Swivel":
                    return SwivelTransitionElement(mode);
                case "Turnstile":
                    return TurnstileTransitionElement(mode);
            }
            return null;
        }

        private void See(object sender, RoutedEventArgs e)
        {
            string family = (string)Family.SelectedItem;
            string mode = (string)Mode.SelectedItem;
            TransitionElement transitionElement = null;
            if (family.Equals("Roll"))
            {
                transitionElement = new RollTransition();
            }
            else
            {
                transitionElement = TransitionElement(family, mode);
            }
            PhoneApplicationPage phoneApplicationPage = (PhoneApplicationPage)(((PhoneApplicationFrame)Application.Current.RootVisual)).Content;
            ITransition transition = transitionElement.GetTransition(phoneApplicationPage);
            transition.Completed += delegate
            {
                transition.Stop();
            };
            transition.Begin();
        }

        private void Forward(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Samples/NavigationTransitionSample1.xaml", UriKind.Relative));
        }

        private void FamilySelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string family = (string)Family.SelectedItem;
            Mode.Visibility = family.Equals("Roll") ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;
            if (s == null)
            {
                return null;
            }
            switch (s)
            {
                case "Roll":
                    return new List<string>();
                case "Rotate":
                    return new List<string>
                    {
                        "In90Clockwise",
                        "In90Counterclockwise",
                        "In180Clockwise",
                        "In180Counterclockwise",
                        "Out90Clockwise",
                        "Out90Counterclockwise",
                        "Out180Clockwise",
                        "Out180Counterclockwise"
                    };
                case "Slide":
                    return new List<string>
                    {
                        "SlideUpFadeIn",
                        "SlideUpFadeOut",
                        "SlideDownFadeIn",
                        "SlideDownFadeOut",
                        "SlideLeftFadeIn",
                        "SlideLeftFadeOut",
                        "SlideRightFadeIn",
                        "SlideRightFadeOut"
                    };
                case "Swivel":
                    return new List<string>
                    {
                        "FullScreenIn",
                        "FullScreenOut",
                        "ForwardIn",
                        "ForwardOut",
                        "BackwardIn",
                        "BackwardOut"
                    };
                case "Turnstile":
                    return new List<string>
                    {
                        "ForwardIn",
                        "ForwardOut",
                        "BackwardIn",
                        "BackwardOut"
                    };
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}