// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Input.Design
{
    /// <summary>
    /// To register design time metadata for SSWC.NumericUpDown.
    /// </summary>
    internal class NumericUpDownMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for SSWC.NumericUpDown.
        /// </summary>
        public NumericUpDownMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.NumericUpDown),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.NumericUpDown>(x => x.IsEditable),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.NumericUpDown>(x => x.Minimum),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.NumericUpDown>(x => x.Maximum),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.NumericUpDown>(x => x.Value),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(new DefaultBindingPropertyAttribute(
                        Extensions.GetMemberName<SSWC.NumericUpDown>(x => x.Value)));

#if MWD40
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Controls, true));
#endif
                });
        }
    }
}