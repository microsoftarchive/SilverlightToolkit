// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating the DataPager.
    /// </summary>
    [Sample("DataPager", DifficultyLevel.Basic)]
    [Category("DataPager")]
    public partial class DataPagerSample : UserControl
    {
        /// <summary>
        /// Initializes a DataPagerSample.
        /// </summary>
        public DataPagerSample()
        {
            InitializeComponent();
            PagedCollectionView pcv = new PagedCollectionView(Airport.SampleAirports.ToArray());
            pcv.PageSize = 6;
            DataContext = pcv;
        }

        /// <summary>
        /// Initializes the sample when the Loaded event is raised.
        /// </summary>
        /// <param name="sender">The sample page.</param>
        /// <param name="e">Event arguments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by event defined in Xaml.")]
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            pagerFln.DisplayMode = PagerDisplayMode.FirstLastNumeric;
            pagerFlpn.DisplayMode = PagerDisplayMode.FirstLastPreviousNext;
            pagerFlpnn.DisplayMode = PagerDisplayMode.FirstLastPreviousNextNumeric;
            pagerN.DisplayMode = PagerDisplayMode.Numeric;
            pagerPn.DisplayMode = PagerDisplayMode.PreviousNext;
            pagerPnn.DisplayMode = PagerDisplayMode.PreviousNextNumeric;
            pagerFln.PageIndex = 1;
            pagerFln.PageIndex = 0;
        }
    }
}
