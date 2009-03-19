// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace System.Windows.Controls.DataVisualization.Charting
{
    /// <summary>
    /// Represents an item used by a Series in the Legend of a Chart.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class LegendItem : ContentControl
    {
#if !SILVERLIGHT
        /// <summary>
        /// Initializes the static members of the LegendItem class.
        /// </summary>
        static LegendItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LegendItem), new FrameworkPropertyMetadata(typeof(LegendItem)));
        }

#endif
        /// <summary>
        /// Initializes a new instance of the LegendItem class.
        /// </summary>
        public LegendItem()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(LegendItem);
#endif
        }
    }
}
