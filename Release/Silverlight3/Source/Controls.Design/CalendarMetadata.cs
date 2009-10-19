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

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for Calendar.
    /// </summary>
    internal class CalendarMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for Calendar.
        /// </summary>
        public CalendarMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.Calendar),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Calendar>(x => x.DisplayDate),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Calendar>(x => x.DisplayMode),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Calendar>(x => x.SelectedDate),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        "SelectedDates",
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Calendar>(x => x.SelectionMode),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Calendar>(x => x.DisplayDateEnd),
                        new CategoryAttribute(Properties.Resources.Calendar));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Calendar>(x => x.DisplayDateStart),
                        new CategoryAttribute(Properties.Resources.Calendar));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Calendar>(x => x.FirstDayOfWeek),
                        new CategoryAttribute(Properties.Resources.Calendar));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.Calendar>(x => x.IsTodayHighlighted),
                        new CategoryAttribute(Properties.Resources.Calendar));

                    b.AddCustomAttributes(
                        "SelectedDates",
                        new NewItemTypesAttribute(typeof(DateTime)));
                    b.AddCustomAttributes(
                        "BlackoutDates",
                        new NewItemTypesAttribute(typeof(DateTime)));

                    b.AddCustomAttributes(new DefaultBindingPropertyAttribute(
                        Extensions.GetMemberName<SSWC.Calendar>(x => x.SelectedDate)));
#if MWD40
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Controls, true));
#endif
                });
        }
    }
}
