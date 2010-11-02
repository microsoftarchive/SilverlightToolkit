// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace PhoneToolkitSample.Data
{
    public class Movie
    {
        private static Random _rnd = new Random(42);

        public static readonly string[] Categories = { "Action", "Romance", "Thrillers", "Comedy", "Documentaries", "Drama" };
        private static int _categoryIndex;

        private static readonly string[] Ratings = { "G", "PG", "PG-13", "R" };

        public string Title { get; private set; }
        public string Description { get; private set; }
        public int Year { get; private set; }
        public string Rating { get; private set; }
        public string Star1 { get; private set; }
        public string Star2 { get; private set; }
        public TimeSpan RunTime { get; private set; }

        public string ImageUrl { get; set; }

        public string Stars { get { return Star1 + ", " + Star2; } }
        public string Information { get { return string.Format("{0} {1} {2}:{3:D2}", Year, Rating, RunTime.Hours, RunTime.Minutes); } }
        public string Category { get; private set; }

        public static Movie CreateRandom()
        {
            string category = Categories[_categoryIndex];
            _categoryIndex = (_categoryIndex + 1) % Categories.Length;
            return CreateRandom(category);
        }

        public static Movie CreateRandom(string category)
        {
            Movie movie = new Movie();
            movie.Title = LoremIpsum.GetWords(_rnd.Next(1, 5), LoremIpsum.Capitalization.AllWords);
            movie.Year = _rnd.Next(1965, 2010);
            movie.Rating = _rnd.Next(Ratings);
            movie.RunTime = TimeSpan.FromMinutes(_rnd.Next(60, 180));
            movie.Description = LoremIpsum.GetParagraph(_rnd.Next(3, 7));
            movie.Star1 = RandomPeople.GetRandomFullName();
            movie.Star2 = RandomPeople.GetRandomFullName();
            movie.Category = category;
            movie.ImageUrl = "/Images/Movie.jpg";

            return movie;
        }
    }
}
