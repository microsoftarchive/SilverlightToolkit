// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;

namespace PhoneToolkitSample.Data
{
    public class MoviesInCategory : ObservableCollection<Movie>
    {
        public MoviesInCategory(string category)
        {
            Key = category;
        }

        public string Key { get; set; }

        public string GetMore { get { return "More " + Key; } }
    }
}
