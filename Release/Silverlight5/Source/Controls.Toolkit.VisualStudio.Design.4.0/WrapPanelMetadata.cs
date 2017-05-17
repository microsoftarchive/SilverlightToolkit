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
    /// To register design time metadata for WrapPanel.
    /// </summary>
    internal class WrapPanelMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for WrapPanel.
        /// </summary>
        public WrapPanelMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.WrapPanel),
                b =>
                {
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWC.WrapPanel>(x => x.Orientation)));
                });
        }
    }
}