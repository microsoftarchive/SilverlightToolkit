// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for Expander.
    /// </summary>
    internal class ExpanderMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for Expander.
        /// </summary>
        public ExpanderMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.Expander),
                b =>
                {
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWC.Expander>(x => x.Header)));
                    b.AddCustomAttributes(new DefaultEventAttribute("Expanded"));
                });
        }
    }
}