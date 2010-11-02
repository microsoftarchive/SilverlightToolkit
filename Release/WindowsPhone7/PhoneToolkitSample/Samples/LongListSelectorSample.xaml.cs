using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using PhoneToolkitSample.Data;
using System;

namespace PhoneToolkitSample.Samples
{
    public partial class LongListSelectorSample : PhoneApplicationPage
    {
        public LongListSelectorSample()
        {
            InitializeComponent();

            LoadLinqMovies();

            linqMovies.SelectionChanged += MovieSelectionChanged;
            codeMovies.SelectionChanged += MovieSelectionChanged;
            buddies.SelectionChanged += PersonSelectionChanged;
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

        void MovieSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        void PersonSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Person person = buddies.SelectedItem as Person;
            if (person != null)
            {
                NavigationService.Navigate(new Uri("/Samples/PersonDetail.xaml?ID=" + person.ID, UriKind.Relative));
            }
        }
    }
}