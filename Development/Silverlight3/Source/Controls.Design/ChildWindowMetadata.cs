// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for ChildWindow.
    /// </summary>
    public class ChildWindowMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for ChildWindow.
        /// </summary>
        public ChildWindowMetadata()
            : base()
        {
            AddCallback(
                typeof(ChildWindow),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<ChildWindow>(x => x.DialogResult), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<ChildWindow>(x => x.Title), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
