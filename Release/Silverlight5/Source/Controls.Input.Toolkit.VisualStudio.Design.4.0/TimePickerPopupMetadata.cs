// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Input.Design
{
    /// <summary>
    /// Implements design time support for SSWC.TimePickerPopup.
    /// </summary>
    internal class TimePickerPopupMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for SSWC.TimePickerPopup.
        /// </summary>
        public TimePickerPopupMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.TimePickerPopup),
                b =>
                {
                    b.AddCustomAttributes(new DefaultPropertyAttribute(Extensions.GetMemberName<SSWC.TimePickerPopup>(x => x.Value)));
                    b.AddCustomAttributes(new DefaultEventAttribute("ValueChanged"));
                });
        }
    }
}