// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace System.Windows.Controls.Samples
{  
    /// <summary>
    /// Sample demonstrating the TreeView used in a Master/Detail scenario.
    /// </summary>
    [Sample("MasterDetail", DifficultyLevel.Scenario, "TreeView")]
    public partial class MasterDetailSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the MasterDetailSample class.
        /// </summary>
        public MasterDetailSample()
        {
            InitializeComponent();
            MasterTree.ItemsSource = Taxonomy.Life;
        }

        /// <summary>
        /// Handle the TreeView.SelectedItemChanged event.
        /// </summary>
        /// <param name="sender">The TreeView.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "The event handler is declared in XAML.")]
        private void MasterTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Setting the DataContext on the panel containing all the 
            // detail controls allows setting the Master object once
            // instead of once per control.
            DetailsPanel.DataContext = e.NewValue;

            // Simulate looking up data in another data source.
            if (e.NewValue != null)
            {
                Taxonomy taxonomy = (Taxonomy)e.NewValue;                
                
                StringBuilder information = new StringBuilder();
                information.AppendFormat(CultureInfo.CurrentCulture, "The {0} {1}, represents a signifigant portion of this sample text.\n\n", taxonomy.Rank, taxonomy.Classification);

                switch (taxonomy.Subclasses.Count)
                {
                    case 0:
                        information.Append("Doesn't contain any subclasses.");
                        break;                    

                    case 1:
                        information.Append("This contains only a single subclass.");
                        break;

                    default:
                        information.AppendFormat(CultureInfo.CurrentCulture, "Contains {0} subclasses.", taxonomy.Subclasses.Count);
                        break;
                }

                LookupDetailText.Text = information.ToString();
            }
        }
    }
}
