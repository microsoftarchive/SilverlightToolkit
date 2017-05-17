// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating DomainUpDown.
    /// </summary>
    [Sample("Animations", DifficultyLevel.Advanced, "DomainUpDown")]
    public partial class DomainUpDownAnimationsSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the DomainUpDownSlider class.
        /// </summary>
        public DomainUpDownAnimationsSample()
        {
            InitializeComponent();
            DudSlide.ItemsSource = Tutorial.Tutorials;
            DudSlideReflection.ItemsSource = Tutorial.Tutorials;
            DudSlide.Incremented += DudSlide_Incremented;
            DudSlide.Decremented += DudSlide_Decremented;
        }

        /// <summary>
        /// Responds to TransitioningDomainUpDown Incrementation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.EventArgs"/> 
        /// instance containing the event data.</param>
        private void DudSlide_Decremented(object sender, EventArgs e)
        {
            DudSlideReflection.Decrement();
        }

        /// <summary>
        /// Responds to TransitioningDomainUpDown Decrementation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.EventArgs"/> 
        /// instance containing the event data.</param>
        private void DudSlide_Incremented(object sender, EventArgs e)
        {
            DudSlideReflection.Increment();
        }

         /// <summary>
        /// Responds to Animation Setting Changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedPropertyChangedEventArgs"/> 
        /// instance containing the event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by event defined in Xaml.")]
        private void AnimationSettingsChanged(object sender, RoutedEventArgs e)
        {
            if (Rotation3D.IsChecked == true)
            {
                Style TransitionRotation3D = this.Resources["TransitioningContentRotation3D"] as Style;
                if (TransitionRotation3D != null)
                {
                    DudSlide.TransitioningContentControlStyle = TransitionRotation3D;
                    DudSlideReflection.TransitioningContentControlStyle = TransitionRotation3D;
                    DudSlide.Increment();
                    DudSlideReflection.Increment();
                }
            }
            else if (FadeInOut.IsChecked == true)
            {
                Style TransitioningContentFadeInOut = this.Resources["TransitioningContentFadeInOut"] as Style;

                if (TransitioningContentFadeInOut != null)
                {
                    DudSlide.TransitioningContentControlStyle = TransitioningContentFadeInOut;
                    DudSlideReflection.TransitioningContentControlStyle = TransitioningContentFadeInOut;
                    DudSlide.Increment();
                    DudSlideReflection.Increment();
                }
            }
            else
            {
                Style TransitioningContentTranslation = this.Resources["TransitioningContentTranslation"] as Style;
                if (TransitioningContentTranslation != null)
                {
                    DudSlide.TransitioningContentControlStyle = TransitioningContentTranslation;
                    DudSlideReflection.TransitioningContentControlStyle = TransitioningContentTranslation;
                    DudSlide.Increment();
                    DudSlideReflection.Increment();
                }
            }
        }
    }
}
