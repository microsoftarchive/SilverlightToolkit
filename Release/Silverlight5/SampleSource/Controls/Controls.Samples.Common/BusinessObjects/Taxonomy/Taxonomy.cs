// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Represents an item contained in a level of a Linnaean taxonomy.
    /// </summary>
    [ContentProperty("Subclasses")]
    public abstract class Taxonomy
    {
        /// <summary>
        /// Gets a subset of all life on the planet.
        /// </summary>
        public static IEnumerable<Taxonomy> Life
        {
            get
            {
                IEnumerable<object> data = Application.Current.Resources["TreeOfLife"] as IEnumerable<object>;
                return (data != null) ?
                    data.Cast<Taxonomy>() :
                    Enumerable.Empty<Taxonomy>();
            }
        }

        /// <summary>
        /// Gets the name of the TaxonomicRank.
        /// </summary>
        public string Rank
        {
            get { return GetType().Name; }
        }

        /// <summary>
        /// Gets or sets the classification of the item being ranked.
        /// </summary>
        public string Classification { get; set; }

        /// <summary>
        /// Gets the subclasses of of the item being ranked.
        /// </summary>
        public Collection<Taxonomy> Subclasses { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TaxonomicItem class.
        /// </summary>
        protected Taxonomy()
        {
            Subclasses = new Collection<Taxonomy>();
        }

        /// <summary>
        /// Get a string representation of the TaxonomicItem.
        /// </summary>
        /// <returns>String representation of the TaxonomicItem.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}: {1}", Rank, Classification);
        }
    }
}