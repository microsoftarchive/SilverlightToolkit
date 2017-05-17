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
using SS = Silverlight::System;
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
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.GlobalCalendar>(x => x.DisplayDate),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.GlobalCalendar>(x => x.DisplayMode),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.GlobalCalendar>(x => x.SelectedDate),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        "SelectedDates",
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.GlobalCalendar>(x => x.SelectionMode),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.GlobalCalendar>(x => x.DisplayDateEnd),
                        new CategoryAttribute(Properties.Resources.Calendar));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.GlobalCalendar>(x => x.DisplayDateStart),
                        new CategoryAttribute(Properties.Resources.Calendar));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.GlobalCalendar>(x => x.FirstDayOfWeek),
                        new CategoryAttribute(Properties.Resources.Calendar));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.GlobalCalendar>(x => x.IsTodayHighlighted),
                        new CategoryAttribute(Properties.Resources.Calendar));

                    b.AddCustomAttributes(
                        "SelectedDates",
                        new NewItemTypesAttribute(typeof(DateTime)));

                    b.AddCustomAttributes(
                        "BlackoutDates",
                        new NewItemTypesAttribute(typeof(DateTime))); // Todo: use SL types instead

                    b.AddCustomAttributes(new DefaultBindingPropertyAttribute(
                        Extensions.GetMemberName<SSWC.GlobalCalendar>(x => x.SelectedDate)));

#if MWD40
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Controls, true));
#endif
                });
        }
    }
}
