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
    /// To register design time metadata for DataForm.
    /// </summary>
    public class DataFormMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DataForm.
        /// </summary>
        public DataFormMetadata()
            : base()
        {
            AddCallback(
                typeof(System.Windows.Controls.DataForm), b =>
                    {
                        b.AddCustomAttributes(Extensions.GetMemberName<System.Windows.Controls.DataForm>(x => x.CommandButtonsVisibility), new CategoryAttribute(Properties.Resources.Buttons));
                        b.AddCustomAttributes(Extensions.GetMemberName<System.Windows.Controls.DataForm>(x => x.CancelButtonContent), new CategoryAttribute(Properties.Resources.Buttons));
                        b.AddCustomAttributes(Extensions.GetMemberName<System.Windows.Controls.DataForm>(x => x.CommitButtonContent), new CategoryAttribute(Properties.Resources.Buttons));
                        b.AddCustomAttributes(Extensions.GetMemberName<System.Windows.Controls.DataForm>(x => x.Header), new CategoryAttribute(Properties.Resources.CommonProperties));
                        b.AddCustomAttributes(Extensions.GetMemberName<System.Windows.Controls.DataForm>(x => x.FieldLabelPosition), new CategoryAttribute(Properties.Resources.Fields));
                        b.AddCustomAttributes("Fields", new CategoryAttribute(Properties.Resources.Fields));
                        b.AddCustomAttributes(Extensions.GetMemberName<System.Windows.Controls.DataForm>(x => x.CanUserAddItems), new CategoryAttribute(Properties.Resources.Items));
                        b.AddCustomAttributes(Extensions.GetMemberName<System.Windows.Controls.DataForm>(x => x.CanUserDeleteItems), new CategoryAttribute(Properties.Resources.Items));
                    });
        }
    }
}
