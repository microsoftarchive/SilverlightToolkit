// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls.DataVisualization.Charting;
using Microsoft.Silverlight.Testing;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Unit tests for children of the DataPointSeries class.
    /// </summary>
    public abstract partial class DataPointSeriesWithAxesBase : DataPointSeriesBase
    {
        /// <summary>
        /// Gets a default instance of DataPointSeriesWithAxes (or a derived type) to test.
        /// </summary>
        public DataPointSeriesWithAxes DataPointSeriesWithAxesToTest
        {
            get
            {
                return DefaultSeriesToTest as DataPointSeriesWithAxes;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DataPointSeriesWithAxesBase class.
        /// </summary>
        protected DataPointSeriesWithAxesBase()
        {
        }
    }
}