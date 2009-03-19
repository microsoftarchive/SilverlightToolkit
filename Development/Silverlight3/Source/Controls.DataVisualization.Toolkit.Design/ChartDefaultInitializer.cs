// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using SSW = Silverlight::System.Windows;
using SSWD = Silverlight::System.Windows.Data;
using SSWM = Silverlight::System.Windows.Media;

namespace System.Windows.Controls.DataVisualization.Design
{
    /// <summary>
    /// Default initializer for chart. 
    /// </summary>
    public class ChartDefaultInitializer : DefaultInitializer
    {
        /// <summary>
        /// Sets the default property values for chart. 
        /// </summary>
        /// <param name="item">Chart ModelItem.</param>
        public override void InitializeDefaults(ModelItem item)
        {
            string propertyName;

            // <Charting:Chart Title="Chart Title">
            propertyName = Extensions.GetMemberName<Chart>(x => x.Title);
            item.Properties[propertyName].SetValue(Properties.Resources.ChartTitle);

            // <Charting:Chart.DataContext>
            //     <PointCollection>
            //         <Point X="1" Y="10" />
            //         <Point X="2" Y="20" />
            //         <Point X="3" Y="30" />
            //         <Point X="4" Y="40" />
            //     </PointCollection>
            // </Charting:Chart.DataContext>

            SSWM::PointCollection defaultItemsSource = new SSWM::PointCollection();
            for (int i = 1; i <= 4; i++)
            {
                defaultItemsSource.Add(new SSW::Point(i, 10 * i));
            }

            propertyName = Extensions.GetMemberName<Chart>(x => x.DataContext);
            item.Properties[propertyName].SetValue(defaultItemsSource);

            // <Charting:Chart.Series>
            //     <Charting:ColumnSeries ItemsSource="{Binding}"
            //         DependentValuePath="X"
            //         IndependentValuePath="Y" />
            // </Charting:Chart.Series>

            ModelItem columnSeries = ModelFactory.CreateItem(item.Context, typeof(ColumnSeries));
            propertyName = Extensions.GetMemberName<ColumnSeries>(x => x.ItemsSource);
            columnSeries.Properties[propertyName].SetValue(ModelFactory.CreateItem(columnSeries.Context, typeof(SSWD::Binding)));
            propertyName = Extensions.GetMemberName<ColumnSeries>(x => x.DependentValuePath);
            columnSeries.Properties[propertyName].SetValue("X");
            propertyName = Extensions.GetMemberName<ColumnSeries>(x => x.IndependentValuePath);
            columnSeries.Properties[propertyName].SetValue("Y");

            propertyName = Extensions.GetMemberName<Chart>(x => x.Series);
            item.Properties[propertyName].Collection.Add(columnSeries);
        }
    }
}