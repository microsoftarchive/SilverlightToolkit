// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Model;
using SSW = Silverlight::System.Windows;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// Default initializer for DragDropTarget. 
    /// </summary>
    internal class DragDropTargetDefaultInitializer : DefaultInitializer
    {
        /// <summary>
        /// Sets the default property values for DragDropTarget. 
        /// </summary>
        /// <param name="item">SSWC.DragDropTarget ModelItem.</param>
        public override void InitializeDefaults(ModelItem item)
        {
            string propertyName;

            // <toolkit:DragDropTarget Content={x:Null}" 
            propertyName = Extensions.GetMemberName<SSWC.ContentControl>(x => x.Content);
            item.Properties[propertyName].SetValue(null);
            // VerticalContentAlignment
            propertyName = Extensions.GetMemberName<SSWC.ContentControl>(x => x.VerticalContentAlignment);
            item.Properties[propertyName].SetValue(SSW.VerticalAlignment.Stretch);
            // HorizontalContentAlignment
            propertyName = Extensions.GetMemberName<SSWC.ContentControl>(x => x.HorizontalContentAlignment);
            item.Properties[propertyName].SetValue(SSW.HorizontalAlignment.Stretch);
        }
    }
}