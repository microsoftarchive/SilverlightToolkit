// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Model;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.VisualStudio.Design
{
    /// <summary>
    /// Default initializer for chart. 
    /// </summary>
    internal class GridSplitterDefaultInitializer : DefaultInitializer
    {
        /// <summary>
        /// Sets the default property values for GridSplitter. 
        /// </summary>
        /// <param name="item">SSWC.GridSplitter ModelItem.</param>
        public override void InitializeDefaults(ModelItem item)
        {
            string propertyName;

            propertyName = Extensions.GetMemberName<SSWC.GridSplitter>(x => x.Width);
            item.Properties[propertyName].SetValue("10.0");

            propertyName = Extensions.GetMemberName<SSWC.GridSplitter>(x => x.Height);
            item.Properties[propertyName].SetValue("100.0");
        }
    }
}