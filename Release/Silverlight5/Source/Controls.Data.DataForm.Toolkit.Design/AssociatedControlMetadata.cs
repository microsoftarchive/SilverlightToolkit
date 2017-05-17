// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

namespace System.Windows.Controls.Data.DataForm.Design
{
    /// <summary>
    /// To register design time metadata for AssociatedControl.
    /// </summary>
    public class AssociatedControlMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for AssociatedControl.
        /// </summary>
        public AssociatedControlMetadata()
            : base()
        {
            AddCallback(
                typeof(AssociatedControl),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<AssociatedControl>(x => x.PropertyPath), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<AssociatedControl>(x => x.Target), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
