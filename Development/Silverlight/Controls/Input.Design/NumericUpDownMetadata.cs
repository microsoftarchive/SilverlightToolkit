// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using Microsoft.Windows.Controls.Design.Common;
using Microsoft.Windows.Design.Metadata;

namespace Microsoft.Windows.Controls.Input.Design
{
    /// <summary>
    /// To register design time metadata for NumericUpDown.
    /// </summary>
    internal class NumericUpDownMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for NumericUpDown.
        /// </summary>
        public NumericUpDownMetadata()
            : base()
        {
            AddCallback(
                typeof(NumericUpDown),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<NumericUpDown>(x => x.IsEditable), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<NumericUpDown>(x => x.Minimum), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<NumericUpDown>(x => x.Maximum), new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(Extensions.GetMemberName<NumericUpDown>(x => x.Value), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
