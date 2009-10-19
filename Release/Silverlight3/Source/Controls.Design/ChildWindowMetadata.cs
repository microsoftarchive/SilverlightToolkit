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
    /// To register design time metadata for ChildWindow.
    /// </summary>
    internal class ChildWindowMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for ChildWindow.
        /// </summary>
        public ChildWindowMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.ChildWindow),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.ChildWindow>(x => x.Title),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(new DefaultBindingPropertyAttribute("Title"));
                });
        }
    }
}
