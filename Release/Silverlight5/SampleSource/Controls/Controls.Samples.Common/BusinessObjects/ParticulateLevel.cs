// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// ParticulateLevel business object used for charting samples.
    /// </summary>
    public class ParticulateLevel
    {
        /// <summary>
        /// Gets or sets the particulate count.
        /// </summary>
        public int Particulate { get; set; }

        /// <summary>
        /// Gets or sets the daily rainfall.
        /// </summary>
        public double Rainfall { get; set; }

        /// <summary>
        /// Initializes a new instance of the ParticulateLevel class.
        /// </summary>
        public ParticulateLevel()
        {
        }

        /// <summary>
        /// Gets a collection of particulate levels for rainfall.
        /// </summary>
        /// <remarks>
        /// Sample data from http://office.microsoft.com/en-us/help/HA102274781033.aspx.
        /// </remarks>
        public static ObjectCollection LevelsInRainfall
        {
            get
            {
                ObjectCollection levelsInRainfall = new ObjectCollection();
                levelsInRainfall.Add(new ParticulateLevel { Particulate = 122, Rainfall = 4.1 });
                levelsInRainfall.Add(new ParticulateLevel { Particulate = 117, Rainfall = 4.3 });
                levelsInRainfall.Add(new ParticulateLevel { Particulate = 112, Rainfall = 5.7 });
                levelsInRainfall.Add(new ParticulateLevel { Particulate = 114, Rainfall = 5.4 });
                levelsInRainfall.Add(new ParticulateLevel { Particulate = 110, Rainfall = 5.9 });
                levelsInRainfall.Add(new ParticulateLevel { Particulate = 114, Rainfall = 5.0 });
                levelsInRainfall.Add(new ParticulateLevel { Particulate = 128, Rainfall = 3.6 });
                levelsInRainfall.Add(new ParticulateLevel { Particulate = 137, Rainfall = 1.9 });
                levelsInRainfall.Add(new ParticulateLevel { Particulate = 104, Rainfall = 7.3 });
                return levelsInRainfall;
            }
        }
    }
}