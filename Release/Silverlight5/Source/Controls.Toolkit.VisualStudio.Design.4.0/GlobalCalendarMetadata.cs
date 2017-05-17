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
    /// To register design time metadata for GlobalCalendar.
    /// </summary>
    internal class GlobalCalendarMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for GlobalCalendar.
        /// </summary>
        public GlobalCalendarMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.GlobalCalendar),
                b =>
                {
                    b.AddCustomAttributes(new DefaultPropertyAttribute(
                        Extensions.GetMemberName<SSWC.GlobalCalendar>(x => x.SelectedDate)));
                    b.AddCustomAttributes(new DefaultEventAttribute("SelectedDatesChanged"));
                });
        }
    }
}