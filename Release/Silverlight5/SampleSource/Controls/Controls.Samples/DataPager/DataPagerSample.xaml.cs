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
    [Sample("DataPager", DifficultyLevel.Basic, "DataPager")]
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
    }
}
