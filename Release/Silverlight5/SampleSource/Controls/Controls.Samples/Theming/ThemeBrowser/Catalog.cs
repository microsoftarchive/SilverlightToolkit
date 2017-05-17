// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.Windows.Markup;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Represents a Catalog of MediaItems. 
    /// </summary>
    [ContentProperty("MediaItems")]
    public class Catalog
    {
        /// <summary>
        /// Gets or sets the name of the Catalog.
        /// </summary>
        public string CatalogName { get; set; }

        /// <summary>
        /// Gets or sets the Image representing the catalog.
        /// </summary>
        public Image CatalogImage { get; set; }

        /// <summary>
        /// Gets or sets a collection of media items. 
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Simplifies samples.")]
        public Collection<MediaItem> MediaItems { get; set; }

        /// <summary>
        /// Initializes a new instance of the Catalog class. 
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="resourceName">
        /// Name of the resource representing the catalog thumbnail.
        /// </param>
        public Catalog(string catalogName, string resourceName)
        {
            CatalogName = catalogName;
            CatalogImage = SharedResources.GetImage(resourceName);
            MediaItems = new Collection<MediaItem>();
        }

        #region Sample Data
        /// <summary>
        /// Gets a Vacation Catalog.
        /// </summary>
        public static Catalog VacationCatalog
        {
            get
            {
                Catalog catalog = new Catalog("My Vacations", "Sunset.jpg");
                catalog.MediaItems = Catalog.VacationMediaItems;
                return catalog;
            }
        }

        /// <summary>
        /// Gets a collection of media items representing a catog of upcoming
        /// vacation trips.
        /// </summary>
        public static Collection<MediaItem> PlannedVacationMediaItems
        {
            get
            {
                return new Collection<MediaItem> 
                {
                    new MediaItem("Mountains.jpg", "Australia", "My Australian Vacation", new DateTime(2006, 3, 1)),
                    new MediaItem("Oryx Antelope.jpg", "Sahara", "Sahara Desert", new DateTime(2007, 3, 12)),
                    new MediaItem("River.jpg", "Italy", "The best days of my life", new DateTime(2008, 4, 11)),
                    new MediaItem("Sunset.jpg", "Los Angeles", "I love LA", new DateTime(2008, 4, 11)),
                    new MediaItem("Desert Landscape.jpg", "Grand Canyon", "A Day in Grand Caniao", new DateTime(2008, 6, 13)),
                    new MediaItem("Mountains.jpg", "Australia", "My Australian Vacation", new DateTime(2006, 3, 1)),
                    new MediaItem("Oryx Antelope.jpg", "Sahara", "Sahara Desert", new DateTime(2007, 3, 12)),
                    new MediaItem("River.jpg", "Italy", "The best days of my life", new DateTime(2008, 4, 11)),
                    new MediaItem("Sunset.jpg", "Los Angeles", "I love LA", new DateTime(2008, 4, 11)),
                    new MediaItem("Desert Landscape.jpg", "Grand Canyon", "A Day in Grand Caniao", new DateTime(2008, 6, 13)),
                    new MediaItem("Mountains.jpg", "Australia", "My Australian Vacation", new DateTime(2006, 3, 1)),
                    new MediaItem("Oryx Antelope.jpg", "Sahara", "Sahara Desert", new DateTime(2007, 3, 12)),
                    new MediaItem("River.jpg", "Italy", "The best days of my life", new DateTime(2008, 4, 11)),
                    new MediaItem("Sunset.jpg", "Los Angeles", "I love LA", new DateTime(2008, 4, 11)),
                    new MediaItem("Desert Landscape.jpg", "Grand Canyon", "A Day in Grand Caniao", new DateTime(2008, 6, 13)) 
                };
            }
        }

        /// <summary>
        /// Gets a collection of media items representing a catog of vacaction
        /// destinations.
        /// </summary>
        public static Collection<MediaItem> VacationMediaItems
        {
            get
            {
                return new Collection<MediaItem> 
                {
                    new MediaItem("Mountains.jpg", "Australia", "My Australian Vacation", new DateTime(2006, 3, 1)),
                    new MediaItem("Oryx Antelope.jpg", "Sahara", "Sahara Desert", new DateTime(2007, 3, 12)),
                    new MediaItem("River.jpg", "Italy", "The best days of my life", new DateTime(2008, 4, 11)),
                    new MediaItem("Sunset.jpg", "Los Angeles", "I love LA", new DateTime(2008, 4, 11)),
                    new MediaItem("Desert Landscape.jpg", "Grand Canyon", "A Day in Grand Caniao", new DateTime(2008, 6, 13)),
                    new MediaItem("Humpback Whale.jpg", "Alaska", "It's Cold in here....", new DateTime(2008, 1, 12)) 
                };
            }
        }
        #endregion Sample Data
    }
}