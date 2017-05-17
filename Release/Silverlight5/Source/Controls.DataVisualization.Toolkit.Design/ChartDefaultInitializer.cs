// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Model;
using SSW = Silverlight::System.Windows;
using SSWCDC = Silverlight::System.Windows.Controls.DataVisualization.Charting;
using SSWD = Silverlight::System.Windows.Data;
using SSWM = Silverlight::System.Windows.Media;

namespace System.Windows.Controls.DataVisualization.Design
{
    /// <summary>
    /// Default initializer for chart. 
    /// </summary>
    internal class ChartDefaultInitializer : DefaultInitializer
    {
        /// <summary>
        /// Sets the default property values for chart. 
        /// </summary>
        /// <param name="item">SSWCDC.Chart ModelItem.</param>
        public override void InitializeDefaults(ModelItem item)
        {
            string propertyName;

            // <toolkit:Chart Title="Chart Title">
            //     <toolkit:Charting:Chart.Series>
            //         <toolkit:Charting:ColumnSeries DependentValuePath="X" IndependentValuePath="Y" >
            //             <toolkit:ColumnSeries.ItemsSource>
            //                 <PointCollection>
            //                     <Point X="1" Y="10" />
            //                     <Point X="2" Y="20" />
            //                     <Point X="3" Y="30" />
            //                     <Point X="4" Y="40" />
            //                 </PointCollection>
            //             </toolkit:ColumnSeries.ItemsSource>
            //         </toolkit:Charting:ColumnSeries>
            //     </toolkit:Chart.Series>
            // </toolkit:Chart>

            propertyName = Extensions.GetMemberName<SSWCDC.Chart>(x => x.Title);
            item.Properties[propertyName].SetValue(Properties.Resources.ChartTitle);

            ModelItem columnSeries = ModelFactory.CreateItem(item.Context, typeof(SSWCDC.ColumnSeries));
            propertyName = Extensions.GetMemberName<SSWCDC.ColumnSeries>(x => x.DependentValuePath);
            columnSeries.Properties[propertyName].SetValue("X");
            propertyName = Extensions.GetMemberName<SSWCDC.ColumnSeries>(x => x.IndependentValuePath);
            columnSeries.Properties[propertyName].SetValue("Y");

            SSWM::PointCollection defaultItemsSource = new SSWM::PointCollection();
            for (int i = 1; i <= 4; i++)
            {
                defaultItemsSource.Add(new SSW::Point(i, 10 * i));
            }
            propertyName = Extensions.GetMemberName<SSWCDC.ColumnSeries>(x => x.ItemsSource);
            columnSeries.Properties[propertyName].SetValue(defaultItemsSource);

            propertyName = Extensions.GetMemberName<SSWCDC.Chart>(x => x.Series);
            item.Properties[propertyName].Collection.Add(columnSeries);
        }
    }
}