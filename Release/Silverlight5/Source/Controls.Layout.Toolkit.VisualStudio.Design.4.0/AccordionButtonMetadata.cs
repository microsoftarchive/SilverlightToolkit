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
using SSWCP = Silverlight::System.Windows.Controls.Primitives;

namespace System.Windows.Controls.Layout.Design
{
    /// <summary>
    /// To register design time metadata for SSWC.AccordionButton.
    /// </summary>
    internal class AccordionButtonMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for SSWC.AccordionButton.
        /// </summary>
        public AccordionButtonMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWCP.AccordionButton),
                b =>
                {
                    b.AddCustomAttributes(new DefaultPropertyAttribute(Extensions.GetMemberName<SSWCP.AccordionButton>(x => x.Content)));
                    b.AddCustomAttributes(new DefaultEventAttribute("Click"));
                });
        }
    }
}
