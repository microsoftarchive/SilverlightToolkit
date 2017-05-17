// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample for the experimental control: TransitioningContentControl.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    [Sample("TransitioningContentControl", DifficultyLevel.Basic, "TransitioningContentControl")]
    public partial class TransitioningContentControlSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransitioningContentControlSample"/> class.
        /// </summary>
        public TransitioningContentControlSample()
        {
            InitializeComponent();

            Loaded += (sender, e) => dud.ItemsSource = Airport.SampleAirports;
        }

        /// <summary>
        /// Changes the content with the default transition.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by Xaml.")]
        private void ChangeContentSample1(object sender, RoutedEventArgs e)
        {
            defaultTCC.Transition = TransitioningContentControl.DefaultTransitionState;
            defaultTCC.Content = DateTime.Now.Ticks;
        }

        /// <summary>
        /// Changes the content with the down transition.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by Xaml.")]
        private void ChangeContentSample1Down(object sender, RoutedEventArgs e)
        {
            defaultTCC.Transition = "DownTransition";
            defaultTCC.Content = DateTime.Now.Ticks;
        }

        /// <summary>
        /// Changes the content up transition.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by Xaml.")]
        private void ChangeContentSample1Up(object sender, RoutedEventArgs e)
        {
            defaultTCC.Transition = "UpTransition";
            defaultTCC.Content = DateTime.Now.Ticks;
        }
    }
}