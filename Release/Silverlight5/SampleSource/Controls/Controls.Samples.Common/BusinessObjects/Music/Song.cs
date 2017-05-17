// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Controls;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Represents a music song.
    /// </summary>
    public class Song
    {
        /// <summary>
        /// Gets or sets the title of the Song.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the length of the Song.
        /// </summary>
        public string Length { get; set; }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        public static Image Icon
        {
            get
            {
                return SharedResources.GetIcon("Song.png");
            }
        }
    }
}
