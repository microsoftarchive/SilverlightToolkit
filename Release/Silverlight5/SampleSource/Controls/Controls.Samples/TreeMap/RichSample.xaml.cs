// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Data;

[assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "System.Windows.Controls.Samples.RichSample.#itemContainer")]
namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// Sample showing the TreeMap with richer visuals and functionality.
    /// </summary>
    [Sample("(5)Rich sample", DifficultyLevel.Intermediate, "TreeMap")]
    public partial class RichSample : UserControl
    {
        /// <summary>
        /// Internal cache of the data source, used when refreshing the TreeMap.
        /// </summary>
        private IList<NhlNode> _dataSource;

        /// <summary>
        /// Initializes a new instance of the RichSample class.
        /// </summary>
        public RichSample()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(RichSample_Loaded);
        }

        /// <summary>
        /// Loads the XML sample data and populates the TreeMap.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void RichSample_Loaded(object sender, RoutedEventArgs e)
        {
            // Sample browser-specific layout change
            SampleHelpers.ChangeSampleAlignmentToStretch(this);

            sizeMetric.SelectedIndex = 1;
            colorMetric.SelectedIndex = 0;

            _dataSource = NhlDataHelper.LoadDefaultFile();

            RefreshDataSource();
        }

        /// <summary>
        /// Helper to force the TreeMap to refresh its data.
        /// </summary>
        private void RefreshDataSource()
        {
            treeMapControl.ItemsSource = null;
            treeMapControl.ItemsSource = _dataSource;
        }

        /// <summary>
        /// Changes the property used to generate sizes for rectangles.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Attached as an event handler in XAML")]
        private void OnSizeMetricSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ComboBoxItem item = e.AddedItems[0] as ComboBoxItem;
                treeMapControl.ItemDefinition.ValueBinding = new Binding(item.Tag as string);

                RefreshDataSource();
            }
        }

        /// <summary>
        /// Changes the property used to set the background color on rectangles.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Attached as an event handler in XAML")]
        private void OnColorMetricSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ComboBoxItem item = e.AddedItems[0] as ComboBoxItem;
                treeMapControl.Interpolators[0].DataRangeBinding = new Binding(item.Tag as string);

                RefreshDataSource();
            }
        }

        /// <summary>
        /// This is a workaround for the ToolTip behavior - when tooltip is a 
        /// nested element, DataContext is not inherited.
        /// It is not a TreeMap specific workaround.
        /// </summary>
        /// <param name="sender">Sending UI element - Border in our case.</param>
        /// <param name="e">Events - irrelevant in our case.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Attached as an event handler in XAML")]
        private void ItemContainer_Loaded(object sender, RoutedEventArgs e)
        {
            Border border = sender as Border;

            // see summary
            if (border != null)
            {
                ((FrameworkElement) ToolTipService.GetToolTip(border)).DataContext = border.DataContext;
            }
        }
    }
}
