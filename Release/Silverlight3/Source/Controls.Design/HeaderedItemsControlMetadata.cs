// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for HeaderedItemsControl.
    /// </summary>
    internal class HeaderedItemsControlMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for HeaderedItemsControl.
        /// </summary>
        public HeaderedItemsControlMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.HeaderedItemsControl),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.HeaderedItemsControl>(x => x.Header),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.HeaderedItemsControl>(x => x.Items),
                        new NewItemTypesAttribute(typeof(SSWC.ContentPresenter)));

#if MWD40
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.HeaderedItemsControl>(x => x.Header),
                        new AlternateContentPropertyAttribute());
                    b.AddCustomAttributes(
                    Extensions.GetMemberName<SSWC.HeaderedItemsControl>(x => x.HeaderTemplate),
                    new DataContextValueSourceAttribute(
                        Extensions.GetMemberName<SSWC.HeaderedItemsControl>(x => x.Header),
                        false));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.HeaderedItemsControl>(x => x.ItemContainerStyle),
                        new DataContextValueSourceAttribute(
                        Extensions.GetMemberName<SSWC.HeaderedItemsControl>(x => x.ItemsSource),
                        true));
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.BasicControls, false));
#endif
                });
        }
    }
}
