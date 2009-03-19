// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

namespace System.Windows.Controls.Layout.Design
{
    /// <summary>
    /// To register design time metadata for AccordionItem.
    /// </summary>
    public class AccordionItemMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for AccordionItem.
        /// </summary>
        public AccordionItemMetadata()
            : base()
        {
            AddCallback(
                typeof(Accordion),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<AccordionItem>(x => x.Header),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<AccordionItem>(x => x.ExpandDirection),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<AccordionItem>(x => x.IsSelected),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
