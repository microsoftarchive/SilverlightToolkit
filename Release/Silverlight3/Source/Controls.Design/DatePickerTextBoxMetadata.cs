// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using SSWCP = Silverlight::System.Windows.Controls.Primitives;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for DatePickerTextBox.
    /// </summary>
    internal class DatePickerTextBoxMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DatePickerTextBox.
        /// </summary>
        public DatePickerTextBoxMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWCP.DatePickerTextBox),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWCP.DatePickerTextBox>(x => x.Watermark),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(new DefaultBindingPropertyAttribute("Text"));

#if MWD40
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.BasicControls, false));
#endif
                });
        }
    }
}
