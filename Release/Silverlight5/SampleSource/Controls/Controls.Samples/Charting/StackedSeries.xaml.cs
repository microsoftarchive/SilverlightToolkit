// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample page demonstrating stacked series.
    /// </summary>
    [Sample("Stacked Series", DifficultyLevel.Basic, "Stacked Series")]
    public partial class StackedSeriesSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the StackedSeriesSample class.
        /// </summary>
        public StackedSeriesSample()
        {
            InitializeComponent();
            CheckupsQ1 = new Pet[]
            {
                new Pet { Species = "Dog", Count = 20 },
                new Pet { Species = "Cat", Count = 22 },
                new Pet { Species = "Bird", Count = 8 },
                new Pet { Species = "Gerbil", Count = 3 },
                new Pet { Species = "Turtle", Count = 2 },
            };
            CheckupsQ2 = new Pet[]
            {
                new Pet { Species = "Dog", Count = 18 },
                new Pet { Species = "Cat", Count = 25 },
                new Pet { Species = "Bird", Count = 9 },
                new Pet { Species = "Gerbil", Count = 2 },
                new Pet { Species = "Turtle", Count = 1 },
            };
            CheckupsQ3 = new Pet[]
            {
                new Pet { Species = "Dog", Count = 24 },
                new Pet { Species = "Cat", Count = 19 },
                new Pet { Species = "Bird", Count = 6 },
                new Pet { Species = "Gerbil", Count = 2 },
                new Pet { Species = "Turtle", Count = 0 },
            };
            CheckupsQ4 = new Pet[]
            {
                new Pet { Species = "Dog", Count = 21 },
                new Pet { Species = "Cat", Count = 21 },
                new Pet { Species = "Bird", Count = 8 },
                new Pet { Species = "Gerbil", Count = 5 },
                new Pet { Species = "Turtle", Count = 3 },
            };

            DataContext = this;
        }

        /// <summary>
        /// Gets the number of checkups for each pet during Q1.
        /// </summary>
        public IEnumerable<Pet> CheckupsQ1 { get; private set; }

        /// <summary>
        /// Gets the number of checkups for each pet during Q2.
        /// </summary>
        public IEnumerable<Pet> CheckupsQ2 { get; private set; }

        /// <summary>
        /// Gets the number of checkups for each pet during Q3.
        /// </summary>
        public IEnumerable<Pet> CheckupsQ3 { get; private set; }

        /// <summary>
        /// Gets the number of checkups for each pet during Q4.
        /// </summary>
        public IEnumerable<Pet> CheckupsQ4 { get; private set; }
    }
}
