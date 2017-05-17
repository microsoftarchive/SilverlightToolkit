// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using SSWCDC = Silverlight::System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.Design;

namespace System.Windows.Controls.DataVisualization.Design
{
    /// <summary>
    /// To register design time metadata for SSWCDC.DataPointSeriesDragDropTarget.
    /// </summary>
    internal class DataPointSeriesDragDropTargetMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for SSWCDC.DataPointSeriesDragDropTarget.
        /// </summary>
        public DataPointSeriesDragDropTargetMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWCDC.DataPointSeriesDragDropTarget),
                b =>
                {
                    b.AddCustomAttributes(new FeatureAttribute(typeof(DragDropTargetDefaultInitializer)));
                });
        }
    }
}