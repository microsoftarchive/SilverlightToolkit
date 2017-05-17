// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Controls.Samples;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    ///  Business Object used in the ThemeBrowserSample.
    /// </summary>
    public sealed partial class MediaItem
    {
        /// <summary>
        /// Gets the name of the Media.
        /// </summary>
        public string MediaName { get; private set; }

        /// <summary>
        /// Gets an Image representing the media.
        /// </summary>
        public Image Image { get; private set; }

        /// <summary>
        /// Gets or sets Description of the media.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Date Stump of the media.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Initializes a new instance of the Catalog class.
        /// </summary>
        /// <param name="resourceName">Name of the resource defining the Image of the media.</param>
        /// <param name="mediaName">Name of the media.</param>
        /// <param name="description">Description of the media.</param>
        /// <param name="date">Date Stump for the media.</param>
        public MediaItem(string resourceName, string mediaName, string description, DateTime date)
        {
            MediaName = mediaName;
            Description = description;
            Image = SharedResources.GetImage(resourceName);
            Date = date;
        }

        /// <summary>
        /// Overrides the string to return the name.
        /// </summary>
        /// <returns>Returns the photograph name.</returns>
        public override string ToString()
        {
            return MediaName;
        }       
    }
}
