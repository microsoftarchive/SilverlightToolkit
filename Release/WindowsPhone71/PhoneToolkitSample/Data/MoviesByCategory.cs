// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PhoneToolkitSample.Data
{
    public class MoviesByCategory : ObservableCollection<MoviesInCategory>
    {
        public MoviesByCategory()
        {
            List<string> sortedCategories = new List<string>(Movie.Categories);
            sortedCategories.Sort();

            foreach (string category in sortedCategories)
            {
                MoviesInCategory group = new MoviesInCategory(category);
                this.Add(group);

                for (int i = 0; i < 5; ++i)
                {
                    group.Add(Movie.CreateRandom(category));
                }                
            }
        }
    }

    public class MoreCommand : ICommand
    {
        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            MoviesInCategory group = parameter as MoviesInCategory;
            if (group != null)
            {
                group.Add(Movie.CreateRandom(group.Key));
            }
        }

        #endregion
    }
}
