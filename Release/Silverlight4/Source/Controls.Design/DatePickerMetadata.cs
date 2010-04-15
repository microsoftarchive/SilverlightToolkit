// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Design
{
    /// <summary>
    /// To register design time metadata for DatePicker.
    /// </summary>
    internal class DatePickerMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for DatePicker.
        /// </summary>
        public DatePickerMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.DatePicker),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DatePicker>(x => x.DisplayDate),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DatePicker>(x => x.SelectedDate),
                        new CategoryAttribute(Properties.Resources.CommonProperties));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DatePicker>(x => x.SelectedDateFormat),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(
                        "BlackoutDates",
                        new CategoryAttribute(Properties.Resources.DatePicker));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DatePicker>(x => x.DisplayDateEnd),
                        new CategoryAttribute(Properties.Resources.DatePicker));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DatePicker>(x => x.DisplayDateStart),
                        new CategoryAttribute(Properties.Resources.DatePicker));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DatePicker>(x => x.FirstDayOfWeek),
                        new CategoryAttribute(Properties.Resources.DatePicker));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DatePicker>(x => x.IsDropDownOpen),
                        new CategoryAttribute(Properties.Resources.DatePicker));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DatePicker>(x => x.IsTodayHighlighted),
                        new CategoryAttribute(Properties.Resources.DatePicker));
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.DatePicker>(x => x.Text),
                        new CategoryAttribute(Properties.Resources.DatePicker));

                    b.AddCustomAttributes(new DefaultBindingPropertyAttribute("SelectedDate"));

#if MWD40
                    b.AddCustomAttributes(
                        "BlackoutDates",
                        PropertyValueEditor.CreateEditorAttribute(typeof(EmptyEditor)));
                    b.AddCustomAttributes(
                        "SelectedDates",
                        PropertyValueEditor.CreateEditorAttribute(typeof(EmptyEditor)));

                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Controls, true));

                    b.AddCustomAttributes(new FeatureAttribute(typeof(DatePickerIsDropDownOpenDesignModeValueProvider)));
                    b.AddCustomAttributes(new FeatureAttribute(typeof(DatePickerIsDropDownOpenDesignModeValueProvider.AdornerProxy)));
#endif
                });
        }
    }
}
