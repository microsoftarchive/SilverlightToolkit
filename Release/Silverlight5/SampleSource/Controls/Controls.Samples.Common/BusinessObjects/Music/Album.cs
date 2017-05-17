// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Markup;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Represents a music album.
    /// </summary>
    [ContentProperty("Songs")]
    public class Album
    {
        /// <summary>
        /// Gets or sets the title of the Album.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the year the Album was released.
        /// </summary>
        public string ReleaseYear { get; set; }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        public static Image Icon
        {
            get
            {
                return SharedResources.GetIcon("Album.png");
            }
        }

        /// <summary>
        /// Gets a collection of songs contained in the Album.
        /// </summary>
        public Collection<Song> Songs { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Album class.
        /// </summary>
        public Album()
        {
            Songs = new Collection<Song>();
        }
    }
}
