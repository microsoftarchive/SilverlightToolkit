// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for SSWC.DragDropTarget.
    /// </summary>
    internal class DragDropTargetMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DragDropTargets.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "By design.")]
        public DragDropTargetMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.ListBoxDragDropTarget),
                b =>
                {
                    b.AddCustomAttributes(new FeatureAttribute(typeof(DragDropTargetDefaultInitializer)));
                });

            AddCallback(
                typeof(SSWC.TreeViewDragDropTarget),
                b =>
                {
                    b.AddCustomAttributes(new FeatureAttribute(typeof(DragDropTargetDefaultInitializer)));
                });
        }
    }
}