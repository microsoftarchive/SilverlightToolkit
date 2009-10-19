// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Features;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This is Visual studio 4.0 specific Metadata and this is the function to add metadata without creating subclasses for different types"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is Visual studio 4.0 specific Metadata and this is the function to add metadata without creating subclasses")]
        protected override void AddAttributes(AttributeTableBuilder builder)
        {
            builder.AddCallback(
                typeof(SSWC.Calendar),
                b =>
                {
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWC.Calendar>(x => x.SelectedDate)));
                    b.AddCustomAttributes(new DefaultEventAttribute("SelectedDatesChanged"));
                });

            builder.AddCallback(
                typeof(SSWCP.CalendarButton),
                b =>
                {
                    b.AddCustomAttributes(new ToolboxBrowsableAttribute(false));
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWCP.CalendarButton>(x => x.Content)));
                });

            builder.AddCallback(
                typeof(SSWCP.CalendarDayButton),
                b =>
                {
                    b.AddCustomAttributes(new ToolboxBrowsableAttribute(false));
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWCP.CalendarDayButton>(x => x.Content)));
                });

            builder.AddCallback(
                typeof(SSWC.DatePicker),
                b =>
                {
                    b.AddCustomAttributes(new ToolboxBrowsableAttribute(false));
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWC.DatePicker>(x => x.SelectedDate)));
                    b.AddCustomAttributes(new DefaultEventAttribute("SelectedDateChanged"));
                });

            builder.AddCallback(
                typeof(SSWCP.DatePickerTextBox),
                b =>
                {
                    b.AddCustomAttributes(new ToolboxBrowsableAttribute(false));
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWCP.DatePickerTextBox>(x => x.Watermark)));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCP.DatePickerTextBox>(x => x.Watermark), 
                        new TypeConverterAttribute(typeof(StringConverter)));
                });

            builder.AddCallback(
                typeof(SSWC.TabControl),
                b =>
                {
                    b.AddCustomAttributes(new ToolboxBrowsableAttribute(false));
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWC.TabControl>(x => x.Items)));
                    b.AddCustomAttributes(new DefaultEventAttribute("SelectionChanged"));
                    b.AddCustomAttributes(new ComplexBindingPropertiesAttribute(String.Empty, String.Empty));
                });

            builder.AddCallback(
                typeof(SSWC.TabItem),
                b =>
                {
                    b.AddCustomAttributes(new ToolboxBrowsableAttribute(false));
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWC.TabItem>(x => x.Header)));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.TabItem>(x => x.Header), 
                        new TypeConverterAttribute(typeof(StringConverter)));
                });

            builder.AddCallback(
                typeof(SSWC.TreeView),
                b =>
                {
                    b.AddCustomAttributes(new ToolboxBrowsableAttribute(false));
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWC.TreeView>(x => x.Items)));
                    b.AddCustomAttributes(new DefaultEventAttribute("SelectedItemChanged"));
                    b.AddCustomAttributes(new ComplexBindingPropertiesAttribute(
                        Extensions.GetMemberName<SSWC.TreeView>(x => x.ItemsSource),
                        Extensions.GetMemberName<SSWC.TreeView>(x => x.DisplayMemberPath)));
                });

            builder.AddCallback(
                typeof(SSWC.TreeViewItem),
                b =>
                {
                    b.AddCustomAttributes(new ToolboxBrowsableAttribute(false));
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWC.TreeViewItem>(x => x.Header)));
                    b.AddCustomAttributes(new DefaultEventAttribute("Selected"));
                    b.AddCustomAttributes(new ComplexBindingPropertiesAttribute(
                        Extensions.GetMemberName<SSWC.TreeViewItem>(x => x.ItemsSource),
                        Extensions.GetMemberName<SSWC.TreeViewItem>(x => x.DisplayMemberPath)));
                });

            builder.AddCallback(
            typeof(SSWC.GridSplitter),
            b =>
            {
                b.AddCustomAttributes(new ToolboxBrowsableAttribute(false));
                b.AddCustomAttributes(new DefaultPropertyAttribute(
                    Extensions.GetMemberName<SSWC.GridSplitter>(x => x.ShowsPreview)));
                b.AddCustomAttributes(new DefaultEventAttribute("MouseLeftButtonUp"));
                b.AddCustomAttributes(new FeatureAttribute(typeof(GridSplitterDefaultInitializer)));
            });

            builder.AddCallback(
            typeof(SSWC.HeaderedItemsControl),
            b =>
            {
                b.AddCustomAttributes(new ToolboxBrowsableAttribute(false));
                b.AddCustomAttributes(new DefaultPropertyAttribute(
                    Extensions.GetMemberName<SSWC.HeaderedItemsControl>(x => x.Header)));
                b.AddCustomAttributes(new ComplexBindingPropertiesAttribute(
                    Extensions.GetMemberName<SSWC.HeaderedItemsControl>(x => x.ItemsSource),
                    Extensions.GetMemberName<SSWC.HeaderedItemsControl>(x => x.DisplayMemberPath)));
                b.AddCustomAttributes(
                    Extensions.GetMemberName<SSWC.HeaderedItemsControl>(x => x.Header), 
                    new TypeConverterAttribute(typeof(StringConverter)));
            });

            builder.AddCallback(
                typeof(SSWC.ChildWindow),
                b =>
                {
                    b.AddCustomAttributes(new ToolboxBrowsableAttribute(false));
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWC.ChildWindow>(x => x.Title)));
                    b.AddCustomAttributes(new DefaultEventAttribute("Closed"));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.ChildWindow>(x => x.Title), 
                        new TypeConverterAttribute(typeof(StringConverter)));
                });

            builder.AddCallback(
                typeof(SSWCP.CalendarItem),
                b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
        }
    }
}