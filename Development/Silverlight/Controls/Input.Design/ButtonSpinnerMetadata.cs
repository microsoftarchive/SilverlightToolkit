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
    /// To register design time metadata for ButtonSpinner.
    /// </summary>
    internal class ButtonSpinnerMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for ButtonSpinner.
        /// </summary>
        public ButtonSpinnerMetadata()
            : base()
        {
            AddCallback(
                typeof(ButtonSpinner),
                b =>
                {
                    b.AddCustomAttributes(Extensions.GetMemberName<ButtonSpinner>(x => x.Content), new CategoryAttribute(Properties.Resources.CommonProperties));
                });
        }
    }
}
