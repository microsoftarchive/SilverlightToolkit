// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Layout.Design
{
    /// <summary>
    /// To register design time metadata for Accordion.
    /// </summary>
    internal class AccordionMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for Accordion.
        /// </summary>
        public AccordionMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.Accordion),
                b =>
                {
                    b.AddCustomAttributes(new DefaultPropertyAttribute(Extensions.GetMemberName<SSWC.Accordion>(x => x.Items)));
                    b.AddCustomAttributes(new DefaultEventAttribute("SelectionChanged"));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Accordion>(x => x.Items),
                        new NewItemTypesAttribute(typeof(SSWC.AccordionItem)));

                    b.AddCustomAttributes(new ComplexBindingPropertiesAttribute(
                        Extensions.GetMemberName<SSWC.Accordion>(x => x.ItemsSource),
                        Extensions.GetMemberName<SSWC.Accordion>(x => x.DisplayMemberPath)));
                });
        }
    }
}
