// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using SSW = Silverlight::System.Windows;
using SSWC = Silverlight::System.Windows.Controls;
using SSWCP = Silverlight::System.Windows.Controls.Primitives;

[assembly: ProvideMetadata(typeof(System.Windows.Controls.VisualStudio.Design.MetadataRegistration))]

namespace System.Windows.Controls.VisualStudio.Design
{
    /// <summary>
    /// MetadataRegistration class.
    /// </summary>
    public partial class MetadataRegistration : MetadataRegistrationBase, IProvideAttributeTable
    {
        /// <summary>
        /// Gets the AttributeTable for design time metadata.
        /// </summary>
        public AttributeTable AttributeTable
        {
            get
            {
                return BuildAttributeTable();
            }
        }

        /// <summary>
        /// Provide a place to add custom attributes without creating a AttributeTableBuilder subclass.
        /// </summary>
        /// <param name="builder">The assembly attribute table builder.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Necessary for providing metadata defaults.")]
        protected override void AddAttributes(AttributeTableBuilder builder)
        {
            GenericDefaultInitializer.SetAction(
                typeof(SSWC.Expander),
                (modelitem, context) =>
                {
                    ModelItem newGrid = ModelFactory.CreateItem(context, typeof(SSWC.Grid), CreateOptions.InitializeDefaults);
                    newGrid.Properties["Height"].SetValue(Double.NaN);
                    newGrid.Properties["Width"].SetValue(Double.NaN);
                    newGrid.Properties["HorizontalAlignment"].SetValue(SSW.HorizontalAlignment.Stretch);
                    newGrid.Properties["VerticalAlignment"].SetValue(SSW.VerticalAlignment.Stretch);
                    modelitem.Content.SetValue(newGrid);
                });

            GenericDefaultInitializer.AddDefault<SSWC.WrapPanel>(x => x.Height, 100d);
            GenericDefaultInitializer.AddDefault<SSWC.WrapPanel>(x => x.Width, 200d);

            GenericDefaultInitializer.AddDefault<SSWC.Expander>(x => x.Height, 100d);
            GenericDefaultInitializer.AddDefault<SSWC.Expander>(x => x.Width, 150d);

            GenericDefaultInitializer.SetAction(
                typeof(SSWC.HeaderedContentControl),
                (modelitem, context) =>
                {
                    ModelItem newGrid = ModelFactory.CreateItem(context, typeof(SSWC.Grid), CreateOptions.InitializeDefaults);
                    newGrid.Properties["Height"].SetValue(Double.NaN);
                    newGrid.Properties["Width"].SetValue(Double.NaN);
                    newGrid.Properties["HorizontalAlignment"].SetValue(SSW.HorizontalAlignment.Stretch);
                    newGrid.Properties["VerticalAlignment"].SetValue(SSW.VerticalAlignment.Stretch);
                    modelitem.Content.SetValue(newGrid);
                });

            GenericDefaultInitializer.AddDefault<SSWC.HeaderedContentControl>(x => x.Height, 16d);
            GenericDefaultInitializer.AddDefault<SSWC.HeaderedContentControl>(x => x.Width, 120d);

            builder.AddCallback(
                typeof(SSWC.HeaderedContentControl),
                b => b.AddCustomAttributes(new FeatureAttribute(typeof(GenericDefaultInitializer))));
            builder.AddCallback(
                typeof(SSWC.WrapPanel),
                b => b.AddCustomAttributes(new FeatureAttribute(typeof(GenericDefaultInitializer))));
            builder.AddCallback(
                typeof(SSWC.Expander),
                b => b.AddCustomAttributes(new FeatureAttribute(typeof(GenericDefaultInitializer))));

            builder.AddCallback(
                typeof(SSWCP.GlobalCalendarButton),
                b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(
                typeof(SSWCP.GlobalCalendarDayButton),
                b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(
                typeof(SSWCP.GlobalCalendarItem),
                b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

            builder.AddCallback(
               typeof(SSWC.TreeViewItemCheckBox),
               b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

            builder.AddCallback(
               typeof(SSWC.ListBoxDragDropTarget),
               b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(
               typeof(SSWC.TreeViewDragDropTarget),
               b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        }
    }
}
