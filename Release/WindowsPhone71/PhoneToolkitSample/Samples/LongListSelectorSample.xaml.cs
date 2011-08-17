// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using PhoneToolkitSample.Data;

namespace PhoneToolkitSample.Samples
{
    public partial class LongListSelectorSample : PhoneApplicationPage
    {
        private LongListSelector currentSelector;

        public LongListSelectorSample()
        {
            InitializeComponent();

            LoadLinqMovies();
            buddies.SelectionChanged += PersonSelectionChanged;
        }

        private void LongListSelector_GroupViewOpened(object sender, GroupViewOpenedEventArgs e)
        {
            //Hold a reference to the active long list selector.
            currentSelector = sender as LongListSelector;

            //Construct and begin a swivel animation to pop in the group view.
            IEasingFunction quadraticEase = new QuadraticEase { EasingMode = EasingMode.EaseOut };
            Storyboard _swivelShow = new Storyboard();
            ItemsControl groupItems = e.ItemsControl;            

            foreach (var item in groupItems.Items)
            {
                UIElement container = groupItems.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
                if (container != null)
                {
                    Border content = VisualTreeHelper.GetChild(container, 0) as Border;
                    if (content != null)
                    {
                        DoubleAnimationUsingKeyFrames showAnimation = new DoubleAnimationUsingKeyFrames();

                        EasingDoubleKeyFrame showKeyFrame1 = new EasingDoubleKeyFrame();
                        showKeyFrame1.KeyTime = TimeSpan.FromMilliseconds(0);
                        showKeyFrame1.Value = -60;
                        showKeyFrame1.EasingFunction = quadraticEase;

                        EasingDoubleKeyFrame showKeyFrame2 = new EasingDoubleKeyFrame();
                        showKeyFrame2.KeyTime = TimeSpan.FromMilliseconds(85);
                        showKeyFrame2.Value = 0;
                        showKeyFrame2.EasingFunction = quadraticEase;

                        showAnimation.KeyFrames.Add(showKeyFrame1);
                        showAnimation.KeyFrames.Add(showKeyFrame2);

                        Storyboard.SetTargetProperty(showAnimation, new PropertyPath(PlaneProjection.RotationXProperty));
                        Storyboard.SetTarget(showAnimation, content.Projection);

                        _swivelShow.Children.Add(showAnimation);
                    }
                }
            }

            _swivelShow.Begin();
        }

        private void LongListSelector_GroupViewClosing(object sender, GroupViewClosingEventArgs e)
        {
            //Cancelling automatic closing and scrolling to do it manually.
            e.Cancel = true;
            if (e.SelectedGroup != null)
            {
                currentSelector.ScrollToGroup(e.SelectedGroup);
            }

            //Dispatch the swivel animation for performance on the UI thread.
            Dispatcher.BeginInvoke(() =>
                {
                    //Construct and begin a swivel animation to pop out the group view.
                    IEasingFunction quadraticEase = new QuadraticEase { EasingMode = EasingMode.EaseOut };
                    Storyboard _swivelHide = new Storyboard();
                    ItemsControl groupItems = e.ItemsControl;

                    foreach (var item in groupItems.Items)
                    {
                        UIElement container = groupItems.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
                        if (container != null)
                        {
                            Border content = VisualTreeHelper.GetChild(container, 0) as Border;
                            if (content != null)
                            {
                                DoubleAnimationUsingKeyFrames showAnimation = new DoubleAnimationUsingKeyFrames();

                                EasingDoubleKeyFrame showKeyFrame1 = new EasingDoubleKeyFrame();
                                showKeyFrame1.KeyTime = TimeSpan.FromMilliseconds(0);
                                showKeyFrame1.Value = 0;
                                showKeyFrame1.EasingFunction = quadraticEase;

                                EasingDoubleKeyFrame showKeyFrame2 = new EasingDoubleKeyFrame();
                                showKeyFrame2.KeyTime = TimeSpan.FromMilliseconds(125);
                                showKeyFrame2.Value = 90;
                                showKeyFrame2.EasingFunction = quadraticEase;

                                showAnimation.KeyFrames.Add(showKeyFrame1);
                                showAnimation.KeyFrames.Add(showKeyFrame2);

                                Storyboard.SetTargetProperty(showAnimation, new PropertyPath(PlaneProjection.RotationXProperty));
                                Storyboard.SetTarget(showAnimation, content.Projection);

                                _swivelHide.Children.Add(showAnimation);
                            }
                        }
                    }

                    _swivelHide.Completed += _swivelHide_Completed;
                    _swivelHide.Begin();
                    
                });            
        }

        private void _swivelHide_Completed(object sender, EventArgs e)
        {
            //Close group view.
            if (currentSelector != null)
            {
                currentSelector.CloseGroupView();
                currentSelector = null;
            }
        }

        private void LoadLinqMovies()
        {
            List<Movie> movies = new List<Movie>();

            for (int i = 0; i < 50; ++i)
            {
                movies.Add(Movie.CreateRandom());
            }

            var moviesByCategory = from movie in movies
                                   group movie by movie.Category into c
                                   orderby c.Key
                                   select new PublicGrouping<string, Movie>(c);

            linqMovies.ItemsSource = moviesByCategory;
        }

        private void PersonSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Person person = buddies.SelectedItem as Person;
            if (person != null)
            {
                NavigationService.Navigate(new Uri("/Samples/PersonDetail.xaml?ID=" + person.ID, UriKind.Relative));
            }
        }   
    }
}