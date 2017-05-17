// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Represents a musical artist.
    /// </summary>
    [ContentProperty("Albums")]
    public class Artist
    {
        /// <summary>
        /// Gets or sets the name of the Artist.
        /// </summary>
        public string ArtistName { get; set; }

        /// <summary>
        /// Gets or sets the genre of the Artist.
        /// </summary>
        public string Genre { get; set; }

        /// <summary>
        /// Gets a collection of albums released by the Artist.
        /// </summary>
        public Collection<Album> Albums { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Artist class.
        /// </summary>
        public Artist()
        {
            Albums = new Collection<Album>();
        }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        public static Image Icon
        {
            get
            {
                return SharedResources.GetIcon("Artist.png");
            }
        }

        /// <summary>
        /// Gets a collection of artists.
        /// </summary>
        public static IEnumerable<Artist> AllArtists
        {
            get
            {
                IEnumerable<object> data = Application.Current.Resources["MusicCatalog"] as IEnumerable<object>;
                return (data != null) ?
                    data.OfType<Artist>() :
                    Enumerable.Empty<Artist>();
            }
        }
    }
}
